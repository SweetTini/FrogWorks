using System;
using System.Collections;
using System.Collections.Generic;

namespace FrogWorks
{
    public abstract class Manager<C, P> : IEnumerable<C>, IEnumerable
        where C : Manageable<P>
        where P : class
    {
        ManagerState _state;

        protected P Parent { get; private set; }

        protected List<C> Children { get; private set; }

        protected Queue<ManagerCommand<C, P>> Commands { get; private set; }

        public ManagerState State
        {
            get { return _state; }
            set
            {
                _state = value;
                ProcessQueues();
            }
        }

        public C this[int index]
        {
            get
            {
                return Children.WithinRange(index)
                    ? Children[index]
                    : null;
            }
        }

        public int Count => Children.Count;

        protected Manager(P parent)
        {
            Parent = parent;
            Children = new List<C>();
            Commands = new Queue<ManagerCommand<C, P>>();
        }

        void ProcessQueues()
        {
            while (Commands.Count > 0)
            {
                var command = Commands.Dequeue();

                switch (command.Type)
                {
                    case ManagerCommandType.Add:
                        TryAdd(command.Child);
                        break;
                    case ManagerCommandType.Remove:
                        TryRemove(command.Child);
                        break;
                }
            }

            PostProcessQueues();
        }

        protected virtual void PostProcessQueues()
        {
        }

        internal virtual void Update(float deltaTime)
        {
            State = ManagerState.Queue;

            C child;

            for (int i = 0; i < Children.Count; i++)
            {
                child = Children[i];
                
                if (child.IsActive)
                    child.UpdateInternally(deltaTime);
            }

            State = ManagerState.Opened;
        }

        internal virtual void Draw(RendererBatch batch)
        {
            State = ManagerState.ThrowError;

            C child;

            for (int i = 0; i < Children.Count; i++)
            {
                child = Children[i];

                if (child.IsVisible)
                    child.DrawInternally(batch);
            }

            State = ManagerState.Opened;
        }

        public void Add(C child)
        {
            if (child != null)
            {
                switch (State)
                {
                    case ManagerState.Opened:
                        TryAdd(child);
                        break;
                    case ManagerState.Queue:
                        var command = new ManagerCommand<C, P>(child, ManagerCommandType.Add);
                        Commands.Enqueue(command);
                        break;
                    case ManagerState.ThrowError:
                        throw new Exception($"Cannot add {typeof(C).Name} at this time.");
                }
            }
        }

        public void Add(params C[] children)
        {
            for (int i = 0; i < children.Length; i++)
                Add(children[i]);
        }

        public void Add(IEnumerable<C> children)
        {
            foreach (var child in children)
                Add(child);
        }

        public void Remove(C child)
        {
            if (child != null)
            {
                switch (State)
                {
                    case ManagerState.Opened:
                        TryRemove(child);
                        break;
                    case ManagerState.Queue:
                        var command = new ManagerCommand<C, P>(child, ManagerCommandType.Remove);
                        Commands.Enqueue(command);
                        break;
                    case ManagerState.ThrowError:
                        throw new Exception($"Cannot remove {typeof(C).Name} at this time.");
                }
            }
        }

        public void Remove(params C[] children)
        {
            for (int i = 0; i < children.Length; i++)
                Remove(children[i]);
        }

        public void Remove(IEnumerable<C> children)
        {
            foreach (var child in children)
                Remove(child);
        }

        protected void TryAdd(C child)
        {
            if (!Children.Contains(child))
            {
                Children.Add(child);
                child.OnAddedInternally(Parent);
                OnChildrenUpdated();
            }
        }

        protected void TryRemove(C child)
        {
            if (Children.Contains(child))
            {
                Children.Remove(child);
                child.OnRemovedInternally();
                OnChildrenUpdated();
            }
        }

        protected virtual void OnChildrenUpdated()
        {
        }

        public bool Contains(C child)
        {
            return Children.Contains(child);
        }

        public int IndexOf(C child)
        {
            return Children.IndexOf(child);
        }

        public IEnumerator<C> GetEnumerator()
        {
            return Children.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public struct ManagerCommand<C, P>
        where C : Manageable<P>
        where P : class
    {
        public C Child { get; }

        public ManagerCommandType Type { get; }

        internal ManagerCommand(C child, ManagerCommandType type)
            : this()
        {
            Child = child;
            Type = type;
        }
    }

    public enum ManagerState
    {
        Opened,
        Queue,
        ThrowError
    }

    public enum ManagerCommandType
    {
        Add,
        Remove
    }
}
