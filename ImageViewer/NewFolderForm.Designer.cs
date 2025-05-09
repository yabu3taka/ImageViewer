namespace ImageViewer
{
    partial class NewFolderForm
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
            TBNewName = new System.Windows.Forms.TextBox();
            BTOk = new System.Windows.Forms.Button();
            BTClose = new System.Windows.Forms.Button();
            LKFolder = new System.Windows.Forms.LinkLabel();
            SuspendLayout();
            // 
            // TBNewName
            // 
            TBNewName.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            TBNewName.Location = new System.Drawing.Point(72, 12);
            TBNewName.Name = "TBNewName";
            TBNewName.Size = new System.Drawing.Size(360, 23);
            TBNewName.TabIndex = 1;
            // 
            // BTOk
            // 
            BTOk.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            BTOk.Location = new System.Drawing.Point(250, 41);
            BTOk.Name = "BTOk";
            BTOk.Size = new System.Drawing.Size(88, 29);
            BTOk.TabIndex = 2;
            BTOk.Text = "作成";
            BTOk.UseVisualStyleBackColor = true;
            BTOk.Click += BTOk_Click;
            // 
            // BTClose
            // 
            BTClose.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            BTClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            BTClose.Location = new System.Drawing.Point(344, 41);
            BTClose.Name = "BTClose";
            BTClose.Size = new System.Drawing.Size(88, 29);
            BTClose.TabIndex = 3;
            BTClose.Text = "閉じる";
            BTClose.UseVisualStyleBackColor = true;
            // 
            // LKFolder
            // 
            LKFolder.AutoSize = true;
            LKFolder.Location = new System.Drawing.Point(12, 15);
            LKFolder.Name = "LKFolder";
            LKFolder.Size = new System.Drawing.Size(54, 15);
            LKFolder.TabIndex = 4;
            LKFolder.TabStop = true;
            LKFolder.Text = "フォルダ名";
            LKFolder.LinkClicked += LKFolder_LinkClicked;
            // 
            // NewFolderForm
            // 
            AcceptButton = BTOk;
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            CancelButton = BTClose;
            ClientSize = new System.Drawing.Size(444, 81);
            Controls.Add(LKFolder);
            Controls.Add(TBNewName);
            Controls.Add(BTOk);
            Controls.Add(BTClose);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            KeyPreview = true;
            Margin = new System.Windows.Forms.Padding(4);
            Name = "NewFolderForm";
            Text = "フォルダ作成";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Button BTOk;
        private System.Windows.Forms.Button BTClose;
        private System.Windows.Forms.TextBox TBNewName;
        private System.Windows.Forms.LinkLabel LKFolder;
    }
}