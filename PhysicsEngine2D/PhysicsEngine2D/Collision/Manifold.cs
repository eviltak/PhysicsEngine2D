using System;

using Microsoft.Xna.Framework;

namespace PhysicsEngine2D
{
    internal class Manifold : IEquatable<Manifold>
    {
        public Body A, B;
        public float penetration;
        public Vector2 normal;

        //Accumulated impulse for less jitter
        public float accumImpulse;
        public float accumFriction;

        //Since only one contact is generated
        public bool colliding;

        //Bias based on penetration of bodies
        float bias;

        public Manifold(Body A, Body B)
        {
            this.A = A;
            this.B = B;
            penetration = accumFriction = accumImpulse = bias = 0;
            normal = default(Vector2);
            colliding = false;
        }

        public void Collide()
        {
            //Check whether colliding and fill data in us
            colliding = Collision.CollisionCallbacks[(int)A.shape.type][(int)B.shape.type](this);
        }

        // Step before applying impulse for Warm Start (Temporal Coherence)
        public void PreStep(float inv_dt)
        {
            if (!colliding) return;

            const float k_allowedPenetration = 0.05f;
            float k_biasFactor = 0.3f;

            Vector2 tangent = MathUtil.Cross(normal, 1);

            //Move bodies further if they are penetrating
            bias = -k_biasFactor * inv_dt * MathHelper.Min(0.0f, -penetration + k_allowedPenetration);

            //Warm Start! (Temporal Coherence)
            Vector2 p = accumImpulse * normal + accumFriction * tangent;

            A.velocity -= A.inverseMass * p;
            B.velocity += B.inverseMass * p;
        }

        public void ApplyImpulse()
        {
            if (!colliding) return;
            if (A.inverseMass + B.inverseMass == 0) return;

            Vector2 rv = B.velocity - A.velocity;

            //Calculate relative velocity in terms of the normal direction
            float velAlongNormal = Vector2.Dot(rv, normal);

            //Calculate restitution (currently removed)
            float e = 0;//(A.restitution + B.restitution) / 2;

            //Calculate impulse scalar
            float j = -(1 + e) * velAlongNormal + bias;
            j /= A.inverseMass + B.inverseMass;

            //Find impulse to apply after clamping since we applied the accumulated impulse in the warm start
            float pn0 = accumImpulse;
            accumImpulse = MathHelper.Max(pn0 + j, 0);
            j = accumImpulse - pn0;

            A.velocity -= A.inverseMass * j * normal;
            B.velocity += B.inverseMass * j * normal;

            //Friction start

            //Get tangent perpendicular to normal by crossing
            Vector2 tangent = MathUtil.Cross(normal, 1);

            if (tangent.LengthSquared() > 0)
                tangent.Normalize();

            //Solve for magnitude to apply along the friction vector
            float jt = -Vector2.Dot(rv, tangent);
            jt /= A.inverseMass + B.inverseMass;

            //Use to approximate mu given friction coefficients of each body
            float mu = (A.friction + B.friction) / 2;

            //Accumulated friction impulse clamp and applicaton
            float maxPt = accumImpulse * mu;
            float pt0 = accumFriction;
            accumFriction = MathHelper.Clamp(pt0 + jt, -maxPt, maxPt);
            jt = accumFriction - pt0;

            // Apply
            A.velocity -= A.inverseMass * jt * tangent;
            B.velocity += B.inverseMass * jt * tangent;
        }

        //Required for BroadPhase
        public bool Equals(Manifold other)
        {
            return (other.A.Equals(A) && other.B.Equals(B) || other.A.Equals(B) && other.B.Equals(A));
        }

        //Required for BroadPhase
        public override int GetHashCode()
        {
            return A.GetHashCode() + B.GetHashCode();
        }
    }
}