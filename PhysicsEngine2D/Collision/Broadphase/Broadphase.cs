using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhysicsEngine2D
{
    public abstract class Broadphase
    {
        public abstract void Add(Body body);
        public abstract void Remove(Body body);
        public abstract void Update(List<Body> bodies);
        internal abstract void ComputePairs(List<Body> bodies, HashSet<Manifold> manifolds);
    }
}
