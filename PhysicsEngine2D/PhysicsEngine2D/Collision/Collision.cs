using System;

using Microsoft.Xna.Framework;

namespace PhysicsEngine2D
{
    static class Collision
    {
        //2D jump table for easy narrow phase function call
        internal delegate bool CollisionCheck( Manifold m);

        internal static CollisionCheck[][] CollisionCallbacks = new CollisionCheck[][] {
                new CollisionCheck[] { AABBToAABB, AABBToCircle },
                new CollisionCheck[] { CircleToAABB, CircleToCircle }
        };

        public static bool CircleToCircle( Manifold m)
        {
            Circle shapeA = m.A.shape as Circle;
            Circle shapeB = m.B.shape as Circle;

            Vector2 n = shapeB.body.position - shapeA.body.position;
            float r = shapeA.radius + shapeB.radius;

            // If distance greater than sum of radii? Then exit
            if (n.LengthSquared() > r * r)
                return false;

            float d = n.Length();

            if (d != 0)
            {
                m.penetration = r - d;
                m.normal = n / d;
            }
            else
            {
                // Circles are concentric, take a consistent value
                m.penetration = shapeA.radius;
                m.normal = Vector2.UnitY;
            }

            return true;
        }
        
        // Extended Separating Axis Theorem
        public static bool AABBToAABB( Manifold m)
        {
            AABB shapeA = m.A.shape as AABB;
            AABB shapeB = m.B.shape as AABB;

            Vector2 n = shapeB.body.position - shapeA.body.position;

            float aExtent = (shapeA.max.X - shapeA.min.X) / 2;
            float bExtent = (shapeB.max.X - shapeB.min.X) / 2;
            
            // Find overlap on X axis
            float overlapX = aExtent + bExtent - Math.Abs(n.X);

            if (overlapX > 0)
            {
                aExtent = (shapeA.max.Y - shapeA.min.Y) / 2;
                bExtent = (shapeB.max.Y - shapeB.min.Y) / 2;
                
                // Overlap on Y axis
                float overlapY = aExtent + bExtent - Math.Abs(n.Y);
                
                if (overlapY > 0)
                {
                    // Find axis of greatest penetration
                    if (overlapX < overlapY)
                    {
                        if (n.X < 0)
                            m.normal = -Vector2.UnitX;
                        else
                            m.normal = Vector2.UnitX;
                        m.penetration = overlapX;
                        return true;
                    }
                    else
                    {
                        if (n.Y < 0)
                            m.normal = -Vector2.UnitY;
                        else
                            m.normal = Vector2.UnitY;
                        m.penetration = overlapY;
                        return true;
                    }
                }
            }

            return false;
        }

        public static bool AABBToCircle( Manifold m)
        {
            AABB shapeA = m.A.shape as AABB;
            Circle shapeB = m.B.shape as Circle;

            Vector2 n = shapeB.body.position - shapeA.body.position;
            
            Vector2 closest = n;
            
            float extentX = (shapeA.max.X - shapeA.min.X) / 2;
            float extentY = (shapeA.max.Y - shapeA.min.Y) / 2;
            
            closest.X = MathHelper.Clamp(closest.X, -extentX, extentX);
            closest.Y = MathHelper.Clamp(closest.Y, -extentY, extentY);

            bool inside = false;

            // Circle is inside the AABB, so we need to clamp the circle's center to the closest edge
            if (n == closest)
            {
                inside = true;

                // Find closest axis
                if (Math.Abs(n.X) > Math.Abs(n.Y))
                {
                    if (closest.X > 0)
                        closest.X = extentX;
                    else
                        closest.X = -extentX;
                }

                // Y axis is shorter
                else
                {
                    if (closest.Y > 0)
                        closest.Y = extentY;
                    else
                        closest.Y = -extentY;
                }
            }

            n -= closest;
            float d = n.LengthSquared();
            float r = shapeB.radius;

            // Out if distance is greater than radius of circle and circle not inside AABB
            if (d > r * r && !inside)
                return false;

            // Avoided sqrt until we needed
            d = n.Length();

            // Collision m.normal needs to be flipped to point outside if circle was inside the AABB
            if (inside)
            {
                m.normal = -n / d;
                m.penetration = r - d;
            }
            else
            {
                m.normal = n / d;
                m.penetration = r - d;
            }

            return true;
        }

        public static bool CircleToAABB( Manifold m)
        {
            //Just switching the input so that we can pass it to the function above
            var temp = m.B;
            m.B = m.A;
            m.A = temp;

            return AABBToCircle( m);
        }
    }
}
