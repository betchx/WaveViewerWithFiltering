using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WaveFilterTool
{
    class Data
    {
        public int Tap { get; set; }
        public int UpperFc { get; set; }
        public int LowerFc { get; set; }
        public double[][] Waves { get; set; }
        public double[][] Spectums { get; set; }

        public Data()
        {
        }
    }
}
