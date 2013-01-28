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
            update_display_data_length();
            update_tap_info();
            upper_fc_track.Value = tap_track.Value;
            lower_fc_track.Value = 0;
            sampling_rate.Text = "1000.0";
            targets = new ComboBox[] { ch_P1, ch_P2, ch_Ya, ch_Za };
            thresholds = new TextBox[] { th_P1, th_P2, th_Ya, th_Za };
            required_lengths = new TextBox[] { rl_P1, rl_P2, rl_Ya, rl_Za };
        }

        private int nfft {  get { return data[ch].nfft; } }
        private int tap {  get { return tap_track.Value; } }
        private WaveData fir { get { return data[ch]; } }
        ComboBox[] targets;
        TextBox[] thresholds;
        TextBox[] required_lengths;



        //-----------------------------------------------------------------//

        List<WaveData> data;

        int ch;

        int num_point;
        int step;
        const int max_points = 5000;

        private double fs;
        private double fn;
        private Famos famos;

        private void data_update()
        {
            if (famos == null)
                return;
            
            update_wave_chart_source();
            update_wave_chart_filtered();
            update_filter_chart();
            update_sp_wave();
            update_freq_chart();
            update_peak_chart();
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
                    var peaks = finder.apply(wave.over_sampled);
                    var x0 = wave.xvalues[0];
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
            peak_chart.ChartAreas[0].AxisX.Minimum = wave_chart.ChartAreas[0].AxisX.Minimum;
            peak_chart.ChartAreas[0].AxisX.Maximum = wave_chart.ChartAreas[0].AxisX.Maximum;
            peak_chart.ChartAreas[0].RecalculateAxesScale();
        }

        private void update_wave_chart_filtered()
        {
            var s = wave_chart.Series[1].Points;
            var ans = data[ch].filtered;
            var xvalues = data[ch].xvalues;

            if (s.Count == num_point)
            {
                // just overwrite
                for (int i = 0; i < num_point; i++)
                {
                    s[i].XValue = xvalues[i*step]; 
                    s[i].YValues[0] = ans[i*step]; 
                }
            }
            else
            {
                s.Clear();
                for (int i = 0; i < num_point; i++)
                {
                    s.AddXY(xvalues[i], ans[i*step]);  
                }
            }
            wave_chart.ChartAreas[0].RecalculateAxesScale();
        }



        private void update_freq_chart()
        {
            var s = this.freq_chart.Series[0].Points;
            double df = fs / tap / 2;
            var gains = data[ch].gains;

            if (s.Count == tap + 1)
            {
                for (int i = 0; i <= tap; i++)
                {
                    s[i].YValues[0] = gains[i];
                }
            }
            else
            {
                s.Clear();
                for (int i = 0; i <= tap; i++)
                {
                    s.AddXY(i * df, gains[i]);
                }
            }
        }

        private void update_filter_chart()
        {
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
            double n = nfft / 2 + 1;
            
            if (s.Count == n)
            {
                for (int i = 0; i < n; i++)
                {
                    // Same s.Count implies same xvalues.
                    s[i].YValues[0] = amps[i];
                }
            }
            else
            {
                s.Clear();
                for (int i = 0; i < n; i++)
                {
                    s.AddXY(df * i, amps[i]);
                }
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
                    data[ch].num_disp = value;
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
        private double[] xvalues {  get { return data[ch].xvalues; } }

        private void update_wave_chart_source()
        {
            int pos = data_start.Value;
            double dt = famos.dt(ch);
            double x0 = famos.data_types[ch].x0;

            num_point = Math.Min(num_disp, num_data - pos - 1) / step;
            var s = wave_chart.Series[0].Points;
            var vals = data[ch].source;
            if (s.Count == num_point)
            {
                for (int i = 0; i < num_point; i++)
                {
                    int k = i * step;
                    s[i].XValue = xvalues[k];
                    s[i].YValues[0] = vals[k];
                }
                wave_chart.ChartAreas[0].RecalculateAxesScale();
            }
            else
            {
                s.Clear();
                for (int i = 0; i < num_point; i++)
                {
                    int k = i * step;
                    s.AddXY(xvalues[k], vals[k]);
                }
            }
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

            data = new List<WaveData>(famos.cols);

            for (int i = 0; i < famos.cols; i++)
            {
                progressBar1.Value = i + 2;
                var wave = new WaveData(famos, i);
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

        private void update_display_data_length()
        {
            int val;
            if (int.TryParse(display_data_length.Text, out val))
            {
                if (val > num_data)
                    num_disp = num_data;
                else
                    num_disp = val;

                step = 1 + ((num_disp - 1) / max_points);
                data_start.LargeChange = num_disp / 5;

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
            data_length.Text = count.ToString();
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
            update_wave_chart_source();
            CheckUpdate();
        }

        private void gain_TextChanged(object sender, EventArgs e)
        {
            update_gain();
        }

        private void umi_Click(object sender, EventArgs e)
        {
            search_channels("UMI");
            CheckUpdate();
        }

        private void yama_Click(object sender, EventArgs e)
        {
            search_channels("YAMA");
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


    }
}
