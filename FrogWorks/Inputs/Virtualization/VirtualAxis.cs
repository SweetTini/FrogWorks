using System.Collections.Generic;

namespace FrogWorks
{
    public class VirtualAxis : VirtualInput
    {
        protected List<VirtualAxisNode> Nodes { get; private set; }

        public float CurrentValue { get; private set; }

        public float LastValue { get; private set; }

        public VirtualAxis()
            : base()
        {
            Nodes = new List<VirtualAxisNode>();
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
            if (!Nodes.Contains(node))
                Nodes.Add(node);
        }

        public void Register(params VirtualAxisNode[] nodes)
        {
            for (int i = 0; i < nodes.Length; i++)
                Register(nodes[i]);
        }

        public void Register(IEnumerable<VirtualAxisNode> nodes)
        {
            foreach (var node in Nodes)
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
