
namespace ImageViewer.UI
{
    partial class FileEditControl
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
            System.Windows.Forms.GroupBox groupBox2;
            LVFile = new ImageFileListView();
            BTCommit = new System.Windows.Forms.Button();
            CMenuConv = new System.Windows.Forms.ContextMenuStrip(components);
            MenuConvCmd = new System.Windows.Forms.ToolStripMenuItem();
            MenuConvExplorer = new System.Windows.Forms.ToolStripMenuItem();
            BTConv = new SplitButton();
            groupBox2 = new System.Windows.Forms.GroupBox();
            groupBox2.SuspendLayout();
            CMenuConv.SuspendLayout();
            SuspendLayout();
            // 
            // groupBox2
            // 
            groupBox2.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            groupBox2.Controls.Add(LVFile);
            groupBox2.Location = new System.Drawing.Point(4, 3);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new System.Drawing.Size(152, 265);
            groupBox2.TabIndex = 10;
            groupBox2.TabStop = false;
            groupBox2.Text = "変更対象";
            // 
            // LVFile
            // 
            LVFile.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            LVFile.BgColorFunc = null;
            LVFile.FullRowSelect = true;
            LVFile.Location = new System.Drawing.Point(6, 23);
            LVFile.Name = "LVFile";
            LVFile.Size = new System.Drawing.Size(140, 234);
            LVFile.TabIndex = 11;
            LVFile.UseCompatibleStateImageBehavior = false;
            LVFile.View = System.Windows.Forms.View.SmallIcon;
            // 
            // BTCommit
            // 
            BTCommit.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            BTCommit.Location = new System.Drawing.Point(86, 275);
            BTCommit.Name = "BTCommit";
            BTCommit.Size = new System.Drawing.Size(70, 29);
            BTCommit.TabIndex = 22;
            BTCommit.Text = "コミット";
            BTCommit.UseVisualStyleBackColor = true;
            BTCommit.Click += BTCommit_Click;
            // 
            // CMenuConv
            // 
            CMenuConv.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { MenuConvCmd, MenuConvExplorer });
            CMenuConv.Name = "CMenuConv";
            CMenuConv.Size = new System.Drawing.Size(181, 70);
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
            // BTConv
            // 
            BTConv.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            BTConv.Location = new System.Drawing.Point(4, 275);
            BTConv.Name = "BTConv";
            BTConv.Size = new System.Drawing.Size(70, 29);
            BTConv.SubMenu = CMenuConv;
            BTConv.TabIndex = 21;
            BTConv.Text = "変換";
            BTConv.UseVisualStyleBackColor = true;
            BTConv.Click += BTConv_Click;
            // 
            // FileEditControl
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            Controls.Add(BTConv);
            Controls.Add(BTCommit);
            Controls.Add(groupBox2);
            Margin = new System.Windows.Forms.Padding(4);
            Name = "FileEditControl";
            Size = new System.Drawing.Size(160, 312);
            groupBox2.ResumeLayout(false);
            CMenuConv.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion
        private ImageFileListView LVFile;
        private System.Windows.Forms.Button BTCommit;
        private SplitButton BTConv;
        private System.Windows.Forms.ContextMenuStrip CMenuConv;
        private System.Windows.Forms.ToolStripMenuItem MenuConvCmd;
        private System.Windows.Forms.ToolStripMenuItem MenuConvExplorer;
    }
}
