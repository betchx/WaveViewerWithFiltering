using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace WaveFilterTool
{
    class Data : System.ComponentModel.INotifyPropertyChanged 
    {
        private WaveViewerWithFilering.WaveDataSet dataSet;

        public Data()
        {
            channelNames = new List<string>();
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
            set { upperFcNum = value;
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
                lowerFcNum = value; NotifyPropertyChanged("LowerFcNum");
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

                    NotifyPropertyChanged("Waves");
                    NotifyPropertyChanged("Spectrums");
                }
                NotifyPropertyChanged("CurrentChannel");
            }
        }

        private List<string> channelNames;


        
        private WaveFile.IWaveFile waveFile;

        public WaveFile.IWaveFile WaveFile
        {
            get { return waveFile; }
            set
            {
                waveFile = value;
                Dt = waveFile.dt(0);
                channelNames.Clear();
                for (int i = 0; i < waveFile.cols; i++)
                {
                    channelNames.Add(waveFile.name(i));                    
                }
                NotifyPropertyChanged("ChannelNames");
                CurrentChannel = currentChannel;
            }
        }

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
    }
}
