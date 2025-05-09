
namespace ImageViewer
{
    partial class SizeBoundForm
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
            components = new System.ComponentModel.Container();
            BTOk = new System.Windows.Forms.Button();
            BTClose = new System.Windows.Forms.Button();
            tabControl1 = new System.Windows.Forms.TabControl();
            TPSize = new System.Windows.Forms.TabPage();
            groupBox2 = new System.Windows.Forms.GroupBox();
            LBSizeHeightMin = new System.Windows.Forms.Label();
            TBSizeHeight = new System.Windows.Forms.TextBox();
            LBSizeHeightMax = new System.Windows.Forms.Label();
            groupBox1 = new System.Windows.Forms.GroupBox();
            LBSizeWidthMin = new System.Windows.Forms.Label();
            LBSizeWidthMax = new System.Windows.Forms.Label();
            TBSizeWidth = new System.Windows.Forms.TextBox();
            LBTarget = new System.Windows.Forms.ListBox();
            LBList = new System.Windows.Forms.Label();
            TBClear = new System.Windows.Forms.Button();
            EPWidthHeight = new System.Windows.Forms.ErrorProvider(components);
            TMMain = new System.Windows.Forms.Timer(components);
            tabControl1.SuspendLayout();
            TPSize.SuspendLayout();
            groupBox2.SuspendLayout();
            groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)EPWidthHeight).BeginInit();
            SuspendLayout();
            // 
            // BTOk
            // 
            BTOk.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            BTOk.Location = new System.Drawing.Point(219, 370);
            BTOk.Name = "BTOk";
            BTOk.Size = new System.Drawing.Size(88, 29);
            BTOk.TabIndex = 22;
            BTOk.Text = "設定";
            BTOk.UseVisualStyleBackColor = true;
            BTOk.Click += BTOk_Click;
            // 
            // BTClose
            // 
            BTClose.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            BTClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            BTClose.Location = new System.Drawing.Point(314, 370);
            BTClose.Name = "BTClose";
            BTClose.Size = new System.Drawing.Size(88, 29);
            BTClose.TabIndex = 23;
            BTClose.Text = "閉じる";
            BTClose.UseVisualStyleBackColor = true;
            // 
            // tabControl1
            // 
            tabControl1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            tabControl1.Controls.Add(TPSize);
            tabControl1.Location = new System.Drawing.Point(11, 12);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new System.Drawing.Size(176, 346);
            tabControl1.TabIndex = 1;
            // 
            // TPSize
            // 
            TPSize.Controls.Add(groupBox2);
            TPSize.Controls.Add(groupBox1);
            TPSize.Location = new System.Drawing.Point(4, 24);
            TPSize.Name = "TPSize";
            TPSize.Size = new System.Drawing.Size(168, 318);
            TPSize.TabIndex = 0;
            TPSize.Text = "サイズ";
            TPSize.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            groupBox2.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            groupBox2.Controls.Add(LBSizeHeightMin);
            groupBox2.Controls.Add(TBSizeHeight);
            groupBox2.Controls.Add(LBSizeHeightMax);
            groupBox2.Location = new System.Drawing.Point(3, 120);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new System.Drawing.Size(158, 109);
            groupBox2.TabIndex = 3;
            groupBox2.TabStop = false;
            groupBox2.Text = "高さ";
            // 
            // LBSizeHeightMin
            // 
            LBSizeHeightMin.AutoSize = true;
            LBSizeHeightMin.Location = new System.Drawing.Point(5, 79);
            LBSizeHeightMin.Name = "LBSizeHeightMin";
            LBSizeHeightMin.Size = new System.Drawing.Size(43, 15);
            LBSizeHeightMin.TabIndex = 3;
            LBSizeHeightMin.Text = "最小：";
            // 
            // TBSizeHeight
            // 
            TBSizeHeight.Location = new System.Drawing.Point(7, 22);
            TBSizeHeight.Name = "TBSizeHeight";
            TBSizeHeight.Size = new System.Drawing.Size(56, 23);
            TBSizeHeight.TabIndex = 3;
            TBSizeHeight.TextChanged += TBSizeWidthHeight_TextChanged;
            TBSizeHeight.Validating += TBSizeWidthHeight_Validating;
            // 
            // LBSizeHeightMax
            // 
            LBSizeHeightMax.AutoSize = true;
            LBSizeHeightMax.Location = new System.Drawing.Point(5, 54);
            LBSizeHeightMax.Name = "LBSizeHeightMax";
            LBSizeHeightMax.Size = new System.Drawing.Size(43, 15);
            LBSizeHeightMax.TabIndex = 3;
            LBSizeHeightMax.Text = "最大：";
            // 
            // groupBox1
            // 
            groupBox1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            groupBox1.Controls.Add(LBSizeWidthMin);
            groupBox1.Controls.Add(LBSizeWidthMax);
            groupBox1.Controls.Add(TBSizeWidth);
            groupBox1.Location = new System.Drawing.Point(3, 4);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new System.Drawing.Size(158, 109);
            groupBox1.TabIndex = 2;
            groupBox1.TabStop = false;
            groupBox1.Text = "幅";
            // 
            // LBSizeWidthMin
            // 
            LBSizeWidthMin.AutoSize = true;
            LBSizeWidthMin.Location = new System.Drawing.Point(6, 80);
            LBSizeWidthMin.Name = "LBSizeWidthMin";
            LBSizeWidthMin.Size = new System.Drawing.Size(43, 15);
            LBSizeWidthMin.TabIndex = 3;
            LBSizeWidthMin.Text = "最小：";
            // 
            // LBSizeWidthMax
            // 
            LBSizeWidthMax.AutoSize = true;
            LBSizeWidthMax.Location = new System.Drawing.Point(6, 55);
            LBSizeWidthMax.Name = "LBSizeWidthMax";
            LBSizeWidthMax.Size = new System.Drawing.Size(43, 15);
            LBSizeWidthMax.TabIndex = 3;
            LBSizeWidthMax.Text = "最大：";
            // 
            // TBSizeWidth
            // 
            TBSizeWidth.Location = new System.Drawing.Point(5, 22);
            TBSizeWidth.Name = "TBSizeWidth";
            TBSizeWidth.Size = new System.Drawing.Size(56, 23);
            TBSizeWidth.TabIndex = 2;
            TBSizeWidth.TextChanged += TBSizeWidthHeight_TextChanged;
            TBSizeWidth.Validating += TBSizeWidthHeight_Validating;
            // 
            // LBTarget
            // 
            LBTarget.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            LBTarget.FormattingEnabled = true;
            LBTarget.ItemHeight = 15;
            LBTarget.Location = new System.Drawing.Point(193, 37);
            LBTarget.Name = "LBTarget";
            LBTarget.Size = new System.Drawing.Size(208, 319);
            LBTarget.TabIndex = 20;
            // 
            // LBList
            // 
            LBList.AutoSize = true;
            LBList.Location = new System.Drawing.Point(193, 18);
            LBList.Name = "LBList";
            LBList.Size = new System.Drawing.Size(31, 15);
            LBList.TabIndex = 3;
            LBList.Text = "対象";
            // 
            // TBClear
            // 
            TBClear.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            TBClear.Location = new System.Drawing.Point(11, 370);
            TBClear.Name = "TBClear";
            TBClear.Size = new System.Drawing.Size(88, 29);
            TBClear.TabIndex = 21;
            TBClear.Text = "クリア";
            TBClear.UseVisualStyleBackColor = true;
            TBClear.Click += TBClear_Click;
            // 
            // EPWidthHeight
            // 
            EPWidthHeight.BlinkStyle = System.Windows.Forms.ErrorBlinkStyle.NeverBlink;
            EPWidthHeight.ContainerControl = this;
            // 
            // TMMain
            // 
            TMMain.Interval = 1000;
            TMMain.Tick += TMMain_Tick;
            // 
            // SizeBoundForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            CancelButton = BTClose;
            ClientSize = new System.Drawing.Size(414, 411);
            Controls.Add(LBList);
            Controls.Add(LBTarget);
            Controls.Add(tabControl1);
            Controls.Add(TBClear);
            Controls.Add(BTOk);
            Controls.Add(BTClose);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            Margin = new System.Windows.Forms.Padding(4);
            MinimumSize = new System.Drawing.Size(430, 450);
            Name = "SizeBoundForm";
            Text = "サイズ制限";
            tabControl1.ResumeLayout(false);
            TPSize.ResumeLayout(false);
            groupBox2.ResumeLayout(false);
            groupBox2.PerformLayout();
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)EPWidthHeight).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Button BTOk;
        private System.Windows.Forms.Button BTClose;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage TPSize;
        private System.Windows.Forms.TextBox TBSizeWidth;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox TBSizeHeight;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label LBSizeWidthMax;
        private System.Windows.Forms.ListBox LBTarget;
        private System.Windows.Forms.Label LBSizeHeightMin;
        private System.Windows.Forms.Label LBSizeHeightMax;
        private System.Windows.Forms.Label LBSizeWidthMin;
        private System.Windows.Forms.Label LBList;
        private System.Windows.Forms.Button TBClear;
        private System.Windows.Forms.ErrorProvider EPWidthHeight;
        private System.Windows.Forms.Timer TMMain;
    }
}