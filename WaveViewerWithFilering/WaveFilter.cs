using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using fftwlib;
using System.Runtime.InteropServices; 

namespace WaveViewerWithFilering
{
    public partial class wave_filter : Form
    {
        public wave_filter()
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
        }

        private int nfft {  get { return data[ch].nfft; } }
        private int tap {  get { return tap_track.Value; } }
        private WaveDataSet fir { get { return data[ch]; } }
        ComboBox[] targets;
        TextBox[] thresholds;
        TextBox[] required_lengths;

        private int over_sampling_; 
        public int over_sampling {
            get { return over_sampling_; }
            set {
                if (over_sampling_ != value)
                {
                    over_sampling_ = value;
                    foreach (var item in data)
                    {
                        item.over_sample = value;
                    }
                }
            }
        }


        //-----------------------------------------------------------------//

        List<WaveDataSet> data;

        int ch;

        int num_point;
        int step;
        const int max_points = 5000;

        private double fs;
        private double fn;
        private Famos famos;

        bool[] hide_flag;
        const int HIDE_WAVE = 0;
        const int HIDE_ANS = 1;
        const int HIDE_OVER = 2;


        private void data_update()
        {
            if (famos == null)
                return;

            update_wave_chart_source();
            update_wave_chart_filtered();
            update_filter_chart();
            update_wave_chart_oversampled();
            wave_chart.ChartAreas[0].RecalculateAxesScale();
            update_sp_wave();
            update_freq_chart();
            if (show_fft_data.Checked)
            {
                update_wave_fft_chart();
            }
            else
            {
                update_peak_chart();
            }
        }

        private void update_wave_chart_oversampled()
        {
            // skip update if id does not changed
            if (over_id == data[ch].over_id)
                return;

            over_id = data[ch].over_id;

            var s = wave_chart.Series[2].Points;

            if (hide_over.Checked || data[ch].over_sampled == null || step > 1)
            {
                s.Clear();
                return;
            }

            var val = data[ch].over_sampled.ToList();
            double x0 = data[ch].xvalues.First();
            double dx = dt / over_sampling;
            if (num_disp  == s.Count)
            {
                // overwrite
                for (int i = 0; i < num_disp; i++)
                {
                    s[i].XValue = x0 + dx * i;
                    s[i].YValues[0] = val[i];
                }
            }
            else
            {
                // renew
                s.Clear();
                for (int i = 0; i < num_disp; i++)
                {
                    double x = x0 + dx * i;
                    s.AddXY(x, val[i]);
                }
            }
        }

