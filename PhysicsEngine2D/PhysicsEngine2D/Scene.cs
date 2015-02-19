using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;

namespace Physics2DTutorial
{
    public class Scene
    {
        public static Vector2 gravity = new Vector2(0, 300f);

        public List<Body> bodies = new List<Body>();

        public List<Manifold> manifolds = new List<Manifold>();

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
            for (int i = 0; i < bodies.Count - 1; i++) {
                for (int j = i + 1; j < bodies.Count; j++) {
                    manifolds.Add(new Manifold(bodies[i], bodies[j]));
                }
            }

            for (int i = 0; i < 5; i++)
                foreach (Manifold m in manifolds)
                    m.Solve();

            foreach (Body b in bodies)
                b.Update(dt);

            manifolds.Clear();
        }

        public void Draw()
        {
            foreach (Body b in bodies)
                b.Draw();
        }
    }
}
