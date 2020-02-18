using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrogWorks
{
    public sealed class ComponentManager : Manager<Component, Entity>
    {
        internal ComponentManager(Entity entity)
            : base(entity)
        {
        }
    }
}
