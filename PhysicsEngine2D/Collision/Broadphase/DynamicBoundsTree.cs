using System;
using System.Collections.Generic;

namespace PhysicsEngine2D
{
    internal class DynamicBoundsTree : IBroadphase
    {
        private const float Margin = 0.2f;

        private class Node
        {
            public Bounds bounds;
            public Body body;

            public Node child0;
            public Node child1;

            public Node parent;

            public Node() { }

            public Node(Body b)
            {
                body = b;
                body.data = this;
                bounds = body.bounds.Fatten(Margin);
            }

            public bool IsLeaf
            {
                get
                {
                    return child0 == null && child1 == null;
                }
            }

            public Node Sibling
            {
                get
                {
                    return this == parent.child0 ? parent.child1 : parent.child0;
                }
            }

            public Node Clone()
            {
                Node node = new Node();

                Clone(node);

                return node;
            }

            public void Clone(Node node)
            {
                node.child0 = child0;
                node.child1 = child1;
                node.parent = parent;

                node.bounds = bounds;
                node.body = body;
                if (node.body != null)
                    node.body.data = node;
            }


            public void SetBranch(Node a, Node b)
            {
                a.parent = b.parent = this;
                child0 = a;
                child1 = b;
                //body.data = null;
                body = null;
            }

            public void SetLeaf(Node node)
            {
                body = node.body;
                if (body != null) body.data = this;
                bounds = node.bounds;

                child0 = node.child0;
                child1 = node.child1;
            }

            public void UpdateBounds()
            {
                if (IsLeaf)
                    bounds = body.bounds.Fatten(Margin);
                else
                    bounds = child0.bounds.Union(child1.bounds);
            }
        }

        private Node root;

        public int count;
        private HashSet<Manifold> pairs = new HashSet<Manifold>();

        void IBroadphase.Add(Body body)
        {
            if (root != null)
            {
                // not first node, insert node to tree
                Node node = new Node(body);

                InsertNode(node);
            }
            else
            {
                // first node, make root
                root = new Node(body);
            }
        }

        private void InsertNode(Node node)
        {
            if (root == null)
            {
                root = node;
                count++;
                return;
            }

            Node current = root;

            while (!current.IsLeaf)
            {
                float volumeDiff0 =
                    current.child0.bounds.Union(node.bounds).Volume - current.child0.bounds.Volume;

                float volumeDiff1 =
                    current.child1.bounds.Union(node.bounds).Volume - current.child1.bounds.Volume;

                if (volumeDiff0 < volumeDiff1)
                    current = current.child0;
                else
                    current = current.child1;

            }

            // Found sibling, now make a new branch
            current.SetBranch(node, current.Clone());
            count++;

            // Walk back up the tree fixing AABBs
            UpdateTreeNodes(node.parent);
        }

        private static void UpdateTreeNodes(Node startNode)
        {
            while (startNode != null)
            {
                startNode.UpdateBounds();

                startNode = startNode.parent;
            }
        }

        void IBroadphase.Remove(Body body)
        {
            if (root == null) return;
            if (root.IsLeaf)
            {
                if (root.body == body)
                    root = null;
            }
            else
            {
                RemoveNode((Node)body.data);
            }
        }

        bool IBroadphase.Raycast(Ray2 ray, float distance, out RaycastResult result)
        {
            result = new RaycastResult();
            float tmin = Ray2.Tmax;

            Queue<Node> q = new Queue<Node>();

            if (root != null)
                q.Enqueue(root);

            while (q.Count > 0)
            {
                Node node = q.Dequeue();

                if (node.bounds.Raycast(ray, distance))
                {
                    if (node.IsLeaf)
                    {
                        RaycastResult tempResult;
                        if (node.body.shape.Raycast(ray, distance, out tempResult))
                        {
                            if (tempResult.distance < tmin)
                            {
                                result = tempResult;
                                tmin = tempResult.distance;
                            }
                        }
                    }
                    else
                    {
                        q.Enqueue(node.child0);
                        q.Enqueue(node.child1);
                    }
                }
            }

            return tmin < Ray2.Tmax;
        }

        void IBroadphase.ComputePairs(List<Body> bodies, HashSet<Manifold> manifolds)
        {
            this.manifolds = manifolds;
            pairs.Clear();

            for (int i = 0; i < bodies.Count; i++)
            {
                Body body = bodies[i];
                Query(body.bounds, body);
            }

            manifolds.RemoveWhere(m => !pairs.Contains(m));
        }

        void IBroadphase.Clear()
        {
            root = null;
        }

        private HashSet<Manifold> manifolds;

        private bool QueryCallback(Body body, Body queryBody)
        {
            if (body == queryBody)
                return true;

            Manifold pair = new Manifold(body, queryBody);
            pairs.Add(pair);
            manifolds.Add(pair);

            return true;
        }

        private void Query(Bounds bounds, Body queryBody)
        {
            Queue<Node> q = new Queue<Node>();
            if (root != null)
                q.Enqueue(root);

            while (q.Count > 0)
            {
                Node node = q.Dequeue();

                if (node.bounds.Overlaps(bounds))
                {
                    if (node.IsLeaf)
                    {
                        if (!QueryCallback(node.body, queryBody))
                            return;
                    }
                    else
                    {
                        q.Enqueue(node.child0);
                        q.Enqueue(node.child1);
                    }
                }
            }
        }

        private void RemoveNode(Node node)
        {
            count--;

            if (root == node)
            {
                root = null;
                return;
            }

            Node parent = node.parent;
            Node grandparent = parent.parent;
            Node sibling = node.Sibling;

            if (grandparent != null)
            {
                if (grandparent.child0 == parent)
                    grandparent.child0 = sibling;
                else
                    grandparent.child1 = sibling;

                sibling.parent = grandparent;

                // Update nodes up from new parent
                UpdateTreeNodes(grandparent);
            }
            else
            {
                root = sibling;
                sibling.parent = null;

            }
        }

        void IBroadphase.Update(List<Body> bodies)
        {
            if (root == null)
                return;

            if (root.IsLeaf)
            {
                root.UpdateBounds();
            }
            else
            {
                List<Node> invalidNodes = new List<Node>();

                for (int i = 0; i < bodies.Count; i++)
                {
                    Body body = bodies[i];
                    Node node = (Node)body.data;
                    if (!node.bounds.Contains(node.body.bounds))
                        invalidNodes.Add(node);
                }

                for (int i = 0; i < invalidNodes.Count; i++)
                {
                    Node node = invalidNodes[i];
                    RemoveNode(node);

                    node.UpdateBounds();

                    InsertNode(node);
                }
            }
        }

        void IBroadphase.DebugDraw(IDebugDrawer debugDrawer)
        {
            Queue<Tuple<Node, int>> q = new Queue<Tuple<Node, int>>();

            if (root != null)
                q.Enqueue(new Tuple<Node, int>(root, 1));

            while (q.Count > 0)
            {
                var param = q.Dequeue();
                Node node = param.Item1;
                int depth = param.Item2;

                Vec2[] verts = new Vec2[] {
                        node.bounds.min,
                        new Vec2(node.bounds.max.x, node.bounds.min.y),
                        node.bounds.max,
                        new Vec2(node.bounds.min.x, node.bounds.max.y)
                };

                debugDrawer.Draw(verts, (object)depth);

                if (!node.IsLeaf)
                {
                    q.Enqueue(new Tuple<Node, int>(node.child0, depth + 1));
                    q.Enqueue(new Tuple<Node, int>(node.child1, depth + 1));
                }
            }
        }
    }
}
