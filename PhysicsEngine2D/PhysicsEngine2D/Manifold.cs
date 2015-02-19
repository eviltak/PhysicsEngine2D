using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;

namespace PhysicsEngine2D
{
    public class Manifold
    {
        public Body A, B;
        public float penetration;
        public Vector2 normal;

        public Manifold(Body A, Body B)
        {
            this.A = A;
            this.B = B;

            CollisionCallbacks = new CollisionCheck[][] {
                new CollisionCheck[] { AABBToAABB, AABBToCircle },
                new CollisionCheck[] { CircleToAABB, CircleToCircle }
            };
        }

        delegate bool CollisionCheck();

        CollisionCheck[][] CollisionCallbacks;

        public void Solve()
        {
            if (CollisionCallbacks[(int)A.shape.type][(int)B.shape.type]())
                ApplyImpulse();
        }

        bool CircleToCircle()
        {
            Circle shapeA = A.shape as Circle;
            Circle shapeB = B.shape as Circle;

            Vector2 n = shapeB.body.position - shapeA.body.position;
            float r = shapeA.radius + shapeB.radius;

            if (n.LengthSquared() > r * r)
                return false;

            float d = n.Length();

            if (d != 0) {
                penetration = r - d;
                normal = n / d;
            }
            else {
                penetration = shapeA.radius;
                normal = Vector2.UnitY;
            }

            return true;
        }


        bool AABBToAABB()
        {
            AABB shapeA = A.shape as AABB;
            AABB shapeB = B.shape as AABB;

            Vector2 n = shapeB.body.position - shapeA.body.position;

            float aExtent = (shapeA.max.X - shapeA.min.X) / 2;
            float bExtent = (shapeB.max.X - shapeB.min.X) / 2;

            // Calculate overlap on x axis
            float overlapX = aExtent + bExtent - Math.Abs(n.X);

            if (overlapX > 0) {
                // Calculate half extents along x axis for each object
                aExtent = (shapeA.max.Y - shapeA.min.Y) / 2;
                bExtent = (shapeB.max.Y - shapeB.min.Y) / 2;

                // Calculate overlap on Y axis
                float overlapY = aExtent + bExtent - Math.Abs(n.Y);

                // SAT test on Y axis
                if (overlapY > 0) {
                    // Find out which axis is axis of least penetration
                    if (overlapX < overlapY) {
                        // Point towards B knowing that n points from A to B
                        if (n.X < 0)
                            normal = -Vector2.UnitX;
                        else
                            normal = Vector2.UnitX;
                        penetration = overlapX;
                        return true;
                    }
                    else {
                        // Point toward B knowing that n points from A to B
                        if (n.Y < 0)
                            normal = -Vector2.UnitY;
                        else
                            normal = Vector2.UnitY;
                        penetration = overlapY;
                        return true;
                    }
                }
            }

            return false;
        }

        bool AABBToCircle()
        {
            AABB shapeA = A.shape as AABB;
            Circle shapeB = B.shape as Circle;

            Vector2 n = shapeB.body.position - shapeA.body.position;

            // Closest point on A to center of B
            Vector2 closest = n;

            // Calculate half eXtents along each aXis
            float extentX = (shapeA.max.X - shapeA.min.X) / 2;
            float extentY = (shapeA.max.Y - shapeA.min.Y) / 2;

            // Clamp point to edges of the AABB
            closest.X = MathHelper.Clamp(closest.X, -extentX, extentX);
            closest.Y = MathHelper.Clamp(closest.Y, -extentY, extentY);

            bool inside = false;

            // Circle is inside the AABB, so we need to Math.Clamp the circle's center
            // to the closest edge
            if (n == closest) {
                inside = true;

                // Find closest aXis
                if (Math.Abs(n.X) > Math.Abs(n.Y)) {
                    // Math.Clamp to closest eXtent
                    if (closest.X > 0)
                        closest.X = extentX;
                    else
                        closest.X = -extentX;
                }

                // Y aXis is shorter
                else {
                    // Math.Clamp to closest eXtent
                    if (closest.Y > 0)
                        closest.Y = extentY;
                    else
                        closest.Y = -extentY;
                }
            }

            n -= closest;
            float d = n.LengthSquared();
            float r = shapeB.radius;

            // EarlY out of the radius is shorter than distance to closest point and
            // Circle not inside the AABB
            if (d > r * r && !inside)
                return false;

            // Avoided sqrt until we needed
            d = n.Length();

            // Collision normal needs to be flipped to point outside if circle was
            // inside the AABB
            if (inside) {
                normal = -n / d;
                penetration = r - d;
            }
            else {
                normal = n / d;
                penetration = r - d;
            }

            return true;
        }

        bool CircleToAABB()
        {
            var temp = B;
            B = A;
            A = temp;

            return AABBToCircle();
        }

        void ApplyImpulse()
        {
            Vector2 rv = B.velocity - A.velocity;

            // Calculate relative velocity in terms of the normal direction
            float velAlongNormal = Vector2.Dot(rv, normal);

            // Do not resolve if velocities are separating
            if (velAlongNormal > 0)
                return;

            // Calculate restitution
            float e = (A.restitution + B.restitution) / 2;

            // Calculate impulse scalar
            float j = -(1 + e) * velAlongNormal;
            j /= A.inverseMass + B.inverseMass;

            // Apply impulse
            Vector2 impulse = j * normal;
            A.velocity -= A.inverseMass * impulse;
            B.velocity += B.inverseMass * impulse;

            //Friction start
            rv = B.velocity - A.velocity;

            // Solve for the tangent vector
            Vector2 tangent = rv - Vector2.Dot(rv, normal) * normal;

            if (tangent.LengthSquared() > 0)
                tangent.Normalize();

            // Solve for magnitude to apply along the friction vector
            float jt = -Vector2.Dot(rv, tangent);
            jt /= A.inverseMass + B.inverseMass;

            // Use to approximate mu given friction coefficients of each body
            float mu = (A.staticFriction + B.staticFriction) / 2;

            // Clamp magnitude of friction and create impulse vector
            Vector2 frictionImpulse;
            if (Math.Abs(jt) < j * mu)
                frictionImpulse = jt * tangent;
            else {
                float dynamicFriction = (A.dynamicFriction + B.dynamicFriction) / 2;
                frictionImpulse = -j * tangent * dynamicFriction;
            }

            // Apply
            A.velocity -= A.inverseMass * frictionImpulse;
            B.velocity += B.inverseMass * frictionImpulse;

            PositionalCorrection();
        }

        void PositionalCorrection()
        {
            const float percent = 0.5f; // usually 20% to 80%
            const float slop = 0.01f; // usually 0.01 to 0.1

            Vector2 correction = Math.Max(penetration - slop, 0.0f) / (A.inverseMass + B.inverseMass) * percent * normal;
            A.position -= A.inverseMass * correction;
            B.position += B.inverseMass * correction;
        }
    }
}