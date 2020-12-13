using System;
using System.Collections.Generic;
using System.Linq;
using ComplexArrayLib;
using WaveFile;

namespace WaveViewerWithFilering
{
  public class WaveDataSet : System.ComponentModel.INotifyPropertyChanged
  {
    #region Construction
    public WaveDataSet(double[] wave, double delta_t, bool acc_data = false)
    {
      data = wave;
      TimeIncrement = delta_t;
      IsAcc = acc_data;
      Init();
    }
    public WaveDataSet(IWaveFile wavefile, int ch)
    {
      data = wavefile[ch];
      TimeIncrement = wavefile.TimeIncrement(ch);
      IsAcc = wavefile.Name(ch).Contains("_Ya_") || wavefile.Name(ch).Contains("_Za_");

      Init();

      foreach (var tag in new string[] { "速度", "キロ程" })
      {
        if (wavefile.Name(ch).Contains(tag))
          DisableBaselineShift = true;
      }
    }
    private void Init()
    {
      initialized = false;
      DisableBaselineShift = false;
      dataStart = 0;
      filter = new FIRFilter
      {
        SamplingRate = 1.0 / TimeIncrement
      };
      overSample = 1;
      rawWaveStart = -1;
      if (IsAcc)
        integral = 0;
      else
        integral = -1;
      Nfft = 0;
      UpdateNfft();
    }
    #endregion

    #region Properties

    // with Setter
    public int Lower { get { return filter.Lower; } set { filter.Lower = value; UpdateFactors(); } }
    public int Upper { get { return filter.Upper; } set { filter.Upper = value; UpdateFactors(); } }
    public double Gain { get { return filter.Gain; } set { filter.Gain = value; UpdateFactors(); } }
    public double Alpha { get { return filter.Alpha; } set { filter.Alpha = value; UpdateFactors(); } }

    public FIRFilter.WindowType WindowType
    {
      get { return filter.windowType; }
      set { filter.windowType = value; UpdateFactors(); }
    }

    public int NumDisp
    {
      get { return numDisp; }
      set
      {
        numDisp = value;
        UpdateNfft();
        SetupRawWave();
      }
    }

    public int DataStart
    {
      get { return dataStart; }
      set
      {
        dataStart = value;
        SetupRawWave();
        NotifyPropertyChanged("data_start");
      }
    }

    public int Tap
    {
      get { return filter.Tap; }
      set
      {
        filter.Tap = value;
        hann = new HannWindow(value);
        UpdateNfft();
        NotifyPropertyChanged("tap");
      }
    }

    public int OverSample
    {
      get { return overSample; }
      set
      {
        switch (value)
        {
          case 1:
          case 2:
          case 4:
          case 8:
            if (OverSample != value)
            {
              overSample = value;
              lock (fftw)
              {
                over = new WaveData(Nfft * OverSample);
              }
              Update();
            }
            break;
          default:
            throw new ArgumentException("over_sample can be 1, 2, 4 or 8");
        }
        NotifyPropertyChanged("over_sample");
      }
    }

    public string Category
    {
      get
      {
        if (integral < 0)
          return CATEGORY.Last();
        return CATEGORY[integral];
      }
      set
      {
        if (IsAcc)
        {
          var val = Array.FindIndex(CATEGORY, s => s == value);
          if (val != integral)
          {
            integral = val;
            UpdateWave(true);
            NotifyPropertyChanged("category");
          }
        }
      }
    }
    public NotchFilterInfo.NotchesDataTable Notch { get { return filter.Notch; } set { filter.Notch = value; } }
    public bool DisableBaselineShift { get; set; }

    // Read only properties
    public IEnumerable<double> Data { get { return data; } }
    public WindowFunction Window { get { return filter.window; } }
    public bool IsAcc { get; private set; }
    public int Length { get { return data.Length; } }
    public int Nfft { get; private set; }
    public double TimeIncrement { get; private set; }
    public double StartTime { get { return dataStart * TimeIncrement; } }
    public double[] Factor { get { return filter.Factors; } }
    public double[] Gains { get { return filter.Gains; } }
    private readonly double[] data; //{ get; private set; }
    public double[] Freqs { get; private set; }
    public double[] Source { get; private set; }
    public double[] Xvalues { get; private set; }
    public double[] OverSampled { get; private set; }
    public double[] Filtered { get; private set; }

    public uint OverId { get { return over.wave_id; } }
    public uint FilteredId { get { return ans.wave_id; } }
    public uint FactorId { get { return factors.wave_id; } }
    public uint GainId { get { return factors.sp_id; } }
    public uint SourceId { get { return wave.wave_id; } }

    public bool IsValid
    {
      get
      {
        if (data == null) return false;
        if (filter == null) return false;
        if (numDisp == 0) return false;
        if (Nfft == 0) return false;
        if (TimeIncrement == 0.0) return false;

        return true;
      }
    }

    #endregion

    #region Public Methods

    public double[] WaveSpectrumAmplitudeIn_dB()
    {
      return wave.dB.ToArray();
    }

