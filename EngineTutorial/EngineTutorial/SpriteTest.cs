using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Innovation
{
    public class SpriteTest : Component, I2DComponent
    {
        Texture2D spriteTexture;
        SpriteBatch spriteBatch;
        public Rectangle Rectangle { get; set; }

        public SpriteTest(Texture2D tex, GameScreen parent)
            : base(parent)
        {
            LoadSpriteFromTexture(tex);
        }

        public void LoadSpriteFromTexture(Texture2D tex)
        {
            spriteBatch = new SpriteBatch(Engine.GraphicsDevice);
            spriteTexture = tex;
            Rectangle = new Rectangle(0, 0, tex.Width, tex.Height);
        }

        public override void Draw()
        {
            spriteBatch.Begin();
            Vector2 position = new Vector2(Rectangle.Left, Rectangle.Top);
            spriteBatch.Draw(spriteTexture, position, Color.White);
            spriteBatch.End();
        }
    }
}
