namespace PhysicsEngine2D
{
    public struct Ray2
    {
        public Vec2 origin;
        public Vec2 direction;

        public Ray2(Vec2 orig, Vec2 dir)
        {
            origin = orig;
            direction = dir;
        }
    }
}
