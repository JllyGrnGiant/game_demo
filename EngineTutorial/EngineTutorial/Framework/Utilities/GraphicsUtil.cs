using Microsoft.Xna.Framework.Graphics;

namespace Innovation
{
    public static class GraphicsUtil
    {
        // Creates a RenderTarget2D with the specified parameters
        public static RenderTarget2D CreateRenderTarget()
        {
            return CreateRenderTarget(Engine.GraphicsDevice.Viewport.Width,
                Engine.GraphicsDevice.Viewport.Height);
        }

        // Creates a RenderTarget2D w/ specified parameters
        public static RenderTarget2D CreateRenderTarget(int Width, int Height)
        {
            return new RenderTarget2D(Engine.GraphicsDevice, Width, Height);
        }
    }
}