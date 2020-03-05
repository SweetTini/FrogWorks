using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace FrogWorks
{
    public class CollisionResult
    {
        List<Manifold> _hits;

        public Collider Collider { get; private set; }

        public ReadOnlyCollection<Manifold> Hits { get; private set; }

        private CollisionResult()
            : base()
        {
            _hits = new List<Manifold>();
            Hits = new ReadOnlyCollection<Manifold>(_hits);
        }

        internal CollisionResult(Collider collider)
            : this()
        {
            Collider = collider;
        }

        internal void Add(Manifold hit)
        {
            hit.Collider = Collider;
            _hits.Add(hit);
        }
    }
}
