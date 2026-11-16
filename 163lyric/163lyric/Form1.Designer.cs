namespace _163lyric
{
    partial class Form1
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.MainTabs = new System.Windows.Forms.TabControl();
            this.pageSearch = new System.Windows.Forms.TabPage();
            this.pageScan = new System.Windows.Forms.TabPage();
            this.SrcLable = new System.Windows.Forms.Label();
            this.SrcNetease = new System.Windows.Forms.RadioButton();
            this.SrcKugou = new System.Windows.Forms.RadioButton();
            this.SrcLrclib = new System.Windows.Forms.RadioButton();
            this.KeywordLable = new System.Windows.Forms.Label();
            this.Keywordtb = new System.Windows.Forms.TextBox();
            this.Searchbtn = new System.Windows.Forms.Button();
            this.ResultList = new System.Windows.Forms.ListView();
            this.colTitle = new System.Windows.Forms.ColumnHeader();
            this.colArtist = new System.Windows.Forms.ColumnHeader();
            this.colAlbum = new System.Windows.Forms.ColumnHeader();
            this.colDuration = new System.Windows.Forms.ColumnHeader();
            this.DLbtn = new System.Windows.Forms.Button();
            this.Save2btn = new System.Windows.Forms.Button();
            this.Copy2btn = new System.Windows.Forms.Button();
            this.SearchLyrictb = new System.Windows.Forms.TextBox();
            this.btnAbout = new System.Windows.Forms.Button();
            this.DirLable = new System.Windows.Forms.Label();
            this.Dirtb = new System.Windows.Forms.TextBox();
            this.Browsebtn = new System.Windows.Forms.Button();
            this.PrioLable = new System.Windows.Forms.Label();
            this.PrioNetease = new System.Windows.Forms.CheckBox();
            this.PrioKugou = new System.Windows.Forms.CheckBox();
            this.PrioLrclib = new System.Windows.Forms.CheckBox();
            this.SkipExisting = new System.Windows.Forms.CheckBox();
            this.Scanbtn = new System.Windows.Forms.Button();
            this.Stopbtn = new System.Windows.Forms.Button();
            this.ScanProgress = new System.Windows.Forms.ProgressBar();
            this.ScanLogtb = new System.Windows.Forms.TextBox();
            this.MainTabs.SuspendLayout();
            this.pageSearch.SuspendLayout();
            this.pageScan.SuspendLayout();
            this.SuspendLayout();
            //
            // MainTabs
            //
            this.MainTabs.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.MainTabs.Controls.Add(this.pageSearch);
            this.MainTabs.Controls.Add(this.pageScan);
            this.MainTabs.Location = new System.Drawing.Point(6, 6);
            this.MainTabs.Name = "MainTabs";
            this.MainTabs.SelectedIndex = 0;
            this.MainTabs.Size = new System.Drawing.Size(672, 520);
            this.MainTabs.TabIndex = 0;
            //
            // pageSearch
            //
            this.pageSearch.Controls.Add(this.SrcLable);
            this.pageSearch.Controls.Add(this.SrcNetease);
            this.pageSearch.Controls.Add(this.SrcKugou);
            this.pageSearch.Controls.Add(this.SrcLrclib);
            this.pageSearch.Controls.Add(this.KeywordLable);
            this.pageSearch.Controls.Add(this.Keywordtb);
            this.pageSearch.Controls.Add(this.Searchbtn);
            this.pageSearch.Controls.Add(this.ResultList);
            this.pageSearch.Controls.Add(this.DLbtn);
            this.pageSearch.Controls.Add(this.Save2btn);
            this.pageSearch.Controls.Add(this.Copy2btn);
            this.pageSearch.Controls.Add(this.SearchLyrictb);
            this.pageSearch.Controls.Add(this.btnAbout);
            this.pageSearch.Location = new System.Drawing.Point(4, 22);
            this.pageSearch.Name = "pageSearch";
            this.pageSearch.Padding = new System.Windows.Forms.Padding(3);
            this.pageSearch.Size = new System.Drawing.Size(664, 494);
            this.pageSearch.TabIndex = 0;
            this.pageSearch.Text = "搜索歌词";
            this.pageSearch.UseVisualStyleBackColor = true;
            //
            // SrcLable
            //
            this.SrcLable.AutoSize = true;
            this.SrcLable.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.SrcLable.Location = new System.Drawing.Point(12, 16);
            this.SrcLable.Name = "SrcLable";
            this.SrcLable.Size = new System.Drawing.Size(72, 16);
            this.SrcLable.TabIndex = 0;
            this.SrcLable.Text = "歌词源";
            //
            // SrcNetease
            //
            this.SrcNetease.AutoSize = true;
            this.SrcNetease.Checked = true;
            this.SrcNetease.Location = new System.Drawing.Point(66, 13);
            this.SrcNetease.Name = "SrcNetease";
            this.SrcNetease.Size = new System.Drawing.Size(54, 20);
            this.SrcNetease.TabIndex = 1;
            this.SrcNetease.TabStop = true;
            this.SrcNetease.Text = "网易";
            this.SrcNetease.UseVisualStyleBackColor = true;
            //
            // SrcKugou
            //
            this.SrcKugou.AutoSize = true;
            this.SrcKugou.Location = new System.Drawing.Point(126, 13);
            this.SrcKugou.Name = "SrcKugou";
            this.SrcKugou.Size = new System.Drawing.Size(54, 20);
            this.SrcKugou.TabIndex = 2;
            this.SrcKugou.Text = "酷狗";
            this.SrcKugou.UseVisualStyleBackColor = true;
            //
            // SrcLrclib
            //
            this.SrcLrclib.AutoSize = true;
            this.SrcLrclib.Location = new System.Drawing.Point(186, 13);
            this.SrcLrclib.Name = "SrcLrclib";
            this.SrcLrclib.Size = new System.Drawing.Size(72, 20);
            this.SrcLrclib.TabIndex = 3;
            this.SrcLrclib.Text = "LRCLIB";
            this.SrcLrclib.UseVisualStyleBackColor = true;
            //
            // KeywordLable
            //
            this.KeywordLable.AutoSize = true;
            this.KeywordLable.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.KeywordLable.Location = new System.Drawing.Point(12, 50);
            this.KeywordLable.Name = "KeywordLable";
            this.KeywordLable.Size = new System.Drawing.Size(48, 16);
            this.KeywordLable.TabIndex = 4;
            this.KeywordLable.Text = "关键词";
            //
            // Keywordtb
            //
            this.Keywordtb.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Keywordtb.Location = new System.Drawing.Point(62, 46);
            this.Keywordtb.Name = "Keywordtb";
            this.Keywordtb.Size = new System.Drawing.Size(464, 21);
            this.Keywordtb.TabIndex = 5;
            //
            // Searchbtn
            //
            this.Searchbtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.Searchbtn.Location = new System.Drawing.Point(540, 45);
            this.Searchbtn.Name = "Searchbtn";
            this.Searchbtn.Size = new System.Drawing.Size(80, 23);
            this.Searchbtn.TabIndex = 6;
            this.Searchbtn.Text = "搜索";
            this.Searchbtn.UseVisualStyleBackColor = true;
            this.Searchbtn.Click += new System.EventHandler(this.Searchbtn_Click);
            //
            // ResultList
            //
            this.ResultList.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ResultList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colTitle,
            this.colArtist,
            this.colAlbum,
            this.colDuration});
            this.ResultList.FullRowSelect = true;
            this.ResultList.HideSelection = true;
            this.ResultList.Location = new System.Drawing.Point(12, 78);
            this.ResultList.MultiSelect = false;
            this.ResultList.Name = "ResultList";
            this.ResultList.Size = new System.Drawing.Size(640, 156);
            this.ResultList.TabIndex = 7;
            this.ResultList.UseCompatibleStateImageBehavior = false;
            this.ResultList.View = System.Windows.Forms.View.Details;
            this.ResultList.DoubleClick += new System.EventHandler(this.ResultList_DoubleClick);
            //
            // colTitle
            //
            this.colTitle.Text = "歌曲";
            this.colTitle.Width = 190;
            //
            // colArtist
            //
            this.colArtist.Text = "歌手";
            this.colArtist.Width = 150;
            //
            // colAlbum
            //
            this.colAlbum.Text = "专辑";
            this.colAlbum.Width = 160;
            //
            // colDuration
            //
            this.colDuration.Text = "时长";
            this.colDuration.Width = 56;
            //
            // DLbtn
            //
            this.DLbtn.Location = new System.Drawing.Point(12, 242);
            this.DLbtn.Name = "DLbtn";
            this.DLbtn.Size = new System.Drawing.Size(84, 23);
            this.DLbtn.TabIndex = 8;
            this.DLbtn.Text = "下载歌词";
            this.DLbtn.UseVisualStyleBackColor = true;
            this.DLbtn.Click += new System.EventHandler(this.DLbtn_Click);
            //
            // Save2btn
            //
            this.Save2btn.Location = new System.Drawing.Point(104, 242);
            this.Save2btn.Name = "Save2btn";
            this.Save2btn.Size = new System.Drawing.Size(84, 23);
            this.Save2btn.TabIndex = 9;
            this.Save2btn.Text = "保存LRC";
            this.Save2btn.UseVisualStyleBackColor = true;
            this.Save2btn.Click += new System.EventHandler(this.Save2btn_Click);
            //
            // Copy2btn
            //
            this.Copy2btn.Location = new System.Drawing.Point(196, 242);
            this.Copy2btn.Name = "Copy2btn";
            this.Copy2btn.Size = new System.Drawing.Size(70, 23);
            this.Copy2btn.TabIndex = 10;
            this.Copy2btn.Text = "复制";
            this.Copy2btn.UseVisualStyleBackColor = true;
            this.Copy2btn.Click += new System.EventHandler(this.Copy2btn_Click);
            //
            // SearchLyrictb
            //
            this.SearchLyrictb.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.SearchLyrictb.Location = new System.Drawing.Point(12, 272);
            this.SearchLyrictb.Multiline = true;
            this.SearchLyrictb.Name = "SearchLyrictb";
            this.SearchLyrictb.ReadOnly = true;
            this.SearchLyrictb.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.SearchLyrictb.Size = new System.Drawing.Size(640, 214);
            this.SearchLyrictb.TabIndex = 11;
            //
            // btnAbout
            //
            this.btnAbout.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAbout.Location = new System.Drawing.Point(578, 12);
            this.btnAbout.Name = "btnAbout";
            this.btnAbout.Size = new System.Drawing.Size(74, 23);
            this.btnAbout.TabIndex = 12;
            this.btnAbout.Text = "关于";
            this.btnAbout.UseVisualStyleBackColor = true;
            this.btnAbout.Click += new System.EventHandler(this.btnAbout_Click);
            //
            // pageScan
            //
            this.pageScan.Controls.Add(this.DirLable);
            this.pageScan.Controls.Add(this.Dirtb);
            this.pageScan.Controls.Add(this.Browsebtn);
            this.pageScan.Controls.Add(this.PrioLable);
            this.pageScan.Controls.Add(this.PrioNetease);
            this.pageScan.Controls.Add(this.PrioKugou);
            this.pageScan.Controls.Add(this.PrioLrclib);
            this.pageScan.Controls.Add(this.SkipExisting);
            this.pageScan.Controls.Add(this.Scanbtn);
            this.pageScan.Controls.Add(this.Stopbtn);
            this.pageScan.Controls.Add(this.ScanProgress);
            this.pageScan.Controls.Add(this.ScanLogtb);
            this.pageScan.Location = new System.Drawing.Point(4, 22);
            this.pageScan.Name = "pageScan";
            this.pageScan.Padding = new System.Windows.Forms.Padding(3);
            this.pageScan.Size = new System.Drawing.Size(664, 494);
            this.pageScan.TabIndex = 1;
            this.pageScan.Text = "批量扫描";
            this.pageScan.UseVisualStyleBackColor = true;
            //
            // DirLable
            //
            this.DirLable.AutoSize = true;
            this.DirLable.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.DirLable.Location = new System.Drawing.Point(12, 17);
            this.DirLable.Name = "DirLable";
            this.DirLable.Size = new System.Drawing.Size(36, 16);
            this.DirLable.TabIndex = 0;
            this.DirLable.Text = "目录";
            //
            // Dirtb
            //
            this.Dirtb.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Dirtb.Location = new System.Drawing.Point(50, 14);
            this.Dirtb.Name = "Dirtb";
            this.Dirtb.Size = new System.Drawing.Size(516, 21);
            this.Dirtb.TabIndex = 1;
            //
            // Browsebtn
            //
            this.Browsebtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.Browsebtn.Location = new System.Drawing.Point(574, 13);
            this.Browsebtn.Name = "Browsebtn";
            this.Browsebtn.Size = new System.Drawing.Size(78, 23);
            this.Browsebtn.TabIndex = 2;
            this.Browsebtn.Text = "浏览...";
            this.Browsebtn.UseVisualStyleBackColor = true;
            this.Browsebtn.Click += new System.EventHandler(this.Browsebtn_Click);
            //
            // PrioLable
            //
            this.PrioLable.AutoSize = true;
            this.PrioLable.Location = new System.Drawing.Point(12, 51);
            this.PrioLable.Name = "PrioLable";
            this.PrioLable.Size = new System.Drawing.Size(113, 12);
            this.PrioLable.TabIndex = 3;
            this.PrioLable.Text = "歌词源(按顺序尝试)";
            //
            // PrioNetease
            //
            this.PrioNetease.AutoSize = true;
            this.PrioNetease.Checked = true;
            this.PrioNetease.CheckState = System.Windows.Forms.CheckState.Checked;
            this.PrioNetease.Location = new System.Drawing.Point(130, 48);
            this.PrioNetease.Name = "PrioNetease";
            this.PrioNetease.Size = new System.Drawing.Size(54, 20);
            this.PrioNetease.TabIndex = 4;
            this.PrioNetease.Text = "网易";
            this.PrioNetease.UseVisualStyleBackColor = true;
            //
            // PrioKugou
            //
            this.PrioKugou.AutoSize = true;
            this.PrioKugou.Checked = true;
            this.PrioKugou.CheckState = System.Windows.Forms.CheckState.Checked;
            this.PrioKugou.Location = new System.Drawing.Point(190, 48);
            this.PrioKugou.Name = "PrioKugou";
            this.PrioKugou.Size = new System.Drawing.Size(54, 20);
            this.PrioKugou.TabIndex = 5;
            this.PrioKugou.Text = "酷狗";
            this.PrioKugou.UseVisualStyleBackColor = true;
            //
            // PrioLrclib
            //
            this.PrioLrclib.AutoSize = true;
            this.PrioLrclib.Checked = true;
            this.PrioLrclib.CheckState = System.Windows.Forms.CheckState.Checked;
            this.PrioLrclib.Location = new System.Drawing.Point(250, 48);
            this.PrioLrclib.Name = "PrioLrclib";
            this.PrioLrclib.Size = new System.Drawing.Size(72, 20);
            this.PrioLrclib.TabIndex = 6;
            this.PrioLrclib.Text = "LRCLIB";
            this.PrioLrclib.UseVisualStyleBackColor = true;
            //
            // SkipExisting
            //
            this.SkipExisting.AutoSize = true;
            this.SkipExisting.Checked = true;
            this.SkipExisting.CheckState = System.Windows.Forms.CheckState.Checked;
            this.SkipExisting.Location = new System.Drawing.Point(336, 48);
            this.SkipExisting.Name = "SkipExisting";
            this.SkipExisting.Size = new System.Drawing.Size(150, 20);
            this.SkipExisting.TabIndex = 7;
            this.SkipExisting.Text = "跳过已有歌词文件";
            this.SkipExisting.UseVisualStyleBackColor = true;
            //
            // Scanbtn
            //
            this.Scanbtn.Location = new System.Drawing.Point(12, 84);
            this.Scanbtn.Name = "Scanbtn";
            this.Scanbtn.Size = new System.Drawing.Size(90, 25);
            this.Scanbtn.TabIndex = 8;
            this.Scanbtn.Text = "开始扫描";
            this.Scanbtn.UseVisualStyleBackColor = true;
            this.Scanbtn.Click += new System.EventHandler(this.Scanbtn_Click);
            //
            // Stopbtn
            //
            this.Stopbtn.Enabled = false;
            this.Stopbtn.Location = new System.Drawing.Point(110, 84);
            this.Stopbtn.Name = "Stopbtn";
            this.Stopbtn.Size = new System.Drawing.Size(70, 25);
            this.Stopbtn.TabIndex = 9;
            this.Stopbtn.Text = "停止";
            this.Stopbtn.UseVisualStyleBackColor = true;
            this.Stopbtn.Click += new System.EventHandler(this.Stopbtn_Click);
            //
            // ScanProgress
            //
            this.ScanProgress.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ScanProgress.Location = new System.Drawing.Point(12, 118);
            this.ScanProgress.Name = "ScanProgress";
            this.ScanProgress.Size = new System.Drawing.Size(640, 18);
            this.ScanProgress.TabIndex = 10;
            //
            // ScanLogtb
            //
            this.ScanLogtb.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ScanLogtb.Location = new System.Drawing.Point(12, 146);
            this.ScanLogtb.Multiline = true;
            this.ScanLogtb.Name = "ScanLogtb";
            this.ScanLogtb.ReadOnly = true;
            this.ScanLogtb.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.ScanLogtb.Size = new System.Drawing.Size(640, 340);
            this.ScanLogtb.TabIndex = 11;
            //
            // Form1
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(684, 532);
            this.Controls.Add(this.MainTabs);
            this.MinimumSize = new System.Drawing.Size(560, 420);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "OneLyric";
            this.MainTabs.ResumeLayout(false);
            this.pageSearch.ResumeLayout(false);
            this.pageSearch.PerformLayout();
            this.pageScan.ResumeLayout(false);
            this.pageScan.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl MainTabs;
        private System.Windows.Forms.TabPage pageSearch;
        private System.Windows.Forms.TabPage pageScan;
        private System.Windows.Forms.Label SrcLable;
        private System.Windows.Forms.RadioButton SrcNetease;
        private System.Windows.Forms.RadioButton SrcKugou;
        private System.Windows.Forms.RadioButton SrcLrclib;
        private System.Windows.Forms.Label KeywordLable;
        private System.Windows.Forms.TextBox Keywordtb;
        private System.Windows.Forms.Button Searchbtn;
        private System.Windows.Forms.ListView ResultList;
        private System.Windows.Forms.ColumnHeader colTitle;
        private System.Windows.Forms.ColumnHeader colArtist;
        private System.Windows.Forms.ColumnHeader colAlbum;
        private System.Windows.Forms.ColumnHeader colDuration;
        private System.Windows.Forms.Button DLbtn;
        private System.Windows.Forms.Button Save2btn;
        private System.Windows.Forms.Button Copy2btn;
        private System.Windows.Forms.TextBox SearchLyrictb;
        private System.Windows.Forms.Button btnAbout;
        private System.Windows.Forms.Label DirLable;
        private System.Windows.Forms.TextBox Dirtb;
        private System.Windows.Forms.Button Browsebtn;
        private System.Windows.Forms.Label PrioLable;
        private System.Windows.Forms.CheckBox PrioNetease;
        private System.Windows.Forms.CheckBox PrioKugou;
        private System.Windows.Forms.CheckBox PrioLrclib;
        private System.Windows.Forms.CheckBox SkipExisting;
        private System.Windows.Forms.Button Scanbtn;
        private System.Windows.Forms.Button Stopbtn;
        private System.Windows.Forms.ProgressBar ScanProgress;
        private System.Windows.Forms.TextBox ScanLogtb;
    }
}
