using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace WaveFilterTool
{
  public partial class WaveFilterView : Form
  {
    private readonly WaveDataAndConfigs conf;

    public WaveFilterView()
    {
      InitializeComponent();
      conf = new WaveDataAndConfigs();
      conf.PropertyChanged += new PropertyChangedEventHandler(data_PropertyChanged);
      conf.AddPropetryChangedHandlerToDataSet(new PropertyChangedEventHandler(dataset_PropertyChanged));
      this.dataBindingSource.Add(conf);

      this.SetBounds(100, 100, groupBox1.Right + spChart.Width, groupBox1.Bottom + waveChart.Height+ statusStrip1.Height);

      conf.TapMax = 300;
      conf.Tap = 150;
    }


    void dataset_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (e.PropertyName == "source")
      {
        waveChart.Series[0].Points.DataBindXY(conf.XValues, conf.SourceWave);
        spChart.Series[0].Points.DataBindXY(conf.Frequencies, conf.PowerOfSource);
      }
      if (e.PropertyName == "filtered")
      {
        waveChart.Series[1].Points.DataBindXY(conf.XValues, conf.FilterdWave);
        spChart.Series[1].Points.DataBindXY(conf.Frequencies, conf.PowerOfFiltered);
      }
    }

    void data_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (e.PropertyName == "ChannelNames") updateChannelListBox();
      if (e.PropertyName == "Tap") updateTap();
    }

    private void updateTap()
    {
      int tap = conf.Tap;
      int tick = (tap + 1) / 5;
      upperFcTrackBar.TickFrequency = tick;
      lowerFcTrackBar.TickFrequency = tick;
    }


    void updateChannelListBox()
    {
      this.channelListBox.Items.Clear();
      foreach (var name in conf.ChannelNames)
      {
        this.channelListBox.Items.Add(name);
      }
    }

    private void waveFilterView_Resize(object sender, EventArgs e)
    {
      spChart.Width = this.ClientSize.Width - spChart.Left - 20;

      if (this.ClientSize.Height > waveChart.Top - statusStrip1.Height)
      {
        waveChart.Size = new System.Drawing.Size(this.ClientSize.Width, this.ClientSize.Height - waveChart.Top - statusStrip1.Height);
      }
    }

    private void tapNumericUpDown_Validating(object sender, CancelEventArgs e)
    {
      if (tapNumericUpDown.Value > conf.TapMax)
      {
        conf.TapMax = (int)tapNumericUpDown.Value;
      }
    }

    private void readFamosFileToolStripMenuIte_Click(object sender, EventArgs e)
    {
      if (openFamosFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.Cancel)
        return;
      conf.WaveFile = new WaveFile.Famos(openFamosFileDialog.FileName);
    }

    private void readKyowaFormatCsvFileToolStripMenuItem_Click(object sender, EventArgs e)
    {
      if (openCsvFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.Cancel)
        return;
      conf.WaveFile = WaveFile.DelimFile.OpenKyowaCsv(openCsvFileDialog.FileName);
    }

    private void currentChannelListBox_SelectedIndexChanged(object sender, EventArgs e)
    {
      conf.CurrentChannel = channelListBox.SelectedIndex;
      conf.Update();
    }

    private void terminateToolStripMenuItem_Click(object sender, EventArgs e)
    {
      this.Close();
    }

    private void readGeneralCsvFileToolStripMenuItem_Click(object sender, EventArgs e)
    {
      if (openCsvFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.Cancel)
        return;
      conf.WaveFile = WaveFile.DelimFile.OpenGeneralCsv(openCsvFileDialog.FileName);
    }

    private void numberOfDisplayedDataComboBox_TextChanged(object sender, EventArgs e)
    {
      if (int.TryParse(this.numberOfDisplayedDataComboBox.Text, out int value))
      {
        conf.NumberOfDisplayedData = value;
        conf.Update();
      }
    }

    private void gainComboBox_SelectedIndexChanged(object sender, EventArgs e)
    {
      if (double.TryParse(this.gainComboBox.Text, out double value))
      {
        conf.Gain = value;
        conf.Update();
      }
    }

    private void tapNumericUpDown_ValueChanged(object sender, EventArgs e)
    {
      conf.Tap = (int)tapNumericUpDown.Value;
      conf.Update();
    }

    private void upperFcNumericUpDown_ValueChanged(object sender, EventArgs e)
    {
      conf.UpperCutOffFrequencyIndex = (int)upperFcNumericUpDown.Value;
      conf.Update();
    }

    private void lowerFcNumericUpDown_ValueChanged(object sender, EventArgs e)
    {
      conf.LowerCutOffFrequencyIndex = (int)lowerFcNumericUpDown.Value;
      conf.Update();
    }
  }
}
