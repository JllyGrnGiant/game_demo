using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Innovation
{
    public class MenuItem : Component, I2DComponent
    {
        SpriteBatch spriteBatch;
        SpriteFont spriteFont;
        string menuText;
        public Rectangle Rectangle { get; set; }

        public MenuItem(SpriteFont font, string text)
            : base()
        {
            Setup(font, text, new Rectangle(0, 0, 10, 10));
        }

        public MenuItem(SpriteFont font, string text, GameScreen parent)
            : base(parent)
        {
            Setup(font, text, new Rectangle(0, 0, 10, 10));
        }

        public MenuItem(SpriteFont font, string text, Rectangle rect)
            : base()
        {
            Setup(font, text, rect);
        }

        public MenuItem(SpriteFont font, string text, Rectangle rect, GameScreen parent)
            : base(parent)
        {
            Setup(font, text, rect);
        }

        private void Setup(SpriteFont font, string text, Rectangle rect)
        {
            spriteBatch = new SpriteBatch(Engine.GraphicsDevice);
            spriteFont = font;
            Rectangle = rect;
            menuText = text;
        }

        public override void Draw()
        {
            spriteBatch.Begin();
            Vector2 position = new Vector2(Rectangle.Left, Rectangle.Top);
            Texture2D tex = new Texture2D(Engine.GraphicsDevice, 1, 1);
            tex.SetData(new Color[] {Color.Black} );
            spriteBatch.Draw(tex, Rectangle, Color.Black);
            spriteBatch.DrawString(spriteFont, menuText, position, Color.White);
            spriteBatch.End();
        }

        public void Subscribe(InputEventHandler<MouseButtons, MouseState> handler)
        {
            MouseDevice mouse = Engine.Services.GetService<MouseDevice>();
            mouse.ButtonReleased += handler;
        }
    }
}
