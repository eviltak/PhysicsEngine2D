using System.Collections.Generic;

namespace PhysicsEngine2D
{
    //Broad Phasing code using Sweep and Prune (SAP)
    //TODO: Add grid based SAP
    public class SweepAndPruneBroadphase : Broadphase
    {
        private class AxisPoint
        {
            public Body body;
            public bool isMin;
            private int axis;

            public AxisPoint(Body body, bool isMin, int axis)
            {
                this.body = body;
                this.isMin = isMin;
                this.axis = axis;
            }

            public float Value
            {
                get
                {
                    if (isMin)
                    {
                        return axis == 0 ? body.bounds.min.x : body.bounds.min.y;
                    }

                    return axis == 0 ? body.bounds.max.x : body.bounds.max.y;
                }
            }
        }


        // Overlapping bodies
        private HashSet<Manifold> overlaps;

        // Coherent method with Insertion Sort
        private void ProcessAxis(List<AxisPoint> axis)
        {
            for (int j = 1; j < axis.Count; j++)
            {
                AxisPoint currentPoint = axis[j];
                float current = currentPoint.Value;

                int i = j - 1;

                while (i >= 0 && axis[i].Value > current)
                {
                    AxisPoint greater = axis[i];

                    // Make sure we are comparing minimum and maximum points
                    if (currentPoint.isMin && !greater.isMin)
                    {
                        // Check bounds and add if possible collision
                        if (CheckBounds(greater.body, currentPoint.body))
                        {
                            Manifold possible = new Manifold(greater.body, currentPoint.body);

                            // If we already have reported these two objects colliding,
                            // keep them in memory so that impulses can be balanced
                            overlaps.Add(possible);
                        }
                    }

                    //If objects have separated, remove from list
                    if (!currentPoint.isMin && greater.isMin)
                        overlaps.Remove(new Manifold(greater.body, currentPoint.body));

                    //Sort
                    axis[i + 1] = greater;
                    --i;
                }
                axis[i + 1] = currentPoint;
            }
        }

        //Compare bounding boxes
        private bool CheckBounds(Body a, Body b)
        {
            return a.bounds.Overlaps(b.bounds);
        }

        public override void Update(List<Body> bodies)
        {
            
        }

        public override bool Raycast(Ray2 ray, float distance, out RaycastResult result)
        {
            result = new RaycastResult();
            return false;
        }

        //Check both X and Y axes
        internal override void ComputePairs(List<Body> bodies, HashSet<Manifold> manifolds)
        {
            overlaps = manifolds;
            ProcessAxis(GenerateSweepPoints(bodies, 0));
            ProcessAxis(GenerateSweepPoints(bodies, 1));
        }

        public override void Clear()
        {
            
        }

        //Generate min and max points on axis for each body
        private List<AxisPoint> GenerateSweepPoints(List<Body> bodies, int axis)
        {
            List<AxisPoint> points = new List<AxisPoint>();

            foreach (Body b in bodies)
            {
                points.Add(new AxisPoint(b, true, axis));
                points.Add(new AxisPoint(b, false, axis));
            }

            return points;
        }

        public override void Add(Body body)
        {
        }

        public override void Remove(Body body)
        {
        }
    }
}
