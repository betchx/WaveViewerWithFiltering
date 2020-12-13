using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace WaveFilterTool
{
  public partial class WaveFilterView : Form
  {
    private readonly WaveDataAndConfigs data_;
    public WaveFilterView()
    {
      InitializeComponent();
      data_ = new WaveDataAndConfigs();
      data_.PropertyChanged += new PropertyChangedEventHandler(Data_PropertyChanged);
      data_.AddPropetryChangedHandlerToDataSet(new PropertyChangedEventHandler(Dataset_PropertyChanged));
      this.dataBindingSource.Add(data_);
      AdjustSizeAndLocationOfControls();

      data_.TapMax = 300;
      data_.Tap = 150;
    }


    void Dataset_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (e.PropertyName == "source")
      {
        wave_chart.Series[0].Points.DataBindXY(data_.XValues, data_.SourceWave);
        sp_chart.Series[0].Points.DataBindXY(data_.Frequencies, data_.PowerOfSource);
      }
      if (e.PropertyName == "filtered")
      {
        wave_chart.Series[1].Points.DataBindXY(data_.XValues, data_.FilterdWave);
        sp_chart.Series[1].Points.DataBindXY(data_.Frequencies, data_.PowerOfFiltered);
      }
    }

    void Data_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (e.PropertyName == "ChannelNames") UpdateChannelListBox();
      if (e.PropertyName == "Tap") UpdateTap();
    }

    private void UpdateTap()
    {
      int tap = data_.Tap;
      int tick = (tap + 1) / 5;
      upperFcTrackBar.TickFrequency = tick;
      lowerFcTrackBar.TickFrequency = tick;
    }


    void UpdateChannelListBox()
    {
      this.channelListBox.Items.Clear();
      foreach (var name in data_.ChannelNames)
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

    private void TapNumericUpDown_Validating(object sender, CancelEventArgs e)
    {
      if (tapNumericUpDown.Value > data_.TapMax)
      {
        data_.TapMax = (int)tapNumericUpDown.Value;
      }
    }

    private void FamosファイルToolStripMenuItem_Click(object sender, EventArgs e)
    {
      if (openFamosFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.Cancel)
        return;
      data_.WaveFile = new WaveFile.Famos(openFamosFileDialog.FileName);
    }

    private void 共和電業形式ToolStripMenuItem_Click(object sender, EventArgs e)
    {
      if (openCsvFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.Cancel)
        return;
      data_.WaveFile = WaveFile.DelimFile.OpenKyowaCsv(openCsvFileDialog.FileName);
    }

    private void CurrentChannelListBox_SelectedIndexChanged(object sender, EventArgs e)
    {
      data_.CurrentChannel = channelListBox.SelectedIndex;
    }

    private void 終了ToolStripMenuItem_Click(object sender, EventArgs e)
    {
      this.Close();
    }

    private void CSVファイルToolStripMenuItem_Click(object sender, EventArgs e)
    {
      if (openCsvFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.Cancel)
        return;
      data_.WaveFile = WaveFile.DelimFile.OpenGeneralCsv(openCsvFileDialog.FileName);
    }

    private void WaveFilterView_Load(object sender, EventArgs e)
    {

    }
  }
}
