using System;

using Microsoft.Xna.Framework;

namespace PhysicsEngine2D
{
    static class Collision
    {
        //2D jump table for easy narrow phase function call
        internal delegate void CollisionCheck( Manifold m);

        internal static CollisionCheck[][] CollisionCallbacks = new CollisionCheck[][] {
                new CollisionCheck[] { PolygonToPolygon, PolygonToCircle },
                new CollisionCheck[] { CircleToPolygon, CircleToCircle }
        };

        public static void CircleToCircle( Manifold m)
        {
            Circle shapeA = m.A.shape as Circle;
            Circle shapeB = m.B.shape as Circle;

            m.contactCount = 0;

            Vector2 n = shapeB.body.position - shapeA.body.position;
            float r = shapeA.radius + shapeB.radius;

            // If distance greater than sum of radii? Then exit
            if (n.LengthSquared() > r * r)
                return;

            float d = n.Length();
            
            Contact contact;

            if (d != 0)
            {
                contact = new Contact(m.normal * shapeA.radius + m.A.position);
                contact.penetration = r - d;
                m.normal = n / d;
            }
            else
            {
                contact = new Contact(m.A.position);
                // Circles are concentric, take a consistent value
                contact.penetration = shapeA.radius;
                m.normal = Vector2.UnitY;
            }

            m.Update(1, contact);
        }
        
        public static void PolygonToPolygon( Manifold m)
        {
            Polygon shapeA = m.A.shape as Polygon;
            Polygon shapeB = m.B.shape as Polygon;

            m.contactCount = 0;

            //Check for separating axis on A's normals
            int faceA;
            float penetrationA = FindLeastPenetrationAxis(shapeA, shapeB, out faceA);

            if (penetrationA >= 0) return;

            //Check for separating axis on B's normals
            int faceB;
            float penetrationB = FindLeastPenetrationAxis(shapeB, shapeA, out faceB);

            if (penetrationB >= 0) return;

            int referenceIndex;
            //Make sure we always point from A to B for consistent results
            bool flip;


            Polygon refPoly, incPoly;

            if(MathUtil.BiasGreaterThan(penetrationA, penetrationB))
            {
                refPoly = shapeA;
                incPoly = shapeB;
                referenceIndex = faceA;
                flip = false;
            }
            else
            {
                refPoly = shapeB;
                incPoly = shapeA;
                referenceIndex = faceB;
                flip = true;
            }

            Vector2[] incidentFace;
            GetIncidentFace(out incidentFace, refPoly, incPoly, referenceIndex);

            //Set up vertices
            Vector2 v1 = refPoly.vertices[referenceIndex];
            Vector2 v2 = refPoly.vertices[(referenceIndex + 1) % refPoly.vertexCount];

            //Transform to world space
            v1 = refPoly.u * v1 + refPoly.body.position;
            v2 = refPoly.u * v2 + refPoly.body.position;

            Vector2 sidePlaneNormal = v2 - v1;
            sidePlaneNormal.Normalize();

            Vector2 refFaceNormal = new Vector2(sidePlaneNormal.Y, -sidePlaneNormal.X);
            
            float refC = Vector2.Dot(refFaceNormal, v1);
            float negSide = -Vector2.Dot(sidePlaneNormal, v1);
            float posSide = Vector2.Dot(sidePlaneNormal, v2);

            // Due to floating point error, possible to not have required points
            if (Clip(-sidePlaneNormal, negSide, ref incidentFace) < 2)
                return; 

            if (Clip(sidePlaneNormal, posSide, ref incidentFace) < 2)
                return;

            m.normal = flip ? -refFaceNormal : refFaceNormal;

            Contact[] contacts = new Contact[2];

            // Keep points behind reference face
            int cp = 0; // clipped points behind reference face
            for (int i = 0; i < 2; i++)
            {
                float separation = Vector2.Dot(refFaceNormal, incidentFace[i]) - refC;
                if (separation <= 0.0f)
                {
                    contacts[cp] = new Contact(incidentFace[i]);
                    contacts[cp].penetration = -separation;
                    ++cp;
                }
            }

            m.contactCount = cp;
            m.Update(cp, contacts);
        }

        private static float FindLeastPenetrationAxis(Polygon A, Polygon B, out int face)
        {
            float bestDistance = float.MinValue;
            face = 0;

            for (int i = 0; i < A.vertexCount; i++)
            {
                //Get world space normal of A's face
                Vector2 n = A.normals[i];
                Vector2 nW = A.u * n;

                //Retrieve normal in B's spac
                n = B.uT * nW;

                //Get furthest point in negative normal direction
                Vector2 s = B.GetSupportPoint(-n);

                //Get vertex on A's face, transform to B's space
                Vector2 v = A.vertices[i];
                v = A.u * v + A.body.position;
                v -= B.body.position;
                v = B.uT * v;

                //Find penetration distance
                float d = Vector2.Dot(s - v, n);

                //Store greatest distance
                if(d > bestDistance)
                {
                    bestDistance = d;
                    face = i;
                }
            }

            return bestDistance;
        }

