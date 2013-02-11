using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using fftwlib;
using ComplexArrayLib;

namespace WaveViewerWithFilering
{

    class WaveDataSet
    {

        public WaveDataSet(double[] wave_data, double delta_t, bool acc_data = false)
        {
            data = wave_data;
            dt = delta_t;
            is_acc = acc_data;
            init();
        }
        public WaveDataSet(Famos famos, int ch)
        {
            data = famos[ch];
            dt = famos.dt(ch);
            is_acc = famos.channel_info[ch].name.Contains("_Ya_") ||
                famos.channel_info[ch].name.Contains("_Za_");
            init();
        }

        private void init()
        {
            data_start_ = 0;
            filter = new FIRFilter();
            over_sample_ = 1;

            if (is_acc)
                integral = 0;
            else
                integral = -1;

            update_nfft();
        }

        private int integral;

        static readonly string[] CATEGORY = new string[] { "ACC", "VEL", "DIS", "NONE" };

        public string category
        {
            get
            {
                if (integral < 0)
                    return CATEGORY.Last();
                return CATEGORY[integral];
            }
            set
            {
                if (is_acc)
                {
                    var val = Array.FindIndex(CATEGORY, s => s == value);
                    if (val != integral)
                    {
                        integral = val;
                        update_wave();
                    }
                }
            }
        }

        // properties
        public double[] data { get; private set; }
        public bool is_acc { get; private set; }

        public double dt { get; private set; }

        private FIRFilter filter;

        private int num_disp_;
        public int num_disp
        {
            get { return num_disp_; }
            set
            {
                num_disp_ = value;
                filtered_ = new double[value];
                update_nfft();
            }
        }

        // number of FFT
        private int nfft_; public int nfft { get { return nfft_; } }

        private int data_start_;
        public int data_start
        {
            get { return data_start_; }
            set
            {
                data_start_ = value;
                setup_raw_wave();
            }
        }

        public bool is_valid
        {
            get
            {
                if (data == null) return false;
                if (filter == null) return false;
                if (num_disp_ == 0) return false;
                if (nfft_ == 0) return false;
                if (dt == 0.0) return false;

                return true;
            }
        }

        private double[] filtered_;
       
        public double[] filtered
        {
            get
            {
                if (is_valid)
                {
                    if (filtered_ == null)
                        filtered_ = new double[num_disp];
                    apply_filter();
                    return filtered_;
                }
                return new double [0];
            }
        }


        public int length { get { return data.Length; } }
        public int tap
        {
            get { return filter.tap; }
            set
            {
                filter.tap = value;
                update_nfft();
            }
        }
        public int lower { get { return filter.lower; } set { filter.lower = value; update_factors(); } }
        public int upper { get { return filter.upper; } set { filter.upper = value; update_factors(); } }
        public double gain { get { return filter.gain; } set { filter.gain = value; update_factors(); } }
        public double[] gains { get { return filter.gains; }}
        public double[] factor { get { return filter.factor; }}
        public double[] source { get; private set; }
        public double[] xvalues { get; private set; }

        public WindowFunction window { get { return filter.window; } }
        public double alpha { get { return filter.alpha; } set { filter.alpha = value; } }
        public FIRFilter.WindowType  window_type { get { return filter.window_type; } set { filter.window_type = value; } }

        //---Public Methods--------------------------------//



        public double[] wave_spectrum_amplitude_in_dB()
        {
            int sz = nfft / 2 + 1;
            return sp_wave.Power.Take(sz).Select(p => 20.0 * Math.Log10(Math.Max(p, 1e-100))).ToArray();
        }

        private double[] over_sampled_; public double[] over_sampled { get {  return over_sampled_; } }

        private int over_sample_;
        public int over_sample
        {
            get { return over_sample_; }
            set
            {
                switch (value)
                {
                    case 1:
                    case 2:
                    case 4:
                    case 8:
                    over_sample_ = value;
                    update_over_sampled();
                        break;
                    default:
                        throw new ArgumentException("over_sample can be 1, 2, 4 or 8"); 
                }
            }
        }

        //---Inernal Use--------------------------------//


        //--Private members
        private double[] raw_wave;    // raw wave. this is partial copy of data. 
        private double[] wave;        // wave data (it can be disp or vel wave)
        private double[] factors;     // expanded filter data
        private double[] ans;         // filtered wave (iDFT of sp_ans)
        private double[] over;
        private double base_line;
        private ComplexArray sp_raw_wave; // DFT of raw_wave;
        private ComplexArray sp_wave;     // DFT of wave
        private ComplexArray sp_factors;  // DFT of factors
        private ComplexArray sp_ans;      // sp_wave * sp_factors 
        private ComplexArray sp_over;
        private ComplexArray omega;

        //---Private methods

        private void update_nfft()
        {
            int val = 1024;
            while (val < num_disp + filter.tap * 4)
                val *= 2;

            nfft_ = val;
            double fs = 1.0 / dt;
            double df = fs / nfft;

            var half = Enumerable.Range(1, nfft / 2 - 1).Select(n => - 1.0/( n * df * 2 * Math.PI));
            var i_arr = Enumerable.Repeat(0.0, 1)
                .Concat(half)
                .Concat(Enumerable.Repeat(0.0, 1))
                .Concat(half.Reverse()).ToArray();
            omega = ComplexArray.imag(i_arr);
            if (omega.Length != nfft)
                throw new ApplicationException();

            wave = null; //  to skip apply_filter()
            update_factors();

            // next
            setup_raw_wave();
        }

