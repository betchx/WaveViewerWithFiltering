using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WaveViewerWithFilering
{
    public class Complex
    {
        private double[] data;
        private int pos;

        public Complex(double[] arr, int index)
        {
            data = arr;
            pos = index * 2;
        }
        public Complex(double r, double i = 0.0)
        {
            data = new double[2];
            pos = 0;
            real = r;
            imag = i;
        }
        public Complex()
        {
            data = new double[2];
            pos = 0;
            real = 0.0;
            imag = 0.0;
        }
        public Complex(Complex c)
        {
            data = new double[2];
            pos = 0;
            real = c.real;
            imag = c.imag;
        }

        public static Complex I { get { return new Complex(0.0, 1.0); } }

        // properties
        public virtual double real { get { return data[pos + 0]; } set { data[pos + 0] = value; } }
        public virtual double imag { get { return data[pos + 1]; } set { data[pos + 1] = value; } }
        public double abs { get { return Math.Abs(power); } }
        public double angl { get { return Math.Atan2(imag, real); } }
        public double power { get { return real * real + imag * imag; } }
        public Complex conj
        {
            get { return new Complex(real, -1 * imag); }
        }

        // operator overload
        public static Complex operator +(Complex l, Complex r)
        {
            return new Complex(l.real + r.real, l.imag + r.imag);
        }
        public static Complex operator +(Complex lhs, double rhs)
        {
            return new Complex(lhs.real + rhs, lhs.imag);
        }
        public static Complex operator +(double lhs, Complex rhs)
        {
            return new Complex(lhs + rhs.real, rhs.imag);
        }
        public static Complex operator -(Complex lhs, Complex rhs)
        {
            return new Complex(lhs.real - rhs.real, lhs.imag - rhs.imag);
        }
        public static Complex operator -(Complex lhs, double rhs)
        {
            return new Complex(lhs.real - rhs, lhs.imag);
        }
        public static Complex operator -(double lhs, Complex rhs)
        {
            return new Complex(lhs - rhs.real, -rhs.imag);
        }
        public static Complex operator -(Complex c)
        {
            return new Complex(-c.real, -c.imag);
        }
        public static Complex operator *(Complex lhs, Complex rhs)
        {
            double a = lhs.real;
            double b = lhs.imag;
            double c = rhs.real;
            double d = rhs.imag;
            double r = a * c - b * d;
            double i = a * d + b * c;
            return new Complex(r, i);
        }
        public static Complex operator /(Complex lhs, Complex rhs)
        {
            double a = lhs.real;
            double b = lhs.imag;
            double c = rhs.real;
            double d = rhs.imag;
            double div = rhs.power;
            double r = (a * c + b * d) / div;
            double i = (b * c - a * d) / div;
            return new Complex(r, i);
        }

        public static implicit operator Complex(double d)
        {
            return new Complex(d, 0.0);
        }
        public static ComplexArray operator *(double[] arr, Complex c)
        {
            return ComplexArray.real(arr) * c;
        }

        public static ComplexArray operator /(double[] arr, Complex c)
        {
            return ComplexArray.real(arr) / c;
        }
    }
}
