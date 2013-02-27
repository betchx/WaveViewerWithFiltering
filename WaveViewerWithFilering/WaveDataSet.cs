using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using fftwlib;
using ComplexArrayLib;
using WaveFile;

namespace WaveViewerWithFilering
{
    public class WaveDataSet : System.ComponentModel.INotifyPropertyChanged
    {
        #region Construction
        public WaveDataSet(double[] wave, double delta_t, bool acc_data = false)
        {
            data = wave;
            dt = delta_t;
            is_acc = acc_data;
            init();
        }
        public WaveDataSet(IWaveFile wavefile, int ch)
        {
            data = wavefile[ch];
            dt = wavefile.dt(ch);
            is_acc = wavefile.name(ch).Contains("_Ya_") ||
                wavefile.name(ch).Contains("_Za_");
            init();
        }
        private void init()
        {
            initialized = false;
            data_start_ = 0;
            filter = new FIRFilter();
            over_sample_ = 1;
            raw_wave_start = -1;
            if (is_acc)
                integral = 0;
            else
                integral = -1;
            nfft = 0;
            update_nfft();
        }
        #endregion

        #region Properties

        // with Setter
        public int lower { get { return filter.lower; } set { filter.lower = value; update_factors(); } }
        public int upper { get { return filter.upper; } set { filter.upper = value; update_factors(); } }
        public double gain { get { return filter.gain; } set { filter.gain = value; update_factors(); } }
        public double alpha { get { return filter.alpha; } set { filter.alpha = value; update_factors(); } }
        public FIRFilter.WindowType window_type
        {
            get { return filter.window_type; }
            set { filter.window_type = value; update_factors(); }
        }

        public int num_disp
        {
            get { return num_disp_; }
            set
            {
                num_disp_ = value;
                update_nfft();
                setup_raw_wave();
            }
        }

        public int data_start
        {
            get { return data_start_; }
            set
            {
                data_start_ = value;
                setup_raw_wave();
                NotifyPropertyChanged("data_start");
            }
        }

        public int tap
        {
            get { return filter.tap; }
            set
            {
                filter.tap = value;
                hann = new HannWindow(value);
                update_nfft();
                NotifyPropertyChanged("tap");
            }
        }

        public int over_sample
        {
            get { return over_sample_; }
            set
            {
                switch (value)
                {
                    case 1:
                    case 2:
                    case 4:
                    case 8:
                        if (over_sample != value)
                        {
                            over_sample_ = value;
                            lock (fftw)
                            {
                                over = new WaveData(nfft * over_sample);
                            }
                            update();
                        }
                        break;
                    default:
                        throw new ArgumentException("over_sample can be 1, 2, 4 or 8");
                }
                NotifyPropertyChanged("over_sample");
            }
        }

        public string category
        {
            get
            {
                if (integral < 0)
                    return CATEGORY.Last();
                return CATEGORY[integral];
            }
            set
            {
                if (is_acc)
                {
                    var val = Array.FindIndex(CATEGORY, s => s == value);
                    if (val != integral)
                    {
                        integral = val;
                        update_wave(true);
                        NotifyPropertyChanged("category");
                    }
                }
            }
        }

        // Read only properties
        public IEnumerable<double> Data { get { return data; } }
        public WindowFunction window { get { return filter.window; } }
        public bool is_acc { get; private set; }
        public int length { get { return data.Length; } }
        public int nfft { get; private set; }
        public double dt { get; private set; }
        public double x0 { get { return data_start_ * dt; } }
        public double[] factor { get { return filter.factor; } }
        public double[] gains { get { return filter.gains; } }
        public double[] data { get; private set; }
        public double[] freqs { get; private set; }
        public double[] source { get; private set; }
        public double[] xvalues { get; private set; }
        public double[] over_sampled { get; private set; }
        public double[] filtered { get; private set; }

        public uint over_id { get { return over.wave_id; } }
        public uint filtered_id { get { return ans.wave_id; } }
        public uint factor_id { get { return factors.wave_id; } }
        public uint gain_id { get { return factors.sp_id; } }
        public uint source_id { get { return wave.wave_id; } }


        public bool is_valid
        {
            get
            {
                if (data == null) return false;
                if (filter == null) return false;
                if (num_disp_ == 0) return false;
                if (nfft == 0) return false;
                if (dt == 0.0) return false;

                return true;
            }
        }

        #endregion

        #region Public Methods

        public double[] wave_spectrum_amplitude_in_dB()
        {
            return wave.dB.ToArray();
        }

