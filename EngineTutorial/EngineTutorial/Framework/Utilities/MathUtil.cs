using Microsoft.Xna.Framework;

namespace Innovation
{
    public static class MathUtil
    {
        // Generates a projection matrix for a draw call
        public static Matrix CreateProjectionMatrix()
        {
            return Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4,
                (float)Engine.GraphicsDevice.Viewport.Width
                / (float)Engine.GraphicsDevice.Viewport.Height,
                0.01f, 1000000);
        }

        // Creates a world matrix
        public static Matrix CreateWorldMatrix(Vector3 Translation)
        {
            return CreateWorldMatrix(Translation, Matrix.Identity);
        }

        // Creates a world matrix
        public static Matrix CreateWorldMatrix(Vector3 Translation,
            Matrix Rotation)
        {
            return CreateWorldMatrix(Translation, Rotation, Vector3.One);
        }

        // Creates a world matrix
        public static Matrix CreateWorldMatrix(Vector3 Translation, Matrix Rotation,
            Vector3 Scale)
        {
            return Matrix.CreateScale(Scale) * Rotation * Matrix.CreateTranslation(Translation);
        }

        // Converts a rotation vector into a rotation matrix
        public static Matrix Vector3ToMatrix(Vector3 Rotation)
        {
            return Matrix.CreateFromYawPitchRoll(Rotation.Y, Rotation.X, Rotation.Z);
        }

        // Converts a rotation matrix into a rotation vector
        public static Vector3 MatrixToVector3(Matrix Rotation)
        {
            Quaternion q = Quaternion.CreateFromRotationMatrix(Rotation);
            return new Vector3(q.X, q.Y, q.Z);
        }
    }
}