using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace ImageViewer.Util
{
    static class GeometryUtil
    {
        public static bool IsNear(this Point p, int distance)
        {
            return Math.Abs(p.X) < distance && Math.Abs(p.Y) < distance;
        }
        public static bool IsNear(this Point p, Point to, int distance)
        {
            return Math.Abs(p.X - to.X) < distance && Math.Abs(p.Y - to.Y) < distance;
        }

        public static Point Add(Point p1, Point p2)
        {
            return new Point(p1.X + p2.X, p1.Y + p2.Y);
        }
        public static Point Subtract(Point p1, Point p2)
        {
            return new Point(p1.X - p2.X, p1.Y - p2.Y);
        }
        public static Point Multiplicate(this Point p, double m)
        {
            return new Point(Convert.ToInt32(p.X * m), Convert.ToInt32(p.Y * m));
        }
        public static Point Division(this Point p, double d)
        {
            return new Point(Convert.ToInt32(p.X / d), Convert.ToInt32(p.Y / d));
        }

        public static double Distance(this Point p1, Point p2)
        {
            return Math.Pow((p2.X - p1.X) * (p2.X - p1.X) + (p2.Y - p1.Y) * (p2.Y - p1.Y), 0.5);
        }

        public static double DistanceFromLine(this Point p, Point lineStart, Point lineEnd)
        {
            var a = lineEnd.X - lineStart.X;
            var b = lineEnd.Y - lineStart.Y;
            var a2 = a * a;
            var b2 = b * b;
            var r2 = a2 + b2;
            var tt = -(a * (lineStart.X - p.X) + b * (lineStart.Y - p.Y));
            if (tt < 0)
            {
                return p.Distance(lineStart);
            }
            if (tt > r2)
            {
                return p.Distance(lineEnd);
            }
            var f1 = a * (lineStart.Y - p.Y) - b * (lineStart.X - p.X);
            return Math.Pow((f1 * f1) / r2, 0.5);
        }

        public static Rectangle CircleRectangle(this Point p, int size)
        {
            return new Rectangle(p.X - size, p.Y - size, size * 2, size * 2);
        }

        public static Rectangle BoundRectangle(IEnumerable<Point> points)
        {
            var r = new Rectangle();
            {
                var xList = points.Select(p => p.X);
                r.X = xList.Min();
                r.Width = xList.Max() - r.X;
            }
            {
                var yList = points.Select(p => p.Y);
                r.Y = yList.Min();
                r.Height = yList.Max() - r.Y;
            }
            return r;
        }

        public static Rectangle BoundRectangle(params Point[] points)
        {
            return BoundRectangle(points.AsEnumerable());
        }

        public static Size LimitSize(this Size imgSize, Size maxSize)
        {
            double ratioWidth = (double)maxSize.Width / imgSize.Width;
            double ratioHeight = (double)maxSize.Height / imgSize.Height;
            int width, height;
            if (ratioWidth > ratioHeight)
            {
                width = (int)(imgSize.Width * ratioHeight);
                height = maxSize.Height;
            }
            else
            {
                width = maxSize.Width;
                height = (int)(imgSize.Height * ratioWidth);
            }
            return new Size(width, height);
        }

        public static Size ScaleDown(this Size s, int d)
        {
            return new Size(s.Width / d, s.Height / d);
        }

        public static String ToDimString(this Size s)
        {
            return $"({s.Width},{s.Height})";
        }
    }
}
