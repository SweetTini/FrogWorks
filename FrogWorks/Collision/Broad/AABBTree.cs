using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace FrogWorks
{
    public class AABBTree
    {
        private List<AABBNode> _nodes;
        private int _nodeCount, _capacity, _growthSize;
        private int _rootIndex, _freeIndex;
        private float _padding;

        private AABBTree()
        {
            _nodes = new List<AABBNode>();
            _rootIndex = -1;
        }

        public AABBTree(float padding = 0f)
            : this(1, padding)
        {
        }

        public AABBTree(int capacity, float padding = 0f)
            : this()
        {
            _growthSize = _capacity = Math.Max(Math.Abs(capacity), 1);
            _padding = Math.Abs(padding);

            for (int i = 0; i < _capacity; i++)
            {
                var node = new AABBNode();
                node.NextIndex = i < _capacity - 1 ? i + 1 : -1;
                _nodes.Add(node);
            }
        }

        public void Insert(IAABBContainer container)
        {
            if (!Exists(container))
            {
                var index = AllocateNode();
                var node = _nodes[index];

                node.Bounds = container.Bounds;
                node.Entity = container;
                _nodes[index] = node;

                InsertLeaf(index);
            }
        }

        public void Remove(IAABBContainer container)
        {
            int index;

            if (Exists(container, out index))
            {
                RemoveLeaf(index);
                DeallocateNode(index);
            }
        }

        public void Update(IAABBContainer container)
        {
            int index;

            if (Exists(container, out index))
                UpdateLeaf(index, container.Bounds);
        }

        public void Clear()
        {
            _nodes.Clear();
            _nodeCount = _capacity = 0;
            _rootIndex = _freeIndex = -1;
        }

        public List<IAABBContainer> Query(IAABBContainer container)
        {
            var containers = new List<IAABBContainer>();
            var stack = new Stack<int>();
            var target = container.Bounds;

            stack.Push(_rootIndex);

            while (stack.Count > 0)
            {
                var index = stack.Pop();
                if (index <= -1) continue;

                var node = _nodes[index];

                if (node.Bounds.Overlaps(target))
                {
                    if (node.IsLeaf && node.Entity != container)
                    {
                        containers.Add(node.Entity);
                        continue;
                    }

                    stack.Push(node.LeftIndex);
                    stack.Push(node.RightIndex);
                }
            }

            return containers;
        }

        public void Draw(RendererBatch batch, Color leafColor, Color treeColor)
        {
            for (int i = 0; i < _nodes.Count; i++)
            {
                var aabb = _nodes[i].Bounds;
                if (aabb.Equals(default(AABB))) continue;

                var color = _nodes[i].IsLeaf ? leafColor : treeColor;
                batch.DrawPrimitives((primitive) => primitive.DrawRectangle(aabb.Upper, aabb.Lower - aabb.Upper, color));
            }
        }

        private int AllocateNode()
        {
            if (_freeIndex <= -1)
            {
                _capacity += _growthSize;

                for (int i = _nodeCount; i < _capacity; i++)
                {
                    var node = new AABBNode();
                    node.NextIndex = i < _capacity - 1 ? i + 1 : -1;
                    _nodes.Add(node);
                }

                _freeIndex = _nodeCount;
            }

            var index = _freeIndex;
            var alloNode = _nodes[index];

            alloNode.ParentIndex = alloNode.LeftIndex = alloNode.RightIndex = -1;
            _nodes[index] = alloNode;
            _freeIndex = alloNode.NextIndex;
            _nodeCount++;

            return index;
        }

        private void DeallocateNode(int index)
        {
            var node = _nodes[index];
            node.Entity = null;
            node.Bounds = default(AABB);
            node.NextIndex = _freeIndex;

            _nodes[index] = node;
            _freeIndex = index;
            _nodeCount--;
        }

        private void InsertLeaf(int leafIndex)
        {
            if (_rootIndex <= -1)
            {
                _rootIndex = leafIndex;
            }
            else
            {
                var leaf = _nodes[leafIndex];
                var treeIndex = _rootIndex;
                var tree = _nodes[treeIndex];

                while (!tree.IsLeaf)
                {
                    var leftIndex = tree.LeftIndex;
                    var left = _nodes[leftIndex];
                    var rightIndex = tree.RightIndex;
                    var right = _nodes[rightIndex];

                    var merged = tree.Bounds.Merge(leaf.Bounds, _padding);
                    var parentCost = 2f * merged.SurfaceArea;
                    var lowCost = 2f * (merged.SurfaceArea - tree.Bounds.SurfaceArea);

                    var leftMerged = leaf.Bounds.Merge(left.Bounds, _padding);
                    var leftCost = left.IsLeaf
                        ? leftMerged.SurfaceArea + lowCost
                        : (leftMerged.SurfaceArea - left.Bounds.SurfaceArea) + lowCost;

                    var rightMerged = leaf.Bounds.Merge(right.Bounds, _padding);
                    var rightCost = right.IsLeaf
                        ? rightMerged.SurfaceArea + lowCost
                        : (rightMerged.SurfaceArea - right.Bounds.SurfaceArea) + lowCost;

                    if (parentCost < leftCost && parentCost < rightCost)
                        break;

                    treeIndex = leftCost < rightCost ? leftIndex : rightIndex;
                    tree = _nodes[treeIndex];
                }

                var siblingIndex = treeIndex;
                var sibling = _nodes[siblingIndex];
                var oldParentIndex = sibling.ParentIndex;
                var newParentIndex = AllocateNode();
                var newParent = _nodes[newParentIndex];

                newParent.Bounds = leaf.Bounds.Merge(sibling.Bounds, _padding);
                newParent.ParentIndex = oldParentIndex;
                newParent.LeftIndex = siblingIndex;
                newParent.RightIndex = leafIndex;
                leaf.ParentIndex = sibling.ParentIndex = newParentIndex;

                if (oldParentIndex <= -1)
                {
                    _rootIndex = newParentIndex;
                }
                else
                {
                    var oldParent = _nodes[oldParentIndex];

                    if (oldParent.LeftIndex == siblingIndex)
                        oldParent.LeftIndex = newParentIndex;
                    else oldParent.RightIndex = newParentIndex;

                    _nodes[oldParentIndex] = oldParent;
                }

                _nodes[leafIndex] = leaf;
                _nodes[siblingIndex] = sibling;
                _nodes[newParentIndex] = newParent;

                treeIndex = leaf.ParentIndex;
                UpdateTree(treeIndex);
            }
        }

        private void RemoveLeaf(int leafIndex)
        {
            if (leafIndex == _rootIndex)
            {
                _rootIndex = -1;
            }
            else
            {
                var leaf = _nodes[leafIndex];
                var parentIndex = leaf.ParentIndex;
                var parent = _nodes[parentIndex];
                var grandParentIndex = parent.ParentIndex;
                var siblingIndex = parent.LeftIndex == leafIndex ? parent.RightIndex : parent.LeftIndex;
                var sibling = _nodes[siblingIndex];

                if (grandParentIndex > -1)
                {
                    var grandParent = _nodes[grandParentIndex];

                    if (grandParent.LeftIndex == parentIndex)
                        grandParent.LeftIndex = siblingIndex;
                    else grandParent.RightIndex = siblingIndex;

                    sibling.ParentIndex = grandParentIndex;
                    _nodes[siblingIndex] = sibling;
                    _nodes[grandParentIndex] = grandParent;

                    DeallocateNode(parentIndex);
                    UpdateTree(grandParentIndex);
                }
                else
                {
                    _rootIndex = siblingIndex;
                    sibling.ParentIndex = -1;
                    _nodes[siblingIndex] = sibling;
                    DeallocateNode(parentIndex);
                }

                leaf.ParentIndex = -1;
                _nodes[leafIndex] = leaf;
            }
        }

        private void UpdateLeaf(int leafIndex, AABB newAabb)
        {
            var leaf = _nodes[leafIndex];

            if (!leaf.Bounds.Contains(newAabb))
            {
                RemoveLeaf(leafIndex);
                leaf.Bounds = newAabb;
                _nodes[leafIndex] = leaf;
                InsertLeaf(leafIndex);
            }
        }

        private void UpdateTree(int treeIndex)
        {
            while (treeIndex > -1)
            {
                var tree = _nodes[treeIndex];
                var left = _nodes[tree.LeftIndex];
                var right = _nodes[tree.RightIndex];

                tree.Bounds = left.Bounds.Merge(right.Bounds, _padding);
                _nodes[treeIndex] = tree;
                treeIndex = tree.ParentIndex;
            }
        }

        private bool Exists(IAABBContainer container)
        {
            int index;
            return Exists(container, out index);
        }

        private bool Exists(IAABBContainer container, out int index)
        {
            for (int i = 0; i < _nodes.Count; i++)
            {
                var node = _nodes[i];

                if (node.Entity == container)
                {
                    index = i;
                    return true;
                }
            }

            index = -1;
            return false;
        }
    }

    internal struct AABBNode
    {
        public IAABBContainer Entity { get; set; }

        public AABB Bounds { get; set; }

        public int ParentIndex { get; set; }

        public int LeftIndex { get; set; }

        public int RightIndex { get; set; }

        public int NextIndex { get; set; }

        public bool IsLeaf => LeftIndex < 0;
    }
}
