using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WaveFile;

namespace WaveViewerWithFilering
{
  public partial class WaveFilter : Form
  {
    public WaveFilter()
    {
      InitializeComponent();
      data = null;
      UpdateTapInfo();
      upperCutOffFrequencyTrack.Value = tapTrack.Value;
      lowerCutOffFrequencyTrack.Value = 0;
      samplingRate.Text = "1000.0";
      displayDataLength.Text = "100";
      targets = new ComboBox[] { ch_P1, ch_P2, ch_Ya, ch_Za };
      thresholds = new TextBox[] { th_P1, th_P2, th_Ya, th_Za };
      required_lengths = new TextBox[] { rl_P1, rl_P2, rl_Ya, rl_Za };
      overSampling = 1;
      integ.Text = "ACC";
      UpdateChartVisible();
      hide_flag = new bool[] { false, false, false, false, false };
      factor_id = 0u;
      filtered_id = 0u;
      gain_id = 0u;
      source_id = 0u;
      over_id = 0u;
      peakChartPrimaryMax.Tag = peakChart.ChartAreas[0].AxisY;
      peakChartPrimaryMin.Tag = peakChart.ChartAreas[0].AxisY;
      peakChartSecondaryMax.Tag = peakChart.ChartAreas[0].AxisY2;
      peakChartSecondaryMin.Tag = peakChart.ChartAreas[0].AxisY2;
      waveChartMax.Tag = waveChart.ChartAreas[0].AxisY;
      waveChartMin.Tag = waveChart.ChartAreas[0].AxisY;
      freqChartXMin.Tag = freqChart.ChartAreas[0].AxisX;
      freqChartXMax.Tag = freqChart.ChartAreas[0].AxisX;
      freqChartYPrimaryMax.Tag = freqChart.ChartAreas[0].AxisY;
      freqChartYPrimaryMin.Tag = freqChart.ChartAreas[0].AxisY;
      freqChartYSecondaryMax.Tag = freqChart.ChartAreas[0].AxisY2;
      freqChartYSecondaryMin.Tag = freqChart.ChartAreas[0].AxisY2;
    }


    #region Internal Use
    #region InternalStruct
    private struct RangePair
    {
      public double max;
      public double min;
    }
    private struct AxisRanges
    {
      public RangePair wave;
      //public RangePair peak_P;
      //public RangePair peak_a;
    }
    #endregion
    #region Field
    List<WaveDataSet> data;
    int ch;
#pragma warning disable IDE0052 // 読み取られていないプライベート メンバーを削除
    private int num_point;
#pragma warning restore IDE0052 // 読み取られていないプライベート メンバーを削除
    int step;
    const int max_points = 10000;
    private double fs;
    private double fn;
    private IWaveFile wavefile;
    readonly bool[] hide_flag;
    const int HIDE_WAVE = 0;
    const int HIDE_ANS = 1;
    const int HIDE_OVER = 2;
    readonly ComboBox[] targets;
    readonly TextBox[] thresholds;
    readonly TextBox[] required_lengths;
    private int overSampling;
    const int MAX_DISP_SIZE = 100000;
    private uint over_id;
    private uint source_id;
    private uint filtered_id;
    private uint factor_id;
    private uint gain_id;
    private double[] tap_freqs;
    AxisRanges[] axes_ranges;
    private bool force_chart_update;
    #endregion
    #region Properties
    private int Nfft { get { return data[ch].Nfft; } }
    private int Tap { get { return tapTrack.Value; } }
    private WaveDataSet Fir { get { return data[ch]; } }// fir setting affects only active data set.

    private int OverSampling
    {
      get { return overSampling; }
      set
      {
        if (overSampling != value)
        {
          overSampling = value;
          if (data != null)
            System.Threading.Tasks.Parallel.ForEach(data, (wd) =>
            {
              wd.OverSample = value;
            });
        }
      }
    }

    private int NumberOfDisplayed
    {
      get
      {
        if (data == null) return 0;
        return data[ch].NumDisp;
      }
      set
      {
        if (data != null)
          System.Threading.Tasks.Parallel.ForEach(data, (d) => d.NumDisp = value);
      }
    }
    private int NumberOfData
    {
      get
      {
        if (data == null) return 0;
        return data[ch].Length;
      }
    }

    private double TimeIncrement { get { return data[ch].TimeIncrement; } }
    #endregion
    #region PrivateMethods
    #region UpdateMethods

    private void CheckUpdate()
    {
      if (autoUpdate.Checked)
        DataUpdate();
    }

    private void DataUpdate()
    {
      if (wavefile == null)
        return;

      data[ch].Update(); // for debug
                         //System.Threading.Tasks.Parallel.ForEach(data, d => d.update());
      data.AsParallel().ForAll(d => d.Update());

      UpdateWaveChart();
      UpdateFilterChart();
      UpdateFrequencyChart();
      if (showFftData.Checked)
        UpdateWaveFftChart();
      else
        UpdatePeakChart();
    }

    #region ChartUpdaters
    private void UpdateWaveFftChart()
    {
      var waves = data[ch].DebugWaves();

      for (int i = 0; i < waves.Length; i++)
      {
        var s = waveFftChart.Series[i].Points;
        s.Clear();
        if (hide_flag[i])
          continue;
        foreach (var v in waves[i])
        {
          s.AddY(v);
        }
      }
    }

