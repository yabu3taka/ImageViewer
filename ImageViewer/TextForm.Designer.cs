namespace ImageViewer
{
    partial class TextForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
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
            TBPhotoText = new System.Windows.Forms.TextBox();
            BTOk = new System.Windows.Forms.Button();
            BTCancel = new System.Windows.Forms.Button();
            SuspendLayout();
            // 
            // TBPhotoText
            // 
            TBPhotoText.AcceptsReturn = true;
            TBPhotoText.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            TBPhotoText.Location = new System.Drawing.Point(12, 12);
            TBPhotoText.Multiline = true;
            TBPhotoText.Name = "TBPhotoText";
            TBPhotoText.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            TBPhotoText.Size = new System.Drawing.Size(650, 392);
            TBPhotoText.TabIndex = 1;
            // 
            // BTOk
            // 
            BTOk.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            BTOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            BTOk.Location = new System.Drawing.Point(478, 410);
            BTOk.Name = "BTOk";
            BTOk.Size = new System.Drawing.Size(88, 29);
            BTOk.TabIndex = 2;
            BTOk.Text = "保存(F1)";
            BTOk.UseVisualStyleBackColor = true;
            // 
            // BTCancel
            // 
            BTCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            BTCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            BTCancel.Location = new System.Drawing.Point(574, 410);
            BTCancel.Name = "BTCancel";
            BTCancel.Size = new System.Drawing.Size(88, 29);
            BTCancel.TabIndex = 3;
            BTCancel.Text = "閉じる";
            BTCancel.UseVisualStyleBackColor = true;
            // 
            // TextForm
            // 
            AcceptButton = BTOk;
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            CancelButton = BTCancel;
            ClientSize = new System.Drawing.Size(674, 451);
            Controls.Add(BTCancel);
            Controls.Add(BTOk);
            Controls.Add(TBPhotoText);
            KeyPreview = true;
            Margin = new System.Windows.Forms.Padding(4);
            MinimumSize = new System.Drawing.Size(340, 240);
            Name = "TextForm";
            Text = "画像テキスト";
            KeyDown += TextForm_KeyDown;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.TextBox TBPhotoText;
        private System.Windows.Forms.Button BTOk;
        private System.Windows.Forms.Button BTCancel;
    }
}