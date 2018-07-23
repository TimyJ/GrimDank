using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrimDank.MObjects.Components
{
    class Component
    {
        public MObject Parent { get; }

        public Component(MObject parent)
        {
            Parent = parent;
        }
    }
}
