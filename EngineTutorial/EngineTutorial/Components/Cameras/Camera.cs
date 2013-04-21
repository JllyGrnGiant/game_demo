using Microsoft.Xna.Framework;
using System;

namespace Innovation
{
    public class Camera : Component, I3DComponent
    {
        Vector3 position = Vector3.Zero;
        Matrix rotationMatrix = Matrix.Identity;
        Vector3 target = new Vector3(0, 0, -1);
        Vector3 up = Vector3.Up;
        Matrix view;
        Matrix projection;

        // The point the camera is looking at
        public virtual Vector3 Target
        {
            get { return target; }
            set { target = value; }
        }

        // The View and Projection matrices commonly used for rendering
        public virtual Matrix View
        {
            get { return view; }
            set { view = value; }
        }

        public virtual Matrix Projection
        {
            get { return projection; }
            set { projection = value; }
        }

        public virtual Vector3 Up
        {
            get { return up; }
            set { up = value; }
        }

        public virtual Vector3 Position
        {
            get { return position; }
            set { position = value; }
        }

        public virtual Vector3 Scale
        {
            get { return Vector3.One; }
            set {}
        }

        public Vector3 EulerRotation
        {
            get { return MathUtil.MatrixToVector3(Rotation); }
            set { Rotation = MathUtil.Vector3ToMatrix(value); }
        }

        public virtual Matrix Rotation
        {
            get { return rotationMatrix; }
            set { rotationMatrix = value; }
        }

        public virtual BoundingBox BoundingBox
        {
            get { return new BoundingBox(Position - Vector3.One, Position + Vector3.One); }
        }

        // Constructors
        public Camera(GameScreen Parent) : base(Parent) { }
        public Camera() : base() { }

        // Update camera
        public override void Update()
        {
            // Calculate the direction from the position to the target and normalize
            Vector3 newForward = Target - Position;
            newForward.Normalize();

            // Set rotation matrix's forward to this vector
            Matrix rotationMatrixCopy = this.Rotation;
            rotationMatrixCopy.Forward = newForward;

            // Save a copy of "Up" (0, 1, 0)
            Vector3 referenceVector = Vector3.Up;

            // On the slim chance the camera is pointed perfectly parallel with the
            // Y axis, we cannot use cross product, so we change the reference vector
            // to the forward axis (Z)
            if (rotationMatrixCopy.Forward.Y == referenceVector.Y
                || rotationMatrixCopy.Forward.Y == -referenceVector.Y)
                referenceVector = Vector3.Backward;

            // Calculate the other parts of the rotation matrix
            rotationMatrixCopy.Right = Vector3.Cross(this.Rotation.Forward,
                referenceVector);
            rotationMatrixCopy.Up = Vector3.Cross(this.Rotation.Right,
                this.Rotation.Forward);

            this.Rotation = rotationMatrixCopy;

            // Use rotation matrix to find new up
            Up = Rotation.Up;

            // Recalculate View and Projection using new Position, Target, and Up
            View = Matrix.CreateLookAt(Position, Target, Up);
            Projection = MathUtil.CreateProjectionMatrix();
        }
    }
}