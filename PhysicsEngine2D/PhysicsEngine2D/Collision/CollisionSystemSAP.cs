using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace PhysicsEngine2D
{
    //Broad Phasing code using Sweep and Prune (SAP)
    //TODO: Add grid based SAP
    class CollisionSystemSAP
    {
        private class AxisPoint
        {
            public Body body;
            public bool isMin;
            public int axis;

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
                        if (axis == 0) return body.bounds.min.X;
                        else return body.bounds.min.Y;
                    }
                    else
                    {
                        if (axis == 0) return body.bounds.max.X;
                        else return body.bounds.max.Y;
                    }
                }
            }
        }


        //Overlapping bodies
        private HashSet<Manifold> overlaps = new HashSet<Manifold>();

        //Coherent method with Insertion Sort
        private void ProcessAxis(List<AxisPoint> axis)
        {
            for (int j = 1; j < axis.Count; j++)
            {
                AxisPoint keyPoint = axis[j];
                float key = keyPoint.Value;

                int i = j - 1;
                
                while (i >= 0 && axis[i].Value > key)
                {
                    AxisPoint swapper = axis[i];

                    //Make sure we are comparing minimum and maximum points
                    if (keyPoint.isMin && !swapper.isMin)
                    {
                        //Check bounds and add if possible collision
                        if (CheckBounds(swapper.body, keyPoint.body))
                        {
                            var possible = new Manifold(swapper.body, keyPoint.body);
                            if (!overlaps.Contains(possible))
                                overlaps.Add(possible);
                        }
                    }
                    
                    //If objects have separated, remove from list
                    if (!keyPoint.isMin && swapper.isMin)
                        overlaps.Remove(new Manifold(swapper.body, keyPoint.body));

                    //Sort
                    axis[i + 1] = swapper;
                    i = i - 1;
                }
                axis[i + 1] = keyPoint;
            }
        }

        //Compare bounding boxes
        private bool CheckBounds(Body a, Body b)
        {
            return a.bounds.Overlaps(b.bounds);
        }

        public CollisionSystemSAP(HashSet<Manifold> manifoldList)
        {
            overlaps = manifoldList;
        }

        //Check both X and Y axes
        public void BroadPhase(List<Body> bodies)
        {
            ProcessAxis(GenerateSweepPoints(bodies, 0));
            ProcessAxis(GenerateSweepPoints(bodies, 1));
        }

        //Generate min and max points on axis for each body
        private List<AxisPoint> GenerateSweepPoints(List<Body> bodies, int axis)
        {
            List<AxisPoint> points = new List<AxisPoint>();

            foreach(Body b in bodies)
            {
                points.Add(new AxisPoint(b, true, axis));
                points.Add(new AxisPoint(b, false, axis));
            }

            return points;
        }
    }
}
