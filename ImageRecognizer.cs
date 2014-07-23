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
        private Dictionary<Point, int> edgeLineDict;

        public ImageRecognizer(Bitmap b)
        {
            imageBitmap = b;
            nodeSet = new SortedSet<Node>();
            edgeList = new List<Edge>();
            edgeLineDict = new Dictionary<Point, int>();
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
                            x += SearchEdge(curPoint);
                        }
                        else
                        {
                            x += jump;
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

        private enum SearchDirection { UP, DOWN, UPDOWN, NOSEARCH }

        private int SearchEdge(Point startPoint)
        {
            Queue<Point> downSearchQueue = new Queue<Point>();
            Queue<Point> upDownSearchQueue = new Queue<Point>();
            Queue<Point> upSearchQueue = new Queue<Point>();

            List<Point> adjNodePointList = new List<Point>();

            downSearchQueue.Enqueue(startPoint);

            while (downSearchQueue.Count > 0 && upDownSearchQueue.Count > 0 && upSearchQueue.Count > 0)
            {
                if (downSearchQueue.Count > 0)
                {
                    Point curPoint = downSearchQueue.Dequeue();

                    Point leftPoint = ScanEdgeLineLeft(curPoint, SearchDirection.DOWN,
                        upSearchQueue, downSearchQueue, upDownSearchQueue, adjNodePointList);
                    Point rightPoint = ScanEdgeLineRight(curPoint, SearchDirection.DOWN,
                        upSearchQueue, downSearchQueue, upDownSearchQueue, adjNodePointList);

                    int width = rightPoint.X - leftPoint.X + 1;

                    edgeLineDict.Add(curPoint, width);

                    // 아랫점 조사
                    Point downPoint = new Point(curPoint.X, curPoint.Y + 1);
                    Color downColor = imageBitmap.GetPixel(downPoint.X, downPoint.Y);
                    if (downColor.ToArgb() == Color.Black.ToArgb())
                    {
                        Point downLeftPoint = ScanEdgeLineLeft(curPoint, SearchDirection.NOSEARCH,
                            upSearchQueue, downSearchQueue, upDownSearchQueue, adjNodePointList);
                        Point downRightPoint = ScanEdgeLineRight(curPoint, SearchDirection.NOSEARCH,
                            upSearchQueue, downSearchQueue, upDownSearchQueue, adjNodePointList);

                        if (downLeftPoint.X <= leftPoint.X || downRightPoint.X >= rightPoint.X)
                            upDownSearchQueue.Enqueue(downPoint);
                        else
                            downSearchQueue.Enqueue(downPoint);
                    }
                }

                if (upDownSearchQueue.Count > 0)
                {
                    Point curPoint = upDownSearchQueue.Dequeue();

                    Point leftPoint = ScanEdgeLineLeft(curPoint, SearchDirection.UP,
                        upSearchQueue, downSearchQueue, upDownSearchQueue, adjNodePointList);
                    Point rightPoint = ScanEdgeLineRight(curPoint, SearchDirection.UP,
                        upSearchQueue, downSearchQueue, upDownSearchQueue, adjNodePointList);

                    int width = rightPoint.X - leftPoint.X + 1;

                    edgeLineDict.Add(curPoint, width);

                    // 윗점 조사
                    Point upPoint = new Point(curPoint.X, curPoint.Y - 1);
                    Color upColor = imageBitmap.GetPixel(upPoint.X, upPoint.Y);
                    if (upColor.ToArgb() == Color.Black.ToArgb())
                    {
                        Point upLeftPoint = ScanEdgeLineLeft(curPoint, SearchDirection.NOSEARCH,
                            upSearchQueue, downSearchQueue, upDownSearchQueue, adjNodePointList);
                        Point upRightPoint = ScanEdgeLineRight(curPoint, SearchDirection.NOSEARCH,
                            upSearchQueue, downSearchQueue, upDownSearchQueue, adjNodePointList);

                        if (upLeftPoint.X <= leftPoint.X || upRightPoint.X >= rightPoint.X)
                            upDownSearchQueue.Enqueue(upPoint);
                        else
                            upSearchQueue.Enqueue(upPoint);
                    }
                }

                if (upSearchQueue.Count > 0)
                {
                    Point curPoint = upSearchQueue.Dequeue();

                    Point leftPoint = ScanEdgeLineLeft(curPoint, SearchDirection.UPDOWN,
                        upSearchQueue, downSearchQueue, upDownSearchQueue, adjNodePointList);
                    Point rightPoint = ScanEdgeLineRight(curPoint, SearchDirection.UPDOWN,
                        upSearchQueue, downSearchQueue, upDownSearchQueue, adjNodePointList);

                    int width = rightPoint.X - leftPoint.X + 1;

                    edgeLineDict.Add(curPoint, width);

                    // 아랫점 조사
                    Point downPoint = new Point(curPoint.X, curPoint.Y + 1);
                    Color downColor = imageBitmap.GetPixel(downPoint.X, downPoint.Y);
                    if (downColor.ToArgb() == Color.Black.ToArgb())
                    {
                        Point downLeftPoint = ScanEdgeLineLeft(curPoint, SearchDirection.NOSEARCH,
                            upSearchQueue, downSearchQueue, upDownSearchQueue, adjNodePointList);
                        Point downRightPoint = ScanEdgeLineRight(curPoint, SearchDirection.NOSEARCH,
                            upSearchQueue, downSearchQueue, upDownSearchQueue, adjNodePointList);

                        if (downLeftPoint.X <= leftPoint.X || downRightPoint.X >= rightPoint.X)
                            upDownSearchQueue.Enqueue(downPoint);
                        else
                            downSearchQueue.Enqueue(downPoint);
                    }

                    // 윗점 조사
                    Point upPoint = new Point(curPoint.X, curPoint.Y - 1);
                    Color upColor = imageBitmap.GetPixel(upPoint.X, upPoint.Y);
                    if (upColor.ToArgb() == Color.Black.ToArgb())
                    {
                        Point upLeftPoint = ScanEdgeLineLeft(curPoint, SearchDirection.NOSEARCH,
                            upSearchQueue, downSearchQueue, upDownSearchQueue, adjNodePointList);
                        Point upRightPoint = ScanEdgeLineRight(curPoint, SearchDirection.NOSEARCH,
                            upSearchQueue, downSearchQueue, upDownSearchQueue, adjNodePointList);

                        if (upLeftPoint.X <= leftPoint.X || upRightPoint.X >= rightPoint.X)
                            upDownSearchQueue.Enqueue(upPoint);
                        else
                            upSearchQueue.Enqueue(upPoint);
                    }
                }

            }

            Edge newEdge = new Edge(adjNodePointList);
            edgeList.Add(newEdge);

            return JumpSizeExistEdge(startPoint);
        }

        private Point ScanEdgeLineLeft(Point seedPoint, SearchDirection direction,
            Queue<Point> upSearchQueue, Queue<Point> downSearchQueue, Queue<Point> upDownSearchQueue, List<Point> adjNodePointList)
        {
            bool continuous = false;
            Point dummyPoint = new Point(-1, -1);
            Point newSeedPoint = dummyPoint;
            Queue<Point> searchQueue = upDownSearchQueue;

            int x = seedPoint.X - 1;
            for (; x >= 0; x--)
            {
                Point checkPoint = new Point(x, seedPoint.Y);
                Color checkColor = imageBitmap.GetPixel(checkPoint.X, checkPoint.Y);
                // 간선
                if (checkColor.ToArgb() == Color.Black.ToArgb())
                {
                    if (direction == SearchDirection.NOSEARCH) break;

                    // 상 또는 하 탐색점 선정
                    Point searchPoint = dummyPoint;
                    Point searchPoint2 = dummyPoint;

                    switch (direction)
                    {
                        case SearchDirection.UP:
                            searchPoint = new Point(x, seedPoint.Y - 1);
                            searchQueue = upSearchQueue;
                            break;
                        case SearchDirection.DOWN:
                            searchPoint = new Point(x, seedPoint.Y + 1);
                            searchQueue = downSearchQueue;
                            break;
                        case SearchDirection.UPDOWN:
                            searchPoint = new Point(x, seedPoint.Y - 1);
                            searchPoint2 = new Point(x, seedPoint.Y + 1);
                            searchQueue = upSearchQueue;
                            break;
                        default:
                            throw new IndexOutOfRangeException();
                    }

                    // 탐색점 색 확인 (UPDOWN인 경우만 2회 탐색)
                    for(int i = 0; i < 2; i++)
                    {
                        Color searchColor = imageBitmap.GetPixel(searchPoint.X, searchPoint.Y);
                        // 상하 탐색점이 간선
                        if (searchColor.ToArgb() == Color.Black.ToArgb())
                        {
                            if (!continuous)
                            {
                                continuous = true;
                                if (newSeedPoint != dummyPoint)
                                    searchQueue.Enqueue(newSeedPoint);
                                newSeedPoint = searchPoint;
                            }
                        }
                        // 상하 탐색점이 배경
                        else if (searchColor.ToArgb() == Color.White.ToArgb())
                        {
                            continuous = false;
                        }
                        // 상하 탐색점이 노드
                        else
                        {
                            continuous = false;
                            adjNodePointList.Add(searchPoint);
                        }
                        // UPDOWN이 아니면 빠져나오고 UPDOWN이면 조건을 바꿈
                        if (searchPoint2 == dummyPoint) break;
                        else
                        {
                            searchPoint = searchPoint2;
                            searchQueue = downSearchQueue;
                        }
                    }
                }
                // 배경
                else if (checkColor.ToArgb() == Color.White.ToArgb()) break;
                // 노드
                else
                {
                    adjNodePointList.Add(checkPoint);
                    break;
                }
            }

            // 마지막 seedPoint 추가
            if (newSeedPoint != dummyPoint)
            {
                // 끝 점인데 상(또는 하) 방향에 인접 간선 점이 있을 경우
                if (continuous)
                {
                    upDownSearchQueue.Enqueue(newSeedPoint);
                }
                else
                {
                    searchQueue.Enqueue(newSeedPoint);
                }
            }
            return new Point(x + 1, seedPoint.Y);
        }

        private Point ScanEdgeLineRight(Point seedPoint, SearchDirection direction,
            Queue<Point> upSearchQueue, Queue<Point> downSearchQueue, Queue<Point> upDownSearchQueue, List<Point> adjNodePointList)
        {
            bool continuous = false;
            Point dummyPoint = new Point(-1, -1);
            Point newSeedPoint = dummyPoint;

            int x = seedPoint.X + 1; // 현재 점 오른 쪽 부터
            for (; x < imageBitmap.Width; x++)
            {
                Point checkPoint = new Point(x, seedPoint.Y);
                Color checkColor = imageBitmap.GetPixel(checkPoint.X, checkPoint.Y);
                // 간선
                if (checkColor.ToArgb() == Color.Black.ToArgb())
                {
                    Point searchPoint;
                    Color searchColor;
                    // 상하 탐색(탐색 방향에 따라)
                    switch (direction)
                    {
                        case SearchDirection.UP:
                            searchPoint = new Point(x, seedPoint.Y - 1);
                            break;
                        case SearchDirection.DOWN:
                            searchPoint = new Point(x, seedPoint.Y + 1);
                            break;
                        case SearchDirection.UPDOWN:
                            searchPoint = new Point(x, seedPoint.Y - 1);
                            // UPDOWN의 경우 두 개의 점을 봐야 함.
                            searchColor = imageBitmap.GetPixel(searchPoint.X, searchPoint.Y);

                            // 상하 탐색점이 간선
                            if (searchColor.ToArgb() == Color.Black.ToArgb())
                            {
                                if (!continuous)
                                {
                                    continuous = true;
                                    if (newSeedPoint != dummyPoint)
                                        upSearchQueue.Enqueue(newSeedPoint);
                                    newSeedPoint = searchPoint;
                                }
                            }
                            // 상하 탐색점이 배경
                            else if (searchColor.ToArgb() == Color.White.ToArgb())
                            {
                                continuous = false;
                            }
                            // 상하 탐색점이 노드
                            else
                            {
                                continuous = false;
                                adjNodePointList.Add(searchPoint);
                            }
                            searchPoint = new Point(x, seedPoint.Y + 1);
                            break;
                        default:
                            throw new IndexOutOfRangeException();
                    }
                    searchColor = imageBitmap.GetPixel(searchPoint.X, searchPoint.Y);

                    // 상하 탐색점이 간선
                    if (searchColor.ToArgb() == Color.Black.ToArgb())
                    {
                        if (!continuous)
                        {
                            continuous = true;
                            if (newSeedPoint != dummyPoint)
                                upSearchQueue.Enqueue(newSeedPoint);
                            newSeedPoint = searchPoint;
                        }
                    }
                    // 상하 탐색점이 배경
                    else if (searchColor.ToArgb() == Color.White.ToArgb())
                    {
                        continuous = false;
                    }
                    // 상하 탐색점이 노드
                    else
                    {
                        continuous = false;
                        adjNodePointList.Add(searchPoint);
                    }
                }
                // 배경
                else if (checkColor.ToArgb() == Color.White.ToArgb()) break;
                // 노드
                else
                {
                    adjNodePointList.Add(checkPoint);
                    break;
                }
            }

            // 끝 점인데 상(또는 하) 방향에 인접 간선 점이 있을 경우
            if (continuous && newSeedPoint != dummyPoint)
            {
                upDownSearchQueue.Enqueue(newSeedPoint);
            }
            return new Point(x + 1, seedPoint.Y);
        }

        private bool IsValidPoint(Point p)
        {
            if (p.X < 0 || p.X >= imageBitmap.Width || p.Y < 0 || p.Y >= imageBitmap.Height) return false;
            else return true;
        }

        private int JumpSizeExistEdge(Point searchPoint)
        {
            int width;
            if (edgeLineDict.TryGetValue(searchPoint, out width))
                return width - 1;
            else
               return 0;
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
