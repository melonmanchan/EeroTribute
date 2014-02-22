using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
namespace ErppuTribute
{
    class Cube
    {
        #region Fields
        private GraphicsDevice device;
        private Texture2D texture;

        private float rotation = 0f;
        private float zrotation = 0f;
        private Random rand = new Random();

        private Vector3 location;

        private VertexBuffer cubeVertexBuffer;
        private List<VertexPositionNormalTexture> vertices = new List<VertexPositionNormalTexture>();
        #endregion

        #region Constructor
        public Cube(GraphicsDevice graphicsDevice, Vector3 playerLocation, float minDistance, Texture2D texture)
        {
            device = graphicsDevice;
            this.texture = texture;

            PositionCube(playerLocation, minDistance);

            //tee kuution sivut;
            BuildFace(new Vector3(0, 0, 0), new Vector3(0, 1, 1));
            BuildFace(new Vector3(0, 0, 1), new Vector3(1, 1, 1));
            BuildFace(new Vector3(1, 0, 1), new Vector3(1, 1, 0));
            BuildFace(new Vector3(1, 0, 0), new Vector3(0, 1, 0));

            BuildFaceHorizontal(new Vector3(0, 1, 0), new Vector3(1, 1, 1));
            BuildFaceHorizontal(new Vector3(0, 0, 1), new Vector3(1, 0, 0));

            cubeVertexBuffer = new VertexBuffer(device, VertexPositionNormalTexture.VertexDeclaration, vertices.Count, BufferUsage.WriteOnly);
            cubeVertexBuffer.SetData<VertexPositionNormalTexture>(vertices.ToArray());

        }
        #endregion

        #region Helper Methdods
        private void BuildFace(Vector3 p1, Vector3 p2)
        {
            vertices.Add(BuildVertex(p1.X, p1.Y, p1.Z, 1, 0));
            vertices.Add(BuildVertex(p1.X, p2.Y, p1.Z, 1, 1));
            vertices.Add(BuildVertex(p2.X, p2.Y, p2.Z, 0, 1));
            vertices.Add(BuildVertex(p2.X, p2.Y, p2.Z, 0, 1));
            vertices.Add(BuildVertex(p2.X, p1.Y, p2.Z, 0, 0));
            vertices.Add(BuildVertex(p1.X, p1.Y, p1.Z, 1, 0));
        }


        private void BuildFaceHorizontal(Vector3 p1, Vector3 p2)
        {
            vertices.Add(BuildVertex(p1.X, p1.Y, p1.Z, 0, 1));
            vertices.Add(BuildVertex(p2.X, p1.Y, p1.Z, 1, 1));
            vertices.Add(BuildVertex(p2.X, p2.Y, p2.Z, 1, 0));
            vertices.Add(BuildVertex(p1.X, p1.Y, p1.Z, 0, 1));
            vertices.Add(BuildVertex(p2.X, p1.Y, p2.Z, 1, 0));
            vertices.Add(BuildVertex(p1.X, p1.Y, p2.Z, 0, 0));


        }

        private VertexPositionNormalTexture BuildVertex(
            float x,
            float y,
            float z,
            float u,
            float v)
        {
            return new VertexPositionNormalTexture(new Vector3(x, y, z), new Vector3(x, y, z),new Vector2(u, v));
        }

        public void PositionCube(Vector3 PlayerLocation, float minDistance)
        {
            Vector3 newLocation;
            do
            {
                newLocation = new Vector3(rand.Next(0, Maze.mazeWidth) + 0.5f, 0.5f, rand.Next(0, Maze.mazeHeight) + 0.5f);
            }
            while (Vector3.Distance(PlayerLocation, newLocation) < minDistance);

            location = newLocation;
        }

        #endregion

        #region Draw

        public void Draw(Camera camera, BasicEffect effect)
        {
            effect.VertexColorEnabled = false;
            effect.TextureEnabled = true;
            effect.Texture = texture;
            
            Matrix center = Matrix.CreateTranslation(new Vector3(-0.5f, -0.5f, -0.5f));
            Matrix scale = Matrix.CreateScale(0.5f);
            Matrix translate = Matrix.CreateTranslation(location);

            Matrix rot = Matrix.CreateRotationY(rotation);
            Matrix zrot = Matrix.CreateRotationZ(zrotation);

            effect.World = center * rot * zrot * scale * translate;
            effect.View = camera.View;
            effect.Projection = camera.Projection;

            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                device.SetVertexBuffer(cubeVertexBuffer);
                device.DrawPrimitives(
                    PrimitiveType.TriangleList,
                    0,
                    cubeVertexBuffer.VertexCount / 3);
            }
        }
        #endregion

        #region Update
        public void Update(GameTime gameTime)
        {
            rotation = MathHelper.WrapAngle(rotation + 0.05f);
            zrotation = MathHelper.WrapAngle(zrotation + 0.025f);

        }
        #endregion
    }
}
