namespace PhysicsEngine2D
{
    public struct RaycastResult
    {
        public Vec2 point;
        public Vec2 normal;

        public float distance;

        public RaycastResult(Vec2 point, Vec2 normal, float distance)
        {
            this.point = point;
            this.normal = normal;
            this.distance = distance;
        }
    }
}
