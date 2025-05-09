namespace ImageConverter
{
    partial class Form1
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
            System.Windows.Forms.StatusStrip statusStrip1;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            System.Windows.Forms.GroupBox groupBox1;
            System.Windows.Forms.GroupBox groupBox2;
            System.Windows.Forms.TabPage PageConvert;
            System.Windows.Forms.TabPage PageTool;
            System.Windows.Forms.GroupBox groupBox6;
            SBtnShow = new System.Windows.Forms.ToolStripSplitButton();
            SLbStatus = new System.Windows.Forms.ToolStripStatusLabel();
            TbPass = new System.Windows.Forms.TextBox();
            BtnPass = new System.Windows.Forms.Button();
            BtnSync = new System.Windows.Forms.Button();
            TbSyncDir = new System.Windows.Forms.TextBox();
            ChkMod = new System.Windows.Forms.CheckBox();
            BtnSyncTabletCheck = new System.Windows.Forms.Button();
            label5 = new System.Windows.Forms.Label();
            BtnSyncTabletCopy = new System.Windows.Forms.Button();
            TbFilter = new System.Windows.Forms.TextBox();
            BtnSyncTabletDel = new System.Windows.Forms.Button();
            ChkPreview = new System.Windows.Forms.CheckBox();
            groupBox7 = new System.Windows.Forms.GroupBox();
            ChkNoSmall = new System.Windows.Forms.CheckBox();
            BtnAdd = new System.Windows.Forms.Button();
            ChkBlank = new System.Windows.Forms.CheckBox();
            BtnDelete = new System.Windows.Forms.Button();
            BtnRename = new System.Windows.Forms.Button();
            TbNewName = new System.Windows.Forms.TextBox();
            CmbSort = new System.Windows.Forms.ComboBox();
            BtnRevert = new System.Windows.Forms.Button();
            BtnGSyncLight = new System.Windows.Forms.Button();
            BtnGSync = new System.Windows.Forms.Button();
            BtnAllSelect = new System.Windows.Forms.Button();
            LbFolder = new System.Windows.Forms.ListBox();
            BtnConvert = new System.Windows.Forms.Button();
            BtnSetting = new System.Windows.Forms.Button();
            ChkLog = new System.Windows.Forms.CheckBox();
            TabMain = new System.Windows.Forms.TabControl();
            statusStrip1 = new System.Windows.Forms.StatusStrip();
            groupBox1 = new System.Windows.Forms.GroupBox();
            groupBox2 = new System.Windows.Forms.GroupBox();
            PageConvert = new System.Windows.Forms.TabPage();
            PageTool = new System.Windows.Forms.TabPage();
            groupBox6 = new System.Windows.Forms.GroupBox();
            statusStrip1.SuspendLayout();
            groupBox1.SuspendLayout();
            groupBox2.SuspendLayout();
            PageConvert.SuspendLayout();
            groupBox7.SuspendLayout();
            PageTool.SuspendLayout();
            groupBox6.SuspendLayout();
            TabMain.SuspendLayout();
            SuspendLayout();
            // 
            // statusStrip1
            // 
            statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { SBtnShow, SLbStatus });
            statusStrip1.Location = new System.Drawing.Point(0, 469);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Padding = new System.Windows.Forms.Padding(1, 0, 16, 0);
            statusStrip1.Size = new System.Drawing.Size(384, 22);
            statusStrip1.TabIndex = 23;
            statusStrip1.Text = "statusStrip1";
            // 
            // SBtnShow
            // 
            SBtnShow.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            SBtnShow.Image = (System.Drawing.Image)resources.GetObject("SBtnShow.Image");
            SBtnShow.ImageTransparentColor = System.Drawing.Color.Magenta;
            SBtnShow.Name = "SBtnShow";
            SBtnShow.Size = new System.Drawing.Size(47, 20);
            SBtnShow.Text = "起動";
            SBtnShow.ButtonClick += SBtnShow_ButtonClick;
            SBtnShow.DropDownOpening += SBtnShow_DropDownOpening;
            // 
            // SLbStatus
            // 
            SLbStatus.Name = "SLbStatus";
            SLbStatus.Size = new System.Drawing.Size(19, 17);
            SLbStatus.Text = "    ";
            // 
            // groupBox1
            // 
            groupBox1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            groupBox1.Controls.Add(TbPass);
            groupBox1.Controls.Add(BtnPass);
            groupBox1.Location = new System.Drawing.Point(8, 62);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new System.Drawing.Size(360, 52);
            groupBox1.TabIndex = 26;
            groupBox1.TabStop = false;
            groupBox1.Text = "パスワードファイル作成";
            // 
            // TbPass
            // 
            TbPass.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            TbPass.Location = new System.Drawing.Point(5, 19);
            TbPass.Name = "TbPass";
            TbPass.Size = new System.Drawing.Size(284, 23);
            TbPass.TabIndex = 43;
            // 
            // BtnPass
            // 
            BtnPass.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            BtnPass.Location = new System.Drawing.Point(295, 19);
            BtnPass.Name = "BtnPass";
            BtnPass.Size = new System.Drawing.Size(60, 23);
            BtnPass.TabIndex = 44;
            BtnPass.Text = "作成";
            BtnPass.UseVisualStyleBackColor = true;
            BtnPass.Click += BtnPass_Click;
            // 
            // groupBox2
            // 
            groupBox2.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            groupBox2.Controls.Add(BtnSync);
            groupBox2.Controls.Add(TbSyncDir);
            groupBox2.Location = new System.Drawing.Point(8, 3);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new System.Drawing.Size(360, 53);
            groupBox2.TabIndex = 27;
            groupBox2.TabStop = false;
            groupBox2.Text = "同期";
            // 
            // BtnSync
            // 
            BtnSync.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            BtnSync.Location = new System.Drawing.Point(295, 19);
            BtnSync.Name = "BtnSync";
            BtnSync.Size = new System.Drawing.Size(59, 23);
            BtnSync.TabIndex = 42;
            BtnSync.Text = "同期";
            BtnSync.UseVisualStyleBackColor = true;
            BtnSync.Click += BtnSync_Click;
            // 
            // TbSyncDir
            // 
            TbSyncDir.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            TbSyncDir.Location = new System.Drawing.Point(5, 19);
            TbSyncDir.Name = "TbSyncDir";
            TbSyncDir.Size = new System.Drawing.Size(283, 23);
            TbSyncDir.TabIndex = 41;
            // 
            // PageConvert
            // 
            PageConvert.Controls.Add(ChkMod);
            PageConvert.Controls.Add(BtnSyncTabletCheck);
            PageConvert.Controls.Add(label5);
            PageConvert.Controls.Add(BtnSyncTabletCopy);
            PageConvert.Controls.Add(TbFilter);
            PageConvert.Controls.Add(BtnSyncTabletDel);
            PageConvert.Controls.Add(ChkPreview);
            PageConvert.Controls.Add(groupBox7);
            PageConvert.Controls.Add(CmbSort);
            PageConvert.Controls.Add(BtnRevert);
            PageConvert.Controls.Add(BtnGSyncLight);
            PageConvert.Controls.Add(BtnGSync);
            PageConvert.Controls.Add(BtnAllSelect);
            PageConvert.Controls.Add(LbFolder);
            PageConvert.Controls.Add(BtnConvert);
            PageConvert.Location = new System.Drawing.Point(4, 24);
            PageConvert.Name = "PageConvert";
            PageConvert.Size = new System.Drawing.Size(376, 441);
            PageConvert.TabIndex = 1;
            PageConvert.Text = "変換";
            PageConvert.UseVisualStyleBackColor = true;
            // 
            // CBFile
            // 
            ChkMod.AutoSize = true;
            ChkMod.Location = new System.Drawing.Point(226, 14);
            ChkMod.Name = "CBFile";
            ChkMod.Size = new System.Drawing.Size(50, 19);
            ChkMod.TabIndex = 37;
            ChkMod.Text = "確認";
            ChkMod.UseVisualStyleBackColor = true;
            ChkMod.CheckedChanged += ChkMod_CheckedChanged;
            // 
            // BtnSyncTabletCheck
            // 
            BtnSyncTabletCheck.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            BtnSyncTabletCheck.Location = new System.Drawing.Point(288, 147);
            BtnSyncTabletCheck.Name = "BtnSyncTabletCheck";
            BtnSyncTabletCheck.Size = new System.Drawing.Size(79, 23);
            BtnSyncTabletCheck.TabIndex = 36;
            BtnSyncTabletCheck.Text = "確認";
            BtnSyncTabletCheck.UseVisualStyleBackColor = true;
            BtnSyncTabletCheck.Click += BtnSyncTabletCheck_Click;
            // 
            // label5
            // 
            label5.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            label5.AutoSize = true;
            label5.Location = new System.Drawing.Point(288, 125);
            label5.Name = "label5";
            label5.Size = new System.Drawing.Size(31, 15);
            label5.TabIndex = 35;
            label5.Text = "端末";
            // 
            // BtnSyncTabletCopy
            // 
            BtnSyncTabletCopy.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            BtnSyncTabletCopy.Location = new System.Drawing.Point(289, 205);
            BtnSyncTabletCopy.Name = "BtnSyncTabletCopy";
            BtnSyncTabletCopy.Size = new System.Drawing.Size(80, 23);
            BtnSyncTabletCopy.TabIndex = 34;
            BtnSyncTabletCopy.Text = "コピー";
            BtnSyncTabletCopy.UseVisualStyleBackColor = true;
            BtnSyncTabletCopy.Click += BtnSyncTabletCopy_Click;
            // 
            // TbFilter
            // 
            TbFilter.Location = new System.Drawing.Point(8, 42);
            TbFilter.Name = "TbFilter";
            TbFilter.Size = new System.Drawing.Size(274, 23);
            TbFilter.TabIndex = 33;
            TbFilter.KeyDown += TbFilter_KeyDown;
            // 
            // BtnSyncTabletDel
            // 
            BtnSyncTabletDel.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            BtnSyncTabletDel.Location = new System.Drawing.Point(288, 176);
            BtnSyncTabletDel.Name = "BtnSyncTabletDel";
            BtnSyncTabletDel.Size = new System.Drawing.Size(80, 23);
            BtnSyncTabletDel.TabIndex = 32;
            BtnSyncTabletDel.Text = "削除";
            BtnSyncTabletDel.UseVisualStyleBackColor = true;
            BtnSyncTabletDel.Click += BtnSyncTabletDel_Click;
            // 
            // ChkPreview
            // 
            ChkPreview.AutoSize = true;
            ChkPreview.Checked = true;
            ChkPreview.CheckState = System.Windows.Forms.CheckState.Checked;
            ChkPreview.Location = new System.Drawing.Point(154, 14);
            ChkPreview.Name = "ChkPreview";
            ChkPreview.Size = new System.Drawing.Size(67, 19);
            ChkPreview.TabIndex = 22;
            ChkPreview.Text = "プレビュー";
            ChkPreview.UseVisualStyleBackColor = true;
            ChkPreview.CheckedChanged += ChkPreview_CheckedChanged;
            // 
            // groupBox7
            // 
            groupBox7.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            groupBox7.Controls.Add(ChkNoSmall);
            groupBox7.Controls.Add(BtnAdd);
            groupBox7.Controls.Add(ChkBlank);
            groupBox7.Controls.Add(BtnDelete);
            groupBox7.Controls.Add(BtnRename);
            groupBox7.Controls.Add(TbNewName);
            groupBox7.Location = new System.Drawing.Point(8, 337);
            groupBox7.Name = "groupBox7";
            groupBox7.Size = new System.Drawing.Size(360, 101);
            groupBox7.TabIndex = 31;
            groupBox7.TabStop = false;
            groupBox7.Text = "操作";
            // 
            // ChkNoSmall
            // 
            ChkNoSmall.AutoSize = true;
            ChkNoSmall.Location = new System.Drawing.Point(6, 76);
            ChkNoSmall.Name = "ChkNoSmall";
            ChkNoSmall.Size = new System.Drawing.Size(80, 19);
            ChkNoSmall.TabIndex = 35;
            ChkNoSmall.Text = "縮小版なし";
            ChkNoSmall.UseVisualStyleBackColor = true;
            ChkNoSmall.CheckedChanged += ChkNoSmall_CheckedChanged;
            // 
            // BtnAdd
            // 
            BtnAdd.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            BtnAdd.Location = new System.Drawing.Point(274, 21);
            BtnAdd.Name = "BtnAdd";
            BtnAdd.Size = new System.Drawing.Size(80, 23);
            BtnAdd.TabIndex = 32;
            BtnAdd.Text = "追加";
            BtnAdd.UseVisualStyleBackColor = true;
            BtnAdd.Click += BtnAdd_Click;
            // 
            // ChkBlank
            // 
            ChkBlank.AutoSize = true;
            ChkBlank.Location = new System.Drawing.Point(6, 54);
            ChkBlank.Name = "ChkBlank";
            ChkBlank.Size = new System.Drawing.Size(124, 19);
            ChkBlank.TabIndex = 33;
            ChkBlank.Text = "途中に空白を入れる";
            ChkBlank.UseVisualStyleBackColor = true;
            ChkBlank.CheckedChanged += ChkBlank_CheckedChanged;
            // 
            // BtnDelete
            // 
            BtnDelete.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            BtnDelete.Location = new System.Drawing.Point(274, 51);
            BtnDelete.Name = "BtnDelete";
            BtnDelete.Size = new System.Drawing.Size(80, 23);
            BtnDelete.TabIndex = 34;
            BtnDelete.Text = "削除";
            BtnDelete.UseVisualStyleBackColor = true;
            BtnDelete.Click += BtnDelete_Click;
            // 
            // BtnRename
            // 
            BtnRename.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            BtnRename.Location = new System.Drawing.Point(188, 21);
            BtnRename.Name = "BtnRename";
            BtnRename.Size = new System.Drawing.Size(80, 23);
            BtnRename.TabIndex = 31;
            BtnRename.Text = "名前変更";
            BtnRename.UseVisualStyleBackColor = true;
            BtnRename.Click += BtnRename_Click;
            // 
            // TbNewName
            // 
            TbNewName.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            TbNewName.Location = new System.Drawing.Point(6, 22);
            TbNewName.Name = "TbNewName";
            TbNewName.Size = new System.Drawing.Size(176, 23);
            TbNewName.TabIndex = 30;
            // 
            // CmbSort
            // 
            CmbSort.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            CmbSort.FormattingEnabled = true;
            CmbSort.Items.AddRange(new object[] { "名称順", "更新日順", "要更新" });
            CmbSort.Location = new System.Drawing.Point(8, 12);
            CmbSort.Name = "CmbSort";
            CmbSort.Size = new System.Drawing.Size(140, 23);
            CmbSort.TabIndex = 21;
            CmbSort.SelectedIndexChanged += CBSort_SelectedIndexChanged;
            // 
            // BtnRevert
            // 
            BtnRevert.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            BtnRevert.Location = new System.Drawing.Point(288, 304);
            BtnRevert.Name = "BtnRevert";
            BtnRevert.Size = new System.Drawing.Size(80, 23);
            BtnRevert.TabIndex = 28;
            BtnRevert.Text = "画像戻す";
            BtnRevert.UseVisualStyleBackColor = true;
            BtnRevert.Click += BtnRevert_Click;
            // 
            // BtnGSyncLight
            // 
            BtnGSyncLight.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            BtnGSyncLight.Location = new System.Drawing.Point(288, 99);
            BtnGSyncLight.Name = "BtnGSyncLight";
            BtnGSyncLight.Size = new System.Drawing.Size(80, 23);
            BtnGSyncLight.TabIndex = 27;
            BtnGSyncLight.Text = "GSyncL";
            BtnGSyncLight.UseVisualStyleBackColor = true;
            BtnGSyncLight.Click += GSyncLight_Click;
            // 
            // BtnGSync
            // 
            BtnGSync.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            BtnGSync.Location = new System.Drawing.Point(288, 70);
            BtnGSync.Name = "BtnGSync";
            BtnGSync.Size = new System.Drawing.Size(80, 23);
            BtnGSync.TabIndex = 26;
            BtnGSync.Text = "GSync";
            BtnGSync.UseVisualStyleBackColor = true;
            BtnGSync.Click += BtnGSync_Click;
            // 
            // BtnAllSelect
            // 
            BtnAllSelect.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            BtnAllSelect.Location = new System.Drawing.Point(288, 12);
            BtnAllSelect.Name = "BtnAllSelect";
            BtnAllSelect.Size = new System.Drawing.Size(80, 23);
            BtnAllSelect.TabIndex = 24;
            BtnAllSelect.Text = "全選択";
            BtnAllSelect.UseVisualStyleBackColor = true;
            BtnAllSelect.Click += BtnAllSelect_Click;
            // 
            // LbFolder
            // 
            LbFolder.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            LbFolder.FormattingEnabled = true;
            LbFolder.ItemHeight = 15;
            LbFolder.Location = new System.Drawing.Point(8, 72);
            LbFolder.Name = "LbFolder";
            LbFolder.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            LbFolder.Size = new System.Drawing.Size(274, 259);
            LbFolder.TabIndex = 23;
            LbFolder.SelectedValueChanged += LbFolder_SelectedValueChanged;
            LbFolder.DoubleClick += LbFolder_DoubleClick;
            // 
            // BtnConvert
            // 
            BtnConvert.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            BtnConvert.Location = new System.Drawing.Point(288, 41);
            BtnConvert.Name = "BtnConvert";
            BtnConvert.Size = new System.Drawing.Size(80, 23);
            BtnConvert.TabIndex = 25;
            BtnConvert.Text = "変換";
            BtnConvert.UseVisualStyleBackColor = true;
            BtnConvert.Click += BtnConvert_Click;
            // 
            // PageTool
            // 
            PageTool.Controls.Add(groupBox6);
            PageTool.Controls.Add(groupBox1);
            PageTool.Controls.Add(groupBox2);
            PageTool.Location = new System.Drawing.Point(4, 24);
            PageTool.Name = "PageTool";
            PageTool.Size = new System.Drawing.Size(376, 441);
            PageTool.TabIndex = 2;
            PageTool.Text = "ツール";
            PageTool.UseVisualStyleBackColor = true;
            // 
            // groupBox6
            // 
            groupBox6.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            groupBox6.Controls.Add(BtnSetting);
            groupBox6.Controls.Add(ChkLog);
            groupBox6.Location = new System.Drawing.Point(8, 120);
            groupBox6.Name = "groupBox6";
            groupBox6.Size = new System.Drawing.Size(360, 88);
            groupBox6.TabIndex = 28;
            groupBox6.TabStop = false;
            groupBox6.Text = "設定";
            // 
            // BtnSetting
            // 
            BtnSetting.Location = new System.Drawing.Point(6, 47);
            BtnSetting.Name = "BtnSetting";
            BtnSetting.Size = new System.Drawing.Size(59, 23);
            BtnSetting.TabIndex = 46;
            BtnSetting.Text = "設定";
            BtnSetting.UseVisualStyleBackColor = true;
            BtnSetting.Click += BtnSetting_Click;
            // 
            // ChkLog
            // 
            ChkLog.AutoSize = true;
            ChkLog.Location = new System.Drawing.Point(6, 22);
            ChkLog.Name = "ChkLog";
            ChkLog.Size = new System.Drawing.Size(102, 19);
            ChkLog.TabIndex = 45;
            ChkLog.Text = "詳細なログ出力";
            ChkLog.UseVisualStyleBackColor = true;
            ChkLog.CheckedChanged += ChkLog_CheckedChanged;
            // 
            // TabMain
            // 
            TabMain.Controls.Add(PageConvert);
            TabMain.Controls.Add(PageTool);
            TabMain.Dock = System.Windows.Forms.DockStyle.Fill;
            TabMain.Location = new System.Drawing.Point(0, 0);
            TabMain.Name = "TabMain";
            TabMain.SelectedIndex = 0;
            TabMain.Size = new System.Drawing.Size(384, 469);
            TabMain.TabIndex = 50;
            TabMain.Selecting += TabMain_Selecting;
            TabMain.Selected += TabMain_Selected;
            TabMain.Deselecting += TabMain_Deselecting;
            // 
            // Form1
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(384, 491);
            Controls.Add(TabMain);
            Controls.Add(statusStrip1);
            Margin = new System.Windows.Forms.Padding(4);
            MinimumSize = new System.Drawing.Size(400, 530);
            Name = "Form1";
            Text = "ImageConverter";
            FormClosing += Form1_FormClosing;
            Load += Form1_Load;
            Shown += Form1_Shown;
            ResizeEnd += Form1_ResizeEnd;
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            groupBox2.ResumeLayout(false);
            groupBox2.PerformLayout();
            PageConvert.ResumeLayout(false);
            PageConvert.PerformLayout();
            groupBox7.ResumeLayout(false);
            groupBox7.PerformLayout();
            PageTool.ResumeLayout(false);
            groupBox6.ResumeLayout(false);
            groupBox6.PerformLayout();
            TabMain.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private System.Windows.Forms.TextBox TbPass;
        private System.Windows.Forms.Button BtnPass;
        private System.Windows.Forms.Button BtnRevert;
        private System.Windows.Forms.Button BtnConvert;
        private System.Windows.Forms.ToolStripStatusLabel SLbStatus;
        private System.Windows.Forms.Button BtnSync;
        private System.Windows.Forms.TextBox TbSyncDir;
        private System.Windows.Forms.ListBox LbFolder;
        private System.Windows.Forms.Button BtnAllSelect;
        private System.Windows.Forms.TabControl TabMain;
        private System.Windows.Forms.Button BtnGSync;
        private System.Windows.Forms.Button BtnGSyncLight;
        private System.Windows.Forms.CheckBox ChkLog;
        private System.Windows.Forms.ComboBox CmbSort;
        private System.Windows.Forms.Button BtnRename;
        private System.Windows.Forms.TextBox TbNewName;
        private System.Windows.Forms.GroupBox groupBox7;
        private System.Windows.Forms.Button BtnDelete;
        private System.Windows.Forms.ToolStripSplitButton SBtnShow;
        private System.Windows.Forms.CheckBox ChkBlank;
        private System.Windows.Forms.CheckBox ChkPreview;
        private System.Windows.Forms.Button BtnAdd;
        private System.Windows.Forms.Button BtnSyncTabletDel;
        private System.Windows.Forms.TextBox TbFilter;
        private System.Windows.Forms.Button BtnSyncTabletCopy;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button BtnSyncTabletCheck;
        private System.Windows.Forms.CheckBox ChkNoSmall;
        private System.Windows.Forms.Button BtnSetting;
        private System.Windows.Forms.CheckBox ChkMod;
    }
}

