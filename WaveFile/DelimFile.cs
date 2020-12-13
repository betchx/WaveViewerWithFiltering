using System;
using System.Linq;
using System.Text;
using System.IO;

namespace WaveFile
{
  public class DelimFile : IWaveFile
  {
    private readonly int cols_;
    private readonly int rows_;
    private readonly double[][] data_;
    private double[] x_;
    private double dt_;
    private double Fs_;
#pragma warning disable IDE0052, CS0414 // 読み取られていないプライベート メンバーを削除
    private string[] headers_;
#pragma warning restore IDE0052, CS0414 // 読み取られていないプライベート メンバーを削除
    private string[] names_;
    private string[] comments_;

    public DelimFile(int ncol)
    {
      cols_ = ncol;
      rows_ = 0;
      this.Opened = false;
    }

    public DelimFile(int ncol, int nrow)
    {
      cols_ = ncol;
      rows_ = nrow;
      data_ = Enumerable.Range(0, ncol).Select((i) => new double[nrow]).ToArray();
      this.Opened = false;
    }

    public string Title { get; private set; }

    public DateTime Time { get; private set; }

    public double SamplingRate { get { return Fs_; } set { Fs_ = value; dt_ = 1.0 / value; } }

    public double[] this[int index]
    {
      get
      {
        return data_[index];
      }
    }

    public double[] X
    {
      get { return x_; }
    }

    public double TimeIncrement(int i) { return dt_; }

    int IWaveFile.Cols
    {
      get { return this.cols_; }
    }

    int IWaveFile.Rows
    {
      get { return this.rows_; }
    }


    public string Name(int ch)
    {
      return names_[ch];
    }

    public string Comment(int ch)
    {
      return comments_[ch];
    }

    public bool Opened { get; private set; }

    // generation with file
    #region Kyowa
    private enum KyowaHeaderType
    {
      ID = 0,
      Title = 1,
      DateTime = 2,
      NumberOfChannels = 3,
      DigitalInput = 4,
      SamplingFrequency = 5,
      NumberOfData = 6,
      DurationInSecond = 7,
      ChannelNames = 8,
      ChannelNumbers = 9,
      Ranges = 10,
      CalibrationFactor = 11,
      Offsets = 12,
      Units = 13,
      NumberOfHeader = 14,
    }

    private static string[] Kyowa_header(string[] header, KyowaHeaderType target)
    {
      return header[(int)target].TrimEnd(',').Split(',').Skip(1).Select(s => s.Trim('"')).ToArray();
    }

    private static string Kyowa_header_str(string[] header, KyowaHeaderType target)
    {
      return string.Join(" ", Kyowa_header(header, target));
    }

    private static int Kyowa_header_int(string[] header, KyowaHeaderType target)
    {
      if (int.TryParse(Kyowa_header_str(header, target), out int res))
        return res;
      return 0;
    }

    public static bool IsKyowaCsv(string path)
    {
      bool res;
      using (var sr = new StreamReader(path, Encoding.Default))
      {
        var line = sr.ReadLine();
        if (line == null)
          res = false;
        else
        {
          var tag = line.Split(',').FirstOrDefault();
          // format check
          res = tag == "ID番号" || tag == "\"ID番号\"";
        }
      }
      return res;
    }

