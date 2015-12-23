using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Microsoft.Xna.Framework;
using XNAPrimitives2D;

namespace PhysicsEngine2D
{
    public class DynamicAABBTree : Broadphase
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

            public void DebugDraw(int height)
            {
                Vector2[] vertices = { bounds.min, new Vector2(bounds.min.X, bounds.max.Y),
                    bounds.max, new Vector2(bounds.max.X, bounds.min.Y) };

                Vector2 c = vertices.Aggregate(Vector2.Zero, (current, v) => current + v);
                c /= 4;

                for (int i = 0; i < vertices.Length; i++)
                {
                    vertices[i] -= c;
                }

                Primitives2D.DrawPolygon(c, vertices, MathUtil.Random(height));

                if (!IsLeaf)
                {
                    child0.DebugDraw(height + 1);
                    child1.DebugDraw(height + 1);
                }
            }
        }

        private Node root;

        public int count;
        private HashSet<Manifold> pairs = new HashSet<Manifold>();

        public override void Add(Body body)
        {
            if (root != null)
            {
                // not first node, insert node to tree
                Node node = new Node(body);
                //InsertNodeRecursive(ref root, node);
                InsertNodeIterative(node);
            }
            else
            {
                // first node, make root
                root = new Node(body);
            }
        }

        private void InsertNodeIterative(Node node)
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

        public void DebugDraw()
        {
            root?.DebugDraw(0);
        }

        public override void Remove(Body body)
        {
            if (root == null) return;
            if (root.IsLeaf)
            {
                if (root.body == body)
                    root = null;
            }
            else
            {
                RemoveNodeIterative((Node)body.data);
            }
        }

        internal override void ComputePairs(List<Body> bodies, HashSet<Manifold> manifolds)
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

        private void RemoveNodeIterative(Node node)
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

        public override void Update(List<Body> bodies)
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
                //IsNodeInvalid(root, invalidNodes);
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
                    RemoveNodeIterative(node);

                    node.UpdateBounds();
                    //InsertNodeRecursive(ref root, node);
                    InsertNodeIterative(node);
                }
            }
        }

        private void IsNodeInvalid(Node node, List<Node> invalidNodes)
        {

            if (node.IsLeaf)
            {
                // check if fat AABB doesn't 
                // contain the collider's AABB anymore
                if (!node.bounds.Contains(node.body.bounds))
                    invalidNodes.Add(node);
            }
            else
            {
                IsNodeInvalid(node.child0, invalidNodes);
                IsNodeInvalid(node.child1, invalidNodes);
            }
        }
    }
}
