using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PhysicsEngine2DDemo.Demos
{
	public static class Primitives2D
	{
		// Reference of the GraphicsDevice of the Main Game class.
		private static GraphicsDevice graphicsDevice;

		// Contains the shader required to draw.
		private static BasicEffect basicEffect;

		private const int Segments = 60;
		private static Vector2[] unitCircle = new Vector2[Segments];

		/// <summary>
		/// Initializes the class variables one time only for optimization.
		/// Call in Initialize or Load method of Game class.
		/// </summary>
		/// <param name="device">GraphicsDevice on which to draw.</param>
		/// <param name="height"></param>
		public static void Initialize(GraphicsDevice device, float height)
		{
			graphicsDevice = device;

			basicEffect = new BasicEffect(graphicsDevice);
			basicEffect.VertexColorEnabled = true;
			basicEffect.Projection =
				Matrix.CreateOrthographic(graphicsDevice.Viewport.AspectRatio * height, height, -1, 1);

			for (int i = 0; i < Segments; i++)
			{
				float angle = (float)(i / (double)Segments * Math.PI * 2);
				unitCircle[i].X = (float)Math.Cos(angle);
				unitCircle[i].Y = (float)Math.Sin(angle);
			}
		}

		/// <summary>
		/// Draws a line from <paramref name="vertex1"/> to <paramref name="vertex2"/> in the specified color.
		/// </summary>
		/// <param name="vertex1">First Vertex.</param>
		/// <param name="vertex2">Second Vertex.</param>
		/// <param name="color">Color of the line.</param>
		public static void DrawLine(Vector2 vertex1, Vector2 vertex2, Color color)
		{
			VertexPositionColor[] vertices = new VertexPositionColor[2];

			vertices[0].Position = new Vector3(vertex1, 0);
			vertices[0].Color = color;

			vertices[1].Position = new Vector3(vertex2, 0);
			vertices[1].Color = color;

			basicEffect.CurrentTechnique.Passes[0].Apply();
			graphicsDevice.DrawUserPrimitives(PrimitiveType.LineStrip, vertices, 0, 1);
		}

		/// <summary>
		/// Draws a polygon with the specified vertices.
		/// </summary>
		/// <param name="position"></param>
		/// <param name="verts">Array of the vertices. The value in the last index should be equal to 
		/// the first index to close the polygon.</param>
		/// <param name="color">Color of the polygon.</param>
		public static void DrawPolygon(Vector2 position, Vector2[] verts, Color color)
		{
			VertexPositionColor[] vertices = new VertexPositionColor[verts.Length + 1];

			for (int i = 0; i < verts.Length; i++)
			{
				vertices[i].Position = new Vector3(position + verts[i], 0);
				vertices[i].Color = color;
			}

			vertices[verts.Length] = vertices[0];

			basicEffect.CurrentTechnique.Passes[0].Apply();
			graphicsDevice.DrawUserPrimitives(PrimitiveType.LineStrip, vertices, 0, vertices.Length - 1);
		}

		/// <summary>
		/// Draws a circle with the specified position and radius.
		/// </summary>
		/// <param name="position">Position of the circle (ie. the center)</param>
		/// <param name="radius">Radius of the circle in pixels.</param>
		/// <param name="color">Color of the circle.</param>
		public static void DrawCircle(Vector2 position, float radius, Color color)
		{
			DrawEllipse(position, radius, radius, color);
		}

		/// <summary>
		/// Draws an ellipse with the specified width and the height.
		/// </summary>
		/// <param name="position">Position of the ellipse (ie. the center)</param>
		/// <param name="halfWidth">Half of the width of the ellipse.</param>
		/// <param name="halfHeight">Half of the height of the ellipse.</param>
		/// <param name="color">Color of the ellipse.</param>
		public static void DrawEllipse(Vector2 position, float halfWidth, float halfHeight, Color color)
		{
			VertexPositionColor[] vertices = new VertexPositionColor[Segments + 1];

			for (int i = 0; i < Segments; i++)
			{
				vertices[i].Position = new Vector3(position.X + unitCircle[i].X * halfWidth,
					position.Y + unitCircle[i].Y * halfHeight, 0);
				vertices[i].Color = color;
			}

			vertices[Segments] = vertices[0];

			basicEffect.CurrentTechnique.Passes[0].Apply();
			graphicsDevice.DrawUserPrimitives(PrimitiveType.LineStrip, vertices, 0, vertices.Length - 1);
		}
	}
}