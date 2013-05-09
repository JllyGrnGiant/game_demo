using JigLibX.Collision;
using JigLibX.Geometry;
using JigLibX.Physics;
using Microsoft.Xna.Framework;
using JigLibX.Math;

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

        protected override void InitializeBody()
        {
            Body = new BoxBody();
            CollisionSkin = new CollisionSkin(Body);
            Body.CollisionSkin = this.CollisionSkin;


            //Vector3 com = SetMass(1.0f);
            //Body.Mass = 1000;
            //Body.MoveTo(com, Matrix.Identity);
            //CollisionSkin.ApplyLocalTransform(new Transform(-com, Matrix.Identity));
            
            Body.Mass = 1000;
            //Body.SetBodyInvInertia(0.0f, 0.0f, 0.0f);
            //Body.AllowFreezing = false;
            Body.EnableBody();

        }
    }

    public class BoxBody : Body
    {
        public BoxBody()
            : base()
        {
        }

        public Vector3 DesiredForce { get; set; }

        public override void AddExternalForces(float dt)
        {
            if (DesiredForce.Length() > 0)
                SetActive();

            ClearForces();
            AddWorldForce(DesiredForce / Mass * 100000000);
            DesiredForce = Vector3.Zero;

            AddGravityToExternalForce();
        }
    }
}