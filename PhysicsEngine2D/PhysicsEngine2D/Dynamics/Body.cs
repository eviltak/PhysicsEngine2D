
using Microsoft.Xna.Framework;

namespace PhysicsEngine2D
{
    public class Body
    {
        public Shape shape;

        public Bounds bounds;

        public Vector2 position;
        public Vector2 velocity;

        public Vector2 force;
        public float inverseMass;

        public float restitution;
        public float friction;

        public float gravityScale;

        public Body(Shape shape, Vector2 position, float mass = 1, float restitution = 0, 
            float friction = 0, float gravityScale = 1)
        {
            this.shape = shape;
            this.shape.body = this;

            this.position = position;

            if (mass != 0)
                inverseMass = 1 / mass;
            else
                inverseMass = 0;

            this.restitution = restitution;

            this.friction = friction;

            this.gravityScale = gravityScale;
        }

        public void SetStatic()
        {
            inverseMass = 0;
        }

        public void IntegrateForces(float dt)
        {
            if (inverseMass == 0)
                return;

            velocity += dt * (Scene.gravity * gravityScale + inverseMass * force);
            force = Vector2.Zero;

        }

        public void IntegrateVelocity(float dt)
        {
            if (inverseMass == 0)
                return;
            
            position += velocity * dt;
        }

        public void Draw()
        {
            shape.Draw();
        }

        public void Update()
        {
            //Update bounds for Broad phase
            bounds = shape.GetBoundingBox(position);
        }
    }
}
