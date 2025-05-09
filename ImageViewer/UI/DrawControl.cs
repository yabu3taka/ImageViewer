using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace ImageViewer.UI
{
    interface IDrawControlLayer
    {
        void DrawLayer(Graphics g, PointConverter conv);
        void NotifyLayerChanged(DrawControl c);
        bool IsOverLayerParts(DrawControl c, MouseEventArgs e);
        IMouseDragEventManager CreateDragEventManager(DrawControl c, MouseEventArgs e);
        void ClickForSelection(DrawControl c, MouseEventArgs e);
    }

    partial class DrawControl : PictureBox
    {
        private Bitmap _curImg = null;
        private double _imgRatio = 1.0;
        private bool _adjust = false;

        public DrawControl()
        {
            InitializeComponent();
        }

        #region Image Setting
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Bitmap CurImg
        {
            get { return _curImg; }
            set
            {
                DisposeImg();
                _curImg = value;
                UpdatePaint(true);
            }
        }

        [Browsable(true)]
        [DefaultValue(typeof(double), "1")]
        public double ImgRatio
        {
            get { return _imgRatio; }
            set
            {
                _imgRatio = value;
                UpdatePaint(false);
            }
        }

        [Browsable(false)]
        public Size CurImgSize
        {
            get
            {
                if (CurImg is null)
                {
                    return new Size(5, 5);
                }
                else
                {
                    return new Size((int)(CurImg.Width * ImgRatio), (int)(CurImg.Height * ImgRatio));
                }
            }
        }

        [Browsable(true)]
        [DefaultValue(typeof(bool), "false")]
        public bool Adjust
        {
            get { return _adjust; }
            set
            {
                _adjust = value;
                this.Dock = _adjust ? DockStyle.Fill : DockStyle.None;
                UpdatePaint(false);
            }
        }

        public void SetImageAndRatio(Bitmap b, double r)
        {
            DisposeImg();
            _curImg = b;
            _imgRatio = r;
            UpdatePaint(true);
        }

        private void DisposeImg()
        {
            if (_curImg is not null)
            {
                StopGifAnimation();
                _curImg.Dispose();
            }
            _curImg = null;
        }

        public PointConverter GetPointConverter()
        {
            int x = 0;
            int y = 0;
            int width, height;
            double ratio;
            if (Adjust && CurImg is not null)
            {
                double ratioWidth = (double)this.Width / CurImg.Width;
                double ratioHeight = (double)this.Height / CurImg.Height;
                if (ratioWidth > ratioHeight)
                {
                    ratio = ratioHeight;
                    width = (int)(CurImg.Width * ratioHeight);
                    x = (this.Width - width) / 2;
                    height = this.Height;
                }
                else
                {
                    ratio = ratioWidth;
                    width = this.Width;
                    height = (int)(CurImg.Height * ratioWidth);
                    y = (this.Height - height) / 2;
                }
            }
            else
            {
                width = this.Width;
                height = this.Height;
                ratio = ImgRatio;
            }
            return new PointConverter(new Point(x, y), new Size(width, height), ratio);
        }
        #endregion

        #region Layer Setting
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IDrawControlLayer ImageLayer { get; set; }

        public void RedrawLayer()
        {
            Invalidate();
        }

        public void NotifyLayerChanged()
        {
            ImageLayer?.NotifyLayerChanged(this);
        }

        public bool IsOverLayerParts(MouseEventArgs e)
        {
            return ImageLayer?.IsOverLayerParts(this, e) ?? false;
        }

        public void ClickForSelection(MouseEventArgs e)
        {
            ImageLayer?.ClickForSelection(this, e);
        }

        public IMouseDragEventManager CreateDragEventManager(MouseEventArgs e)
        {
            return ImageLayer?.CreateDragEventManager(this, e);
        }
        #endregion

        #region Paint
        private void UpdatePaint(bool imgChanged)
        {
            if (_adjust)
            {
                if (imgChanged)
                {
                    Invalidate();
                }
            }
            else
            {
                this.Size = CurImgSize;
                Invalidate();
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);

            if (CurImg is null)
            {
                return;
            }

            var conv = GetPointConverter();
            pe.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            pe.Graphics.DrawImage(CurImg, conv.ImgRect);

            ImageLayer?.DrawLayer(pe.Graphics, conv);
        }
        #endregion

        #region Gif
        public void StartGifAnimation()
        {
            if (ImageAnimator.CanAnimate(_curImg))
            {
                ImageAnimator.Animate(_curImg, Image_FrameChanged);
            }
        }

        public void StopGifAnimation()
        {
            if (ImageAnimator.CanAnimate(_curImg))
            {
                ImageAnimator.StopAnimate(_curImg, Image_FrameChanged);
            }
        }

        private void Image_FrameChanged(object sender, EventArgs e)
        {
            ImageAnimator.UpdateFrames(_curImg);
            Invalidate();
        }
        #endregion
    }
}
