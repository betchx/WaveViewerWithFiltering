using System;

namespace WaveFile
{
  public interface IWaveFile
  {
    double TimeIncrement(int channel_number);
    int Cols { get; }
    int Rows { get; }
    double[] this[int ch] { get; }
    DateTime Time { get; }
    string Name(int ch);
    string Comment(int ch);
    bool Opened { get; }
  }
}
