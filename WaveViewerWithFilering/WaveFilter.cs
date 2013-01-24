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
        }
        
        //-----------------------------------------------------------------//

        int ch;
        int num_disp;
        int nfft;
        int tap;
        int filter_size;


        private double dt;
        private double fs;
        private double fn;
        private Famos famos;
        //private double[] data;

        FIRFilter fir;

        private void data_update()
        {
            //data = famos[ch];

            wave_chart.Series[0].Points.Clear();
            foreach (var y in fir.factor)
            {
                wave_chart.Series[0].Points.Add(y);
            }

        }

        private void CheckUpdate()
        {
            if (auto_update.Checked)
            {
                data_update();
            }
        }
        private void channel_change()
        {
            ch = channel_track.Value;
            channel.Text = ch.ToString();

            channel_name.Text = famos.channel_info[ch].name;
            channel_comment.Text = famos.channel_info[ch].comment;

            data_length.Text = famos.len(ch).ToString();
            dt = famos.dt(ch);
            fs = 1.0 / dt;
            sampling_rate.Text = fs.ToString();
            fn = fs / 2.0;
            nyquist_frequency.Text = fn.ToString();
            upper_fc_track.Maximum = (int)(fn);
            lower_fc_track.Maximum = (int)(fn);
        }


        private void update_filter_info()
        {
            fir.design(); // refresh filter if needed
        }


        //-----------------------------------------------------------------//

        private void tap_track_Scroll(object sender, EventArgs e)
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
            nfft = 1024;
            while (nfft < num_disp + tap * 4)
                nfft *= 2;
             
            CheckUpdate();
        }

        private void upper_fc_track_Scroll(object sender, EventArgs e)
        {
            upper_fc.Text = (upper_fc_track.Value*fn/tap).ToString();
            CheckUpdate();
        }

        private void lower_fc_track_Scroll(object sender, EventArgs e)
        {
            lower_fc.Text = (lower_fc_track.Value*fn/tap).ToString();
            CheckUpdate();
        }

        private void update_Click(object sender, EventArgs e)
        {
            data_update();
        }

        private void channel_track_Scroll(object sender, EventArgs e)
        {
            channel_change();
        }

        private void select_file_Click(object sender, EventArgs e)
        {
            if (openFamosDialog.ShowDialog()== System.Windows.Forms.DialogResult.Cancel)
                return;

            famos = new Famos(openFamosDialog.FileName);

            if (!famos.opened)
                return;

            channel_track.Value = 0;
            channel_track.Maximum = famos.cols;

            channel_change();
            upper_fc_track.Value = upper_fc_track.Maximum;

            CheckUpdate();
        }

        private void display_data_length_TextChanged(object sender, EventArgs e)
        {
            num_disp = int.Parse(display_data_length.Text);
        }


 
    }
}
