using System;
using System.Collections.Generic;
using System.Text;

namespace Simcity3000_2
{
    class Entity
    {

        public Guid Guid { get; private set; }
        public Entity()
        {
            Guid = Guid.NewGuid();
        }
    }
}
