using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace FrogWorks
{
    public class DynamicAABBTree
    {
        Dictionary<Collider, Node> _nodes;
        Node _root;
        float _padding;

        internal DynamicAABBTree(float padding = 0f)
        {
            _nodes = new Dictionary<Collider, Node>();
            _padding = padding.Abs();
        }

        public IEnumerable<Collider> Query(AABB aabb, Func<Collider, bool> onCollide)
        {
            var colliders = new List<Collider>();
            var nodes = new Stack<Node>();

            nodes.Push(_root);

            while (nodes.Count > 0)
            {
                var node = nodes.Pop();

                if (node != null && node.AABB.Overlaps(aabb))
                {
                    if (node.IsLeaf)
                    {
                        var collider = node.Collider;

                        if (collider != null && onCollide(collider))
                            colliders.Add(collider);
                    }
                    else
                    {
                        nodes.Push(node.Left);
                        nodes.Push(node.Right);
                    }
                }
            }

            return colliders;
        }

        public void Draw(RendererBatch batch, Color treeColor, Color leafColor)
        {
            var treeNodes = new Stack<Node>();

            treeNodes.Push(_root);

            while (treeNodes.Count > 0)
            {
                var treeNode = treeNodes.Pop();

                if (treeNode != null && !treeNode.IsLeaf) 
                {
                    batch.DrawPrimitives(p =>
                    {
                        var aabb = treeNode.AABB;
                        p.DrawRectangle(aabb.Min, aabb.Size, treeColor);
                    });

                    treeNodes.Push(treeNode.Left);
                    treeNodes.Push(treeNode.Right);
                }
            }

            foreach (var leafNode in _nodes.Values)
            {
                batch.DrawPrimitives(p =>
                {
                    var aabb = leafNode.AABB;
                    p.DrawRectangle(aabb.Min, aabb.Size, leafColor);
                });
            }
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
                        AABB = collider.CreateAABB().Expand(_padding)
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
                var aabb = collider.CreateAABB().Expand(_padding);
                UpdateLeaf(_nodes[collider], aabb);
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
                var sibling = GetTree(leaf);
                var oldParent = sibling.Parent;
                var newParent = new Node()
                {
                    AABB = leaf.AABB.Merge(sibling.AABB).Expand(_padding),
                    Parent = sibling.Parent,
                    Left = sibling,
                    Right = leaf,
                    Height = sibling.Height + 1
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

                UpdateTree(leaf.Parent);
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
                var merge = tree.AABB.Merge(leaf.AABB).Expand(_padding);
                var parentCost = 2f * merge.Area;
                var minCost = 2f * (merge.Area - tree.AABB.Area);

                var leftMerge = leaf.AABB.Merge(left.AABB).Expand(_padding);
                var leftCost = left.IsLeaf
                    ? leftMerge.Area + minCost
                    : leftMerge.Area - left.AABB.Area + minCost;

                var rightMerge = leaf.AABB.Merge(right.AABB).Expand(_padding);
                var rightCost = right.IsLeaf
                    ? rightMerge.Area + minCost
                    : rightMerge.Area - right.AABB.Area + minCost;

                if (parentCost < leftCost && parentCost < rightCost)
                    break;

                tree = leftCost < rightCost ? left : right;
            }

            return tree;
        }

        void UpdateTree(Node tree)
        {
            while (tree != null)
            {
                tree = BalanceTree(tree);

                var left = tree.Left;
                var right = tree.Right;

                tree.AABB = left.AABB.Merge(right.AABB).Expand(_padding);
                tree.Height = Math.Max(left.Height, right.Height) + 1;
                tree = tree.Parent;
            }
        }

        Node BalanceTree(Node tree)
        {
            if (!tree.IsLeaf && tree.Height > 1)
            {
                var left = tree.Left;
                var right = tree.Right;
                var balance = right.Height - left.Height;

                if (balance.Abs() > 1)
                {
                    var fixLeft = balance.Sign() < 0;
                    var node = fixLeft ? left : right;
                    var oNode = fixLeft ? right : left;

                    var lNode = node.Left;
                    var rNode = node.Right;

                    node.Left = tree;
                    node.Parent = tree.Parent;
                    tree.Parent = node;

                    if (node.Parent != null)
                    {
                        if (node.Parent.Left == tree)
                            node.Parent.Left = node;
                        else node.Parent.Right = node;
                    }
                    else
                    {
                        _root = node;
                    }

                    fixLeft = lNode.Height > rNode.Height;
                    var hiNode = fixLeft ? lNode : rNode;
                    var loNode = fixLeft ? rNode : lNode;

                    node.Right = hiNode;
                    tree.Right = loNode;
                    loNode.Parent = tree;

                    tree.AABB = oNode.AABB.Merge(loNode.AABB).Expand(_padding);
                    node.AABB = tree.AABB.Merge(hiNode.AABB).Expand(_padding);

                    tree.Height = Math.Max(oNode.Height, loNode.Height) + 1;
                    node.Height = Math.Max(tree.Height, hiNode.Height) + 1;

                    return node;
                }
            }

            return tree;
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
