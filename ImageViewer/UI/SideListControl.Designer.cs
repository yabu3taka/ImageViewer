
namespace ImageViewer.UI
{
    partial class SideListControl
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
            SideGroupBox = new System.Windows.Forms.GroupBox();
            CBFilter = new System.Windows.Forms.ComboBox();
            LVFile = new ImageFileListView();
            LBInfoFile = new System.Windows.Forms.Label();
            LBInfoDir = new System.Windows.Forms.LinkLabel();
            CMenuOpenInfo = new System.Windows.Forms.ContextMenuStrip(components);
            TTInfoDir = new System.Windows.Forms.ToolTip(components);
            SideGroupBox.SuspendLayout();
            SuspendLayout();
            // 
            // SideGroupBox
            // 
            SideGroupBox.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            SideGroupBox.Controls.Add(CBFilter);
            SideGroupBox.Controls.Add(LVFile);
            SideGroupBox.Location = new System.Drawing.Point(4, 40);
            SideGroupBox.Name = "SideGroupBox";
            SideGroupBox.Size = new System.Drawing.Size(152, 268);
            SideGroupBox.TabIndex = 2;
            SideGroupBox.TabStop = false;
            SideGroupBox.Text = "種類";
            // 
            // CBFilter
            // 
            CBFilter.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            CBFilter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            CBFilter.FormattingEnabled = true;
            CBFilter.Location = new System.Drawing.Point(6, 20);
            CBFilter.Name = "CBFilter";
            CBFilter.Size = new System.Drawing.Size(140, 23);
            CBFilter.TabIndex = 3;
            CBFilter.SelectionChangeCommitted += CBFilter_SelectionChangeCommitted;
            // 
            // LVFile
            // 
            LVFile.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            LVFile.BgColorFunc = null;
            LVFile.FullRowSelect = true;
            LVFile.Location = new System.Drawing.Point(6, 49);
            LVFile.Name = "LVFile";
            LVFile.Size = new System.Drawing.Size(140, 210);
            LVFile.TabIndex = 4;
            LVFile.UseCompatibleStateImageBehavior = false;
            LVFile.View = System.Windows.Forms.View.SmallIcon;
            LVFile.ItemActivate += LVFile_ItemActivate;
            // 
            // LBInfoFile
            // 
            LBInfoFile.AutoEllipsis = true;
            LBInfoFile.AutoSize = true;
            LBInfoFile.Location = new System.Drawing.Point(4, 22);
            LBInfoFile.Name = "LBInfoFile";
            LBInfoFile.Size = new System.Drawing.Size(41, 15);
            LBInfoFile.TabIndex = 1;
            LBInfoFile.Text = "ファイル";
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
            // SideListControl
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackColor = System.Drawing.SystemColors.Control;
            Controls.Add(LBInfoDir);
            Controls.Add(SideGroupBox);
            Controls.Add(LBInfoFile);
            Name = "SideListControl";
            Size = new System.Drawing.Size(160, 312);
            SideGroupBox.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private System.Windows.Forms.GroupBox SideGroupBox;
        private ImageFileListView LVFile;
        private System.Windows.Forms.ComboBox CBFilter;
        private System.Windows.Forms.Label LBInfoFile;
        private System.Windows.Forms.LinkLabel LBInfoDir;
        private System.Windows.Forms.ToolTip TTInfoDir;
        private System.Windows.Forms.ContextMenuStrip CMenuOpenInfo;
    }
}
