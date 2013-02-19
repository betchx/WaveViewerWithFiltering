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
            this.wave_info = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.MainMenu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.wave_chart)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.sp_chart)).BeginInit();
            this.SuspendLayout();
            // 
            // MainMenu
            // 
            this.MainMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ファイルToolStripMenuItem,
            this.書き出しToolStripMenuItem});
            this.MainMenu.Location = new System.Drawing.Point(0, 0);
            this.MainMenu.Name = "MainMenu";
            this.MainMenu.Size = new System.Drawing.Size(870, 24);
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
            // 
            // 一般ToolStripMenuItem
            // 
            this.一般ToolStripMenuItem.Name = "一般ToolStripMenuItem";
            this.一般ToolStripMenuItem.Size = new System.Drawing.Size(142, 22);
            this.一般ToolStripMenuItem.Text = "一般";
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
            this.statusStrip1.Location = new System.Drawing.Point(0, 658);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(870, 22);
            this.statusStrip1.TabIndex = 5;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // WaveFilterView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(870, 680);
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
        private System.Windows.Forms.TextBox wave_info;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.StatusStrip statusStrip1;
    }
}

