using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using fftwlib;

namespace WaveViewerWithFilering
{
    class WaveData
    {

        public WaveData(double[] wave_data, double delta_t, bool acc_data = false)
        {
            data = wave_data;
            dt = delta_t;
            is_acc = acc_data;
            init();
        }
        public WaveData(Famos famos, int ch)
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

            invalidate_factors();
            invalidate_waves();
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
                        invalidate_waves();
                    }
                }
            }
        }



        // properties
        private double[] data_;
        public double[] data
        {
            get { return data_; }
            set
            {
                data_ = value;
                invalidate_waves();
            }
        }

        public bool is_acc { get; private set; }

        private double dt_;
        public double dt
        {
            get { return dt_; }
            set
            {
                dt_ = value;
                invalidate_waves();
                filter_dirty = true;
            }
        }

        // dirty flags
        private bool filter_dirty;
        private bool wave_dirty;
        private bool ans_dirty;

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
                invalidate_waves();
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
                invalidate_waves();
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
                invalidate_waves();
                invalidate_factors();
            }
        }
        public int lower { get { return filter.lower; } set { filter.lower = value; invalidate_factors(); } }
        public int upper { get { return filter.upper; } set { filter.upper = value; invalidate_factors(); } }
        public double gain { get { return filter.gain; } set { filter.gain = value; invalidate_factors(); } }
        public double[] gains { get { return filter.gains; }}
        public double[] factor { get { return filter.factor; }}
        public WindowFunction window { get { return filter.window; } }
        public double alpha { get { return filter.alpha; } set { filter.alpha = value; } }
        public FIRFilter.WindowType  window_type { get { return filter.window_type; } set { filter.window_type = value; } }

        //---Public Methods--------------------------------//

        private double[] source_;
        public double[] source
        {
            get
            {
                update_source();
                return source_;
            }
        }


        private double[] xvalues_;
        public double[] xvalues
        {
            get
            {
                update_xvalues();
                return xvalues_;
            }
        }

        public double[] wave_spectrum_amplitude_in_dB()
        {
            int sz = nfft / 2 + 1;
            var res = new double[nfft / 2 + 1];
            for (int i = 0; i < sz; i++)
            {
                res[i] = sp_wave_in_dB(i);
            }

            return res;
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
                    alloc_over();
                    update_over_sampled();
                        break;
                    default:
                        throw new ArgumentException("over_sample can be 1, 2,4 or 8"); 
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
        private double[] sp_raw_wave; // DFT of raw_wave;
        private double[] sp_wave;     // DFT of wave
        private double[] sp_factors;  // DFT of factors
        private double[] sp_ans;      // sp_wave * sp_factors 
        private double[] sp_over;

        // Pointers for FFTW
        IntPtr pfin, pfout, plan_f;  // forward
        IntPtr prin, prout, plan_r;  // Reverse (inverse)

        //---Private methods

        private void invalidate_waves()
        {
            source_ = null;
            xvalues_ = null;
            wave_dirty = true;
            ans_dirty = true;
        }

        private void invalidate_factors()
        {
            filter_dirty = true;
            ans_dirty = true;
        }

        private void update_source()
        {
            if (source_ == null)
            {
                update_sp_wave();
                source_ = new double[num_disp];
                for (int i = 0; i < num_disp; i++)
                {
                    source_[i] = wave[2 * (i + 2 * tap)];
                }
            }
        }

        private void update_xvalues()
        {
            if (xvalues_  == null)
            {
                xvalues_ = new double[num_disp];
                for (int i = 0; i < num_disp; i++)
                {
                    xvalues_[i] = (i + data_start) * dt;
                }
            }
        }

        private void update_nfft()
        {
            int val = 1024;
            while (val < num_disp + filter.tap * 4)
                val *= 2;
            if (val != nfft)
            {
                nfft_ = val;
                raw_wave = new double[nfft * 2];
                sp_raw_wave = new double[nfft * 2];
                wave = new double[nfft * 2];
                factors = new double[nfft * 2];
                sp_factors = new double[nfft * 2];
                sp_wave = new double[nfft * 2];
                sp_ans = new double[nfft * 2];
                ans = new double[nfft * 2];
                alloc_over();
                fftw_setup();
            }
        }

        private void alloc_over()
        {
            over = new double[nfft * 2 * over_sample];
            sp_over = new double[nfft * 2 * over_sample];
        }

        private void fftw_setup()
        {
            if (pfin != null)
                fftw_free();
            pfin = fftw.malloc(sizeof(double) * nfft * 2);
            prin = fftw.malloc(sizeof(double) * nfft * 2);
            pfout = fftw.malloc(sizeof(double) * nfft * 2);
            prout = fftw.malloc(sizeof(double) * nfft * 2);
            plan_f = fftw.dft_1d(nfft, pfin, pfout, fftw_direction.Forward, fftw_flags.Estimate);
            plan_r = fftw.dft_1d(nfft, prin, prout, fftw_direction.Backward, fftw_flags.Estimate);
            filter_dirty = true;
        }

        private void fftw_free()
        {
            fftw.free(pfin);
            fftw.free(pfout);
            fftw.free(prin);
            fftw.free(prout);
            fftw.destroy_plan(plan_f);
            fftw.destroy_plan(plan_r);
        }

        private double sp_wave_in_dB(int i)
        {
            if (sp_wave == null)
            {
                update_nfft();
                update_sp_wave();
            }

            double real = sp_wave[i * 2];
            double imag = sp_wave[i * 2 + 1];
            double amp = Math.Max(real * real + imag * imag, 1e-40); // 1e-10: avoid -Inf 
            amp /= nfft;  //  normalize
            double db = 20.0 * Math.Log10(amp); // convert to dB
            return db;
        }

        private void update_sp_wave()
        {
            if (wave_dirty)
            {
                setup_raw_wave();
                Marshal.Copy(raw_wave, 0, pfin, nfft * 2);
                fftw.execute(plan_f);
                Marshal.Copy(pfout, sp_raw_wave, 0, nfft * 2);


                if (integral < 1)
                {
                    // ACC and others are not need integration.
                    //Array.Copy(sp_raw_wave, sp_wave, nfft * 2);
                    //sp_wave = sp_raw_wave.Select(e => e / nfft);
                    for (int i = 0; i < nfft*2; i++)
                    {
                        sp_wave[i] = sp_raw_wave[i] / nfft_;
                    }
                    Array.Copy(raw_wave, wave, nfft * 2);
                }
                else
                {
                    // VEL and DISP need integration.

                    integrate_wave();
                }
                wave_dirty  = false;
                ans_dirty = true;
            }
        }



        private void integrate_wave()
        {
            // Integrate in frequency domain.
            double c0, d0;
            switch (integral)
            {
                case 1: // vel
                    c0 = 0.0;
                    d0 = 1.0;
                    break;
                case 2: // dis
                    c0 = -1.0;
                    d0 = 0.0;
                    break;
                default:
                    c0 = 1.0;
                    d0 = 0.0;
                    break;
            }
            double fs = 1.0 / dt;
            double df = fs / nfft;
            sp_wave[0] = 0.0; // eliminating DC compornent.  (avoiding divide by zero)
            sp_wave[1] = 0.0;
            double[] omega = new double[nfft];
            omega[0] = 0.0;
            for (int i = 1; i <= nfft / 2; i++)
            {
                omega[nfft - i] = omega[i] = Math.Pow(df * i * 2 * Math.PI, integral);
            }

            for (int i = 1; i < nfft; i++)
            {
                double a = sp_raw_wave[2 * i];
                double b = sp_raw_wave[2 * i + 1];
                double c = c0 * omega[i];
                double d = d0 * omega[i];
                double div = 1.0 / (c * c + d * d) / nfft;
                sp_wave[i * 2] = (a * c - b * d) * div;
                sp_wave[i * 2 + 1] = (a * d - b * c) * div;
            }
            Marshal.Copy(sp_wave, 0, prin, nfft * 2);
            fftw.execute(plan_r);
            Marshal.Copy(prout, wave, 0, nfft * 2);
        }
        

        // copy wave data and FFT
        private void setup_raw_wave()
        {
            int tap = filter.tap;
            int n_start = data_start_ - tap * 2;

            update_nfft();

            // obtain average
            double base_line = data.Skip(n_start).Take(num_disp).Average();

            // clear data with baseline
            for (int i = 0; i < nfft; i++)
            {
                raw_wave[i*2] = base_line;
                raw_wave[i * 2 + 1] = 0.0;
            }

            HannWindow hann = new HannWindow(tap);


            // copy with filter
            for (int i = 0; i < tap; i++)
            {
                int k = n_start + i;
                double amp = hann[tap - i -1];
                double value = data[Math.Max(k, 0)];
                raw_wave[2 * i] = (value - base_line ) * amp + base_line;
            }
            // copy pre_data
            for (int i = tap; i < tap * 2; i++)
            {
                int k = n_start + i;
                raw_wave[2 * i] = data[Math.Max(k,0)];
            }
            // copy main_data
            for (int i = 0; i < num_disp; i++)
            {
                int j = i + 2 * tap;
                int k = n_start + j;
                raw_wave[2 * j] = data[k];
            }
            // copy postdata
            int last_index = data.Length - 1;
            for (int i = 0; i < tap; i++)
            {
                int j = i + 2 * tap + num_disp;
                int k = Math.Min(data_start + num_disp + i, last_index);
                raw_wave[2 * j] = data[k];
            }
            // copy with filter
            for (int i = 0; i < tap; i++)
            {
                int j = i + 3 * tap + num_disp;
                int k = Math.Min(data_start + num_disp + tap + i, last_index);
                double amp = hann[i];
                raw_wave[2 * j] = (data[k] - base_line) * amp + base_line;
            }
            // rest data are base_line.
        }

        private void update_sp_factors()
        {
            if (!filter_dirty &&  ! filter.is_dirty)
                return;

            update_factors();

            Marshal.Copy(factors, 0, pfin, nfft * 2);
            fftw.execute(plan_f);
            Marshal.Copy(pfout, sp_factors, 0, nfft * 2);
            filter_dirty = false;
            ans_dirty = true;
        }

        private void update_factors()
        {

            filter.design(); // refresh filter if needed
            factors = new double[2 * nfft];

            for (int i = 0; i < filter.size; i++)
            {
                int k = Math.Abs(i - filter.tap);
                factors[i * 2] = filter.factor[k];
            }
        }

        private void apply_filter()
        {
            update_sp_wave();
            update_sp_factors();
            if (ans_dirty)
            {
                for (int i = 0; i < nfft; i++)
                {
                    // multiply complex value
                    double a = sp_wave[i * 2];
                    double b = sp_wave[i * 2 + 1];
                    double c = sp_factors[i * 2];
                    double d = sp_factors[i * 2 + 1];
                    sp_ans[i * 2] = a * c - b * d;
                    sp_ans[i * 2 + 1] = a * d + b * c;
                }

                Marshal.Copy(sp_ans, 0, prin, nfft * 2);
                fftw.execute(plan_r);
                Marshal.Copy(prout, ans, 0, nfft * 2);

                // copy result to filtered_
                int offset = filter.tap * 3;  // 2*tap is pre_data, 1*tap is delay 

                for (int i = 0; i < num_disp; i++)
                {
                    int k = i + offset;
                    filtered_[i] = ans[2 * k]; // sp_wave was normalized already.
                }

                // clear dirty flag
                ans_dirty = false;
            }
        }

        private void update_over_sampled()
        {
            apply_filter();  // update required.

            over_sampled_ = new double[num_disp * over_sample];

            // noneed to upsampling;
            if (over_sample == 1)
            {
                Array.Copy(filtered_, over_sampled_, num_disp);
                Array.Copy(ans, over, nfft*2);
                Array.Copy(sp_ans, sp_over, nfft*2);
                return;
            }

            IntPtr pin = IntPtr.Zero, pout = IntPtr.Zero, plan = IntPtr.Zero;
            int size = nfft * over_sample;


            sp_over[0] = sp_ans[0];
            sp_over[1] = sp_ans[1];
            for (int i = 2; i < nfft; i++)
            {
                sp_over[i] = sp_ans[i];
                sp_over[2*size -nfft + i] = sp_ans[nfft + i];
            }

            try
            {
                pin = fftw.malloc(sizeof(double) * size * 2);
                pout = fftw.malloc(sizeof(double) * size * 2);
                plan = fftw.dft_1d(size, pin, pout, fftw_direction.Backward, fftw_flags.Estimate);
                Marshal.Copy(sp_over, 0, pin, size * 2);
                fftw.execute(plan);
                Marshal.Copy(pout, over, 0, size * 2);
                int offset = 3 * tap * over_sample;
                for (int i = 0; i < num_disp * over_sample; i++)
                {
                    over_sampled_[i] = over[2 * (i + offset)];
                }
            }
            finally
            {
                fftw.free(pin);
                fftw.free(pout);
                fftw.destroy_plan(plan);
            }
        }

        // for debug
        public double[][] debug_waves()
        {
            return new double[][]{
                wave,
                ans,
                over,
                raw_wave,
            };
        }
        public double[][] debug_spectrums()
        {
            return new double[][]{
                sp_wave,
                sp_ans,
                sp_over,
                sp_raw_wave,
            };
        }
        
    }
}
