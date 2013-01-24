using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using fftwlib;

namespace WaveViewerWithFilering
{
    public partial class wave_filter : Form
    {
        public wave_filter()
        {
            InitializeComponent();
            fir = new FIRFilter();
            update_display_data_length();
            update_tap_info();
            source = new double[max_points];
            xvalues = new double[max_points];
            upper_fc_track.Value = tap_track.Value;
            lower_fc_track.Value = 0;
            update_gain();
        }
        
        //-----------------------------------------------------------------//

        int ch;
        int num_disp;
        int num_data;
        int nfft;
        int tap;
        int filter_size;

        int num_point;
        int step;
        const int max_points = 5000;


        private double dt;
        private double fs;
        private double fn;
        private Famos famos;
        private double[] data;
        private double[] source;
        private double[] xvalues;
        private double[] wave;
        private double[] ans;
        private int source_start;
        IntPtr pin, pout, plan;
    

        FIRFilter fir;

        private void data_update()
        {
            if (famos == null)
                return;

            update_filter_info();


            obtain_source();


            var f = filter_chart.Series[0].Points;
            var s = filter_chart.Series[1].Points;
            f.Clear();
            s.Clear();
            for (int i = -tap; i < tap; i++)
            {
                f.Add(fir.factor[Math.Abs(i)]);
                s.Add(fir.window[i]);
            }


            s = this.freq_chart.Series[0].Points;
            double df = fs / tap / 2;
            s.Clear();
            for (int i = 0; i < tap; i++)
            {
                if (!(double.IsNaN(fir.gains[i]) || double.IsInfinity(fir.gains[i])))
                    s.AddXY(i * df, fir.gains[i]);
            }
        }

        private void CheckUpdate()
        {
            if (auto_update.Checked)
            {
                data_update();
            }
        }

        private void obtain_source()
        {
            if (famos == null)
                return;

            data = famos[ch];
            int pos = data_start.Value;


            double dt = famos.dt(ch);
            double x0 = famos.data_types[ch].x0;

            num_point = Math.Min(num_disp, num_data - pos - 1) / step;
            var s = wave_chart.Series[0].Points;
            if (s.Count == num_point)
            {
                for (int i = 0; i < num_point; i++)
                {
                    int x = i * step + pos;
                    s[i].XValue = x0 + dt * x;
                    s[i].YValues[0] = data[x];
                }
                wave_chart.ChartAreas[0].RecalculateAxesScale();
            }
            else
            {
                s.Clear();
                for (int i = 0; i < num_point; i++)
                {
                    int x = i * step + pos;
                    s.AddXY(x0 + dt * x, data[x]);
                }
            }


            int n_start = pos - tap * 2;
            int nfil = 0;
            int last = Math.Min(nfft, num_data - n_start);
            if (n_start < 0)
            {
                nfil = n_start * -1;
                n_start = 0;
                for (int i = 0; i < nfil; i++)
                {
                    wave[i * 2] = 0.0;
                }
                for (int i = nfil; i < last; i++)
                {
                    wave[i*2] = data[i - nfil];
                }
                for (int i = last; i < nfft; i++)
                {
                    wave[i * 2] = 0.0;
                }
            }
            else if (n_start + nfft > num_data)
            {
                for (int i = 0; i < last; i++)
                {
                    wave[i*2] = data[i + n_start];
                }
                for (int i = last; i < nfft; i++)
                {
                    wave[i * 2] = 0.0;
                }
            }
            else
            {
                for (int i = 0; i < nfft; i++)
                {
                    wave[i * 2] = data[i + n_start];
                }
            }
            
        }

        private void channel_change()
        {
            ch = channel_track.Value;
            channel.Text = ch.ToString();

            channel_name.Text = famos.channel_info[ch].name;
            channel_comment.Text = famos.channel_info[ch].comment;

            num_data = famos.len(ch);
            data_length.Text = num_data.ToString();
            dt = famos.dt(ch);
            fs = 1.0 / dt;
            sampling_rate.Text = fs.ToString();
            fn = fs / 2.0;
            nyquist_frequency.Text = fn.ToString();

            freq_chart.ChartAreas[0].AxisX.Maximum = fn;

            data_start.Maximum = num_data - num_disp;
            update_lower_fc();
            update_upper_fc();
            update_display_data_length();
            obtain_source();
        }


        private void update_filter_info()
        {
            fir.design(); // refresh filter if needed
        }

        private void update_tap_info()
        {
            tap = tap_track.Value;
            if (lower_fc_track.Value > tap)
                lower_fc_track.Value = tap;
            lower_fc_track.Maximum = tap;
            if (upper_fc_track.Value > tap)
                upper_fc_track.Value = tap;
            upper_fc_track.Maximum = tap;

            fir.tap = tap_track.Value;
            number_of_tap.Text = fir.tap.ToString();
            filter_size = fir.tap * 2 - 1;
            filter_length.Text = filter_size.ToString();
            update_nfft();
        }

        private void set_alpha()
        {
            fir.alpha = double.Parse(alpha.Text);
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
                update_nfft();
            }
        }

        private void update_nfft()
        {
            //            nfft = num_disp + 2 * tap;
            nfft = 1024;
            while (nfft < num_disp + tap * 4)
                nfft *= 2;
            wave = new double[nfft * 2];
        }


        private void update_upper_fc()
        {
            upper_fc.Text = (upper_fc_track.Value * fn / tap).ToString();
            if (upper_fc_track.Value <= lower_fc_track.Value)
            {
                lower_fc_track.Value = upper_fc_track.Value - 1;
            }
            fir.upper = upper_fc_track.Value;
        }

        private void update_lower_fc()
        {
            lower_fc.Text = (lower_fc_track.Value * fn / tap).ToString();
            if (lower_fc_track.Value >= upper_fc_track.Value)
            {
                upper_fc_track.Value = lower_fc_track.Value + 1;
            }
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

            famos = new Famos(openFamosDialog.FileName);

            if (!famos.opened)
                return;

            channel_track.Value = 0;
            channel_track.Maximum = famos.cols - 1;

//            upper_fc_track.Value = upper_fc_track.Maximum;
            channel_change();
            CheckUpdate();
        }

        private void display_data_length_TextChanged(object sender, EventArgs e)
        {
            update_display_data_length();
            CheckUpdate();
        }


        private void kaiser_window_CheckedChanged(object sender, EventArgs e)
        {
            set_alpha();
            CheckUpdate();
        }

        private void alpha_TextChanged(object sender, EventArgs e)
        {
            if(kaiser_window.Checked )
                set_alpha();
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

        private void data_start_Scroll(object sender, ScrollEventArgs e)
        {
            obtain_source();
            CheckUpdate();
        }

        private void data_start_ValueChanged(object sender, EventArgs e)
        {
            obtain_source();
            CheckUpdate();
        }

        private void gain_TextChanged(object sender, EventArgs e)
        {
            update_gain();
        }


    }
}
