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
            nfft = 0;
            wave_dirty = true;
            filter_dirty = true;
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
        IntPtr pwin, pwout, plan_w;  // wave
        IntPtr pfin, pfout, plan_f;  // filter
        IntPtr prin, prout, plan_r;  // result

        bool wave_dirty;
        bool filter_dirty;

        FIRFilter fir;
        private double[] factors;
        private double[] sp_factors;
        private double[] sp_wave;
        private double[] sp_ans;

        private void data_update()
        {
            if (famos == null)
                return;

            update_filter_info();
            obtain_source();
            apply_filter();
            update_wave_chart_filtered();
            update_filter_chart();
            update_freq_chart();

        }

        private void update_wave_chart_filtered()
        {
            var s = wave_chart.Series[1].Points;
            if (s.Count == num_point)
            {
                // just overwrite
                for (int i = 0; i < num_point; i++)
                {
                    int k = 2 * (i + 3 * tap);
                    s[i].XValue = xvalues[i]; // 3 * tap - 1 is delay and offset
                    s[i].YValues[0] = ans[k] / nfft; 
                }
            }
            else
            {
                s.Clear();
                for (int i = 0; i < num_point; i++)
                {
                    int k = 2 *  (i + 3 * tap);// 3 * tap  is delay and offset
                    s.AddXY(xvalues[i], ans[k] / nfft);  
                }
            }
            wave_chart.ChartAreas[0].RecalculateAxesScale();
        }

        private void apply_filter()
        {
            for (int i = 0; i < nfft; i++)
            {
                // multiply complex value
                double a = sp_wave[i * 2];
                double b = sp_wave[i * 2 + 1];
                double c = sp_factors[i * 2];
                double d = sp_factors[i * 2 + 1];
                sp_ans[i * 2] = a * c - b * d;
                sp_ans[i * 2 + 1] = a * d + b * c;
            }
            Marshal.Copy(sp_ans, 0, prin, nfft * 2);
            fftw.execute(plan_r);
            Marshal.Copy(prout, ans, 0, nfft * 2);
        }

        private void update_freq_chart()
        {
            var s = this.freq_chart.Series[0].Points;
            double df = fs / tap / 2;
            s.Clear();
            for (int i = 0; i < tap; i++)
            {
                if (!(double.IsNaN(fir.gains[i]) || double.IsInfinity(fir.gains[i])))
                    s.AddXY(i * df, fir.gains[i]);
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
                f.Add(fir.factor[Math.Abs(i)]);
                s.Add(fir.window[i]);
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

            if (!wave_dirty)
                return;

            data = famos[ch];

            update_wave_chart_source();
            generate_sp_wave();
            wave_dirty = false;
        }

        private void generate_sp_wave()
        {
            setup_wave();

            Marshal.Copy(wave, 0, pwin, nfft * 2);
            fftw.execute(plan_w);
            Marshal.Copy(pwout, sp_wave, 0, nfft * 2);
        }

        private void setup_wave()
        {
            int pos = data_start.Value;
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
                    wave[i * 2] = data[i - nfil];
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
                    wave[i * 2] = data[i + n_start];
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

        private void update_wave_chart_source()
        {
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
                    xvalues[i] = x0 + dt * x;
                    s[i].XValue = xvalues[i];
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
                    xvalues[i] = x0 + dt * x;
                    s.AddXY(xvalues[i], data[x]);
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
            wave_dirty = true;
            obtain_source();
        }


        private void update_filter_info()
        {
            if (fir.is_dirty || filter_dirty )
            {
                fir.design(); // refresh filter if needed

                Array.Clear(factors, 0, 2 * nfft);
                for (int i = 0; i < filter_size; i++)
                {
                    int k = Math.Abs(i - tap);
                    factors[i * 2] = fir.factor[k];
                }

                Marshal.Copy(factors, 0, pfin, nfft * 2);
                fftw.execute(plan_f);
                Marshal.Copy(pfout, sp_factors, 0, nfft * 2);
                filter_dirty = false;
            }
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
            int val = 1024;
            while (val < num_disp + tap * 4)
                val *= 2;
            if (val != nfft)
            {
                nfft = val;
                wave = new double[nfft * 2];
                factors = new double[nfft * 2];
                sp_factors = new double[nfft * 2];
                sp_wave = new double[nfft * 2];
                sp_ans = new double[nfft * 2];
                ans = new double[nfft * 2];
                fftw_setup();
            }
        }

        private void fftw_setup()
        {
            if (pwin != null)
                fftw_free();
            pwin = fftw.malloc(sizeof(double) * nfft * 2);
            pfin = fftw.malloc(sizeof(double) * nfft * 2);
            prin = fftw.malloc(sizeof(double) * nfft * 2);
            pwout = fftw.malloc(sizeof(double) * nfft * 2);
            pfout = fftw.malloc(sizeof(double) * nfft * 2);
            prout = fftw.malloc(sizeof(double) * nfft * 2);
            plan_w = fftw.dft_1d(nfft, pwin, pwout, fftw_direction.Forward, fftw_flags.Estimate);
            plan_f = fftw.dft_1d(nfft, pfin, pfout, fftw_direction.Forward, fftw_flags.Estimate);
            plan_r = fftw.dft_1d(nfft, prin, prout, fftw_direction.Backward, fftw_flags.Estimate);

            wave_dirty = true;
            filter_dirty = true;
        }

        private void fftw_free()
        {
            fftw.free(pwin);
            fftw.free(pwout);
            fftw.free(pfin);
            fftw.free(pfout);
            fftw.free(prin);
            fftw.free(prout);
            fftw.destroy_plan(plan_w);
            fftw.destroy_plan(plan_f);
            fftw.destroy_plan(plan_r);
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
            update_wave_chart_source();
            CheckUpdate();
        }

        private void data_start_ValueChanged(object sender, EventArgs e)
        {
            wave_dirty = true;
            obtain_source();
            CheckUpdate();
        }

        private void gain_TextChanged(object sender, EventArgs e)
        {
            update_gain();
        }





    }
}
