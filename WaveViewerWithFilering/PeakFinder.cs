using System;
using System.Collections.Generic;

namespace WaveViewerWithFilering
{
  class PeakFinder
  {
    readonly double th;
    readonly int num;
    int idx;
    double peak;
    int dur;
    public PeakFinder(double threshold, int required_count)
    {
      th = threshold;
      num = required_count;
      idx = 0;
      peak = 0.0;
      dur = 0;
    }

    public List<KeyValuePair<int, double>> Apply(double[] wave)
    {
      var ans = new List<KeyValuePair<int, double>>();
      for (int i = 0; i < wave.Length; i++)
      {
        var amp = Math.Abs(wave[i]);
        if (amp > th)
        {
          if (dur > 0)
          {
            if (peak < amp)
            {
              idx = i;
              peak = amp;
            }
          }
          else
          {
            idx = i;
            peak = amp;
          }
          dur += 1;
        }
        else
        {
          if (dur > num)
          {
            //store
            ans.Add(new KeyValuePair<int, double>(idx, peak));
            idx = 0;
            peak = 0.0;
          }
          dur = 0;
        }
      }

      return ans;
    }
  }
}
