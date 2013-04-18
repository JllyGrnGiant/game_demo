using JigLibX.Collision;
using JigLibX.Geometry;
using JigLibX.Physics;
using Microsoft.Xna.Framework;

namespace Innovation
{
    public class BoxObject : PhysicsObject
    {
        Vector3 sideLengths;

        public Vector3 SideLengths
        {
            get { return sideLengths; }
            set
            {
                sideLengths = value;

                CollisionSkin.RemoveAllPrimitives();
                CollisionSkin.AddPrimitive(new Box(-0.5f * value, Body.Orientation, value),
                    new MaterialProperties(0.8f, 0.8f, 0.7f));
                this.Mass = this.Mass;
            }
        }

        public BoxObject()
            : base()
        {
            InitializeBody();
            SideLengths = Vector3.One;
        }

        public BoxObject(Vector3 SideLengths)
            : base()
        {
            SetupSkin(SideLengths, Vector3.Zero, Vector3.Zero);
        }

        public BoxObject(Vector3 SideLengths, Vector3 Position, Vector3 Rotation)
            : base()
        {
            SetupSkin(SideLengths, Position, Rotation);
        }

        public BoxObject(Vector3 SideLengths, Vector3 Position, Vector3 Rotation, GameScreen Parent)
            : base(Parent)
        {
            SetupSkin(SideLengths, Position, Rotation);
        }

        void SetupSkin(Vector3 SideLengths, Vector3 Position, Vector3 Rotation)
        {
            InitializeBody();

            this.SideLengths = SideLengths;
            this.Position = Position;
            this.EulerRotation = Rotation;
        }
    }
}