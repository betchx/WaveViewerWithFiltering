using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace WaveFile
{
    public class Famos : IWaveFile
    {
        #region Construction
        // default constructor
        public Famos()
        { init(); }

        // open given file automatically
        public Famos(string filename)
        {
            init();
            open_dat(filename);
        }
        #endregion

        #region Properties
        // Auto Implemented Properties
        public List<DataType> data_types { get; private set; }
        public List<Event> events { get; private set; }
        public List<PacketInfo> packet_info { get; private set; }
        public List<BufferRef> buffer_refs { get; private set; }
        public List<ValueRange> value_ranges { get; private set; }
        public List<ChannelInfo> channel_info { get; private set; }
        public List<long> origins { get; private set; }
        public List<Value> values { get; private set; }
        public bool opened { get; private set; }

        public DateTime Time { get { return time(); } }
        public int cols { get { return channel_info.Count; } }
        public int rows { get { return this.length(0); } }

        //Indexer
        public double[] this[int ch]
        {
            get
            {
                if (!column_cache.ContainsKey(ch))
                    column_cache.Add(ch, read_column_as_double(ch));
                return column_cache[ch];
            }
        }
        #endregion

        #region SubClasses(Public)

        public class Event
        {
            public Event(int length, DateTime startTime)
            {
                this.len = length;
                this.start_time = startTime;
            }
            public int len { get; private set; }
            public DateTime start_time { get; private set; }
        }

        public class DataType
        {
            public DataType(double dx, bool cal, string unit, double x0 = 0.0, bool pre = false)
            {
                this.dx = dx;
                this.calibrated = cal;
                this.unit = unit;
                this.x0 = x0;
                this.pre_trigger = pre;
            }
            public double dx { get; private set; }
            public bool calibrated { get; private set; }
            public string unit { get; private set; }
            public double x0 { get; private set; }
            public bool pre_trigger { get; private set; }
        }

        public class PacketInfo
        {
            public PacketInfo(int buffer_ref_id, int bytes, int number_format, int significant_bits, int mask,
                int offset, int direct_sequence_number, int interval_bytes)
            {
                this.buffer_ref_id = buffer_ref_id;
                this.bytes = bytes;
                this.number_format = number_format;
                this.significant_bits = significant_bits;
                this.mask = mask;
                this.offset = offset;
                this.direct_sequence_number = direct_sequence_number;
                this.interval_bytes = interval_bytes;
            }
            public int buffer_ref_id { get; private set; }
            public int bytes { get; private set; }
            public int number_format { get; private set; }
            public int significant_bits { get; private set; }
            public int mask { get; private set; }
            public int offset { get; private set; }
            public int direct_sequence_number { get; private set; }
            public int interval_bytes { get; private set; }
        }

        public class BufferRef
        {
            public BufferRef(int num, int index, int offset, int size, int offset_sample,
                int buffer_filled_bytes, int x0, double add_time, string user_info)
            {
                this.num = num;
                this.index = index;
                this.offset = offset;
                this.size = size;
                this.offset_sample = offset_sample;
                this.buffer_filled_bytes = buffer_filled_bytes;
                this.x0 = x0;
                this.add_time = add_time;
                this.user_info = user_info;
            }
            public int num { get; private set; }
            public int index { get; private set; }
            public int offset { get; private set; }
            public int size { get; private set; }
            public int offset_sample { get; private set; }
            public int buffer_filled_bytes { get; private set; }
            public int x0 { get; private set; }
            public double add_time { get; private set; }
            public string user_info { get; private set; }
        }

        public class ValueRange
        {
            public ValueRange(bool trans, double factor, double offset, bool calibrated, string unit)
            {
                this.transform = trans;
                this.factor = factor;
                this.offset = offset;
                this.calibrated = calibrated;
                this.unit = unit;
            }
            public bool transform { get; private set; }
            public double factor { get; private set; }
            public double offset { get; private set; }
            public bool calibrated { get; private set; }
            public string unit { get; private set; }
        }

        public class ChannelInfo
        {
            public ChannelInfo(int index, int index_bit, string name, string comment)
            {
                this.index = index;
                this.index_bit = index_bit;
                this.name = name;
                this.comment = comment;
            }
            public int index { get; private set; }
            public int index_bit { get; private set; }
            public string name { get; private set; }
            public string comment { get; private set; }
        }

        public class Value
        {
            public Value(int index, int num_format, string name, int value, string unit, string comment, double time)
            {
                this.index = index;
                this.num_format = num_format;
                this.name = name;
                this.value = value;
                this.unit = unit;
                this.comment = comment;
                this.time = time;
            }
            public int index { get; private set; }
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
            public int num_format { get; private set; }
            public string name { get; private set; }
            public int value { get; private set; }
            public string unit { get; private set; }
            public string comment { get; private set; }
            public double time { get; private set; }
        }

        public class FamosReader
        {
            private Famos parent_;
            public int length { get; private set; }
            public FamosReader(Famos parent, int ch)
            {
                this.parent_ = parent;
                this.length = parent_.length(ch);
            }
        }
        #endregion

        #region PublicMethods

        public double duration(int ch = 0, int event_id = 0)
        {
            if (events.Count > 0)
                return event_duration(ch, event_id);
            return reader(ch).length * dt(ch);
        }

        public DateTime event_time(int ch, int event_id)
        {
            return events[event_index(ch, event_id)].start_time;
        }

        public double dt(int ch = 0)
        {
            return data_types[ch].dx;
        }
        public static bool is_famos(string path)
        {
            var tr = new System.IO.StreamReader(path);
            char[] buf = new char[TAG_CF.Length];
            bool ans = false;
            if (tr.Read(buf, 0, TAG_CF.Length) == TAG_CF.Length)
            {
                if (buf.ToString() == TAG_CF)
                    ans = true;
            }
            tr.Close();
            return ans;
        }

        public FamosReader reader(int ch)
        {
            return new FamosReader(this, ch);
        }

        public DateTime time(int ch = -1, int event_id = 0)
        {
            if (events.Count > 0)
            {
                if (ch < 0) ch = 0;
                return event_time(ch, event_id).AddSeconds(time_offset_);
            }
            else
            {
                double time_shift = time_offset_;
                if (ch >= 0)
                    time_shift += buffer_refs[ch].add_time;
                return time_.AddSeconds(time_shift);
            }
        }

        public bool open_dat(string filename)
        {
            check_and_close_stream();

            stream_ = new System.IO.FileStream(filename, System.IO.FileMode.Open);
            reader_ = new BinaryReader(stream_, Encoding.Default);
            var t = new Tag(r);
            if (t.key != "CF")
                throw new ArgumentException(filename + " is not Famos file");

            tags.Add(t);
            while (stream_.Position < stream_.Length)
            {
                tags.Add(new Tag(r));
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
                    var arr = line.Split(comma_seprator, 2);
                    if (AVAILABLE_FIX_ITEMS.ContainsKey(arr[0]))
                    {
                        AVAILABLE_FIX_ITEMS[arr[0]].add(arr[1]);
                    }
                }
            }

            opened = true;
            parse_tags();
            return true;
        }

        public string name(int ch)
        {
            return channel_info[ch].name;
        }

        public string comment(int ch)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region InternalUse

        #region fields
        private readonly static char[] comma_seprator = { ',' };
        private readonly static char[] blank_seprator = { ' ', '\t', '\n' };
        private const string TAG_CF = "|CF,2,1,1;";
        private Dictionary<int, double[]> column_cache;
        private Dictionary<string, IFix> AVAILABLE_FIX_ITEMS;
        private Dictionary<string, ITag> tag_parsers;
        private DateTime time_;
        private double time_offset_;
        private double[] value_offsets_;
        private Stream stream_;
        private BinaryReader reader_;
        private List<Tag> tags_;
        #endregion

        #region PrivateProperies
        private List<Tag> tags { get { return tags_; } }
        private BinaryReader r { get { return reader_; } }
        private int n_events { get { return events.Count / cols; } }
        #endregion

        #region Classes
        private class Tag
        {
            #region Field
            private BinaryReader r;
            private string key_;
            private char ver_;
            private int size_;
            private List<string> data;
            private bool cont;
            #endregion

            public Tag(BinaryReader reader)
            {
                r = reader;
                key_ = next_tag();
                ver_ = r.ReadChar();
                skip(1);
                size_ = read_int();

                // CS and CV may have binary data. Thus, special care is required.
                if (key == "CS")
                {
                    long orig = r.BaseStream.Position;
                    // Read raw data
                    data = new List<string>(2);
                    data.Add(read_string());
                    data.Add(r.BaseStream.Position.ToString());
                    r.BaseStream.Position = orig;
                    r.BaseStream.Seek(size, System.IO.SeekOrigin.Current);
                    char x = r.ReadChar();
                    if (x != ';')
                    {
                        throw new Exception("Tag Data delimiter ';' was not found at he end of CS. The found char was '" + x + "'");
                    }
                }
                else if (key == "CV")
                {
                    data = new List<string>(3);
                    long orig = r.BaseStream.Position;
                    data.Add(read_string());
                    data.Add(read_string());
                    raw = r.ReadBytes(size - (int)(r.BaseStream.Position - orig));
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
                        data.Add(read_string());
                    }
                }

            }

            public byte[] raw { get; set; }
            public string key { get { return key_; } }
            public int size { get { return size_; } }
            public char ver { get { return ver_; } }
            public string this[int idx] { get { return data[idx]; } }

            public override string ToString()
            {
                StringBuilder sb = new StringBuilder(size + 20);
                sb.Append('|');
                sb.Append(key);
                sb.Append(',');
                sb.Append(ver);
                sb.Append(',');
                sb.Append(size);
                foreach (var item in data)
                {
                    sb.Append(',');
                    sb.Append(item);
                }
                sb.Append(';');
                return sb.ToString();
            }

            #region PrivateMethods
            private void skip(int count = 1) { r.ReadBytes(count); }
            private int read_int() { return int.Parse(read_string()); }

            private string read_string()
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

            private string next_tag()
            {
                byte ch = 44; // = '.'
                while (r.BaseStream.Position < r.BaseStream.Length)
                {
                    ch = r.ReadByte();
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
            void parse(Tag tag);
        }

        private interface IFix
        {
            void add(string value);
        }
        #endregion

        #region Methods
        private static double get_double(string arg) { return double.Parse(arg); }
        private static int get_int(string arg) { return int.Parse(arg); }

        private void parse_tags()
        {
            foreach (var tag in tags)
            {
                tag_parsers[tag.key].parse(tag);
            }
        }

        // close reader and streams if they were open.
        private void check_and_close_stream()
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
        private int event_index(int ch, int event_id)
        {
            return ch * n_events + event_id;
        }

        private double event_duration(int ch, int event_id)
        {
            return events[event_index(ch, event_id)].len * dt(ch);
        }

        private int data_type_id(int ch)
        {
            return packet_info[ch].number_format;
        }

        private double[] read_column_with_convert(int ch, Func<byte[], double> conv)
        {
            BufferRef bref = buffer_refs[ch];
            int total_buffer_length = bref.size;
            int data_byte = packet_info[ch].bytes;
            int count_of_data = total_buffer_length / data_byte;
            double[] res = new double[count_of_data];

            r.BaseStream.Seek(origins[bref.index] + bref.offset, System.IO.SeekOrigin.Begin);
            if (value_ranges.Count > ch && value_ranges[ch].transform)
            {
                double amp = value_ranges[ch].factor;
                double offset = value_ranges[ch].offset;
                for (int i = 0; i < count_of_data; i++)
                {
                    res[i] = conv(r.ReadBytes(data_byte)) * amp + offset;
                }
            }
            else
            {
                for (int i = 0; i < count_of_data; i++)
                {
                    res[i] = conv(r.ReadBytes(data_byte));
                }
            }
            return res;
        }

        private double[] read_column_as_double(int ch)
        {
            switch (packet_info[ch].number_format)
            {
                case 4: // singed short
                    return read_column_with_convert(ch,
                        ba => BitConverter.ToInt16(ba, 0));
                case 8: // double
                    return read_column_with_convert(ch,
                        ba => BitConverter.ToDouble(ba, 0));
                case 11: // t-Byte Word digital for Taikou Kenchi
                    return read_column_with_convert(ch,
                        ba => BitConverter.ToUInt16(ba, 0));
                default:
                    throw new NotImplementedException();
            }
        }

        // fix base time
        private class fix_time_offset : IFix
        {
            private Famos parent_;
            public fix_time_offset(Famos parent)
            {
                parent_ = parent;
            }
            public void add(string value)
            {
                parent_.time_offset_ = double.Parse(value);
            }
        }

        private class fix_value_offsets : IFix
        {
            private Famos parent_;

            public fix_value_offsets(Famos parent)
            {
                parent_ = parent;
            }

            public void add(string values)
            {
                string[] buf = values.Split(Famos.blank_seprator);
                parent_.value_offsets_ = new double[buf.Length];
                for (int i = 0; i < buf.Length; i++)
                {
                    parent_.value_offsets_[i] = double.Parse(buf[i]);
                }
            }
        }

        // Initialize members
        private void init()
        {
            events = new List<Event>();
            channel_info = new List<ChannelInfo>();
            value_ranges = new List<ValueRange>();
            data_types = new List<DataType>();
            packet_info = new List<PacketInfo>();
            buffer_refs = new List<BufferRef>();
            origins = new List<long>();
            values = new List<Value>();
            column_cache = new Dictionary<int, double[]>();
            tags_ = new List<Tag>();
            opened = false;
            AVAILABLE_FIX_ITEMS = new Dictionary<string, IFix>()
            {
                {"time_offset", new fix_time_offset(this)},
                {"value_offsets", new fix_value_offsets(this)}
            };
            DummyParser dummy = new DummyParser();
            tag_parsers = new Dictionary<string, ITag>(){
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

        private int length(int ch)
        {
            return buffer_refs[ch].size / packet_info[ch].bytes;
        }
        #endregion // Methods

        #region Parser

        private class DataTypeParser : ITag
        {
            private Famos parent_;
            public DataTypeParser(Famos parent)
            { parent_ = parent; }

            public void parse(Tag tag)
            {
                double dx = get_double(tag[0]);
                bool cal = tag[1][0] == '1';
                string unit = (tag[3]);
                if (tag.ver == '1')
                {
                    parent_.data_types.Add(new DataType(dx, cal, unit));
                }
                else
                {
                    double x0 = get_double(tag[4]);
                    bool pre = tag[5][0] == '1';
                    parent_.data_types.Add(new DataType(dx, cal, unit, x0, pre));
                }
            }
        }

        private class FileTagParser : ITag
        {
            public void parse(Tag tag)
            {
                if (tag.ToString() != TAG_CF)
                    throw new ArgumentException("No magic string was found.");
            }
        }

        // DumyParser
        private class DummyParser : ITag
        {
            public void parse(Tag tag)
            { }
        }

        private class TimeParser : ITag
        {
            private Famos parent_;
            public TimeParser(Famos parent)
            { parent_ = parent; }

            public void parse(Tag tag)
            {
                int year = get_int(tag[2]);
                int month = get_int(tag[1]);
                int day = get_int(tag[0]);
                int hour = get_int(tag[3]);
                int min = get_int(tag[4]);
                double sec = get_double(tag[5]);
                DateTime time = new DateTime(year, month, day, hour, min, 0);
                parent_.time_ = time.AddSeconds(sec);
            }
        }

        private class EventParser : ITag
        {
            Famos parent_;

            public EventParser(Famos parent)
            {
                parent_ = parent;
            }

            public void parse(Tag tag)
            {
                int n = int.Parse(tag[1]);
                DateTime origin = new DateTime(1980, 1, 1);
                byte[] data = tag.raw;
                int sz = data.Length / n;
                for (int i = 0; i < data.Length; i += sz)
                {
                    int xx = BitConverter.ToInt32(data, i);
                    int len = BitConverter.ToInt32(data, i + 4);
                    double start = BitConverter.ToDouble(data, i + 8);
                    DateTime start_time = origin.AddSeconds(start);
                    parent_.events.Add(new Event(len, start_time));
                }
            }
        }

        private class PacketInfoParser : ITag
        {
            private Famos parent_;
            public PacketInfoParser(Famos parent)
            { parent_ = parent; }

            public void parse(Tag tag)
            {
                int[] args = new int[8];
                for (int i = 0; i < 8; i++)
                {
                    args[i] = int.Parse(tag[i].ToString());
                }
                parent_.packet_info.Add(new PacketInfo(args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7]));
            }
        }

        private class BufferRefParse : ITag
        {
            private Famos parent_;

            public BufferRefParse(Famos parent)
            { parent_ = parent; }

            public void parse(Tag tag)
            {
                int size = get_int(tag[0]);
                int[] ivals = new int[7];
                for (int i = 0; i < size; i++)
                {
                    for (int k = 0; k < 7; k++)
                    {
                        ivals[k] = int.Parse(tag[2 + i * 9 + k].ToString());
                    }
                    double add_time = get_double(tag[2 + i * 9 + 7]);
                    string user = tag[2 + i * 9 + 8].ToString();
                    BufferRef bref = new BufferRef(ivals[0], ivals[1], ivals[2], ivals[3], ivals[4], ivals[5], ivals[6], add_time, user);
                    parent_.buffer_refs.Add(bref);
                }
            }
        }

        private class ValueRangeParser : ITag
        {
            private Famos parent_;
            public ValueRangeParser(Famos parent)
            { parent_ = parent; }

            public void parse(Tag tag)
            {
                bool trans = tag[0][0] == '1';
                double factor = get_double(tag[1]);
                double offset = get_double(tag[2]);
                bool cal = tag[3][0] == '1';
                string unit = (tag[5]);
                parent_.value_ranges.Add(new ValueRange(trans, factor, offset, cal, unit));
            }
        }

        private class ChannelInfoParser : ITag
        {
            private Famos parent_;
            public ChannelInfoParser(Famos parent)
            { parent_ = parent; }

            public void parse(Tag tag)
            {
                int index = get_int(tag[0]);
                int bit = get_int(tag[2]);
                string name = (tag[4]);
                string comment = (tag[6]);
                parent_.channel_info.Add(new ChannelInfo(index, bit, name, comment));
            }
        }

        private class OriginParser : ITag
        {
            Famos parent_;
            public OriginParser(Famos parent)
            { parent_ = parent; }

            public void parse(Tag tag)
            {
                int ch = get_int(tag[0]);
                long pos = long.Parse(tag[1]);
                while (parent_.origins.Count <= ch)
                    parent_.origins.Add(0);
                parent_.origins[ch] = pos;
            }
        }

        private class ValueParser : ITag
        {
            private Famos parent_;
            public ValueParser(Famos parent)
            { parent_ = parent; }

            public void parse(Tag tag)
            {
                int index = get_int(tag[0]);
                int num_format = get_int(tag[1]);
                string name = (tag[3]);
                int value = get_int(tag[4]);
                string unit = (tag[6]);
                string comment = (tag[8]);
                double time = get_double(tag[9]);
                parent_.values.Add(new Value(index, num_format, name, value, unit, comment, time));
            }
        }
        #endregion

        #endregion // Private
    }
}
