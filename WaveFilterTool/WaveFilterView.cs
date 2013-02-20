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
            AdjustSizeAndLocationOfControls();
        }

        private void AdjustSizeAndLocationOfControls()
        {
            var half_w = this.Width / 2;
            wave_info.Width = half_w;
            sp_chart.Width = half_w;
            sp_chart.Left = half_w;

            wave_chart.Height = Math.Max(100, this.Height - wave_chart.Top - statusStrip1.Height);
            wave_chart.Width = this.Width;
        }

        private void WaveFilterView_Resize(object sender, EventArgs e)
        {
            AdjustSizeAndLocationOfControls();
        }

        private void tapNumericUpDown_Validating(object sender, CancelEventArgs e)
        {
            if (tapNumericUpDown.Value > data.TapMax)
            {
                data.TapMax = (int)tapNumericUpDown.Value;
            }
        }

        private void famosファイルToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFamosFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.Cancel)
                return;
            data.WaveFile = new WaveFile.Famos(openFamosFileDialog.FileName);
        }

        private void 共和電業形式ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openCsvFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.Cancel)
                return;
            data.WaveFile = WaveFile.DelimFile.KyowaCsv(openCsvFileDialog.FileName);
        }

        private void 一般ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openCsvFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.Cancel)
                return;
            data.WaveFile = WaveFile.DelimFile.GeneralCsv(openCsvFileDialog.FileName);
        }
    }
}
