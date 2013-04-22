using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Innovation
{
    // Applies a post processing effect to a texture or the frame buffer
    public class PostProcessor : Component, I2DComponent
    {
        // The post postprocessor fx file
        public Effect Effect;

        protected int Width;
        protected int Height;

        Texture2D input;

        public Texture2D Input
        {
            get { return input; }
            set
            {
                input = value;

                if (Effect.Parameters["InputTexture"] != null)
                    Effect.Parameters["InputTexture"].SetValue(value);
            }
        }

        // Draw rectangle
        public Rectangle Rectangle
        {
            get { return new Rectangle(0, 0, Width, Height); }
            set
            {
                this.Width = value.Width;
                this.Height = value.Height;
            }
        }

        public PostProcessor()
            : base()
        {
            Setup(null, Engine.GraphicsDevice.Viewport.Width, Engine.GraphicsDevice.Viewport.Height);
        }

        public PostProcessor(Effect Effect, int Width, int Height)
            : base()
        {
            Setup(Effect, Width, Height);
        }

        public PostProcessor(Effect Effect, int Width, int Height, GameScreen Parent)
            : base(Parent)
        {
            Setup(Effect, Width, Height);
        }

        private void Setup(Effect Effect, int Width, int Height)
        {
            this.Effect = Effect;
            Input = new Texture2D(Engine.GraphicsDevice, 1, 1);
            Input.SetData<Color>(new Color[] { Color.White });
            this.Width = Width;
            this.Height = Height;
        }

        // Gets the current scene texture from the frame buffer
        public void GetInputFromFrameBuffer()
        {
            if (!(Input is RenderTarget2D))
                Input = GraphicsUtil.CreateRenderTarget(Width, Height);

            int[] backBuffer = new int[Width * Height];
            Engine.GraphicsDevice.GetBackBufferData(backBuffer);

            Engine.GraphicsDevice.Textures[0] = null;
            Input.SetData(backBuffer);
        }

        public override void Draw()
        {
            
            Engine.GraphicsDevice.Clear(Color.Black);

            // Begin in a mode that will overwrite everything and draw immediately
            Engine.SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque);

            foreach (EffectPass pass in Effect.CurrentTechnique.Passes)
            {
                pass.Apply();

                // Draw input texture with effect applied
                Engine.SpriteBatch.Draw(Input, Rectangle, Color.White);
            }

            Engine.SpriteBatch.End();
            
            base.Draw();
        }
    }
}