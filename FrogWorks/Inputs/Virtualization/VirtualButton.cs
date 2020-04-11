using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace FrogWorks
{
    public class VirtualButton : VirtualInput
    {
        List<VirtualButtonNode> _nodes;
        float _timer, _repeatTimer;
        float _bufferTime, 
            _initialRepeatTime, 
            _multiRepeatTime;
        bool _canRepeat,
            _isConsumed;

        protected ReadOnlyCollection<VirtualButtonNode> Nodes { get; private set; }

        public bool IsRepeating { get; private set; }

        public float BufferTime
        {
            get { return _bufferTime; }
            set { _bufferTime = Math.Abs(value); }
        }

        public bool IsDown
        {
            get
            {
                if (Input.IsDisabled) 
                    return false;

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
                if (Input.IsDisabled || _isConsumed) 
                    return false;

                if (_timer > 0f || IsRepeating)
                    return true;

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
                if (Input.IsDisabled) 
                    return false;

                for (int i = 0; i < Nodes.Count; i++)
                    if (Nodes[i].IsReleased)
                        return true;

                return false;
            }
        }

        public VirtualButton(float bufferTime = 0f)
            : base()
        {
            _nodes = new List<VirtualButtonNode>();
            Nodes = new ReadOnlyCollection<VirtualButtonNode>(_nodes);
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
            var bypass = _isConsumed = false;
            _timer -= deltaTime;

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
                IsRepeating = false;
                return;
            }
            else if (_canRepeat)
            {
                IsRepeating = false;

                if (_repeatTimer == 0f)
                {
                    _repeatTimer = _initialRepeatTime;
                }
                else
                {
                    _repeatTimer -= deltaTime;
                    
                    if (_repeatTimer <= 0f)
                    {
                        IsRepeating = true;
                        _repeatTimer = _multiRepeatTime;
                    }
                }
            }
        }

        public void Register(VirtualButtonNode node)
        {
            if (!_nodes.Contains(node))
                _nodes.Add(node);
        }

        public void Register(params VirtualButtonNode[] nodes)
        {
            for (int i = 0; i < nodes.Length; i++)
                Register(nodes[i]);
        }

        public void Register(IEnumerable<VirtualButtonNode> nodes)
        {
            foreach (var node in nodes)
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
                IsRepeating = false;
        }

        public static implicit operator bool(VirtualButton button)
        {
            return button.IsDown;
        }
    }

    public abstract class VirtualButtonNode : VirtualInputNode
    {
        public abstract bool IsDown { get; }

        public abstract bool IsPressed { get; }

        public abstract bool IsReleased { get; }
    }
}
