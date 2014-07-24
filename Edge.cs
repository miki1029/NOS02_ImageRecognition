using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace NOS02
{
    class Edge : IComparable<Edge>
    {
        private List<Point> adjNodePointList;
        
        private Node prev;
        private Node next;

        private int prevIdx;
        private int nextIdx;

        public Edge(List<Point> npl)
        {
            adjNodePointList = npl;
            prev = null; next = null;
            prevIdx = -1; nextIdx = -1;
        }

        public void FindAdjacentNode(SortedSet<Node> nodeSet)
        {
            bool find = false;
            foreach (Point point in adjNodePointList)
            {
                int nodeIdx = -1;
                foreach (Node node in nodeSet)
                {
                    nodeIdx++;

                    // point가 node에 존재하면
                    if (node.IsInsidePoint(point))
                    {
                        // 첫 노드 발견시
                        if (prev == null)
                        {
                            prev = node; prevIdx = nodeIdx;
                            break;
                        }
                        // 이미 찾았던 노드이면 넘어감
                        else if (prev == node)
                        {
                            break;
                        }
                        // 새로운 노드 발견시(두 번째 노드)
                        else
                        {
                            // 현재 인덱스가 더 작을 경우->prev
                            if (nodeIdx < prevIdx)
                            {
                                next = prev; nextIdx = prevIdx;
                                prev = node; prevIdx = nodeIdx;
                            }
                            else if (nodeIdx == prevIdx) throw new Exception();
                            // 현재 인덱스가 더 클 경우->next
                            else
                            {
                                next = node; nextIdx = nodeIdx;
                            }
                            find = true;
                            break;
                        }
                    }
                }
                if (find) break;
            }
            if (!find)
            {
                Console.WriteLine("2개의 노드와 연결되지 않은 간선이 존재합니다!");
                if (prev != null)
                {
                    Console.WriteLine("노드 ({0}, {1})와 연결된 간선을 확인해 보세요.", prev.X, prev.Y);
                }
            }
        }

        public int PrevIdx
        {
            get
            {
                if (prevIdx < 0) throw new IndexOutOfRangeException();
                else return prevIdx;
            }
        }
        public int NextIdx
        {
            get
            {
                if (nextIdx < 0) throw new IndexOutOfRangeException();
                else return nextIdx;
            }
        }

        public override string ToString()
        {
            return PrevIdx + " " + NextIdx;
        }

        public int CompareTo(Edge other)
        {
            int cmp = PrevIdx.CompareTo(other.PrevIdx);
            if(cmp == 0)
            {
                cmp = NextIdx.CompareTo(other.NextIdx);
            }
            return cmp;
        }
    }
}