        private void update_wave_fft_chart()
        {
            var waves = data[ch].debug_waves();

            for (int i = 0; i < waves.Length; i++)
            {
                var s = wave_fft_chart.Series[i].Points;
                s.Clear();
                if (hide_flag[i])
                    continue;
                s.DataBindY(waves[i]);
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
                    var x0 = wave.xvalues.First();
                    var dx = wave.dt / over_sampling;
                    var s = peak_chart.Series[i].Points;
                    s.Clear();

                    bool add_wave = ch == channel;
                    foreach (var item in peaks)
                    {
                        double x = x0 + dx * item.Key;
                        s.AddXY(x, item.Value);
                        if (add_wave) wave_peaks.AddXY(x, item.Value);
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

        private void update_wave_chart_filtered()
        {
            if (filtered_id == data[ch].filtered_id)
                return;
            filtered_id = data[ch].filtered_id;

            var s = wave_chart.Series[1].Points;
            if (hide_result.Checked)
                s.Clear();
            else
            {
                if (step > 1)
                    s.DataBindXY(data[ch].xvalues.Where((x, i) => i % step == 0).ToList(),
                           data[ch].filtered.Where((x, i) => i % step == 0).ToList());
                else
                    s.DataBindXY(data[ch].xvalues, data[ch].filtered);
            }
            wave_chart.ChartAreas[0].RecalculateAxesScale();
        }

        private void update_freq_chart()
        {
            if (gain_id == data[ch].gain_id)
                return;
            gain_id = data[ch].gain_id;

            var s = this.freq_chart.Series[0].Points;
            double df = fs / tap / 2;
            var gains = data[ch].gains.ToList();

            if (s.Count == tap + 1)
            {
                for (int i = 0; i < s.Count; i++)
                {
                    s[i].YValues[0] = gains[i];
                }
            }
            else
            {
                s.DataBindXY(data[ch].freqs, gains);
            }
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

        private void CheckUpdate()
        {
            if (auto_update.Checked)
            {
                data_update();
            }
        }

        private void update_sp_wave()
        {
            update_freq_chart_source();
        }

        /// <summary>
        ///  Update PowerSpectrum Series in Frequency Chart
        /// </summary>
        private void update_freq_chart_source()
        {
            double df = fs / nfft;
            var s = freq_chart.Series[1].Points;
            var amps = data[ch].wave_spectrum_amplitude_in_dB();
            int n = nfft / 2;
            if (data[ch].category == "DIS" || data[ch].category == "VEL")
            {
                n -= 1;
            }
            if (s.Count == n)
            {
                int i =0;
                foreach( var v in amps.Skip(1))
                {
                    s[i++].YValues[0] = v;
                }
            }
            else
            {
                s.DataBindXY(data[ch].freqs.Skip(1).ToArray(), amps.Skip(1).ToList());
            }
            freq_chart.ChartAreas[0].RecalculateAxesScale();
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
                {
                    foreach (var item in data)
                    {
                        item.num_disp = value;
                    }
                }
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
        private void update_wave_chart_source()
        {
            if (source_id == data[ch].source_id)
                return;

            source_id = data[ch].source_id;

            int pos = data_start.Value;
            num_point = Math.Min(num_disp, num_data - pos - 1) / step;

            var s = wave_chart.Series[0].Points;
            if (hide_source.Checked)// (data[ch].category == "DIS" || data[ch].category == "VEL")
            {
                // integrated wave should not show
                s.Clear();
                return;
            }
            s.DataBindXY(data[ch].xvalues.Where((x, i) => i % step == 0).ToList(),
                data[ch].source.Where((x, i) => i % step == 0).ToList());
            wave_chart.ChartAreas[0].RecalculateAxesScale();
        }

        private double dt { get { return data[ch].dt; } }

        private void open_file(string file_name)
        {
            file_path.Text = file_name;

            famos = new Famos(file_name);

            progressBar1.Value = 0;
            progressBar1.Visible = true;
            if (!famos.opened)
                return;
            progressBar1.Maximum = famos.cols + 2;
            progressBar1.Value = 1;

            var default_window_type = FIRFilter.WindowType.None;
            double a = 1.5;
            double.TryParse(alpha.Text,out a);
            if (rectangle_window.Checked) default_window_type = FIRFilter.WindowType.Rectangle;
            if (hann_window.Checked) default_window_type = FIRFilter.WindowType.Han;
            if (hamming_widow.Checked) default_window_type = FIRFilter.WindowType.Hamming;
            if (blackman_window.Checked) default_window_type = FIRFilter.WindowType.Blackman;
            if (kaiser_window.Checked) default_window_type = FIRFilter.WindowType.Kaiser;

            data = new List<WaveDataSet>(famos.cols);

            for (int i = 0; i < famos.cols; i++)
            {
                progressBar1.Value = i + 2;
                var wave = new WaveDataSet(famos, i);
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
                for (int i = 0; i < famos.cols; i++)
                {
                    item.Items.Add(i.ToString());
                }
            }

            channel_track.Value = 0;
            channel_track.Maximum = famos.cols - 1;

            //            upper_fc_track.Value = upper_fc_track.Maximum;
            channel_change();
            CheckUpdate();

            progressBar1.Visible = false;

            umi.Enabled = true;
            yama.Enabled = true;
            len_0_1sec.Enabled = true;
            len_0_5sec.Enabled = true;
            len_1sec.Enabled = true;
            len_5sec.Enabled = true;

        }

        private void channel_change()
        {
            ch = channel_track.Value;

            channel.Text = ch.ToString();
            channel_name.Text = famos.channel_info[ch].name;
            channel_comment.Text = famos.channel_info[ch].comment;
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

            var window_type = data[ch].window_type; // save

            alpha.Text = data[ch].alpha.ToString(); // filter's window type is chened to Kaiser automatically.
            
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
                data[ch].tap = tap;
        }

        private void set_alpha(string text)
        {
            double val;
            if (double.TryParse(text, out val))
            {
                data[ch].alpha = val;
            }
        }
        const int MAX_DISP_SIZE = 100000;
        private uint over_id;
        private uint source_id;
        private uint filtered_id;
        private uint factor_id;
        private uint gain_id;

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

        private string find_channel(string in_name)
        {
            for (int i = 0; i < famos.cols; i++)
            {
                if (famos.channel_info[i].name.Contains(in_name))
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

        //-----------------------------------------------------------------//

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

        private void ch_P1_SelectedIndexChanged(object sender, EventArgs e)
        {
            CheckUpdate();
        }

        private void ch_P2_SelectedIndexChanged(object sender, EventArgs e)
        {
            CheckUpdate();
        }

        private void ch_Ya_SelectedIndexChanged(object sender, EventArgs e)
        {
            CheckUpdate();
        }

        private void ch_Za_SelectedIndexChanged(object sender, EventArgs e)
        {
            CheckUpdate();
        }

        private void th_P1_TextChanged(object sender, EventArgs e)
        {
            CheckUpdate();
        }

        private void th_P2_TextChanged(object sender, EventArgs e)
        {
            CheckUpdate();
        }

        private void th_Ya_TextChanged(object sender, EventArgs e)
        {
            CheckUpdate();
        }

        private void th_Za_TextChanged(object sender, EventArgs e)
        {
            CheckUpdate();
        }

        private void rl_P1_TextChanged(object sender, EventArgs e)
        {
            CheckUpdate();
        }

        private void rl_P2_TextChanged(object sender, EventArgs e)
        {
            CheckUpdate();
        }

        private void rl_Ya_TextChanged(object sender, EventArgs e)
        {
            CheckUpdate();
        }

        private void rl_Za_TextChanged(object sender, EventArgs e)
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

        private void update_chart_visible()
        {
            wave_fft_chart.Visible = show_fft_data.Checked;
            peak_chart.Visible = !show_fft_data.Checked;
        }


    }
}
