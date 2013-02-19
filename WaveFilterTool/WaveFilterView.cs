using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WaveFilterTool
{
    public partial class WaveFilterView : Form
    {
        private Data data;
        public WaveFilterView()
        {
            InitializeComponent();
            data = new Data();
            this.dataBindingSource.Add(data);

        }

        private void WaveFilterView_Resize(object sender, EventArgs e)
        {
            var half_w = this.Width / 2;
            wave_info.Width = half_w;
            sp_chart.Width = half_w;
            sp_chart.Left = half_w;

            wave_chart.Height = Math.Max(100, this.Height - wave_chart.Top - statusStrip1.Height);
            wave_chart.Width = this.Width;
        }
    }
}