    private void UpdatePeakChart()
    {
      var wave_peaks = waveChart.Series[3].Points;
      wave_peaks.Clear();

      for (int i = 0; i < targets.Length; i++)
      {
        if (int.TryParse(targets[i].Text, out int channel) &&
            double.TryParse(thresholds[i].Text, out double threshold) &&
            int.TryParse(required_lengths[i].Text, out int required_length))
        {
          var wave = data[channel];
          var finder = new PeakFinder(threshold, required_length);
          wave.OverSample = OverSampling;
          var peaks = finder.Apply(wave.OverSampled.ToArray());
          var x0 = wave.StartTime;
          var dx = wave.TimeIncrement / OverSampling;
          var s = peakChart.Series[i].Points;
          s.Clear();

          bool add_wave = ch == channel;
          if (AbsoluteTime.Enabled && AbsoluteTime.Checked)
          {
            var origin = wavefile.Time;
            foreach (var item in peaks)
            {
              var x = origin.AddSeconds(x0 + dx * item.Key);
              s.AddXY(x, item.Value);
              if (add_wave) wave_peaks.AddXY(x, item.Value);
            }
          }
          else
          {
            foreach (var item in peaks)
            {
              double x = x0 + dx * item.Key;
              s.AddXY(x, item.Value);
              if (add_wave) wave_peaks.AddXY(x, item.Value);
            }
          }
        }
      }
      // reset axes
      waveChart.ChartAreas[0].RecalculateAxesScale();
      peakChart.ChartAreas[0].AxisX.Minimum = waveChart.ChartAreas[0].AxisX.Minimum;
      peakChart.ChartAreas[0].AxisX.Maximum = waveChart.ChartAreas[0].AxisX.Maximum;
      peakChart.ChartAreas[0].AxisX.MajorGrid.Interval = waveChart.ChartAreas[0].AxisX.MajorGrid.Interval;
      peakChart.ChartAreas[0].AxisX.MajorTickMark.Interval = waveChart.ChartAreas[0].AxisX.MajorTickMark.Interval;
      peakChart.ChartAreas[0].AxisX.LabelStyle.Interval = waveChart.ChartAreas[0].AxisX.LabelStyle.Interval;
      peakChart.ChartAreas[0].RecalculateAxesScale();
    }

    private void UpdateWaveChart()
    {
      UpdateWaveChartSource();
      UpdateFilteredWaveChart();
      UpdateOverSampledWaveChart();
      waveChart.ChartAreas[0].RecalculateAxesScale();
      waveChart.Invalidate();
      waveChartMenu.Enabled = true;
      saveDisplayedMenu.Enabled = true;
      SaveAllDisplayedMenu.Enabled = true;
    }

    private void UpdateWaveChartSource()
    {
      data[ch].Update();
      if (!force_chart_update && source_id == data[ch].SourceId)
        return;

      var s = waveChart.Series[0].Points;
      source_id = data[ch].SourceId;

      int pos = dataStart.Value;
      num_point = Math.Min(NumberOfDisplayed, NumberOfData - pos - 1) / step;

      s.Clear();
      if (hideSource.Checked) return;
      var val = data[ch].Source;
      if (AbsoluteTime.Enabled && AbsoluteTime.Checked)
      {
        var origin = wavefile.Time;
        var x = data[ch].Xvalues.Select(v => origin.AddSeconds(v)).ToArray();
        for (int i = 0; i < NumberOfDisplayed; i += step)
        {
          s.AddXY(x[i], val[i]);
        }
      }
      else
      {
        var x = data[ch].Xvalues;

        for (int i = 0; i < NumberOfDisplayed; i += step)
        {
          s.AddXY(x[i], val[i]);
        }
      }
      double max = val.Max();
      double min = val.Min();
      double range = max - min;
      peakDisplay.Items[1] = string.Format("     max:{0:g3}", max);
      peakDisplay.Items[2] = string.Format("     min:{0:g3}", min);
      peakDisplay.Items[3] = string.Format("   range:{0:g3}", range);
      peakDisplay.Invalidate();
    }

    private void UpdateFilteredWaveChart()
    {
      if (!force_chart_update && filtered_id == data[ch].FilteredId)
        return;
      filtered_id = data[ch].FilteredId;

      var s = waveChart.Series[1].Points;
      s.Clear();
      if (hideResult.Checked || data[ch].Xvalues == null)
        return;
      var val = data[ch].Filtered;
      if (AbsoluteTime.Enabled && AbsoluteTime.Checked)
      {
        var origin = wavefile.Time;
        waveChart.Series[1].XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Time;
        var x = data[ch].Xvalues.Select(v => origin.AddSeconds(v)).ToArray();
        for (int i = 0; i < NumberOfDisplayed; i += step)
        {
          s.AddXY(x[i], val[i]);
        }
      }
      else
      {
        var x = data[ch].Xvalues;
        waveChart.Series[1].XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Double;
        for (int i = 0; i < NumberOfDisplayed; i += step)
        {
          s.AddXY(x[i], val[i]);
        }
      }
      double max = val.Max();
      double min = val.Min();
      double range = max - min;
      peakDisplay.Items[5] = string.Format("     max:{0:g3}", max);
      peakDisplay.Items[6] = string.Format("     min:{0:g3}", min);
      peakDisplay.Items[7] = string.Format("   range:{0:g3}", range);
      peakDisplay.Invalidate();
    }

    private void UpdateOverSampledWaveChart()
    {
      // skip update if id does not changed
      if (!force_chart_update && over_id == data[ch].OverId)
        return;

      over_id = data[ch].OverId;

      var s = waveChart.Series[2].Points;

      s.Clear();
      if (hideOver.Checked || data[ch].OverSampled == null || step > 1)
        return;

      double x0 = data[ch].StartTime;
      double dx = TimeIncrement / OverSampling;
      var val = data[ch].OverSampled;

      if (AbsoluteTime.Enabled && AbsoluteTime.Checked)
      {
        var origin = wavefile.Time;
        waveChart.Series[2].XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Time;
        var x = Enumerable.Range(0, val.Length).Select(i => origin.AddSeconds((x0 + dx * i))).ToArray();
        for (int i = 0; i < NumberOfDisplayed; i += step)
        {
          s.AddXY(x[i], val[i]);
        }
      }
      else
      {
        var x = Enumerable.Range(0, val.Length).Select(i => x0 + dx * i).ToArray();
        waveChart.Series[2].XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Double;
        for (int i = 0; i < NumberOfDisplayed; i += step)
        {
          s.AddXY(x[i], val[i]);
        }
      }
      double max = val.Max();
      double min = val.Min();
      double range = max - min;
      peakDisplay.Items[9] = string.Format("     max:{0:g3}", max);
      peakDisplay.Items[10] = string.Format("     min:{0:g3}", min);
      peakDisplay.Items[11] = string.Format("   range:{0:g3}", range);
      peakDisplay.Invalidate();
    }

    private void UpdateFilterChart()
    {
      if (factor_id == data[ch].FactorId)
        return;
      factor_id = data[ch].FactorId;

      var f = filterChart.Series[0].Points;
      var s = filterChart.Series[1].Points;
      f.Clear();
      s.Clear();
      for (int i = -Tap; i < Tap; i++)
      {
        f.Add(data[ch].Factor[Math.Abs(i)]);
        s.Add(data[ch].Window[i]);
      }
    }

