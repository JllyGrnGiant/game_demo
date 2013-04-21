namespace Innovation
{
    // Provides a way for the engine to let the game tell it what to draw
    // instead of the somewhat limited ComponentType
    public abstract class ComponentPredicate
    {
        // Decides whether the specified component is elligible
        // for the action being done
        public abstract bool IsComponentEligible(Component Component);
    }

    // Basic ComponentPredicate that uses ComponentType to determine
    // what components to use
    public class ComponentTypePredicate : ComponentPredicate
    {
        ComponentType Type;

        // Accepts a ComponentType to use to determine what
        // components will be accepted
        public ComponentTypePredicate(ComponentType Type)
        {
            this.Type = Type;
        }

        public override bool IsComponentEligible(Component Component)
        {
            if (Type == ComponentType.Both)
            {
                // If the render type is both, we will use all 2D or 3D components
                if (Component is I2DComponent || Component is I3DComponent)
                    return true;
            }
            else if (Type == ComponentType.Component2D)
            {
                if (Component is I2DComponent)
                    return true;
            }
            else if (Type == ComponentType.Component3D)
            {
                if (Component is I3DComponent)
                    return true;
            }
            else
            {
                return true;
            }

            return false;
        }
    }
}