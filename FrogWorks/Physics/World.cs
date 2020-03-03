using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrogWorks
{
    public sealed class World
    {
        DynamicAABBTree _broadphaseTree;

        internal World()
        {
            _broadphaseTree = new DynamicAABBTree(8f);
        }

        internal void AddCollider(Collider collider)
        {
            _broadphaseTree.Add(collider);
        }

        internal void UpdateCollider(Collider collider)
        {
            _broadphaseTree.Update(collider);
        }

        internal void RemoveCollider(Collider collider)
        {
            _broadphaseTree.Remove(collider);
        }

        internal void Reset()
        {
            _broadphaseTree.Clear();
        }
    }
}
