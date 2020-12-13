using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace WaveFile
{
  public class Famos : IWaveFile
  {
    #region Construction
    // default constructor
    public Famos()
    { Init(); }

    // open given file automatically
    public Famos(string filename)
    {
      Init();
      OpenDat(filename);
    }
    #endregion

    #region Properties
    // Auto Implemented Properties
    public List<DataType> DataTypes { get; private set; }
    public List<Event> Events { get; private set; }
    public List<PacketInfo> PacketInfos { get; private set; }
    public List<BufferRef> BufferRefs { get; private set; }
    public List<ValueRange> ValueRanges { get; private set; }
    public List<ChannelInfo> ChannelInfos { get; private set; }
    public List<long> Origins { get; private set; }
    public List<Value> Values { get; private set; }
    public bool Opened { get; private set; }

    public DateTime Time { get; private set; }
    public int Cols { get { return ChannelInfos.Count; } }
    public int Rows { get { return this.Length(0); } }

    //Indexer
    public double[] this[int ch]
    {
      get
      {
        if (!columnCache.ContainsKey(ch))
          columnCache.Add(ch, ReadColumnAsDouble(ch));
        return columnCache[ch];
      }
    }
    #endregion

    #region SubClasses(Public)

    public class Event
    {
      public Event(int length, DateTime startTime)
      {
        this.Len = length;
        this.Start_time = startTime;
      }
      public int Len { get; private set; }
      public DateTime Start_time { get; private set; }
    }

    public class DataType
    {
      public DataType(double dx, bool cal, string unit, double x0 = 0.0, bool pre = false)
      {
        this.Dx = dx;
        this.Calibrated = cal;
        this.Unit = unit;
        this.X0 = x0;
        this.Pre_trigger = pre;
      }
      public double Dx { get; private set; }
      public bool Calibrated { get; private set; }
      public string Unit { get; private set; }
      public double X0 { get; private set; }
      public bool Pre_trigger { get; private set; }
    }

    public class PacketInfo
    {
      public PacketInfo(int buffer_ref_id, int bytes, int number_format, int significant_bits, int mask,
          int offset, int direct_sequence_number, int interval_bytes)
      {
        this.BufferRefId = buffer_ref_id;
        this.Bytes = bytes;
        this.NumberFormat = number_format;
        this.SignificantBits = significant_bits;
        this.Mask = mask;
        this.Offset = offset;
        this.DirectSequenceNumber = direct_sequence_number;
        this.IntervalBytes = interval_bytes;
      }
      public int BufferRefId { get; private set; }
      public int Bytes { get; private set; }
      public int NumberFormat { get; private set; }
      public int SignificantBits { get; private set; }
      public int Mask { get; private set; }
      public int Offset { get; private set; }
      public int DirectSequenceNumber { get; private set; }
      public int IntervalBytes { get; private set; }
    }

    public class BufferRef
    {
      public BufferRef(int num, int index, int offset, int size, int offset_sample,
          int buffer_filled_bytes, int x0, double add_time, string user_info)
      {
        this.Num = num;
        this.Index = index;
        this.Offset = offset;
        this.Size = size;
        this.OffsetSample = offset_sample;
        this.BufferFilledBytes = buffer_filled_bytes;
        this.X0 = x0;
        this.AddTime = add_time;
        this.UserInfo = user_info;
      }
      public int Num { get; private set; }
      public int Index { get; private set; }
      public int Offset { get; private set; }
      public int Size { get; private set; }
      public int OffsetSample { get; private set; }
      public int BufferFilledBytes { get; private set; }
      public int X0 { get; private set; }
      public double AddTime { get; private set; }
      public string UserInfo { get; private set; }
    }

    public class ValueRange
    {
      public ValueRange(bool trans, double factor, double offset, bool calibrated, string unit)
      {
        this.Transform = trans;
        this.Factor = factor;
        this.Offset = offset;
        this.Calibrated = calibrated;
        this.Unit = unit;
      }
      public bool Transform { get; private set; }
      public double Factor { get; private set; }
      public double Offset { get; private set; }
      public bool Calibrated { get; private set; }
      public string Unit { get; private set; }
    }

    public class ChannelInfo
    {
      public ChannelInfo(int index, int index_bit, string name, string comment)
      {
        this.Index = index;
        this.IndexBit = index_bit;
        this.Name = name;
        this.Comment = comment;
      }
      public int Index { get; private set; }
      public int IndexBit { get; private set; }
      public string Name { get; private set; }
      public string Comment { get; private set; }
    }

    public class Value
    {
      public Value(int index, int num_format, string name, int value, string unit, string comment, double time)
      {
        this.Index = index;
        this.NumFormat = num_format;
        this.Name = name;
        this.Data = value;
        this.Unit = unit;
        this.Comment = comment;
        this.Time = time;
      }
      public int Index { get; private set; }
      // NumFormat
      // 1 unsigned byte
      // 2 signed byte
      // 3 unsigned short
      // 4 signed short
      // 5 unsigned long
      // 6 singned long
      // 7 float
      // 8 double
      // 11 LSB in 2 byte-Word digital
      // 13 6bye unsigned long
      public int NumFormat { get; private set; }
      public string Name { get; private set; }
      public int Data { get; private set; }
      public string Unit { get; private set; }
      public string Comment { get; private set; }
      public double Time { get; private set; }
    }

    public class FamosReader
    {
      private readonly Famos parent_;
      public int Length { get; private set; }
      public FamosReader(Famos parent, int ch)
      {
        this.parent_ = parent;
        this.Length = parent_.Length(ch);
      }
    }
    #endregion

    #region PublicMethods

    public double Duration(int ch = 0, int event_id = 0)
    {
      if (Events.Count > 0)
        return EventDuration(ch, event_id);
      return Reader(ch).Length * TimeIncrement(ch);
    }

    public DateTime EventTime(int ch, int event_id)
    {
      return Events[EventIndex(ch, event_id)].Start_time;
    }

    public double TimeIncrement(int ch = 0)
    {
      return DataTypes[ch].Dx;
    }

    public static bool IsFamos(string path)
    {
      var tr = new System.IO.StreamReader(path);
      char[] buf = new char[TAG_CF.Length];
      bool ans = false;
      if (tr.Read(buf, 0, TAG_CF.Length) == TAG_CF.Length)
      {
        if (new string(buf) == TAG_CF)
          ans = true;
      }
      tr.Close();
      return ans;
    }

    public FamosReader Reader(int ch)
    {
      return new FamosReader(this, ch);
    }

    public DateTime GetTime(int ch = -1, int event_id = 0)
    {
      if (Events.Count > 0)
      {
        if (ch < 0) ch = 0;
        return EventTime(ch, event_id).AddSeconds(timeOffset_);
      }
      else
      {
        double time_shift = timeOffset_;
        if (ch >= 0)
          time_shift += BufferRefs[ch].AddTime;
        return time_.AddSeconds(time_shift);
      }
    }

    public bool OpenDat(string filename)
    {
      CheckAndCloseStream();

      stream_ = new System.IO.FileStream(filename, System.IO.FileMode.Open);
      reader_ = new BinaryReader(stream_, Encoding.Default);
      var t = new Tag(R);
      if (t.Key != "CF")
        throw new ArgumentException(filename + " is not Famos file");

      tags.Add(t);
      while (stream_.Position < stream_.Length)
      {
        tags.Add(new Tag(R));
      }

      // fix check
      var fix_file = filename + ".FIX";

      if (System.IO.File.Exists(fix_file))
      {
        // need fix
        var sr = new StreamReader(fix_file);
        while (!sr.EndOfStream)
        {
          var line = sr.ReadLine();
          var arr = line.Split(commaSeprator, 2);
          if (availableFixItems.ContainsKey(arr[0]))
          {
            availableFixItems[arr[0]].Add(arr[1]);
          }
        }
      }

      Opened = true;
      ParseTags();

      // update time
      Time = GetTime();

      return true;
    }

    public string Name(int ch)
    {
      return ChannelInfos[ch].Name;
    }

    public string Comment(int ch)
    {
      return ChannelInfos[ch].Comment;
    }

    #endregion

    #region InternalUse

    #region fields
    private readonly static char[] commaSeprator = { ',' };
    private readonly static char[] blankSeprator = { ' ', '\t', '\n' };
    private const string TAG_CF = "|CF,2,1,1;";
    private Dictionary<int, double[]> columnCache;
    private Dictionary<string, IFix> availableFixItems;
    private Dictionary<string, ITag> tagParsers;
    private DateTime time_;
    private double timeOffset_;
    private double[] valueOffsets_;
    private Stream stream_;
    private BinaryReader reader_;
    private List<Tag> tags_;
    #endregion

    #region PrivateProperies
    private List<Tag> tags { get { return tags_; } }
    private BinaryReader R { get { return reader_; } }
    private int nEvents { get { return Events.Count / Cols; } }
    #endregion

    #region Classes
    private class Tag
    {
      #region Field
      private readonly BinaryReader r;
      private readonly string key_;
      private readonly char ver_;
      private readonly int size_;
      private readonly List<string> data;
      private bool cont;
      #endregion

      public Tag(BinaryReader reader)
      {
        r = reader;
        key_ = NextTag();
        ver_ = r.ReadChar();
        Skip(1);
        size_ = ReadInt();

        // CS and CV may have binary data. Thus, special care is required.
        if (Key == "CS")
        {
          long orig = r.BaseStream.Position;
          // Read raw data
          data = new List<string>(2)
          {
            ReadString(),
            r.BaseStream.Position.ToString()
          };
          r.BaseStream.Position = orig;
          r.BaseStream.Seek(Size, System.IO.SeekOrigin.Current);
          char x = r.ReadChar();
          if (x != ';')
          {
            throw new Exception("Tag Data delimiter ';' was not found at he end of CS. The found char was '" + x + "'");
          }
        }
        else if (Key == "CV")
        {
          data = new List<string>(3);
          long orig = r.BaseStream.Position;
          data.Add(ReadString());
          data.Add(ReadString());
          Raw = r.ReadBytes(Size - (int)(r.BaseStream.Position - orig));
          char x = r.ReadChar();
          if (x != ';')
          {
            throw new Exception("Tag Data delimiter ';' was not found at he end of CV. The found char was '" + x + "'");
          }
        }
        else
        {
          data = new List<string>(20);
          //var buf = r.ReadBytes(size);
          while (cont)
          {
            data.Add(ReadString());
          }
        }

      }

      public byte[] Raw { get; set; }
      public string Key { get { return key_; } }
      public int Size { get { return size_; } }
      public char Ver { get { return ver_; } }
      public string this[int idx] { get { return data[idx]; } }

      public override string ToString()
      {
        StringBuilder sb = new StringBuilder(Size + 20);
        sb.Append('|');
        sb.Append(Key);
        sb.Append(',');
        sb.Append(Ver);
        sb.Append(',');
        sb.Append(Size);
        foreach (var item in data)
        {
          sb.Append(',');
          sb.Append(item);
        }
        sb.Append(';');
        return sb.ToString();
      }

      #region PrivateMethods
      private void Skip(int count = 1) { r.ReadBytes(count); }
      private int ReadInt() { return int.Parse(ReadString()); }

      private string ReadString()
      {
        StringBuilder sb = new StringBuilder();
        while (r.BaseStream.Position < r.BaseStream.Length)
        {
          char ch = r.ReadChar();
          if (ch == ',') break;
          if (ch == ';' || ch == '|')
          {
            cont = false;
            break;
          }
          sb.Append(ch);
        }
        return sb.ToString();
      }

      private string NextTag()
      {
        while (r.BaseStream.Position < r.BaseStream.Length)
        {
          byte ch = r.ReadByte();
          if (ch == 124)  // '|' = 124
            break;
        }
        var a = r.ReadChars(2);
        r.ReadChar();
        cont = true;
        return new string(a);
      }
      #endregion

    } // end of class Tag

    private interface ITag
    {
      void Parse(Tag tag);
    }

    private interface IFix
    {
      void Add(string value);
    }
    #endregion

    #region Methods
    private static double Get_double(string arg) { return double.Parse(arg); }
    private static int Get_int(string arg) { return int.Parse(arg); }

    private void ParseTags()
    {
      foreach (var tag in tags)
      {
        tagParsers[tag.Key].Parse(tag);
      }
    }

    // close reader and streams if they were open.
    private void CheckAndCloseStream()
    {
      if (reader_ != null)
      {
        reader_.Close();
        reader_ = null;
        stream_ = null;  // stream_ will be closed by reader_;
      }
      if (stream_ != null)
      {
        stream_.Close();
        stream_ = null;
      }
    }
    private int EventIndex(int ch, int event_id)
    {
      return ch * nEvents + event_id;
    }

    private double EventDuration(int ch, int event_id)
    {
      return Events[EventIndex(ch, event_id)].Len * TimeIncrement(ch);
    }

    //private int Data_type_id(int ch)
    //{
    //  return Packet_info[ch].Number_format;
    //}

    private double[] ReadColumnWithConvert(int ch, Func<byte[], double> conv)
    {
      BufferRef bref = BufferRefs[ch];
      int total_buffer_length = bref.Size;
      int data_byte = PacketInfos[ch].Bytes;
      int count_of_data = total_buffer_length / data_byte;
      double[] res = new double[count_of_data];

      R.BaseStream.Seek(Origins[bref.Index] + bref.Offset, System.IO.SeekOrigin.Begin);
      if (ValueRanges.Count > ch && ValueRanges[ch].Transform)
      {
        double amp = ValueRanges[ch].Factor;
        double offset = ValueRanges[ch].Offset;
        for (int i = 0; i < count_of_data; i++)
        {
          res[i] = conv(R.ReadBytes(data_byte)) * amp + offset;
        }
      }
      else
      {
        for (int i = 0; i < count_of_data; i++)
        {
          res[i] = conv(R.ReadBytes(data_byte));
        }
      }
      return res;
    }

    private double[] ReadColumnAsDouble(int ch)
    {
      switch (PacketInfos[ch].NumberFormat)
      {
        case 4: // singed short
          return ReadColumnWithConvert(ch,
              ba => BitConverter.ToInt16(ba, 0));
        case 8: // double
          return ReadColumnWithConvert(ch,
              ba => BitConverter.ToDouble(ba, 0));
        case 11: // t-Byte Word digital for Taikou Kenchi
          return ReadColumnWithConvert(ch,
              ba => BitConverter.ToUInt16(ba, 0));
        default:
          throw new NotImplementedException();
      }
    }

    // fix base time
    private class FixTimeOffset : IFix
    {
      private readonly Famos parent_;
      public FixTimeOffset(Famos parent)
      {
        parent_ = parent;
      }
      public void Add(string value)
      {
        parent_.timeOffset_ = double.Parse(value);
      }
    }

    private class FixValueOffsets : IFix
    {
      private readonly Famos parent_;

      public FixValueOffsets(Famos parent)
      {
        parent_ = parent;
      }

      public void Add(string values)
      {
        string[] buf = values.Split(Famos.blankSeprator);
        parent_.valueOffsets_ = new double[buf.Length];
        for (int i = 0; i < buf.Length; i++)
        {
          parent_.valueOffsets_[i] = double.Parse(buf[i]);
        }
      }
    }

    // Initialize members
    private void Init()
    {
      Events = new List<Event>();
      ChannelInfos = new List<ChannelInfo>();
      ValueRanges = new List<ValueRange>();
      DataTypes = new List<DataType>();
      PacketInfos = new List<PacketInfo>();
      BufferRefs = new List<BufferRef>();
      Origins = new List<long>();
      Values = new List<Value>();
      columnCache = new Dictionary<int, double[]>();
      tags_ = new List<Tag>();
      Opened = false;
      availableFixItems = new Dictionary<string, IFix>()
            {
                {"time_offset", new FixTimeOffset(this)},
                {"value_offsets", new FixValueOffsets(this)}
            };
      DummyParser dummy = new DummyParser();
      tagParsers = new Dictionary<string, ITag>(){
                {"CF", new FileTagParser()},
                {"CV", new EventParser(this)},
                {"CK", dummy},
                {"Cv", dummy},
                {"CN", new ChannelInfoParser(this)},
                {"NO", dummy},
                {"CB", dummy},
                {"CT", dummy},
                {"CG", dummy},
                {"CD", new DataTypeParser(this)},
                {"Cb", new BufferRefParse(this)},
                {"CC", dummy},
                {"CZ", dummy},
                {"CP", new PacketInfoParser(this)},
                {"CR", new ValueRangeParser(this)},
                {"ND", dummy},
                {"CS", new OriginParser(this)},
                {"NU", dummy},
                {"CI", new ValueParser(this)},
                {"NT", new TimeParser(this)},
            };
    }

    private int Length(int ch)
    {
      return BufferRefs[ch].Size / PacketInfos[ch].Bytes;
    }
    #endregion // Methods

    #region Parser

    private class DataTypeParser : ITag
    {
      private readonly Famos parent_;
      public DataTypeParser(Famos parent)
      { parent_ = parent; }

      public void Parse(Tag tag)
      {
        double dx = Get_double(tag[0]);
        bool cal = tag[1][0] == '1';
        string unit = (tag[3]);
        if (tag.Ver == '1')
        {
          parent_.DataTypes.Add(new DataType(dx, cal, unit));
        }
        else
        {
          double x0 = Get_double(tag[4]);
          bool pre = tag[5][0] == '1';
          parent_.DataTypes.Add(new DataType(dx, cal, unit, x0, pre));
        }
      }
    }

    private class FileTagParser : ITag
    {
      public void Parse(Tag tag)
      {
        if (tag.ToString() != TAG_CF)
          throw new ArgumentException("No magic string was found.");
      }
    }

    // DumyParser
    private class DummyParser : ITag
    {
      public void Parse(Tag tag)
      { }
    }

    private class TimeParser : ITag
    {
      private readonly Famos parent_;
      public TimeParser(Famos parent)
      { parent_ = parent; }

      public void Parse(Tag tag)
      {
        int year = Get_int(tag[2]);
        int month = Get_int(tag[1]);
        int day = Get_int(tag[0]);
        int hour = Get_int(tag[3]);
        int min = Get_int(tag[4]);
        double sec = Get_double(tag[5]);
        DateTime time = new DateTime(year, month, day, hour, min, 0);
        parent_.time_ = time.AddSeconds(sec);
      }
    }

    private class EventParser : ITag
    {
      readonly Famos parent_;

      public EventParser(Famos parent)
      {
        parent_ = parent;
      }

      public void Parse(Tag tag)
      {
        int n = int.Parse(tag[1]);
        //DateTime origin = new DateTime(1980, 1, 1);
        var origin = parent_.time_;
        byte[] data = tag.Raw;
        int sz = data.Length / n;
        for (int i = 0; i < data.Length; i += sz)
        {
          _ = BitConverter.ToInt32(data, i);
          int len = BitConverter.ToInt32(data, i + 4);
          double start = BitConverter.ToDouble(data, i + 8);
          if (start == 0.0)
            parent_.Events.Add(new Event(len, parent_.time_));
          else
            parent_.Events.Add(new Event(len, origin.AddSeconds(start)));
        }
      }
    }

    private class PacketInfoParser : ITag
    {
      private readonly Famos parent_;
      public PacketInfoParser(Famos parent)
      { parent_ = parent; }

      public void Parse(Tag tag)
      {
        int[] args = new int[8];
        for (int i = 0; i < 8; i++)
        {
          args[i] = int.Parse(tag[i].ToString());
        }
        parent_.PacketInfos.Add(new PacketInfo(args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7]));
      }
    }

    private class BufferRefParse : ITag
    {
      private readonly Famos parent_;

      public BufferRefParse(Famos parent)
      { parent_ = parent; }

      public void Parse(Tag tag)
      {
        int size = Get_int(tag[0]);
        int[] ivals = new int[7];
        for (int i = 0; i < size; i++)
        {
          for (int k = 0; k < 7; k++)
          {
            ivals[k] = int.Parse(tag[2 + i * 9 + k].ToString());
          }
          double add_time = Get_double(tag[2 + i * 9 + 7]);
          string user = tag[2 + i * 9 + 8].ToString();
          BufferRef bref = new BufferRef(ivals[0], ivals[1], ivals[2], ivals[3], ivals[4], ivals[5], ivals[6], add_time, user);
          parent_.BufferRefs.Add(bref);
        }
      }
    }

    private class ValueRangeParser : ITag
    {
      private readonly Famos parent_;
      public ValueRangeParser(Famos parent)
      { parent_ = parent; }

      public void Parse(Tag tag)
      {
        bool trans = tag[0][0] == '1';
        double factor = Get_double(tag[1]);
        double offset = Get_double(tag[2]);
        bool cal = tag[3][0] == '1';
        string unit = (tag[5]);
        parent_.ValueRanges.Add(new ValueRange(trans, factor, offset, cal, unit));
      }
    }

    private class ChannelInfoParser : ITag
    {
      private readonly Famos parent_;
      public ChannelInfoParser(Famos parent)
      { parent_ = parent; }

      public void Parse(Tag tag)
      {
        int index = Get_int(tag[0]);
        int bit = Get_int(tag[2]);
        string name = (tag[4]);
        string comment = (tag[6]);
        parent_.ChannelInfos.Add(new ChannelInfo(index, bit, name, comment));
      }
    }

    private class OriginParser : ITag
    {
      readonly Famos parent_;
      public OriginParser(Famos parent)
      { parent_ = parent; }

      public void Parse(Tag tag)
      {
        int ch = Get_int(tag[0]);
        long pos = long.Parse(tag[1]);
        while (parent_.Origins.Count <= ch)
          parent_.Origins.Add(0);
        parent_.Origins[ch] = pos;
      }
    }

    private class ValueParser : ITag
    {
      private readonly Famos parent_;
      public ValueParser(Famos parent)
      { parent_ = parent; }

      public void Parse(Tag tag)
      {
        int index = Get_int(tag[0]);
        int num_format = Get_int(tag[1]);
        string name = (tag[3]);
        int value = Get_int(tag[4]);
        string unit = (tag[6]);
        string comment = (tag[8]);
        double time = Get_double(tag[9]);
        parent_.Values.Add(new Value(index, num_format, name, value, unit, comment, time));
      }
    }
    #endregion

    #endregion // Private
  }
}
