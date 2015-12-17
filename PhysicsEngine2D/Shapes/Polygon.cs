using Microsoft.Xna.Framework;

namespace PhysicsEngine2D
{
    public class Polygon : Shape
    {
        public Vector2[] vertices;
        public Vector2[] normals;

        // Object to world Matrix
        public Matrix2 u;

        // World to object matrix (simply u.Transpose() but shorter)
        // ReSharper disable once InconsistentNaming
        public Matrix2 uT
        {
            get
            {
                return u.Transpose();
            }
        }

        public int VertexCount
        {
            get
            {
                return vertices.Length;
            }
        }

        public void SetVertices(params Vector2[] verts)
        {
            vertices = verts.Clone() as Vector2[];

            normals = new Vector2[VertexCount];

            for (int i = 0; i < VertexCount; i++)
            {
                Vector2 face = vertices[(i + 1) % VertexCount] - vertices[i];
                normals[i] = MathUtil.Cross(face, 1);
                normals[i].Normalize();
            }
        }

        public void SetBox(float halfWidth, float halfHeight)
        {
            Vector2 min = new Vector2(-halfWidth, -halfHeight);
            Vector2 topLeft = new Vector2(-halfWidth, halfHeight);
            SetVertices(min, -topLeft, -min, topLeft);
        }

        public Polygon(params Vector2[] verts)
        {
            SetVertices(verts);
            type = ShapeType.Polygon;
        }

        public Polygon(float halfWidth, float halfHeight)
        {
            SetBox(halfWidth, halfHeight);
            type = ShapeType.Polygon;
        }

        public override Shape Clone()
        {
            return new Polygon(vertices);
        }

        //Generate bounding box for this polygon
        public override Bounds GetBoundingBox()
        {
            Vector2 min = Vector2.One * float.MaxValue;
            Vector2 max = Vector2.One * float.MinValue;

            for (int i = 0; i < VertexCount; i++)
            {
                Vector2 vertex = u * vertices[i];
                if (vertex.X < min.X) min.X = vertex.X;
                if (vertex.Y < min.Y) min.Y = vertex.Y;

                if (vertex.X > max.X) max.X = vertex.X;
                if (vertex.Y > max.Y) max.Y = vertex.Y;
            }

            min += body.position;
            max += body.position;

            return new Bounds(min, max);
        }

        public override void ComputeMass(float density)
        {
            Vector2 c = Vector2.Zero; // centroid
            float area = 0.0f;
            float I = 0.0f;
            const float OneBy3 = 1.0f / 3.0f;

            for (int i1 = 0; i1 < VertexCount; ++i1)
            {
                // Triangle vertices, third vertex implied as (0, 0)
                Vector2 p1 = vertices[i1];
                int i2 = i1 + 1 < VertexCount ? i1 + 1 : 0;
                Vector2 p2 = vertices[i2];

                float d = MathUtil.Cross(p1, p2);
                float triangleArea = 0.5f * d;

                area += triangleArea;

                // Use area to weight the centroid average, not just vertex position
                c += triangleArea * OneBy3 * (p1 + p2);

                float intx2 = p1.X * p1.X + p2.X * p1.X + p2.X * p2.X;
                float inty2 = p1.Y * p1.Y + p2.Y * p1.Y + p2.Y * p2.Y;
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

        public override void SetOrientation(float orientation)
        {
            u.SetRotation(orientation);
        }

        //Get furthest vertex on polygon in a direction
        public Vector2 GetSupportPoint(Vector2 dir)
        {
            Vector2 support = Vector2.Zero;
            float maxProjection = float.MinValue;

            foreach(Vector2 vertex in vertices)
            {
                float projection = Vector2.Dot(vertex, dir);

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
