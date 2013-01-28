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
        // Construct
        //public WaveData()
        //{
        //    data_start_ = 0;
        //    invalidate_factors();
        //    invalidate_waves();
        //}
        public WaveData(double[] wave_data, double delta_t)
        {
            data = wave_data;
            dt = delta_t;
            init();
        }
        public WaveData(Famos famos, int ch)
        {
            data = famos[ch];
            dt = famos.dt(ch);
            init();
        }

        private void init()
        {
            data_start_ = 0;
            filter = new FIRFilter();

            invalidate_factors();
            invalidate_waves();
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


        //---Inernal Use--------------------------------//


        //--Private members
        private double[] wave;        // wave data
        private double[] factors;     // expanded filter data
        private double[] ans;         // filtered wave (iDFT of sp_ans)
        private double[] sp_wave;     // DFT of wave
        private double[] sp_factors;  // DFT of factors
        private double[] sp_ans;      // sp_wave * sp_factors 

        // Pointers for FFTW
        IntPtr pwin, pwout, plan_w;  // wave
        IntPtr pfin, pfout, plan_f;  // filter
        IntPtr prin, prout, plan_r;  // result

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
                source_ = new double[num_disp];
                for (int i = 0; i < num_disp; i++)
                {
                    source_[i] = data[i + data_start];
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
                wave = new double[nfft * 2];
                factors = new double[nfft * 2];
                sp_factors = new double[nfft * 2];
                sp_wave = new double[nfft * 2];
                sp_ans = new double[nfft * 2];
                ans = new double[nfft * 2];
                fftw_setup();
            }
        }

        private void fftw_setup()
        {
            if (pwin != null)
                fftw_free();
            pwin = fftw.malloc(sizeof(double) * nfft * 2);
            pfin = fftw.malloc(sizeof(double) * nfft * 2);
            prin = fftw.malloc(sizeof(double) * nfft * 2);
            pwout = fftw.malloc(sizeof(double) * nfft * 2);
            pfout = fftw.malloc(sizeof(double) * nfft * 2);
            prout = fftw.malloc(sizeof(double) * nfft * 2);
            plan_w = fftw.dft_1d(nfft, pwin, pwout, fftw_direction.Forward, fftw_flags.Estimate);
            plan_f = fftw.dft_1d(nfft, pfin, pfout, fftw_direction.Forward, fftw_flags.Estimate);
            plan_r = fftw.dft_1d(nfft, prin, prout, fftw_direction.Backward, fftw_flags.Estimate);
            filter_dirty = true;
        }

        private void fftw_free()
        {
            fftw.free(pwin);
            fftw.free(pwout);
            fftw.free(pfin);
            fftw.free(pfout);
            fftw.free(prin);
            fftw.free(prout);
            fftw.destroy_plan(plan_w);
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
            double amp = Math.Max(real * real + imag * imag, 1e-10); // 1e-10: avoid -Inf 
            amp /= nfft;  //  normalize
            double db = 20.0 * Math.Log10(amp); // convert to dB
            return db;
        }

        private void update_sp_wave()
        {
            if (wave_dirty)
            {
                setup_wave();
                Marshal.Copy(wave, 0, pwin, nfft * 2);
                fftw.execute(plan_w);
                Marshal.Copy(pwout, sp_wave, 0, nfft * 2);
                wave_dirty  = false;
                ans_dirty = true;
            }
        }
        

        // copy wave data and FFT
        private void setup_wave()
        {
            int tap = filter.tap;
            int pos = data_start;
            int n_start = pos - tap * 2;

            // Zero clear
            for (int i = 0; i < nfft * 2; i++)
            {
                wave[i] = 0.0;
            }

            HannWindow hann = new HannWindow(tap);

            // copy with filter
            for (int i = 0; i < tap; i++)
            {
                int k = n_start + i;
                double amp = hann[tap - k];
                double value = (k < 0) ? 0.0 : data[k];
                wave[2 * i] = value * amp;
            }
            // copy pre_data
            for (int i = tap; i < tap * 2; i++)
            {
                int k = n_start + i;
                wave[2 * i] = (k < 0) ? 0.0 : data[k];
            }
            // copy main_data and post_data
            for (int i = 0; i < num_disp + tap; i++)
            {
                int j = i + 2 * tap;
                int k = n_start + j;
                wave[2 * j] = data[k];
            }
            // copy with filter
            for (int i = 0; i < tap; i++)
            {
                int j = i + 3 * tap + num_disp;
                int k = n_start + j;
                double amp = hann[i + 1];
                wave[2 * j] = data[k] * amp;
            }
            // rest data are zero
        }

        private void update_sp_factors()
        {
            if (!filter_dirty)
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
                    filtered_[i] = ans[2 * k];
                }

                // clear dirty flag
                ans_dirty = false;
            }
        }



    }
}
