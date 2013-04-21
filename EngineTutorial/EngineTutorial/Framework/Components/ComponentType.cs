using Microsoft.Xna.Framework;

namespace Innovation
{
    // Represents a 3D object which will be drawn before 2D objects
    public interface I3DComponent
    {
        // Position in the Cartesian system (X, Y, Z)
        Vector3 Position { get; set; }

        // Rotation represented as a Vector3.
        // Shouldn't be used for calculations.
        // Left so rotation can be easily modified by hand
        Vector3 EulerRotation { get; set; }

        // Rotation as a Matrix.
        // Gives smoother and cleaner calculations than a Vector3
        Matrix Rotation { get; set; }

        // Scale for each axis (X, Y, Z)
        Vector3 Scale { get; set; }

        // BoundingBox to use for picking and pre-collision
        BoundingBox BoundingBox { get; }
    }

    // Represents a 2D object drawn after 3D objects
    public interface I2DComponent
    {
        Rectangle Rectangle { get; set; }
    }

    public enum ComponentType
    {
        // Represents all 2D components (I2DComponent)
        Component2D,
        // Represents all 3D components (I3DComponent)
        Component3D,
        // Represents all 2D or 3D components
        Both,
        // Represents all components regardless of type
        All
    }
}