    private void UpdateFrequencyChart()
    {
      UpdateFrequencyChartSource();
      UpdateFilderedFrequencyChart();
      UpdateFrequencyChartGain();
      if (chkShowPassBandOnly.Checked)
      {
        foreach (var ca in freqChart.ChartAreas)
        {
          ca.AxisX.Maximum = Math.Ceiling(double.Parse(upperCutOffFrequency.Text));
          ca.AxisX.Minimum = Math.Floor(double.Parse(lowerCutOffFrequency.Text));
        }
      }
    }

    /// <summary>
    ///  Update PowerSpectrum Series in Frequency Chart
    /// </summary>
    private void UpdateFrequencyChartSource()
    {
      var s = freqChart.Series[0].Points;
      var amps = data[ch].WaveSpectrumAmplitudeIn_dB();
      var x = data[ch].Freqs;
      int n = Nfft / 2;
      if (data[ch].Category == "DIS" || data[ch].Category == "VEL")
      {
        n -= 1;
      }
      s.Clear();
      for (int i = 1; i <= n; ++i)
      {
        if (!Double.IsInfinity(amps[i]))
          s.AddXY(x[i], amps[i]);
      }
    }

    private void UpdateFilderedFrequencyChart()
    {
      var s = freqChart.Series[1].Points;
      var amps = data[ch].Power(WaveDataSet.State.Filtered).Select(v => 20.0 * Math.Log10(v)).ToArray();
      var x = data[ch].Freqs;
      int n = Nfft / 2;
      if (data[ch].Category == "DIS" || data[ch].Category == "VEL")
      {
        n -= 1;
      }
      s.Clear();
      for (int i = 1; i <= n; ++i)
      {
        if (!Double.IsInfinity(amps[i]))
          s.AddXY(x[i], amps[i]);
      }
    }

    private void UpdateFrequencyChartGain()
    {
      if (gain_id == data[ch].GainId)
        return;
      gain_id = data[ch].GainId;

      var s = this.freqChart.Series[2].Points;
      var gains = data[ch].Gains.Take(Tap + 1).ToArray();

      s.Clear();
      for (int i = 0; i < Tap + 1; i++)
        s.AddXY(tap_freqs[i], gains[i]);
    }

    private void UpdateChartVisible()
    {
      waveFftChart.Visible = showFftData.Checked;
      peakChart.Visible = !showFftData.Checked;
    }
    #endregion

    private void ChannelChange()
    {
      ch = channelTrack.Value;

      channel.Text = ch.ToString();
      channelName.Text = wavefile.Name(ch);
      channelComment.Text = wavefile.Comment(ch);
      dataLength.Text = data[ch].Length.ToString();

      fs = 1.0 / TimeIncrement;
      samplingRate.Text = fs.ToString();
      samplingRate.ReadOnly = true;
      fn = fs / 2.0;
      nyquistFrequency.Text = fn.ToString();

      freqChart.ChartAreas[0].AxisX.Maximum = fn;
      UpdateDisplayedDataLength();
      dataStart.Maximum = NumberOfData - NumberOfDisplayed;
      // change data_start
      data[ch].DataStart = dataStart.Value;

      // update filter information
      var window_type = data[ch].WindowType; // save
      tapTrack.Value = data[ch].Tap;
      UpdateTapInfo();

      alpha.Text = data[ch].Alpha.ToString(); // filter's window type is chened to Kaiser automatically.

      lowerCutOffFrequencyTrack.Value = data[ch].Lower;
      UpdateLowerCutOffFrequency();
      upperCutOffFrequencyTrack.Value = data[ch].Upper;
      UpdateUpperCutOffFrequency();


      // switch to actual window_type
      switch (window_type)
      {
        case FIRFilter.WindowType.None:
        case FIRFilter.WindowType.Rectangle:
          rectangleWindow.Checked = true;
          break;
        case FIRFilter.WindowType.Han:
          hannWindow.Checked = true;
          break;
        case FIRFilter.WindowType.Hamming:
          hammingWindow.Checked = true;
          break;
        case FIRFilter.WindowType.Kaiser:
          kaiserWindow.Checked = true;
          break;
        case FIRFilter.WindowType.Blackman:
          blackmanWindow.Checked = true;
          break;
        default:
          throw new ApplicationException("Error in Window type");
      }
      lowerCutOffFrequencyTrack.Value = data[ch].Lower;
      UpdateLowerCutOffFrequency();
      upperCutOffFrequencyTrack.Value = data[ch].Upper;
      UpdateUpperCutOffFrequency();
      tapTrack.Value = data[ch].Tap;
      UpdateTapInfo();
      UpdateDisplayedDataLength();
      CheckUpdate();
    }

    private void UpdateTapInfo()
    {
      int tap = tapTrack.Value;

      if (lowerCutOffFrequencyTrack.Value > tap)
        lowerCutOffFrequencyTrack.Value = tap - 1;
      lowerCutOffFrequencyTrack.Maximum = tap - 1;
      if (upperCutOffFrequencyTrack.Value > tap)
        upperCutOffFrequencyTrack.Value = tap;
      upperCutOffFrequencyTrack.Maximum = tap;

      numberOfTap.Text = tap.ToString();
      int filter_size = tap * 2 - 1;
      filterLength.Text = filter_size.ToString();

      if (data != null)
        Fir.Tap = tap;

      double df = fs / tap / 2;
      tap_freqs = Enumerable.Range(0, tap + 1).Select(i => df * i).ToArray();
    }

    private void UpdateDisplayedDataLength()
    {
      if (int.TryParse(displayDataLength.Text, out int val))
      {
        if (val > NumberOfData || val > MAX_DISP_SIZE)
        {
          displayDataLength.Text = Math.Min(NumberOfData, MAX_DISP_SIZE).ToString();
          return;
        }
        NumberOfDisplayed = val;
        step = 1 + ((NumberOfDisplayed - 1) / max_points);
        dataStart.LargeChange = NumberOfDisplayed / 5;
        int largest = NumberOfData - NumberOfDisplayed;
        //if (data_start.Value > largest)
        //    data_start.Value = largest;
        dataStart.Maximum = largest;
      }
    }

