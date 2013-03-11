using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using fftwlib;
using System.Runtime.InteropServices;  // for IntPtr

namespace WaveViewerWithFilering
{
    public class FIRFilter
    {
        private bool dirty;

        public bool is_dirty { get { return dirty; } }


        public FIRFilter()
        {
            window_type_ = WindowType.None;
        }

        public enum WindowType
        {
            None,
            Han,
            Hamming,
            Kaiser,
            Blackman,
            Rectangle,
        }
        private WindowFunction window_; public WindowFunction window { get { return window_; } }

        private WindowType window_type_; public WindowType window_type { get { return window_type_; }
            set {
                window_type_ = value;
                setup_window();
            }
        }

        private double alpha_;
        public double alpha
        {
            get { return alpha_; }
            set
            {
                alpha_ = value;
                window_type_ = WindowType.Kaiser;
                setup_window();
            }
        }

        private void setup_window()
        {
            if (tap == 0)
                return;
            switch (window_type)
            {
                case WindowType.None:
                case WindowType.Rectangle:
                    window_ = new RectangleWindow(tap);
                    break;
                case WindowType.Kaiser :
                    window_ = new KaiserWindow(tap, alpha);
                    break;
                case WindowType.Han:
                    window_ = new HannWindow(tap);
                    break;
                case WindowType.Hamming:
                    window_ = new HammingWindow(tap);
                    break;
                case WindowType.Blackman:
                    window_ = new BlackmanWindow(tap);
                    break;
            }
            dirty = true;
        }

        // attributes
        private int tap_;
        public int tap
        {
            get { return tap_; }
            set { 
                tap_ = value; 
                size = value * 2;

                amp = 1.0 / size;

                buf = new double[size * 2];
                factor = new double[size];
                gains = new double[size];
 
                Free();
                pin = fftw.malloc(sizeof(double) * 2 * size);
                pout = fftw.malloc(sizeof(double) * 2 * size);
                plan_b = fftw.dft_1d(size, pin, pout, fftw_direction.Backward, fftw_flags.Estimate);
                psin = fftw.malloc(sizeof(double) * 2 * size);
                psout = fftw.malloc(sizeof(double) * 2 * size);
                plan_f = fftw.dft_1d(size, psin, psout, fftw_direction.Forward, fftw_flags.Estimate);
                setup_window();
            }
        }
        private int lower_; public int lower { get { return lower_; } set { lower_ = value; dirty = true; } }
        private int upper_; public int upper { get { return upper_; } set { upper_ = value; dirty = true; } }
        private double gain_; public double gain { get { return gain_; } set { gain_ = value; dirty = true; } }
        
        // getter only
        public int size { get; private set; }
        public double[] factor { get; private set; }
        public double[] gains { get; private set; }

        private double[] buf;
        private IntPtr pin, pout, plan_b, plan_f, psin, psout;
        private double amp;

        public bool design()
        {
            if(tap == 0)
                return false;
            if (plan_b == null)
                return false;
            if (!dirty)
                return false;

            double low_amp = amp * Math.Pow(10.0, 20 * gain);

            // set value
            buf[0] = (lower == 0) ? amp : low_amp;
            buf[1] = 0.0;
            buf[tap * 2 + 1] = 0.0;
            buf[tap * 2] = (upper == tap) ? amp : low_amp;
            for (int i = 1; i < tap; i++)
            {
                buf[2 * (size - i)] = buf[i * 2] = (lower <= i && i <= upper) ? amp : low_amp;
                buf[2 * (size - i) + 1] = buf[i * 2 + 1] = 0.0;
            }
            // copy to FFTW memory
            Marshal.Copy(buf, 0, pin, size * 2);

            // DFT
            fftw.execute(plan_b);

            // copy with window
            double[] wk = new double[size * 2];
            Marshal.Copy(pout, wk, 0, size * 2);
            for (int i = 0; i < size; i++)
            {
                double y = (i > tap) ? window[size - i] : window[i];
                wk[i * 2] *= y;
                wk[i * 2 + 1] = 0.0;
                factor[i] = wk[i * 2];
            }

            Marshal.Copy(wk, 0, psin, size * 2);

            // Forward  histroy to spectrum
            fftw.execute(plan_f);

            Marshal.Copy(psout, wk, 0, size * 2);

            for (int i = 0; i < size; i++)
            {
                double r2 = Math.Pow(wk[i * 2], 2.0);
                double i2 = Math.Pow(wk[i * 2+1], 2.0);
                double a = Math.Sqrt(Math.Max(r2 + i2, 1e-20)); // must be positive
                // gain in dB
                gains[i] = 20.0 * Math.Log10(a);
            }

            dirty = false;

            return true;
        }

        void Free()
        {
            if(pin == null)
                return;
            fftw.free(pin);
            fftw.free(pout);
            fftw.destroy_plan(plan_b);
            fftw.destroy_plan(plan_f);
        }
    }
}
