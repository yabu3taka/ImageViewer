
namespace ImageViewer
{
    partial class PGSettingForm
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
            BTCancel = new System.Windows.Forms.Button();
            BTOk = new System.Windows.Forms.Button();
            tabControl1 = new System.Windows.Forms.TabControl();
            TPMain = new System.Windows.Forms.TabPage();
            TBConvParamWidth = new System.Windows.Forms.TextBox();
            label6 = new System.Windows.Forms.Label();
            CBConvWait = new System.Windows.Forms.CheckBox();
            BTConvSelect = new System.Windows.Forms.Button();
            TBConvOutput = new System.Windows.Forms.TextBox();
            TBConvParam = new System.Windows.Forms.TextBox();
            TBConvCmd = new System.Windows.Forms.TextBox();
            label3 = new System.Windows.Forms.Label();
            label5 = new System.Windows.Forms.Label();
            label4 = new System.Windows.Forms.Label();
            label2 = new System.Windows.Forms.Label();
            label1 = new System.Windows.Forms.Label();
            TabMisc = new System.Windows.Forms.TabPage();
            BtnUrlCmd = new System.Windows.Forms.Button();
            TbUrlCmd = new System.Windows.Forms.TextBox();
            label7 = new System.Windows.Forms.Label();
            CmdFileDialog = new System.Windows.Forms.OpenFileDialog();
            tabControl1.SuspendLayout();
            TPMain.SuspendLayout();
            TabMisc.SuspendLayout();
            SuspendLayout();
            // 
            // BTCancel
            // 
            BTCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            BTCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            BTCancel.Location = new System.Drawing.Point(384, 283);
            BTCancel.Name = "BTCancel";
            BTCancel.Size = new System.Drawing.Size(88, 29);
            BTCancel.TabIndex = 51;
            BTCancel.Text = "閉じる";
            BTCancel.UseVisualStyleBackColor = true;
            // 
            // BTOk
            // 
            BTOk.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            BTOk.Location = new System.Drawing.Point(289, 283);
            BTOk.Name = "BTOk";
            BTOk.Size = new System.Drawing.Size(88, 29);
            BTOk.TabIndex = 50;
            BTOk.Text = "保存(F1)";
            BTOk.UseVisualStyleBackColor = true;
            BTOk.Click += BTOk_Click;
            // 
            // tabControl1
            // 
            tabControl1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            tabControl1.Controls.Add(TPMain);
            tabControl1.Controls.Add(TabMisc);
            tabControl1.Location = new System.Drawing.Point(12, 12);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new System.Drawing.Size(461, 264);
            tabControl1.TabIndex = 1;
            // 
            // TPMain
            // 
            TPMain.Controls.Add(TBConvParamWidth);
            TPMain.Controls.Add(label6);
            TPMain.Controls.Add(CBConvWait);
            TPMain.Controls.Add(BTConvSelect);
            TPMain.Controls.Add(TBConvOutput);
            TPMain.Controls.Add(TBConvParam);
            TPMain.Controls.Add(TBConvCmd);
            TPMain.Controls.Add(label3);
            TPMain.Controls.Add(label5);
            TPMain.Controls.Add(label4);
            TPMain.Controls.Add(label2);
            TPMain.Controls.Add(label1);
            TPMain.Location = new System.Drawing.Point(4, 24);
            TPMain.Name = "TPMain";
            TPMain.Size = new System.Drawing.Size(453, 236);
            TPMain.TabIndex = 0;
            TPMain.Text = "変換";
            TPMain.UseVisualStyleBackColor = true;
            // 
            // TBConvParamWidth
            // 
            TBConvParamWidth.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            TBConvParamWidth.Location = new System.Drawing.Point(78, 74);
            TBConvParamWidth.Name = "TBConvParamWidth";
            TBConvParamWidth.Size = new System.Drawing.Size(302, 23);
            TBConvParamWidth.TabIndex = 5;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new System.Drawing.Point(16, 77);
            label6.Name = "label6";
            label6.Size = new System.Drawing.Size(63, 15);
            label6.TabIndex = 4;
            label6.Text = "引数(横長)";
            // 
            // CBConvWait
            // 
            CBConvWait.AutoSize = true;
            CBConvWait.Location = new System.Drawing.Point(78, 141);
            CBConvWait.Name = "CBConvWait";
            CBConvWait.Size = new System.Drawing.Size(189, 19);
            CBConvWait.TabIndex = 3;
            CBConvWait.Text = "プログラム終了を待ち、出力を移動";
            CBConvWait.UseVisualStyleBackColor = true;
            CBConvWait.CheckedChanged += CBConvWait_CheckedChanged;
            // 
            // BTConvSelect
            // 
            BTConvSelect.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            BTConvSelect.Location = new System.Drawing.Point(386, 8);
            BTConvSelect.Name = "BTConvSelect";
            BTConvSelect.Size = new System.Drawing.Size(52, 29);
            BTConvSelect.TabIndex = 2;
            BTConvSelect.Text = "選択";
            BTConvSelect.UseVisualStyleBackColor = true;
            BTConvSelect.Click += BTConvSelect_Click;
            // 
            // TBConvOutput
            // 
            TBConvOutput.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            TBConvOutput.Location = new System.Drawing.Point(82, 172);
            TBConvOutput.Name = "TBConvOutput";
            TBConvOutput.Size = new System.Drawing.Size(302, 23);
            TBConvOutput.TabIndex = 1;
            // 
            // TBConvParam
            // 
            TBConvParam.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            TBConvParam.Location = new System.Drawing.Point(78, 43);
            TBConvParam.Name = "TBConvParam";
            TBConvParam.Size = new System.Drawing.Size(302, 23);
            TBConvParam.TabIndex = 1;
            // 
            // TBConvCmd
            // 
            TBConvCmd.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            TBConvCmd.Location = new System.Drawing.Point(78, 12);
            TBConvCmd.Name = "TBConvCmd";
            TBConvCmd.Size = new System.Drawing.Size(302, 23);
            TBConvCmd.TabIndex = 1;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new System.Drawing.Point(6, 171);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(66, 15);
            label3.TabIndex = 0;
            label3.Text = "出力フォルダ";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new System.Drawing.Point(78, 195);
            label5.Name = "label5";
            label5.Size = new System.Drawing.Size(200, 15);
            label5.TabIndex = 0;
            label5.Text = "変換対象のフォルダからの相対パスです。";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new System.Drawing.Point(78, 112);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(175, 15);
            label4.TabIndex = 0;
            label4.Text = "{} に変換対象のフォルダがあります。";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(41, 46);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(31, 15);
            label2.TabIndex = 0;
            label2.Text = "引数";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(7, 15);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(65, 15);
            label1.TabIndex = 0;
            label1.Text = "変換コマンド";
            // 
            // TabMisc
            // 
            TabMisc.Controls.Add(BtnUrlCmd);
            TabMisc.Controls.Add(TbUrlCmd);
            TabMisc.Controls.Add(label7);
            TabMisc.Location = new System.Drawing.Point(4, 24);
            TabMisc.Name = "TabMisc";
            TabMisc.Padding = new System.Windows.Forms.Padding(3);
            TabMisc.Size = new System.Drawing.Size(453, 236);
            TabMisc.TabIndex = 1;
            TabMisc.Text = "その他";
            TabMisc.UseVisualStyleBackColor = true;
            // 
            // BtnUrlCmd
            // 
            BtnUrlCmd.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            BtnUrlCmd.Location = new System.Drawing.Point(387, 2);
            BtnUrlCmd.Name = "BtnUrlCmd";
            BtnUrlCmd.Size = new System.Drawing.Size(52, 29);
            BtnUrlCmd.TabIndex = 5;
            BtnUrlCmd.Text = "選択";
            BtnUrlCmd.UseVisualStyleBackColor = true;
            BtnUrlCmd.Click += BtnUrlCmd_Click;
            // 
            // TbUrlCmd
            // 
            TbUrlCmd.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            TbUrlCmd.Location = new System.Drawing.Point(79, 6);
            TbUrlCmd.Name = "TbUrlCmd";
            TbUrlCmd.Size = new System.Drawing.Size(302, 23);
            TbUrlCmd.TabIndex = 4;
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new System.Drawing.Point(8, 9);
            label7.Name = "label7";
            label7.Size = new System.Drawing.Size(62, 15);
            label7.TabIndex = 3;
            label7.Text = "URLコマンド";
            // 
            // CmdFileDialog
            // 
            CmdFileDialog.Filter = "Exeファイル|*.exe";
            CmdFileDialog.Title = "実行ファイル選択";
            // 
            // PGSettingForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            CancelButton = BTCancel;
            ClientSize = new System.Drawing.Size(485, 325);
            Controls.Add(tabControl1);
            Controls.Add(BTCancel);
            Controls.Add(BTOk);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            KeyPreview = true;
            Margin = new System.Windows.Forms.Padding(4);
            Name = "PGSettingForm";
            Text = "PGSettingForm";
            tabControl1.ResumeLayout(false);
            TPMain.ResumeLayout(false);
            TPMain.PerformLayout();
            TabMisc.ResumeLayout(false);
            TabMisc.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Button BTCancel;
        private System.Windows.Forms.Button BTOk;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage TPMain;
        private System.Windows.Forms.TextBox TBConvCmd;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button BTConvSelect;
        private System.Windows.Forms.TextBox TBConvParam;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.OpenFileDialog CmdFileDialog;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox TBConvOutput;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.CheckBox CBConvWait;
        private System.Windows.Forms.TextBox TBConvParamWidth;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TabPage TabMisc;
        private System.Windows.Forms.Button BtnUrlCmd;
        private System.Windows.Forms.TextBox TbUrlCmd;
        private System.Windows.Forms.Label label7;
    }
}