    private void UpdateUpperCutOffFrequency()
    {
      upperValue.Text = upperCutOffFrequencyTrack.Value.ToString();
      upperCutOffFrequency.Text = (upperCutOffFrequencyTrack.Value * fn / Tap).ToString();
      if (upperCutOffFrequencyTrack.Value <= lowerCutOffFrequencyTrack.Value)
      {
        lowerCutOffFrequencyTrack.Value = upperCutOffFrequencyTrack.Value - 1;
      }
      if (data != null)
        Fir.Upper = upperCutOffFrequencyTrack.Value;
    }

    private void UpdateLowerCutOffFrequency()
    {
      lowerValue.Text = lowerCutOffFrequencyTrack.Value.ToString();
      lowerCutOffFrequency.Text = (lowerCutOffFrequencyTrack.Value * fn / Tap).ToString();
      if (lowerCutOffFrequencyTrack.Value >= upperCutOffFrequencyTrack.Value)
      {
        upperCutOffFrequencyTrack.Value = lowerCutOffFrequencyTrack.Value + 1;
      }
      if (data != null)
        Fir.Lower = lowerCutOffFrequencyTrack.Value;
    }

    private void UpdateGain()
    {
      if (double.TryParse(gain.Text, out double val))
      {
        Fir.Gain = val;
      }
    }
    #endregion
    #region Operations

    private void OpenFile(string file_name)
    {
      filePath.Text = file_name;
      var ext = System.IO.Path.GetExtension(file_name).ToUpper();
      if (ext == ".DAT")
      {
        if (Famos.IsFamos(file_name))
        {
          wavefile = new Famos(file_name);
          if (wavefile != null)
            AbsoluteTime.Enabled = true;
        }
        else
        {
          //throw new NotImplementedException("Only famos format file is supported.");
          MessageBox.Show("Only famos format file is supported for .DAT files.");
          return;
        }
      }
      if (ext == ".CSV")
      {
        if (DelimFile.IsKyowaCsv(file_name))
        {
          wavefile = DelimFile.OpenKyowaCsv(file_name);
          if (wavefile != null)
            AbsoluteTime.Enabled = true;
        }
        else
        {
          wavefile = DelimFile.OpenGeneralCsv(file_name);
          if (wavefile != null)
            AbsoluteTime.Enabled = false;
        }
      }

      if (wavefile == null)
      {
        MessageBox.Show("DAT and CSV are supported format");
        return;
        //throw new NotImplementedException("DAT and CSV are supported format");
      }
      progressBar1.Value = 0;
      progressBar1.Visible = true;
      if (!wavefile.Opened)
        return;
      progressBar1.Maximum = wavefile.Cols + 2;
      progressBar1.Value = 1;

      var default_window_type = FIRFilter.WindowType.None;
      double.TryParse(alpha.Text, out double a);
      if (rectangleWindow.Checked) default_window_type = FIRFilter.WindowType.Rectangle;
      if (hannWindow.Checked) default_window_type = FIRFilter.WindowType.Han;
      if (hammingWindow.Checked) default_window_type = FIRFilter.WindowType.Hamming;
      if (blackmanWindow.Checked) default_window_type = FIRFilter.WindowType.Blackman;
      if (kaiserWindow.Checked) default_window_type = FIRFilter.WindowType.Kaiser;

      data = new List<WaveDataSet>(wavefile.Cols);

      for (int i = 0; i < wavefile.Cols; i++)
      {
        progressBar1.Value = i + 2;
        var wave = new WaveDataSet(wavefile, i);
        data.Add(wave);
        wave.Tap = tapTrack.Value;
        wave.Lower = lowerCutOffFrequencyTrack.Value;
        wave.Upper = upperCutOffFrequencyTrack.Value;
        wave.Alpha = a;
        wave.WindowType = default_window_type;

        wave.Gain = -80.0;
      }
      progressBar1.Value = progressBar1.Maximum;

      // reset combobox
      foreach (var item in targets)
      {
        item.Text = "";
        item.Items.Clear();
        for (int i = 0; i < wavefile.Cols; i++)
        {
          item.Items.Add(i.ToString());
        }
      }

      channelTrack.Value = 0;
      channelTrack.Maximum = wavefile.Cols - 1;
      axes_ranges = new AxisRanges[wavefile.Cols];
      for (int i = 0; i < wavefile.Cols; i++)
      {
        // NaN means automatic
        axes_ranges[i].wave.max = axes_ranges[i].wave.min = double.NaN;
        //axes_ranges[i].peak_P.max = axes_ranges[i].peak_P.min = double.NaN;
        //axes_ranges[i].peak_a.max = axes_ranges[i].peak_a.min = double.NaN;
      }

      UpdateTapInfo();

      ChannelChange();

      progressBar1.Visible = false;

      umi.Enabled = true;
      yama.Enabled = true;
      len_0_1sec.Enabled = true;
      len_0_5sec.Enabled = true;
      len_1sec.Enabled = true;
      len_5sec.Enabled = true;

      freqChart.ChartAreas[0].RecalculateAxesScale();
      waveChart.ChartAreas[0].RecalculateAxesScale();
      filterChart.ChartAreas[0].RecalculateAxesScale();
      peakChart.ChartAreas[0].RecalculateAxesScale();
    }

    private void SetAlpha(string text)
    {
      if (double.TryParse(text, out double val))
      {
        Fir.Alpha = val;
      }
    }

    private string FindChannel(string in_name)
    {
      for (int i = 0; i < wavefile.Cols; i++)
      {
        if (wavefile.Name(i).Contains(in_name))
          return i.ToString();
      }
      return "";
    }

    void SearchChannels(string side)
    {
      ch_P1.Text = FindChannel(side + "_P1").ToString();
      ch_P2.Text = FindChannel(side + "_P2").ToString();
      ch_Ya.Text = FindChannel(side + "_Ya").ToString();
      ch_Za.Text = FindChannel(side + "_Za").ToString();
    }

    private void SetDataLengthByDuration(double dur)
    {
      int count = (int)(dur / TimeIncrement);
      displayDataLength.Text = count.ToString();
    }