        public void update()
        {
            lock (this)
            {
                if (now_updating) return;  // To avoid stack overflow
                now_updating = true;
            }

            setup_raw_wave();
            apply_filter();

            if (current_over_sample != over_sample || over.wave_id < ans.wave_id)
            {
                if (over.Length != ans.Length * over_sample)
                {
                    lock (fftw)
                    {
                        over = new WaveData(ans.Length * over_sample);
                    }
                }

                if (over_sample == 1)
                {
                    // noneed to upsampling;
                    over.Wave = ans.Wave;
                }
                else
                {
                    over.clear_sp(); // need clear
                    over.Spectrum = ans.Spectrum.Select(x=>x*over_sample);
                }
                over_sampled = over.Wave.Take(num_disp * over_sample).ToArray();
                current_over_sample = over_sample; //calc
                NotifyPropertyChanged("over_sampled");
            }
            initialized = true;
            lock (this)
            {
                now_updating = false;
            }
        }

        // for debug
        public IEnumerable<double>[] debug_waves()
        {
            int zeros = Math.Max(2 * tap - data_start, 0);
            return new IEnumerable<double>[]{
                wave.Wave,
                ans.Wave,
                over.Wave,
                raw_wave.Wave,
                Enumerable.Repeat(0.0,zeros).Concat(data.Skip(data_start - 2*tap).Take(num_disp + 4*tap-zeros))
            };
        }

        public enum State{
            Raw,
            Source,
            Filtered,
            Oversampled,
        }

        public double[] Wave(State target)
        {
            switch (target)
            {
                case State.Raw:
                    return raw_wave.Wave.ToArray();
                case State.Source:
                    return source;
                case State.Filtered:
                    return filtered;
                case State.Oversampled:
                    return over_sampled;
                default:
                    throw new ArgumentException();
           }
        }
        public double[] Power(State target)
        {
            switch (target)
            {
                case State.Raw:
                    return raw_wave.Power.ToArray();
                case State.Source:
                    return wave.Power.ToArray();
                case State.Filtered:
                    return ans.Power.ToArray();
                case State.Oversampled:
                    return over.Power.ToArray();
                default:
                    throw new ArgumentException();
            }
        }

        public IEnumerable<double>[] debug_spectrums()
        {
            return new IEnumerable<double>[]{
                wave.Abs,
                ans.Abs,
                over.Abs,
                raw_wave.Abs,
            };
        }
        #endregion

        #region Internal Use
        #region Private Fields
        //--Private members
        private WaveData raw_wave;    // raw wave. this is partial copy of data. 
        private WaveData wave;        // wave data (it can be disp or vel wave)
        private WaveData factors;     // expanded filter data
        private WaveData ans;         // filtered wave (iDFT of sp_ans)
        private WaveData over;
        private double base_line;
        private ComplexArray omega;
        private HannWindow hann;
        private int raw_wave_start;
        private int current_over_sample;
        private int raw_wave_num_disp;
        private double[] extracted_raw_wave;
        private ComplexArray omega2;
        private FIRFilter filter;
        private int integral;

        private int data_start_;
        private int num_disp_;
        private int over_sample_;

        // Used for lock
        private static object fftw = 1;

        private static readonly string[] CATEGORY = new string[] { "ACC", "VEL", "DIS", "NONE" };
        private bool initialized;

        #endregion

        #region Private methods

        /// <summary>
        /// NFFT： Number of data for FFT (= 2^n > 4tap + num_disp, >1024)
        /// </summary>
        private void update_nfft()
        {
            int val = 1024;
            while (val < num_disp + filter.tap * 4)
                val *= 2;

            if (nfft != val)
            {
                // update
                nfft = val;

                lock (fftw)
                {
                    over = new WaveData(nfft * over_sample_);
                    ans = new WaveData(nfft);
                    factors = new WaveData(nfft);
                    wave = new WaveData(nfft);
                    raw_wave = new WaveData(nfft);
                }
                extracted_raw_wave = new double[nfft];

                // update Omega
                double fs = 1.0 / dt;
                double df = fs / nfft;
                lock (fftw)
                {
                    omega = new ComplexArray(nfft / 2 + 1);
                    omega2 = new ComplexArray(nfft / 2 + 1);
                }

                double df0 = -1.0 / (df*2*Math.PI);
                double v;
                for (int i = 1; i < nfft/2; i++)
                {
                    v = df0 / i;
                    omega[i].Imag = v;
                    omega2[i].Real = - v * v;
                }

                // update freqs
                freqs = Enumerable.Range(0, nfft / 2 + 1).Select(i => df * i).ToArray();
            }
        }

