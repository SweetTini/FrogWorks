using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace FrogWorks
{
    public class VirtualAxis : VirtualInput
    {
        List<VirtualAxisNode> _nodes;

        protected ReadOnlyCollection<VirtualAxisNode> Nodes { get; private set; }

        public float CurrentValue { get; private set; }

        public float LastValue { get; private set; }

        public VirtualAxis()
            : base()
        {
            _nodes = new List<VirtualAxisNode>();
            Nodes = new ReadOnlyCollection<VirtualAxisNode>(_nodes);
        }

        public VirtualAxis(params VirtualAxisNode[] nodes)
            : this()
        {
            Register(nodes);
        }

        public VirtualAxis(IEnumerable<VirtualAxisNode> nodes)
            : this()
        {
            Register(nodes);
        }

        public override void Update(float deltaTime)
        {
            LastValue = CurrentValue;
            CurrentValue = 0f;
            var bypass = false;

            for (int i = 0; i < Nodes.Count; i++)
            {
                Nodes[i].Update(deltaTime);

                if (!bypass && Nodes[i].Value != 0f)
                {
                    CurrentValue = Nodes[i].Value;
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

        public static implicit operator float(VirtualAxis axis)
        {
            return axis.CurrentValue;
        }
    }

    public abstract class VirtualAxisNode : VirtualInputNode
    {
        public abstract float Value { get; }
    }
}
