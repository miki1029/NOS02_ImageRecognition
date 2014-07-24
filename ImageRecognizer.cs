using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;

namespace NOS02
{
    class ImageRecognizer
    {
        private Bitmap imageBitmap;
        private SortedSet<Node> nodeSet;
        private List<Edge> edgeList;
        private Dictionary<int, SortedSet<EdgeLine>> edgeLineSetDict;

        public ImageRecognizer(Bitmap b)
        {
            imageBitmap = b;
            nodeSet = new SortedSet<Node>();
            edgeList = new List<Edge>();
            edgeLineSetDict = new Dictionary<int, SortedSet<EdgeLine>>();
        }

        public void RecognizeImage()
        {
            // 노드 구성 및 간선 초기화
            for (int y = 0; y < imageBitmap.Height; y++)
            {
                for (int x = 0; x < imageBitmap.Width; x++)
                {
                    Point curPoint = new Point(x, y);
                    Color curColor = imageBitmap.GetPixel(curPoint.X, curPoint.Y);

                    // 배경
                    if (curColor.ToArgb() == Color.White.ToArgb()) continue;
                    // 간선
                    else if (curColor.ToArgb() == Color.Black.ToArgb())
                    {
                        int jump = JumpSizeExistEdge(curPoint);
                        if (jump == 0)
                        {
                            x += SearchEdge(curPoint) - 1;
                        }
                        else
                        {
                            x += jump - 1;
                        }
                    }
                    // 노드
                    else
                    {
                        int jump = JumpSizeExistNode(curPoint, curColor);
                        if (jump == 0)
                        {
                            x += SearchNode(curPoint, curColor);
                        }
                        else
                        {
                            x += jump;
                        }
                    }
                }
            }
            // 노도의 set을 참고하여 간선 구성
            foreach (Edge item in edgeList)
            {
                // 인접 노드 계산
                item.FindAdjacentNode(nodeSet);
            }
            // 간선 정렬
            edgeList.Sort();
        }

        public void DisposImage()
        {
            imageBitmap.Dispose();
        }

        public enum SearchDirection { UP, DOWN, NOSEARCH }

        private class SeedPointInfo
        {
            private Point seedPoint;
            private int prevLeft;
            private int prevRight;
            private SearchDirection flag;

            public SeedPointInfo(Point s, int pl, int pr, SearchDirection f) {
                seedPoint = s; prevLeft = pl; prevRight = pr; flag = f;
            }
            public Point Seed { get { return seedPoint; } }
            public int PrevLeft { get { return prevLeft; } }
            public int PrevRight { get { return prevRight; } }
            public SearchDirection Flag { get { return flag; } }

        }