        // copy wave data and FFT
        private bool setup_raw_wave(bool force = false)
        {
            if (num_disp == 0)
                return false;

            if (initialized && ! force &&
                raw_wave_start == data_start &&
                raw_wave_num_disp == num_disp)
                return false;

            xvalues = Enumerable.Range(0, num_disp).Select(i => (i + data_start_) * dt).ToArray();

            int n_start = data_start_ - tap * 2;
            var base_wave = data.Skip(n_start).Take(num_disp);
            base_line = base_wave.Average();
            var range = base_wave.Max() - base_wave.Min();
            var delta = range / 1e6;

            for (int i = 0; i < 20; i++)
            {
                take_raw_wave_with_baseline();
                var error = extracted_raw_wave.Average();
                if (Math.Abs(error) < delta)
                    break;
                base_line += error * 0.75; //update
            }
            raw_wave.Wave = extracted_raw_wave;
            NotifyPropertyChanged("raw_wave");
            raw_wave_start = data_start;
            raw_wave_num_disp = num_disp;

            return true;
        }

        private void take_raw_wave_with_baseline()
        {
            int tap = filter.tap;
            int n_start = data_start - tap * 2;
            int n_end = data_start + num_disp;

            //raw_wave.clear_wave();
            Array.Clear(extracted_raw_wave, 0, nfft);

            // copy pre_data with filter
            for (int i = 0; i < tap; i++)
            {
                extracted_raw_wave[nfft - 2 * tap + i] = (data[Math.Abs(i + n_start)] - base_line) * hann[tap - i];
                extracted_raw_wave[nfft - 1 * tap + i] = data[Math.Abs(i + tap + n_start)] - base_line;
            }
            // copy main_data
            int last_index = data.Length - 1;
            for (int i = 0; i < num_disp; i++)
            {
                int k = last_index - Math.Abs(last_index - (data_start + i));
                extracted_raw_wave[i] = data[k] - base_line;
            }
            // copy postdata
            for (int i = 0; i < tap; i++)
            {
                int k = last_index - Math.Abs(last_index - (n_end + i));
                extracted_raw_wave[num_disp + i] = data[k] - base_line;
            }
            // copy with filter
            for (int i = 0; i < tap; i++)
            {
                int j = i + 3 * tap + num_disp;
                int k = last_index - Math.Abs(last_index - (n_end + tap + i));
                extracted_raw_wave[num_disp + tap + i] = (data[k] - base_line) * hann[i];
            }
        }

        private bool update_wave(bool force = false)
        {
            setup_raw_wave();
            if (force || wave.wave_id < raw_wave.wave_id)
            {
                if (integral < 1)
                {
                    // ACC and others are not need integration.
                    wave.Spectrum = raw_wave.Spectrum;
                }
                else
                {
                    // VEL and DISP need integration.
                    ComplexArray w = integral == 1 ? omega : omega2;

                    // Integrate in frequency domain.
                    wave.Spectrum = raw_wave.Spectrum.Zip(w, (a, b) => a * b);
                }
                source = wave.Wave.Take(num_disp).ToArray(); // calc
                NotifyPropertyChanged("source");
                return true;
            }
            return false;
        }

        private bool update_factors(bool force = false)
        {

            if (force || filter.design()|| factors.wave_id == 0) // refresh filter if needed
            {
                factors[0] = filter.factor[0];
                for (int i = 1; i <= filter.tap; i++)
                {
                    factors[nfft - i] = factors[i] = filter.factor[i];
                }
                NotifyPropertyChanged("factors");
                return true;
            }
            return false;
        }

        private void apply_filter(bool force = false)
        {
            if (wave.wave_id < raw_wave.wave_id)
            {
                update_wave();
            }

            update_factors();

            if (force ||
                ans.wave_id < wave.wave_id ||
                ans.wave_id < factors.wave_id)
            {
                // convolution in Frequency domain
                ans.Spectrum = wave.Spectrum.Zip(factors.Spectrum, (a, b) => a * b);
                // copy result to filtered_
                filtered = ans.Wave.Take(num_disp).ToArray(); // calc
                NotifyPropertyChanged("filtered");
            }
        }

        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion // Private Methods
        #endregion // Internal Use

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        private bool now_updating;
    }
}
