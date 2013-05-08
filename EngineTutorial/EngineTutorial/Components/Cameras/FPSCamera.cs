using System;
using Microsoft.Xna.Framework;

namespace Innovation
{
    public class FPSCamera : Camera
    {
        public PhysicsObject PhysicsObject;
        Vector3 rotation;

        public FPSCamera(PhysicsObject Object, GameScreen Parent) : base(Parent)
        {
            PhysicsObject = Object;
        }
        
        public FPSCamera(PhysicsObject Object) : base()
        {
            PhysicsObject = Object;
        }

        public void Jump() {
            Vector3 v = PhysicsObject.Velocity;
            if (v.Y > -0.5 && v.Y <0)
                PhysicsObject.Velocity += new Vector3(0, 10, 0);
        }

        public override void Update()
        {
            Position = PhysicsObject.Position;
            Rotation = MathUtil.Vector3ToMatrix(rotation);
            
            Target = Vector3.Add(Position, Rotation.Forward);

            base.Update();
        }

        // Adds to rotation and translation to change camera view
        public void Rotate(Vector3 Rotation)
        {
            rotation += Rotation;
        }
    }
}
