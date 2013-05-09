using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Innovation
{
    public class CrossHair : Component, I2DComponent
    {
        SpriteBatch spriteBatch;
        public Rectangle Rectangle { get; set; }
        public Rectangle hor;
        public Rectangle vert;
        Texture2D tex;

        public CrossHair() : base()
        {
            Setup();
        }

        public CrossHair(GameScreen parent) : base(parent)
        {
            Setup();
        }

        private void Setup()
        {
            spriteBatch = new SpriteBatch(Engine.GraphicsDevice);
            int length = 15;
            Rectangle = Engine.GraphicsDevice.Viewport.Bounds;
            hor = new Rectangle(Rectangle.Width / 2 - length, Rectangle.Height / 2 - 1, 2 * length, 3);
            vert = new Rectangle(Rectangle.Width / 2 - 1, Rectangle.Height / 2 - length, 3, 2 * length);
            tex = new Texture2D(Engine.GraphicsDevice, 1, 1);
            tex.SetData(new Color[] { Color.Black });
        }

        public override void Draw()
        {
            spriteBatch.Begin();
            spriteBatch.Draw(tex, hor, Color.White);
            spriteBatch.Draw(tex, vert, Color.White);
            spriteBatch.End();
        }
    }
}
