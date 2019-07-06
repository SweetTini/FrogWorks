using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace FrogWorks
{
    public abstract class VirtualAnalog : VirtualInput
    {
        protected List<VirtualAnalogNode> Nodes { get; private set; }

        public Vector2 CurrentValue { get; private set; }

        public Vector2 LastValue { get; private set; }

        public bool Normalize { get; set; }

        public float? Segments { get; set; }

        public VirtualAnalog()
            : base()
        {
            Nodes = new List<VirtualAnalogNode>();
        }

        public VirtualAnalog(params VirtualAnalogNode[] nodes)
            : this()
        {
            Register(nodes);
        }

        public VirtualAnalog(IEnumerable<VirtualAnalogNode> nodes)
            : this()
        {
            Register(nodes);
        }

        public override void Update(float deltaTime)
        {
            LastValue = CurrentValue;
            CurrentValue = Vector2.Zero;
            var bypass = false;

            for (int i = 0; i < Nodes.Count; i++)
            {
                Nodes[i].Update(deltaTime);
                var nextValue = Nodes[i].Value;

                if (!bypass && nextValue != Vector2.Zero)
                {
                    if (Normalize)
                    {
                        nextValue = Segments.HasValue
                            ? nextValue.SnapAndNormalizeAngle(Segments.Value)
                            : Vector2.Normalize(nextValue);
                    }
                    else if (Segments.HasValue)
                    {
                        nextValue = nextValue.SnapAngle(Segments.Value);
                    }

                    CurrentValue = nextValue;
                    bypass = true;
                }
            }
        }

        public void Register(VirtualAnalogNode node)
        {
            if (!Nodes.Contains(node))
                Nodes.Add(node);
        }

        public void Register(params VirtualAnalogNode[] nodes)
        {
            for (int i = 0; i < nodes.Length; i++)
                Register(nodes[i]);
        }

        public void Register(IEnumerable<VirtualAnalogNode> nodes)
        {
            foreach (var node in Nodes)
                Register(node);
        }

        public static implicit operator Vector2(VirtualAnalog analog)
        {
            return analog.CurrentValue;
        }
    }

    public abstract class VirtualAnalogNode : VirtualInputNode
    {
        public abstract Vector2 Value { get; }
    }
}
