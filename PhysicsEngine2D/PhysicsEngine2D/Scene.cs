using System.Collections.Generic;
using System.Linq;

using Microsoft.Xna.Framework;

namespace PhysicsEngine2D
{
    public class Scene
    {
        public static Vector2 gravity = new Vector2(0, -9.8f);

        public List<Body> bodies = new List<Body>();

        internal HashSet<Manifold> manifolds = new HashSet<Manifold>();

        private CollisionSystemSAP collisionSystem;

        int maxIterations = 5;

        public Scene()
        {
            collisionSystem = new CollisionSystemSAP(manifolds);
        }

        public void AddBody(Body b)
        {
            bodies.Add(b);
        }

        public void RemoveBody(Body b)
        {
            bodies.Remove(b);
        }

        public void Update(float dt)
        {
            foreach (Body b in bodies)
                b.Update();


            //Obsolete Brute Force Collisions
            /*for (int i = 0; i < bodies.Count - 1; i++) {
                for (int j = i + 1; j < bodies.Count; j++) {
                    var key = i + ", " + j;
                    if (!manifolds.ContainsKey(key))
                    manifolds.Add(key, new Manifold(bodies[i], bodies[j]));
                }
            }*/

            //Broad phase
            collisionSystem.BroadPhase(bodies);

            //Narrow phase
            foreach (var m in manifolds)
                m.Collide();

            foreach (var b in bodies)
                b.IntegrateForces(dt);

            foreach (var m in manifolds)
                m.PreStep(1 / dt);

            //Process maxIterations times for more stability
            for (int i = 0; i < maxIterations; i++)
                foreach (var m in manifolds)
                    m.ApplyImpulse();

            //Integrate positions
            foreach (Body b in bodies)
                b.IntegrateVelocity(dt);
        }

        public void Draw()
        {
            foreach (Body b in bodies)
                b.Draw();
        }
    }
}