        private int SearchEdge(Point startPoint)
        {
            List<Point> adjNodePointList = new List<Point>();

            LineFill(startPoint, 0, 0, SearchDirection.NOSEARCH, adjNodePointList);

            Edge newEdge = new Edge(adjNodePointList);
            edgeList.Add(newEdge);

            return JumpSizeExistEdge(startPoint);
        }
        private void LineFill(Point startSeed, int garbageLeft, int garbageRight, SearchDirection startFlag, List<Point> adjNodePointList)
        {
            Queue<SeedPointInfo> readyQueue = new Queue<SeedPointInfo>();
            readyQueue.Enqueue(new SeedPointInfo(startSeed, garbageLeft, garbageRight, startFlag));

            while(readyQueue.Count > 0)
            {
                SeedPointInfo seedPointInfo = readyQueue.Dequeue();
                Point seedPoint = seedPointInfo.Seed;
                int prevLeft = seedPointInfo.PrevLeft;
                int prevRight = seedPointInfo.PrevRight;
                SearchDirection flag = seedPointInfo.Flag;

                int curLeft = ScanEdgeLineLeft(seedPoint, adjNodePointList).X;
                int curRight = ScanEdgeLineRight(seedPoint, adjNodePointList).X;
                if(DrawHorizonLine(curLeft, curRight, seedPoint.Y))
                {
                    if (flag == SearchDirection.UP)
                    {
                        for (int x = curLeft; x < prevLeft; x++)
                        {
                            Point upPoint = new Point(x, seedPoint.Y - 1);
                            if (IsValidPoint(upPoint))
                            {
                                Color upColor = imageBitmap.GetPixel(upPoint.X, upPoint.Y);
                                // 간선
                                if (upColor.ToArgb() == Color.Black.ToArgb())
                                {
                                    if (!IsExistEdgePoint(upPoint))
                                    {
                                        SeedPointInfo newSeed = new SeedPointInfo(upPoint, curLeft, curRight, SearchDirection.DOWN);
                                        readyQueue.Enqueue(newSeed);
                                        x = ScanEdgeLineRight(upPoint).X;
                                    }
                                }
                                // 배경
                                else if (upColor.ToArgb() == Color.White.ToArgb()) continue;
                                // 노드
                                else
                                {
                                    adjNodePointList.Add(upPoint);
                                }
                            }
                        }
                        for (int x = prevRight + 1; x <= curRight; x++)
                        {
                            Point upPoint = new Point(x, seedPoint.Y - 1);
                            if (IsValidPoint(upPoint))
                            {
                                Color upColor = imageBitmap.GetPixel(upPoint.X, upPoint.Y);
                                // 간선
                                if (upColor.ToArgb() == Color.Black.ToArgb())
                                {
                                    if (!IsExistEdgePoint(upPoint))
                                    {
                                        SeedPointInfo newSeed = new SeedPointInfo(upPoint, curLeft, curRight, SearchDirection.DOWN);
                                        readyQueue.Enqueue(newSeed);
                                        x = ScanEdgeLineRight(upPoint).X;
                                    }
                                }
                                // 배경
                                else if (upColor.ToArgb() == Color.White.ToArgb()) continue;
                                // 노드
                                else
                                {
                                    adjNodePointList.Add(upPoint);
                                }
                            }
                        }
                    }
                    else
                    {
                        for (int x = curLeft; x <= curRight; x++)
                        {
                            Point upPoint = new Point(x, seedPoint.Y - 1);
                            if (IsValidPoint(upPoint))
                            {
                                Color upColor = imageBitmap.GetPixel(upPoint.X, upPoint.Y);
                                // 간선
                                if (upColor.ToArgb() == Color.Black.ToArgb())
                                {
                                    if (!IsExistEdgePoint(upPoint))
                                    {
                                        SeedPointInfo newSeed = new SeedPointInfo(upPoint, curLeft, curRight, SearchDirection.DOWN);
                                        readyQueue.Enqueue(newSeed);
                                        x = ScanEdgeLineRight(upPoint).X;
                                    }
                                }
                                // 배경
                                else if (upColor.ToArgb() == Color.White.ToArgb()) continue;
                                // 노드
                                else
                                {
                                    adjNodePointList.Add(upPoint);
                                }
                            }
                        }
                    }

                    if (flag == SearchDirection.DOWN)
                    {
                        for (int x = curLeft; x < prevLeft; x++)
                        {
                            Point downPoint = new Point(x, seedPoint.Y + 1);
                            if (IsValidPoint(downPoint))
                            {
                                Color downColor = imageBitmap.GetPixel(downPoint.X, downPoint.Y);
                                // 간선
                                if (downColor.ToArgb() == Color.Black.ToArgb())
                                {
                                    if (!IsExistEdgePoint(downPoint))
                                    {
                                        SeedPointInfo newSeed = new SeedPointInfo(downPoint, curLeft, curRight, SearchDirection.UP);
                                        readyQueue.Enqueue(newSeed);
                                        x = ScanEdgeLineRight(downPoint).X;
                                    }
                                }
                                // 배경
                                else if (downColor.ToArgb() == Color.White.ToArgb()) continue;
                                // 노드
                                else
                                {
                                    adjNodePointList.Add(downPoint);
                                }
                            }
                        }
                        for (int x = prevRight + 1; x <= curRight; x++)
                        {
                            Point downPoint = new Point(x, seedPoint.Y + 1);
                            if (IsValidPoint(downPoint))
                            {
                                Color downColor = imageBitmap.GetPixel(downPoint.X, downPoint.Y);
                                // 간선
                                if (downColor.ToArgb() == Color.Black.ToArgb())
                                {
                                    if (!IsExistEdgePoint(downPoint))
                                    {
                                        SeedPointInfo newSeed = new SeedPointInfo(downPoint, curLeft, curRight, SearchDirection.UP);
                                        readyQueue.Enqueue(newSeed);
                                        x = ScanEdgeLineRight(downPoint).X;
                                    }
                                }
                                // 배경
                                else if (downColor.ToArgb() == Color.White.ToArgb()) continue;
                                // 노드
                                else
                                {
                                    adjNodePointList.Add(downPoint);
                                }
                            }
                        }
                    }
                    else
                    {
                        for (int x = curLeft; x <= curRight; x++)
                        {
                            Point downPoint = new Point(x, seedPoint.Y + 1);
                            if (IsValidPoint(downPoint))
                            {
                                Color downColor = imageBitmap.GetPixel(downPoint.X, downPoint.Y);
                                // 간선
                                if (downColor.ToArgb() == Color.Black.ToArgb())
                                {
                                    if (!IsExistEdgePoint(downPoint))
                                    {
                                        SeedPointInfo newSeed = new SeedPointInfo(downPoint, curLeft, curRight, SearchDirection.UP);
                                        readyQueue.Enqueue(newSeed);
                                        x = ScanEdgeLineRight(downPoint).X;
                                    }
                                }
                                // 배경
                                else if (downColor.ToArgb() == Color.White.ToArgb()) continue;
                                // 노드
                                else
                                {
                                    adjNodePointList.Add(downPoint);
                                }
                            }
                        }
                    }
                }

            }

        }

