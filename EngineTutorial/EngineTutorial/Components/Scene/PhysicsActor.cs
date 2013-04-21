using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Innovation
{
    public class PhysicsActor : Actor
    {
        public PhysicsObject PhysicsObject;

        public override Vector3 Position
        {
            get
            {
                if (PhysicsObject != null)
                    return PhysicsObject.Position;
                else
                    return Vector3.Zero;
            }
            set
            {
                if (PhysicsObject != null)
                    PhysicsObject.Position = value;
            }
        }

        public override Matrix Rotation
        {
            get
            {
                if (PhysicsObject != null)
                    return PhysicsObject.Rotation;
                else
                    return Matrix.Identity;
            }
            set
            {
                if (PhysicsObject != null)
                    PhysicsObject.Rotation = value;
            }
        }

        public override BoundingBox BoundingBox
        {
            get
            {
                if (PhysicsObject != null)
                    return PhysicsObject.BoundingBox;
                else
                    return new BoundingBox(-Vector3.One, Vector3.One);
            }
        }

        public PhysicsActor(Model Model, PhysicsObject PhysicsObject)
            : base(Model, PhysicsObject.Position)
        {
            this.PhysicsObject = PhysicsObject;
        }

        public PhysicsActor(Model Model, PhysicsObject PhysicsObject, GameScreen Parent)
            : base(Model, PhysicsObject.Position, Parent)
        {
            this.PhysicsObject = PhysicsObject;
        }

        public override void DisableComponent()
        {
            this.PhysicsObject.DisableComponent();
            base.DisableComponent();
        }
    }
}