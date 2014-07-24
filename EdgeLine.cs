using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace NOS02
{
    class EdgeLine : IComparable<EdgeLine>
    {
        private Point leftPoint;
        private int width;

        public EdgeLine(Point lp, Point rp)
        {
            leftPoint = lp;
            width = rp.X - lp.X + 1;
        }
        public EdgeLine(int left, int right, int y)
        {
            leftPoint = new Point(left, y);
            width = right - left + 1;
        }

        public Point LeftPoint
        {
            get { return leftPoint; }
        }

        public int Width
        {
            get { return width; }
            set { width = value; }
        }

        public int CompareTo(EdgeLine other)
        {
            return leftPoint.X.CompareTo(other.leftPoint.X);
        }
    }
}