    private void SaveWave(int idx)
    {
      if (saveFileDialogCSV.ShowDialog() == System.Windows.Forms.DialogResult.Cancel)
        return;

      using (var stream = saveFileDialogCSV.OpenFile())
      {
        using (var writer = new System.IO.StreamWriter(stream, Encoding.Default))
        {
          writer.WriteLine("Original File, {0}", filePath.Text);
          writer.WriteLine("Data Start, {0}", dataStart.Value);
          writer.WriteLine("Data Length, {0}", dataLength.Text);
          writer.WriteLine("Sampling Rate, {0}", fs);
          writer.WriteLine("Ch, {0}", ch);
          writer.WriteLine("Channel name, {0}", channelName.Text);
          writer.WriteLine("Comment of channel, {0}", channelComment.Text);
          writer.WriteLine("");
          writer.WriteLine("time, {0} Wave", waveChart.Series[idx].Name);

          // Setup wave data
          double[] x = data[ch].Xvalues;
          double[] y;
          switch (idx)
          {
            case 0:
              y = data[ch].Source;
              break;
            case 1:
              y = data[ch].Filtered;
              break;
            case 2:
              y = data[ch].OverSampled;
              break;
            case 3:
              y = waveChart.Series[3].Points.Select(v => v.YValues[0]).ToArray();
              x = waveChart.Series[3].Points.Select(v => v.XValue).ToArray();
              break;
            default:
              throw new ArgumentException("index must be in 0 to 3");
          }
          for (int i = 0; i < x.Length; i++)
          {
            writer.WriteLine("{0},{1}", x[i], y[i]);
          }
        }
      }
    }

    private void SaveAllWaves(int idx)
    {
      if (saveFileDialogCSV.ShowDialog() == System.Windows.Forms.DialogResult.Cancel)
        return;

      // need to update data start for all channnels
      System.Threading.Tasks.Parallel.ForEach(data, (d) => { d.DataStart = dataStart.Value; d.Update(); });

      using (var stream = saveFileDialogCSV.OpenFile())
      {
        using (var writer = new System.IO.StreamWriter(stream, Encoding.Default))
        {
          writer.WriteLine("Original File, {0}", filePath.Text);
          writer.WriteLine("Data Start, {0}", dataStart.Value);
          writer.WriteLine("Data Length, {0}", dataLength.Text);
          writer.WriteLine("Sampling Rate, {0}", fs);
          var chs = Enumerable.Range(0, data.Count);
          writer.WriteLine("Ch, {0}", string.Join(",", chs));
          writer.WriteLine("Channel name, {0}", string.Join(",", chs.Select(i => wavefile.Name(i))));
          writer.WriteLine("Comment of channel, {0}", string.Join(",", chs.Select(i => wavefile.Comment(i))));
          writer.WriteLine("Wave Type, {0}", waveChart.Series[idx].Name);
          writer.WriteLine("");
          writer.WriteLine("time, {0}", string.Join(",", chs.Select(i => "Ch" + i.ToString())));

          // Setup wave data
          double[] x = data[0].Xvalues;
          IEnumerable<double[]> y;
          switch (idx)
          {
            case 0:
              y = data.Select(v => v.Source);
              break;
            case 1:
              y = data.Select(v => v.Filtered);
              break;
            case 2:
              y = data.Select(v => v.OverSampled);
              break;
            default:
              throw new ArgumentException("index must be in 0 to 2");
          }
          for (int i = 0; i < x.Length; i++)
          {
            writer.WriteLine("{0},{1}", x[i], string.Join(",", y.Select(v => v[i])));
          }
        }
      }
    }

    private void SaveAllPeaks()
    {
      if (saveFileDialogCSV.ShowDialog() == System.Windows.Forms.DialogResult.Cancel)
        return;

      using (var stream = saveFileDialogCSV.OpenFile())
      {
        using (var writer = new System.IO.StreamWriter(stream, Encoding.Default))
        {
          writer.WriteLine("Original File, {0}", filePath.Text);
          writer.WriteLine("Source Data Start, {0}", dataStart.Value);
          writer.WriteLine("Source Data Length, {0}", dataLength.Text);
          writer.WriteLine("Sampling Rate of Original Data, {0}", fs);
          var chs = targets.Select(t => int.Parse(t.Text));
          writer.WriteLine("Ch, {0}", string.Join(",", chs));
          writer.WriteLine("Channel name, {0}", string.Join(",", chs.Select(i => wavefile.Name(i))));
          writer.WriteLine("Comment of channel, {0}", string.Join(",", chs.Select(i => wavefile.Comment(i))));
          writer.WriteLine("Wave Type, Peaks");
          var peak_counts = peakChart.Series.Select(s => s.Points.Count).ToArray();
          writer.WriteLine("Number of Peaks, {0}", string.Join(",", peak_counts));
          writer.WriteLine("");
          writer.WriteLine("time, P1, time, P2, time, Ya, time, Za");
          var len = peak_counts.Max();
          var res = peak_counts.Select((n, i) =>
              peakChart.Series[i].Points
                  .Select(p => string.Format("{0},{1}", p.XValue, p.YValues[0]))
                  .Concat(Enumerable.Repeat(",", len - n)).ToArray());
          for (int i = 0; i < peak_counts.Max(); i++)
          {
            writer.WriteLine("{0}", string.Join(",", res.Select(a => a[i])));
          }
        }
      }
    }

    private void SavePeaks(int idx)
    {
      if (saveFileDialogCSV.ShowDialog() == System.Windows.Forms.DialogResult.Cancel)
        return;
      using (var stream = saveFileDialogCSV.OpenFile())
      {
        using (var writer = new System.IO.StreamWriter(stream, Encoding.Default))
        {
          writer.WriteLine("Original File, {0}", filePath.Text);
          writer.WriteLine("Source Data Start, {0}", dataStart.Value);
          writer.WriteLine("Source Data Length, {0}", dataLength.Text);
          writer.WriteLine("Sampling Rate of Original Data, {0}", fs);
          var target_ch = int.Parse(targets[idx].Text);
          writer.WriteLine("Ch, {0}", targets[idx].Text);
          writer.WriteLine("Channel name, {0}", wavefile.Name(target_ch));
          writer.WriteLine("Comment of channel, {0}", wavefile.Comment(target_ch));
          writer.WriteLine("Wave Type, Peaks");
          writer.WriteLine("Number of Peaks, {0}", peakChart.Series[idx].Points.Count);
          writer.WriteLine("");
          writer.WriteLine("time, {0}", targets[idx].Name.Substring(3));
          foreach (var item in peakChart.Series[idx].Points)
          {
            writer.WriteLine("{0},{1}", item.XValue, item.YValues[0]);
          }
        }
      }
    }

