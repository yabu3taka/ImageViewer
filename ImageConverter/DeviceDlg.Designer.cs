namespace ImageConverter
{
    partial class DeviceDlg
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
            CmbDevice = new System.Windows.Forms.ComboBox();
            BtnOk = new System.Windows.Forms.Button();
            SuspendLayout();
            // 
            // CmbDevice
            // 
            CmbDevice.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            CmbDevice.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            CmbDevice.FormattingEnabled = true;
            CmbDevice.Location = new System.Drawing.Point(10, 12);
            CmbDevice.Name = "CmbDevice";
            CmbDevice.Size = new System.Drawing.Size(424, 23);
            CmbDevice.TabIndex = 0;
            // 
            // BtnOk
            // 
            BtnOk.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            BtnOk.Location = new System.Drawing.Point(359, 47);
            BtnOk.Name = "BtnOk";
            BtnOk.Size = new System.Drawing.Size(75, 23);
            BtnOk.TabIndex = 1;
            BtnOk.Text = "Ok";
            BtnOk.UseVisualStyleBackColor = true;
            BtnOk.Click += BtnOk_Click;
            // 
            // DeviceDlg
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(446, 83);
            Controls.Add(BtnOk);
            Controls.Add(CmbDevice);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            Name = "DeviceDlg";
            Text = "デバイス選択";
            Load += DeviceDlg_Load;
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.ComboBox CmbDevice;
        private System.Windows.Forms.Button BtnOk;
    }
}