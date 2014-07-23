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
        private HashSet<Point> edgePointSet; // 간선 중 탐색한 Point의 정보를 저장

        public ImageRecognizer(Bitmap b)
        {
            imageBitmap = b;
            nodeSet = new SortedSet<Node>();
            edgeList = new List<Edge>();
            edgePointSet = new HashSet<Point>();
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
                        // 이미 탐색한 적이 있으면 넘어감
                        if (edgePointSet.Contains(curPoint)) continue;
                        else SearchEdge(curPoint);
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

        private void SearchEdge(Point startPoint)
        {
            Queue<Point> readyQueue = new Queue<Point>();
            List<Point> adjNodePointList = new List<Point>();

            readyQueue.Enqueue(startPoint);
            edgePointSet.Add(startPoint);

            while(readyQueue.Count > 0)
            {
                Point curPoint = readyQueue.Dequeue();

                CheckEdge(PointMove.GetUp(curPoint), readyQueue, adjNodePointList);
                CheckEdge(PointMove.GetDown(curPoint), readyQueue, adjNodePointList);
                CheckEdge(PointMove.GetLeft(curPoint), readyQueue, adjNodePointList);
                CheckEdge(PointMove.GetRight(curPoint), readyQueue, adjNodePointList);
            }

            Edge newEdge = new Edge(adjNodePointList);
            edgeList.Add(newEdge);
        }

        private void CheckEdge(Point checkPoint, Queue<Point> readyQueue, List<Point> adjNodePointList)
        {
            // 이미 탐색한 적이 있으면 넘어감
            if (edgePointSet.Contains(checkPoint)) return;
            // 그림 범위를 벗어나면 넘어감
            if (!IsValidPoint(checkPoint)) return;

            Color checkColor = imageBitmap.GetPixel(checkPoint.X, checkPoint.Y);
            // 배경
            if (checkColor.ToArgb() == Color.White.ToArgb()) return;
            // 간선
            else if (checkColor.ToArgb() == Color.Black.ToArgb())
            {
                readyQueue.Enqueue(checkPoint);
                edgePointSet.Add(checkPoint);
            }
            // 노드
            else
            {
                adjNodePointList.Add(checkPoint);
            }
        }
        private bool IsValidPoint(Point p)
        {
            if (p.X < 0 || p.X >= imageBitmap.Width || p.Y < 0 || p.Y >= imageBitmap.Height) return false;
            else return true;
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
