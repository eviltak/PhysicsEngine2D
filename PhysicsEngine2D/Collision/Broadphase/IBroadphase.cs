using System.Collections.Generic;
using System;

namespace PhysicsEngine2D
{
    internal interface IBroadphase
    {
        void Add(Body body);
        void Remove(Body body);
        void Update(List<Body> bodies);

        bool Raycast(Ray2 ray, float distance, out RaycastResult result);

        void ComputePairs(List<Body> bodies, HashSet<Manifold> manifolds);
        void Clear();

        void DebugDraw(IDebugDrawer drawer);
    }
}
