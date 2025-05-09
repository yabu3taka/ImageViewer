namespace ImageViewer
{
    partial class RenameForm
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
            TBNewName = new System.Windows.Forms.TextBox();
            BTOk = new System.Windows.Forms.Button();
            BTClose = new System.Windows.Forms.Button();
            LBOldName = new System.Windows.Forms.Label();
            LKNewName = new System.Windows.Forms.LinkLabel();
            label2 = new System.Windows.Forms.Label();
            SuspendLayout();
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(12, 9);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(65, 15);
            label2.TabIndex = 1;
            label2.Text = "現在の名前";
            // 
            // TBNewName
            // 
            TBNewName.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            TBNewName.Location = new System.Drawing.Point(83, 27);
            TBNewName.Name = "TBNewName";
            TBNewName.Size = new System.Drawing.Size(354, 23);
            TBNewName.TabIndex = 1;
            // 
            // BTOk
            // 
            BTOk.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            BTOk.Location = new System.Drawing.Point(255, 56);
            BTOk.Name = "BTOk";
            BTOk.Size = new System.Drawing.Size(88, 29);
            BTOk.TabIndex = 2;
            BTOk.Text = "変更";
            BTOk.UseVisualStyleBackColor = true;
            BTOk.Click += BTOk_Click;
            // 
            // BTClose
            // 
            BTClose.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            BTClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            BTClose.Location = new System.Drawing.Point(349, 56);
            BTClose.Name = "BTClose";
            BTClose.Size = new System.Drawing.Size(88, 29);
            BTClose.TabIndex = 3;
            BTClose.Text = "閉じる";
            BTClose.UseVisualStyleBackColor = true;
            // 
            // LBOldName
            // 
            LBOldName.AutoSize = true;
            LBOldName.Location = new System.Drawing.Point(83, 9);
            LBOldName.Name = "LBOldName";
            LBOldName.Size = new System.Drawing.Size(22, 15);
            LBOldName.TabIndex = 1;
            LBOldName.Text = "     ";
            // 
            // LKNewName
            // 
            LKNewName.AutoSize = true;
            LKNewName.Location = new System.Drawing.Point(12, 28);
            LKNewName.Name = "LKNewName";
            LKNewName.Size = new System.Drawing.Size(61, 30);
            LKNewName.TabIndex = 4;
            LKNewName.TabStop = true;
            LKNewName.Text = "新しい名前\r\n(F12)";
            LKNewName.LinkClicked += LKNewName_LinkClicked;
            // 
            // RenameForm
            // 
            AcceptButton = BTOk;
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            CancelButton = BTClose;
            ClientSize = new System.Drawing.Size(449, 96);
            Controls.Add(LKNewName);
            Controls.Add(LBOldName);
            Controls.Add(label2);
            Controls.Add(TBNewName);
            Controls.Add(BTOk);
            Controls.Add(BTClose);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            KeyPreview = true;
            Margin = new System.Windows.Forms.Padding(4);
            Name = "RenameForm";
            Text = "ファイル名変更";
            KeyDown += RenameForm_KeyDown;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Button BTOk;
        private System.Windows.Forms.Button BTClose;
        private System.Windows.Forms.TextBox TBNewName;
        private System.Windows.Forms.Label LBOldName;
        private System.Windows.Forms.LinkLabel LKNewName;
    }
}