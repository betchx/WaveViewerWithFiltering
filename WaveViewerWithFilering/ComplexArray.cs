using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using fftwlib;


namespace WaveViewerWithFilering
{
    public
    class ComplexArray
    {
        private double[] data;
        public int size{get; private set;}
        public int len{get; private set;}
        private IntPtr pin;
        private IntPtr pout;
        private IntPtr plan;

        public class None { }
        private class AssignTag { }
        private static AssignTag ASSIGN;

        public ComplexArray(int length)
        {
            len = length;
            size = length * 2;
            data = new double[size];
        }

        // for intarnal use
        private ComplexArray(double[] arr, AssignTag dummy)
        {
            data = arr;  // use original array
            size = arr.Length;
            len = size / 2;

            // check
            if (size != len * 2)
                throw new ArgumentException("Length of Array must be even number.");
        }

        public ComplexArray(double[] real, double[] imag)
        {
            len = real.Length;
            if (len != imag.Length)
                throw new ArgumentException("lengths of real wave and imag wave must be same");
            size = len * 2;
            data = new double[size];
            for (int i = 0; i < len; i++)
            {
                data[i * 2] = real[i];
                data[i * 2 + 1] = imag[i];
            }
        }

        public ComplexArray(ComplexArray ca)
        {
            len = ca.len;
            size = ca.size;
            data = ca.data.ToArray();
        }

        // special construction


        public ComplexArray(double[] wave, ComplexArray.None dummy)
        {
            len = wave.Length;
            size = len * 2;
            data = new double[size];
            for (int i = 0; i < len; i++)
            {
                data[i * 2] = wave[i];
            }
        }

        public ComplexArray(ComplexArray.None dummy, double[] wave)
        {
            len = wave.Length;
            size = len * 2;
            data = new double[size];
            for (int i = 0; i < len; i++)
            {
                data[i * 2 + 1] = wave[i];
            }
        }

        public static ComplexArray assign(double[] data)
        {
            return new ComplexArray(data, ASSIGN);
        }


        public static ComplexArray real(double[] wave)
        {
            int len = wave.Length;
            ComplexArray ans = new ComplexArray(len);
            for (int i = 0; i < len; i++)
            {
                ans.data[i * 2] = wave[i];
            }
            return ans;
        }

        public static ComplexArray imag(double[] wave)
        {
            int len = wave.Length;
            ComplexArray ans = new ComplexArray(len);
            for (int i = 0; i < len; i++)
            {
                ans.data[i * 2 + 1] = wave[i];
            }
            return ans;
        }

        /// <summary>
        /// Constract from DFT result of with real wave.
        /// </summary>
        /// <param name="wave">real wave</param>
        /// <returns>Constructed ComplexArray (Spectrum)</returns>
        public static ComplexArray by_fft(double[] wave)
        {
            throw new NotImplementedException();
            IntPtr pin = IntPtr.Zero;
            IntPtr pout = IntPtr.Zero;
            IntPtr plan = IntPtr.Zero;
            int len = wave.Length;
            int size = len + 2;
            var res = new ComplexArray(size/2);
            try
            {
                pin = fftw.malloc(sizeof(double) * len);
                pout = fftw.malloc(sizeof(double) * size);
                plan = fftw.dft_r2c_1d(len, pin, pout, fftw_flags.Estimate);

                Marshal.Copy(wave, 0, pin, len);
                fftw.execute(plan);
                Marshal.Copy(pout, res.data, 0, size);
            }
            finally
            {
                fftw.free(pin);
                fftw.free(pout);
                fftw.destroy_plan(plan);
            }
            return res;
        }

     

        // accsess
        public Complex this[int index]
        {
            get{
                if (index >= 0 && index < len)
                    return new Complex(data, index);
                throw new IndexOutOfRangeException();
            }
            set
            {
                if (index < 0 || index >= len)
                    throw new IndexOutOfRangeException();
                data[index * 2] = value.real;
                data[index * 2 + 1] = value.imag;
            }
        }



        // operation

        /// <summary>
        /// DFT (Forword).
        /// Not normalized.
        /// </summary>
        /// <returns>Forward DFTed ComplexArray</returns>
        public ComplexArray dft(fftw_direction dir = fftw_direction.Forward)
        {
            ComplexArray ans = new ComplexArray(len);
            try
            {
                pin = fftw.malloc(sizeof(double) * size);
                pout = fftw.malloc(sizeof(double) * size);
                plan = fftw.dft_1d(len, pin, pout, dir, fftw_flags.Estimate);
                Marshal.Copy(data, 0, pin, size);
                fftw.execute(plan);
                Marshal.Copy(pout, ans.data, 0, size);
            }
            finally
            {
                fftw.free(pin);
                fftw.free(pout);
                fftw.destroy_plan(plan);
            }
            return ans;
        }

        /// <summary>
        /// Inverse DFT  (Backword FFT)
        /// Not normalized.
        /// </summary>
        /// <returns>Backword DFTed ComplexArray</returns>
        public ComplexArray idft()
        {
            return dft(fftw_direction.Backward);
        }

