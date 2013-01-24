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
            this.auto_update = new System.Windows.Forms.CheckBox();
            this.wave_chart = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.hScrollBar1 = new System.Windows.Forms.HScrollBar();
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
            this.textBox3 = new System.Windows.Forms.TextBox();
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
            ((System.ComponentModel.ISupportInitialize)(this.wave_chart)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tap_track)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.upper_fc_track)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lower_fc_track)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.channel_track)).BeginInit();
            this.SuspendLayout();
            // 
            // auto_update
            // 
            this.auto_update.AutoSize = true;
            this.auto_update.Location = new System.Drawing.Point(596, 214);
            this.auto_update.Name = "auto_update";
            this.auto_update.Size = new System.Drawing.Size(84, 16);
            this.auto_update.TabIndex = 0;
            this.auto_update.Text = "AutoUpdate";
            this.auto_update.UseVisualStyleBackColor = true;
            // 
            // wave_chart
            // 
            chartArea1.Name = "ChartArea1";
            this.wave_chart.ChartAreas.Add(chartArea1);
            legend1.Name = "Legend1";
            this.wave_chart.Legends.Add(legend1);
            this.wave_chart.Location = new System.Drawing.Point(12, 283);
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
            this.wave_chart.Size = new System.Drawing.Size(774, 337);
            this.wave_chart.TabIndex = 1;
            this.wave_chart.TabStop = false;
            this.wave_chart.Text = "chart1";
            // 
            // hScrollBar1
            // 
            this.hScrollBar1.Location = new System.Drawing.Point(12, 623);
            this.hScrollBar1.Name = "hScrollBar1";
            this.hScrollBar1.Size = new System.Drawing.Size(774, 24);
            this.hScrollBar1.TabIndex = 2;
            // 
            // update
            // 
            this.update.Location = new System.Drawing.Point(700, 207);
            this.update.Name = "update";
            this.update.Size = new System.Drawing.Size(83, 28);
            this.update.TabIndex = 3;
            this.update.Text = "Update";
            this.update.UseVisualStyleBackColor = true;
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
            this.lower_fc.Location = new System.Drawing.Point(370, 174);
            this.lower_fc.Name = "lower_fc";
            this.lower_fc.ReadOnly = true;
            this.lower_fc.Size = new System.Drawing.Size(58, 19);
            this.lower_fc.TabIndex = 5;
            this.lower_fc.Text = "0";
            this.lower_fc.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // upper_fc
            // 
            this.upper_fc.Location = new System.Drawing.Point(370, 135);
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
            this.label3.Location = new System.Drawing.Point(16, 101);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(79, 12);
            this.label3.TabIndex = 4;
            this.label3.Text = "Sampling Rate";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(16, 122);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(101, 12);
            this.label4.TabIndex = 4;
            this.label4.Text = "Nyquist Frequency";
            // 
            // nyquist_frequency
            // 
            this.nyquist_frequency.Location = new System.Drawing.Point(155, 115);
            this.nyquist_frequency.Name = "nyquist_frequency";
            this.nyquist_frequency.ReadOnly = true;
            this.nyquist_frequency.Size = new System.Drawing.Size(58, 19);
            this.nyquist_frequency.TabIndex = 5;
            this.nyquist_frequency.Text = "0";
            this.nyquist_frequency.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // sampling_rate
            // 
            this.sampling_rate.Location = new System.Drawing.Point(155, 94);
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
            // textBox3
            // 
            this.textBox3.Location = new System.Drawing.Point(140, 2);
            this.textBox3.Name = "textBox3";
            this.textBox3.ReadOnly = true;
            this.textBox3.Size = new System.Drawing.Size(646, 19);
            this.textBox3.TabIndex = 8;
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
            this.filter_length.Text = "301";
            this.filter_length.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // upper_fc_track
            // 
            this.upper_fc_track.Location = new System.Drawing.Point(434, 120);
            this.upper_fc_track.Maximum = 300;
            this.upper_fc_track.Minimum = 1;
            this.upper_fc_track.Name = "upper_fc_track";
            this.upper_fc_track.Size = new System.Drawing.Size(349, 42);
            this.upper_fc_track.TabIndex = 10;
            this.upper_fc_track.TickFrequency = 10;
            this.upper_fc_track.Value = 150;
            this.upper_fc_track.Scroll += new System.EventHandler(this.upper_fc_track_Scroll);
            // 
            // lower_fc_track
            // 
            this.lower_fc_track.Location = new System.Drawing.Point(434, 159);
            this.lower_fc_track.Maximum = 300;
            this.lower_fc_track.Minimum = 1;
            this.lower_fc_track.Name = "lower_fc_track";
            this.lower_fc_track.Size = new System.Drawing.Size(349, 42);
            this.lower_fc_track.TabIndex = 10;
            this.lower_fc_track.TickFrequency = 10;
            this.lower_fc_track.Value = 150;
            this.lower_fc_track.Scroll += new System.EventHandler(this.lower_fc_track_Scroll);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(16, 80);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(63, 12);
            this.label8.TabIndex = 4;
            this.label8.Text = "DataLength";
            // 
            // data_length
            // 
            this.data_length.Location = new System.Drawing.Point(155, 73);
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
            this.label12.Location = new System.Drawing.Point(16, 159);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(109, 12);
            this.label12.TabIndex = 4;
            this.label12.Text = "Display Data Length";
            // 
            // display_data_length
            // 
            this.display_data_length.Location = new System.Drawing.Point(155, 156);
            this.display_data_length.Name = "display_data_length";
            this.display_data_length.Size = new System.Drawing.Size(58, 19);
            this.display_data_length.TabIndex = 5;
            this.display_data_length.Text = "0";
            this.display_data_length.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.display_data_length.TextChanged += new System.EventHandler(this.display_data_length_TextChanged);
            // 
            // wave_filter
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(795, 658);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.lower_fc_track);
            this.Controls.Add(this.upper_fc_track);
            this.Controls.Add(this.channel_track);
            this.Controls.Add(this.tap_track);
            this.Controls.Add(this.select_file);
            this.Controls.Add(this.textBox3);
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
            this.Controls.Add(this.upper_fc);
            this.Controls.Add(this.lower_fc);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.update);
            this.Controls.Add(this.hScrollBar1);
            this.Controls.Add(this.wave_chart);
            this.Controls.Add(this.auto_update);
            this.Name = "wave_filter";
            this.Text = "WaveFilter";
            ((System.ComponentModel.ISupportInitialize)(this.wave_chart)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tap_track)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.upper_fc_track)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lower_fc_track)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.channel_track)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox auto_update;
        private System.Windows.Forms.DataVisualization.Charting.Chart wave_chart;
        private System.Windows.Forms.HScrollBar hScrollBar1;
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
        private System.Windows.Forms.TextBox textBox3;
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
    }
}