    private enum FreqSeriesType
    {
      Source = 0,
      Filtered = 1,
      Gain = 2,
    }

    private void SaveSpectrum(FreqSeriesType type)
    {
      if (saveFileDialogCSV.ShowDialog() == System.Windows.Forms.DialogResult.Cancel)
        return;

      using (var stream = saveFileDialogCSV.OpenFile())
      {
        using (var writer = new System.IO.StreamWriter(stream, Encoding.Default))
        {
          writer.WriteLine("Original File, {0}", filePath.Text);
          writer.WriteLine("Data Start, {0}", dataStart.Value);
          writer.WriteLine("Data Length, {0}", dataLength.Text);
          writer.WriteLine("Sampling Rate, {0}", fs);
          writer.WriteLine("Ch, {0}", ch);
          writer.WriteLine("Channel name, {0}", channelName.Text);
          writer.WriteLine("Comment of channel, {0}", channelComment.Text);
          if (type == FreqSeriesType.Filtered)
          {
            writer.WriteLine("Tap, {0}", Fir.Tap);
            writer.WriteLine("Upper Fc, {1}, ({0} Hz)", upperCutOffFrequency.Text, upperValue.Text);
            writer.WriteLine("Lower Fc, {1}, ({0} Hz)", lowerCutOffFrequency.Text, lowerValue.Text);
            writer.WriteLine("Stop band gain, {0}", gain.Text);
            writer.Write("Filter type, {0}", Fir.WindowType.ToString());
            if (Fir.WindowType == FIRFilter.WindowType.Kaiser)
              writer.WriteLine(",{}", Fir.Alpha);
            else
              writer.WriteLine();
          }
          writer.WriteLine("");

          // Setup wave data
          double[] x = data[ch].Freqs;
          double[] y;
          switch (type)
          {
            case FreqSeriesType.Source:
              y = data[ch].WaveSpectrumAmplitudeIn_dB();
              writer.WriteLine("Frequency, Source");
              break;
            case FreqSeriesType.Filtered:
              y = data[ch].Power(WaveDataSet.State.Filtered).ToArray();
              writer.WriteLine("Frequency, Filtered");
              break;
            case FreqSeriesType.Gain:
              y = data[ch].Gains;
              writer.WriteLine("Frequency, Gain (dB)");
              break;
            default:
              throw new ArgumentException("Bug: index must be FreqSeriesType");
          }
          for (int i = 0; i < x.Length; i++)
          {
            writer.WriteLine("{0},{1}", x[i], y[i]);
          }
        }
      }
    }


    #endregion // Operations
    #endregion // PrivateMethods
    #endregion // Internal Use

    #region EventHandler

    private void TapTrack_Scroll(object sender, EventArgs e)
    {
      UpdateTapInfo();
      CheckUpdate();
    }

    private void UpperCutOffFrecquencyTrack_Scroll(object sender, EventArgs e)
    {
      UpdateUpperCutOffFrequency();
      CheckUpdate();
    }


    private void LowerCutOffFrequencyTrack_Scroll(object sender, EventArgs e)
    {
      UpdateLowerCutOffFrequency();
      CheckUpdate();
    }


    private void Update_Click(object sender, EventArgs e)
    {
      DataUpdate();
    }

    private void ChannelTrack_Scroll(object sender, EventArgs e)
    {
      ChannelChange();
      waveChart.ChartAreas[0].AxisY.Maximum = axes_ranges[ch].wave.max;
      waveChart.ChartAreas[0].AxisY.Minimum = axes_ranges[ch].wave.min;
      //peak_chart.ChartAreas[0].AxisY.Maximum = axes_ranges[ch].peak_P.max;
      //peak_chart.ChartAreas[0].AxisY.Minimum = axes_ranges[ch].peak_P.min;
      //peak_chart.ChartAreas[0].AxisY2.Maximum = axes_ranges[ch].peak_a.max;
      //peak_chart.ChartAreas[0].AxisY2.Minimum = axes_ranges[ch].peak_a.min;
      CheckUpdate();
    }

    private void SelectFile_Click(object sender, EventArgs e)
    {
      if (openFamosDialog.ShowDialog() == System.Windows.Forms.DialogResult.Cancel)
        return;

      OpenFile(openFamosDialog.FileName);
    }


    private void DisplayDataLength_TextChanged(object sender, EventArgs e)
    {
      UpdateDisplayedDataLength();
      CheckUpdate();
    }


    private void KaiserWindow_CheckedChanged(object sender, EventArgs e)
    {
      SetAlpha(alpha.Text);
      CheckUpdate();
    }

    private void Alpha_TextChanged(object sender, EventArgs e)
    {
      if (kaiserWindow.Checked)
        SetAlpha(alpha.Text);
      CheckUpdate();
    }

    private void RectangleWindow_CheckedChanged(object sender, EventArgs e)
    {
      Fir.WindowType = FIRFilter.WindowType.Rectangle;
      CheckUpdate();
    }

    private void BlackmanWindow_CheckedChanged(object sender, EventArgs e)
    {
      Fir.WindowType = FIRFilter.WindowType.Blackman;
      CheckUpdate();
    }

    private void HannWindow_CheckedChanged(object sender, EventArgs e)
    {
      Fir.WindowType = FIRFilter.WindowType.Han;
      CheckUpdate();
    }

    private void HammingWindow_CheckedChanged(object sender, EventArgs e)
    {
      Fir.WindowType = FIRFilter.WindowType.Hamming;
      CheckUpdate();
    }

    private void DataStart_ValueChanged(object sender, EventArgs e)
    {
      DataStart.Text = dataStart.Value.ToString();
      if (data != null)
        data[ch].DataStart = dataStart.Value;
      foreach (var item in targets)
      {
        if (int.TryParse(item.Text, out int i))
        {
          data[i].DataStart = dataStart.Value;
        }
      }
      CheckUpdate();
    }

    private void Gain_TextChanged(object sender, EventArgs e)
    {
      UpdateGain();
    }

    private void Umi_Click(object sender, EventArgs e)
    {
      bool state = autoUpdate.Checked;
      autoUpdate.Checked = false;
      SearchChannels("UMI");
      autoUpdate.Checked = state;
      DataStart_ValueChanged(sender, e);
      CheckUpdate();
    }

