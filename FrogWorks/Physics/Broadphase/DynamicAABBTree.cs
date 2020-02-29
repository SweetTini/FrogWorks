using System.Collections.Generic;

namespace FrogWorks
{
    public class DynamicAABBTree
    {
        Dictionary<Collider, Node> _nodes;
        Node _root;

        internal DynamicAABBTree()
        {
            _nodes = new Dictionary<Collider, Node>();
        }

        public void Add(Collider collider)
        {
            if (collider != null)
            {
                if (!_nodes.ContainsKey(collider))
                {
                    var node = new Node()
                    {
                        Collider = collider,
                        AABB = collider.CreateAABB()
                    };

                    _nodes.Add(collider, node);
                    AddLeaf(node);
                }
                else
                {
                    Update(collider);
                }
            }
        }

        public void Update(Collider collider)
        {
            if (collider != null && _nodes.ContainsKey(collider))
            {
                UpdateLeaf(
                    _nodes[collider],
                    collider.CreateAABB());
            }
        }

        public void Remove(Collider collider)
        {
            if (collider != null && _nodes.ContainsKey(collider))
            {
                RemoveLeaf(_nodes[collider]);
                _nodes.Remove(collider);
            }
        }

        public void Clear()
        {
            _nodes.Clear();
            _root = null;
        }

        void AddLeaf(Node leaf)
        {
            if (_root == null)
            {
                _root = leaf;
            }
            else
            {
                var tree = GetTree(leaf);
                var sibling = tree;
                var oldParent = sibling.Parent;
                var newParent = new Node()
                {
                    AABB = leaf.AABB.Merge(sibling.AABB),
                    Parent = sibling.Parent,
                    Left = sibling,
                    Right = leaf
                };

                sibling.Parent = newParent;
                leaf.Parent = newParent;

                if (oldParent == null)
                {
                    _root = newParent;
                }
                else
                {
                    if (oldParent.Left == sibling)
                        oldParent.Left = newParent;
                    else oldParent.Right = newParent;
                }

                tree = leaf.Parent;
                UpdateTree(tree);
            }
        }

        void UpdateLeaf(Node leaf, AABB aabb)
        {
            if (!leaf.AABB.Contains(aabb))
            {
                RemoveLeaf(leaf);
                leaf.AABB = aabb;
                AddLeaf(leaf);
            }
        }

        void RemoveLeaf(Node leaf)
        {
            var parent = leaf.Parent;
            var grandparent = parent.Parent;
            var sibling = parent.Left != leaf
                ? parent.Left
                : parent.Right;

            if (grandparent != null)
            {
                if (grandparent.Left == parent)
                    grandparent.Left = sibling;
                else grandparent.Right = sibling;

                sibling.Parent = grandparent;
                UpdateTree(grandparent);
            }
            else
            {
                _root = sibling;
                sibling.Parent = null;
            }

            leaf.Parent = null;
        }

        Node GetTree(Node leaf)
        {
            var tree = _root;

            while (!tree.IsLeaf)
            {
                var left = tree.Left;
                var right = tree.Right;
                var merge = tree.AABB.Merge(leaf.AABB);
                var parentCost = 2f * merge.Area;
                var minCost = 2f * (merge.Area - tree.AABB.Area);

                var leftMerge = leaf.AABB.Merge(left.AABB);
                var leftCost = left.IsLeaf
                    ? leftMerge.Area + minCost
                    : leftMerge.Area - left.AABB.Area + minCost;

                var rightMerge = leaf.AABB.Merge(right.AABB);
                var rightCost = right.IsLeaf
                    ? rightMerge.Area + minCost
                    : rightMerge.Area - right.AABB.Area + minCost;

                if (parentCost < leftCost && parentCost < rightCost)
                    break;

                tree = leftCost < rightCost
                    ? left
                    : right;
            }

            return tree;
        }

        void UpdateTree(Node tree)
        {
            while (tree != null)
            {
                var left = tree.Left;
                var right = tree.Right;

                tree.AABB = left.AABB.Merge(right.AABB);
                tree = tree.Parent;
            }
        }

        #region Node
        class Node
        {
            public Collider Collider { get; set; }

            public AABB AABB { get; set; }

            public Node Parent { get; set; }

            public Node Left { get; set; }

            public Node Right { get; set; }

            public int Height { get; set; }

            public bool IsLeaf => Left == null;
        }
        #endregion
    }
}