        /// <summary>
        /// get real part of iDFT of this.
        /// (Spectrum to Time-History)
        /// </summary>
        /// <returns>real wave by iDFT</returns>
        public double[] idft_wave()
        {
            throw new NotImplementedException();

            int nfft = size - 2;
            double[] ans = new double[nfft];
            try
            {
                pin = fftw.malloc(sizeof(double) * size);
                pout = fftw.malloc(sizeof(double) * nfft);
                plan = fftw.dft_c2r_1d(len, pin, pout, fftw_flags.Estimate);
                Marshal.Copy(data, 0, pin, size);
                fftw.execute(plan);
                Marshal.Copy(pout, ans, 0, nfft);
            }
            finally
            {
                fftw.free(pin);
                fftw.free(pout);
                fftw.destroy_plan(plan);
            } 
            return ans;
        }


        public double[] real()
        {
            double[] ans = new double[len];
            for (int i = 0; i < len; i++)
            {
                ans[i] = data[i*2];
            }
            return ans;
        }
        public double[] imag()
        {
            double[] ans = new double[len];
            for (int i = 0; i < len; i++)
            {
                ans[i] = data[i * 2 + 1];
            }
            return ans;
        }

        public double[] abs()
        {
            double[] ans = new double[len];
            for (int i = 0; i < len; i++)
            {
                ans[i] = this[i].abs;
            }
            return ans;
        }

        public double[] power()
        {
            double[] ans = new double[len];
            for (int i = 0; i < len; i++)
            {
                ans[i] = this[i].power;
            }
            return ans;
        }

        public double[] angl()
        {
            double[] ans = new double[len];
            for (int i = 0; i < len; i++)
            {
                ans[i] = this[i].angl;
            }
            return ans;
        }


        // operators


        public static ComplexArray operator * (ComplexArray lhs, double d)
        {
            ComplexArray ans = new ComplexArray(lhs.len);
            for (int i = 0; i < lhs.size; i++)
			{
                ans.data[i] = lhs.data[i] * d;
			}
            return ans;
        }
        public static ComplexArray operator *(double d, ComplexArray rhs)
        {
            ComplexArray ans = new ComplexArray(rhs.len);
            for (int i = 0; i < rhs.size; i++)
            {
                ans.data[i] = rhs.data[i] * d;
            }
            return ans;
        }



        public static ComplexArray operator *(ComplexArray lhs, Complex c)
        {
            ComplexArray ans = new ComplexArray(lhs.len);
            for (int i = 0; i < lhs.len; i++)
            {
                ans[i] = lhs[i] * c;
            }
            return ans;
        }

        public static ComplexArray operator /(ComplexArray lhs, Complex c)
        {
            ComplexArray ans = new ComplexArray(lhs.len);
            for (int i = 0; i < lhs.len; i++)
            {
                ans[i] = lhs[i] / c;
            }
            return ans;
        }

        public static ComplexArray operator *(Complex c, ComplexArray rhs)
        {
            ComplexArray ans = new ComplexArray(rhs.len);
            for (int i = 0; i < rhs.len; i++)
            {
                var x = rhs[i] * c;
                
            }
            return ans;
        }

        public static ComplexArray operator *(ComplexArray lhs, ComplexArray rhs)
        {
            if (lhs.len != rhs.len)
                throw new ArgumentException("Array size must be same");

            ComplexArray ans = new ComplexArray(lhs.len);
            for (int i = 0; i < lhs.len; i++)
            {
                ans[i] = lhs[i] * rhs[i];
            }
            return ans;
        }
        public static ComplexArray operator /(ComplexArray lhs, ComplexArray rhs)
        {
            if (lhs.len != rhs.len)
                throw new ArgumentException("Array size must be same");

            ComplexArray ans = new ComplexArray(lhs.len);
            for (int i = 0; i < lhs.len; i++)
            {
                ans[i] = lhs[i] / rhs[i];
            }
            return ans;
        }
        public static ComplexArray operator +(ComplexArray lhs, ComplexArray rhs)
        {
            if (lhs.len != rhs.len)
                throw new ArgumentException("Array size must be same");

            ComplexArray ans = new ComplexArray(lhs.len);
            for (int i = 0; i < lhs.len; i++)
            {
                ans[i] = lhs[i] + rhs[i];
            }
            return ans;
        }
        public static ComplexArray operator -(ComplexArray lhs, ComplexArray rhs)
        {
            if (lhs.len != rhs.len)
                throw new ArgumentException("Array size must be same");

            ComplexArray ans = new ComplexArray(lhs.len);
            for (int i = 0; i < lhs.len; i++)
            {
                ans[i] = lhs[i] - rhs[i];
            }
            return ans;
        }


        public static ComplexArray operator +(ComplexArray lhs, double d)
        {
            ComplexArray ans = new ComplexArray(lhs.len);
            for (int i = 0; i < lhs.len; i++)
            {
                ans.data[i*2] = lhs.data[2*i] + d;
            }
            return ans;
        }
        public static ComplexArray operator -(ComplexArray lhs, double d)
        {
            ComplexArray ans = new ComplexArray(lhs.len);
            for (int i = 0; i < lhs.len; i++)
            {
                ans.data[i * 2] = lhs.data[2 * i] - d;
            }
            return ans;
        }



    }
}
