using System;
using System.IO;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace ImageConverter.Util
{
    static partial class FormUtil
    {
        #region Message
        public static void ShowError(Form w, string mess)
        {
            MessageBox.Show(w, mess, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public static void ShowInfo(Form w, string mess)
        {
            MessageBox.Show(w, mess, "", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public static bool Confirm(Form w, string mess)
        {
            return MessageBox.Show(w, mess, "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;
        }
        #endregion

        #region Menu
        public static void SetMenuItemAvailable(ToolStripMenuItem item, bool available)
        {
            item.Enabled = available;
            item.Available = available;
            if (item.HasDropDownItems)
            {
                foreach (var subitem in item.DropDownItems)
                {
                    if (subitem is ToolStripMenuItem menuItem)
                    {
                        SetMenuItemAvailable(menuItem, available);
                    }
                }
            }
        }

        public static void ClearMenuItems(ToolStripItemCollection items, ToolStripItem endItem)
        {
            for (int i = items.Count - 1; i >= 0; --i)
            {
                if (items[i] == endItem)
                {
                    break;
                }
                items.RemoveAt(i);
            }
        }
        #endregion

        #region Control
        public static void SelectFileNameExceptFileName(TextBox c)
        {
            string ext = Path.GetExtension(c.Text);
            c.Select(0, c.Text.Length - ext.Length);
        }

        public static int GetWheelTick(MouseEventArgs e)
        {
            return e.Delta / 120;
        }
        #endregion

        #region Validation
        public static void ShowValidationError(Control c, string mess)
        {
            MessageBox.Show(c, mess, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            c.Select();
        }

        public static bool CheckRequired(TextBoxBase t, string mess)
        {
            if (string.IsNullOrEmpty(t.Text))
            {
                ShowValidationError(t, mess);
                return false;
            }
            return true;
        }

        public static bool CheckFileExist(TextBoxBase t, string mess)
        {
            if (!File.Exists(t.Text))
            {
                ShowValidationError(t, mess);
                return false;
            }
            return true;
        }
        #endregion

        #region ListView
        public static int MakeLong(short lowPart, short highPart)
        {
            return (int)(((ushort)lowPart) | (uint)(highPart << 16));
        }

        [LibraryImport("user32.dll")]
        public static partial int SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

        public static void ListViewItem_SetSpacing(ListView listview, int leftPadding, int topPadding)
        {
            const int LVM_FIRST = 0x1000;
            const int LVM_SETICONSPACING = LVM_FIRST + 53;
            int v = MakeLong((short)leftPadding, (short)topPadding);
            SendMessage(listview.Handle, LVM_SETICONSPACING, IntPtr.Zero, (IntPtr)v);
        }
        #endregion

        #region Open
        public static bool IsBrowserable(string file)
        {
            if (Uri.IsWellFormedUriString(file, UriKind.Absolute))
            {
                return true;
            }
            if (Path.IsPathRooted(file))
            {
                if (file.EndsWith(".url", StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }

        public static void OpenFile(string file)
        {
            if (IsBrowserable(file))
            {
                var setting = AppSetting.Default;
                if (!string.IsNullOrEmpty(setting.UrlCommand))
                {
                    var pi2 = new ProcessStartInfo()
                    {
                        FileName = setting.UrlCommand,
                        Arguments = '"' + file + '"',
                        UseShellExecute = true,
                        Verb = "open"
                    };
                    Process.Start(pi2);
                    return;
                }
            }
            var pi = new ProcessStartInfo()
            {
                FileName = file,
                UseShellExecute = true,
                Verb = "open"
            };
            Process.Start(pi);
        }
        #endregion

        #region Form
        public static F CreateFormIfNeed<F>(Form parent, F form, Func<F> func) where F : Form
        {
            if (form is null || form.IsDisposed)
            {
                form = func();
                form.Show(parent);
            }
            return form;
        }

        public static void ChangeFormIfValid<F>(F form, Action<F> a) where F : Form
        {
            if (!(form is null || form.IsDisposed))
            {
                a(form);
            }
        }
        #endregion
    }
}
