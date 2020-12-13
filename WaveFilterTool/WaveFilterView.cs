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
      conf.PropertyChanged += new PropertyChangedEventHandler(Data_PropertyChanged);
      conf.AddPropetryChangedHandlerToDataSet(new PropertyChangedEventHandler(Dataset_PropertyChanged));
      this.dataBindingSource.Add(conf);
      AdjustSizeAndLocationOfControls();

      conf.TapMax = 300;
      conf.Tap = 150;
    }


    void Dataset_PropertyChanged(object sender, PropertyChangedEventArgs e)
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

    void Data_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (e.PropertyName == "ChannelNames") UpdateChannelListBox();
      if (e.PropertyName == "Tap") UpdateTap();
    }

    private void UpdateTap()
    {
      int tap = conf.Tap;
      int tick = (tap + 1) / 5;
      upperFcTrackBar.TickFrequency = tick;
      lowerFcTrackBar.TickFrequency = tick;
    }


    void UpdateChannelListBox()
    {
      this.channelListBox.Items.Clear();
      foreach (var name in conf.ChannelNames)
      {
        this.channelListBox.Items.Add(name);
      }
    }

    private void AdjustSizeAndLocationOfControls()
    {
      var half_w = this.Width / 2;
      wave_info.Width = half_w;
      spChart.Width = half_w;
      spChart.Left = half_w;

      waveChart.Height = statusStrip1.Top - waveChart.Top; //   Math.Max(100, this.Height - wave_chart.Top - statusStrip1.Height);
      waveChart.Width = this.Width;
    }

    private void WaveFilterView_Resize(object sender, EventArgs e)
    {
      AdjustSizeAndLocationOfControls();
    }

    private void TapNumericUpDown_Validating(object sender, CancelEventArgs e)
    {
      if (tapNumericUpDown.Value > conf.TapMax)
      {
        conf.TapMax = (int)tapNumericUpDown.Value;
      }
    }

    private void FamosファイルToolStripMenuItem_Click(object sender, EventArgs e)
    {
      if (openFamosFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.Cancel)
        return;
      conf.WaveFile = new WaveFile.Famos(openFamosFileDialog.FileName);
    }

    private void 共和電業形式ToolStripMenuItem_Click(object sender, EventArgs e)
    {
      if (openCsvFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.Cancel)
        return;
      conf.WaveFile = WaveFile.DelimFile.OpenKyowaCsv(openCsvFileDialog.FileName);
    }

    private void CurrentChannelListBox_SelectedIndexChanged(object sender, EventArgs e)
    {
      conf.CurrentChannel = channelListBox.SelectedIndex;
      conf.Update();
    }

    private void 終了ToolStripMenuItem_Click(object sender, EventArgs e)
    {
      this.Close();
    }

    private void CSVファイルToolStripMenuItem_Click(object sender, EventArgs e)
    {
      if (openCsvFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.Cancel)
        return;
      conf.WaveFile = WaveFile.DelimFile.OpenGeneralCsv(openCsvFileDialog.FileName);
    }

    private void NumberOfDisplayedDataComboBox_TextChanged(object sender, EventArgs e)
    {
      if (int.TryParse(this.numberOfDisplayedDataComboBox.Text, out int value))
      {
        conf.NumberOfDisplayedData = value;
        conf.Update();
      }
    }

    private void GainComboBox_SelectedIndexChanged(object sender, EventArgs e)
    {
      if (double.TryParse(this.gainComboBox.Text, out double value))
      {
        conf.Gain = value;
        conf.Update();
      }
    }

    private void TapNumericUpDown_ValueChanged(object sender, EventArgs e)
    {
      conf.Tap = (int)tapNumericUpDown.Value;
      conf.Update();
    }

    private void UpperFcNumericUpDown_ValueChanged(object sender, EventArgs e)
    {
      conf.UpperCutOffFrequencyIndex = (int)upperFcNumericUpDown.Value;
      conf.Update();
    }

    private void LowerFcNumericUpDown_ValueChanged(object sender, EventArgs e)
    {
      conf.LowerCutOffFrequencyIndex = (int)lowerFcNumericUpDown.Value;
      conf.Update();
    }
  }
}
