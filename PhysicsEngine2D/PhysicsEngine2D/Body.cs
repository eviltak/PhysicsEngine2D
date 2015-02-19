using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;

namespace Physics2DTutorial
{
    public class Body
    {
        public Shape shape;

        public Vector2 position;
        public Vector2 velocity;

        public float mass;
        public float inverseMass;

        public float restitution;

        public float staticFriction;
        public float dynamicFriction;

        public float gravityScale;

        public Body(Shape shape, Vector2 position, float mass = 1, float restitution = 0, 
            float staticFriction = 0, float dynamicFriction = 0, float gravityScale = 1)
        {
            this.shape = shape;
            this.shape.body = this;

            this.position = position;

            this.mass = mass;

            if (mass != 0)
                inverseMass = 1 / mass;
            else
                inverseMass = 0;

            this.restitution = restitution;

            this.staticFriction = staticFriction;
            this.dynamicFriction = dynamicFriction;

            this.gravityScale = gravityScale;
        }

        public void SetStatic()
        {
            inverseMass = 0;
        }

        public void Update(float dt)
        {
            if (inverseMass == 0)
                return;

            velocity += Scene.gravity * dt * gravityScale;
            position += velocity * dt;
        }

        public void Draw()
        {
            shape.Draw();
        }
    }
}
