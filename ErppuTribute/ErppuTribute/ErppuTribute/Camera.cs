using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace ErppuTribute
{
    class Camera
    {
#region Fields
        private Vector3 position = Vector3.Zero;
        private float rotation;
        private Vector3 lookAt;
        private Vector3 baseCameraReference = new Vector3(0,0,1);
        private bool needViewResync = true;
        private Matrix cachedViewMatrix;

#endregion

#region Properties
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
            Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, aspectRatio, nearClip, farClip);
            MoveTo(position, rotation);
        }
#endregion

#region Helper Methods
        private void UpdateLookAt()
        {
            Matrix rotationMatrix = Matrix.CreateRotationY(rotation);
           // Matrix rotationMatrix = Matrix.CreateRotationX(rotation);
            Vector3 lookAtOffset = Vector3.Transform(
                baseCameraReference,
                rotationMatrix);
            lookAt = position + lookAtOffset;
            needViewResync = true;
        }

        public void MoveTo(Vector3 position, float rotation)
        {
            this.position = position;
            this.rotation = rotation;
            UpdateLookAt();
        }

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

