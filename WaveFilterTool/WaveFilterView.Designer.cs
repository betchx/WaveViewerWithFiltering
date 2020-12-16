﻿namespace WaveFilterTool
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
      System.Windows.Forms.Label numberOfDisplayedDataLabel;
      System.Windows.Forms.Label gainLabel;
      System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea3 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
      System.Windows.Forms.DataVisualization.Charting.Legend legend3 = new System.Windows.Forms.DataVisualization.Charting.Legend();
      System.Windows.Forms.DataVisualization.Charting.Series series5 = new System.Windows.Forms.DataVisualization.Charting.Series();
      System.Windows.Forms.DataVisualization.Charting.Series series6 = new System.Windows.Forms.DataVisualization.Charting.Series();
      System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea4 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
      System.Windows.Forms.DataVisualization.Charting.Legend legend4 = new System.Windows.Forms.DataVisualization.Charting.Legend();
      System.Windows.Forms.DataVisualization.Charting.Series series7 = new System.Windows.Forms.DataVisualization.Charting.Series();
      System.Windows.Forms.DataVisualization.Charting.Series series8 = new System.Windows.Forms.DataVisualization.Charting.Series();
      System.Windows.Forms.DataVisualization.Charting.Title title2 = new System.Windows.Forms.DataVisualization.Charting.Title();
      this.MainMenu = new System.Windows.Forms.MenuStrip();
      this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.readFamosFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.readGeneralCsvFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.readKyowaFormatCsvFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.outputCSVToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
      this.terminateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.waveChart = new System.Windows.Forms.DataVisualization.Charting.Chart();
      this.spChart = new System.Windows.Forms.DataVisualization.Charting.Chart();
      this.label1 = new System.Windows.Forms.Label();
      this.statusStrip1 = new System.Windows.Forms.StatusStrip();
      this.tapTrackBar = new System.Windows.Forms.TrackBar();
      this.dataBindingSource = new System.Windows.Forms.BindingSource(this.components);
      this.tapNumericUpDown = new System.Windows.Forms.NumericUpDown();
      this.lowerFcNumericUpDown = new System.Windows.Forms.NumericUpDown();
      this.lowerFcTrackBar = new System.Windows.Forms.TrackBar();
      this.upperFcNumericUpDown = new System.Windows.Forms.NumericUpDown();
      this.upperFcTrackBar = new System.Windows.Forms.TrackBar();
      this.groupBox1 = new System.Windows.Forms.GroupBox();
      this.gainComboBox = new System.Windows.Forms.ComboBox();
      this.lowerFcTextBox = new System.Windows.Forms.TextBox();
      this.upperFcTextBox = new System.Windows.Forms.TextBox();
      this.openFamosFileDialog = new System.Windows.Forms.OpenFileDialog();
      this.openCsvFileDialog = new System.Windows.Forms.OpenFileDialog();
      this.wave_info = new System.Windows.Forms.TextBox();
      this.channelNamesBindingSource = new System.Windows.Forms.BindingSource(this.components);
      this.channelListBox = new System.Windows.Forms.ListBox();
      this.currentChannelLabel2 = new System.Windows.Forms.Label();
      this.numberOfDisplayedDataComboBox = new System.Windows.Forms.ComboBox();
      tapLabel1 = new System.Windows.Forms.Label();
      lowerFcLabel = new System.Windows.Forms.Label();
      upperFcLabel = new System.Windows.Forms.Label();
      currentChannelLabel = new System.Windows.Forms.Label();
      numberOfDisplayedDataLabel = new System.Windows.Forms.Label();
      gainLabel = new System.Windows.Forms.Label();
      this.MainMenu.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.waveChart)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.spChart)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.tapTrackBar)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.dataBindingSource)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.tapNumericUpDown)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.lowerFcNumericUpDown)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.lowerFcTrackBar)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.upperFcNumericUpDown)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.upperFcTrackBar)).BeginInit();
      this.groupBox1.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.channelNamesBindingSource)).BeginInit();
      this.SuspendLayout();
      //
      // tapLabel1
      //
      tapLabel1.AutoSize = true;
      tapLabel1.Location = new System.Drawing.Point(93, 50);
      tapLabel1.Margin = new System.Windows.Forms.Padding(7, 0, 7, 0);
      tapLabel1.Name = "tapLabel1";
      tapLabel1.Size = new System.Drawing.Size(52, 24);
      tapLabel1.TabIndex = 9;
      tapLabel1.Text = "Tap:";
      //
      // lowerFcLabel
      //
      lowerFcLabel.AutoSize = true;
      lowerFcLabel.Location = new System.Drawing.Point(33, 150);
      lowerFcLabel.Margin = new System.Windows.Forms.Padding(7, 0, 7, 0);
      lowerFcLabel.Name = "lowerFcLabel";
      lowerFcLabel.Size = new System.Drawing.Size(108, 24);
      lowerFcLabel.TabIndex = 10;
      lowerFcLabel.Text = "Lower Fc:";
      //
      // upperFcLabel
      //
      upperFcLabel.AutoSize = true;
      upperFcLabel.Location = new System.Drawing.Point(33, 100);
      upperFcLabel.Margin = new System.Windows.Forms.Padding(7, 0, 7, 0);
      upperFcLabel.Name = "upperFcLabel";
      upperFcLabel.Size = new System.Drawing.Size(106, 24);
      upperFcLabel.TabIndex = 12;
      upperFcLabel.Text = "Upper Fc:";
      //
      // currentChannelLabel
      //
      currentChannelLabel.AutoSize = true;
      currentChannelLabel.Location = new System.Drawing.Point(4, 412);
      currentChannelLabel.Margin = new System.Windows.Forms.Padding(7, 0, 7, 0);
      currentChannelLabel.Name = "currentChannelLabel";
      currentChannelLabel.Size = new System.Drawing.Size(103, 24);
      currentChannelLabel.TabIndex = 16;
      currentChannelLabel.Text = "チャンネル";
      //
      // numberOfDisplayedDataLabel
      //
      numberOfDisplayedDataLabel.AutoSize = true;
      numberOfDisplayedDataLabel.Location = new System.Drawing.Point(371, 412);
      numberOfDisplayedDataLabel.Margin = new System.Windows.Forms.Padding(7, 0, 7, 0);
      numberOfDisplayedDataLabel.Name = "numberOfDisplayedDataLabel";
      numberOfDisplayedDataLabel.Size = new System.Drawing.Size(177, 24);
      numberOfDisplayedDataLabel.TabIndex = 18;
      numberOfDisplayedDataLabel.Text = "表示するデータ数";
      //
      // gainLabel
      //
      gainLabel.AutoSize = true;
      gainLabel.Location = new System.Drawing.Point(665, 42);
      gainLabel.Margin = new System.Windows.Forms.Padding(7, 0, 7, 0);
      gainLabel.Name = "gainLabel";
      gainLabel.Size = new System.Drawing.Size(59, 24);
      gainLabel.TabIndex = 18;
      gainLabel.Text = "Gain:";
      //
      // MainMenu
      //
      this.MainMenu.GripMargin = new System.Windows.Forms.Padding(2, 2, 0, 2);
      this.MainMenu.ImageScalingSize = new System.Drawing.Size(32, 32);
      this.MainMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.outputCSVToolStripMenuItem,
            this.toolStripMenuItem1,
            this.terminateToolStripMenuItem});
      this.MainMenu.Location = new System.Drawing.Point(0, 0);
      this.MainMenu.Name = "MainMenu";
      this.MainMenu.Padding = new System.Windows.Forms.Padding(7, 2, 0, 2);
      this.MainMenu.Size = new System.Drawing.Size(1974, 48);
      this.MainMenu.TabIndex = 0;
      this.MainMenu.Text = "menuStrip1";
      //
      // fileToolStripMenuItem
      //
      this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.readFamosFileToolStripMenuItem,
            this.readGeneralCsvFileToolStripMenuItem,
            this.readKyowaFormatCsvFileToolStripMenuItem});
      this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
      this.fileToolStripMenuItem.Size = new System.Drawing.Size(105, 44);
      this.fileToolStripMenuItem.Text = "開く(&O)";
      //
      // readFamosFileToolStripMenuItem
      //
      this.readFamosFileToolStripMenuItem.Name = "readFamosFileToolStripMenuItem";
      this.readFamosFileToolStripMenuItem.Size = new System.Drawing.Size(350, 44);
      this.readFamosFileToolStripMenuItem.Text = "Famos ファイル";
      this.readFamosFileToolStripMenuItem.Click += new System.EventHandler(this.readFamosFileToolStripMenuIte_Click);
      //
      // readGeneralCsvFileToolStripMenuItem
      //
      this.readGeneralCsvFileToolStripMenuItem.Name = "readGeneralCsvFileToolStripMenuItem";
      this.readGeneralCsvFileToolStripMenuItem.Size = new System.Drawing.Size(350, 44);
      this.readGeneralCsvFileToolStripMenuItem.Text = "CSV ファイル";
      this.readGeneralCsvFileToolStripMenuItem.Click += new System.EventHandler(this.readGeneralCsvFileToolStripMenuItem_Click);
      //
      // readKyowaFormatCsvFileToolStripMenuItem
      //
      this.readKyowaFormatCsvFileToolStripMenuItem.Name = "readKyowaFormatCsvFileToolStripMenuItem";
      this.readKyowaFormatCsvFileToolStripMenuItem.Size = new System.Drawing.Size(350, 44);
      this.readKyowaFormatCsvFileToolStripMenuItem.Text = "CSV(共和電業形式)";
      this.readKyowaFormatCsvFileToolStripMenuItem.Click += new System.EventHandler(this.readKyowaFormatCsvFileToolStripMenuItem_Click);
      //
      // outputCSVToolStripMenuItem
      //
      this.outputCSVToolStripMenuItem.Name = "outputCSVToolStripMenuItem";
      this.outputCSVToolStripMenuItem.Size = new System.Drawing.Size(206, 44);
      this.outputCSVToolStripMenuItem.Text = "CSVで書き出し(&E)";
      //
      // toolStripMenuItem1
      //
      this.toolStripMenuItem1.Name = "toolStripMenuItem1";
      this.toolStripMenuItem1.Size = new System.Drawing.Size(84, 44);
      this.toolStripMenuItem1.Text = "       ";
      //
      // terminateToolStripMenuItem
      //
      this.terminateToolStripMenuItem.Name = "terminateToolStripMenuItem";
      this.terminateToolStripMenuItem.Size = new System.Drawing.Size(83, 44);
      this.terminateToolStripMenuItem.Text = "終了";
      this.terminateToolStripMenuItem.Click += new System.EventHandler(this.terminateToolStripMenuItem_Click);
      //
      // waveChart
      //
      chartArea3.Name = "ChartArea1";
      this.waveChart.ChartAreas.Add(chartArea3);
      legend3.Name = "Legend1";
      this.waveChart.Legends.Add(legend3);
      this.waveChart.Location = new System.Drawing.Point(0, 800);
      this.waveChart.Margin = new System.Windows.Forms.Padding(7, 6, 7, 6);
      this.waveChart.MinimumSize = new System.Drawing.Size(1500, 500);
      this.waveChart.Name = "waveChart";
      series5.ChartArea = "ChartArea1";
      series5.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.FastLine;
      series5.Legend = "Legend1";
      series5.Name = "元データ";
      series6.ChartArea = "ChartArea1";
      series6.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.FastLine;
      series6.Legend = "Legend1";
      series6.Name = "フィルタ後";
      this.waveChart.Series.Add(series5);
      this.waveChart.Series.Add(series6);
      this.waveChart.Size = new System.Drawing.Size(1770, 500);
      this.waveChart.TabIndex = 1;
      this.waveChart.Text = "chart1";
      //
      // spChart
      //
      chartArea4.AxisX.IntervalAutoMode = System.Windows.Forms.DataVisualization.Charting.IntervalAutoMode.VariableCount;
      chartArea4.AxisX.Title = "周波数(Hz)";
      chartArea4.AxisY.IntervalAutoMode = System.Windows.Forms.DataVisualization.Charting.IntervalAutoMode.VariableCount;
      chartArea4.AxisY.IsLogarithmic = true;
      chartArea4.AxisY.Title = "パワースペクトル";
      chartArea4.Name = "ChartArea1";
      this.spChart.ChartAreas.Add(chartArea4);
      legend4.Docking = System.Windows.Forms.DataVisualization.Charting.Docking.Top;
      legend4.Name = "Legend1";
      this.spChart.Legends.Add(legend4);
      this.spChart.Location = new System.Drawing.Point(950, 50);
      this.spChart.Margin = new System.Windows.Forms.Padding(2);
      this.spChart.MinimumSize = new System.Drawing.Size(500, 740);
      this.spChart.Name = "spChart";
      series7.ChartArea = "ChartArea1";
      series7.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.FastLine;
      series7.Legend = "Legend1";
      series7.Name = "元データ";
      series8.ChartArea = "ChartArea1";
      series8.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.FastLine;
      series8.Legend = "Legend1";
      series8.Name = "フィルタ後";
      this.spChart.Series.Add(series7);
      this.spChart.Series.Add(series8);
      this.spChart.Size = new System.Drawing.Size(820, 740);
      this.spChart.TabIndex = 2;
      this.spChart.Text = "chart1";
      title2.Name = "Title1";
      title2.Text = "パワースペクトル";
      this.spChart.Titles.Add(title2);
      //
      // label1
      //
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(0, 70);
      this.label1.Margin = new System.Windows.Forms.Padding(7, 0, 7, 0);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(183, 24);
      this.label1.TabIndex = 4;
      this.label1.Text = "波形データの情報";
      //
      // statusStrip1
      //
      this.statusStrip1.ImageScalingSize = new System.Drawing.Size(32, 32);
      this.statusStrip1.Location = new System.Drawing.Point(0, 1407);
      this.statusStrip1.Name = "statusStrip1";
      this.statusStrip1.Padding = new System.Windows.Forms.Padding(2, 0, 30, 0);
      this.statusStrip1.Size = new System.Drawing.Size(1974, 22);
      this.statusStrip1.TabIndex = 5;
      this.statusStrip1.Text = "statusStrip1";
      //
      // tapTrackBar
      //
      this.tapTrackBar.AutoSize = false;
      this.tapTrackBar.DataBindings.Add(new System.Windows.Forms.Binding("Value", this.dataBindingSource, "Tap", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
      this.tapTrackBar.DataBindings.Add(new System.Windows.Forms.Binding("Maximum", this.dataBindingSource, "TapMax", true, System.Windows.Forms.DataSourceUpdateMode.OnValidation, "100"));
      this.tapTrackBar.Location = new System.Drawing.Point(429, 36);
      this.tapTrackBar.Margin = new System.Windows.Forms.Padding(7, 6, 7, 6);
      this.tapTrackBar.Maximum = 1000;
      this.tapTrackBar.Minimum = 1;
      this.tapTrackBar.Name = "tapTrackBar";
      this.tapTrackBar.Size = new System.Drawing.Size(225, 38);
      this.tapTrackBar.TabIndex = 8;
      this.tapTrackBar.TickFrequency = 100;
      this.tapTrackBar.Value = 1;
      //
      // dataBindingSource
      //
      this.dataBindingSource.DataSource = typeof(WaveFilterTool.WaveDataAndConfigs);
      //
      // tapNumericUpDown
      //
      this.tapNumericUpDown.DataBindings.Add(new System.Windows.Forms.Binding("Value", this.dataBindingSource, "Tap", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
      this.tapNumericUpDown.DataBindings.Add(new System.Windows.Forms.Binding("Maximum", this.dataBindingSource, "TapMax", true));
      this.tapNumericUpDown.Location = new System.Drawing.Point(299, 36);
      this.tapNumericUpDown.Margin = new System.Windows.Forms.Padding(7, 6, 7, 6);
      this.tapNumericUpDown.Name = "tapNumericUpDown";
      this.tapNumericUpDown.Size = new System.Drawing.Size(117, 31);
      this.tapNumericUpDown.TabIndex = 10;
      this.tapNumericUpDown.ValueChanged += new System.EventHandler(this.tapNumericUpDown_ValueChanged);
      this.tapNumericUpDown.Validating += new System.ComponentModel.CancelEventHandler(this.tapNumericUpDown_Validating);
      //
      // lowerFcNumericUpDown
      //
      this.lowerFcNumericUpDown.DataBindings.Add(new System.Windows.Forms.Binding("Value", this.dataBindingSource, "LowerCutOffFrequencyIndex", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
      this.lowerFcNumericUpDown.DataBindings.Add(new System.Windows.Forms.Binding("Maximum", this.dataBindingSource, "Tap", true));
      this.lowerFcNumericUpDown.Location = new System.Drawing.Point(299, 136);
      this.lowerFcNumericUpDown.Margin = new System.Windows.Forms.Padding(7, 6, 7, 6);
      this.lowerFcNumericUpDown.Name = "lowerFcNumericUpDown";
      this.lowerFcNumericUpDown.Size = new System.Drawing.Size(117, 31);
      this.lowerFcNumericUpDown.TabIndex = 11;
      this.lowerFcNumericUpDown.ValueChanged += new System.EventHandler(this.lowerFcNumericUpDown_ValueChanged);
      //
      // lowerFcTrackBar
      //
      this.lowerFcTrackBar.AutoSize = false;
      this.lowerFcTrackBar.DataBindings.Add(new System.Windows.Forms.Binding("Value", this.dataBindingSource, "LowerCutOffFrequencyIndex", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
      this.lowerFcTrackBar.DataBindings.Add(new System.Windows.Forms.Binding("Maximum", this.dataBindingSource, "Tap", true));
      this.lowerFcTrackBar.Location = new System.Drawing.Point(429, 136);
      this.lowerFcTrackBar.Margin = new System.Windows.Forms.Padding(7, 6, 7, 6);
      this.lowerFcTrackBar.Name = "lowerFcTrackBar";
      this.lowerFcTrackBar.Size = new System.Drawing.Size(225, 38);
      this.lowerFcTrackBar.TabIndex = 12;
      //
      // upperFcNumericUpDown
      //
      this.upperFcNumericUpDown.DataBindings.Add(new System.Windows.Forms.Binding("Value", this.dataBindingSource, "UpperCutOffFrequencyIndex", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
      this.upperFcNumericUpDown.DataBindings.Add(new System.Windows.Forms.Binding("Maximum", this.dataBindingSource, "Tap", true));
      this.upperFcNumericUpDown.Location = new System.Drawing.Point(299, 86);
      this.upperFcNumericUpDown.Margin = new System.Windows.Forms.Padding(7, 6, 7, 6);
      this.upperFcNumericUpDown.Name = "upperFcNumericUpDown";
      this.upperFcNumericUpDown.Size = new System.Drawing.Size(117, 31);
      this.upperFcNumericUpDown.TabIndex = 13;
      this.upperFcNumericUpDown.ValueChanged += new System.EventHandler(this.upperFcNumericUpDown_ValueChanged);
      //
      // upperFcTrackBar
      //
      this.upperFcTrackBar.AutoSize = false;
      this.upperFcTrackBar.DataBindings.Add(new System.Windows.Forms.Binding("Value", this.dataBindingSource, "UpperCutOffFrequencyIndex", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
      this.upperFcTrackBar.DataBindings.Add(new System.Windows.Forms.Binding("Maximum", this.dataBindingSource, "Tap", true));
      this.upperFcTrackBar.Location = new System.Drawing.Point(429, 86);
      this.upperFcTrackBar.Margin = new System.Windows.Forms.Padding(7, 6, 7, 6);
      this.upperFcTrackBar.Name = "upperFcTrackBar";
      this.upperFcTrackBar.Size = new System.Drawing.Size(225, 38);
      this.upperFcTrackBar.TabIndex = 15;
      //
      // groupBox1
      //
      this.groupBox1.Controls.Add(gainLabel);
      this.groupBox1.Controls.Add(this.gainComboBox);
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
      this.groupBox1.Location = new System.Drawing.Point(4, 600);
      this.groupBox1.Margin = new System.Windows.Forms.Padding(7, 6, 7, 6);
      this.groupBox1.Name = "groupBox1";
      this.groupBox1.Padding = new System.Windows.Forms.Padding(7, 6, 7, 6);
      this.groupBox1.Size = new System.Drawing.Size(938, 190);
      this.groupBox1.TabIndex = 16;
      this.groupBox1.TabStop = false;
      this.groupBox1.Text = "フィルタ設定";
      //
      // gainComboBox
      //
      this.gainComboBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.dataBindingSource, "Gain", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
      this.gainComboBox.FormattingEnabled = true;
      this.gainComboBox.Items.AddRange(new object[] {
            "0",
            "-20",
            "-40",
            "-60",
            "-80",
            "-100",
            "-120",
            "-140",
            "-160",
            "-180",
            "-200"});
      this.gainComboBox.Location = new System.Drawing.Point(743, 36);
      this.gainComboBox.Margin = new System.Windows.Forms.Padding(7, 6, 7, 6);
      this.gainComboBox.Name = "gainComboBox";
      this.gainComboBox.Size = new System.Drawing.Size(177, 32);
      this.gainComboBox.TabIndex = 19;
      this.gainComboBox.SelectedIndexChanged += new System.EventHandler(this.gainComboBox_SelectedIndexChanged);
      //
      // lowerFcTextBox
      //
      this.lowerFcTextBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.dataBindingSource, "LowerCutOffFrequency", true));
      this.lowerFcTextBox.Location = new System.Drawing.Point(163, 136);
      this.lowerFcTextBox.Margin = new System.Windows.Forms.Padding(7, 6, 7, 6);
      this.lowerFcTextBox.Name = "lowerFcTextBox";
      this.lowerFcTextBox.ReadOnly = true;
      this.lowerFcTextBox.Size = new System.Drawing.Size(119, 31);
      this.lowerFcTextBox.TabIndex = 17;
      //
      // upperFcTextBox
      //
      this.upperFcTextBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.dataBindingSource, "UpperCutOffFrequency", true));
      this.upperFcTextBox.Location = new System.Drawing.Point(163, 94);
      this.upperFcTextBox.Margin = new System.Windows.Forms.Padding(7, 6, 7, 6);
      this.upperFcTextBox.Name = "upperFcTextBox";
      this.upperFcTextBox.ReadOnly = true;
      this.upperFcTextBox.Size = new System.Drawing.Size(119, 31);
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
      this.wave_info.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.dataBindingSource, "Description", true));
      this.wave_info.Location = new System.Drawing.Point(0, 100);
      this.wave_info.Margin = new System.Windows.Forms.Padding(7, 6, 7, 6);
      this.wave_info.MaximumSize = new System.Drawing.Size(940, 290);
      this.wave_info.MinimumSize = new System.Drawing.Size(940, 290);
      this.wave_info.Multiline = true;
      this.wave_info.Name = "wave_info";
      this.wave_info.ReadOnly = true;
      this.wave_info.ScrollBars = System.Windows.Forms.ScrollBars.Both;
      this.wave_info.Size = new System.Drawing.Size(940, 290);
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
      this.channelListBox.ItemHeight = 24;
      this.channelListBox.Location = new System.Drawing.Point(4, 442);
      this.channelListBox.Margin = new System.Windows.Forms.Padding(7, 6, 7, 6);
      this.channelListBox.Name = "channelListBox";
      this.channelListBox.Size = new System.Drawing.Size(307, 148);
      this.channelListBox.TabIndex = 17;
      this.channelListBox.SelectedIndexChanged += new System.EventHandler(this.currentChannelListBox_SelectedIndexChanged);
      //
      // currentChannelLabel2
      //
      this.currentChannelLabel2.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.dataBindingSource, "CurrentChannel", true));
      this.currentChannelLabel2.Location = new System.Drawing.Point(128, 412);
      this.currentChannelLabel2.Margin = new System.Windows.Forms.Padding(7, 0, 7, 0);
      this.currentChannelLabel2.Name = "currentChannelLabel2";
      this.currentChannelLabel2.Size = new System.Drawing.Size(189, 24);
      this.currentChannelLabel2.TabIndex = 18;
      //
      // numberOfDisplayedDataComboBox
      //
      this.numberOfDisplayedDataComboBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.dataBindingSource, "NumberOfDisplayedData", true));
      this.numberOfDisplayedDataComboBox.FormattingEnabled = true;
      this.numberOfDisplayedDataComboBox.Items.AddRange(new object[] {
            "100",
            "200",
            "500",
            "1000",
            "2000",
            "5000",
            "10000",
            "20000",
            "50000"});
      this.numberOfDisplayedDataComboBox.Location = new System.Drawing.Point(574, 406);
      this.numberOfDisplayedDataComboBox.Margin = new System.Windows.Forms.Padding(7, 6, 7, 6);
      this.numberOfDisplayedDataComboBox.Name = "numberOfDisplayedDataComboBox";
      this.numberOfDisplayedDataComboBox.Size = new System.Drawing.Size(258, 32);
      this.numberOfDisplayedDataComboBox.TabIndex = 19;
      this.numberOfDisplayedDataComboBox.TextChanged += new System.EventHandler(this.numberOfDisplayedDataComboBox_TextChanged);
      //
      // WaveFilterView
      //
      this.AutoScaleDimensions = new System.Drawing.SizeF(13F, 24F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
      this.Controls.Add(numberOfDisplayedDataLabel);
      this.Controls.Add(this.numberOfDisplayedDataComboBox);
      this.Controls.Add(this.currentChannelLabel2);
      this.Controls.Add(this.channelListBox);
      this.Controls.Add(currentChannelLabel);
      this.Controls.Add(this.groupBox1);
      this.Controls.Add(this.statusStrip1);
      this.Controls.Add(this.label1);
      this.Controls.Add(this.wave_info);
      this.Controls.Add(this.spChart);
      this.Controls.Add(this.waveChart);
      this.Controls.Add(this.MainMenu);
      this.MainMenuStrip = this.MainMenu;
      this.Margin = new System.Windows.Forms.Padding(7, 6, 7, 6);
      this.MaximumSize = new System.Drawing.Size(2800, 1800);
      this.MinimumSize = new System.Drawing.Size(1800, 1300);
      this.Name = "WaveFilterView";
      this.Text = "波形データフィルタリングツール";
      this.Resize += new System.EventHandler(this.waveFilterView_Resize);
      this.MainMenu.ResumeLayout(false);
      this.MainMenu.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.waveChart)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.spChart)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.tapTrackBar)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.dataBindingSource)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.tapNumericUpDown)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.lowerFcNumericUpDown)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.lowerFcTrackBar)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.upperFcNumericUpDown)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.upperFcTrackBar)).EndInit();
      this.groupBox1.ResumeLayout(false);
      this.groupBox1.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.channelNamesBindingSource)).EndInit();
      this.ResumeLayout(false);
      this.ClientSize = new System.Drawing.Size(1974, 1429);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.MenuStrip MainMenu;
    private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem readFamosFileToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem readGeneralCsvFileToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem outputCSVToolStripMenuItem;
    private System.Windows.Forms.DataVisualization.Charting.Chart waveChart;
    private System.Windows.Forms.DataVisualization.Charting.Chart spChart;
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
    private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
    private System.Windows.Forms.ToolStripMenuItem terminateToolStripMenuItem;
    private System.Windows.Forms.ComboBox numberOfDisplayedDataComboBox;
    private System.Windows.Forms.ComboBox gainComboBox;
    private System.Windows.Forms.ToolStripMenuItem readKyowaFormatCsvFileToolStripMenuItem;
  }
}

