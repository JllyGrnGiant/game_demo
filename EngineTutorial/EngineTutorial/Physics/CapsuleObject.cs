using JigLibX.Collision;
using JigLibX.Geometry;
using Microsoft.Xna.Framework;

namespace Innovation
{
    // A capsule physics simulation object
    public class CapsuleObject : PhysicsObject
    {
        float radius;
        float length;

        // Radius of capsule
        public float Radius
        {
            get { return radius; }
            set { SetupSkin(Length, value); }
        }

        // Length of capsule
        public float Length
        {
            get { return length; }
            set { SetupSkin(value, Radius); }
        }

        // Constructors

        public CapsuleObject() : base()
        {
            InitializeBody();
        }

        public CapsuleObject(float Length, float Radius) : base()
        {
            InitializeBody();
            SetupSkin(Length, Radius);
        }

        public CapsuleObject(float Length, float Radius, Vector3 Position, 
            Vector3 Rotation) : base()
        {
            InitializeBody();
            SetupSkin(Length, Radius);
            this.Position = Position;
            this.EulerRotation = Rotation;
        }

        public CapsuleObject(float Length, float Radius, Vector3 Position, 
            Vector3 Rotation, GameScreen Parent) : base(Parent)
        {
            InitializeBody();
            SetupSkin(Length, Radius);
            this.Position = Position;
            this.EulerRotation = Rotation;
        }

        // Sets up the collision skin
        void SetupSkin(float length, float radius)
        {
            // Set new size values
            this.length = length;
            this.radius = radius;

            // Update the collision skin
            CollisionSkin.RemoveAllPrimitives();
            CollisionSkin.AddPrimitive(new Capsule(
                Vector3.Transform(new Vector3(-0.5f, 0, 0), Rotation), 
                Rotation, radius, length), 
                new MaterialProperties(0.8f, 0.7f, 0.6f));
        }
    }
}