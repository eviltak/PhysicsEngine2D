namespace PhysicsEngine2D
{
    internal static class ContactSolver
    {
        //2D jump table for easy narrow phase function call
        internal delegate void SolveContactCallback(Manifold m, Body a, Body b);

        internal static SolveContactCallback[][] solveContacts = {
                new SolveContactCallback[] { PolygonToPolygon, PolygonToCircle },
                new SolveContactCallback[] { CircleToPolygon, CircleToCircle }
        };

        public static void CircleToCircle(Manifold m, Body a, Body b)
        {
            Circle shapeA = a.shape as Circle;
            Circle shapeB = b.shape as Circle;

            m.contactCount = 0;

            Vec2 n = shapeB.body.position - shapeA.body.position;
            float r = shapeA.radius + shapeB.radius;

            // If distance greater than sum of radii Then exit
            if (n.LengthSquared > r * r)
                return;

            float d = n.Length;

            Contact contact;

            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (d != 0)
            {
                contact = new Contact(m.normal * shapeA.radius + a.position);
                contact.penetration = r - d;
                m.normal = n / d;
            }
            else
            {
                contact = new Contact(a.position);
                // Circles are concentric, take a consistent value
                contact.penetration = shapeA.radius;
                m.normal = Vec2.UnitY;
            }

            m.Update(1, contact);
        }

        public static void PolygonToPolygon(Manifold m, Body a, Body b)
        {
            Polygon shapeA = a.shape as Polygon;
            Polygon shapeB = b.shape as Polygon;

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

            if (Mathf.BiasGreaterThan(penetrationA, penetrationB))
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

            Vec2[] incidentFace;
            GetIncidentFace(out incidentFace, refPoly, incPoly, referenceIndex);

            //Set up vertices
            Vec2 v1 = refPoly.vertices[referenceIndex];
            Vec2 v2 = refPoly.vertices[(referenceIndex + 1) % refPoly.VertexCount];

            //Transform to world space
            v1 = refPoly.transform.LocalToWorldPosition(v1);
            v2 = refPoly.transform.LocalToWorldPosition(v2);

            Vec2 sidePlaneNormal = v2 - v1;
            sidePlaneNormal.Normalize();

            Vec2 refFaceNormal = new Vec2(sidePlaneNormal.y, -sidePlaneNormal.x);

            float refC = Vec2.Dot(refFaceNormal, v1);
            float negSide = -Vec2.Dot(sidePlaneNormal, v1);
            float posSide = Vec2.Dot(sidePlaneNormal, v2);

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
                float separation = Vec2.Dot(refFaceNormal, incidentFace[i]) - refC;
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

        private static float FindLeastPenetrationAxis(Polygon a, Polygon b, out int face)
        {
            float bestDistance = float.MinValue;
            face = 0;

            for (int i = 0; i < a.VertexCount; i++)
            {
                //Get world space normal of A's face
                Vec2 n = a.normals[i];
                Vec2 nW = a.transform.LocalToWorldDirection(n);

                //Retrieve normal in B's space
                n = b.transform.WorldToLocalDirection(nW);

                //Get furthest point in negative normal direction
                Vec2 s = b.GetSupportPoint(-n);

                //Get vertex on A's face, transform to B's space
                Vec2 v = a.vertices[i];
                v = a.transform.LocalToWorldPosition(v);
                v = b.transform.WorldToLocalPosition(v);

                //Find penetration distance
                float d = Vec2.Dot(s - v, n);

                //Store greatest distance
                if (d > bestDistance)
                {
                    bestDistance = d;
                    face = i;
                }
            }

            return bestDistance;
        }

        private static void GetIncidentFace(out Vec2[] face,
            Polygon refP, Polygon incP, int referenceIndex)
        {
            face = new Vec2[2];
            //Retrieve the reference normal
            Vec2 n = refP.normals[referenceIndex];

            //Transform into incident poly's space
            n = refP.transform.LocalToWorldDirection(n);    //To world space
            n = incP.transform.WorldToLocalDirection(n);    //To incident poly's space

            //Find face whose normal is most normal (perpendicular) to the normal (oh...)
            int incidentFace = -1;
            float minDot = float.MaxValue;

            for (int i = 0; i < incP.VertexCount; i++)
            {
                float dot = Vec2.Dot(n, incP.normals[i]);
                if (dot < minDot)
                {
                    minDot = dot;
                    incidentFace = i;
                }
            }

            //Get world space face
            face[0] = incP.transform.LocalToWorldPosition(incP.vertices[incidentFace]);
            face[1] = incP.transform.LocalToWorldPosition(incP.vertices[(incidentFace + 1) % incP.VertexCount]);
        }

        private static int Clip(Vec2 n, float c, ref Vec2[] face)
        {
            int clippedPoints = 0;
            Vec2[] res = { face[0], face[1] };

            //Get distance to line (ax + by = -c, n = (a, b))
            float d1 = Vec2.Dot(n, face[0]) - c;
            float d2 = Vec2.Dot(n, face[1]) - c;

            //If behind plane, clip
            //Here we aren't actually clipping, but we are just incrementing the count
            if (d1 <= 0) res[clippedPoints++] = face[0];
            if (d2 <= 0) res[clippedPoints++] = face[1];

            //Check whether one of the points is ahead and other behind
            //(-) * (+) = (-)
            if (d1 * d2 < 0)
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

        public static void CircleToPolygon(Manifold m, Body a, Body b)
        {
            Circle shapeA = a.shape as Circle;
            Polygon shapeB = b.shape as Polygon;

            m.contactCount = 0;

            // Transform circle center to Polygon model space
            Vec2 center = a.position;
            center = shapeB.transform.WorldToLocalPosition(center);

            // Find edge with minimum penetration
            // Exact concept as using support points in Polygon vs Polygon
            float separation = float.MinValue;
            int faceNormal = 0;
            for (int i = 0; i < shapeB.VertexCount; ++i)
            {
                float s = Vec2.Dot(shapeB.normals[i], center - shapeB.vertices[i]);

                if (s > shapeA.radius)
                    return;

                if (s > separation)
                {
                    separation = s;
                    faceNormal = i;
                }
            }

            // Grab face's vertices
            Vec2 v1 = shapeB.vertices[faceNormal];
            int i2 = faceNormal + 1 < shapeB.VertexCount ? faceNormal + 1 : 0;
            Vec2 v2 = shapeB.vertices[i2];

            Contact c = new Contact(Vec2.Zero);

            // Check to see if center is within polygon
            if (separation < float.Epsilon)
            {
                m.normal = -shapeB.transform.LocalToWorldDirection(shapeB.normals[faceNormal]);
                c.position = m.normal * shapeA.radius + a.position;
                c.penetration = shapeA.radius;
                m.Update(1, c);
                return;
            }

            // Determine which voronoi region of the edge center of circle lies within
            float dot1 = Vec2.Dot(center - v1, v2 - v1);
            float dot2 = Vec2.Dot(center - v2, v1 - v2);
            c.penetration = shapeA.radius - separation;

            // Closest to v1
            if (dot1 <= 0.0f)
            {
                if (Vec2.DistanceSquared(center, v1) > shapeA.radius * shapeA.radius)
                    return;

                Vec2 n = v1 - center;
                n = shapeB.transform.LocalToWorldDirection(n);
                n.Normalize();
                m.normal = n;

                v1 = shapeB.transform.LocalToWorldPosition(v1);
                c.position = v1;
                m.Update(1, c);
            }

            // Closest to v2
            else if (dot2 <= 0.0f)
            {
                if (Vec2.DistanceSquared(center, v2) > shapeA.radius * shapeA.radius)
                    return;

                Vec2 n = v2 - center;
                v2 = shapeB.transform.LocalToWorldPosition(v2);
                c.position = v2;
                m.Update(1, c);
                
                n = shapeB.transform.LocalToWorldDirection(n);
                n.Normalize();
                m.normal = n;
            }

            // Closest to face
            else
            {
                Vec2 n = shapeB.normals[faceNormal];
                if (Vec2.Dot(center - v1, n) > shapeA.radius)
                    return;

                n = shapeB.transform.LocalToWorldDirection(n);
                m.normal = -n;
                c.position = m.normal * shapeA.radius + a.position;
                m.Update(1, c);
            }
        }

        public static void PolygonToCircle(Manifold m, Body a, Body b)
        {
            //Just switching the input so that we can pass it to the function above
            CircleToPolygon(m, b, a);

            //Make sure that normal always points from A to B
            m.normal = -m.normal;
        }
    }
}