        private Point ScanEdgeLineLeft(Point seedPoint, List<Point> adjNodePointList)
        {
            int x = seedPoint.X - 1;
            for (; x >= 0; x--)
            {
                Point checkPoint = new Point(x, seedPoint.Y);
                Color checkColor = imageBitmap.GetPixel(checkPoint.X, checkPoint.Y);
                // 간선
                if (checkColor.ToArgb() == Color.Black.ToArgb()) continue;
                // 배경
                else if (checkColor.ToArgb() == Color.White.ToArgb()) break;
                // 노드
                else
                {
                    if (adjNodePointList != null)
                        adjNodePointList.Add(checkPoint);
                    break;
                }
            }
            return new Point(x + 1, seedPoint.Y);
        }

        private Point ScanEdgeLineRight(Point seedPoint, List<Point> adjNodePointList = null)
        {
            int x = seedPoint.X + 1;
            for (; x < imageBitmap.Width; x++)
            {
                Point checkPoint = new Point(x, seedPoint.Y);
                Color checkColor = imageBitmap.GetPixel(checkPoint.X, checkPoint.Y);
                // 간선
                if (checkColor.ToArgb() == Color.Black.ToArgb()) continue;
                // 배경
                else if (checkColor.ToArgb() == Color.White.ToArgb()) break;
                // 노드
                else
                {
                    if(adjNodePointList != null)
                        adjNodePointList.Add(checkPoint);
                    break;
                }
            }
            return new Point(x - 1, seedPoint.Y);
        }

        private bool DrawHorizonLine(int left, int right, int y)
        {
            EdgeLine newLine = new EdgeLine(left, right, y);
            SortedSet<EdgeLine> lineSet;
            if (edgeLineSetDict.TryGetValue(y, out lineSet))
            {
                foreach (EdgeLine item in lineSet)
                {
                    // 같은 간선인 경우
                    if (item.LeftPoint.X == newLine.LeftPoint.X)
                    {
                        return false;
                    }
                    // 이어지는 간선일 경우
                    else if (item.LeftPoint.X + item.Width == newLine.LeftPoint.X)
                    {
                        item.Width += newLine.Width;
                        return true;
                    }
                    // 다른 간선인 경우
                    else if (item.LeftPoint.X + item.Width > newLine.LeftPoint.X) break;
                }
                lineSet.Add(newLine);
            }
            else
            {
                lineSet = new SortedSet<EdgeLine>();
                lineSet.Add(newLine);
                edgeLineSetDict.Add(newLine.LeftPoint.Y, lineSet);
            }
            return true;
        }

