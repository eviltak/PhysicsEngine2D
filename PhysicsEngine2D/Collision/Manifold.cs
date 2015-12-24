using System;

namespace PhysicsEngine2D
{
    internal class Contact
    {
        public Vec2 position;
        public float accumImpulse;
        public float accumFriction;

        //Bias based on penetration of bodies
        public float penetration;
        public float bias;

        public float normalMass;
        public float tangentMass;

        public Contact(Vec2 position, float impulse = 0, float friction = 0, float penetration = 0)
        {
            this.position = position;
            accumFriction = friction;
            accumImpulse = impulse;
            this.penetration = penetration;
        }

        public Contact Clone()
        {
            return new Contact(position, accumImpulse, accumFriction, penetration);
        }
    }

    internal class Manifold : IEquatable<Manifold>
    {
        public readonly Body bodyA;
        public readonly Body bodyB;
        public Vec2 normal;

        //We only need two contact points
        public Contact[] contacts = new Contact[2];
        public int contactCount;

        public Manifold(Body bodyA, Body bodyB)
        {
            this.bodyA = bodyA;
            this.bodyB = bodyB;
            normal = default(Vec2);
        }

        public void Update(int numNewContacts, params Contact[] newContacts)
        {
            Contact[] mergedContacts = new Contact[2];

            for(int i = 0; i < numNewContacts; i++)
            {
                Contact cOld = contacts[i];
                Contact cNew = newContacts[i];
                mergedContacts[i] = cNew.Clone();

                if (cOld != null)
                {
                    mergedContacts[i].accumFriction = cOld.accumFriction;
                    mergedContacts[i].accumImpulse = cOld.accumImpulse;
                }
            }

            for (int i = 0; i < numNewContacts; ++i)
                contacts[i] = mergedContacts[i].Clone();

            contactCount = numNewContacts;
        }

        public void SolveContacts()
        {
            //Check whether colliding and fill data in us
            ContactSolver.solveContacts[(int)bodyA.shape.type][(int)bodyB.shape.type](this, bodyA, bodyB);
        }

        // Step before applying impulse for Accumulated impulse
        public void PreStep(float invDt)
        {
            if (bodyA.inverseMass + bodyB.inverseMass == 0) return;

            const float KAllowedPenetration = 0.01f;
            const float KBiasFactor = 0.05f;

            for (int i = 0; i < contactCount; i++)
            {
                Contact c = contacts[i];

                if (c == null) continue;

                Vec2 r1 = c.position - bodyA.position;
                Vec2 r2 = c.position - bodyB.position;

                Vec2 tangent = Vec2.Cross(normal, 1);

                float rn1 = Vec2.Dot(r1, normal);
                float rn2 = Vec2.Dot(r2, normal);
                float inverseMassSum = bodyA.inverseMass + bodyB.inverseMass;

                c.normalMass = inverseMassSum + bodyA.inverseInertia * (Vec2.Dot(r1, r1) - rn1 * rn1) + 
                    bodyB.inverseInertia * (Vec2.Dot(r2, r2) - rn2 * rn2);
                c.normalMass = 1 / c.normalMass;

                float rt1 = Vec2.Dot(r1, tangent);
                float rt2 = Vec2.Dot(r2, tangent);

                c.tangentMass = inverseMassSum + bodyA.inverseInertia * (Vec2.Dot(r1, r1) - rt1 * rt1) +
                    bodyB.inverseInertia * (Vec2.Dot(r2, r2) - rt2 * rt2);
                c.tangentMass = 1 / c.tangentMass;

                //Move bodies further if they are penetrating
                c.bias = KBiasFactor * invDt * Math.Max(0.0f, c.penetration - KAllowedPenetration);

                //Accumulated impulses
                Vec2 p = c.accumImpulse * normal + c.accumFriction * tangent;

                bodyA.ApplyImpulse(-p, r1);
                bodyB.ApplyImpulse(p, r2);
            }

        }

        public void ApplyImpulse()
        {
            if (bodyA.inverseMass + bodyB.inverseMass == 0) return;

            for (int i = 0; i < contactCount; i++) {
                Contact c = contacts[i];
                if (c == null) continue;
                Vec2 ra = c.position - bodyA.position;
                Vec2 rb = c.position - bodyB.position;

                Vec2 rv = bodyB.velocity + Vec2.Cross(bodyB.angularVelocity, rb) -
                    bodyA.velocity - Vec2.Cross(bodyA.angularVelocity, ra);

                //Calculate relative velocity in terms of the normal direction
                float velAlongNormal = Vec2.Dot(rv, normal); 

                //Calculate impulse scalar
                float j = -velAlongNormal + c.bias;
                j *= c.normalMass;

                //Find impulse to apply after clamping since we applied the accumulated impulse in the warm start
                float pn0 = c.accumImpulse;
                c.accumImpulse = Math.Max(pn0 + j, 0);
                j = c.accumImpulse - pn0;

                Vec2 pn = j * normal;
                bodyA.ApplyImpulse(-pn, ra);
                bodyB.ApplyImpulse(pn, rb);

                //Friction start

                //Get tangent perpendicular to normal by crossing
                rv = bodyB.velocity + Vec2.Cross(bodyB.angularVelocity, rb) -
                    bodyA.velocity - Vec2.Cross(bodyA.angularVelocity, ra);

                Vec2 tangent = Vec2.Cross(normal, 1);

                if (tangent.LengthSquared > 0)
                    tangent.Normalize();

                //Solve for magnitude to apply along the friction vector
                float jt = -Vec2.Dot(rv, tangent) * c.tangentMass;

                //Use to approximate mu given friction coefficients of each body
                float mu = (bodyA.friction + bodyB.friction) / 2;

                //Accumulated friction impulse clamp and applicaton
                float maxPt = c.accumImpulse * mu;
                float pt0 = c.accumFriction;
                c.accumFriction = Mathf.Clamp(pt0 + jt, -maxPt, maxPt);
                jt = c.accumFriction - pt0;

                Vec2 pt = jt * tangent;

                bodyA.ApplyImpulse(-pt, ra);
                bodyB.ApplyImpulse(pt, rb);
            }
        }

        //Required for Broad Phase
        public bool Equals(Manifold other)
        {
            return other.bodyA.Equals(bodyA) && other.bodyB.Equals(bodyB) || other.bodyA.Equals(bodyB) && other.bodyB.Equals(bodyA);
        }

        //Required for Broad Phase
        public override int GetHashCode()
        {
            return bodyA.GetHashCode() + bodyB.GetHashCode();
        }
    }
}