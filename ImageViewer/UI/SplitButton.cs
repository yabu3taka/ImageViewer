using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace ImageViewer.UI
{
    public partial class SplitButton : Button
    {
        [Browsable(true)]
        [DefaultValue(null)]
        public ContextMenuStrip SubMenu { get; set; }

        [Browsable(true)]
        [DefaultValue(20)]
        public int SplitWidth { get; set; }

        public SplitButton()
        {
            SplitWidth = 20;
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            var splitRect = new Rectangle(this.Width - this.SplitWidth, 0, this.SplitWidth, this.Height);

            if (SubMenu is not null &&
                e.Button == MouseButtons.Left &&
                splitRect.Contains(e.Location))
            {
                SubMenu.Show(this, 0, this.Height);
            }
            else
            {
                base.OnMouseDown(e);
            }
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);

            if (this.SubMenu is not null && this.SplitWidth > 0)
            {
                int arrowX = ClientRectangle.Width - 14;
                int arrowY = ClientRectangle.Height / 2 - 1;

                var arrowBrush = Enabled ? SystemBrushes.ControlText : SystemBrushes.ButtonShadow;
                var arrows = new Point[] {
                    new(arrowX, arrowY),
                    new(arrowX + 7, arrowY),
                    new(arrowX + 3, arrowY + 4)
                };
                pe.Graphics.FillPolygon(arrowBrush, arrows);

                int lineX = ClientRectangle.Width - this.SplitWidth;
                int lineYFrom = arrowY - 4;
                int lineYTo = arrowY + 8;
                using var separatorPen = new Pen(Brushes.DarkGray) { DashStyle = DashStyle.Dot };
                pe.Graphics.DrawLine(separatorPen, lineX, lineYFrom, lineX, lineYTo);
            }
        }
    }
}
