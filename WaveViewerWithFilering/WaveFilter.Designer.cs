namespace WaveViewerWithFilering
{
    partial class wave_filter
    {
        /// <summary>
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージ リソースが破棄される場合 true、破棄されない場合は false です。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows フォーム デザイナーで生成されたコード

        /// <summary>
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series2 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea2 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend2 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series3 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series4 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea3 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend3 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series5 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series6 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea4 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend4 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series7 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series8 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.auto_update = new System.Windows.Forms.CheckBox();
            this.wave_chart = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.data_start = new System.Windows.Forms.HScrollBar();
            this.update = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.lower_fc = new System.Windows.Forms.TextBox();
            this.upper_fc = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.nyquist_frequency = new System.Windows.Forms.TextBox();
            this.sampling_rate = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.file_path = new System.Windows.Forms.TextBox();
            this.select_file = new System.Windows.Forms.Button();
            this.tap_track = new System.Windows.Forms.TrackBar();
            this.label6 = new System.Windows.Forms.Label();
            this.number_of_tap = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.filter_length = new System.Windows.Forms.TextBox();
            this.upper_fc_track = new System.Windows.Forms.TrackBar();
            this.lower_fc_track = new System.Windows.Forms.TrackBar();
            this.label8 = new System.Windows.Forms.Label();
            this.data_length = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.channel = new System.Windows.Forms.TextBox();
            this.channel_track = new System.Windows.Forms.TrackBar();
            this.channel_name = new System.Windows.Forms.TextBox();
            this.channel_comment = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.openFamosDialog = new System.Windows.Forms.OpenFileDialog();
            this.label12 = new System.Windows.Forms.Label();
            this.display_data_length = new System.Windows.Forms.TextBox();
            this.filter_chart = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.freq_chart = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.rectangle_window = new System.Windows.Forms.RadioButton();
            this.blackman_window = new System.Windows.Forms.RadioButton();
            this.hann_window = new System.Windows.Forms.RadioButton();
            this.hamming_widow = new System.Windows.Forms.RadioButton();
            this.kaiser_window = new System.Windows.Forms.RadioButton();
            this.label14 = new System.Windows.Forms.Label();
            this.alpha = new System.Windows.Forms.TextBox();
            this.label15 = new System.Windows.Forms.Label();
            this.gain = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label13 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.yama = new System.Windows.Forms.Button();
            this.umi = new System.Windows.Forms.Button();
            this.ch_Za = new System.Windows.Forms.ComboBox();
            this.ch_Ya = new System.Windows.Forms.ComboBox();
            this.ch_P2 = new System.Windows.Forms.ComboBox();
            this.ch_P1 = new System.Windows.Forms.ComboBox();
            this.label19 = new System.Windows.Forms.Label();
            this.peak_chart = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.upper_val = new System.Windows.Forms.TextBox();
            this.lower_val = new System.Windows.Forms.TextBox();
            this.label20 = new System.Windows.Forms.Label();
            this.label21 = new System.Windows.Forms.Label();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            ((System.ComponentModel.ISupportInitialize)(this.wave_chart)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tap_track)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.upper_fc_track)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lower_fc_track)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.channel_track)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.filter_chart)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.freq_chart)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.peak_chart)).BeginInit();
            this.SuspendLayout();
            // 
            // auto_update
            // 
            this.auto_update.AutoSize = true;
            this.auto_update.Checked = true;
            this.auto_update.CheckState = System.Windows.Forms.CheckState.Checked;
            this.auto_update.Location = new System.Drawing.Point(613, 35);
            this.auto_update.Name = "auto_update";
            this.auto_update.Size = new System.Drawing.Size(84, 16);
            this.auto_update.TabIndex = 0;
            this.auto_update.Text = "AutoUpdate";
            this.auto_update.UseVisualStyleBackColor = true;
            this.auto_update.Visible = false;
            // 
            // wave_chart
            // 
            chartArea1.AxisX.IsMarginVisible = false;
            chartArea1.AxisX.IsStartedFromZero = false;
            chartArea1.Name = "ChartArea1";
            this.wave_chart.ChartAreas.Add(chartArea1);
            legend1.Name = "Legend1";
            this.wave_chart.Legends.Add(legend1);
            this.wave_chart.Location = new System.Drawing.Point(12, 310);
            this.wave_chart.Name = "wave_chart";
            this.wave_chart.Palette = System.Windows.Forms.DataVisualization.Charting.ChartColorPalette.Bright;
            series1.ChartArea = "ChartArea1";
            series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series1.Legend = "Legend1";
            series1.Name = "source";
            series2.ChartArea = "ChartArea1";
            series2.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series2.Legend = "Legend1";
            series2.Name = "result";
            this.wave_chart.Series.Add(series1);
            this.wave_chart.Series.Add(series2);
            this.wave_chart.Size = new System.Drawing.Size(774, 300);
            this.wave_chart.TabIndex = 1;
            this.wave_chart.TabStop = false;
            this.wave_chart.Text = "chart1";
            // 
            // data_start
            // 
            this.data_start.Location = new System.Drawing.Point(12, 283);
            this.data_start.Name = "data_start";
            this.data_start.Size = new System.Drawing.Size(774, 24);
            this.data_start.TabIndex = 2;
            this.data_start.ValueChanged += new System.EventHandler(this.data_start_ValueChanged);
            // 
            // update
            // 
            this.update.Location = new System.Drawing.Point(703, 28);
            this.update.Name = "update";
            this.update.Size = new System.Drawing.Size(83, 28);
            this.update.TabIndex = 3;
            this.update.Text = "Update";
            this.update.UseVisualStyleBackColor = true;
            this.update.Visible = false;
            this.update.Click += new System.EventHandler(this.update_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(300, 159);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(128, 12);
            this.label1.TabIndex = 4;
            this.label1.Text = "Lower Cutoff Frequency";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(300, 120);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(128, 12);
            this.label2.TabIndex = 4;
            this.label2.Text = "Upper Cutoff Frequency";
            // 
            // lower_fc
            // 
            this.lower_fc.Location = new System.Drawing.Point(349, 174);
            this.lower_fc.Name = "lower_fc";
            this.lower_fc.ReadOnly = true;
            this.lower_fc.Size = new System.Drawing.Size(58, 19);
            this.lower_fc.TabIndex = 5;
            this.lower_fc.Text = "0";
            this.lower_fc.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // upper_fc
            // 
            this.upper_fc.Location = new System.Drawing.Point(349, 134);
            this.upper_fc.Name = "upper_fc";
            this.upper_fc.ReadOnly = true;
            this.upper_fc.Size = new System.Drawing.Size(58, 19);
            this.upper_fc.TabIndex = 5;
            this.upper_fc.Text = "0";
            this.upper_fc.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(81, 66);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(79, 12);
            this.label3.TabIndex = 4;
            this.label3.Text = "Sampling Rate";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(166, 66);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(101, 12);
            this.label4.TabIndex = 4;
            this.label4.Text = "Nyquist Frequency";
            // 
            // nyquist_frequency
            // 
            this.nyquist_frequency.Location = new System.Drawing.Point(212, 84);
            this.nyquist_frequency.Name = "nyquist_frequency";
            this.nyquist_frequency.ReadOnly = true;
            this.nyquist_frequency.Size = new System.Drawing.Size(58, 19);
            this.nyquist_frequency.TabIndex = 5;
            this.nyquist_frequency.Text = "0";
            this.nyquist_frequency.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // sampling_rate
            // 
            this.sampling_rate.Location = new System.Drawing.Point(80, 84);
            this.sampling_rate.Name = "sampling_rate";
            this.sampling_rate.ReadOnly = true;
            this.sampling_rate.Size = new System.Drawing.Size(58, 19);
            this.sampling_rate.TabIndex = 5;
            this.sampling_rate.Text = "0";
            this.sampling_rate.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(10, 5);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(52, 12);
            this.label5.TabIndex = 7;
            this.label5.Text = "Data File";
            // 
            // file_path
            // 
            this.file_path.Location = new System.Drawing.Point(140, 2);
            this.file_path.Name = "file_path";
            this.file_path.ReadOnly = true;
            this.file_path.Size = new System.Drawing.Size(646, 19);
            this.file_path.TabIndex = 8;
            // 
            // select_file
            // 
            this.select_file.Location = new System.Drawing.Point(80, 2);
            this.select_file.Name = "select_file";
            this.select_file.Size = new System.Drawing.Size(54, 18);
            this.select_file.TabIndex = 9;
            this.select_file.Text = "Select";
            this.select_file.UseVisualStyleBackColor = true;
            this.select_file.Click += new System.EventHandler(this.select_file_Click);
            // 
            // tap_track
            // 
            this.tap_track.Location = new System.Drawing.Point(434, 75);
            this.tap_track.Maximum = 300;
            this.tap_track.Minimum = 1;
            this.tap_track.Name = "tap_track";
            this.tap_track.Size = new System.Drawing.Size(349, 42);
            this.tap_track.TabIndex = 10;
            this.tap_track.TickFrequency = 10;
            this.tap_track.Value = 150;
            this.tap_track.Scroll += new System.EventHandler(this.tap_track_Scroll);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(294, 76);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(48, 12);
            this.label6.TabIndex = 11;
            this.label6.Text = "# of Tap";
            // 
            // number_of_tap
            // 
            this.number_of_tap.Location = new System.Drawing.Point(370, 73);
            this.number_of_tap.Name = "number_of_tap";
            this.number_of_tap.ReadOnly = true;
            this.number_of_tap.Size = new System.Drawing.Size(58, 19);
            this.number_of_tap.TabIndex = 5;
            this.number_of_tap.Text = "150";
            this.number_of_tap.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(294, 101);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(70, 12);
            this.label7.TabIndex = 11;
            this.label7.Text = "Filter Length";
            // 
            // filter_length
            // 
            this.filter_length.Location = new System.Drawing.Point(370, 98);
            this.filter_length.Name = "filter_length";
            this.filter_length.ReadOnly = true;
            this.filter_length.Size = new System.Drawing.Size(58, 19);
            this.filter_length.TabIndex = 5;
            this.filter_length.Text = "299";
            this.filter_length.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // upper_fc_track
            // 
            this.upper_fc_track.Location = new System.Drawing.Point(434, 120);
            this.upper_fc_track.Maximum = 150;
            this.upper_fc_track.Minimum = 1;
            this.upper_fc_track.Name = "upper_fc_track";
            this.upper_fc_track.Size = new System.Drawing.Size(349, 42);
            this.upper_fc_track.TabIndex = 10;
            this.upper_fc_track.TickFrequency = 10;
            this.upper_fc_track.Value = 150;
            this.upper_fc_track.Scroll += new System.EventHandler(this.upper_fc_track_Scroll);
            this.upper_fc_track.ValueChanged += new System.EventHandler(this.upper_fc_track_Scroll);
            // 
            // lower_fc_track
            // 
            this.lower_fc_track.Location = new System.Drawing.Point(434, 159);
            this.lower_fc_track.Maximum = 149;
            this.lower_fc_track.Name = "lower_fc_track";
            this.lower_fc_track.Size = new System.Drawing.Size(349, 42);
            this.lower_fc_track.TabIndex = 10;
            this.lower_fc_track.TickFrequency = 10;
            this.lower_fc_track.Scroll += new System.EventHandler(this.lower_fc_track_Scroll);
            this.lower_fc_track.ValueChanged += new System.EventHandler(this.lower_fc_track_Scroll);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(10, 66);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(63, 12);
            this.label8.TabIndex = 4;
            this.label8.Text = "DataLength";
            // 
            // data_length
            // 
            this.data_length.Location = new System.Drawing.Point(15, 84);
            this.data_length.Name = "data_length";
            this.data_length.ReadOnly = true;
            this.data_length.Size = new System.Drawing.Size(58, 19);
            this.data_length.TabIndex = 5;
            this.data_length.Text = "0";
            this.data_length.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(16, 29);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(46, 12);
            this.label9.TabIndex = 4;
            this.label9.Text = "Channel";
            // 
            // channel
            // 
            this.channel.Location = new System.Drawing.Point(18, 44);
            this.channel.Name = "channel";
            this.channel.ReadOnly = true;
            this.channel.Size = new System.Drawing.Size(44, 19);
            this.channel.TabIndex = 5;
            this.channel.Text = "0";
            this.channel.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // channel_track
            // 
            this.channel_track.LargeChange = 1;
            this.channel_track.Location = new System.Drawing.Point(68, 25);
            this.channel_track.Maximum = 4;
            this.channel_track.Name = "channel_track";
            this.channel_track.Size = new System.Drawing.Size(192, 42);
            this.channel_track.TabIndex = 10;
            this.channel_track.Scroll += new System.EventHandler(this.channel_track_Scroll);
            // 
            // channel_name
            // 
            this.channel_name.Location = new System.Drawing.Point(370, 25);
            this.channel_name.Name = "channel_name";
            this.channel_name.ReadOnly = true;
            this.channel_name.Size = new System.Drawing.Size(416, 19);
            this.channel_name.TabIndex = 5;
            this.channel_name.Text = "0";
            // 
            // channel_comment
            // 
            this.channel_comment.Location = new System.Drawing.Point(370, 44);
            this.channel_comment.Name = "channel_comment";
            this.channel_comment.ReadOnly = true;
            this.channel_comment.Size = new System.Drawing.Size(416, 19);
            this.channel_comment.TabIndex = 5;
            this.channel_comment.Text = "0";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(266, 28);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(79, 12);
            this.label10.TabIndex = 4;
            this.label10.Text = "Channel Name";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(266, 47);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(98, 12);
            this.label11.TabIndex = 4;
            this.label11.Text = "Channel Comment";
            // 
            // openFamosDialog
            // 
            this.openFamosDialog.DefaultExt = "DAT";
            this.openFamosDialog.Filter = "Famos File|*.DAT|All Files|*.*";
            this.openFamosDialog.Title = "Data File Selection";
            this.openFamosDialog.ValidateNames = false;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(13, 259);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(109, 12);
            this.label12.TabIndex = 4;
            this.label12.Text = "Display Data Length";
            // 
            // display_data_length
            // 
            this.display_data_length.Location = new System.Drawing.Point(131, 256);
            this.display_data_length.Name = "display_data_length";
            this.display_data_length.Size = new System.Drawing.Size(58, 19);
            this.display_data_length.TabIndex = 5;
            this.display_data_length.Text = "100";
            this.display_data_length.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.display_data_length.TextChanged += new System.EventHandler(this.display_data_length_TextChanged);
            // 
            // filter_chart
            // 
            chartArea2.AxisY2.MajorGrid.Enabled = false;
            chartArea2.AxisY2.Maximum = 1D;
            chartArea2.AxisY2.Minimum = 0D;
            chartArea2.Name = "ChartArea1";
            this.filter_chart.ChartAreas.Add(chartArea2);
            legend2.Docking = System.Windows.Forms.DataVisualization.Charting.Docking.Top;
            legend2.LegendStyle = System.Windows.Forms.DataVisualization.Charting.LegendStyle.Row;
            legend2.Name = "Legend1";
            this.filter_chart.Legends.Add(legend2);
            this.filter_chart.Location = new System.Drawing.Point(808, 5);
            this.filter_chart.Name = "filter_chart";
            this.filter_chart.Palette = System.Windows.Forms.DataVisualization.Charting.ChartColorPalette.Bright;
            series3.ChartArea = "ChartArea1";
            series3.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series3.Legend = "Legend1";
            series3.Name = "Factors (Left Axis)";
            series4.ChartArea = "ChartArea1";
            series4.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series4.Legend = "Legend1";
            series4.Name = "Window (Right Axis)";
            series4.YAxisType = System.Windows.Forms.DataVisualization.Charting.AxisType.Secondary;
            this.filter_chart.Series.Add(series3);
            this.filter_chart.Series.Add(series4);
            this.filter_chart.Size = new System.Drawing.Size(352, 300);
            this.filter_chart.TabIndex = 1;
            this.filter_chart.TabStop = false;
            this.filter_chart.Text = "chart1";
            // 
            // freq_chart
            // 
            chartArea3.AxisX.IsMarginVisible = false;
            chartArea3.AxisX.Maximum = 500D;
            chartArea3.AxisX.Minimum = 0D;
            chartArea3.AxisX.Title = "Frequency";
            chartArea3.AxisY.Interval = 20D;
            chartArea3.AxisY.IsStartedFromZero = false;
            chartArea3.AxisY.Maximum = 20D;
            chartArea3.AxisY.Minimum = -100D;
            chartArea3.AxisY.TextOrientation = System.Windows.Forms.DataVisualization.Charting.TextOrientation.Rotated270;
            chartArea3.AxisY.Title = "Gain (dB)";
            chartArea3.AxisY2.Interval = 20D;
            chartArea3.AxisY2.IsStartedFromZero = false;
            chartArea3.AxisY2.MajorGrid.Enabled = false;
            chartArea3.AxisY2.Title = "Power (dB)";
            chartArea3.Name = "ChartArea1";
            this.freq_chart.ChartAreas.Add(chartArea3);
            legend3.Docking = System.Windows.Forms.DataVisualization.Charting.Docking.Top;
            legend3.LegendStyle = System.Windows.Forms.DataVisualization.Charting.LegendStyle.Row;
            legend3.Name = "Legend1";
            this.freq_chart.Legends.Add(legend3);
            this.freq_chart.Location = new System.Drawing.Point(808, 310);
            this.freq_chart.Name = "freq_chart";
            this.freq_chart.Palette = System.Windows.Forms.DataVisualization.Charting.ChartColorPalette.Bright;
            series5.BorderWidth = 2;
            series5.ChartArea = "ChartArea1";
            series5.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series5.Legend = "Legend1";
            series5.Name = "Filter Gain";
            series6.ChartArea = "ChartArea1";
            series6.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series6.Legend = "Legend1";
            series6.Name = "Source Spectrum";
            series6.YAxisType = System.Windows.Forms.DataVisualization.Charting.AxisType.Secondary;
            this.freq_chart.Series.Add(series5);
            this.freq_chart.Series.Add(series6);
            this.freq_chart.Size = new System.Drawing.Size(352, 300);
            this.freq_chart.TabIndex = 1;
            this.freq_chart.TabStop = false;
            this.freq_chart.Text = "chart1";
            // 
            // rectangle_window
            // 
            this.rectangle_window.AutoSize = true;
            this.rectangle_window.Checked = true;
            this.rectangle_window.Location = new System.Drawing.Point(10, 21);
            this.rectangle_window.Name = "rectangle_window";
            this.rectangle_window.Size = new System.Drawing.Size(106, 16);
            this.rectangle_window.TabIndex = 12;
            this.rectangle_window.TabStop = true;
            this.rectangle_window.Text = "RectagleWindow";
            this.rectangle_window.UseVisualStyleBackColor = true;
            this.rectangle_window.CheckedChanged += new System.EventHandler(this.rectangle_window_CheckedChanged);
            // 
            // blackman_window
            // 
            this.blackman_window.AutoSize = true;
            this.blackman_window.Location = new System.Drawing.Point(122, 23);
            this.blackman_window.Name = "blackman_window";
            this.blackman_window.Size = new System.Drawing.Size(111, 16);
            this.blackman_window.TabIndex = 12;
            this.blackman_window.Text = "BlackmanWindow";
            this.blackman_window.UseVisualStyleBackColor = true;
            this.blackman_window.CheckedChanged += new System.EventHandler(this.blackman_window_CheckedChanged);
            // 
            // hann_window
            // 
            this.hann_window.AutoSize = true;
            this.hann_window.Location = new System.Drawing.Point(10, 43);
            this.hann_window.Name = "hann_window";
            this.hann_window.Size = new System.Drawing.Size(87, 16);
            this.hann_window.TabIndex = 12;
            this.hann_window.Text = "HannWindow";
            this.hann_window.UseVisualStyleBackColor = true;
            this.hann_window.CheckedChanged += new System.EventHandler(this.hann_window_CheckedChanged);
            // 
            // hamming_widow
            // 
            this.hamming_widow.AutoSize = true;
            this.hamming_widow.Location = new System.Drawing.Point(122, 45);
            this.hamming_widow.Name = "hamming_widow";
            this.hamming_widow.Size = new System.Drawing.Size(108, 16);
            this.hamming_widow.TabIndex = 12;
            this.hamming_widow.Text = "HammingWindow";
            this.hamming_widow.UseVisualStyleBackColor = true;
            this.hamming_widow.CheckedChanged += new System.EventHandler(this.hamming_widow_CheckedChanged);
            // 
            // kaiser_window
            // 
            this.kaiser_window.AutoSize = true;
            this.kaiser_window.Location = new System.Drawing.Point(245, 24);
            this.kaiser_window.Name = "kaiser_window";
            this.kaiser_window.Size = new System.Drawing.Size(93, 16);
            this.kaiser_window.TabIndex = 12;
            this.kaiser_window.Text = "KaiserWindow";
            this.kaiser_window.UseVisualStyleBackColor = true;
            this.kaiser_window.CheckedChanged += new System.EventHandler(this.kaiser_window_CheckedChanged);
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(260, 43);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(34, 12);
            this.label14.TabIndex = 4;
            this.label14.Text = "Alpha";
            // 
            // alpha
            // 
            this.alpha.Location = new System.Drawing.Point(300, 40);
            this.alpha.Name = "alpha";
            this.alpha.Size = new System.Drawing.Size(37, 19);
            this.alpha.TabIndex = 5;
            this.alpha.Text = "1.5";
            this.alpha.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.alpha.TextChanged += new System.EventHandler(this.alpha_TextChanged);
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(300, 207);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(85, 12);
            this.label15.TabIndex = 4;
            this.label15.Text = "Stop Band Gain";
            // 
            // gain
            // 
            this.gain.Location = new System.Drawing.Point(389, 204);
            this.gain.Name = "gain";
            this.gain.Size = new System.Drawing.Size(42, 19);
            this.gain.TabIndex = 5;
            this.gain.Text = "-80";
            this.gain.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.gain.TextChanged += new System.EventHandler(this.gain_TextChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.hamming_widow);
            this.groupBox1.Controls.Add(this.kaiser_window);
            this.groupBox1.Controls.Add(this.hann_window);
            this.groupBox1.Controls.Add(this.blackman_window);
            this.groupBox1.Controls.Add(this.rectangle_window);
            this.groupBox1.Controls.Add(this.alpha);
            this.groupBox1.Controls.Add(this.label14);
            this.groupBox1.Location = new System.Drawing.Point(437, 207);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(349, 73);
            this.groupBox1.TabIndex = 13;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Window for Filter";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(10, 39);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(50, 12);
            this.label13.TabIndex = 14;
            this.label13.Text = "Ch of P1";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(78, 39);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(50, 12);
            this.label16.TabIndex = 14;
            this.label16.Text = "Ch of P2";
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(140, 39);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(50, 12);
            this.label17.TabIndex = 14;
            this.label17.Text = "Ch of Ya";
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(204, 39);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(50, 12);
            this.label18.TabIndex = 14;
            this.label18.Text = "Ch of Za";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.yama);
            this.groupBox2.Controls.Add(this.umi);
            this.groupBox2.Controls.Add(this.ch_Za);
            this.groupBox2.Controls.Add(this.ch_Ya);
            this.groupBox2.Controls.Add(this.ch_P2);
            this.groupBox2.Controls.Add(this.ch_P1);
            this.groupBox2.Controls.Add(this.label18);
            this.groupBox2.Controls.Add(this.label17);
            this.groupBox2.Controls.Add(this.label16);
            this.groupBox2.Controls.Add(this.label19);
            this.groupBox2.Controls.Add(this.label13);
            this.groupBox2.Location = new System.Drawing.Point(6, 122);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(277, 85);
            this.groupBox2.TabIndex = 15;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Target Wheel";
            // 
            // yama
            // 
            this.yama.Location = new System.Drawing.Point(189, 12);
            this.yama.Name = "yama";
            this.yama.Size = new System.Drawing.Size(65, 20);
            this.yama.TabIndex = 18;
            this.yama.Text = "Yama";
            this.yama.UseVisualStyleBackColor = true;
            this.yama.Click += new System.EventHandler(this.yama_Click);
            // 
            // umi
            // 
            this.umi.Location = new System.Drawing.Point(118, 12);
            this.umi.Name = "umi";
            this.umi.Size = new System.Drawing.Size(65, 20);
            this.umi.TabIndex = 18;
            this.umi.Text = "Umi";
            this.umi.UseVisualStyleBackColor = true;
            this.umi.Click += new System.EventHandler(this.umi_Click);
            // 
            // ch_Za
            // 
            this.ch_Za.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ch_Za.FormattingEnabled = true;
            this.ch_Za.Location = new System.Drawing.Point(196, 58);
            this.ch_Za.Name = "ch_Za";
            this.ch_Za.Size = new System.Drawing.Size(58, 20);
            this.ch_Za.TabIndex = 17;
            // 
            // ch_Ya
            // 
            this.ch_Ya.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ch_Ya.FormattingEnabled = true;
            this.ch_Ya.Location = new System.Drawing.Point(134, 58);
            this.ch_Ya.Name = "ch_Ya";
            this.ch_Ya.Size = new System.Drawing.Size(58, 20);
            this.ch_Ya.TabIndex = 17;
            // 
            // ch_P2
            // 
            this.ch_P2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ch_P2.FormattingEnabled = true;
            this.ch_P2.Location = new System.Drawing.Point(70, 58);
            this.ch_P2.Name = "ch_P2";
            this.ch_P2.Size = new System.Drawing.Size(58, 20);
            this.ch_P2.TabIndex = 17;
            // 
            // ch_P1
            // 
            this.ch_P1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ch_P1.FormattingEnabled = true;
            this.ch_P1.Location = new System.Drawing.Point(9, 58);
            this.ch_P1.Name = "ch_P1";
            this.ch_P1.Size = new System.Drawing.Size(58, 20);
            this.ch_P1.TabIndex = 17;
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(44, 16);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(68, 12);
            this.label19.TabIndex = 14;
            this.label19.Text = "Auto Search";
            // 
            // peak_chart
            // 
            chartArea4.AxisX.IsMarginVisible = false;
            chartArea4.AxisX.IsStartedFromZero = false;
            chartArea4.Name = "ChartArea1";
            this.peak_chart.ChartAreas.Add(chartArea4);
            legend4.Name = "Legend1";
            this.peak_chart.Legends.Add(legend4);
            this.peak_chart.Location = new System.Drawing.Point(12, 616);
            this.peak_chart.Name = "peak_chart";
            this.peak_chart.Palette = System.Windows.Forms.DataVisualization.Charting.ChartColorPalette.Bright;
            series7.ChartArea = "ChartArea1";
            series7.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series7.Legend = "Legend1";
            series7.Name = "source";
            series8.ChartArea = "ChartArea1";
            series8.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series8.Legend = "Legend1";
            series8.Name = "result";
            this.peak_chart.Series.Add(series7);
            this.peak_chart.Series.Add(series8);
            this.peak_chart.Size = new System.Drawing.Size(774, 300);
            this.peak_chart.TabIndex = 1;
            this.peak_chart.TabStop = false;
            this.peak_chart.Text = "chart1";
            // 
            // upper_val
            // 
            this.upper_val.Location = new System.Drawing.Point(302, 134);
            this.upper_val.Name = "upper_val";
            this.upper_val.ReadOnly = true;
            this.upper_val.Size = new System.Drawing.Size(29, 19);
            this.upper_val.TabIndex = 5;
            this.upper_val.Text = "150";
            this.upper_val.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // lower_val
            // 
            this.lower_val.Location = new System.Drawing.Point(302, 174);
            this.lower_val.Name = "lower_val";
            this.lower_val.ReadOnly = true;
            this.lower_val.Size = new System.Drawing.Size(29, 19);
            this.lower_val.TabIndex = 5;
            this.lower_val.Text = "0";
            this.lower_val.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(413, 137);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(18, 12);
            this.label20.TabIndex = 11;
            this.label20.Text = "Hz";
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Location = new System.Drawing.Point(413, 180);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(18, 12);
            this.label21.TabIndex = 11;
            this.label21.Text = "Hz";
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(258, 423);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(409, 31);
            this.progressBar1.TabIndex = 16;
            this.progressBar1.Visible = false;
            // 
            // wave_filter
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1172, 923);
            this.Controls.Add(this.auto_update);
            this.Controls.Add(this.update);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label21);
            this.Controls.Add(this.label20);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.lower_fc_track);
            this.Controls.Add(this.upper_fc_track);
            this.Controls.Add(this.channel_track);
            this.Controls.Add(this.tap_track);
            this.Controls.Add(this.select_file);
            this.Controls.Add(this.file_path);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.filter_length);
            this.Controls.Add(this.number_of_tap);
            this.Controls.Add(this.channel_comment);
            this.Controls.Add(this.channel_name);
            this.Controls.Add(this.channel);
            this.Controls.Add(this.data_length);
            this.Controls.Add(this.sampling_rate);
            this.Controls.Add(this.display_data_length);
            this.Controls.Add(this.nyquist_frequency);
            this.Controls.Add(this.lower_val);
            this.Controls.Add(this.upper_val);
            this.Controls.Add(this.upper_fc);
            this.Controls.Add(this.gain);
            this.Controls.Add(this.lower_fc);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label15);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.data_start);
            this.Controls.Add(this.freq_chart);
            this.Controls.Add(this.filter_chart);
            this.Controls.Add(this.peak_chart);
            this.Controls.Add(this.wave_chart);
            this.Name = "wave_filter";
            this.Text = "WaveFilter";
            ((System.ComponentModel.ISupportInitialize)(this.wave_chart)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tap_track)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.upper_fc_track)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lower_fc_track)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.channel_track)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.filter_chart)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.freq_chart)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.peak_chart)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox auto_update;
        private System.Windows.Forms.DataVisualization.Charting.Chart wave_chart;
        private System.Windows.Forms.HScrollBar data_start;
        private System.Windows.Forms.Button update;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox lower_fc;
        private System.Windows.Forms.TextBox upper_fc;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox nyquist_frequency;
        private System.Windows.Forms.TextBox sampling_rate;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox file_path;
        private System.Windows.Forms.Button select_file;
        private System.Windows.Forms.TrackBar tap_track;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox number_of_tap;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox filter_length;
        private System.Windows.Forms.TrackBar upper_fc_track;
        private System.Windows.Forms.TrackBar lower_fc_track;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox data_length;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox channel;
        private System.Windows.Forms.TrackBar channel_track;
        private System.Windows.Forms.TextBox channel_name;
        private System.Windows.Forms.TextBox channel_comment;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.OpenFileDialog openFamosDialog;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox display_data_length;
        private System.Windows.Forms.DataVisualization.Charting.Chart filter_chart;
        private System.Windows.Forms.RadioButton rectangle_window;
        private System.Windows.Forms.RadioButton blackman_window;
        private System.Windows.Forms.RadioButton hann_window;
        private System.Windows.Forms.RadioButton hamming_widow;
        private System.Windows.Forms.RadioButton kaiser_window;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.TextBox alpha;
        protected System.Windows.Forms.DataVisualization.Charting.Chart freq_chart;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.TextBox gain;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ComboBox ch_Za;
        private System.Windows.Forms.ComboBox ch_Ya;
        private System.Windows.Forms.ComboBox ch_P2;
        private System.Windows.Forms.ComboBox ch_P1;
        private System.Windows.Forms.Button yama;
        private System.Windows.Forms.Button umi;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.DataVisualization.Charting.Chart peak_chart;
        private System.Windows.Forms.TextBox upper_val;
        private System.Windows.Forms.TextBox lower_val;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.ProgressBar progressBar1;
    }
}

