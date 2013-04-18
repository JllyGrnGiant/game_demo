using Innovation;
using Microsoft.Xna.Framework;

namespace GameEnvironment
{
    class ClearScreen : Component
    {
        // Override the component's draw method
        public override void Draw()
        {
            Engine.GraphicsDevice.Clear(Color.Red);
        }
    }
}