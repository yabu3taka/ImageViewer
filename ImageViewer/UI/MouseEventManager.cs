using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using ImageViewer.Util;

namespace ImageViewer.UI
{
    interface IMouseDragEventManager
    {
        void DragMoveStart(object sender, Point startPoint);
        void DragMove(object sender, MouseEventArgs e, Point diff);
        void DragMoveEnd(object sender, MouseEventArgs e);
    }

    class MouseEventManager
    {
        /* Setting */
        public Cursor DragCursor { get; set; } = Cursors.Hand;

        /* Event */
        public event Action<object, int> Rocker;
        public event Action<object, MouseEventArgs> MouseClick;
        public event Action<object, MouseEventArgs> MouseDoubleClick;

        /* Status */
        public bool NormalMove
        {
            get { return _draggingManger is null && !_roker; }
        }

        /* Process */
        private const int NEAR_DISTANCE = 5;

        private bool _pressed = false;
        private bool _roker = false;

        private MouseButtons _pushed = MouseButtons.None;
        private Point _downPos;

        private IMouseDragEventManager _draggingManger = null;
        private bool _draggingStarted = false;

        private int _clickCount = 0;

        public void MouseDown(object sender, MouseEventArgs e, IMouseDragEventManager dragManager)
        {
            _clickCount = e.Clicks;

            if (_pressed)
            {
                // 右→左、左→右クリックを検出
                Rocker?.Invoke(sender, _pushed == MouseButtons.Left ? 1 : -1);
                _roker = true;
            }
            else
            {
                _pressed = true;
                _roker = false;

                _pushed = e.Button;
                _downPos = ((Control)sender).PointToScreen(e.Location);

                // Dragは左のみ
                if (e.Button != MouseButtons.Left)
                {
                    dragManager = null;
                }

                _draggingManger = dragManager;
                _draggingStarted = false;
            }
        }

        public void MouseMove(object sender, MouseEventArgs e)
        {
            if (_roker)
            {
                return;
            }

            if (_draggingManger is not null)
            {
                var dragFirst = !_draggingStarted;
                var delta = GeometryUtil.Subtract(((Control)sender).PointToScreen(e.Location), _downPos);
                if (!delta.IsNear(NEAR_DISTANCE))
                {
                    _draggingStarted = true;
                    Cursor.Current = DragCursor;
                }

                if (_draggingStarted)
                {
                    if (dragFirst)
                    {
                        _draggingManger.DragMoveStart(sender, _downPos);
                    }
                    _draggingManger.DragMove(sender, e, delta);
                }
            }
        }

        public void MouseUp(object sender, MouseEventArgs e)
        {
            if (_pushed == e.Button)
            {
                var mousePos = ((Control)sender).PointToScreen(e.Location);
                if (!_roker && _downPos.IsNear(mousePos, NEAR_DISTANCE))
                {
                    if (_clickCount <= 1)
                    {
                        if (_draggingStarted)
                        {
                            _draggingManger.DragMoveEnd(sender, e);
                        }
                        else
                        {
                            MouseClick?.Invoke(sender, e);
                        }
                    }
                    else
                    {
                        // ダブルクリックは左のみ
                        if (e.Button == MouseButtons.Left)
                        {
                            MouseDoubleClick?.Invoke(sender, e);
                        }
                    }
                }
                _pressed = false;
                _pushed = MouseButtons.None;
                _roker = false;
            }

            _draggingManger = null;
            _draggingStarted = false;
            Cursor.Current = Cursors.Default;
        }
    }

    class MouseDragScrollManager : IMouseDragEventManager
    {
        private readonly ScrollableControl _control;
        private Point _scrollPos;

        private MouseDragScrollManager(ScrollableControl c)
        {
            _control = c;
            _scrollPos = c.AutoScrollPosition;
        }

        public static MouseDragScrollManager Create(ScrollableControl c)
        {
            if (c.HorizontalScroll.Visible || c.VerticalScroll.Visible)
            {
                return new MouseDragScrollManager(c);
            }
            else
            {
                return null;
            }
        }

        public void DragMoveStart(object sender, Point startPoint)
        {
        }

        public void DragMove(object sender, MouseEventArgs e, Point diff)
        {
            _control.AutoScrollPosition = new Point(-_scrollPos.X - diff.X, -_scrollPos.Y - diff.Y);
        }

        public void DragMoveEnd(object sender, MouseEventArgs e)
        {
        }
    }
}
