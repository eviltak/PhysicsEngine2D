
namespace PhysicsEngine2D
{
    public class Body
    {
        public Shape shape;

        public Bounds bounds;

        public Transform transform;

        public Vec2 velocity;
        public float angularVelocity;

        public Vec2 force;
        public float torque;

        public float inverseMass;
        public float inverseInertia;

        public float restitution;
        public float friction;

        public float gravityScale;

        // For use in broadphase
        public object data;

        public Vec2 position
        {
            get
            {
                return transform.position;
            }
        }

        public Body(Shape shape, Vec2 position, float rotation = 0,  float friction = 0.1f, float gravityScale = 1)
        {
            this.shape = shape;
            this.shape.body = this;

            transform = new Transform(position, rotation);
            this.shape.transform = transform;

            this.friction = friction;

            this.gravityScale = gravityScale;
            
            shape.ComputeMass(1);

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

            force = Vec2.Zero;
            torque = 0;
        }

        public void IntegrateVelocity(float dt)
        {
            if (inverseMass == 0)
                return;
            
            transform.position += velocity * dt;
            transform.rotation += angularVelocity * dt;

            transform.UpdateMatrix();
        }

        public void Update()
        {
            //Update bounds for Broad phase
            bounds = shape.GetBoundingBox();
        }

        public void ApplyImpulse(Vec2 impulse, Vec2 contactRadius)
        {
            velocity += impulse * inverseMass;
            angularVelocity += inverseInertia * Vec2.Cross(contactRadius, impulse);
        }
    }
}
