using System.Windows.Forms;

namespace ImageViewer
{
    partial class TextForm : Form
    {
        public TextForm(string str)
        {
            InitializeComponent();

            TBPhotoText.Text = str;
            TBPhotoText.Select(0, 0);
        }

        public string PhotoText { get { return TBPhotoText.Text; } }

        private void TextForm_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.F1:
                    BTOk.PerformClick();
                    break;
            }
        }
    }
}
