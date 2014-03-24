using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace ErppuTribute
{
    class Camera
    {
#region Fields
       public  AudioListener listener = new AudioListener();

        //kameran sijainti 
        private Vector3 position = Vector3.Zero;
        //kameran rotaatio y-akselin suhteen
        private float rotation;
        //piste jota kohti kamera katsoo.
        private Vector3 lookAt;
        //referenssi kameralle, osoittaa positiivista z-akselia pitkin. tarvitaan lookAtin käyttöön
        private Vector3 baseCameraReference = new Vector3(0,0,1);
        //kertoo tarvitaanko view-matriisi rakentaa uudelleen
        private bool needViewResync = true;
        private Matrix cachedViewMatrix;

#endregion

#region Properties
        //kameran projektion pystyy hakemaan kuka tahansa, mutta sitä pystyy muuttamaan vain kamera itse
        public Matrix Projection{get; private set; }

        public Vector3 Position
        {
            get
            {
                return position;
            }
            set
            {
                position = value;
                listener.Position = position;
                UpdateLookAt();
            }
        }

        public float Rotation
        {

            get
            {
                return rotation;
            }
            set
            {
                rotation = value;
                UpdateLookAt();
            }
        }
        //view-matriisi määrittelee paikan 3d-maailmassa, jossa kamera katsoo ja mihin se katsoo.
        //Matrix.CreateLookAt ottaa parametrikseen kameran paikan, pisteen johon kamera katsoo
        //sekä suunnan joka on pelimaailmassa sovittu olevan "ylös"
        //http://stackoverflow.com/questions/1309154/xna-view-matrix-seeking-explanation
        public Matrix View
        {
            get
            {
                if (needViewResync)
                    cachedViewMatrix = Matrix.CreateLookAt(
                        Position,
                        lookAt,
                        Vector3.Up);

                return cachedViewMatrix;
            }
        }


#endregion

#region Constructor
        public Camera(
            Vector3 position,
            float rotation,
            float aspectRatio,
            float nearClip,
            float farClip)
        {
            //lyhyesti sanottuna projektiomatriisi kertoo näytönohjaimelle mitä piirretään näytölle ja miten
            //kolmiulotteisesta maailmasta tehdään siis kaksiulotteinen projektio tietokoneen näytölle.
            // 1. parametri = field of view. kertoo kuinka laaja kameran "linssi" on. Tässä tapauksessa käytetään 45-asteen kameraa, joka on melko tyypillinen FPS-peleillä
            // 2. parametri. kameran aspektiratio. Tuttu telkkareista ja näytöistä (4:3, 16:10, jne...)
            //3. ja 4. parametri. Rajaavat alueen, jonka sisällä olevat asiat piirretään näytölle
            Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, aspectRatio, nearClip, farClip);
            MoveTo(position, rotation);
        }
#endregion

#region Helper Methods
        private void UpdateLookAt()
        {
            //luodaan rotatio y-akselin suhteen ja transformataan lookAtOffset
            //näin saadaan Vector3 joka kertoo mihin kohti katsotaan maailman origoon verrattuna
            Matrix rotationMatrix = Matrix.CreateRotationY(rotation);
           // Matrix rotationMatrix = Matrix.CreateRotationX(rotation);
            Vector3 lookAtOffset = Vector3.Transform(
                baseCameraReference,
                rotationMatrix);
            //Yhdistetaan kameran nykyiseen sijaintiin offset. Näin kameran sijainnista lähtee vektori joka osoittaa mihin suuntaan kamera katsoo.
            lookAt = position + lookAtOffset;
            needViewResync = true;
        }

        public void MoveTo(Vector3 position, float rotation)
        {
            //muutetaan kameran paikkaa ja katsomiskulmaa y-akselin suhteen
            this.position = position;
            this.rotation = rotation;
            UpdateLookAt();
        }

        //previewaa liikkumista. voi käyttää katsomaan käveleekö pelaaj esim. seinää päin. ottaa parametrikseen määrän jota kameraa halutaan liikuttaa
        //
        public Vector3 PreviewMove(float scale)
        {
            Matrix rotate = Matrix.CreateRotationY(rotation);
            //Matrix rotate = Matrix.CreateRotationX(rotation);
            Vector3 forward = new Vector3(0, 0, scale);
            forward = Vector3.Transform(forward, rotate);
            return (position + forward);

        }

        public Vector3 PreviewMoveSideways(float scale)
        {
            Matrix rotate = Matrix.CreateRotationX(rotation);
            Vector3 sideways = new Vector3(scale, 0, 0);
            sideways = Vector3.Transform(sideways, rotate);
            return (position + sideways);
        }

        public void MoveForward(float scale)
        {
            MoveTo(PreviewMove(scale), rotation);
        }

        public void MoveSideways(float scale)
        {
            MoveTo(PreviewMoveSideways(scale), rotation);
        }

#endregion

     }
}

