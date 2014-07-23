using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace NOS02
{
    class Node : IComparable<Node>
    {
        private int width, height;
        private Color color;
        private Point pos; // 좌상단 위치

        public Node(int w, int h, Color c, Point p)
        {
            width = w; height = h; color = c; pos = p;
        }

        public int Width
        {
            get { return width; }
        }
        public int Height
        {
            get { return height; }
        }
        public Color RGB
        {
            get { return color; }
        }
        public byte R
        {
            get { return color.R; }
        }
        public byte G
        {
            get { return color.G; }
        }
        public byte B
        {
            get { return color.B; }
        }
        public Point Pos
        {
            get { return pos; }
        }
        public int X
        {
            get { return pos.X; }
        }
        public int Y
        {
            get { return pos.Y; }
        }

        public bool IsInsidePoint(Point p)
        {
            Point leftTop = pos;
            Point rightBottom = new Point(pos.X + Width - 1, pos.Y + Height - 1);
            if (p.X >= leftTop.X && p.X <= rightBottom.X && p.Y >= leftTop.Y && p.Y <= rightBottom.Y)
                return true;
            else
                return false;
        }
        public override string ToString()
        {
            return Width + " " + Height + " " + R + " " + G + " " + B;
        }
        public int CompareTo(Node other)
        {
            int cmp = Y.CompareTo(other.Y);
            if (cmp == 0)
            {
                cmp = X.CompareTo(other.X);
                if (cmp == 0)
                {
                    cmp = R.CompareTo(other.R);
                    if (cmp == 0)
                    {
                        cmp = G.CompareTo(other.G);
                        if (cmp == 0)
                        {
                            cmp = B.CompareTo(other.B);
                        }
                    }
                }
            }
            return cmp;
        }
    }
}
