using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace WaveFilterTool
{
    public class Data : System.ComponentModel.INotifyPropertyChanged
    {
        private WaveViewerWithFilering.WaveDataSet dataSet;

        public Data()
        {
            channelNames = new System.Collections.ObjectModel.ObservableCollection<string>();
            TapMax = 300;
            Gain = -80;
            Tap = 150;
            UpperFcNum = tap;
            LowerFcNum = 0;
            NumberOfDisplayedData = 1000;
        }

        private double dt;

        public double Dt
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
                if (dataSet != null) dataSet.tap = value;
                NotifyPropertyChanged("Tap");
            }
        }
        public double UpperFc { get; private set; }
        private int upperFcNum;

        public int UpperFcNum
        {
            get { return upperFcNum; }
            set
            {
                upperFcNum = value;
                if (dataSet != null)
                {
                    dataSet.upper = value;
                    dataSet.update();
                }
                NotifyPropertyChanged("UpperFcNum");
                if (dt != 0.0)
                {
                    UpperFc = upperFcNum / (2 * tap * dt);
                    NotifyPropertyChanged("UpperFc");
                }
            }
        }

        public double LowerFc { get; private set; }
        private int lowerFcNum;

        public int LowerFcNum
        {
            get { return lowerFcNum; }
            set
            {
                lowerFcNum = value;
                if (dataSet != null)
                {
                    dataSet.lower = value;
                    dataSet.update();
                }
                NotifyPropertyChanged("LowerFcNum");
                if (dt != 0.0)
                {
                    LowerFc = lowerFcNum / (2 * tap * dt);
                    NotifyPropertyChanged("LowerFc");
                }
            }
        }
        private double[][] waves;

        public double[][] Waves
        {
            get { return waves; }
            set { waves = value; }
        }

        public IEnumerable<double> SourceWave { get { dataSet.update(); return dataSet.source; } }
        public IEnumerable<double> FilterdWave { get { dataSet.update(); return dataSet.filtered; } }


        private double[][] spectrums;

        public double[][] Spectrums
        {
            get { return spectrums; }
            set { spectrums = value; }
        }
        public double[] Frequencies { get { return dataSet.freqs; } }
        public double[] PowerOfSource { get { return dataSet.Power(WaveViewerWithFilering.WaveDataSet.State.Source); } }
        public double[] PowerOfFiltered { get { return dataSet.Power(WaveViewerWithFilering.WaveDataSet.State.Filtered); } }

        private void NotifyPropertyChanged(string property_name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(property_name));
            }
        }

        public int TapMax { get; set; }

        private int currentChannel;

        public int CurrentChannel
        {
            get { return currentChannel; }
            set
            {
                currentChannel = value;
                if (waveFile != null)
                {
                    if (waveFile.cols <= currentChannel) currentChannel = waveFile.cols - 1;
                    dataSet = new WaveViewerWithFilering.WaveDataSet(waveFile, currentChannel);
                    dataSet.PropertyChanged += datasetHandler;
                    dt = dataSet.dt;
                    dataSet.tap = Tap;
                    dataSet.upper = upperFcNum;
                    dataSet.lower = lowerFcNum;
                    dataSet.num_disp = numberOfDisplayedData;
                    dataSet.gain = gain;
                    dataSet.update();
                }
                NotifyPropertyChanged("CurrentChannel");
            }
        }

        private System.Collections.ObjectModel.ObservableCollection<string> channelNames;

        public System.Collections.ObjectModel.ObservableCollection<string> ChannelNames
        {
            get { return channelNames; }
            private set { channelNames = value; }
        }

        
        private WaveFile.IWaveFile waveFile;

        public WaveFile.IWaveFile WaveFile
        {
            get { return waveFile; }
            set
            {
                waveFile = value;
                Dt = waveFile.dt(0);
                CurrentChannel = currentChannel;
                dataSet.num_disp = dataSet.length;
                UpdateChannelNames();
            }
        }

        private void UpdateChannelNames()
        {
            channelNames.Clear();
            for (int i = 0; i < waveFile.cols; i++)
            {
                channelNames.Add(waveFile.name(i));
            }
            NotifyPropertyChanged("ChannelNames");
        }

        private int numberOfDisplayedData;

        public int NumberOfDisplayedData
        {
            get { return numberOfDisplayedData; }
            set { numberOfDisplayedData = value; NotifyPropertyChanged("NumberOfDisplayedData");}
        }


        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
    }
}