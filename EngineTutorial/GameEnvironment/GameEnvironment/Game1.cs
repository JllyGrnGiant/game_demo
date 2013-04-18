using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Innovation;

namespace GameEnvironment
{
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        //bool fired = false;

        GaussianBlur blur;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
        }

        protected override void LoadContent()
        {
            Engine.SetupEngine(graphics);

            // Create camera
            FPSCamera camera = new FPSCamera();

            // Setup its position and target
            camera.Position = new Vector3(0, 3, 5);

            // Add services to container
            Engine.Services.AddService(typeof(Camera), camera);
            Engine.Services.AddService(typeof(Physics), new Physics());
            Engine.Services.AddService(typeof(MouseDevice), new MouseDevice());
            Engine.Services.AddService(typeof(KeyboardDevice), new KeyboardDevice());

            // Create plane
            PhysicsActor plane = new PhysicsActor(
                Engine.Content.Load<Model>("Content/ig_plane"),
                new BoxObject(new Vector3(4, 0.01f, 4), new Vector3(0, 0.1f, 0), Vector3.Zero));
            plane.PhysicsObject.Immovable = true;

            // Create Terrain
            /*Terrain terrain = new Terrain(
                Engine.Content.Load<Texture2D>("Content/heightmap"),
                Engine.Content.Load<Texture2D>("Content/grass"));
            */
            Terrain2 terrain = new Terrain2(Engine.Content.Load<Texture2D>("Content/heightmap"));
            terrain.TextureMap = "Content/grass";
            terrain.RedTexture = "Content/grass";
            terrain.GreenTexture = "Content/blue";
            terrain.BlueTexture = "Content/blue";
            terrain.AlphaTexture = "Content/red";

            Model model = Engine.Content.Load<Model>("Content/ig_box");

            for (int y = 0; y < 3; ++y)
            {
                for (int x = 0; x < 3; ++x)
                {
                    PhysicsActor act = new PhysicsActor(model, new BoxObject(
                        new Vector3(0.5f),
                        new Vector3(-0.5f + (x * 0.52f), 0.5f + (y * 0.52f), -1),
                        Vector3.Zero));
                    act.Scale = new Vector3(0.5f);
                }
            }

            blur = new GaussianBlur(Engine.GraphicsDevice.Viewport.Width, Engine.GraphicsDevice.Viewport.Height);
            blur.Visible = false; // This'll keep the engine from drawing it before we want it to
        }

        protected override void Update(GameTime gameTime)
        {
            Engine.Update(gameTime);

            KeyboardDevice keyboard = Engine.Services.GetService<KeyboardDevice>();
            MouseDevice mouse = Engine.Services.GetService<MouseDevice>();
            FPSCamera cam = (FPSCamera)Engine.Services.GetService<Camera>();

            if (keyboard.IsKeyDown(Keys.Escape))
                Exit();

            Vector3 inputModifier = new Vector3(
                (keyboard.IsKeyDown(Keys.A) ? -1 : 0) + (keyboard.IsKeyDown(Keys.D) ? 1 : 0),
                (keyboard.IsKeyDown(Keys.Q) ? -1 : 0) + (keyboard.IsKeyDown(Keys.E) ? 1 : 0),
                (keyboard.IsKeyDown(Keys.W) ? -1 : 0) + (keyboard.IsKeyDown(Keys.S) ? 1 : 0));

            cam.RotateTranslate(new Vector3(mouse.Delta.Y * -0.002f, mouse.Delta.X * -0.002f, 0), inputModifier * 0.5f);

            if (Engine.Services.GetService<MouseDevice>().WasButtonPressed(MouseButtons.Left))
            {
                PhysicsActor act = new PhysicsActor(
                    Engine.Content.Load<Model>("Content/ig_box"),
                    new BoxObject(new Vector3(0.5f), cam.Position, Vector3.Zero));

                act.Scale = new Vector3(0.5f);
                act.PhysicsObject.Mass = 1000;

                Vector3 dir = cam.Target - cam.Position;
                dir.Normalize();
                act.PhysicsObject.Velocity = dir * 10;
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            Engine.Draw(gameTime, ComponentType.All);
            base.Draw(gameTime);
            //blur.Draw();
        }
    }
}
