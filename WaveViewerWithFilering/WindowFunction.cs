using System;

namespace WaveViewerWithFilering
{
  public class WindowFunction
  {
    private readonly double[] factor; public double[] Factor { get { return factor; } }
    private readonly int size; public int Size { get { return size; } }

    public WindowFunction(int n)
    {
      size = n;
      factor = new double[n + 1];
    }

    public double this[int k]
    {
      get
      {
        if (Math.Abs(k) > Size)
          return 0.0;
        return Factor[Math.Abs(k)];
      }
    }
  }

  class RectangleWindow : WindowFunction
  {
    public RectangleWindow(int n)
        : base(n)
    {
      for (int i = 0; i <= n; i++)
      {
        Factor[i] = 1.0;
      }
    }
  }

  class HannWindow : WindowFunction
  {
    public HannWindow(int n)
        : base(n)
    {
      for (int i = 0; i <= n; i++)
      {
        double x = 0.5 + (0.5 * i) / n;
        Factor[i] = 0.5 - 0.5 * Math.Cos(2 * Math.PI * x);
      }
    }
  }

  class HanningWindow : HannWindow
  {
    public HanningWindow(int n) : base(n) { }
  }

  class HammingWindow : WindowFunction
  {
    public HammingWindow(int n)
        : base(n)
    {
      for (int i = 0; i <= n; i++)
      {
        double x = 0.5 + (0.5 * i) / n;
        Factor[i] = 0.54 - 0.46 * Math.Cos(2 * Math.PI * x);
      }
    }
  }

  class BlackmanWindow : WindowFunction
  {
    public BlackmanWindow(int n)
        : base(n)
    {
      for (int i = 0; i <= n; i++)
      {
        double x = 0.5 + (0.5 * i) / n;
        Factor[i] = 0.42 - 0.5 * Math.Cos(2 * Math.PI * x) + 0.08 * Math.Cos(4 * Math.PI * x);
      }
    }
  }

  class KaiserWindow : WindowFunction
  {
    public KaiserWindow(int n, double alpha)
        : base(n)
    {
      double z = bessi0(Math.PI * alpha);
      for (int i = 0; i <= n; i++)
      {
        double x = 0.5 + (0.5 * i) / n;
        double y = Math.PI * alpha * Math.Sqrt(1 - Math.Pow(2 * x - 1, 2));
        Factor[i] = bessi0(y) / z;
      }
    }

    /// <summary>
    /// Modified bessel funcion I0
    ///
    ///  From Numerical Recipes in C pp. 187-189. (source: p 189)
    /// </summary>
    /// <param name="x">arbitrary real number</param>
    /// <returns>function value</returns>
    static double bessi0(double x)
    {
      double ax = Math.Abs(x);
      double y;
      if (ax < 3.75)
      {
        y = x / 3.75;
        y *= y;
        return 1.0 + y * (3.5156229 + y * (3.0899424 + y * (1.2067492
            + y * (0.2659732 + y * (0.0360768 + y * 0.45813E-2)))));
      }
      else
      {
        y = 3.75 / ax;
        return (Math.Exp(ax) / Math.Sqrt(ax)) * (0.39894228 + y * (0.1328592e-1
            + y * (0.225319e-2 + y * (-0.157565e-2 + y * (0.916281e-2
            + y * (-0.2057706e-1 + y * (0.2635537e-1 + y * (-0.1647633e-1
            + y * 0.392377e-2))))))));
      }
    }
  }

}
