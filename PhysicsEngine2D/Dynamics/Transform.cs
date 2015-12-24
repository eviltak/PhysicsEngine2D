namespace PhysicsEngine2D
{
    public class Transform
    {
        public Vec2 position;
        public float rotation;

        public Matrix2 localToWorldRotation;

        // ReSharper disable once InconsistentNaming
        public Matrix2 worldToLocalRotation
        {
            get
            {
                return localToWorldRotation.Transpose();
            }
        }

        public Transform(Vec2 position, float rotation)
        {
            this.position = position;
            this.rotation = rotation;
            UpdateMatrix();
        }

        public Vec2 LocalToWorldPosition(Vec2 localPosition)
        {
            return localToWorldRotation * localPosition + position;
        }

        public Vec2 LocalToWorldDirection(Vec2 localDirection)
        {
            return localToWorldRotation * localDirection;
        }

        public Vec2 WorldToLocalPosition(Vec2 worldPosition)
        {
            return worldToLocalRotation * (worldPosition - position);
        }

        public Vec2 WorldToLocalDirection(Vec2 worldDirection)
        {
            return worldToLocalRotation * worldDirection;
        }

        public void SetRotation(float radians)
        {
            rotation = radians;
            localToWorldRotation.SetRotation(radians);
        }

        public void UpdateMatrix()
        {
            localToWorldRotation.SetRotation(rotation);
        }
    }
}
