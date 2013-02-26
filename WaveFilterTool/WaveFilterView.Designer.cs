namespace WaveFilterTool
{
    partial class WaveFilterView
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
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.Label tapLabel1;
            System.Windows.Forms.Label lowerFcLabel;
            System.Windows.Forms.Label upperFcLabel;
            System.Windows.Forms.Label currentChannelLabel;
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series2 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea2 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend2 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series3 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series4 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Title title1 = new System.Windows.Forms.DataVisualization.Charting.Title();
            this.MainMenu = new System.Windows.Forms.MenuStrip();
            this.ファイルToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.famosファイルToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cSVファイルToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.共和電業形式ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.一般ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.書き出しToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.wave_chart = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.sp_chart = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.label1 = new System.Windows.Forms.Label();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.tapTrackBar = new System.Windows.Forms.TrackBar();
            this.tapNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.lowerFcNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.lowerFcTrackBar = new System.Windows.Forms.TrackBar();
            this.upperFcNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.upperFcTrackBar = new System.Windows.Forms.TrackBar();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lowerFcTextBox = new System.Windows.Forms.TextBox();
            this.upperFcTextBox = new System.Windows.Forms.TextBox();
            this.openFamosFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.openCsvFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.wave_info = new System.Windows.Forms.TextBox();
            this.channelNamesBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.channelListBox = new System.Windows.Forms.ListBox();
            this.currentChannelLabel2 = new System.Windows.Forms.Label();
            this.dataBindingSource = new System.Windows.Forms.BindingSource(this.components);
            tapLabel1 = new System.Windows.Forms.Label();
            lowerFcLabel = new System.Windows.Forms.Label();
            upperFcLabel = new System.Windows.Forms.Label();
            currentChannelLabel = new System.Windows.Forms.Label();
            this.MainMenu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.wave_chart)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.sp_chart)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tapTrackBar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tapNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lowerFcNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lowerFcTrackBar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.upperFcNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.upperFcTrackBar)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.channelNamesBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // tapLabel1
            // 
            tapLabel1.AutoSize = true;
            tapLabel1.Location = new System.Drawing.Point(43, 25);
            tapLabel1.Name = "tapLabel1";
            tapLabel1.Size = new System.Drawing.Size(26, 12);
            tapLabel1.TabIndex = 9;
            tapLabel1.Text = "Tap:";
            // 
            // lowerFcLabel
            // 
            lowerFcLabel.AutoSize = true;
            lowerFcLabel.Location = new System.Drawing.Point(15, 75);
            lowerFcLabel.Name = "lowerFcLabel";
            lowerFcLabel.Size = new System.Drawing.Size(54, 12);
            lowerFcLabel.TabIndex = 10;
            lowerFcLabel.Text = "Lower Fc:";
            // 
            // upperFcLabel
            // 
            upperFcLabel.AutoSize = true;
            upperFcLabel.Location = new System.Drawing.Point(15, 50);
            upperFcLabel.Name = "upperFcLabel";
            upperFcLabel.Size = new System.Drawing.Size(54, 12);
            upperFcLabel.TabIndex = 12;
            upperFcLabel.Text = "Upper Fc:";
            // 
            // currentChannelLabel
            // 
            currentChannelLabel.AutoSize = true;
            currentChannelLabel.Location = new System.Drawing.Point(2, 206);
            currentChannelLabel.Name = "currentChannelLabel";
            currentChannelLabel.Size = new System.Drawing.Size(51, 12);
            currentChannelLabel.TabIndex = 16;
            currentChannelLabel.Text = "チャンネル";
            // 
            // MainMenu
            // 
            this.MainMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ファイルToolStripMenuItem,
            this.書き出しToolStripMenuItem});
            this.MainMenu.Location = new System.Drawing.Point(0, 0);
            this.MainMenu.Name = "MainMenu";
            this.MainMenu.Size = new System.Drawing.Size(891, 24);
            this.MainMenu.TabIndex = 0;
            this.MainMenu.Text = "menuStrip1";
            // 
            // ファイルToolStripMenuItem
            // 
            this.ファイルToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.famosファイルToolStripMenuItem,
            this.cSVファイルToolStripMenuItem});
            this.ファイルToolStripMenuItem.Name = "ファイルToolStripMenuItem";
            this.ファイルToolStripMenuItem.Size = new System.Drawing.Size(51, 20);
            this.ファイルToolStripMenuItem.Text = "開く(&O)";
            // 
            // famosファイルToolStripMenuItem
            // 
            this.famosファイルToolStripMenuItem.Name = "famosファイルToolStripMenuItem";
            this.famosファイルToolStripMenuItem.Size = new System.Drawing.Size(142, 22);
            this.famosファイルToolStripMenuItem.Text = "Famos ファイル";
            this.famosファイルToolStripMenuItem.Click += new System.EventHandler(this.famosファイルToolStripMenuItem_Click);
            // 
            // cSVファイルToolStripMenuItem
            // 
            this.cSVファイルToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.共和電業形式ToolStripMenuItem,
            this.一般ToolStripMenuItem});
            this.cSVファイルToolStripMenuItem.Name = "cSVファイルToolStripMenuItem";
            this.cSVファイルToolStripMenuItem.Size = new System.Drawing.Size(142, 22);
            this.cSVファイルToolStripMenuItem.Text = "CSV ファイル";
            // 
            // 共和電業形式ToolStripMenuItem
            // 
            this.共和電業形式ToolStripMenuItem.Name = "共和電業形式ToolStripMenuItem";
            this.共和電業形式ToolStripMenuItem.Size = new System.Drawing.Size(142, 22);
            this.共和電業形式ToolStripMenuItem.Text = "共和電業形式";
            this.共和電業形式ToolStripMenuItem.Click += new System.EventHandler(this.共和電業形式ToolStripMenuItem_Click);
            // 
            // 一般ToolStripMenuItem
            // 
            this.一般ToolStripMenuItem.Name = "一般ToolStripMenuItem";
            this.一般ToolStripMenuItem.Size = new System.Drawing.Size(142, 22);
            this.一般ToolStripMenuItem.Text = "一般";
            this.一般ToolStripMenuItem.Click += new System.EventHandler(this.一般ToolStripMenuItem_Click);
            // 
            // 書き出しToolStripMenuItem
            // 
            this.書き出しToolStripMenuItem.Name = "書き出しToolStripMenuItem";
            this.書き出しToolStripMenuItem.Size = new System.Drawing.Size(107, 20);
            this.書き出しToolStripMenuItem.Text = "CSVで書き出し(&E)";
            // 
            // wave_chart
            // 
            chartArea1.Name = "ChartArea1";
            this.wave_chart.ChartAreas.Add(chartArea1);
            legend1.Name = "Legend1";
            this.wave_chart.Legends.Add(legend1);
            this.wave_chart.Location = new System.Drawing.Point(0, 407);
            this.wave_chart.MinimumSize = new System.Drawing.Size(200, 100);
            this.wave_chart.Name = "wave_chart";
            series1.ChartArea = "ChartArea1";
            series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.FastLine;
            series1.Legend = "Legend1";
            series1.Name = "元データ";
            series2.ChartArea = "ChartArea1";
            series2.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.FastLine;
            series2.Legend = "Legend1";
            series2.Name = "フィルタ後";
            this.wave_chart.Series.Add(series1);
            this.wave_chart.Series.Add(series2);
            this.wave_chart.Size = new System.Drawing.Size(871, 261);
            this.wave_chart.TabIndex = 1;
            this.wave_chart.Text = "chart1";
            // 
            // sp_chart
            // 
            chartArea2.AxisX.Title = "周波数(Hz)";
            chartArea2.AxisY.IsLogarithmic = true;
            chartArea2.AxisY.Title = "パワースペクトル";
            chartArea2.Name = "ChartArea1";
            this.sp_chart.ChartAreas.Add(chartArea2);
            legend2.Docking = System.Windows.Forms.DataVisualization.Charting.Docking.Top;
            legend2.Name = "Legend1";
            this.sp_chart.Legends.Add(legend2);
            this.sp_chart.Location = new System.Drawing.Point(439, 27);
            this.sp_chart.Margin = new System.Windows.Forms.Padding(1);
            this.sp_chart.MinimumSize = new System.Drawing.Size(100, 100);
            this.sp_chart.Name = "sp_chart";
            series3.ChartArea = "ChartArea1";
            series3.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.FastLine;
            series3.Legend = "Legend1";
            series3.Name = "元データ";
            series4.ChartArea = "ChartArea1";
            series4.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.FastLine;
            series4.Legend = "Legend1";
            series4.Name = "フィルタ後";
            this.sp_chart.Series.Add(series3);
            this.sp_chart.Series.Add(series4);
            this.sp_chart.Size = new System.Drawing.Size(432, 388);
            this.sp_chart.TabIndex = 2;
            this.sp_chart.Text = "chart1";
            title1.Name = "Title1";
            title1.Text = "パワースペクトル";
            this.sp_chart.Titles.Add(title1);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(0, 35);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(91, 12);
            this.label1.TabIndex = 4;
            this.label1.Text = "波形データの情報";
            // 
            // statusStrip1
            // 
            this.statusStrip1.Location = new System.Drawing.Point(0, 688);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(891, 22);
            this.statusStrip1.TabIndex = 5;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // tapTrackBar
            // 
            this.tapTrackBar.AutoSize = false;
            this.tapTrackBar.DataBindings.Add(new System.Windows.Forms.Binding("Value", this.dataBindingSource, "Tap", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.tapTrackBar.DataBindings.Add(new System.Windows.Forms.Binding("Maximum", this.dataBindingSource, "TapMax", true, System.Windows.Forms.DataSourceUpdateMode.OnValidation, "100"));
            this.tapTrackBar.Location = new System.Drawing.Point(198, 18);
            this.tapTrackBar.Maximum = 1000;
            this.tapTrackBar.Minimum = 1;
            this.tapTrackBar.Name = "tapTrackBar";
            this.tapTrackBar.Size = new System.Drawing.Size(104, 19);
            this.tapTrackBar.TabIndex = 8;
            this.tapTrackBar.TickFrequency = 100;
            this.tapTrackBar.Value = 1;
            // 
            // tapNumericUpDown
            // 
            this.tapNumericUpDown.DataBindings.Add(new System.Windows.Forms.Binding("Value", this.dataBindingSource, "Tap", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.tapNumericUpDown.Location = new System.Drawing.Point(138, 18);
            this.tapNumericUpDown.Name = "tapNumericUpDown";
            this.tapNumericUpDown.Size = new System.Drawing.Size(54, 19);
            this.tapNumericUpDown.TabIndex = 10;
            this.tapNumericUpDown.Validating += new System.ComponentModel.CancelEventHandler(this.tapNumericUpDown_Validating);
            // 
            // lowerFcNumericUpDown
            // 
            this.lowerFcNumericUpDown.DataBindings.Add(new System.Windows.Forms.Binding("Value", this.dataBindingSource, "LowerFcNum", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.lowerFcNumericUpDown.Location = new System.Drawing.Point(138, 68);
            this.lowerFcNumericUpDown.Name = "lowerFcNumericUpDown";
            this.lowerFcNumericUpDown.Size = new System.Drawing.Size(54, 19);
            this.lowerFcNumericUpDown.TabIndex = 11;
            // 
            // lowerFcTrackBar
            // 
            this.lowerFcTrackBar.AutoSize = false;
            this.lowerFcTrackBar.DataBindings.Add(new System.Windows.Forms.Binding("Value", this.dataBindingSource, "LowerFcNum", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.lowerFcTrackBar.DataBindings.Add(new System.Windows.Forms.Binding("Maximum", this.dataBindingSource, "TapMax", true, System.Windows.Forms.DataSourceUpdateMode.OnValidation, "100"));
            this.lowerFcTrackBar.Location = new System.Drawing.Point(198, 68);
            this.lowerFcTrackBar.Name = "lowerFcTrackBar";
            this.lowerFcTrackBar.Size = new System.Drawing.Size(104, 19);
            this.lowerFcTrackBar.TabIndex = 12;
            // 
            // upperFcNumericUpDown
            // 
            this.upperFcNumericUpDown.DataBindings.Add(new System.Windows.Forms.Binding("Value", this.dataBindingSource, "UpperFcNum", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.upperFcNumericUpDown.Location = new System.Drawing.Point(138, 43);
            this.upperFcNumericUpDown.Name = "upperFcNumericUpDown";
            this.upperFcNumericUpDown.Size = new System.Drawing.Size(54, 19);
            this.upperFcNumericUpDown.TabIndex = 13;
            // 
            // upperFcTrackBar
            // 
            this.upperFcTrackBar.AutoSize = false;
            this.upperFcTrackBar.DataBindings.Add(new System.Windows.Forms.Binding("Value", this.dataBindingSource, "UpperFcNum", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.upperFcTrackBar.DataBindings.Add(new System.Windows.Forms.Binding("Maximum", this.dataBindingSource, "TapMax", true, System.Windows.Forms.DataSourceUpdateMode.OnValidation, "100"));
            this.upperFcTrackBar.Location = new System.Drawing.Point(198, 43);
            this.upperFcTrackBar.Name = "upperFcTrackBar";
            this.upperFcTrackBar.Size = new System.Drawing.Size(104, 19);
            this.upperFcTrackBar.TabIndex = 15;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.upperFcTrackBar);
            this.groupBox1.Controls.Add(this.lowerFcTextBox);
            this.groupBox1.Controls.Add(this.upperFcTextBox);
            this.groupBox1.Controls.Add(upperFcLabel);
            this.groupBox1.Controls.Add(this.upperFcNumericUpDown);
            this.groupBox1.Controls.Add(this.lowerFcTrackBar);
            this.groupBox1.Controls.Add(lowerFcLabel);
            this.groupBox1.Controls.Add(this.lowerFcNumericUpDown);
            this.groupBox1.Controls.Add(tapLabel1);
            this.groupBox1.Controls.Add(this.tapNumericUpDown);
            this.groupBox1.Controls.Add(this.tapTrackBar);
            this.groupBox1.Location = new System.Drawing.Point(2, 302);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(310, 99);
            this.groupBox1.TabIndex = 16;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "フィルタ設定";
            // 
            // lowerFcTextBox
            // 
            this.lowerFcTextBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.dataBindingSource, "LowerFc", true));
            this.lowerFcTextBox.Location = new System.Drawing.Point(75, 68);
            this.lowerFcTextBox.Name = "lowerFcTextBox";
            this.lowerFcTextBox.ReadOnly = true;
            this.lowerFcTextBox.Size = new System.Drawing.Size(57, 19);
            this.lowerFcTextBox.TabIndex = 17;
            // 
            // upperFcTextBox
            // 
            this.upperFcTextBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.dataBindingSource, "UpperFc", true));
            this.upperFcTextBox.Location = new System.Drawing.Point(75, 47);
            this.upperFcTextBox.Name = "upperFcTextBox";
            this.upperFcTextBox.ReadOnly = true;
            this.upperFcTextBox.Size = new System.Drawing.Size(57, 19);
            this.upperFcTextBox.TabIndex = 18;
            // 
            // openFamosFileDialog
            // 
            this.openFamosFileDialog.Filter = "Famosファイル (*.DAT)|*.DAT|全てのファイル (*.*)|*.*";
            // 
            // openCsvFileDialog
            // 
            this.openCsvFileDialog.FileName = "openFileDialog1";
            this.openCsvFileDialog.Filter = "CSVファイル (*.CSV)|*.CSV|全てのファイル (*.*)|*.*";
            // 
            // wave_info
            // 
            this.wave_info.Location = new System.Drawing.Point(0, 50);
            this.wave_info.MinimumSize = new System.Drawing.Size(100, 4);
            this.wave_info.Multiline = true;
            this.wave_info.Name = "wave_info";
            this.wave_info.ReadOnly = true;
            this.wave_info.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.wave_info.Size = new System.Drawing.Size(435, 147);
            this.wave_info.TabIndex = 3;
            // 
            // channelNamesBindingSource
            // 
            this.channelNamesBindingSource.DataMember = "ChannelNames";
            this.channelNamesBindingSource.DataSource = this.dataBindingSource;
            // 
            // channelListBox
            // 
            this.channelListBox.FormattingEnabled = true;
            this.channelListBox.ItemHeight = 12;
            this.channelListBox.Location = new System.Drawing.Point(2, 221);
            this.channelListBox.Name = "channelListBox";
            this.channelListBox.Size = new System.Drawing.Size(144, 76);
            this.channelListBox.TabIndex = 17;
            this.channelListBox.SelectedIndexChanged += new System.EventHandler(this.currentChannelListBox_SelectedIndexChanged);
            // 
            // currentChannelLabel2
            // 
            this.currentChannelLabel2.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.dataBindingSource, "CurrentChannel", true));
            this.currentChannelLabel2.Location = new System.Drawing.Point(59, 206);
            this.currentChannelLabel2.Name = "currentChannelLabel2";
            this.currentChannelLabel2.Size = new System.Drawing.Size(87, 12);
            this.currentChannelLabel2.TabIndex = 18;
            // dataBindingSource
            // 
            this.dataBindingSource.DataSource = typeof(WaveFilterTool.Data);
            // 
            // WaveFilterView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(891, 710);
            this.Controls.Add(this.currentChannelLabel2);
            this.Controls.Add(this.channelListBox);
            this.Controls.Add(currentChannelLabel);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.wave_info);
            this.Controls.Add(this.sp_chart);
            this.Controls.Add(this.wave_chart);
            this.Controls.Add(this.MainMenu);
            this.MainMenuStrip = this.MainMenu;
            this.Name = "WaveFilterView";
            this.Text = "波形データフィルタリングツール";
            this.Resize += new System.EventHandler(this.WaveFilterView_Resize);
            this.MainMenu.ResumeLayout(false);
            this.MainMenu.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.wave_chart)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.sp_chart)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tapTrackBar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tapNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lowerFcNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lowerFcTrackBar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.upperFcNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.upperFcTrackBar)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.channelNamesBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataBindingSource)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip MainMenu;
        private System.Windows.Forms.ToolStripMenuItem ファイルToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem famosファイルToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cSVファイルToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 共和電業形式ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 一般ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 書き出しToolStripMenuItem;
        private System.Windows.Forms.DataVisualization.Charting.Chart wave_chart;
        private System.Windows.Forms.DataVisualization.Charting.Chart sp_chart;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.BindingSource dataBindingSource;
        private System.Windows.Forms.TrackBar tapTrackBar;
        private System.Windows.Forms.NumericUpDown tapNumericUpDown;
        private System.Windows.Forms.NumericUpDown lowerFcNumericUpDown;
        private System.Windows.Forms.TrackBar lowerFcTrackBar;
        private System.Windows.Forms.NumericUpDown upperFcNumericUpDown;
        private System.Windows.Forms.TrackBar upperFcTrackBar;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox lowerFcTextBox;
        private System.Windows.Forms.TextBox upperFcTextBox;
        private System.Windows.Forms.OpenFileDialog openFamosFileDialog;
        private System.Windows.Forms.OpenFileDialog openCsvFileDialog;
        private System.Windows.Forms.TextBox wave_info;
        private System.Windows.Forms.BindingSource channelNamesBindingSource;
        private System.Windows.Forms.ListBox channelListBox;
        private System.Windows.Forms.Label currentChannelLabel2;
    }
}