    public static DelimFile OpenKyowaCsv(string path)
    {
      // format check
      if (!IsKyowaCsv(path))
        throw new ArgumentException("共和電業のCSVファイルではありません.");

      string[] contents = File.ReadAllLines(path, Encoding.Default);

      int ncol = Kyowa_header_int(contents, KyowaHeaderType.NumberOfChannels);
      if (ncol == 0)
        throw new ArgumentException("ファイルに間違いがあります．(測定チャンネル数)");
      int nrow = Kyowa_header_int(contents, KyowaHeaderType.NumberOfData);

      // allocate
      DelimFile res = new DelimFile(ncol, nrow)
      {
        headers_ = contents.Take((int)KyowaHeaderType.NumberOfHeader).ToArray(),
        Title = Kyowa_header_str(contents, KyowaHeaderType.Title)
      };
      var date_time = Kyowa_header_str(contents, KyowaHeaderType.DateTime);
      if (DateTime.TryParse(date_time, out DateTime dtm))
      {
        res.Time = dtm;
      }
      else
      {
        var fi = new FileInfo(path);
        res.Time = fi.CreationTime;
      }
      res.SamplingRate = double.Parse(Kyowa_header_str(contents, KyowaHeaderType.SamplingFrequency));
      double dt = 1.0 / res.SamplingRate;
      res.x_ = Enumerable.Range(0, nrow).Select(i => i * dt).ToArray();
      res.dt_ = dt;

      res.names_ = Kyowa_header(contents, KyowaHeaderType.ChannelNames);
      // comments
      var cnames = Kyowa_header(contents, KyowaHeaderType.ChannelNames);
      var cnums = Kyowa_header(contents, KyowaHeaderType.ChannelNumbers);
      var ranges = Kyowa_header(contents, KyowaHeaderType.Ranges);
      var factors = Kyowa_header(contents, KyowaHeaderType.CalibrationFactor);
      var offsets = Kyowa_header(contents, KyowaHeaderType.Offsets);
      var units = Kyowa_header(contents, KyowaHeaderType.Units);
      //for (int i = 0; i < ncol; i++)
      res.comments_ = res.names_.Select((ttl, i) =>
      {
        return string.Format("{0} ({1}) レンジ:{2} 校正係数:{3} オフセット：{4}",
                  cnums[i], units[i], ranges[i], factors[i], offsets[i]);
      }).ToArray();

      // read data
      for (int row = 0; row < nrow; row++)
      {
        string line = contents[(int)KyowaHeaderType.NumberOfHeader + row];
        int col = 0;
        foreach (var s in line.Split(',').Skip(1))
        {
          double.TryParse(s, out res.data_[col++][row]);
        }
      }

      res.Opened = true;
      return res;
    }
    #endregion

    public static IWaveFile OpenGeneralCsv(string file_name)
    {
      string[] contents = File.ReadAllLines(file_name);
      double tmp;
      string[] headers = contents.TakeWhile(s => s.Length == 0 || s.Split(',').Any(c => !double.TryParse(c, out tmp))).ToArray();
      int n_header = headers.Length;
      int ncol = contents[headers.Length].Split(',').Length;
      int nrow = contents.Skip(n_header).TakeWhile(s => s.Split(',').Count() == ncol).Count();

      var res = new DelimFile(ncol, nrow);
      var titles = headers.Reverse().TakeWhile(s => s.Split(',').Count() == ncol).Reverse();
      res.names_ = Enumerable.Range(0, ncol).Select(i => string.Format("Ch {0}", i)).ToArray();
      if (titles.Count() == 0)
      {
        res.comments_ = res.names_;
      }
      else
      {
        res.comments_ = new string[ncol];
        var wk = titles.Select(str => str.Split(','));
        for (int i = 0; i < ncol; i++)
        {
          res.comments_[i] = wk.Select(a => a[i]).Aggregate(new StringBuilder(),
              (a, b) => a.Append(b)).ToString();
        }
      }
      // read data body
      for (int row = 0; row < nrow; row++)
      {
        var line = contents[row + n_header];
        int col = 0;
        foreach (var val in line.Split(','))
          double.TryParse(val, out res.data_[col++][row]);
      }
      double dt = res.data_[0][1] - res.data_[0][0];
      if (dt != 0.0)
      {
        res.SamplingRate = 1.0 / dt;
        double rounded = Math.Round(res.SamplingRate);
        if (Math.Abs(res.SamplingRate - rounded) < 1e-5)
        {
          res.SamplingRate = rounded;
        }
        res.x_ = Enumerable.Range(0, nrow).Select(i => i * dt).ToArray();
      }
      res.Opened = true;
      return res;
    }
  }
}
