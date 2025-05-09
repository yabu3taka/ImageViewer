
namespace ImageViewer.UI
{
    partial class FileAddControl
    {
        /// <summary> 
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージド リソースを破棄する場合は true を指定し、その他の場合は false を指定します。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region コンポーネント デザイナーで生成されたコード

        /// <summary> 
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を 
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            GBFile = new System.Windows.Forms.GroupBox();
            LVFile = new ImageFileListView();
            CBFolder = new System.Windows.Forms.ComboBox();
            TBDel = new System.Windows.Forms.Button();
            BTAdd = new System.Windows.Forms.Button();
            BTCommit = new System.Windows.Forms.Button();
            LBInfoDir = new System.Windows.Forms.LinkLabel();
            CMenuOpenInfo = new System.Windows.Forms.ContextMenuStrip(components);
            TTInfoDir = new System.Windows.Forms.ToolTip(components);
            BTMerge = new SplitButton();
            CMenuConv = new System.Windows.Forms.ContextMenuStrip(components);
            MenuConvCmd = new System.Windows.Forms.ToolStripMenuItem();
            MenuConvExplorer = new System.Windows.Forms.ToolStripMenuItem();
            LBInfoFile = new System.Windows.Forms.Label();
            MenuConvCmdWidth = new System.Windows.Forms.ToolStripMenuItem();
            GBFile.SuspendLayout();
            CMenuConv.SuspendLayout();
            SuspendLayout();
            // 
            // GBFile
            // 
            GBFile.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            GBFile.Controls.Add(LVFile);
            GBFile.Controls.Add(CBFolder);
            GBFile.Controls.Add(TBDel);
            GBFile.Controls.Add(BTAdd);
            GBFile.Location = new System.Drawing.Point(4, 40);
            GBFile.Name = "GBFile";
            GBFile.Size = new System.Drawing.Size(152, 226);
            GBFile.TabIndex = 10;
            GBFile.TabStop = false;
            GBFile.Text = "追加";
            // 
            // LVFile
            // 
            LVFile.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            LVFile.BgColorFunc = null;
            LVFile.Location = new System.Drawing.Point(6, 50);
            LVFile.Name = "LVFile";
            LVFile.Size = new System.Drawing.Size(140, 170);
            LVFile.TabIndex = 14;
            LVFile.UseCompatibleStateImageBehavior = false;
            LVFile.View = System.Windows.Forms.View.SmallIcon;
            // 
            // CBFolder
            // 
            CBFolder.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            CBFolder.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            CBFolder.FormattingEnabled = true;
            CBFolder.Location = new System.Drawing.Point(6, 22);
            CBFolder.Name = "CBFolder";
            CBFolder.Size = new System.Drawing.Size(81, 23);
            CBFolder.TabIndex = 11;
            CBFolder.SelectedIndexChanged += CBFolder_SelectedIndexChanged;
            CBFolder.KeyDown += CBFolder_KeyDown;
            // 
            // TBDel
            // 
            TBDel.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            TBDel.Location = new System.Drawing.Point(122, 21);
            TBDel.Name = "TBDel";
            TBDel.Size = new System.Drawing.Size(23, 23);
            TBDel.TabIndex = 13;
            TBDel.Text = "-";
            TBDel.UseVisualStyleBackColor = true;
            TBDel.Click += TBDel_Click;
            // 
            // BTAdd
            // 
            BTAdd.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            BTAdd.Location = new System.Drawing.Point(93, 21);
            BTAdd.Name = "BTAdd";
            BTAdd.Size = new System.Drawing.Size(23, 23);
            BTAdd.TabIndex = 12;
            BTAdd.Text = "+";
            BTAdd.UseVisualStyleBackColor = true;
            BTAdd.Click += BTAdd_Click;
            // 
            // BTCommit
            // 
            BTCommit.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            BTCommit.Location = new System.Drawing.Point(86, 275);
            BTCommit.Name = "BTCommit";
            BTCommit.Size = new System.Drawing.Size(70, 29);
            BTCommit.TabIndex = 2;
            BTCommit.Text = "コミット";
            BTCommit.UseVisualStyleBackColor = true;
            BTCommit.Click += BTCommit_Click;
            // 
            // LBInfoDir
            // 
            LBInfoDir.AutoSize = true;
            LBInfoDir.ContextMenuStrip = CMenuOpenInfo;
            LBInfoDir.Location = new System.Drawing.Point(4, 4);
            LBInfoDir.Name = "LBInfoDir";
            LBInfoDir.Size = new System.Drawing.Size(50, 15);
            LBInfoDir.TabIndex = 1;
            LBInfoDir.TabStop = true;
            LBInfoDir.Text = "フォルダー";
            LBInfoDir.LinkClicked += LBInfoDir_LinkClicked;
            // 
            // CMenuOpenInfo
            // 
            CMenuOpenInfo.Name = "CMenuOpenInfo";
            CMenuOpenInfo.Size = new System.Drawing.Size(61, 4);
            // 
            // BTMerge
            // 
            BTMerge.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            BTMerge.Location = new System.Drawing.Point(4, 275);
            BTMerge.Name = "BTMerge";
            BTMerge.Size = new System.Drawing.Size(70, 29);
            BTMerge.SubMenu = CMenuConv;
            BTMerge.TabIndex = 21;
            BTMerge.Text = "マージ";
            BTMerge.UseVisualStyleBackColor = true;
            BTMerge.Click += BTMerge_Click;
            // 
            // CMenuConv
            // 
            CMenuConv.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { MenuConvCmd, MenuConvCmdWidth, MenuConvExplorer });
            CMenuConv.Name = "CMenuConv";
            CMenuConv.Size = new System.Drawing.Size(181, 92);
            // 
            // MenuConvCmd
            // 
            MenuConvCmd.Name = "MenuConvCmd";
            MenuConvCmd.Size = new System.Drawing.Size(180, 22);
            MenuConvCmd.Text = "変換コマンド";
            MenuConvCmd.Click += MenuConvCmd_Click;
            // 
            // MenuConvExplorer
            // 
            MenuConvExplorer.Name = "MenuConvExplorer";
            MenuConvExplorer.Size = new System.Drawing.Size(180, 22);
            MenuConvExplorer.Text = "エクスプローラで開く";
            MenuConvExplorer.Click += MenuConvExplorer_Click;
            // 
            // LBInfoFile
            // 
            LBInfoFile.AutoEllipsis = true;
            LBInfoFile.AutoSize = true;
            LBInfoFile.Location = new System.Drawing.Point(4, 22);
            LBInfoFile.Name = "LBInfoFile";
            LBInfoFile.Size = new System.Drawing.Size(41, 15);
            LBInfoFile.TabIndex = 9;
            LBInfoFile.Text = "ファイル";
            // 
            // MenuConvCmdWidth
            // 
            MenuConvCmdWidth.Name = "MenuConvCmdWidth";
            MenuConvCmdWidth.Size = new System.Drawing.Size(180, 22);
            MenuConvCmdWidth.Text = "変換コマンド(横長)";
            MenuConvCmdWidth.Click += MenuConvCmdWidth_Click;
            // 
            // FileAddControl
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            Controls.Add(LBInfoFile);
            Controls.Add(BTMerge);
            Controls.Add(LBInfoDir);
            Controls.Add(BTCommit);
            Controls.Add(GBFile);
            Name = "FileAddControl";
            Size = new System.Drawing.Size(160, 312);
            GBFile.ResumeLayout(false);
            CMenuConv.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private System.Windows.Forms.Button BTCommit;
        private System.Windows.Forms.ComboBox CBFolder;
        private System.Windows.Forms.Button TBDel;
        private System.Windows.Forms.Button BTAdd;
        private ImageFileListView LVFile;
        private System.Windows.Forms.GroupBox GBFile;
        private System.Windows.Forms.LinkLabel LBInfoDir;
        private System.Windows.Forms.ContextMenuStrip CMenuOpenInfo;
        private System.Windows.Forms.ToolTip TTInfoDir;
        private SplitButton BTMerge;
        private System.Windows.Forms.ContextMenuStrip CMenuConv;
        private System.Windows.Forms.ToolStripMenuItem MenuConvCmd;
        private System.Windows.Forms.ToolStripMenuItem MenuConvExplorer;
        private System.Windows.Forms.Label LBInfoFile;
        private System.Windows.Forms.ToolStripMenuItem MenuConvCmdWidth;
    }
}
