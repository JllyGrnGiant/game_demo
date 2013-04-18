using JigLibX.Collision;
using JigLibX.Physics;

namespace Innovation
{
    public class Physics : Component
    {
        // The physics simulation
        public PhysicsSystem PhysicsSystem = new PhysicsSystem();

        public bool UpdatePhysics = true;

        public Physics()
        {
            this.PhysicsSystem.EnableFreezing = true;
            this.PhysicsSystem.SolverType = PhysicsSystem.Solver.Normal;
            this.PhysicsSystem.CollisionSystem = new CollisionSystemSAP();
        }

        public override void Update()
        {
            if (UpdatePhysics)
                PhysicsSystem.CurrentPhysicsSystem.Integrate((float)Engine.GameTime.ElapsedGameTime.TotalSeconds);
        }
    }
}