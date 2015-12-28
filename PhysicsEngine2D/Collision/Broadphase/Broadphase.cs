using System.Collections.Generic;

namespace PhysicsEngine2D
{
    public abstract class Broadphase
    {
        public abstract void Add(Body body);
        public abstract void Remove(Body body);
        public abstract void Update(List<Body> bodies);

        public abstract bool Raycast(Ray2 ray, float distance, out RaycastResult result);

        internal abstract void ComputePairs(List<Body> bodies, HashSet<Manifold> manifolds);
        public abstract void Clear();
    }
}
