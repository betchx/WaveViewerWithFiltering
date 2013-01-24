using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace WaveViewerWithFilering
{
    class Famos
    {
        public readonly static char[] comma_seprator = {','}; 
        public readonly static char[] blank_seprator = {' ', '\t', '\n'};


        public class Event
        {
            private int len_; public int len { get { return len_; } }
            private DateTime start_time_; public DateTime start_time { get { return start_time_; } }
            public Event(int length, DateTime startTime)
            {
                len_ = length;
                start_time_ = startTime;
            }
        }

        private List<Event> events_;
        public List<Event> events { get { return events_; } }



        private class Tag
        {
            private BinaryReader r;
            private string key_;
            private char ver_;
            private int size_;
            private List<string> data;
            public byte[] raw { get; set; }
            
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
                    while(cont)
                    {
                        data.Add(read_string());
                    }
                }

            }

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


            private void skip(int count = 1)
            {
                r.ReadBytes(count);
            }

            private int read_int()
            {
                return int.Parse(read_string());
            }
            bool cont;
            private string read_string()
            {
                StringBuilder sb = new StringBuilder();
                while (r.BaseStream.Position < r.BaseStream.Length )
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
                while (r.BaseStream.Position < r.BaseStream.Length )
                {
                    ch = r.ReadByte();
                    if (ch == 124)  // '|' = 124
                        break;
                }
                //return r.ReadChars(3).Take(2).ToString();
                var a = r.ReadChars(2);
                r.ReadChar();
                cont = true;
                return new string(a);
            }
            

        } // end of class Tag

        private interface ITag
        {
            void parse(Tag tag);
        }

        // |CF
        private class FileTagParser : ITag 
        {
            public void parse(Tag tag)
            {
                if (tag.ToString() != TAG_CF)
                {
                    throw new ArgumentException("No magic string was found.");
                }
            }
        }

        // DumyParser
        private class DummyParser : ITag
        {
            public void parse(Tag tag)
            {
            }
        }


        // |CV
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
                for (int i = 0; i < data.Length ; i += sz)
                {
                    int xx = BitConverter.ToInt32(data, i);
                    int len = BitConverter.ToInt32(data, i + 4);
                    double start = BitConverter.ToDouble(data, i + 8);
                    DateTime start_time = origin.AddSeconds(start);
                    parent_.events.Add(new Event(len, start_time));
                }
            }
        }

        private const string TAG_CF = "|CF,2,1,1;";

        private int n_events { get { return events.Count / cols; } }
        
        private int event_index(int ch, int event_id)
        {
            return ch * n_events + event_id;
        }


        public DateTime event_time(int ch, int event_id)
        {
            return events[event_index(ch, event_id)].start_time;
        }

        private double event_duration(int ch, int event_id)
        {
            return events[event_index(ch, event_id)].len * dt(ch);
        }

        public double duration(int ch = 0, int event_id = 0)
        {
            if (events_.Count > 0)
                return event_duration(ch, event_id);
            return reader(ch).length * dt(ch);
        }

        public class DataType
        {
            double dx_; 
            public double dx { get { return dx_; } }
            bool calibrated_; 
            public bool calibrated { get { return calibrated_; } }
            string unit_; 
            public string unit { get { return unit_; } }
            double x0_; 
            public double x0 { get { return x0_; } }
            bool pre_trigger_; 
            public bool pre_trigger { get { return pre_trigger_; } }

            public DataType(double dx, bool cal, string unit, double x0 = 0.0, bool pre = false)
            {
                dx_ = dx;
                calibrated_ = cal;
                unit_ = unit;
                x0_ = x0;
                pre_trigger_ = pre;
            }
        }

        private List<DataType> data_types_; public List<DataType> data_types { get { return data_types_; } }


        static double get_double(string arg)
        {
            return double.Parse(arg);
        }
        static int get_int(string arg)
        {
            return int.Parse(arg);
        }


        private class DataTypeParser : ITag
        {
            private Famos parent_;
            public DataTypeParser(Famos parent) { parent_ = parent; }
            public void parse(Tag tag)
            {
                double dx = get_double(tag[0]);
                bool cal = tag[1][0] == '1';
                string unit = (tag[3]);
                if (tag.ver == '1')
                {
                    parent_.data_types.Add( new DataType(dx,cal,unit));
                }
                else
                {
                    double x0 = get_double(tag[4]);
                    bool pre = tag[5][0] == '1';
                    parent_.data_types.Add(new DataType(dx, cal, unit, x0, pre));
                }
            }

        }

        private class TimeParser : ITag
        {
            private Famos parent_;
            public TimeParser(Famos parent) { parent_ = parent; }
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

        public class PacketInfo
        {
            private int buffer_ref_id_; public int buffer_ref_id { get { return buffer_ref_id_; } }
            private int bytes_; public int bytes { get { return bytes_; } }
            private int number_format_; public int number_format { get { return number_format_; } }
            private int significant_bits_; public int significant_bits { get { return significant_bits_; } }
            private int mask_; public int mask { get { return mask_; } }
            private int offset_; public int offset { get { return offset_; } }
            private int direct_sequence_number_; public int direct_sequence_number { get { return direct_sequence_number_; } }
            private int interval_bytes_; public int interval_bytes { get { return interval_bytes_; } }
            public PacketInfo(int buffer_ref_id, int bytes, int number_format, int significant_bits, int mask,
                int offset, int direct_sequence_number, int interval_bytes)
            {
                buffer_ref_id_ = buffer_ref_id;
                bytes_ = bytes;
                number_format_ = number_format;
                significant_bits_ = significant_bits;
                mask_ = mask;
                offset_ = offset;
                direct_sequence_number_ = direct_sequence_number;
                interval_bytes_ = interval_bytes;
            }
        }
        private List<PacketInfo> packet_info_; public List<PacketInfo> packet_info { get { return packet_info_; } }

        private class PacketInfoParser : ITag
        {
            private Famos parent_;
            public PacketInfoParser(Famos parent) { parent_ = parent; }
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



        public class BufferRef
        {
            private int num_; public int num { get { return num_; } }
            private int index_; public int index { get { return index_; } }
            private int offset_; public int offset { get { return offset_; } }
            private int size_; public int size { get { return size_; } }
            private int offset_sample_; public int offset_sample { get { return offset_sample_; } }
            private int buffer_filled_bytes_; public int buffer_filled_bytes { get { return buffer_filled_bytes_; } }
            private int x0_; public int x0 { get { return x0_; } }
            private double add_time_; public double add_time { get { return add_time_; } }
            private string user_info_; public string user_info { get { return user_info_; } }
            public BufferRef(int num, int index, int offset, int size, int offset_sample,
                int buffer_filled_bytes, int x0, double add_time, string user_info)
            {
                num_ = num;
                index_ = index;
                offset_ = offset;
                size_ = size;
                offset_sample_ = offset_sample;
                buffer_filled_bytes_ = buffer_filled_bytes;
                x0_ = x0;
                add_time_ = add_time;
                user_info_ = user_info;
            }
        }

        private List<BufferRef> buffer_refs_; public List<BufferRef> buffer_refs { get { return buffer_refs_; } }


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

        public class ValueRange
        {
            private bool transform_; public bool transform { get { return transform_; } }
            private double factor_; public double factor { get { return factor_; } }
            private double offset_; public double offset { get { return offset_; } }
            private bool calibrated_; public bool calibrated { get { return calibrated_; } }
            private string unit_; public string unit { get { return unit_; } }
            public ValueRange(bool trans, double factor, double offset, bool calibrated, string unit)
            {
                transform_ = trans;
                factor_ = factor;
                offset_ = offset;
                calibrated_ = calibrated;
                unit_ = unit;
            }
        }
        private List<ValueRange> value_ranges_; public List<ValueRange> value_ranges { get { return value_ranges_; } } 

        private class ValueRangeParser : ITag
        {
            private Famos parent_;
            public ValueRangeParser(Famos parent) { parent_ = parent; }
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

        public class ChannelInfo
        {
            private int index_; public int index { get { return index_; } }
            private int index_bit_; public int index_bit { get { return index_bit_; } }
            private string name_; public string name { get { return name_; } }
            private string comment_; public string comment { get { return comment_; } }
            public ChannelInfo(int index, int index_bit, string name, string comment)
            {
                index_ = index;
                index_bit_ = index_bit;
                name_ = name;
                comment_ = comment;
            }
        }
        private List<ChannelInfo> channel_info_; public List<ChannelInfo> channel_info { get { return channel_info_; } }

        private class ChannelInfoParser : ITag
        {
            private Famos parent_;
            public ChannelInfoParser(Famos parent)
            {
                parent_ = parent;
            }
            public void parse(Tag tag)
            {
                int index = get_int(tag[0]);
                int bit = get_int(tag[2]);
                string name = (tag[4]);
                string comment = (tag[6]);
                parent_.channel_info.Add(new ChannelInfo(index, bit, name, comment));
            }
        }

        private List<long> origins_; public List<long> origins { get { return origins_; } }

        private class OriginParser : ITag
        {
            Famos parent_;
            public OriginParser(Famos parent) { parent_ = parent; }
            public void parse(Tag tag)
            {
                int ch = get_int(tag[0]);
                long pos = long.Parse(tag[1]);
                while (parent_.origins.Count <= ch)
                    parent_.origins.Add(0);
                parent_.origins[ch] = pos;
            }
        }

        public class Value
        {
            private int index_; public int index { get { return index_; } }
            private int num_format_; public int num_format { get { return num_format_; } }
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
            private string name_; public string name { get { return name_; } }
            private int value_; public int value { get { return value_; } }
            private string unit_; public string unit { get { return unit_; } }
            private string comment_; public string comment { get { return comment_; } }
            private double time_; public double time { get { return time_; } }
            public Value(int index, int num_format, string name, int value, string unit, string comment, double time)
            {
                index_ = index;
                num_format_ = num_format;
                name_ = name;
                value_ = value;
                unit_ = unit;
                comment_ = comment;
                time_ = time;
            }
        }

        private List<Value> values_; public List<Value> values { get { return values_; } }


        private class ValueParser : ITag
        {
            private Famos parent_;
            public ValueParser(Famos parent) { parent_ = parent; }
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


        private int data_type_id(int ch)
        {
            return packet_info[ch].number_format;
        }

        //byte[][] read_column_as_byte(int ch)
        //{
        //    BufferRef bref = buffer_refs[ch];
        //    r.BaseStream.Seek(origins[bref.index] + bref.offset, System.IO.SeekOrigin.Begin);
        //    int total_buffer_length = bref.size;
        //    int data_byte = packet_info[ch].bytes;
        //    int count_of_data = total_buffer_length / data_byte;
        //    byte[] buf = r.ReadBytes(total_buffer_length);
        //    byte[][] res = new byte[count_of_data][];
        //    for (int i = 0; i < count_of_data; i++)
        //    {
        //        res[i] = buf.s
        //    }

        //    return res;
        //}

        double[] read_column_with_convert(int ch, Func<byte[], double> conv)
        {
            BufferRef bref = buffer_refs[ch];
            int total_buffer_length = bref.size;
            int data_byte = packet_info[ch].bytes;
            int count_of_data = total_buffer_length / data_byte;
            double[] res = new double[count_of_data];

            r.BaseStream.Seek(origins[bref.index] + bref.offset, System.IO.SeekOrigin.Begin);
            if (value_ranges.Count > ch &&  value_ranges[ch].transform)
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


        double[] read_column_as_double(int ch)
        {
            switch (packet_info[ch].number_format)
            {
                case 4: // singed short
                    return read_column_with_convert(ch, 
                        ba => BitConverter.ToInt16(ba,0));
                case 8: // double
                    return read_column_with_convert(ch,
                        ba => BitConverter.ToDouble(ba, 0));
                default:
                    throw new NotImplementedException();
            }
        }

        private Dictionary<int, double[]> column_cache;

        public double[] this[int ch]
        {
            get
            {
                if ( ! column_cache.ContainsKey(ch))
                {
                    column_cache.Add(ch, read_column_as_double(ch));
                }
                return column_cache[ch];
            }
        }

        public int cols { get { return channel_info.Count; } }

        public double dt(int ch = 0)
        {
            return data_types[ch].dx;
        }

        public int len(int ch)
        {
            return buffer_refs[ch].size / packet_info[ch].bytes;
        }

        public class FamosReader
        {
            private Famos parent_;
            private int length_; public int length { get { return length_; } }
            public FamosReader(Famos parent, int ch)
            {
                parent_ = parent;
                length_ = parent_.len(ch);
            }
        }


        interface IFix
        {
            void add(string value);
        }

        protected double time_offset_;
        // fix base time
        private  class  fix_time_offset : IFix {
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

        protected  double[] value_offsets_;

        private class fix_value_offsets : IFix {
            private Famos parent_;

            public  fix_value_offsets(Famos parent)
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
        //const string[] AVAILABLE_FIX_ITEMS = { "time_offset", "value_offsets" };

        private Dictionary<string, IFix> AVAILABLE_FIX_ITEMS;

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
            FamosReader reader = new FamosReader(this, ch);

            return reader;
        }

        Dictionary<string, ITag> tag_parsers;

        protected  DateTime  time_;

        public DateTime time(int ch = -1, int event_id = 0)
        {
            if (events_.Count > 0)
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

        public bool opened { get; private set; }

        // Initialize members
        private void init()
        {
            events_ = new List<Event>();
            channel_info_ = new List<ChannelInfo>();
            value_ranges_ = new List<ValueRange>();
            data_types_ = new List<DataType>();
            packet_info_ = new List<PacketInfo>();
            buffer_refs_ = new List<BufferRef>();
            origins_ = new List<long>();
            values_ = new List<Value>();
            column_cache = new Dictionary<int, double[]>();
            tags_ = new List<Tag>();

            opened = false;

            AVAILABLE_FIX_ITEMS = new Dictionary<string, IFix>()
            {
                {"time_offset", new fix_time_offset(this)},
                {"value_offsets", new fix_value_offsets(this)}
            };
            DummyParser dummy = new DummyParser();
            tag_parsers = new Dictionary<string,ITag>(){
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

        // default constructor
        public Famos()
        {
            init();
        }

        // open given file automatically
        public Famos(string filename)
        {
            init();
            open_dat(filename);
        }

        private string file_;
        private Stream s_;
        private BinaryReader r_;

        private BinaryReader r
        {
            get { return r_; }
        }

        private List<Tag> tags_;
        private List<Tag> tags
        {
            get { return tags_; }
        }

        // close reader and streams if they were open. 
        private void check_and_close_stream()
        {
            if (r_ != null)
            {
                r_.Close();
                r_ = null;
                s_ = null;  // s_ was closed by r_;
            }
            if (s_ != null)
            {
                s_.Close();
                s_ = null;
            }
        }

        public bool open_dat(string filename)
        {
            check_and_close_stream();

            file_ = filename;

            // main read part
            //try
            //{
                s_ = new System.IO.FileStream(filename, System.IO.FileMode.Open);
                r_ = new BinaryReader(s_,Encoding.Default);
                var t = new Tag(r);
                if (t.key != "CF")
                    throw new ArgumentException(filename + " is not Famos file");

                tags.Add(t);
                while (s_.Position < s_.Length)
                {
                    tags.Add(new Tag(r));
                }
            //}
            //catch (Exception e)
            //{
            //    System.Windows.Forms.MessageBox.Show(e.Message);
            //    check_and_close_stream();
            //    return false;
            //}

            // fix check
            var fix_file = filename + ".FIX";

            if (System.IO.File.Exists(fix_file))
            {
                // need fix
                var sr = new StreamReader(fix_file);
                while (!sr.EndOfStream)
                {
                    var line = sr.ReadLine();
                    var arr = line.Split(comma_seprator,2);
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


        private void parse_tags()
        {
            foreach (var tag in tags)
            {
                tag_parsers[tag.key].parse(tag);
            }
        }



    }
}
