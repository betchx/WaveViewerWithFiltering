using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WaveFile
{
    public interface IWaveFile
    {
        double dt(int channel_number);
        int cols { get; }
        int rows { get; }
        double[] this[int ch] { get; }
        DateTime Time { get; }
        string name(int ch);
        string comment(int ch);
        bool opened { get; }
    }
}
