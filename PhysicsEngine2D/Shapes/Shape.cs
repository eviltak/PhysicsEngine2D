namespace PhysicsEngine2D
{
    public abstract class Shape
    {
        public enum ShapeType
        {
            Polygon, Circle,
            Count //Count is just so that the number of shapes can be referenced easily
        };

        public Body body;
        public ShapeType type;
        
        public abstract Shape Clone();
        public abstract Bounds GetBoundingBox();
        public abstract void ComputeMass(float density);

        public virtual void SetOrientation(float orientation) { }
    }
}
