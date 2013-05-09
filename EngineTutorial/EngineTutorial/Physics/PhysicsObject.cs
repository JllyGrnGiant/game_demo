using JigLibX.Collision;
using JigLibX.Geometry;
using JigLibX.Physics;
using Microsoft.Xna.Framework;

namespace Innovation
{
    public abstract class PhysicsObject : Component, I3DComponent
    {
        float mass = 1;

        public Body Body;
        public CollisionSkin CollisionSkin;

        public float Mass
        {
            get { return mass; }
            set
            {
                mass = value;

                // Fix transforms
                Vector3 com = SetMass(value);
                if (CollisionSkin != null)
                    CollisionSkin.ApplyLocalTransform(new JigLibX.Math.Transform(-com, Matrix.Identity));
            }
        }

        public Vector3 Position
        {
            get { return Body.Position; }
            set { Body.MoveTo(value, Body.Orientation); }
        }

        public Matrix Rotation
        {
            get { return Body.Orientation; }
            set { Body.MoveTo(Body.Position, value); }
        }

        public Vector3 EulerRotation
        {
            get { return MathUtil.MatrixToVector3(Rotation); }
            set { Rotation = MathUtil.Vector3ToMatrix(value); }
        }

        // Whether or not the object is locked in place
        public bool Immovable
        {
            get { return Body.Immovable; }
            set { Body.Immovable = value; }
        }

        public BoundingBox BoundingBox
        {
            get
            {
                if (Body.CollisionSkin != null)
                    return Body.CollisionSkin.WorldBoundingBox;
                else
                    return new BoundingBox(Position - Vector3.One, Position + Vector3.One);
            }
        }

        // Dummy scale value to satisfy I3DComponent
        public Vector3 Scale
        {
            get { return Vector3.One; }
            set { }
        }

        public Vector3 Velocity
        {
            get { return Body.Velocity; }
            set { Body.Velocity = value; }
        }

        public PhysicsObject() : base() { }
        public PhysicsObject(GameScreen Parent) : base(Parent) { }

        // Sets up body and collision skin
        protected virtual void InitializeBody()
        {
            Body = new Body();
            CollisionSkin = new CollisionSkin(Body);
            Body.CollisionSkin = this.CollisionSkin;
            Body.EnableBody();
        }

        public Vector3 SetMass(float mass)
        {
            PrimitiveProperties primitiveProperties = new PrimitiveProperties(
                PrimitiveProperties.MassDistributionEnum.Solid,
                PrimitiveProperties.MassTypeEnum.Density, mass);

            float junk;
            Vector3 com;
            Matrix it;
            Matrix itCoM;

            CollisionSkin.GetMassProperties(primitiveProperties, out junk, out com, out it, out itCoM);
            Body.BodyInertia = itCoM;
            Body.Mass = junk;

            return com;
        }

        // Rotates and moves model relative to physics object to better align model with object
        public void OffsetModel(Vector3 PositionOffset, Matrix RotationOffset)
        {
            CollisionSkin.ApplyLocalTransform(new JigLibX.Math.Transform(PositionOffset, RotationOffset));
        }

        // Disables physics body and component
        public override void DisableComponent()
        {
            Body.DisableBody();
            base.DisableComponent();
        }
    }
}