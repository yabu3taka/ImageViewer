namespace ImageConverter
{
    partial class ModificationForm
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
            CBList = new System.Windows.Forms.CheckedListBox();
            BtnRecheck = new System.Windows.Forms.Button();
            BtnCopy = new System.Windows.Forms.Button();
            SuspendLayout();
            // 
            // CBList
            // 
            CBList.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            CBList.FormattingEnabled = true;
            CBList.Location = new System.Drawing.Point(12, 12);
            CBList.Name = "CBList";
            CBList.Size = new System.Drawing.Size(260, 310);
            CBList.TabIndex = 0;
            CBList.ItemCheck += CBList_ItemCheck;
            // 
            // BtnRecheck
            // 
            BtnRecheck.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            BtnRecheck.Location = new System.Drawing.Point(197, 326);
            BtnRecheck.Name = "BtnRecheck";
            BtnRecheck.Size = new System.Drawing.Size(75, 23);
            BtnRecheck.TabIndex = 1;
            BtnRecheck.Text = "再確認";
            BtnRecheck.UseVisualStyleBackColor = true;
            BtnRecheck.Click += BtnRecheck_Click;
            // 
            // BtnCopy
            // 
            BtnCopy.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            BtnCopy.Location = new System.Drawing.Point(116, 326);
            BtnCopy.Name = "BtnCopy";
            BtnCopy.Size = new System.Drawing.Size(75, 23);
            BtnCopy.TabIndex = 2;
            BtnCopy.Text = "コピー";
            BtnCopy.UseVisualStyleBackColor = true;
            BtnCopy.Click += BtnCopy_Click;
            // 
            // ModificationForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(284, 361);
            Controls.Add(BtnCopy);
            Controls.Add(BtnRecheck);
            Controls.Add(CBList);
            Name = "ModificationForm";
            StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            Text = "変更点";
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.CheckedListBox CBList;
        private System.Windows.Forms.Button BtnRecheck;
        private System.Windows.Forms.Button BtnCopy;
    }
}