        // copy wave data and FFT
        private void setup_raw_wave()
        {
            if (num_disp == 0)
                return;
            raw_wave = new double[nfft];

            int n_start = data_start_ - tap * 2;
            var base_wave = data.Skip(n_start).Take(num_disp);
            base_line = base_wave.Average();
            var range = base_wave.Max() - base_wave.Min();
            var delta = range / 1e6;
            for (int i = 0; i < 20; i++)
            {
                take_raw_wave_with_baseline();

                var error = raw_wave.Average();
                if (Math.Abs(error) < delta)
                    break;
                base_line += error / 2; //update
            }

            // next
            update_wave();
        }

        private void update_wave()
        {
            if (raw_wave == null)
                return;

            if (integral < 1)
            {
                // ACC and others are not need integration.
                var s = ComplexArray.real(raw_wave);
                var w = s.fft();
                sp_raw_wave = w / nfft;
                wave = raw_wave.Select(d => d + base_line).ToArray();
                var ww = ComplexArray.real(wave);
                var ss = ww.fft();
                sp_wave = ss / nfft;
            }
            else
            {
                // VEL and DISP need integration.
                sp_raw_wave = ComplexArray.real(raw_wave).fft()/ nfft;
                sp_wave = new ComplexArray(sp_raw_wave);
                // Integrate in frequency domain.
                for (int i = 0; i < integral; i++)
                {
                    sp_wave *= omega;
                }
                //sp_wave[0] *= 0.0;
                //for (int k = 0; k < integral; k++)
                //{
                //    for (int i = 1; i < nfft / 2; i++)
                //    {
                //        sp_wave[i] *= omega[i];
                //        sp_wave[nfft - i] = sp_wave[i].conj;
                //    }
                //}
                wave = sp_wave.ifft().real();
            }
            source = wave.Take(num_disp).ToArray();
            xvalues = Enumerable.Range(0, num_disp).Select(i => (i + data_start_) * dt).ToArray();

            if (factors == null)
                return;

            apply_filter();
        }


        private void update_factors()
        {

            if (filter.design()) // refresh filter if needed
            {
                factors = new double[nfft];

                factors[0] = filter.factor[0];
                for (int i = 1; i <= filter.tap; i++)
                {
                    factors[nfft - i] = factors[i] = filter.factor[i];
                }
                sp_factors = ComplexArray.real(factors).fft();
                if (wave == null)
                    return;

                apply_filter();
            }
        }

        private void apply_filter()
        {
            if (wave == null || factors == null)
                return;

            // convolution in Frequency domain
            sp_ans = sp_wave * sp_factors;

            // inverse fft
            ans = sp_ans.ifft().real();

            // copy result to filtered_
            filtered_ = ans.Take(num_disp).ToArray();

            // Next is oversamle
            update_over_sampled();
        }

        private void update_over_sampled()
        {
            if (ans == null)
                return;
            // noneed to upsampling;
            if (over_sample == 1)
            {
                over_sampled_ = filtered_.ToArray();
                over = ans.ToArray();
                sp_over = new ComplexArray(sp_ans);
                return;
            }
            int size = sp_ans.Length * over_sample;

            sp_over = new ComplexArray(size);

            sp_over[0] = sp_ans[0];
            for (int i = 1; i <= nfft/2; i++)
            {
                sp_over[i] = sp_ans[i];
                sp_over[size - i] = sp_ans[i].Conj;
            }

            over = sp_over.ifft().real();
            over_sampled_ = over.Take(num_disp * over_sample).ToArray();

            // no next
        }

        private void take_raw_wave_with_baseline()
        {
            int tap = filter.tap;
            int n_start = data_start - tap * 2;
            int n_end = data_start + num_disp;

            Array.Clear(raw_wave, 0, nfft); 

            HannWindow hann = new HannWindow(tap);

            // copy pre_data with filter
            for (int i = 0; i < tap; i++)
            {
                raw_wave[nfft - 2 * tap + i] = (data[Math.Abs(i + n_start)] - base_line) * hann[tap-i];
                raw_wave[nfft - 1 * tap + i] = data[Math.Abs(i + tap + n_start)] - base_line;
            }
            // copy main_data
            int last_index = data.Length - 1;
            for (int i = 0; i < num_disp; i++)
            {
                int k = last_index - Math.Abs(last_index - (data_start + i));
                raw_wave[i] = data[k] - base_line;
            }
            // copy postdata
            for (int i = 0; i < tap; i++)
            {
                int k = last_index - Math.Abs(last_index - (n_end + i));
                raw_wave[num_disp + i] = data[k] - base_line;
            }
            // copy with filter
            for (int i = 0; i < tap; i++)
            {
                int j = i + 3 * tap + num_disp;
                int k = last_index - Math.Abs(last_index - (n_end + tap + i));
                raw_wave[num_disp + tap + i] = (data[k] - base_line) * hann[i];
            }

        }



        // for debug
        public double[][] debug_waves()
        {
            int zeros = Math.Max(2 * tap - data_start,0);
            return new double[][]{
                wave,
                ans,
                over,
                raw_wave,
                Enumerable.Repeat(0.0,zeros).Concat(data.Skip(data_start - 2*tap).Take(num_disp + 4*tap-zeros)).ToArray()
            };
        }


        public double[][] debug_spectrums()
        {
            return new double[][]{
                sp_wave.abs(),
                sp_ans.abs(),
                sp_over.abs(),
                sp_raw_wave.abs(),
            };
        }
        
    }
}
