using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrogWorks
{
    public sealed class EntityManager : Manager<Entity, Scene>
    {
        internal EntityManager(Scene scene)
            : base(scene)
        {
        }
    }
}