    private void Yama_Click(object sender, EventArgs e)
    {
      bool state = autoUpdate.Checked;
      autoUpdate.Checked = false;
      SearchChannels("YAMA");
      autoUpdate.Checked = state;
      DataStart_ValueChanged(sender, e);
      CheckUpdate();
    }

    private void Len_0_1sec_Click(object sender, EventArgs e)
    {
      SetDataLengthByDuration(0.1);
      CheckUpdate();
    }


    private void Len_0_5sec_Click(object sender, EventArgs e)
    {
      SetDataLengthByDuration(0.5);
      CheckUpdate();
    }

    private void Len_1sec_Click(object sender, EventArgs e)
    {
      SetDataLengthByDuration(1.0);
      CheckUpdate();
    }

    private void Len_5sec_Click(object sender, EventArgs e)
    {
      SetDataLengthByDuration(5.0);
      CheckUpdate();
    }

    private void SamplingRate_TextChanged(object sender, EventArgs e)
    {
      if (!samplingRate.ReadOnly)
      {
        if (double.TryParse(samplingRate.Text, out fs))
        {
          fn = (fs / 2.0);
          nyquistFrequency.Text = fn.ToString();
          UpdateUpperCutOffFrequency();
          UpdateLowerCutOffFrequency();
        }
      }
    }
    private void OverSampling1_CheckedChanged(object sender, EventArgs e)
    {
      OverSampling = 1;
      CheckUpdate();
    }

    private void OverSampling2_CheckedChanged(object sender, EventArgs e)
    {
      OverSampling = 2;
      CheckUpdate();
    }

    private void OverSampling4_CheckedChanged(object sender, EventArgs e)
    {
      OverSampling = 4;
      CheckUpdate();
    }

    private void OverSampling8_CheckedChanged(object sender, EventArgs e)
    {
      OverSampling = 8;
      CheckUpdate();
    }

    private void CheckUpdateHandler(object sende, EventArgs e)
    {
      CheckUpdate();
    }


    private void Integ_SelectedIndexChanged(object sender, EventArgs e)
    {
      if (data == null)
        return;
      foreach (var item in data)
      {
        item.Category = integ.Text;
      }
      CheckUpdate();
    }


    private void HideOver_CheckedChanged(object sender, EventArgs e)
    {
      hide_flag[HIDE_OVER] = hideOver.Checked;
      over_id = 0;
      UpdateOverSampledWaveChart();
      waveChart.Invalidate();
    }

    private void HideResult_CheckedChanged(object sender, EventArgs e)
    {
      hide_flag[HIDE_ANS] = hideResult.Checked;
      filtered_id = 0;
      UpdateFilteredWaveChart();
      waveChart.Invalidate();
    }

    private void HideSource_CheckedChanged(object sender, EventArgs e)
    {
      hide_flag[HIDE_WAVE] = hideSource.Checked;
      source_id = 0;
      UpdateWaveChartSource();
      waveChart.Invalidate();
    }

    private void ShowFftData_CheckedChanged(object sender, EventArgs e)
    {
      UpdateChartVisible();
      CheckUpdate();
    }


    private void DataStart_Validated(object sender, EventArgs e)
    {
      dataStart.Value = int.Parse(DataStart.Text);
    }

    private void DataStart_Validating(object sender, CancelEventArgs e)
    {
      if (!int.TryParse(DataStart.Text, out int val))
      {
        e.Cancel = true;
        return;
      }
      if (val < 0) DataStart.Text = "0";
      if (val > dataStart.Maximum) DataStart.Text = dataStart.Maximum.ToString();
    }
    #endregion

    //#region saveWaveHandlers
    private void SourceWaveToolStripMenuItem_Click(object sender, EventArgs e)
    {
      SaveWave(0);
    }
    private void SaveDisplayedFilteredWave_Click(object sender, EventArgs e)
    {
      SaveWave(1);
    }

    private void SaveDisplayedOverWave_Click(object sender, EventArgs e)
    {
      SaveWave(2);
    }

    private void PeakWaveToolStripMenuItem_Click(object sender, EventArgs e)
    {
      SaveWave(3);
    }

    private void SaveAllDisplayedSourceWaves_Click(object sender, EventArgs e)
    {
      SaveAllWaves(0);
    }

    private void SaveAllDisplayedFilteredWaves_Click(object sender, EventArgs e)
    {
      SaveAllWaves(1);
    }

    private void SaveAllDisplayedOversampledWaves_Click(object sender, EventArgs e)
    {
      SaveAllWaves(2);
    }

    private void ChkShowPassBandOnly_CheckedChanged(object sender, EventArgs e)
    {
      UpdateFrequencyChart();
    }

    private void BtnExpandFilterSettings_Click(object sender, EventArgs e)
    {
      var curr = data[ch];
      for (int i = 0; i < data.Count; i++)
      {
        if (i == ch) continue;
        var tgt = data[i];
        tgt.Tap = curr.Tap;
        tgt.Upper = curr.Upper;
        tgt.Lower = curr.Lower;
        tgt.Alpha = curr.Alpha;
        tgt.WindowType = curr.WindowType;
        tgt.Gain = curr.Gain;
        tgt.DataStart += 1;
        tgt.DataStart = curr.DataStart;
        tgt.Update();
      }
    }

    private void DataGridView1_Validated(object sender, EventArgs e)
    {
      if (data != null)
      {
        foreach (var item in data)
        {
          item.Notch = this.notchFilterInfo.Notches;
        }
        CheckUpdate();
      }
    }

    private void ChartRange_Enter(object o_sender, EventArgs e)
    {
      TextBox sender = (TextBox)o_sender;
      sender.Width = 80;

      var ax = (System.Windows.Forms.DataVisualization.Charting.Axis)sender.Tag;
      if (sender.Name.Contains("max"))
      {
        sender.Text = ax.Maximum.ToString();
      }
      else
      {
        sender.Text = ax.Minimum.ToString();
      }

    }

