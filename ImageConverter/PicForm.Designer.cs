
namespace ImageConverter
{
    partial class PicForm
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
            MainPic = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)MainPic).BeginInit();
            SuspendLayout();
            // 
            // MainPic
            // 
            MainPic.Dock = System.Windows.Forms.DockStyle.Fill;
            MainPic.Location = new System.Drawing.Point(0, 0);
            MainPic.Name = "MainPic";
            MainPic.Size = new System.Drawing.Size(284, 361);
            MainPic.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            MainPic.TabIndex = 0;
            MainPic.TabStop = false;
            // 
            // PicForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(284, 361);
            Controls.Add(MainPic);
            Name = "PicForm";
            StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            Text = "画像チェック";
            FormClosed += PicForm_FormClosed;
            Shown += PicForm_Shown;
            ((System.ComponentModel.ISupportInitialize)MainPic).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.PictureBox MainPic;
    }
}