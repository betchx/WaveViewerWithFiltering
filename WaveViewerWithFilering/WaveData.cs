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

        public WaveDataSet(double[] wave, double delta_t, bool acc_data = false)
        {
            data = wave;
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
            raw_wave_start = -1;
            if (is_acc)
                integral = 0;
            else
                integral = -1;
            nfft_ = 0;
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
        public IEnumerable<double> Data { get { return data; } }
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
                update_nfft();
                setup_raw_wave();
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

        private IEnumerable<double> filtered_;

        public IEnumerable<double> filtered
        {
            get
            {
                if (is_valid)
                {
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
                hann = new HannWindow(value);
                update_nfft();
            }
        }
        public int lower { get { return filter.lower; } set { filter.lower = value; update_factors(); } }
        public int upper { get { return filter.upper; } set { filter.upper = value; update_factors(); } }
        public double gain { get { return filter.gain; } set { filter.gain = value; update_factors(); } }
        public IEnumerable<double> gains { get { return filter.gains; } }
        public double[] factor { get { return filter.factor; }}
        public IEnumerable<double> source { get; private set; }
        public IEnumerable<double> xvalues { get; private set; }
        public WindowFunction window { get { return filter.window; } }
        public double alpha { get { return filter.alpha; } set { filter.alpha = value; } }
        public FIRFilter.WindowType  window_type { get { return filter.window_type; } set { filter.window_type = value; } }
        private IEnumerable<double> over_sampled_;
        public IEnumerable<double> over_sampled { get { return over_sampled_.ToList(); } }

        public uint over_id { get { return over.wave_id; } }
        public uint filtered_id { get { return ans.wave_id; } }
        public uint factor_id { get { return factors.wave_id; } }
        public uint gain_id { get { return factors.sp_id; } }
        public uint source_id { get { return wave.wave_id; } }


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
        public double[] freqs { get; private set; }


        //---Public Methods--------------------------------//

        public IEnumerable<double> wave_spectrum_amplitude_in_dB()
        {
            return wave.dB;
        }

        //---Inernal Use--------------------------------//


        //--Private members
        private WaveData raw_wave;    // raw wave. this is partial copy of data. 
        private WaveData wave;        // wave data (it can be disp or vel wave)
        private WaveData factors;     // expanded filter data
        private WaveData ans;         // filtered wave (iDFT of sp_ans)
        private WaveData over;
        private double base_line;
        private ComplexArray omega;
        private HannWindow hann;
        private int raw_wave_start;
        private int current_over_sample;

        //---Private methods

        /// <summary>
        /// NFFT： Number of data for FFT (= 2^n > 4tap + num_disp, >1024)
        /// </summary>
        private void update_nfft()
        {
            int val = 1024;
            while (val < num_disp + filter.tap * 4)
                val *= 2;

            if (nfft != val)
            {
                // update
                nfft_ = val;

                raw_wave = new WaveData(nfft_);
                wave = new WaveData(nfft_);
                ans = new WaveData(nfft_);
                factors = new WaveData(nfft_);
                over = new WaveData(nfft * over_sample_);

                // update Omega
                double fs = 1.0 / dt;
                double df = fs / nfft;
                omega = new ComplexArray(nfft/2+1);

                double df0 = -1.0 / (df*2*Math.PI);
                double v;
                for (int i = 1; i < nfft/2; i++)
                {
                    v = df0 / i;
                    omega[i].Real = v;
                }

                // update freqs
                freqs = Enumerable.Range(0, nfft / 2).Select(i => df * i).ToArray();
            }
        }

        // copy wave data and FFT
        private bool setup_raw_wave()
        {
            if (num_disp == 0)
                return false;

            if (raw_wave_start == data_start)
                return false;

            int n_start = data_start_ - tap * 2;
            var base_wave = data.Skip(n_start).Take(num_disp);
            base_line = base_wave.Average();
            var range = base_wave.Max() - base_wave.Min();
            var delta = range / 1e6;
            for (int i = 0; i < 20; i++)
            {
                take_raw_wave_with_baseline();

                var error = raw_wave.Wave.Average();
                if (Math.Abs(error) < delta)
                    break;
                base_line += error / 2; //update
            }
            raw_wave_start = data_start;
            return true;
        }

        private bool update_wave()
        {
            if (raw_wave == null)
                return false;

            if (setup_raw_wave())
            {

                if (integral < 1)
                {
                    // ACC and others are not need integration.
                    wave.Spectrum = raw_wave.Spectrum;
                }
                else
                {
                    // VEL and DISP need integration.

                    // Integrate in frequency domain.
                    for (int i = 0; i < integral; i++)
                    {
                        wave.Spectrum = raw_wave.Spectrum.Zip(omega, (a, b) => a / b);
                    }
                }
                source = wave.Wave.Take(num_disp);
                xvalues = Enumerable.Range(0, num_disp).Select(i => (i + data_start_) * dt);

                return true;
            }
            return false;
        }


        private bool update_factors()
        {

            if (filter.design()) // refresh filter if needed
            {
                factors[0] = filter.factor[0];
                for (int i = 1; i <= filter.tap; i++)
                {
                    factors[nfft - i] = factors[i] = filter.factor[i];
                }
                return true;
            }
            return false;
        }

        private bool apply_filter()
        {
            if (update_wave() || update_factors())
            {
                // convolution in Frequency domain
                ans = wave * factors;

                // copy result to filtered_
                filtered_ = ans.Wave.Take(num_disp);

                return true;
            }
            return false;
        }

        private void update_over_sampled()
        {
            if (ans == null)
                return;

            if (apply_filter() || current_over_sample != over_sample)
            {
                // noneed to upsampling;
                if (over_sample == 1)
                {
                    over.Wave = ans.Wave;
                }
                else
                {
                    over.clear_sp();
                    over.Spectrum = ans.Spectrum;
                    over_sampled_ = over.Wave.Take(num_disp * over_sample);
                }
                current_over_sample = over_sample;
            }
        }

        private void take_raw_wave_with_baseline()
        {
            int tap = filter.tap;
            int n_start = data_start - tap * 2;
            int n_end = data_start + num_disp;

            raw_wave.clear_wave();

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
        public IEnumerable<double>[] debug_waves()
        {
            int zeros = Math.Max(2 * tap - data_start,0);
            return new IEnumerable<double>[]{
                wave.Wave,
                ans.Wave,
                over.Wave,
                raw_wave.Wave,
                Enumerable.Repeat(0.0,zeros).Concat(data.Skip(data_start - 2*tap).Take(num_disp + 4*tap-zeros))
            };
        }


        public IEnumerable<double>[] debug_spectrums()
        {
            return new IEnumerable<double>[]{
                wave.Abs,
                ans.Abs,
                over.Abs,
                raw_wave.Abs,
            };
        }


    }
}
