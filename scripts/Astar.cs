using Godot;
using System.Collections.Generic;
using System;

public class Astar
{
    private class PfNode : IComparable<PfNode>
    {
        private Vector2I _pt;
        private PfNode _parent = null;
        private int _f;
        private int _g;
        private int _h;

        public Vector2I Pt { get { return _pt; } }
        public PfNode Parent { get { return _parent; } set { _parent = value; } }
        public int F { get { return _f; } set { _f = value; } }
        public int G { get { return _g; } set { _g = value; } }
        public int H { get { return _h; } set { _h = value; } }


        public PfNode(Vector2I pt)
        {
            _pt = pt;
            _parent = null;
            _f = 0;
            _g = 0;
            _h = 0;
        }

        public PfNode(Vector2I pt, PfNode parent, int f, int g, int h)
        {
            _pt = pt;
            _parent = parent;
            _f = f;
            _g = g;
            _h = h;
        }

        public override string ToString()
        {
            return $"PfNode - {_pt.X}, {_pt.Y} | parent:{_parent != null} | f: {_f} | g: {_g} | h: {_h} ";
        }

        public int CompareTo(PfNode node)
        {
            if (_f < node.F)
            {
                return -1;
            }
            else if (_f > node.F)
            {
                return 1;
            }

            return 0;
        }
    }

    private Heap<PfNode> _openHeap = new Heap<PfNode>(10);

    private List<Vector2I> GetReturnPath(PfNode node)
    {
        List<Vector2I> path = new List<Vector2I>();

        PfNode currNode = node;
        while (currNode is not null)
        {
            path.Add(currNode.Pt);
            currNode = currNode.Parent;
        }

        path.Reverse();

        return path;
    }

    private Dictionary<string, Vector2I> GetValidPts(PfNode currNode, Dictionary<string, Vector2I> steps, Vector2I[] blockers, int colSize, int rowSize)
    {
        Dictionary<string, Vector2I> validPtDict = new Dictionary<string, Vector2I>();
        Dictionary<string, bool> blockDict = new Dictionary<string, bool>();
        blockDict["t"] = false;
        blockDict["r"] = false;
        blockDict["b"] = false;
        blockDict["l"] = false;

        foreach (KeyValuePair<string, Vector2I> kvp in steps)
        {
            int newX = kvp.Value.X + currNode.Pt.X;
            int newY = kvp.Value.Y + currNode.Pt.Y;

            if (newX < 0 || newX >= colSize || newY < 0 || newY >= rowSize)
            {
                continue;
            }

            Vector2I newPt = new Vector2I(newX, newY);
            bool inBlockers = false;
            for (int i = 0; i < blockers.Length; i++)
            {
                if (newPt.Equals(blockers[i]))
                {
                    blockDict[kvp.Key] = true;
                    inBlockers = true;
                    break;
                }
            }

            if (inBlockers)
            {
                continue;
            }

            if (kvp.Key.Length == 2)
            {
                if (blockDict[$"{kvp.Key[0]}"] && blockDict[$"{kvp.Key[1]}"])
                {
                    continue;
                }
            }

            validPtDict[kvp.Key] = newPt;
        }

        return validPtDict;
    }

    private PfNode GetExistingNode(Vector2I pt)
    {
        for (int i = 0; i < _openHeap.Count; i++)
        {
            PfNode node = _openHeap.GetAtIndex(i);
            if (pt.Equals(node.Pt))
            {
                return node;
            }
        }

        return null;
    }

    private int Square(Vector2I a, Vector2I b)
    {
        int c = a.X - b.X;
        int d = a.Y = b.Y;

        return c * c + d * d;
    }

    public List<Vector2I> StartPathFinding(int colSize, int rowSize, Vector2I start, Vector2I end, Vector2I[] blockers)
    {
        PfNode startNode = new PfNode(start);
        PfNode endNode = new PfNode(end);

        Dictionary<string, Vector2I> steps = new Dictionary<string, Vector2I>();
        steps["t"] = new Vector2I(0, -1);
        steps["r"] = new Vector2I(1, 0);
        steps["b"] = new Vector2I(0, 1);
        steps["l"] = new Vector2I(-1, 0);
        steps["tr"] = new Vector2I(1, -1);
        steps["br"] = new Vector2I(1, 1);
        steps["bl"] = new Vector2I(-1, 1);
        steps["tl"] = new Vector2I(-1, -1);

        _openHeap.Push(startNode);
        List<Vector2I> closeList = new List<Vector2I>();

        while (_openHeap.Count > 0)
        {
            PfNode currNode = _openHeap.Pop();
            if (currNode.Pt.Equals(endNode.Pt))
            {
                return GetReturnPath(currNode);
            }

            closeList.Add(currNode.Pt);

            Dictionary<string, Vector2I> validPtDict = GetValidPts(currNode, steps, blockers, colSize, rowSize);
            foreach (KeyValuePair<string, Vector2I> kvp in validPtDict)
            {
                Vector2I pt = kvp.Value;

                bool isInCloseList = false;
                for (int i = 0; i < closeList.Count; i++)
                {
                    if (pt.Equals(closeList[i]))
                    {
                        isInCloseList = true;
                        break;
                    }
                }

                if (isInCloseList)
                {
                    continue;
                }

                int g = 0;
                if (kvp.Key.Length == 2)
                {
                    g = currNode.G + 15;
                }
                else
                {
                    g = currNode.G + 10;
                }

                int h = Square(pt, endNode.Pt);
                int f = g + h;

                PfNode existingNode = GetExistingNode(pt);
                if (existingNode is not null)
                {
                    if (existingNode.G > g)
                    {
                        existingNode.G = g;
                        existingNode.H = h;
                        existingNode.F = f;
                        existingNode.Parent = currNode;
                        _openHeap.Fix();
                    }

                    continue;
                }

                PfNode newNode = new PfNode(pt, currNode, f, g, h);
                _openHeap.Push(newNode);
            }
        }

        return null;
    }
}
