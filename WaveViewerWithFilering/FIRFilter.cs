using System;
using fftwlib;
using System.Runtime.InteropServices;  // for IntPtr

namespace WaveViewerWithFilering
{
  public class FIRFilter
  {
    private bool dirty;

    public bool IsDirty { get { return dirty; } }

    public FIRFilter()
    {
      windowType_ = WindowType.None;
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

    private WindowType windowType_; public WindowType windowType
    {
      get { return windowType_; }
      set
      {
        windowType_ = value;
        SetupWindow();
      }
    }

    private double alpha;
    public double Alpha
    {
      get { return alpha; }
      set
      {
        alpha = value;
        windowType_ = WindowType.Kaiser;
        SetupWindow();
      }
    }


    private void SetupWindow()
    {
      if (Tap == 0)
        return;
      switch (windowType)
      {
        case WindowType.None:
        case WindowType.Rectangle:
          window_ = new RectangleWindow(Tap);
          break;
        case WindowType.Kaiser:
          window_ = new KaiserWindow(Tap, Alpha);
          break;
        case WindowType.Han:
          window_ = new HannWindow(Tap);
          break;
        case WindowType.Hamming:
          window_ = new HammingWindow(Tap);
          break;
        case WindowType.Blackman:
          window_ = new BlackmanWindow(Tap);
          break;
      }
      dirty = true;
    }

    // attributes
    private int tap;
    public int Tap
    {
      get { return tap; }
      set
      {
        tap = value;
        Size = value * 2;

        amp = 1.0 / Size;

        buf = new double[Size * 2];
        Factors = new double[Size];
        Gains = new double[Size];

        Free();
        pin = fftw.malloc(sizeof(double) * 2 * Size);
        pout = fftw.malloc(sizeof(double) * 2 * Size);
        plan_b = fftw.dft_1d(Size, pin, pout, fftw_direction.Backward, fftw_flags.Estimate);
        psin = fftw.malloc(sizeof(double) * 2 * Size);
        psout = fftw.malloc(sizeof(double) * 2 * Size);
        plan_f = fftw.dft_1d(Size, psin, psout, fftw_direction.Forward, fftw_flags.Estimate);
        SetupWindow();
      }
    }
    private int lower; public int Lower { get { return lower; } set { lower = value; dirty = true; } }
    private int upper; public int Upper { get { return upper; } set { upper = value; dirty = true; } }
    private double gain; public double Gain { get { return gain; } set { gain = value; dirty = true; } }
    private NotchFilterInfo.NotchesDataTable notchInfo;
    public NotchFilterInfo.NotchesDataTable Notch { get { return notchInfo; } set { notchInfo = value; dirty = true; } }

    public double SamplingRate { get; set; }

    // getter only
    public int Size { get; private set; }
    public double[] Factors { get; private set; }
    public double[] Gains { get; private set; }

    private double[] buf;
    private IntPtr pin, pout, plan_b, plan_f, psin, psout;
    private double amp;

    public bool Design()
    {
      if (Tap == 0)
        return false;
      if (plan_b == null)
        return false;
      if (!dirty)
        return false;

      double low_amp = amp * Math.Pow(10.0, Gain / 20);

      // set value
      buf[0] = (Lower == 0) ? amp : low_amp;
      buf[1] = 0.0;
      buf[Tap * 2 + 1] = 0.0;
      buf[Tap * 2] = (Upper == Tap) ? amp : low_amp;
      for (int i = 1; i < Tap; i++)
      {
        buf[2 * (Size - i)] = buf[i * 2] = (Lower <= i && i <= Upper) ? amp : low_amp;
        buf[2 * (Size - i) + 1] = buf[i * 2 + 1] = 0.0;
      }

      // Notch filtering
      if (SamplingRate > 0.0 && notchInfo != null)
      {
        double df = SamplingRate / 2 / Tap;
        foreach (var set in notchInfo)
        {
          if (set.Enable)
          {
            for (int i = Lower; i <= Upper; i++)
            {
              double f = i * df;
              double delta = Math.Abs(f - set.Frequency);
              if (delta < set.Band)
              {
                double rate = Math.Pow(10.0, set.Gain / 20.0 * (1 - delta / set.Band));
                buf[2 * (Size - i)] = buf[i * 2] = buf[i * 2] * rate;
              }
            }
          }
        }
      }

      // copy to FFTW memory
      Marshal.Copy(buf, 0, pin, Size * 2);

      // DFT
      fftw.execute(plan_b);

      // copy with window
      double[] wk = new double[Size * 2];
      Marshal.Copy(pout, wk, 0, Size * 2);
      for (int i = 0; i < Size; i++)
      {
        double y = (i > Tap) ? window[Size - i] : window[i];
        wk[i * 2] *= y;
        wk[i * 2 + 1] = 0.0;
        Factors[i] = wk[i * 2];
      }

      Marshal.Copy(wk, 0, psin, Size * 2);

      // Forward  histroy to spectrum
      fftw.execute(plan_f);

      Marshal.Copy(psout, wk, 0, Size * 2);

      for (int i = 0; i < Size; i++)
      {
        double r2 = Math.Pow(wk[i * 2], 2.0);
        double i2 = Math.Pow(wk[i * 2 + 1], 2.0);
        double a = Math.Sqrt(Math.Max(r2 + i2, 1e-20)); // must be positive
                                                        // gain in dB
        Gains[i] = 20.0 * Math.Log10(a);
      }

      dirty = false;

      return true;
    }

    void Free()
    {
      if (pin == null)
        return;
      fftw.free(pin);
      fftw.free(pout);
      fftw.destroy_plan(plan_b);
      fftw.destroy_plan(plan_f);
    }
  }
}
