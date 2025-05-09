namespace ImageViewer
{
    partial class SerialForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.Label label4;
            System.Windows.Forms.StatusStrip statusStrip1;
            System.Windows.Forms.Label label2;
            System.Windows.Forms.Label label3;
            System.Windows.Forms.Label label6;
            System.Windows.Forms.Label label1;
            LBStatus = new System.Windows.Forms.ToolStripStatusLabel();
            MainPanel = new System.Windows.Forms.TableLayoutPanel();
            LBFrom = new System.Windows.Forms.ListBox();
            LBTo = new System.Windows.Forms.ListBox();
            PicBox = new System.Windows.Forms.PictureBox();
            BTOk = new System.Windows.Forms.Button();
            BTClose = new System.Windows.Forms.Button();
            CBSortOrder = new System.Windows.Forms.ComboBox();
            BTPrint = new System.Windows.Forms.Button();
            CBSerial = new System.Windows.Forms.CheckBox();
            TBStyle = new System.Windows.Forms.ComboBox();
            label4 = new System.Windows.Forms.Label();
            statusStrip1 = new System.Windows.Forms.StatusStrip();
            label2 = new System.Windows.Forms.Label();
            label3 = new System.Windows.Forms.Label();
            label6 = new System.Windows.Forms.Label();
            label1 = new System.Windows.Forms.Label();
            statusStrip1.SuspendLayout();
            MainPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)PicBox).BeginInit();
            SuspendLayout();
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Dock = System.Windows.Forms.DockStyle.Fill;
            label4.Location = new System.Drawing.Point(188, 0);
            label4.Name = "label4";
            MainPanel.SetRowSpan(label4, 2);
            label4.Size = new System.Drawing.Size(23, 169);
            label4.TabIndex = 9;
            label4.Text = ">>";
            label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // statusStrip1
            // 
            statusStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { LBStatus });
            statusStrip1.Location = new System.Drawing.Point(0, 339);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Padding = new System.Windows.Forms.Padding(1, 0, 16, 0);
            statusStrip1.Size = new System.Drawing.Size(484, 22);
            statusStrip1.TabIndex = 12;
            statusStrip1.Text = "statusStrip1";
            // 
            // LBStatus
            // 
            LBStatus.Name = "LBStatus";
            LBStatus.Size = new System.Drawing.Size(10, 17);
            LBStatus.Text = " ";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(12, 15);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(55, 15);
            label2.TabIndex = 14;
            label2.Text = "変更形式";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new System.Drawing.Point(12, 88);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(31, 15);
            label3.TabIndex = 16;
            label3.Text = "結果";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new System.Drawing.Point(12, 59);
            label6.Name = "label6";
            label6.Size = new System.Drawing.Size(44, 15);
            label6.TabIndex = 18;
            label6.Text = "ソート順";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(73, 38);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(311, 15);
            label1.TabIndex = 19;
            label1.Text = "prefix-(3) とすると prefix-001 から順番にファイル名を変更する。";
            // 
            // MainPanel
            // 
            MainPanel.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            MainPanel.ColumnCount = 3;
            MainPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            MainPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 29F));
            MainPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            MainPanel.Controls.Add(LBFrom, 0, 0);
            MainPanel.Controls.Add(LBTo, 2, 0);
            MainPanel.Controls.Add(label4, 1, 0);
            MainPanel.Controls.Add(PicBox, 2, 1);
            MainPanel.Location = new System.Drawing.Point(73, 85);
            MainPanel.Name = "MainPanel";
            MainPanel.RowCount = 2;
            MainPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            MainPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            MainPanel.Size = new System.Drawing.Size(399, 169);
            MainPanel.TabIndex = 3;
            // 
            // LBFrom
            // 
            LBFrom.Dock = System.Windows.Forms.DockStyle.Fill;
            LBFrom.FormattingEnabled = true;
            LBFrom.ItemHeight = 15;
            LBFrom.Location = new System.Drawing.Point(3, 3);
            LBFrom.Name = "LBFrom";
            MainPanel.SetRowSpan(LBFrom, 2);
            LBFrom.Size = new System.Drawing.Size(179, 163);
            LBFrom.TabIndex = 3;
            LBFrom.SelectedIndexChanged += LBFrom_SelectedIndexChanged;
            LBFrom.DoubleClick += LBFrom_DoubleClick;
            LBFrom.KeyDown += LBFrom_KeyDown;
            // 
            // LBTo
            // 
            LBTo.Dock = System.Windows.Forms.DockStyle.Fill;
            LBTo.FormattingEnabled = true;
            LBTo.ItemHeight = 15;
            LBTo.Location = new System.Drawing.Point(217, 3);
            LBTo.Name = "LBTo";
            LBTo.Size = new System.Drawing.Size(179, 78);
            LBTo.TabIndex = 21;
            LBTo.TabStop = false;
            // 
            // PicBox
            // 
            PicBox.Dock = System.Windows.Forms.DockStyle.Fill;
            PicBox.Location = new System.Drawing.Point(217, 87);
            PicBox.Name = "PicBox";
            PicBox.Size = new System.Drawing.Size(179, 79);
            PicBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            PicBox.TabIndex = 10;
            PicBox.TabStop = false;
            // 
            // BTOk
            // 
            BTOk.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            BTOk.Location = new System.Drawing.Point(290, 307);
            BTOk.Name = "BTOk";
            BTOk.Size = new System.Drawing.Size(88, 29);
            BTOk.TabIndex = 6;
            BTOk.Text = "変更";
            BTOk.UseVisualStyleBackColor = true;
            BTOk.Click += BTOk_Click;
            // 
            // BTClose
            // 
            BTClose.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            BTClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            BTClose.Location = new System.Drawing.Point(384, 307);
            BTClose.Name = "BTClose";
            BTClose.Size = new System.Drawing.Size(88, 29);
            BTClose.TabIndex = 7;
            BTClose.Text = "閉じる";
            BTClose.UseVisualStyleBackColor = true;
            // 
            // CBSortOrder
            // 
            CBSortOrder.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            CBSortOrder.FormattingEnabled = true;
            CBSortOrder.Location = new System.Drawing.Point(73, 56);
            CBSortOrder.Name = "CBSortOrder";
            CBSortOrder.Size = new System.Drawing.Size(188, 23);
            CBSortOrder.TabIndex = 2;
            CBSortOrder.SelectedIndexChanged += CBSortOrder_SelectedIndexChanged;
            // 
            // BTPrint
            // 
            BTPrint.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            BTPrint.Location = new System.Drawing.Point(290, 260);
            BTPrint.Name = "BTPrint";
            BTPrint.Size = new System.Drawing.Size(112, 29);
            BTPrint.TabIndex = 5;
            BTPrint.Text = "表示変更(F1)";
            BTPrint.UseVisualStyleBackColor = true;
            BTPrint.Click += BTPrint_Click;
            // 
            // CBSerial
            // 
            CBSerial.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            CBSerial.AutoSize = true;
            CBSerial.Location = new System.Drawing.Point(76, 266);
            CBSerial.Name = "CBSerial";
            CBSerial.Size = new System.Drawing.Size(110, 19);
            CBSerial.TabIndex = 4;
            CBSerial.Text = "再シリアル化 (F2)";
            CBSerial.UseVisualStyleBackColor = true;
            CBSerial.CheckedChanged += CBSerial_CheckedChanged;
            // 
            // TBStyle
            // 
            TBStyle.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            TBStyle.Location = new System.Drawing.Point(73, 12);
            TBStyle.Name = "TBStyle";
            TBStyle.Size = new System.Drawing.Size(399, 23);
            TBStyle.TabIndex = 1;
            TBStyle.TextChanged += TBStyle_TextChanged;
            TBStyle.KeyDown += TBStyle_KeyDown;
            // 
            // SerialForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            CancelButton = BTClose;
            ClientSize = new System.Drawing.Size(484, 361);
            Controls.Add(TBStyle);
            Controls.Add(label1);
            Controls.Add(CBSerial);
            Controls.Add(BTPrint);
            Controls.Add(CBSortOrder);
            Controls.Add(label6);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(statusStrip1);
            Controls.Add(BTClose);
            Controls.Add(BTOk);
            Controls.Add(MainPanel);
            KeyPreview = true;
            Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            MinimumSize = new System.Drawing.Size(498, 393);
            Name = "SerialForm";
            Text = "順序ファイル名変更";
            KeyDown += SerialForm_KeyDown;
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
            MainPanel.ResumeLayout(false);
            MainPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)PicBox).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Button BTOk;
        private System.Windows.Forms.Button BTClose;
        private System.Windows.Forms.ListBox LBFrom;
        private System.Windows.Forms.ListBox LBTo;
        private System.Windows.Forms.ToolStripStatusLabel LBStatus;
        private System.Windows.Forms.ComboBox CBSortOrder;
        private System.Windows.Forms.PictureBox PicBox;
        private System.Windows.Forms.Button BTPrint;
        private System.Windows.Forms.TableLayoutPanel MainPanel;
        private System.Windows.Forms.CheckBox CBSerial;
        private System.Windows.Forms.ComboBox TBStyle;
    }
}