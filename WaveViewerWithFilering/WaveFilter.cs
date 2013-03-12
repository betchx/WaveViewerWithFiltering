using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using fftwlib;
using WaveFile;

namespace WaveViewerWithFilering
{
    public partial class WaveFilter : Form
    {
        public WaveFilter()
        {
            InitializeComponent();
            data = null;
            update_tap_info();
            upper_fc_track.Value = tap_track.Value;
            lower_fc_track.Value = 0;
            sampling_rate.Text = "1000.0";
            display_data_length.Text = "100";
            targets = new ComboBox[] { ch_P1, ch_P2, ch_Ya, ch_Za };
            thresholds = new TextBox[] { th_P1, th_P2, th_Ya, th_Za };
            required_lengths = new TextBox[] { rl_P1, rl_P2, rl_Ya, rl_Za };
            over_sampling_ = 1;
            integ.Text = "ACC";
            update_chart_visible();
            hide_flag = new bool[] { false, false, false, false, false};
            factor_id = 0u;
            filtered_id = 0u;
            gain_id = 0u;
            source_id = 0u;
            over_id = 0u;
            peak_chart_Primary_max.Tag = peak_chart.ChartAreas[0].AxisY;
            peak_chart_Primary_min.Tag = peak_chart.ChartAreas[0].AxisY;
            peak_chart_Secondary_max.Tag = peak_chart.ChartAreas[0].AxisY2;
            peak_chart_Secondary_min.Tag = peak_chart.ChartAreas[0].AxisY2;
            wave_chart_max.Tag = wave_chart.ChartAreas[0].AxisY;
            wave_chart_min.Tag = wave_chart.ChartAreas[0].AxisY;
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
        int num_point;
        int step;
        const int max_points = 10000;
        private double fs;
        private double fn;
        private IWaveFile wavefile;
        bool[] hide_flag;
        const int HIDE_WAVE = 0;
        const int HIDE_ANS = 1;
        const int HIDE_OVER = 2;
        ComboBox[] targets;
        TextBox[] thresholds;
        TextBox[] required_lengths;
        private int over_sampling_;
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
        private int nfft { get { return data[ch].nfft; } }
        private int tap { get { return tap_track.Value; } }
        private WaveDataSet fir { get { return data[ch]; } }// fir setting affects only active data set.

        private int over_sampling
        {
            get { return over_sampling_; }
            set
            {
                if (over_sampling_ != value)
                {
                    over_sampling_ = value;
                    if (data != null)
                        System.Threading.Tasks.Parallel.ForEach(data, (wd) =>
                        {
                            wd.over_sample = value;
                        });
                }
            }
        }

        private int num_disp
        {
            get
            {
                if (data == null) return 0;
                return data[ch].num_disp;
            }
            set
            {
                if (data != null)
                    System.Threading.Tasks.Parallel.ForEach(data, (d) => d.num_disp = value);
            }
        }
        private int num_data
        {
            get
            {
                if (data == null) return 0;
                return data[ch].data.Length;
            }
        }

        private double dt { get { return data[ch].dt; } }
        #endregion
        #region PrivateMethods
        #region UpdateMethods

        private void CheckUpdate()
        {
            if (auto_update.Checked)
                data_update();
        }

        private void data_update()
        {
            if (wavefile == null)
                return;

            System.Threading.Tasks.Parallel.ForEach(data, d => d.update());

            update_wave_chart();
            update_filter_chart();
            update_freq_chart();
            if (show_fft_data.Checked)
                update_wave_fft_chart();
            else
                update_peak_chart();
        }

        #region ChartUpdaters
        private void update_wave_fft_chart()
        {
            var waves = data[ch].debug_waves();

            for (int i = 0; i < waves.Length; i++)
            {
                var s = wave_fft_chart.Series[i].Points;
                s.Clear();
                if (hide_flag[i])
                    continue;
                foreach (var v in waves[i])
                {
                    s.AddY(v);
                }
            }
        }

        private void update_peak_chart()
        {
            int channel;

            int required_length;
            double threshold;

            var wave_peaks = wave_chart.Series[3].Points;
            wave_peaks.Clear();

            for (int i = 0; i < targets.Length; i++)
            {
                if (int.TryParse(targets[i].Text, out channel) &&
                    double.TryParse(thresholds[i].Text, out threshold) &&
                    int.TryParse(required_lengths[i].Text, out required_length))
                {
                    var wave = data[channel];
                    var finder = new PeakFinder(threshold, required_length);
                    wave.over_sample = over_sampling;
                    var peaks = finder.apply(wave.over_sampled.ToArray());
                    var x0 = wave.x0;
                    var dx = wave.dt / over_sampling;
                    var s = peak_chart.Series[i].Points;
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
            wave_chart.ChartAreas[0].RecalculateAxesScale();
            peak_chart.ChartAreas[0].AxisX.Minimum = wave_chart.ChartAreas[0].AxisX.Minimum;
            peak_chart.ChartAreas[0].AxisX.Maximum = wave_chart.ChartAreas[0].AxisX.Maximum;
            peak_chart.ChartAreas[0].AxisX.MajorGrid.Interval = wave_chart.ChartAreas[0].AxisX.MajorGrid.Interval;
            peak_chart.ChartAreas[0].AxisX.MajorTickMark.Interval = wave_chart.ChartAreas[0].AxisX.MajorTickMark.Interval;
            peak_chart.ChartAreas[0].AxisX.LabelStyle.Interval = wave_chart.ChartAreas[0].AxisX.LabelStyle.Interval;
            peak_chart.ChartAreas[0].RecalculateAxesScale();
        }

        private void update_wave_chart()
        {
            update_wave_chart_source();
            update_wave_chart_filtered();
            update_wave_chart_oversampled();
            wave_chart.ChartAreas[0].RecalculateAxesScale();
            wave_chart.Invalidate();
            waveChartMenu.Enabled = true;
            saveDisplayedMenu.Enabled = true;
            SaveAllDisplayedMenu.Enabled = true;
        }

        private void update_wave_chart_source()
        {
            data[ch].update();
            if (!force_chart_update && source_id == data[ch].source_id)
                return;

            var s = wave_chart.Series[0].Points;
            source_id = data[ch].source_id;

            int pos = data_start.Value;
            num_point = Math.Min(num_disp, num_data - pos - 1) / step;

            s.Clear();
            if (hide_source.Checked) return;
            var val = data[ch].source;
            if (AbsoluteTime.Enabled && AbsoluteTime.Checked)
            {
                var origin = wavefile.Time;
                var x = data[ch].xvalues.Select(v => origin.AddSeconds(v)).ToArray();
                for (int i = 0; i < num_disp; i += step)
                {
                    s.AddXY(x[i], val[i]);
                }
            }
            else
            {
                var x = data[ch].xvalues;
                
                for (int i = 0; i < num_disp; i += step)
                {
                    s.AddXY(x[i], val[i]);
                }
            }
            double max = val.Max();
            double min = val.Min();
            double range = max - min;
            peakdisplay.Items[1] = string.Format("     max:{0:g3}", max);
            peakdisplay.Items[2] = string.Format("     min:{0:g3}", min);
            peakdisplay.Items[3] = string.Format("   range:{0:g3}", range);
            peakdisplay.Invalidate();
        }

        private void update_wave_chart_filtered()
        {
            if (!force_chart_update && filtered_id == data[ch].filtered_id)
                return;
            filtered_id = data[ch].filtered_id;

            var s = wave_chart.Series[1].Points;
            s.Clear();
            if (hide_result.Checked || data[ch].xvalues == null)
                return;
            var val = data[ch].filtered;
            if (AbsoluteTime.Enabled && AbsoluteTime.Checked)
            {
                var origin = wavefile.Time;
                wave_chart.Series[1].XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Time;
                var x = data[ch].xvalues.Select(v => origin.AddSeconds(v)).ToArray();
                for (int i = 0; i < num_disp; i += step)
                {
                    s.AddXY(x[i], val[i]);
                }
            }
            else
            {
                var x = data[ch].xvalues;
                wave_chart.Series[1].XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Double;
                for (int i = 0; i < num_disp; i += step)
                {
                    s.AddXY(x[i], val[i]);
                }
            }
            double max = val.Max();
            double min = val.Min();
            double range = max - min;
            peakdisplay.Items[5] = string.Format("     max:{0:g3}", max);
            peakdisplay.Items[6] = string.Format("     min:{0:g3}", min);
            peakdisplay.Items[7] = string.Format("   range:{0:g3}", range);
            peakdisplay.Invalidate();
        }

        private void update_wave_chart_oversampled()
        {
            // skip update if id does not changed
            if (!force_chart_update && over_id == data[ch].over_id)
                return;

            over_id = data[ch].over_id;

            var s = wave_chart.Series[2].Points;

            s.Clear();
            if (hide_over.Checked || data[ch].over_sampled == null || step > 1)
                return;

            double x0 = data[ch].x0;
            double dx = dt / over_sampling;
            var val = data[ch].over_sampled;

            if (AbsoluteTime.Enabled && AbsoluteTime.Checked)
            {
                var origin = wavefile.Time;
                wave_chart.Series[2].XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Time;
                var x = Enumerable.Range(0, val.Length).Select(i => origin.AddSeconds((x0 + dx * i))).ToArray();
                for (int i = 0; i < num_disp; i += step)
                {
                    s.AddXY(x[i], val[i]);
                }
            }
            else
            {
                var x = Enumerable.Range(0, val.Length).Select(i => x0 + dx * i).ToArray();
                wave_chart.Series[2].XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Double;
                for (int i = 0; i < num_disp; i += step)
                {
                    s.AddXY(x[i], val[i]);
                }
            }
            double max = val.Max();
            double min = val.Min();
            double range = max - min;
            peakdisplay.Items[9] = string.Format("     max:{0:g3}", max);
            peakdisplay.Items[10] = string.Format("     min:{0:g3}", min);
            peakdisplay.Items[11] = string.Format("   range:{0:g3}", range);
            peakdisplay.Invalidate();
        }

        private void update_filter_chart()
        {
            if (factor_id == data[ch].factor_id)
                return;
            factor_id = data[ch].factor_id;

            var f = filter_chart.Series[0].Points;
            var s = filter_chart.Series[1].Points;
            f.Clear();
            s.Clear();
            for (int i = -tap; i < tap; i++)
            {
                f.Add(data[ch].factor[Math.Abs(i)]);
                s.Add(data[ch].window[i]);
            }
        }

        private void update_freq_chart()
        {
            update_freq_chart_source();
            update_freq_chart_filtered();
            update_freq_chart_gain();
            if (chkShowPassBandOnly.Checked)
            {
                foreach (var ca in freq_chart.ChartAreas)
                {
                    ca.AxisX.Maximum = Math.Ceiling(double.Parse(upper_fc.Text));
                    ca.AxisX.Minimum = Math.Floor(double.Parse(lower_fc.Text));
                }
            }
            else
            {
                foreach (var ca in freq_chart.ChartAreas)
                {
                    ca.AxisX.Maximum = double.Parse(nyquist_frequency.Text);
                    ca.AxisX.Minimum = 0.0;
                }
            }
            freq_chart.ChartAreas[0].RecalculateAxesScale();
        }

        /// <summary>
        ///  Update PowerSpectrum Series in Frequency Chart
        /// </summary>
        private void update_freq_chart_source()
        {
            double df = fs / nfft;
            var s = freq_chart.Series[0].Points;
            var amps = data[ch].wave_spectrum_amplitude_in_dB();
            var x = data[ch].freqs;
            int n = nfft / 2;
            if (data[ch].category == "DIS" || data[ch].category == "VEL")
            {
                n -= 1;
            }
            s.Clear();
            for(int i = 1; i <= n; ++i)
            {
                if(! Double.IsInfinity(amps[i]))
                    s.AddXY(x[i], amps[i]);
            }
        }

        private void update_freq_chart_filtered()
        {
            var s = freq_chart.Series[1].Points;
            var amps = data[ch].Power(WaveDataSet.State.Filtered).Select(v => 20.0 * Math.Log10(v)).ToArray();
            var x = data[ch].freqs;
            int n = nfft / 2;
            if (data[ch].category == "DIS" || data[ch].category == "VEL")
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

        private void update_freq_chart_gain()
        {
            if (gain_id == data[ch].gain_id)
                return;
            gain_id = data[ch].gain_id;

            var s = this.freq_chart.Series[2].Points;
            var gains = data[ch].gains.Take(tap + 1).ToArray();

            s.Clear();
            for (int i = 0; i < tap+1; i++)
                s.AddXY(tap_freqs[i], gains[i]);
        }

        private void update_chart_visible()
        {
            wave_fft_chart.Visible = show_fft_data.Checked;
            peak_chart.Visible = !show_fft_data.Checked;
        }
        #endregion

        private void channel_change()
        {
            ch = channel_track.Value;

            channel.Text = ch.ToString();
            channel_name.Text = wavefile.name(ch);
            channel_comment.Text = wavefile.comment(ch);
            data_length.Text = data[ch].length.ToString();

            fs = 1.0 / dt;
            sampling_rate.Text = fs.ToString();
            sampling_rate.ReadOnly = true;
            fn = fs / 2.0;
            nyquist_frequency.Text = fn.ToString();

            freq_chart.ChartAreas[0].AxisX.Maximum = fn;
            update_display_data_length();
            data_start.Maximum = num_data - num_disp;
            // change data_start
            data[ch].data_start = data_start.Value;

            // update filter information
            var window_type = data[ch].window_type; // save
            tap_track.Value = data[ch].tap;
            update_tap_info();

            alpha.Text = data[ch].alpha.ToString(); // filter's window type is chened to Kaiser automatically.
            
            lower_fc_track.Value = data[ch].lower;
            update_lower_fc();
            upper_fc_track.Value = data[ch].upper;
            update_upper_fc();

            
            // switch to actual window_type
            switch (window_type)
            {
                case FIRFilter.WindowType.None:
                case FIRFilter.WindowType.Rectangle:
                    rectangle_window.Checked = true;
                    break;
                case FIRFilter.WindowType.Han:
                    hann_window.Checked = true;
                    break;
                case FIRFilter.WindowType.Hamming:
                    hamming_widow.Checked = true;
                    break;
                case FIRFilter.WindowType.Kaiser:
                    kaiser_window.Checked = true;
                    break;
                case FIRFilter.WindowType.Blackman:
                    blackman_window.Checked = true;
                    break;
                default:
                    throw new ApplicationException("Error in Window type");
            }
            lower_fc_track.Value = data[ch].lower;
            update_lower_fc();
            upper_fc_track.Value = data[ch].upper;
            update_upper_fc();
            tap_track.Value = data[ch].tap;
            update_tap_info();
            update_display_data_length();
            CheckUpdate();
        }

        private void update_tap_info()
        {
            int tap = tap_track.Value;

            if (lower_fc_track.Value > tap)
                lower_fc_track.Value = tap - 1;
            lower_fc_track.Maximum = tap - 1;
            if (upper_fc_track.Value > tap)
                upper_fc_track.Value = tap;
            upper_fc_track.Maximum = tap;

            number_of_tap.Text = tap.ToString();
            int filter_size = tap * 2 - 1;
            filter_length.Text = filter_size.ToString();

            if(data != null)
                fir.tap = tap;

            double df = fs / tap / 2;
            tap_freqs = Enumerable.Range(0, tap + 1).Select(i => df * i).ToArray();
        }

        private void update_display_data_length()
        {
            int val;
            if (int.TryParse(display_data_length.Text, out val))
            {
                if (val > num_data || val > MAX_DISP_SIZE)
                {
                    display_data_length.Text = Math.Min(num_data, MAX_DISP_SIZE).ToString();
                    return;
                }
                num_disp = val;
                step = 1 + ((num_disp - 1) / max_points);
                data_start.LargeChange = num_disp / 5;
                int largest = num_data - num_disp;
                //if (data_start.Value > largest)
                //    data_start.Value = largest;
                data_start.Maximum = largest;
            }
        }

        private void update_upper_fc()
        {
            upper_val.Text = upper_fc_track.Value.ToString();
            upper_fc.Text = (upper_fc_track.Value * fn / tap).ToString();
            if (upper_fc_track.Value <= lower_fc_track.Value)
            {
                lower_fc_track.Value = upper_fc_track.Value - 1;
            }
            if(data != null)
                fir.upper = upper_fc_track.Value;
        }

        private void update_lower_fc()
        {
            lower_val.Text = lower_fc_track.Value.ToString();
            lower_fc.Text = (lower_fc_track.Value * fn / tap).ToString();
            if (lower_fc_track.Value >= upper_fc_track.Value)
            {
                upper_fc_track.Value = lower_fc_track.Value + 1;
            }
            if(data != null)
                fir.lower = lower_fc_track.Value;
        }

        private void update_gain()
        {
            double val;
            if (double.TryParse(gain.Text, out val))
            {
                fir.gain = val;
            }
        }
        #endregion
        #region Operations

        private void open_file(string file_name)
        {
            file_path.Text = file_name;
            var ext = System.IO.Path.GetExtension(file_name).ToUpper();
            if (ext == ".DAT")
            {
                if (Famos.is_famos(file_name))
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
                    wavefile = DelimFile.KyowaCsv(file_name);
                    if (wavefile != null)
                        AbsoluteTime.Enabled = true;
                }
                else
                {
                    wavefile = DelimFile.GeneralCsv(file_name);
                    if(wavefile != null)
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
            if (!wavefile.opened)
                return;
            progressBar1.Maximum = wavefile.cols + 2;
            progressBar1.Value = 1;

            var default_window_type = FIRFilter.WindowType.None;
            double a = 1.5;
            double.TryParse(alpha.Text, out a);
            if (rectangle_window.Checked) default_window_type = FIRFilter.WindowType.Rectangle;
            if (hann_window.Checked) default_window_type = FIRFilter.WindowType.Han;
            if (hamming_widow.Checked) default_window_type = FIRFilter.WindowType.Hamming;
            if (blackman_window.Checked) default_window_type = FIRFilter.WindowType.Blackman;
            if (kaiser_window.Checked) default_window_type = FIRFilter.WindowType.Kaiser;

            data = new List<WaveDataSet>(wavefile.cols);

            for (int i = 0; i < wavefile.cols; i++)
            {
                progressBar1.Value = i + 2;
                var wave = new WaveDataSet(wavefile, i);
                data.Add(wave);
                wave.tap = tap_track.Value;
                wave.lower = lower_fc_track.Value;
                wave.upper = upper_fc_track.Value;
                wave.alpha = a;
                wave.window_type = default_window_type;

                wave.gain = -80.0;
            }
            progressBar1.Value = progressBar1.Maximum;

            // reset combobox
            foreach (var item in targets)
            {
                item.Text = "";
                item.Items.Clear();
                for (int i = 0; i < wavefile.cols; i++)
                {
                    item.Items.Add(i.ToString());
                }
            }

            channel_track.Value = 0;
            channel_track.Maximum = wavefile.cols - 1;
            axes_ranges = new AxisRanges[wavefile.cols];
            for(int i = 0; i < wavefile.cols; i++)
            {
                // NaN means automatic
                axes_ranges[i].wave.max = axes_ranges[i].wave.min = double.NaN;
                //axes_ranges[i].peak_P.max = axes_ranges[i].peak_P.min = double.NaN;
                //axes_ranges[i].peak_a.max = axes_ranges[i].peak_a.min = double.NaN;
            }

            update_tap_info();

            channel_change();

            progressBar1.Visible = false;

            umi.Enabled = true;
            yama.Enabled = true;
            len_0_1sec.Enabled = true;
            len_0_5sec.Enabled = true;
            len_1sec.Enabled = true;
            len_5sec.Enabled = true;

        }

        private void set_alpha(string text)
        {
            double val;
            if (double.TryParse(text, out val))
            {
                fir.alpha = val;
            }
        }

        private string find_channel(string in_name)
        {
            for (int i = 0; i < wavefile.cols; i++)
            {
                if (wavefile.name(i).Contains(in_name))
                    return i.ToString();
            }
            return "";
        }

        void search_channels(string side)
        {
            ch_P1.Text = find_channel(side + "_P1").ToString();
            ch_P2.Text = find_channel(side + "_P2").ToString();
            ch_Ya.Text = find_channel(side + "_Ya").ToString();
            ch_Za.Text = find_channel(side + "_Za").ToString();
        }

        private void set_data_length_by_duration(double dur)
        {
            int count = (int)(dur / dt);
            display_data_length.Text = count.ToString();
        }

        private void save_wave(int idx)
        {
            if (saveFileDialogCSV.ShowDialog() == System.Windows.Forms.DialogResult.Cancel)
                return;

            using (var stream = saveFileDialogCSV.OpenFile())
            {
                using (var writer = new System.IO.StreamWriter(stream, Encoding.Default))
                {
                    writer.WriteLine("Original File, {0}", file_path.Text);
                    writer.WriteLine("Data Start, {0}", data_start.Value);
                    writer.WriteLine("Data Length, {0}", data_length.Text);
                    writer.WriteLine("Sampling Rate, {0}", fs);
                    writer.WriteLine("Ch, {0}", ch);
                    writer.WriteLine("Channel name, {0}", channel_name.Text);
                    writer.WriteLine("Comment of channel, {0}", channel_comment.Text);
                    writer.WriteLine("");
                    writer.WriteLine("time, {0} Wave", wave_chart.Series[idx].Name);

                    // Setup wave data
                    double[] x = data[ch].xvalues;
                    double[] y;
                    switch (idx)
                    {
                        case 0:
                            y = data[ch].source;
                            break;
                        case 1:
                            y = data[ch].filtered;
                            break;
                        case 2:
                            y = data[ch].over_sampled;
                            break;
                        case 3:
                            y = wave_chart.Series[3].Points.Select(v => v.YValues[0]).ToArray();
                            x = wave_chart.Series[3].Points.Select(v => v.XValue).ToArray();
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

        private void save_all_waves(int idx)
        {
            if (saveFileDialogCSV.ShowDialog() == System.Windows.Forms.DialogResult.Cancel)
                return;

            // need to update data start for all channnels
            System.Threading.Tasks.Parallel.ForEach(data, (d) => { d.data_start = data_start.Value; d.update(); });

            using (var stream = saveFileDialogCSV.OpenFile())
            {
                using (var writer = new System.IO.StreamWriter(stream, Encoding.Default))
                {
                    writer.WriteLine("Original File, {0}", file_path.Text);
                    writer.WriteLine("Data Start, {0}", data_start.Value);
                    writer.WriteLine("Data Length, {0}", data_length.Text);
                    writer.WriteLine("Sampling Rate, {0}", fs);
                    var chs = Enumerable.Range(0, data.Count);
                    writer.WriteLine("Ch, {0}", string.Join(",", chs));
                    writer.WriteLine("Channel name, {0}", string.Join(",", chs.Select(i=>wavefile.name(i))));
                    writer.WriteLine("Comment of channel, {0}", string.Join(",",chs.Select(i=>wavefile.comment(i))));
                    writer.WriteLine("Wave Type, {0}", wave_chart.Series[idx].Name);
                    writer.WriteLine("");
                    writer.WriteLine("time, {0}", string.Join(",",chs.Select(i=>"Ch"+i.ToString())));

                    // Setup wave data
                    double[] x = data[0].xvalues;
                    IEnumerable<double[]> y;
                    switch (idx)
                    {
                        case 0:
                            y = data.Select(v=>v.source);
                            break;
                        case 1:
                            y = data.Select(v => v.filtered);
                            break;
                        case 2:
                            y = data.Select(v => v.over_sampled);
                            break;
                        default:
                            throw new ArgumentException("index must be in 0 to 2");
                    }
                    for (int i = 0; i < x.Length; i++)
                    {
                        writer.WriteLine("{0},{1}", x[i], string.Join(",",y.Select(v=>v[i])));
                    }
                }
            }
        }

        private void save_all_peaks()
        {
            if (saveFileDialogCSV.ShowDialog() == System.Windows.Forms.DialogResult.Cancel)
                return;

            using (var stream = saveFileDialogCSV.OpenFile())
            {
                using (var writer = new System.IO.StreamWriter(stream, Encoding.Default))
                {
                    writer.WriteLine("Original File, {0}", file_path.Text);
                    writer.WriteLine("Source Data Start, {0}", data_start.Value);
                    writer.WriteLine("Source Data Length, {0}", data_length.Text);
                    writer.WriteLine("Sampling Rate of Original Data, {0}", fs);
                    var chs = targets.Select(t => int.Parse(t.Text));
                    writer.WriteLine("Ch, {0}", string.Join(",", chs));
                    writer.WriteLine("Channel name, {0}", string.Join(",", chs.Select(i => wavefile.name(i))));
                    writer.WriteLine("Comment of channel, {0}", string.Join(",", chs.Select(i => wavefile.comment(i))));
                    writer.WriteLine("Wave Type, Peaks");
                    var peak_counts = peak_chart.Series.Select(s => s.Points.Count).ToArray();
                    writer.WriteLine("Number of Peaks, {0}", string.Join(",",peak_counts));
                    writer.WriteLine("");
                    writer.WriteLine("time, P1, time, P2, time, Ya, time, Za");
                    var len = peak_counts.Max();
                    var res = peak_counts.Select((n, i) =>
                        peak_chart.Series[i].Points
                            .Select(p => string.Format("{0},{1}", p.XValue, p.YValues[0]))
                            .Concat(Enumerable.Repeat(",", len - n)).ToArray());
                    for (int i = 0; i < peak_counts.Max(); i++)
                    {
                        writer.WriteLine("{0}", string.Join(",", res.Select(a => a[i])));
                    }
                }
            }
        }

        private void save_peaks(int idx)
        {
            if (saveFileDialogCSV.ShowDialog() == System.Windows.Forms.DialogResult.Cancel)
                return;
            using (var stream = saveFileDialogCSV.OpenFile())
            {
                using (var writer = new System.IO.StreamWriter(stream, Encoding.Default))
                {
                    writer.WriteLine("Original File, {0}", file_path.Text);
                    writer.WriteLine("Source Data Start, {0}", data_start.Value);
                    writer.WriteLine("Source Data Length, {0}", data_length.Text);
                    writer.WriteLine("Sampling Rate of Original Data, {0}", fs);
                    var target_ch = int.Parse(targets[idx].Text);
                    writer.WriteLine("Ch, {0}", targets[idx].Text);
                    writer.WriteLine("Channel name, {0}", wavefile.name(target_ch));
                    writer.WriteLine("Comment of channel, {0}", wavefile.comment(target_ch));
                    writer.WriteLine("Wave Type, Peaks");
                    writer.WriteLine("Number of Peaks, {0}", peak_chart.Series[idx].Points.Count);
                    writer.WriteLine("");
                    writer.WriteLine("time, {0}", targets[idx].Name.Substring(3));
                    foreach(var item in peak_chart.Series[idx].Points)
                    {
                        writer.WriteLine("{0},{1}",item.XValue, item.YValues[0]);
                    }
                }
            }
        }

        #endregion // Operations
        #endregion // PrivateMethods
        #endregion // Internal Use

        #region EventHandler

        private void tap_track_Scroll(object sender, EventArgs e)
        {
            update_tap_info();
            CheckUpdate();
        }

        private void upper_fc_track_Scroll(object sender, EventArgs e)
        {
            update_upper_fc();
            CheckUpdate();
        }


        private void lower_fc_track_Scroll(object sender, EventArgs e)
        {
            update_lower_fc();
            CheckUpdate();
        }


        private void update_Click(object sender, EventArgs e)
        {
            data_update();
        }

        private void channel_track_Scroll(object sender, EventArgs e)
        {
            channel_change();
            wave_chart.ChartAreas[0].AxisY.Maximum = axes_ranges[ch].wave.max;
            wave_chart.ChartAreas[0].AxisY.Minimum = axes_ranges[ch].wave.min;
            //peak_chart.ChartAreas[0].AxisY.Maximum = axes_ranges[ch].peak_P.max;
            //peak_chart.ChartAreas[0].AxisY.Minimum = axes_ranges[ch].peak_P.min;
            //peak_chart.ChartAreas[0].AxisY2.Maximum = axes_ranges[ch].peak_a.max;
            //peak_chart.ChartAreas[0].AxisY2.Minimum = axes_ranges[ch].peak_a.min;
            CheckUpdate();
        }

        private void select_file_Click(object sender, EventArgs e)
        {
            if (openFamosDialog.ShowDialog()== System.Windows.Forms.DialogResult.Cancel)
                return;

            open_file(openFamosDialog.FileName);
        }


        private void display_data_length_TextChanged(object sender, EventArgs e)
        {
            update_display_data_length();
            CheckUpdate();
        }


        private void kaiser_window_CheckedChanged(object sender, EventArgs e)
        {
            set_alpha(alpha.Text);
            CheckUpdate();
        }

        private void alpha_TextChanged(object sender, EventArgs e)
        {
            if(kaiser_window.Checked )
                set_alpha(alpha.Text);
            CheckUpdate();
        }

        private void rectangle_window_CheckedChanged(object sender, EventArgs e)
        {
            fir.window_type = FIRFilter.WindowType.Rectangle;
            CheckUpdate();
        }

        private void blackman_window_CheckedChanged(object sender, EventArgs e)
        {
            fir.window_type = FIRFilter.WindowType.Blackman;
            CheckUpdate();
        }

        private void hann_window_CheckedChanged(object sender, EventArgs e)
        {
            fir.window_type = FIRFilter.WindowType.Han;
            CheckUpdate();
        }

        private void hamming_widow_CheckedChanged(object sender, EventArgs e)
        {
            fir.window_type = FIRFilter.WindowType.Hamming;
            CheckUpdate();
        }

        private void data_start_ValueChanged(object sender, EventArgs e)
        {
            DataStart.Text = data_start.Value.ToString();
            if(data != null)
                data[ch].data_start = data_start.Value;
            foreach (var item in targets)
            {
                int i;
                if (int.TryParse(item.Text, out i))
                {
                    data[i].data_start = data_start.Value;
                }
            }
            CheckUpdate();
        }

        private void gain_TextChanged(object sender, EventArgs e)
        {
            update_gain();
        }

        private void umi_Click(object sender, EventArgs e)
        {
            bool state = auto_update.Checked;
            auto_update.Checked = false;
            search_channels("UMI");
            auto_update.Checked = state;
            data_start_ValueChanged(sender, e);
            CheckUpdate();
        }

        private void yama_Click(object sender, EventArgs e)
        {
            bool state = auto_update.Checked;
            auto_update.Checked = false;
            search_channels("YAMA");
            auto_update.Checked = state;
            data_start_ValueChanged(sender, e);
            CheckUpdate();
        }

        private void len_0_1sec_Click(object sender, EventArgs e)
        {
            set_data_length_by_duration(0.1);
            CheckUpdate();
        }


        private void len_0_5sec_Click(object sender, EventArgs e)
        {
            set_data_length_by_duration(0.5);
            CheckUpdate();
        }

        private void len_1sec_Click(object sender, EventArgs e)
        {
            set_data_length_by_duration(1.0);
            CheckUpdate();
        }

        private void len_5sec_Click(object sender, EventArgs e)
        {
            set_data_length_by_duration(5.0);
            CheckUpdate();
        }

        private void sampling_rate_TextChanged(object sender, EventArgs e)
        {
            if (!sampling_rate.ReadOnly)
            {
                if (double.TryParse(sampling_rate.Text, out fs))
                {
                    fn = (fs / 2.0);
                    nyquist_frequency.Text = fn.ToString();
                    update_upper_fc();
                    update_lower_fc();
                }
            }
        }
        private void over_sampling_1_CheckedChanged(object sender, EventArgs e)
        {
            over_sampling = 1;
            CheckUpdate();
        }

        private void over_sampling_2_CheckedChanged(object sender, EventArgs e)
        {
            over_sampling = 2;
            CheckUpdate();
        }

        private void over_sampling_4_CheckedChanged(object sender, EventArgs e)
        {
            over_sampling = 4;
            CheckUpdate();
        }

        private void over_sampling_8_CheckedChanged(object sender, EventArgs e)
        {
            over_sampling = 8;
            CheckUpdate();
        }

        private void CheckUpdateHandler(object sende, EventArgs e)
        {
            CheckUpdate();
        }


        private void integ_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (data == null)
                return;
            foreach (var item in data)
            {
                item.category = integ.Text;
            }
            CheckUpdate();
        }


        private void hide_over_CheckedChanged(object sender, EventArgs e)
        {
            hide_flag[HIDE_OVER] = hide_over.Checked;
            over_id = 0;
            update_wave_chart_oversampled();
            wave_chart.Invalidate();
        }

        private void hide_result_CheckedChanged(object sender, EventArgs e)
        {
            hide_flag[HIDE_ANS] = hide_result.Checked;
            filtered_id = 0;
            update_wave_chart_filtered();
            wave_chart.Invalidate();
        }

        private void hide_source_CheckedChanged(object sender, EventArgs e)
        {
            hide_flag[HIDE_WAVE] = hide_source.Checked;
            source_id = 0;
            update_wave_chart_source();
            wave_chart.Invalidate();
        }

        private void show_fft_data_CheckedChanged(object sender, EventArgs e)
        {
            update_chart_visible();
            CheckUpdate();
        }


        private void DataStart_Validated(object sender, EventArgs e)
        {
            data_start.Value = int.Parse(DataStart.Text);
        }

        private void DataStart_Validating(object sender, CancelEventArgs e)
        {
            int val;
            if (!int.TryParse(DataStart.Text, out val))
            {
                e.Cancel = true;
                return;
            }
            if (val < 0) DataStart.Text = "0";
            if (val > data_start.Maximum) DataStart.Text = data_start.Maximum.ToString();
        }
        #endregion

        //#region saveWaveHandlers
        private void sourceWaveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            save_wave(0);
        }
        private void SaveDisplayedFilteredWave_Click(object sender, EventArgs e)
        {
            save_wave(1);
        }

        private void SaveDisplayedOverWave_Click(object sender, EventArgs e)
        {
            save_wave(2);
        }

        private void peakWaveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            save_wave(3);
        }

        private void saveAllDisplayedSourceWaves_Click(object sender, EventArgs e)
        {
            save_all_waves(0);
        }

        private void saveAllDisplayedFilteredWaves_Click(object sender, EventArgs e)
        {
            save_all_waves(1);
        }

        private void saveAllDisplayedOversampledWaves_Click(object sender, EventArgs e)
        {
            save_all_waves(2);
        }

        private void chkShowPassBandOnly_CheckedChanged(object sender, EventArgs e)
        {
            update_freq_chart();
        }

        private void btnExpandFilterSettings_Click(object sender, EventArgs e)
        {
            var curr = data[ch];
            for (int i = 0; i < data.Count; i++)
            {
                if (i == ch) continue;
                var tgt = data[i];
                tgt.tap = curr.tap;
                tgt.upper = curr.upper;
                tgt.lower = curr.lower;
                tgt.alpha = curr.alpha;
                tgt.window_type = curr.window_type;
                tgt.gain = curr.gain;
                tgt.data_start += 1;
                tgt.data_start = curr.data_start;
                tgt.update();
            }
        }

        private void dataGridView1_Validated(object sender, EventArgs e)
        {
            if (data != null)
            {
                foreach (var item in data)
                {
                    item.notch = this.notchFilterInfo.Notches;
                }
                CheckUpdate();
            }
        }

        private void chart_range_enter(object o_sender, EventArgs e)
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

        private void chart_range_changed(object o_sender, EventArgs e)
        {
            TextBox sender = (TextBox)o_sender;
            var ax = (System.Windows.Forms.DataVisualization.Charting.Axis)sender.Tag;
            double val;
            if (sender.Text == "")
            {
                if (sender.Width == 20)
                    return;
                if (sender.Name.Contains("max"))
                    ax.Maximum = double.NaN;
                else
                    ax.Minimum = double.NaN;
            }
            else if (double.TryParse(sender.Text, out val))
            {
                if (sender.Name.Contains("max"))
                {
                    if(val > ax.Minimum )
                        ax.Maximum = val;
                }
                else
                {
                    if( val < ax.Maximum)
                        ax.Minimum = val;
                }
            }
        }
        private void chart_range_leave(object  o_sender, EventArgs e)
        {
            TextBox sender = (TextBox)o_sender;
            sender.Width = 20;
            sender.Text = "";
        }

        private void channel_track_Enter(object sender, EventArgs e)
        {
            if (axes_ranges != null)
            {
                axes_ranges[ch].wave.max = wave_chart.ChartAreas[0].AxisY.Maximum;
                axes_ranges[ch].wave.min = wave_chart.ChartAreas[0].AxisY.Minimum;
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
            foreach (var item in peak_chart.Series)
            {
                item.XValueType = val;
            }
            foreach (var item in wave_chart.Series)
            {
                item.XValueType = val;
            }
            if (AbsoluteTime.Checked)
            {
                peak_chart.ChartAreas[0].AxisX.LabelStyle.Format = "hh:mm:ss.FFF";
                wave_chart.ChartAreas[0].AxisX.LabelStyle.Format = "hh:mm:ss.FFF";
            }
            else
            {
                peak_chart.ChartAreas[0].AxisX.LabelStyle.Format = "g";
                var ax = wave_chart.ChartAreas[0].AxisX;
                ax.LabelStyle.Format = "g";
                ax.IntervalAutoMode = System.Windows.Forms.DataVisualization.Charting.IntervalAutoMode.FixedCount;
                ax.IntervalType = System.Windows.Forms.DataVisualization.Charting.DateTimeIntervalType.Auto;
            }
            force_chart_update = true;
            update_wave_chart();
            update_peak_chart();
            wave_chart.ChartAreas[0].RecalculateAxesScale();
            peak_chart.ChartAreas[0].RecalculateAxesScale();
            force_chart_update = false;
        }

        private void CopyRangeToAllCh_Click(object sender, EventArgs e)
        {
            var max = wave_chart.ChartAreas[0].AxisY.Maximum;
            var min = wave_chart.ChartAreas[0].AxisY.Minimum;
            for (int i = 0; i < data.Count; i++)
            {
                axes_ranges[i].wave.max = max;
                axes_ranges[i].wave.min = min;
            }
        }

        private void saveAllPeaks_Click(object sender, EventArgs e)
        {
            save_all_peaks();
        }

        private void saveP1Peaks_Click(object sender, EventArgs e)
        {
            save_peaks(0);
        }

        private void saveP2Peaks_Click(object sender, EventArgs e)
        {
            save_peaks(1);
        }

        private void saveYaPeaks_Click(object sender, EventArgs e)
        {
            save_peaks(2);
        }

        private void saveZaPeaks_Click(object sender, EventArgs e)
        {
            save_peaks(3);
        }

    }
}
