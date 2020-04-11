using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace FrogWorks
{
    public class VirtualHardAxis : VirtualInput
    {
        List<VirtualAxisNode> _nodes;

        protected ReadOnlyCollection<VirtualAxisNode> Nodes { get; private set; }

        public int CurrentValue { get; private set; }

        public int LastValue { get; private set; }

        public VirtualHardAxis()
            : base()
        {
            _nodes = new List<VirtualAxisNode>();
            Nodes = new ReadOnlyCollection<VirtualAxisNode>(_nodes);
        }

        public VirtualHardAxis(params VirtualAxisNode[] nodes)
            : this()
        {
            Register(nodes);
        }

        public VirtualHardAxis(IEnumerable<VirtualAxisNode> nodes)
            : this()
        {
            Register(nodes);
        }

        public override void Update(float deltaTime)
        {
            LastValue = CurrentValue;
            CurrentValue = 0;
            var bypass = false;

            for (int i = 0; i < Nodes.Count; i++)
            {
                Nodes[i].Update(deltaTime);

                if (!bypass && Nodes[i].Value != 0f)
                {
                    CurrentValue = Math.Sign(Nodes[i].Value);
                    bypass = true;
                }
            }
        }

        public void Register(VirtualAxisNode node)
        {
            if (!_nodes.Contains(node))
                _nodes.Add(node);
        }

        public void Register(params VirtualAxisNode[] nodes)
        {
            for (int i = 0; i < nodes.Length; i++)
                Register(nodes[i]);
        }

        public void Register(IEnumerable<VirtualAxisNode> nodes)
        {
            foreach (var node in nodes)
                Register(node);
        }

        public static implicit operator int(VirtualHardAxis axis)
        {
            return axis.CurrentValue;
        }
    }
}
