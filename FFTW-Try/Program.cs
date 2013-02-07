using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using fftwlib;

namespace FFTW_Try
{
    class Program
    {
        static void test1()
        {
            const int n = 8;
            IntPtr pin = fftw.malloc(sizeof(double) * n);
            double[] source = Enumerable.Range(1, 8).Select(a=>a+0.0).ToArray();

            Marshal.Copy(source, 0, pin, n);
            IntPtr pout = fftw.malloc(sizeof(double) * 2 * n);
            IntPtr plan = fftw.r2r_1d(n, pin, pout, fftw_kind.R2HC, fftw_flags.Estimate);
            fftw.execute(plan);
            fftw.destroy_plan(plan);
            double[] res = new double[n];
            Marshal.Copy(pout, res, 0, n);

            plan = fftw.r2r_1d(n, pout, pin, fftw_kind.HC2R, fftw_flags.Estimate);
            fftw.execute(plan);
            fftw.destroy_plan(plan);

            double[] res2 = new double[n];
            Marshal.Copy(pin, res2, 0, n);
            fftw.free(pin);
            fftw.free(pout);

            var arr = source.Select((v, i) => new { i, v }).Zip(res.Zip(res2, (a, b) => new { a, b }), (a, b) => new { a.i, a.v, b.a, b.b });

            foreach (var item in arr)
            {
                Console.WriteLine(item);
            }

            Console.ReadLine();
        }
        static void test2()
        {
            const int n = 8;
            IntPtr pin = fftw.malloc(sizeof(double) * n);
            //double[] source = Enumerable.Range(1, 8).Select(a=>a+0.0).ToArray();
            double[] source = new double[n];
            Array.Clear(source, 0, n);
            source[0] = 1.0;

            Marshal.Copy(source, 0, pin, n);
            IntPtr pout = fftw.malloc(sizeof(double) * 2 * n);
            IntPtr plan = fftw.r2r_1d(n, pin, pout, fftw_kind.R2HC, fftw_flags.Estimate);
            fftw.execute(plan);
            fftw.destroy_plan(plan);
            double[] res = new double[n];
            Marshal.Copy(pout, res, 0, n);

            plan = fftw.r2r_1d(n, pout, pin, fftw_kind.HC2R, fftw_flags.Estimate);
            fftw.execute(plan);
            fftw.destroy_plan(plan);

            double[] res2 = new double[n];
            Marshal.Copy(pin, res2, 0, n);
            fftw.free(pin);
            fftw.free(pout);

            var arr = source.Select((v, i) => new { i, v }).Zip(res.Zip(res2, (a, b) => new { a, b }), (a, b) => new { a.i, a.v, b.a, b.b });

            foreach (var item in arr)
            {
                Console.WriteLine(item);
            }

            Console.ReadLine();
        }

        static void test3()
        {
            const int n = 8;
            IntPtr pin = fftw.malloc(sizeof(double) * n);
            double[] source = new double[n];
            Array.Clear(source, 0, n);
            source[1] = 1.0;

            Marshal.Copy(source, 0, pin, n);
            IntPtr pout = fftw.malloc(sizeof(double) * 2 * n);
            IntPtr plan = fftw.dft_r2c_1d(n, pin, pout, fftw_flags.Estimate);
            fftw.execute(plan);
            fftw.destroy_plan(plan);
            double[] res = new double[2*n];
            Marshal.Copy(pout, res, 0, 2*n);

            plan = fftw.dft_c2r_1d(n, pout, pin, fftw_flags.Estimate);
            fftw.execute(plan);
            fftw.destroy_plan(plan);

            double[] res2 = new double[n];
            Marshal.Copy(pin, res2, 0, n);

            fftw.free(pin);
            fftw.free(pout);

            for (int i = 0; i < n; i++)
            {
                Console.WriteLine("{0}: {1}, ({2}, {3}), {4}",
                    i, source[i], res[i * 2], res[i * 2 + 1], res2[i]);
            }
            


            Console.ReadLine();
        }

        static void Main(string[] args)
        {
            test3();
        }
    }
}
