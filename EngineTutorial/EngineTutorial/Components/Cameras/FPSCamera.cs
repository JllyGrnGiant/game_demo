using System;
using Microsoft.Xna.Framework;

namespace Innovation
{
    public class FPSCamera : Camera
    {
        public PhysicsObject PhysicsObject;
        Vector3 rotation;
        Vector3 translation;

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
            translation = Vector3.Transform(translation, Rotation);
            translation = new Vector3(translation.X, 0, translation.Z);
            //if (translation != Vector3.Zero)
            //    PhysicsObject.Velocity += translation;
            
            Target = Vector3.Add(Position, Rotation.Forward);

            base.Update();
        }

        // Adds to rotation and translation to change camera view
        public void RotateTranslate(Vector3 Rotation, Vector3 Translation)
        {
            translation = Translation;
            rotation += Rotation;
        }
    }
}
