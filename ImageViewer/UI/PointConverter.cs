using System;
using System.Drawing;

using ImageViewer.Util;

namespace ImageViewer.UI
{
    class PointConverter(Point location, Size imgSize, double ratio)
    {
        public Point Location { get; } = location;
        public Size ImgSize { get; } = imgSize;
        public double Ratio { get; } = ratio;

        public Rectangle ImgRect => new(Location, ImgSize);

        public Point ToClientPoint(Point p)
        {
            return GeometryUtil.Add(p.Multiplicate(Ratio), Location);
        }

        public Point? ToImagePoint(Point p)
        {
            if (p.X < Location.X)
            {
                return null;
            }
            if (p.Y < Location.Y)
            {
                return null;
            }

            p = GeometryUtil.Subtract(p, Location);

            if (p.X > ImgSize.Width)
            {
                return null;
            }
            if (p.Y > ImgSize.Height)
            {
                return null;
            }

            return p.Division(Ratio);
        }
    }
}
