namespace ImageViewer
{
    partial class BoundForm
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
            System.Windows.Forms.Label label2;
            System.Windows.Forms.Label label3;
            BTClose = new System.Windows.Forms.Button();
            BTOk = new System.Windows.Forms.Button();
            LBFile = new System.Windows.Forms.Label();
            TBFilter = new System.Windows.Forms.ComboBox();
            label2 = new System.Windows.Forms.Label();
            label3 = new System.Windows.Forms.Label();
            SuspendLayout();
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(12, 37);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(49, 15);
            label2.TabIndex = 1;
            label2.Text = "フィルター";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new System.Drawing.Point(12, 9);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(65, 15);
            label3.TabIndex = 1;
            label3.Text = "選択ファイル";
            // 
            // BTClose
            // 
            BTClose.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            BTClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            BTClose.Location = new System.Drawing.Point(344, 63);
            BTClose.Name = "BTClose";
            BTClose.Size = new System.Drawing.Size(88, 29);
            BTClose.TabIndex = 4;
            BTClose.Text = "閉じる";
            BTClose.UseVisualStyleBackColor = true;
            // 
            // BTOk
            // 
            BTOk.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            BTOk.Location = new System.Drawing.Point(248, 63);
            BTOk.Name = "BTOk";
            BTOk.Size = new System.Drawing.Size(88, 29);
            BTOk.TabIndex = 3;
            BTOk.Text = "設定";
            BTOk.UseVisualStyleBackColor = true;
            BTOk.Click += BTOk_Click;
            // 
            // LBFile
            // 
            LBFile.AutoSize = true;
            LBFile.Location = new System.Drawing.Point(85, 9);
            LBFile.Name = "LBFile";
            LBFile.Size = new System.Drawing.Size(32, 15);
            LBFile.TabIndex = 1;
            LBFile.Text = "-----";
            // 
            // TBFilter
            // 
            TBFilter.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            TBFilter.FormattingEnabled = true;
            TBFilter.Location = new System.Drawing.Point(85, 34);
            TBFilter.Name = "TBFilter";
            TBFilter.Size = new System.Drawing.Size(347, 23);
            TBFilter.TabIndex = 2;
            // 
            // BoundForm
            // 
            AcceptButton = BTOk;
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            CancelButton = BTClose;
            ClientSize = new System.Drawing.Size(444, 101);
            Controls.Add(TBFilter);
            Controls.Add(LBFile);
            Controls.Add(label3);
            Controls.Add(BTOk);
            Controls.Add(BTClose);
            Controls.Add(label2);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            Margin = new System.Windows.Forms.Padding(4);
            Name = "BoundForm";
            Text = "ファイル名範囲";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Button BTClose;
        private System.Windows.Forms.Button BTOk;
        private System.Windows.Forms.Label LBFile;
        private System.Windows.Forms.ComboBox TBFilter;
    }
}