    private void ChartRange_Changed(object o_sender, EventArgs e)
    {
      TextBox sender = (TextBox)o_sender;
      var ax = (System.Windows.Forms.DataVisualization.Charting.Axis)sender.Tag;
      if (sender.Text == "")
      {
        if (sender.Width == 20)
          return;
        if (sender.Name.Contains("max"))
          ax.Maximum = double.NaN;
        else
          ax.Minimum = double.NaN;
      }
      else if (double.TryParse(sender.Text, out double val))
      {
        if (sender.Name.Contains("max"))
        {
          if (val > ax.Minimum)
            ax.Maximum = val;
        }
        else
        {
          if (val < ax.Maximum)
            ax.Minimum = val;
        }
      }
    }
    private void ChartRange_Leave(object o_sender, EventArgs e)
    {
      TextBox sender = (TextBox)o_sender;
      sender.Width = 20;
      sender.Text = "";
    }

    private void ChannelTrack_Enter(object sender, EventArgs e)
    {
      if (axes_ranges != null)
      {
        axes_ranges[ch].wave.max = waveChart.ChartAreas[0].AxisY.Maximum;
        axes_ranges[ch].wave.min = waveChart.ChartAreas[0].AxisY.Minimum;
        //axes_ranges[ch].peak_P.max = peak_chart.ChartAreas[0].AxisY.Maximum;
        //axes_ranges[ch].peak_P.min = peak_chart.ChartAreas[0].AxisY.Minimum;
        //axes_ranges[ch].peak_a.max = peak_chart.ChartAreas[0].AxisY2.Maximum;
        //axes_ranges[ch].peak_a.min = peak_chart.ChartAreas[0].AxisY2.Minimum;
      }
    }

    private void AbsoluteTime_CheckedChanged(object sender, EventArgs e)
    {

      System.Windows.Forms.DataVisualization.Charting.ChartValueType val
          = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Double;
      if (AbsoluteTime.Checked)
        val = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Time;
      foreach (var item in peakChart.Series)
      {
        item.XValueType = val;
      }
      foreach (var item in waveChart.Series)
      {
        item.XValueType = val;
      }
      if (AbsoluteTime.Checked)
      {
        peakChart.ChartAreas[0].AxisX.LabelStyle.Format = "hh:mm:ss.FFF";
        waveChart.ChartAreas[0].AxisX.LabelStyle.Format = "hh:mm:ss.FFF";
      }
      else
      {
        peakChart.ChartAreas[0].AxisX.LabelStyle.Format = "g";
        var ax = waveChart.ChartAreas[0].AxisX;
        ax.LabelStyle.Format = "g";
        ax.IntervalAutoMode = System.Windows.Forms.DataVisualization.Charting.IntervalAutoMode.FixedCount;
        ax.IntervalType = System.Windows.Forms.DataVisualization.Charting.DateTimeIntervalType.Auto;
      }
      force_chart_update = true;
      UpdateWaveChart();
      UpdatePeakChart();
      waveChart.ChartAreas[0].RecalculateAxesScale();
      peakChart.ChartAreas[0].RecalculateAxesScale();
      force_chart_update = false;
    }

    private void CopyRangeToAllCh_Click(object sender, EventArgs e)
    {
      var max = waveChart.ChartAreas[0].AxisY.Maximum;
      var min = waveChart.ChartAreas[0].AxisY.Minimum;
      for (int i = 0; i < data.Count; i++)
      {
        axes_ranges[i].wave.max = max;
        axes_ranges[i].wave.min = min;
      }
    }

    private void SaveAllPeaks_Click(object sender, EventArgs e)
    {
      SaveAllPeaks();
    }

    private void SaveP1Peaks_Click(object sender, EventArgs e)
    {
      SavePeaks(0);
    }

    private void SaveP2Peaks_Click(object sender, EventArgs e)
    {
      SavePeaks(1);
    }

    private void SaveYaPeaks_Click(object sender, EventArgs e)
    {
      SavePeaks(2);
    }

    private void SaveZaPeaks_Click(object sender, EventArgs e)
    {
      SavePeaks(3);
    }

    private void NumberOfTap_Validated(object sender, EventArgs e)
    {
      if (int.TryParse(numberOfTap.Text, out int val))
      {
        if (val > 0)
        {
          bool domax = false;
          if (upperCutOffFrequencyTrack.Value == upperCutOffFrequencyTrack.Maximum)
            domax = true;
          if (val > tapTrack.Maximum)
          {
            tapTrack.Maximum = val;
            upperCutOffFrequencyTrack.Maximum = val;
            lowerCutOffFrequencyTrack.Maximum = val;
            int i = val / 10;
            tapTrack.TickFrequency = i;
            upperCutOffFrequencyTrack.TickFrequency = i;
            lowerCutOffFrequencyTrack.TickFrequency = i;
          }
          tapTrack.Value = val;
          UpdateTapInfo(); // update
          if (domax)
            upperCutOffFrequencyTrack.Value = upperCutOffFrequencyTrack.Maximum;
        }
      }
    }

    private void ResetAxes_Click(object sender, EventArgs e)
    {
      freqChart.ChartAreas[0].RecalculateAxesScale();
    }

    private void MenuSpSaveSource_Click(object sender, EventArgs e)
    {
      SaveSpectrum(FreqSeriesType.Source);
    }

    private void MenuSpSaveFiltered_Click(object sender, EventArgs e)
    {
      SaveSpectrum(FreqSeriesType.Filtered);
    }

    private void MenuSpSaveGain_Click(object sender, EventArgs e)
    {
      SaveSpectrum(FreqSeriesType.Gain);
    }

    private void UpperVal_Validating(object sender, CancelEventArgs e)
    {
      e.Cancel = true;
      if (int.TryParse(upperValue.Text, out int i))
      {
        if (i <= tapTrack.Value && i > lowerCutOffFrequencyTrack.Value)
        {
          e.Cancel = false;
        }
      }
    }

    private void UpperVal_Validated(object sender, EventArgs e)
    {
      upperCutOffFrequencyTrack.Value = int.Parse(upperValue.Text);
    }

    private void LowerVal_Validated(object sender, EventArgs e)
    {
      lowerCutOffFrequencyTrack.Value = int.Parse(lowerValue.Text);
    }

    private void LowerVal_Validating(object sender, CancelEventArgs e)
    {
      e.Cancel = true;
      if (int.TryParse(lowerValue.Text, out int i))
      {
        if (i >= 0 && i < upperCutOffFrequencyTrack.Value)
        {
          e.Cancel = false;
        }
      }
    }

  }
}
