﻿using System.Collections.Generic;
using System.Text;


namespace WaveFilterTool
{
  public class WaveDataAndConfigs : System.ComponentModel.INotifyPropertyChanged
  {
    private WaveViewerWithFilering.WaveDataSet dataSet;

    public WaveDataAndConfigs()
    {
      channelNames = new System.Collections.ObjectModel.ObservableCollection<string>();
      TapMax = 300;
      Gain = -80;
      Tap = 150;
      UpperCutOffFrequencyIndex = tap;
      LowerCutOffFrequencyIndex = 0;
      NumberOfDisplayedData = 1000;
    }

    private double dt;

    public double TimeIncrement
    {
      get { return dt; }
      set { dt = value; }
    }

    private int tap;

    public int Tap
    {
      get { return tap; }
      set
      {
        tap = value;
        if (dataSet != null) dataSet.Tap = value;
        notifyPropertyChanged("Tap");
      }
    }
    public double UpperCutOffFrequency { get; private set; }

    private int upperFcNum;

    public int UpperCutOffFrequencyIndex
    {
      get { return upperFcNum; }
      set
      {
        upperFcNum = value;
        if (dataSet != null)
        {
          dataSet.Upper = value;
          dataSet.Update();
        }
        notifyPropertyChanged("UpperCutOffFrequencyIndex");
        if (dt != 0.0)
        {
          UpperCutOffFrequency = upperFcNum / (2 * tap * dt);
          notifyPropertyChanged("UpperCutOffFrequency");
        }
      }
    }

    public double LowerCutOffFrequency { get; private set; }

    private int lowerFcNum;

    public int LowerCutOffFrequencyIndex
    {
      get { return lowerFcNum; }
      set
      {
        lowerFcNum = value;
        if (dataSet != null)
        {
          dataSet.Lower = value;
          dataSet.Update();
        }
        notifyPropertyChanged("LowerCutOffFrequencyIndex");
        if (dt != 0.0)
        {
          LowerCutOffFrequency = lowerFcNum / (2 * tap * dt);
          notifyPropertyChanged("LowerCutOffFrequency");
        }
      }
    }
    private double[][] waves;

    public double[][] Waves
    {
      get { return waves; }
      set { waves = value; }
    }

    public IEnumerable<double> SourceWave
    {
      get { dataSet.Update(); return dataSet.Source; }
    }

    public IEnumerable<double> FilterdWave
    {
      get { dataSet.Update(); return dataSet.Filtered; }
    }

    private double[][] spectrums;

    public double[][] Spectrums
    {
      get { return spectrums; }
      set { spectrums = value; }
    }

    public double[] Frequencies
    {
      get { return dataSet.Freqs; }
    }

    public double[] PowerOfSource
    {
      get { return dataSet.Power(WaveViewerWithFilering.WaveDataSet.State.Source); }
    }

    public double[] PowerOfFiltered
    {
      get { return dataSet.Power(WaveViewerWithFilering.WaveDataSet.State.Filtered); }
    }

    private void notifyPropertyChanged(string property_name)
    {
      PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(property_name));
    }

    private int tapMax;

    public int TapMax
    {
      get { return tapMax; }
      set { tapMax = value; notifyPropertyChanged("TapMax"); }
    }

    private int currentChannel;

    public int CurrentChannel
    {
      get { return currentChannel; }
      set
      {
        currentChannel = value;
        Update();
      }
    }

    private System.Collections.ObjectModel.ObservableCollection<string> channelNames;

    public System.Collections.ObjectModel.ObservableCollection<string> ChannelNames
    {
      get { return channelNames; }
      private set { channelNames = value; }
    }

    private double gain;

    public double Gain
    {
      get
      {
        if (dataSet != null)
          return dataSet.Gain;
        return gain;
      }
      set
      {
        gain = value;
        if (dataSet != null)
        {
          dataSet.Gain = value;
          dataSet.Update();
        }
        notifyPropertyChanged("Gain");
      }
    }

    private WaveFile.IWaveFile waveFile;

    public WaveFile.IWaveFile WaveFile
    {
      get { return waveFile; }
      set
      {
        waveFile = value;
        CurrentChannel = (value.Cols > 1) ? 1 : 0;
        Update();
        dataSet.DataStart = 0;
        dataSet.NumDisp = dataSet.Length;
        updateChannelNames();
        updateDescription();
        notifyPropertyChanged("WaveFile");
      }
    }

    public bool IsValid
    {
      get
      {
        dataSet.Update(); return dataSet.IsValid;
      }
    }

    private void updateDescription()
    {
      var builder = new StringBuilder(100);
      builder.AppendFormat("時間刻み：{0}\r\n", dt);
      builder.AppendFormat("チャンネル数：{0}\r\n", waveFile.Cols);
      builder.AppendFormat("データ長：{0} ({1}秒間)\r\n", waveFile.Rows, waveFile.Rows * dt);
      Description = builder.ToString();
      notifyPropertyChanged("Description");
    }

    private void updateChannelNames()
    {
      channelNames.Clear();
      for (int i = 0; i < waveFile.Cols; i++)
      {
        channelNames.Add(waveFile.Name(i));
      }
      notifyPropertyChanged("ChannelNames");
    }

    private int numberOfDisplayedData;

    public int NumberOfDisplayedData
    {
      get { return numberOfDisplayedData; }
      set { numberOfDisplayedData = value; notifyPropertyChanged("NumberOfDisplayedData"); }
    }

    System.ComponentModel.PropertyChangedEventHandler datasetHandler;

    public void AddPropetryChangedHandlerToDataSet(System.ComponentModel.PropertyChangedEventHandler handler)
    {
      datasetHandler = handler;
      if (dataSet != null)
        dataSet.PropertyChanged += datasetHandler;
    }

    public string Description { get; set; }

    public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

    public IEnumerable<double> XValues
    {
      get { return dataSet.Xvalues; }

    }

    public void Update()
    {
      if (waveFile != null)
      {
        if (waveFile.Cols <= currentChannel) currentChannel = waveFile.Cols - 1;
        dataSet = new WaveViewerWithFilering.WaveDataSet(waveFile, currentChannel);
        dataSet.PropertyChanged += datasetHandler;
        dt = dataSet.TimeIncrement;
        dataSet.Tap = Tap;
        dataSet.Upper = upperFcNum;
        dataSet.Lower = lowerFcNum;
        dataSet.NumDisp = numberOfDisplayedData;
        dataSet.Gain = gain;
        dataSet.Update();
      }
      notifyPropertyChanged("CurrentChannel");
    }
  }
}