    public void Update()
    {
      lock (this)
      {
        if (now_updating) return;  // To avoid stack overflow
        now_updating = true;
      }

      SetupRawWave();
      ApplyFilter();

      if (currentOverSample != OverSample || over.wave_id < ans.wave_id)
      {
        if (over.Length != ans.Length * OverSample)
        {
          lock (fftw)
          {
            over = new WaveData(ans.Length * OverSample);
          }
        }

        if (OverSample == 1)
        {
          // noneed to upsampling;
          over.Wave = ans.Wave;
        }
        else
        {
          over.clear_sp(); // need clear
          over.Spectrum = ans.Spectrum.Select(x => x * OverSample);
        }
        OverSampled = over.Wave.Take(NumDisp * OverSample).ToArray();
        currentOverSample = OverSample; //calc
        NotifyPropertyChanged("over_sampled");
      }
      initialized = true;
      lock (this)
      {
        now_updating = false;
      }
    }

    // for debug
    public IEnumerable<double>[] DebugWaves()
    {
      int zeros = Math.Max(2 * Tap - DataStart, 0);
      return new IEnumerable<double>[]{
                wave.Wave,
                ans.Wave,
                over.Wave,
                rawWave.Wave,
                Enumerable.Repeat(0.0,zeros).Concat(data.Skip(DataStart - 2*Tap).Take(NumDisp + 4*Tap-zeros))
            };
    }

    public enum State
    {
      Raw,
      Source,
      Filtered,
      Oversampled,
    }

    public double[] Wave(State target)
    {
      switch (target)
      {
        case State.Raw:
          return rawWave.Wave.ToArray();
        case State.Source:
          return Source;
        case State.Filtered:
          return Filtered;
        case State.Oversampled:
          return OverSampled;
        default:
          throw new ArgumentException();
      }
    }
    public double[] Power(State target)
    {
      switch (target)
      {
        case State.Raw:
          return rawWave.Power.ToArray();
        case State.Source:
          return wave.Power.ToArray();
        case State.Filtered:
          return ans.Power.ToArray();
        case State.Oversampled:
          return over.Power.ToArray();
        default:
          throw new ArgumentException();
      }
    }

    public IEnumerable<double>[] DebugSpectrums()
    {
      return new IEnumerable<double>[]{
                wave.Abs,
                ans.Abs,
                over.Abs,
                rawWave.Abs,
            };
    }
    #endregion

    #region Internal Use
    #region Private Fields
    //--Private members
    private WaveData rawWave;    // raw wave. this is partial copy of data.
    private WaveData wave;        // wave data (it can be disp or vel wave)
    private WaveData factors;     // expanded filter data
    private WaveData ans;         // filtered wave (iDFT of sp_ans)
    private WaveData over;
    private ComplexArray omega;
    private HannWindow hann;
    private int rawWaveStart;
    private int currentOverSample;
    private int rawWaveNumDisp;
    private double[] extractedRawWave;
    private ComplexArray omega2;
    private FIRFilter filter;
    private int integral;

    private int dataStart;
    private int numDisp;
    private int overSample;

    // Used for lock
    private static readonly object fftw = 1;

    private static readonly string[] CATEGORY = new string[] { "ACC", "VEL", "DIS", "NONE" };
    private bool initialized;

    #endregion

    #region Private methods

    /// <summary>
    /// NFFT： Number of data for FFT (= 2^n > 4tap + num_disp, >1024)
    /// </summary>
    private void UpdateNfft()
    {
      int val = 1024;
      while (val < NumDisp + filter.Tap * 4)
        val *= 2;

      if (Nfft != val)
      {
        // update
        Nfft = val;

        lock (fftw)
        {
          over = new WaveData(Nfft * overSample);
          ans = new WaveData(Nfft);
          factors = new WaveData(Nfft);
          wave = new WaveData(Nfft);
          rawWave = new WaveData(Nfft);
        }
        extractedRawWave = new double[Nfft];

        // update Omega
        double fs = 1.0 / TimeIncrement;
        double df = fs / Nfft;
        lock (fftw)
        {
          omega = new ComplexArray(Nfft / 2 + 1);
          omega2 = new ComplexArray(Nfft / 2 + 1);
        }

        double df0 = -1.0 / (df * 2 * Math.PI);
        double v;
        for (int i = 1; i < Nfft / 2; i++)
        {
          v = df0 / i;
          omega[i].Imag = v;
          omega2[i].Real = -v * v;
        }

        // update freqs
        Freqs = Enumerable.Range(0, Nfft / 2 + 1).Select(i => df * i).ToArray();
      }
    }

