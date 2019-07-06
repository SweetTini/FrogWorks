using System;
using System.Collections.Generic;

namespace FrogWorks
{
    public abstract class VirtualButton : VirtualInput
    {
        private float _timer, _repeatTimer;
        private float _bufferTime, _initialRepeatTime, _multiRepeatTime;
        private bool _canRepeat, _isRepeating, _isConsumed;

        protected List<VirtualButtonNode> Nodes { get; private set; }

        public bool Repeat { get; private set; }

        public float BufferTime
        {
            get { return _bufferTime; }
            set { _bufferTime = Math.Abs(value); }
        }

        public bool IsDown
        {
            get
            {
                if (Input.IsDisabled) return false;

                for (int i = 0; i < Nodes.Count; i++)
                    if (Nodes[i].IsDown)
                        return true;

                return false;
            }
        }

        public bool IsPressed
        {
            get
            {
                if (Input.IsDisabled || _isConsumed) return false;

                for (int i = 0; i < Nodes.Count; i++)
                    if (Nodes[i].IsPressed)
                        return true;

                return false;
            }
        }

        public bool IsReleased
        {
            get
            {
                if (Input.IsDisabled) return false;

                for (int i = 0; i < Nodes.Count; i++)
                    if (Nodes[i].IsReleased)
                        return true;

                return false;
            }
        }

        public VirtualButton(float bufferTime = 0f)
            : base()
        {
            Nodes = new List<VirtualButtonNode>();
            BufferTime = bufferTime;
        }

        public VirtualButton(params VirtualButtonNode[] nodes)
            : this(0f, nodes)
        {
        }

        public VirtualButton(IEnumerable<VirtualButtonNode> nodes)
            : this(0f, nodes)
        {
        }

        public VirtualButton(float bufferTime, params VirtualButtonNode[] nodes)
            : this(bufferTime)
        {
            Register(nodes);
        }

        public VirtualButton(float bufferTime, IEnumerable<VirtualButtonNode> nodes)
            : this(bufferTime)
        {
            Register(nodes);
        }

        public override void Update(float deltaTime)
        {
            _timer -= deltaTime;
            var bypass = _isConsumed = false;

            for (int i = 0; i < Nodes.Count; i++)
            {
                Nodes[i].Update(deltaTime);

                if (!bypass && (Nodes[i].IsPressed || Nodes[i].IsDown))
                {
                    if (Nodes[i].IsPressed)
                        _timer = _bufferTime;
                    bypass = true;
                }
            }

            if (!bypass)
            {
                _timer = _repeatTimer = 0f;
                Repeat = _isRepeating = false;
                return;
            }

            if (_canRepeat)
            {
                _repeatTimer -= deltaTime;
                Repeat = false;

                if (_repeatTimer <= 0f)
                {
                    if (!_isRepeating)
                    {
                        _repeatTimer = _initialRepeatTime;
                        _isRepeating = true;
                    }
                    else
                    {
                        _repeatTimer = _multiRepeatTime;
                        Repeat = true;
                    }
                }
            }
        }

        public void Register(VirtualButtonNode node)
        {
            if (!Nodes.Contains(node))
                Nodes.Add(node);
        }

        public void Register(params VirtualButtonNode[] nodes)
        {
            for (int i = 0; i < nodes.Length; i++)
                Register(nodes[i]);
        }

        public void Register(IEnumerable<VirtualButtonNode> nodes)
        {
            foreach (var node in Nodes)
                Register(node);
        }

        public void ConsumeBuffer()
        {
            _timer = 0f;
        }

        public void ConsumePress()
        {
            _timer = 0f;
            _isConsumed = true;
        }

        public void SetRepeatTime(float time)
        {
            SetRepeatTime(time, time);
        }

        public void SetRepeatTime(float initRepeatTime, float multiRepeatTime)
        {
            _initialRepeatTime = Math.Abs(initRepeatTime);
            _multiRepeatTime = Math.Abs(multiRepeatTime);
            _canRepeat = _initialRepeatTime > 0f;

            if (!_canRepeat)
                Repeat = _isRepeating = false;
        }

        public static implicit operator bool(VirtualButton input)
        {
            return input.IsDown;
        }
    }

    public abstract class VirtualButtonNode : VirtualInputNode
    {
        public abstract bool IsDown { get; }

        public abstract bool IsPressed { get; }

        public abstract bool IsReleased { get; }
    }
}
