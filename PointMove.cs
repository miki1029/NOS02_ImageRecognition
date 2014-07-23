using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace NOS02
{
    class PointMove
    {
        public static Point GetUp(Point p)
        {
            return new Point(p.X, p.Y - 1);
        }
        public static Point GetDown(Point p)
        {
            return new Point(p.X, p.Y + 1);
        }
        public static Point GetLeft(Point p)
        {
            return new Point(p.X - 1, p.Y);
        }
        public static Point GetRight(Point p)
        {
            return new Point(p.X + 1, p.Y);
        }
    }
}
