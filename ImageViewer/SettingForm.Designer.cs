namespace ImageViewer
{
    partial class SettingForm
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
            TBMain = new System.Windows.Forms.TabControl();
            TPComment = new System.Windows.Forms.TabPage();
            label2 = new System.Windows.Forms.Label();
            TBText = new System.Windows.Forms.TextBox();
            TPTitle = new System.Windows.Forms.TabPage();
            label1 = new System.Windows.Forms.Label();
            TBTitle = new System.Windows.Forms.TextBox();
            BTCancel = new System.Windows.Forms.Button();
            BTOk = new System.Windows.Forms.Button();
            TBMain.SuspendLayout();
            TPComment.SuspendLayout();
            TPTitle.SuspendLayout();
            SuspendLayout();
            // 
            // TBMain
            // 
            TBMain.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            TBMain.Controls.Add(TPComment);
            TBMain.Controls.Add(TPTitle);
            TBMain.Location = new System.Drawing.Point(12, 12);
            TBMain.Name = "TBMain";
            TBMain.SelectedIndex = 0;
            TBMain.Size = new System.Drawing.Size(540, 392);
            TBMain.TabIndex = 1;
            // 
            // TPComment
            // 
            TPComment.Controls.Add(label2);
            TPComment.Controls.Add(TBText);
            TPComment.Location = new System.Drawing.Point(4, 24);
            TPComment.Name = "TPComment";
            TPComment.Size = new System.Drawing.Size(532, 364);
            TPComment.TabIndex = 0;
            TPComment.Text = "コメント(F5)";
            TPComment.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(10, 10);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(40, 15);
            label2.TabIndex = 3;
            label2.Text = "コメント";
            // 
            // TBText
            // 
            TBText.AcceptsReturn = true;
            TBText.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            TBText.Location = new System.Drawing.Point(10, 30);
            TBText.Multiline = true;
            TBText.Name = "TBText";
            TBText.Size = new System.Drawing.Size(512, 318);
            TBText.TabIndex = 2;
            // 
            // TPTitle
            // 
            TPTitle.Controls.Add(label1);
            TPTitle.Controls.Add(TBTitle);
            TPTitle.Location = new System.Drawing.Point(4, 24);
            TPTitle.Name = "TPTitle";
            TPTitle.Size = new System.Drawing.Size(532, 364);
            TPTitle.TabIndex = 1;
            TPTitle.Text = "タイトル(F6)";
            TPTitle.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(10, 10);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(43, 15);
            label1.TabIndex = 2;
            label1.Text = "タイトル";
            // 
            // TBTitle
            // 
            TBTitle.AcceptsReturn = true;
            TBTitle.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            TBTitle.Location = new System.Drawing.Point(10, 30);
            TBTitle.Multiline = true;
            TBTitle.Name = "TBTitle";
            TBTitle.Size = new System.Drawing.Size(512, 318);
            TBTitle.TabIndex = 2;
            // 
            // BTCancel
            // 
            BTCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            BTCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            BTCancel.Location = new System.Drawing.Point(464, 410);
            BTCancel.Name = "BTCancel";
            BTCancel.Size = new System.Drawing.Size(88, 29);
            BTCancel.TabIndex = 11;
            BTCancel.Text = "閉じる";
            BTCancel.UseVisualStyleBackColor = true;
            // 
            // BTOk
            // 
            BTOk.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            BTOk.Location = new System.Drawing.Point(370, 410);
            BTOk.Name = "BTOk";
            BTOk.Size = new System.Drawing.Size(88, 29);
            BTOk.TabIndex = 10;
            BTOk.Text = "保存(F1)";
            BTOk.UseVisualStyleBackColor = true;
            BTOk.Click += BTOk_Click;
            // 
            // SettingForm
            // 
            AcceptButton = BTOk;
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            CancelButton = BTCancel;
            ClientSize = new System.Drawing.Size(564, 451);
            Controls.Add(TBMain);
            Controls.Add(BTCancel);
            Controls.Add(BTOk);
            KeyPreview = true;
            Margin = new System.Windows.Forms.Padding(4);
            MinimumSize = new System.Drawing.Size(465, 365);
            Name = "SettingForm";
            Text = "設定";
            KeyDown += SettingForm_KeyDown;
            TBMain.ResumeLayout(false);
            TPComment.ResumeLayout(false);
            TPComment.PerformLayout();
            TPTitle.ResumeLayout(false);
            TPTitle.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Button BTCancel;
        private System.Windows.Forms.Button BTOk;
        private System.Windows.Forms.TabPage TPComment;
        private System.Windows.Forms.TextBox TBText;
        private System.Windows.Forms.TabPage TPTitle;
        private System.Windows.Forms.TextBox TBTitle;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TabControl TBMain;
    }
}