        private bool IsValidPoint(Point p)
        {
            if (p.X < 0 || p.X >= imageBitmap.Width || p.Y < 0 || p.Y >= imageBitmap.Height) return false;
            else return true;
        }

        private int JumpSizeExistEdge(Point searchPoint)
        {
            SortedSet<EdgeLine> findSet;
            if (edgeLineSetDict.TryGetValue(searchPoint.Y, out findSet))
            {
                foreach (EdgeLine item in findSet)
                {
                    if (searchPoint.X >= item.LeftPoint.X && searchPoint.X < item.LeftPoint.X + item.Width)
                    {
                        int jump = item.Width - (searchPoint.X - item.LeftPoint.X);
                        return jump;
                    }
                    else if (item.LeftPoint.X > searchPoint.X) break;
                }
            }
            return 0;
        }

        private bool IsExistEdgePoint(Point searchPoint)
        {
            SortedSet<EdgeLine> findSet;
            if (edgeLineSetDict.TryGetValue(searchPoint.Y, out findSet))
            {
                foreach (EdgeLine item in findSet)
                {
                    if (searchPoint.X >= item.LeftPoint.X && searchPoint.X < item.LeftPoint.X + item.Width)
                    {
                        return true;
                    }
                    else if (item.LeftPoint.X > searchPoint.X) break;
                }
            }
            return false;
        }

        private int SearchNode(Point startPoint, Color curColor)
        {
            int findX = startPoint.X + 1;
            int findY = startPoint.Y + 1;

            for (; findX < imageBitmap.Width; findX++)
            {
                Color findRGB = imageBitmap.GetPixel(findX, startPoint.Y);
                if (findRGB.ToArgb() != curColor.ToArgb()) break;
            }

            for (; findY < imageBitmap.Height; findY++)
            {
                Color findRGB = imageBitmap.GetPixel(startPoint.X, findY);
                if (findRGB.ToArgb() != curColor.ToArgb()) break;
            }

            int width = findX - startPoint.X;
            int height = findY - startPoint.Y;

            // 가로 세로는 최소 2px
            if (width >= 2 && height >= 2)
            {
                Node newNode = new Node(width, height, curColor, startPoint);
                nodeSet.Add(newNode);
            }

            return width - 1;
        }

        private int JumpSizeExistNode(Point searchPoint, Color curColor)
        {
            // 좌상단점 조사
            int findY = searchPoint.Y - 1;

            for (; findY >= 0; findY--)
            {
                Color findRGB = imageBitmap.GetPixel(searchPoint.X, findY);
                if (findRGB.ToArgb() != curColor.ToArgb()) break;
            }

            int goToTop = searchPoint.Y - findY - 1;

            // 현재가 최상단점인 경우(새로운 노드)
            if (goToTop == 0) return 0;
            // 이미 존재하는 노드인 경우
            else
            {
                searchPoint.Y -= goToTop;
                // 어떤 노드인지 확인하여 width-1 값만큼 jump
                foreach (Node item in nodeSet)
                {
                    if (item.Pos == searchPoint)
                    {
                        return item.Width - 1;
                    }
                }
            }
            throw new NullReferenceException();
        }

        public void PrintOutput()
        {
            StreamWriter sw = new StreamWriter("output.txt");
            sw.WriteLine("{0} {1}", nodeSet.Count, edgeList.Count);

            foreach (Node item in nodeSet)
            {
                sw.WriteLine(item);
            }

            foreach (Edge item in edgeList)
            {
                sw.WriteLine(item);
            }

            sw.Close();
        }

    }
}
