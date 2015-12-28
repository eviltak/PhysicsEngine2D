using System;

namespace PhysicsEngine2D
{
    public class Polygon : Shape
    {
        public Vec2[] vertices;
        public Vec2[] normals;

        public int VertexCount
        {
            get
            {
                return vertices.Length;
            }
        }

        public Polygon(params Vec2[] verts)
        {
            SetVertices(verts);
            type = ShapeType.Polygon;
        }

        public Polygon(float halfWidth, float halfHeight)
        {
            SetBox(halfWidth, halfHeight);
            type = ShapeType.Polygon;
        }

        public void SetVertices(params Vec2[] verts)
        {
            vertices = verts.Clone() as Vec2[];

            normals = new Vec2[VertexCount];

            for (int i = 0; i < VertexCount; i++)
            {
                Vec2 face = vertices[(i + 1) % VertexCount] - vertices[i];
                normals[i] = Vec2.Cross(face, 1);
                normals[i].Normalize();
            }
        }

        public void SetBox(float halfWidth, float halfHeight)
        {
            Vec2 min = new Vec2(-halfWidth, -halfHeight);
            Vec2 topLeft = new Vec2(-halfWidth, halfHeight);
            SetVertices(min, -topLeft, -min, topLeft);
        }

        public override Shape Clone()
        {
            return new Polygon(vertices);
        }

        //Generate bounding box for this polygon
        public override Bounds GetBoundingBox()
        {
            Vec2 min = transform.localToWorldRotation * vertices[0];
            Vec2 max = min;

            for (int i = 1; i < VertexCount; i++)
            {
                Vec2 vertex = transform.localToWorldRotation * vertices[i];
                if (vertex.x < min.x) min.x = vertex.x;
                if (vertex.y < min.y) min.y = vertex.y;

                if (vertex.x > max.x) max.x = vertex.x;
                if (vertex.y > max.y) max.y = vertex.y;
            }

            min += body.position;
            max += body.position;

            return new Bounds(min, max);
        }

        public override void ComputeMass(float density)
        {
            Vec2 c = Vec2.Zero; // centroid
            float area = 0.0f;
            float I = 0.0f;
            const float OneBy3 = 1.0f / 3.0f;

            for (int i1 = 0; i1 < VertexCount; ++i1)
            {
                // Triangle vertices, third vertex implied as (0, 0)
                Vec2 p1 = vertices[i1];
                int i2 = i1 + 1 < VertexCount ? i1 + 1 : 0;
                Vec2 p2 = vertices[i2];

                float d = Vec2.Cross(p1, p2);
                float triangleArea = 0.5f * d;

                area += triangleArea;

                // Use area to weight the centroid average, not just vertex position
                c += triangleArea * OneBy3 * (p1 + p2);

                float intx2 = p1.x * p1.x + p2.x * p1.x + p2.x * p2.x;
                float inty2 = p1.y * p1.y + p2.y * p1.y + p2.y * p2.y;
                I += 0.25f * OneBy3 * d * (intx2 + inty2);
            }

            c *= 1.0f / area;

            // Translate vertices to centroid (make the centroid (0, 0) for the polygon in model space)
            for (int i = 0; i < VertexCount; ++i)
                vertices[i] -= c;

            float mass = density * area;
            body.inverseMass = mass != 0 ? 1.0f / mass : 0.0f;

            float inertia = I * density;
            body.inverseInertia = inertia != 0 ? 1.0f / inertia : 0.0f;
        }

        public override bool Raycast(Ray2 ray, float distance, out RaycastResult result)
        {
            result = new RaycastResult();

            float tmin = Ray2.Tmax;
            int crossings = 0;

            for (int i = 0; i < VertexCount; i++)
            {
                float t;
                int j = (i + 1) % VertexCount;

                Vec2 a = transform.LocalToWorldPosition(vertices[i]);
                Vec2 b = transform.LocalToWorldPosition(vertices[j]);

                if (ray.IntersectSegment(a, b, distance, out t))
                {
                    crossings++;
                    if (t < tmin && t <= distance)
                    {
                        tmin = t;

                        result.point = ray.origin + ray.direction * tmin;
                        result.normal = transform.LocalToWorldDirection(normals[i]);
                        result.distance = tmin;
                    }
                }
            }

            // Point in polygon test, to make sure that origin isn't inside polygon
            return crossings > 0 && crossings % 2 == 0;
        }

        //Get furthest vertex on polygon in a direction
        public Vec2 GetSupportPoint(Vec2 dir)
        {
            Vec2 support = Vec2.Zero;
            float maxProjection = float.MinValue;

            foreach(Vec2 vertex in vertices)
            {
                float projection = Vec2.Dot(vertex, dir);

                //If vertex is furthest, projection is greatest
                if(projection > maxProjection)
                {
                    maxProjection = projection;
                    support = vertex;
                }
            }

            return support;
        }
    }
}
