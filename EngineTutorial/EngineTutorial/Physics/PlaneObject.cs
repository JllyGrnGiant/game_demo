using JigLibX.Collision;
using JigLibX.Physics;
using Microsoft.Xna.Framework;

namespace Innovation
{
    // A flat plane physics object
    public class PlaneObject : PhysicsObject
    {
        // Constructors

        public PlaneObject() : base()
        {
            Setup();
        }

        public PlaneObject(GameScreen Parent) : base(Parent)
        {
            Setup();
        }

        // Setup everything
        void Setup()
        {
            // We can't use InitializeBody() here because we want to add a 
            // plane and not have it fall
            Body = new Body();
            CollisionSkin = new CollisionSkin(null);
            CollisionSkin.AddPrimitive(
                new JigLibX.Geometry.Plane(Vector3.Up, 0.0f), 
                new MaterialProperties(0.2f, 0.7f, 0.6f));
            PhysicsSystem.CurrentPhysicsSystem.CollisionSystem.AddCollisionSkin(CollisionSkin);
        }
    }
}
