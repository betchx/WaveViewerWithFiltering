using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using fftwlib;
using System.Runtime.InteropServices;  // for IntPtr

namespace WaveViewerWithFilering
{
    class FIRFilter
    {
        private bool dirty;


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
 
                Free();
                pin = fftw.malloc(sizeof(double) * 2 * size);
                pout = fftw.malloc(sizeof(double) * 2 * size);
                plan = fftw.dft_1d(size, pin, pout, fftw_direction.Backward, fftw_flags.Estimate);
                dirty = true;
            }
        }
        private int lower_; public int lower { get { return lower_; } set { lower_ = value; dirty = true; } }
        private int upper_; public int upper { get { return upper_; } set { upper_ = value; dirty = true; } }

        // getter only
        public int size { get; private set; }
        public double[] factor { get; private set; }

        private double[] buf;
        private IntPtr pin, pout, plan;
        private double amp;

        public bool design()
        {
            if (plan == null)
                return false;
            if (!dirty)
                return true;

            // set value
            buf[0] = (lower==0)?amp:0.0;
            buf[1] = 0.0;
            buf[size*2-1] = 0.0;
            buf[size*2-2] = (upper == tap)?amp:0.0;
            for (int i = 0; i < tap; i++)
            {
                buf[i * 2] = (lower <= i && i <= upper) ? amp : 0.0;
                buf[i * 2 + 1] = 0.0;
            }
            // copy to FFTW memory
            Marshal.Copy(buf, 0, pin, size * 2);

            // DFT
            fftw.execute(plan);

            double[] wk = new double[size * 2];
            Marshal.Copy(pout, wk, 0, size * 2);
            for (int i = 0; i < size; i++)
            {
                factor[i] = wk[i * 2];
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
            fftw.destroy_plan(plan);
        }
    }
}
