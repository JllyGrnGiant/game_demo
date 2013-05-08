using System.Text;
using JigLibX.Physics;
using Microsoft.Xna.Framework;
using JigLibX.Collision;
using Microsoft.Xna.Framework.Graphics;
using JigLibX.Geometry;
using JigLibX.Math;
using Microsoft.Xna.Framework.Input;
namespace Innovation
{
    public class CharacterObject : PhysicsObject
    {
        public Character CharacterBody { get; set; }

        public CharacterObject(Vector3 position) : base()
        {
            Body = new Character();
            CollisionSkin = new CollisionSkin(Body);

            Box capsule = new Box(Vector3.Zero, Matrix.Identity, new Vector3(1.0f,2.0f,1.0f));
            //Capsule capsule = new Capsule(Vector3.Zero, Matrix.CreateRotationX(MathHelper.PiOver2), 1.0f, 1.0f);
            CollisionSkin.AddPrimitive(capsule, (int)MaterialTable.MaterialID.NotBouncyNormal);
            Body.CollisionSkin = this.CollisionSkin;
            Vector3 com = SetMass(1.0f);
            Body.Mass = 1000;
            Body.MoveTo(position + com, Matrix.Identity);
            CollisionSkin.ApplyLocalTransform(new Transform(-com, Matrix.Identity));

            Body.SetBodyInvInertia(0.0f, 0.0f, 0.0f);

            CharacterBody = Body as Character;

            Body.AllowFreezing = false;
            Body.EnableBody();
        }

    }

    public class Character : Body
    {

        public Character() : base()
        {
        }

        public Vector3 DesiredVelocity { get; set; }

        private bool doJump = false;

        public void DoJump()
        {
            doJump = true;
        }


        public override void AddExternalForces(float dt)
        {
            ClearForces();

            if (doJump)
            {
                foreach (CollisionInfo info in CollisionSkin.Collisions)
                {
                    Vector3 N = info.DirToBody0;
                    if (this == info.SkinInfo.Skin1.Owner)
                        Vector3.Negate(ref N, out N);

                    if (Vector3.Dot(N, Orientation.Up) > 0.7f)
                    {
                        Vector3 vel = Velocity; vel.Y = 5.0f;
                        Velocity = vel;
                        break;
                    }
                }
            }

            Vector3 deltaVel = DesiredVelocity - Velocity;

            bool running = true;

            if (DesiredVelocity.LengthSquared() < JiggleMath.Epsilon) running = false;
            else deltaVel.Normalize();

            deltaVel.Y = 0.0f;

            // start fast, slow down slower
            if (running) deltaVel *= 10.0f;
            else deltaVel *= 2.0f;

            float forceFactor = 1000.0f;

            AddBodyForce(deltaVel * Mass * dt * forceFactor);

            doJump = false;
            AddGravityToExternalForce();
        }
    }
}