    // copy wave data and FFT
    private bool SetupRawWave(bool force = false)
    {
      if (NumDisp == 0)
        return false;

      if (initialized && !force &&
          rawWaveStart == DataStart &&
          rawWaveNumDisp == NumDisp)
        return false;

      Xvalues = Enumerable.Range(0, NumDisp).Select(i => (i + dataStart) * TimeIncrement).ToArray();

      int n_start = dataStart - Tap * 2;
      var base_wave = data.Skip(n_start).Take(NumDisp + 4 * Tap);

      if (DisableBaselineShift)
      {
        rawWave.Wave = base_wave;
      }
      else
      {
        // take with basiline adjustment
        //double base_line = base_wave.Take(2 * tap).Average() / 2.0
        //    + base_wave.Skip(base_wave.Count() - 2 * tap).Take(2 * tap).Average() / 2;

        // determine baseline as the most frequent value
        double base_line = base_wave.GroupBy(x => (int)Math.Round(x * 10))
            .Select((a) => new Tuple<int, double>(a.Count(), a.Key * 0.1))
            .OrderByDescending((a) => a.Item1)
            .First().Item2;
        TakeRawWaveWithBaseline(base_line);
        //var range = base_wave.Max() - base_wave.Min();
        //var delta = range / 1e6;

        //for (int i = 0; i < 20; i++)
        //{
        //    take_raw_wave_with_baseline(base_line);
        //    var error = extracted_raw_wave.Skip(tap).Take(tap).Average()/2+
        //        extracted_raw_wave.Skip(2*tap).Skip(num_disp).Take(tap).Average()/2;
        //    if (Math.Abs(error) < delta)
        //        break;
        //    base_line += error * 0.75; //update
        //}
        rawWave.Wave = extractedRawWave;
      }
      NotifyPropertyChanged("raw_wave");
      rawWaveStart = DataStart;
      rawWaveNumDisp = NumDisp;

      return true;
    }

    private void TakeRawWaveWithBaseline(double base_line)
    {
      int tap = filter.Tap;
      int n_start = DataStart - tap * 2;
      int n_end = DataStart + NumDisp;

      //raw_wave.clear_wave();
      Array.Clear(extractedRawWave, 0, Nfft);

      // copy pre_data with filter
      for (int i = 0; i < tap; i++)
      {
        if (i + n_start < 0)
          extractedRawWave[Nfft - 2 * tap + i] = 0.0;
        else
          extractedRawWave[Nfft - 2 * tap + i] = (data[Math.Abs(i + n_start)] - base_line) * hann[tap - i];
        if (i + n_start + tap < 0)
          extractedRawWave[Nfft - 1 * tap + i] = 0.0;
        else
          extractedRawWave[Nfft - 1 * tap + i] = data[Math.Abs(i + tap + n_start)] - base_line;
      }
      // copy main_data
      int last_index = data.Length - 1;
      for (int i = 0; i < NumDisp; i++)
      {
        int k = last_index - Math.Abs(last_index - (DataStart + i));
        extractedRawWave[i] = data[k] - base_line;
      }
      // copy postdata
      for (int i = 0; i < tap; i++)
      {
        int k = last_index - Math.Abs(last_index - (n_end + i));
        extractedRawWave[NumDisp + i] = data[k] - base_line;
      }
      // copy with filter
      for (int i = 0; i < tap; i++)
      {
        int j = i + 3 * tap + NumDisp;
        // int k = last_index - Math.Abs(last_index - (n_end + tap + i));
        if (j > last_index)
          extractedRawWave[NumDisp + tap + i] = 0.0;
        else
          extractedRawWave[NumDisp + tap + i] = (data[j] - base_line) * hann[i];
      }
    }

    private bool UpdateWave(bool force = false)
    {
      SetupRawWave();
      if (force || wave.wave_id < rawWave.wave_id)
      {
        if (integral < 1)
        {
          // ACC and others are not need integration.
          wave.Spectrum = rawWave.Spectrum;
        }
        else
        {
          // VEL and DISP need integration.
          ComplexArray w = integral == 1 ? omega : omega2;

          // Integrate in frequency domain.
          wave.Spectrum = rawWave.Spectrum.Zip(w, (a, b) => a * b);
        }
        Source = wave.Wave.Take(NumDisp).ToArray(); // calc
        NotifyPropertyChanged("source");
        return true;
      }
      return false;
    }

    private bool UpdateFactors(bool force = false)
    {

      if (force || filter.Design() || factors.wave_id == 0) // refresh filter if needed
      {
        factors[0] = filter.Factors[0];
        for (int i = 1; i <= filter.Tap; i++)
        {
          factors[Nfft - i] = factors[i] = filter.Factors[i];
        }
        NotifyPropertyChanged("factors");
        return true;
      }
      return false;
    }

    private void ApplyFilter(bool force = false)
    {
      if (wave.wave_id < rawWave.wave_id)
      {
        UpdateWave();
      }

      UpdateFactors();

      if (force ||
          ans.wave_id < wave.wave_id ||
          ans.wave_id < factors.wave_id)
      {
        // convolution in Frequency domain
        ans.Spectrum = wave.Spectrum.Zip(factors.Spectrum, (a, b) => a * b);
        // copy result to filtered
        Filtered = ans.Wave.Take(NumDisp).ToArray(); // calc
        NotifyPropertyChanged("filtered");
      }
    }

    private void NotifyPropertyChanged(string propertyName)
    {
      PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
    }
    #endregion // Private Methods
    #endregion // Internal Use

    public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
    private bool now_updating;
  }
}
