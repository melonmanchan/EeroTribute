using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace ErppuTribute
{
    class Cube
    {
        #region Fields
        //kuution oma instanssi graphicsdevicesta, hoitaa primitiivien (tässä tapauksessa kolmioiden) piirtämisen
        private GraphicsDevice device;
        //kuution tekstuuri
        private Texture2D texture;

        protected float rotation = 0f;
        protected float zrotation = 0f;
        protected float collisionRadius = 0.25f;
        protected Random rand = new Random();

        //kuution sijainti pelimaailmassa
        public Vector3 location { get; set; }

        public AudioEmitter emitter = new AudioEmitter();
        public SoundEffectInstance soundEffectInstance;

        //kuutin bufferi jotta voidaan piirtää kaikki verteksit kerralla
        private VertexBuffer cubeVertexBuffer;
        //lista kuuton verteksesitä
        private List<VertexPositionNormalTexture> vertices = new List<VertexPositionNormalTexture>();
        #endregion

        #region Properties
        public BoundingSphere Hitbox
        {
            get
            {
                return new BoundingSphere(location, collisionRadius);
            }

        }
        #endregion

        #region Constructor
        public Cube(GraphicsDevice graphicsDevice, Vector3 playerLocation, float minDistance, Texture2D texture, SoundEffect soundEffect)
        {
            device = graphicsDevice;
            this.texture = texture;

            PositionCube(playerLocation, minDistance);

            //luodaan kuution neljä "sivua" jotka ovat vertikaalisia
            BuildFace(new Vector3(0, 0, 0), new Vector3(0, 1, 1));
            BuildFace(new Vector3(0, 0, 1), new Vector3(1, 1, 1));
            BuildFace(new Vector3(1, 0, 1), new Vector3(1, 1, 0));
            BuildFace(new Vector3(1, 0, 0), new Vector3(0, 1, 0));

            //luodaan kuution kaksi sivua jotka ovat horizontaalisia, eli "katto" ja "lattia"
            BuildFaceHorizontal(new Vector3(0, 1, 0), new Vector3(1, 1, 1));
            BuildFaceHorizontal(new Vector3(0, 0, 1), new Vector3(1, 0, 0));

            //alustetaan kuution bufferi verteksien piirtämistä varten. bufferi tarvitsee graphicsedevicen johon piirretään, verteksien tyypin jota piirretään
            //, verteksien määrän joita tullaan piirtämään, sekä tiedon siitä miten bufferia tullaan käyttämään
            cubeVertexBuffer = new VertexBuffer(device, VertexPositionNormalTexture.VertexDeclaration, vertices.Count, BufferUsage.WriteOnly);
            cubeVertexBuffer.SetData<VertexPositionNormalTexture>(vertices.ToArray());


            soundEffectInstance = soundEffect.CreateInstance();
            soundEffectInstance.IsLooped = true;
        }
        #endregion

        #region Helper Methods
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

        //arvotaan kuution lokaatio aina uudelleen ja uudelleen, kunnes se on riittävän kaukana pelaajan aloituspaikasta.
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

        public virtual void Draw(Camera camera, BasicEffect effect)
        {
            //kuutio piirretään tekstuureilla -> basiceffecting vertex-väritys pois päältä
            effect.VertexColorEnabled = false;
            //efektin tekstuurit päälle, ja efektin tekstuuriksi kuution oma tekstuuri
            effect.TextureEnabled = true;
            effect.Texture = texture;
            
            //ennen kuution sijoittamista pelimaailmaan sijoitetaan se niin että sen origo sijaitsee pelimaailman
            //origon keskellä.
            Matrix center = Matrix.CreateTranslation(new Vector3(-0.5f, -0.5f, -0.5f));
            //luodaan kuution skaalalle matriisi joka pienentää sitä 50% (muuten se täyttäisi kokonaisen ruudun laatikosta)
            Matrix scale = Matrix.CreateScale(0.5f);
            //luodaan kuution sijainnista translaatiomatriisi, jotta voidaan sijoittaa se oikealle kohdalle pelimaailmassa
            Matrix translate = Matrix.CreateTranslation(location);

            Matrix rot = Matrix.CreateRotationY(rotation);
            Matrix zrot = Matrix.CreateRotationZ(zrotation);

            /*3d-grafiikoiden kanssa työskennellessä matriisien kertojärjestys on äärimmäisen tärkeää
              Tässä tapauksessa järjestys menee seuraavasti
             * 1. sijoitetaan kuutio origon keskelle
             * 2. käännetään kuutiota
             * 3. skaalataan kuutio oikean kokoiseksi
             * 4. sijoitetaan kuutio oikealle paikalle pelimaailmaan
             *  Lisälukemista http://gamedev.stackexchange.com/questions/16719/what-is-the-correct-order-to-multiply-scale-rotation-and-translation-matrices-f
             * */
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
        public virtual void Update(GameTime gameTime)
        {
            //pyöritellään kuutio, käyttäen MathHelperin WrapAnglea jotta rotaation kulma pysyy sallituilla asteilla

            rotation = MathHelper.WrapAngle(rotation + 0.05f);
            zrotation = MathHelper.WrapAngle(zrotation + 0.025f);
        }
        #endregion
    }
}
