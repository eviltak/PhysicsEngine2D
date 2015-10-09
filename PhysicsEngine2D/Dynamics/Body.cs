
using Microsoft.Xna.Framework;

namespace PhysicsEngine2D
{
    public class Body
    {
        public Shape shape;

        public Bounds bounds;

        public Vector2 position;
        public float orientation;

        public Vector2 velocity;
        public float angularVelocity;

        public Vector2 force;
        public float torque;

        public float inverseMass;
        public float inverseInertia;

        public float restitution;
        public float friction;

        public float gravityScale;

        public Body(Shape shape, Vector2 position, float orientation = 0,  float friction = 0.1f, float gravityScale = 1)
        {
            this.shape = shape;
            this.shape.body = this;

            this.position = position;

            this.orientation = orientation;

            this.friction = friction;

            this.gravityScale = gravityScale;

            shape.ComputeMass(1);
            shape.SetOrientation(orientation);

            bounds = shape.GetBoundingBox();
        }

        public void SetStatic()
        {
            inverseMass = 0;
            inverseInertia = 0;
        }

        public void IntegrateForces(float dt)
        {
            if (inverseMass == 0)
                return;

            velocity += dt * (PhysicsWorld.gravity * gravityScale + inverseMass * force);
            angularVelocity += dt * torque * inverseInertia;

            force = Vector2.Zero;
            torque = 0;
        }

        public void IntegrateVelocity(float dt)
        {
            if (inverseMass == 0)
                return;
            
            position += velocity * dt;
            orientation += angularVelocity * dt;

            shape.SetOrientation(orientation);
        }

        public void Draw()
        {
            shape.Draw();
        }

        public void Update()
        {
            //Update bounds for Broad phase
            bounds = shape.GetBoundingBox();
        }

        public void ApplyImpulse(Vector2 impulse, Vector2 contactRadius)
        {
            velocity += impulse * inverseMass;
            angularVelocity += inverseInertia * MathUtil.Cross(contactRadius, impulse);
        }
    }
}
