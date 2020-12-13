using System;
using System.ComponentModel;
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
      data.PropertyChanged += new PropertyChangedEventHandler(data_PropertyChanged);
      data.AddPropetryChangedHandlerToDataSet(new PropertyChangedEventHandler(dataset_PropertyChanged));
      this.dataBindingSource.Add(data);
      AdjustSizeAndLocationOfControls();

      data.TapMax = 300;
      data.Tap = 150;
    }


    void dataset_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (e.PropertyName == "source")
      {
        wave_chart.Series[0].Points.DataBindXY(data.XValues, data.SourceWave);
        sp_chart.Series[0].Points.DataBindXY(data.Frequencies, data.PowerOfSource);
      }
      if (e.PropertyName == "filtered")
      {
        wave_chart.Series[1].Points.DataBindXY(data.XValues, data.FilterdWave);
        sp_chart.Series[1].Points.DataBindXY(data.Frequencies, data.PowerOfFiltered);
      }
    }

    void data_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (e.PropertyName == "ChannelNames") UpdateChannelListBox();
      if (e.PropertyName == "Tap") UpdateTap();
    }

    private void UpdateTap()
    {
      int tap = data.Tap;
      int tick = (tap + 1) / 5;
      upperFcTrackBar.TickFrequency = tick;
      lowerFcTrackBar.TickFrequency = tick;
    }


    void UpdateChannelListBox()
    {
      this.channelListBox.Items.Clear();
      foreach (var name in data.ChannelNames)
      {
        this.channelListBox.Items.Add(name);
      }
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

    private void currentChannelListBox_SelectedIndexChanged(object sender, EventArgs e)
    {
      data.CurrentChannel = channelListBox.SelectedIndex;
    }

    private void 終了ToolStripMenuItem_Click(object sender, EventArgs e)
    {
      this.Close();
    }
  }
}
