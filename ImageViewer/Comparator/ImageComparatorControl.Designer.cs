namespace ImageViewer.Comparator
{
    partial class ImageComparatorControl
    {
        /// <summary> 
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージ リソースを破棄する場合は true を指定し、その他の場合は false を指定します。</param>
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
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.GroupBox groupBox1;
            System.Windows.Forms.Label label1;
            this.BTRun = new System.Windows.Forms.Button();
            this.UDNear = new System.Windows.Forms.NumericUpDown();
            this.LKNear = new System.Windows.Forms.LinkLabel();
            this.CBMethod = new System.Windows.Forms.ComboBox();
            this.GBResult = new System.Windows.Forms.GroupBox();
            this.LVFile = new ImageViewer.UI.ImageFileListView();
            this.CBKeyFile = new System.Windows.Forms.ComboBox();
            this.CompareWorker = new System.ComponentModel.BackgroundWorker();
            groupBox1 = new System.Windows.Forms.GroupBox();
            label1 = new System.Windows.Forms.Label();
            groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.UDNear)).BeginInit();
            this.GBResult.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            groupBox1.Controls.Add(this.BTRun);
            groupBox1.Controls.Add(this.UDNear);
            groupBox1.Controls.Add(this.LKNear);
            groupBox1.Controls.Add(this.CBMethod);
            groupBox1.Controls.Add(label1);
            groupBox1.Location = new System.Drawing.Point(4, 4);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new System.Drawing.Size(153, 124);
            groupBox1.TabIndex = 1;
            groupBox1.TabStop = false;
            groupBox1.Text = "比較設定";
            // 
            // BTRun
            // 
            this.BTRun.Location = new System.Drawing.Point(46, 81);
            this.BTRun.Name = "BTRun";
            this.BTRun.Size = new System.Drawing.Size(83, 29);
            this.BTRun.TabIndex = 5;
            this.BTRun.Text = "比較";
            this.BTRun.UseVisualStyleBackColor = true;
            // 
            // UDNear
            // 
            this.UDNear.Location = new System.Drawing.Point(46, 50);
            this.UDNear.Name = "UDNear";
            this.UDNear.Size = new System.Drawing.Size(68, 23);
            this.UDNear.TabIndex = 4;
            // 
            // LKNear
            // 
            this.LKNear.AutoSize = true;
            this.LKNear.Location = new System.Drawing.Point(4, 51);
            this.LKNear.Name = "LKNear";
            this.LKNear.Size = new System.Drawing.Size(27, 15);
            this.LKNear.TabIndex = 3;
            this.LKNear.TabStop = true;
            this.LKNear.Text = "近さ";
            this.LKNear.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // CBMethod
            // 
            this.CBMethod.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.CBMethod.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CBMethod.FormattingEnabled = true;
            this.CBMethod.Items.AddRange(new object[] {
            "AveHash",
            "GrayHash"});
            this.CBMethod.Location = new System.Drawing.Point(45, 17);
            this.CBMethod.Name = "CBMethod";
            this.CBMethod.Size = new System.Drawing.Size(101, 23);
            this.CBMethod.TabIndex = 2;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(4, 21);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(31, 15);
            label1.TabIndex = 10;
            label1.Text = "手段";
            label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // GBResult
            // 
            this.GBResult.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.GBResult.Controls.Add(this.LVFile);
            this.GBResult.Controls.Add(this.CBKeyFile);
            this.GBResult.Location = new System.Drawing.Point(4, 131);
            this.GBResult.Name = "GBResult";
            this.GBResult.Size = new System.Drawing.Size(153, 178);
            this.GBResult.TabIndex = 10;
            this.GBResult.TabStop = false;
            this.GBResult.Text = "比較結果";
            this.GBResult.Visible = false;
            // 
            // LVFile
            // 
            this.LVFile.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.LVFile.BgColorFunc = null;
            this.LVFile.HideSelection = false;
            this.LVFile.Location = new System.Drawing.Point(6, 54);
            this.LVFile.Name = "LVFile";
            this.LVFile.Size = new System.Drawing.Size(140, 116);
            this.LVFile.TabIndex = 12;
            this.LVFile.UseCompatibleStateImageBehavior = false;
            this.LVFile.View = System.Windows.Forms.View.SmallIcon;
            this.LVFile.ItemActivate += new System.EventHandler(this.LVFile_ItemActivate);
            // 
            // CBKeyFile
            // 
            this.CBKeyFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.CBKeyFile.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CBKeyFile.Location = new System.Drawing.Point(6, 21);
            this.CBKeyFile.Name = "CBKeyFile";
            this.CBKeyFile.Size = new System.Drawing.Size(140, 23);
            this.CBKeyFile.TabIndex = 11;
            this.CBKeyFile.SelectionChangeCommitted += new System.EventHandler(this.CBKeyFile_SelectionChangeCommitted);
            // 
            // CompareWorker
            // 
            this.CompareWorker.WorkerReportsProgress = true;
            this.CompareWorker.WorkerSupportsCancellation = true;
            this.CompareWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.CompareWorker_DoWork);
            this.CompareWorker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.CompareWorker_ProgressChanged);
            this.CompareWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.CompareWorker_RunWorkerCompleted);
            // 
            // ImageComparatorControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.GBResult);
            this.Controls.Add(groupBox1);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "ImageComparatorControl";
            this.Size = new System.Drawing.Size(160, 312);
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.UDNear)).EndInit();
            this.GBResult.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.ComboBox CBKeyFile;
        private ImageViewer.UI.ImageFileListView LVFile;
        private System.ComponentModel.BackgroundWorker CompareWorker;
        private System.Windows.Forms.GroupBox GBResult;
        private System.Windows.Forms.Button BTRun;
        private System.Windows.Forms.NumericUpDown UDNear;
        private System.Windows.Forms.LinkLabel LKNear;
        private System.Windows.Forms.ComboBox CBMethod;
    }
}