        private static void GetIncidentFace(out Vector2[] face, 
            Polygon refP, Polygon incP, int referenceIndex)
        {
            face = new Vector2[2];
            //Retrieve the reference normal
            Vector2 n = refP.normals[referenceIndex];

            //Transform into incident poly's space
            n = refP.u * n;    //To world space
            n = incP.uT * n;    //To incident poly's space

            //Find face whose normal is most normal (perpendicular) to the normal (oh...)
            int incidentFace = -1;
            float minDot = float.MaxValue;

            for(int i = 0; i < incP.vertexCount; i++)
            {
                float dot = Vector2.Dot(n, incP.normals[i]);
                if (dot < minDot)
                {
                    minDot = dot;
                    incidentFace = i;
                }
            }

            //Get world space face
            face[0] = incP.u * incP.vertices[incidentFace] + incP.body.position;
            face[1] = incP.u * incP.vertices[(incidentFace + 1) % incP.vertexCount]
                + incP.body.position;
        }

        private static int Clip(Vector2 n, float c, ref Vector2[] face)
        {
            int clippedPoints = 0;
            Vector2[] res = new Vector2[] { face[0], face[1] };

            //Get distance to line (ax + by = -c, n = (a, b))
            float d1 = Vector2.Dot(n, face[0]) - c;
            float d2 = Vector2.Dot(n, face[1]) - c;

            //If behind plane, clip
            //Here we aren't actually clipping, but we are just incrementing the count
            if (d1 <= 0) res[clippedPoints++] = face[0];
            if (d2 <= 0) res[clippedPoints++] = face[1];

            //Check whether one of the points is ahead and other behind
            //(-) * (+) = (-)
            if ((d1 * d2) < 0)
            {
                //Intersect
                float t = d1 / (d1 - d2);
                res[clippedPoints] = face[0] + (face[1] - face[0]) * t;
                clippedPoints++;
            }

            face[0] = res[0];
            face[1] = res[1];

            return clippedPoints;
        }

        public static void CircleToPolygon(Manifold m)
        {
            Circle shapeA = m.A.shape as Circle;
            Polygon shapeB = m.B.shape as Polygon;

            m.contactCount = 0;

            // Transform circle center to Polygon model space
            Vector2 center = m.A.position;
            center = shapeB.u.Transpose() * (center - m.B.position);

            // Find edge with minimum penetration
            // Exact concept as using support points in Polygon vs Polygon
            float separation = float.MinValue;
            int faceNormal = 0;
            for (int i = 0; i < shapeB.vertexCount; ++i)
            {
                float s = Vector2.Dot(shapeB.normals[i], center - shapeB.vertices[i]);

                if (s > shapeA.radius)
                    return;

                if (s > separation)
                {
                    separation = s;
                    faceNormal = i;
                }
            }

            // Grab face's vertices
            Vector2 v1 = shapeB.vertices[faceNormal];
            int i2 = faceNormal + 1 < shapeB.vertexCount ? faceNormal + 1 : 0;
            Vector2 v2 = shapeB.vertices[i2];

            Contact c = new Contact(Vector2.Zero);

            // Check to see if center is within polygon
            if (separation < float.Epsilon)
            {
                m.normal = -(shapeB.u * shapeB.normals[faceNormal]);
                c.position = m.normal * shapeA.radius + m.A.position;
                c.penetration = shapeA.radius;
                m.Update(1, c);
                return;
            }

            // Determine which voronoi region of the edge center of circle lies within
            float dot1 = Vector2.Dot(center - v1, v2 - v1);
            float dot2 = Vector2.Dot(center - v2, v1 - v2);
            c.penetration = shapeA.radius - separation;

            // Closest to v1
            if (dot1 <= 0.0f)
            {
                if (Vector2.DistanceSquared(center, v1) > shapeA.radius * shapeA.radius)
                    return;
                
                Vector2 n = v1 - center;
                n = shapeB.u * n;
                n.Normalize();
                m.normal = n;

                v1 = shapeB.u * v1 + m.B.position;
                c.position = v1;
                m.Update(1, c);
            }

            // Closest to v2
            else if (dot2 <= 0.0f)
            {
                if (Vector2.DistanceSquared(center, v2) > shapeA.radius * shapeA.radius)
                    return;

                Vector2 n = v2 - center;
                v2 = shapeB.u * v2 + m.B.position;
                c.position = v2;
                m.Update(1, c);

                n = shapeB.u * n;
                n.Normalize();
                m.normal = n;
            }

            // Closest to face
            else
            {
                Vector2 n = shapeB.normals[faceNormal];
                if (Vector2.Dot(center - v1, n) > shapeA.radius)
                    return;

                n = shapeB.u * n;
                m.normal = -n;
                c.position = m.normal * shapeA.radius + m.A.position;
                m.Update(1, c);
            }
        }

        public static void PolygonToCircle( Manifold m)
        {
            //Just switching the input so that we can pass it to the function above
            var temp = m.B;
            m.B = m.A;
            m.A = temp;

            CircleToPolygon( m);

            //Make sure that normal always points from A to B
            m.normal = -m.normal;
        }
    }
}
