using System.Collections.Generic;
using System.Linq;

namespace FrogWorks
{
    public sealed class EntityManager : SortingManager<Entity, Scene>
    {
        List<Entity> _childrenByPriority;
        bool _unsorted;

        internal EntityManager(Scene scene)
            : base(scene)
        {
            _childrenByPriority = new List<Entity>();
        }

        internal void MarkAsUnsorted()
        {
            _unsorted = true;
        }

        internal override void Update(float deltaTime)
        {
            State = ManagerState.Queue;

            foreach (var child in _childrenByPriority)
                if (child.IsActive)
                    child.UpdateInternally(deltaTime);

            State = ManagerState.Opened;
        }

        protected override void PostProcessQueues()
        {
            base.PostProcessQueues();

            if (_unsorted)
            {
                _childrenByPriority = Children.OrderBy(x => x.Priority).ToList();
                _unsorted = false;
            }
        }

        protected override void OnChildrenUpdated()
        {
            MarkAsUnsorted();
        }
    }
}
