using System;
using System.Collections.Generic;
using System.Linq;
using JigLibX.Physics;
using JigLibX.Collision;
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
        Model boxModel;
        SoundEffect boxSpawn;
        SoundEffect jump;
        SoundEffect gunShot;

        //bool fired = false;

        //Action delegate1 = delegate() { (object o, InputDeviceEventArgs<MouseButtons, MouseState> args) => ResumeHandler(o, args, resume); };

        //GaussianBlur blur;
        //GameScreen pause;
        Vector2 paused_mouse_position;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
        }

        protected override void LoadContent()
        {
            Engine.SetupEngine(graphics);

            boxModel = Engine.Content.Load<Model>("Content/ig_box");
            boxSpawn = Engine.Content.Load<SoundEffect>("Content/box_spawn");
            jump = Engine.Content.Load<SoundEffect>("Content/jump");
            gunShot = Engine.Content.Load<SoundEffect>("Content/gun_shot");
            // Add services to container
            Engine.Services.AddService(typeof(Physics), new Physics());
            Engine.Services.AddService(typeof(MouseDevice), new MouseDevice());
            Engine.Services.AddService(typeof(KeyboardDevice), new KeyboardDevice());

            CharacterObject character = new CharacterObject(new Vector3(0, 3, 5));
            
            //Create camera
            //CharacterObject camBox = new SphereObject(0.25f, new Vector3(0, 3, 5), Vector3.Zero);
            //CapsuleObject camBox = new CapsuleObject(0.25f, .25f);
            //camBox.Mass = 1000;
            FPSCamera camera = new FPSCamera(character);
            //camBox.Position = new Vector3(0, 3, 5);
            Engine.Services.AddService(typeof(Camera), camera);

            // Setup its position and target
            // camera.Position = new Vector3(0, 3, 5);

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

            for (int y = 0; y < 3; ++y)
            {
                for (int x = 0; x < 3; ++x)
                {
                    PhysicsActor act = new PhysicsActor(boxModel, new BoxObject(
                        new Vector3(0.5f),
                        new Vector3(-0.5f + (x * 0.52f), 0.5f + (y * 0.52f), -1),
                        Vector3.Zero));
                    act.Scale = new Vector3(0.5f);
                    act.PhysicsObject.Mass = 1000;
                }
            }

            //blur = new GaussianBlur(Engine.GraphicsDevice.Viewport.Width, Engine.GraphicsDevice.Viewport.Height);
            //blur.Visible = false; // This'll keep the engine from drawing it before we want it to

        }

        protected override void Update(GameTime gameTime)
        {
            Engine.Update(gameTime);

            KeyboardDevice keyboard = Engine.Services.GetService<KeyboardDevice>();
            MouseDevice mouse = Engine.Services.GetService<MouseDevice>();
            FPSCamera cam = (FPSCamera)Engine.Services.GetService<Camera>();
            //cam.Position = new Vector3(0, 3, 5);
            //if (keyboard.IsKeyDown(Keys.Escape))
            //    Exit();

            if (keyboard.WasKeyPressed(Keys.Escape))
            {

                if (Engine.GameScreens.Contains("Pause"))
                {
                    Resume(keyboard, mouse);
                }
                else
                {
                    pause(keyboard, mouse);
                }
            }

            if (!Engine.GameScreens.Contains("Pause"))
            {
                Vector3 inputModifier = new Vector3(
                    (keyboard.IsKeyDown(Keys.A) ? -1 : 0) + (keyboard.IsKeyDown(Keys.D) ? 1 : 0),
                     0,//(keyboard.IsKeyDown(Keys.Q) ? -1 : 0) + (keyboard.IsKeyDown(Keys.E) ? 1 : 0),
                    (keyboard.IsKeyDown(Keys.W) ? -1 : 0) + (keyboard.IsKeyDown(Keys.S) ? 1 : 0));

                cam.Rotate(new Vector3(mouse.Delta.Y * -0.002f, mouse.Delta.X * -0.002f, 0));

                //Matrix camRotation = Matrix.CreateRotationX(MathUtil.MatrixToVector3(cam.Rotation).X) *
                //    Matrix.CreateRotationY(MathUtil.MatrixToVector3(cam.Rotation).Y);
                inputModifier = Vector3.Transform(inputModifier, cam.Rotation);
                //JigLibX.Math.JiggleMath.NormalizeSafe(ref inputModifier);
                //inputModifier *= 10f;

                ((CharacterObject)cam.PhysicsObject).CharacterBody.DesiredVelocity = inputModifier*5;

                if (keyboard.WasKeyPressed(Keys.Space))
                {
                    Character body = ((CharacterObject)cam.PhysicsObject).CharacterBody;
                    body.DoJump();
                    //((CharacterObject)cam.PhysicsObject).CharacterBody.DoJump();
                    if(body.CollisionSkin.Collisions.Count > 0)
                        jump.Play();
                }

                if (Engine.Services.GetService<MouseDevice>().WasButtonPressed(MouseButtons.Right))
                {
                    Vector3 dir = cam.Target - cam.Position;
                    dir.Normalize();
                    PhysicsActor act = new PhysicsActor(
                        boxModel,
                        new BoxObject(new Vector3(0.5f), cam.Position + 2 * dir, Vector3.Zero));
                    //new SphereObject(0.25f, cam.Position+3*dir, Vector3.Zero));
                    act.Scale = new Vector3(0.5f);
                    act.PhysicsObject.Mass = 1000;
                    act.PhysicsObject.Velocity = dir * 10;

                    boxSpawn.Play();
                }

                if (Engine.Services.GetService<MouseDevice>().WasButtonPressed(MouseButtons.Left))
                {
                    gunShot.Play();
                    Vector3 dir = cam.Target - cam.Position;
                    dir.Normalize();
                    //PhysicsActor act = new PhysicsActor(
                    //    boxModel,
                    //    new BoxObject(new Vector3(0.5f), cam.Position + 2*dir, Vector3.Zero));
                    //    //new SphereObject(0.25f, cam.Position+3*dir, Vector3.Zero));
                    //act.Scale = new Vector3(0.5f);
                    //act.PhysicsObject.Mass = 1000;

                   
                    //act.PhysicsObject.Velocity = dir * 10;
                    float dist;
                    CollisionSkin skin;
                    Vector3 pos, normal;

                    ImmovableSkinPredicate pred = new ImmovableSkinPredicate();
                    JigLibX.Geometry.Segment seg = new JigLibX.Geometry.Segment(cam.Position, dir * 1000000000.0f);

                    Engine.Services.GetService<Physics>().PhysicsSystem.CollisionSystem.SegmentIntersect(out dist, out skin, out pos, out normal, seg, pred);

                    if (skin != null)
                    {
                        skin.Owner.Velocity = dir*10;
                    }
                }
            }

            base.Update(gameTime);
        }

        private void pause(KeyboardDevice keyboard, MouseDevice mouse)
        {
            paused_mouse_position = mouse.Position;
            GameScreen pause = new GameScreen("Pause");
            mouse.ResetMouseAfterUpdate = false;
            this.IsMouseVisible = true;
            pause.Components.Add(keyboard);
            pause.Components.Add(mouse);
            Engine.blur.Visible = true;
            pause.BlocksUpdate = true;
            //MenuItem paused = new MenuItem(Engine.Content.Load<SpriteFont>("Content/MenuFont"), "Paused", new Rectangle(350,100,100,30), pause);

            MenuItem resume = new MenuItem(Engine.Content.Load<SpriteFont>("Content/MenuFont"), "Resume", new Rectangle(350, 150, 70, 30), pause);
            resume.Subscribe((object o, InputDeviceEventArgs<MouseButtons, MouseState> args) => ResumeHandler(o, args, resume));
            MenuItem exit = new MenuItem(Engine.Content.Load<SpriteFont>("Content/MenuFont"), "Exit", new Rectangle(350, 200, 50, 30), pause);
            exit.Subscribe((object o, InputDeviceEventArgs<MouseButtons, MouseState> args) => Terminate(o, args, exit));
        }

        private void Resume(KeyboardDevice keyboard, MouseDevice mouse)
        {
            //mouse.ButtonReleased = null;
            mouse.UnsubscribeAll();
            mouse.Position = paused_mouse_position;
            Engine.GameScreens["Pause"].Disable();
            Engine.BackgroundScreen.Components.Add(keyboard);
            Engine.BackgroundScreen.Components.Add(mouse);
            Engine.blur.Visible = false;
            mouse.ResetMouseAfterUpdate = true;
            this.IsMouseVisible = false;
        }

        protected override void Draw(GameTime gameTime)
        {
            Engine.Draw(gameTime, ComponentType.All);
            base.Draw(gameTime);
            //blur.Draw();
        }



        private void Terminate(object sender, InputDeviceEventArgs<MouseButtons, MouseState> args, MenuItem subscriber)
        {
            if (subscriber.Rectangle.Contains(new Point(args.State.X, args.State.Y)))
                Exit();
        }

        private void ResumeHandler(object sender, InputDeviceEventArgs<MouseButtons, MouseState> args, MenuItem subscriber)
        {
            if (subscriber.Rectangle.Contains(new Point(args.State.X, args.State.Y)))
                Resume(Engine.Services.GetService<KeyboardDevice>(), Engine.Services.GetService<MouseDevice>());
        }
    }

    public class ImmovableSkinPredicate : CollisionSkinPredicate1
    {
        CharacterObject character = (CharacterObject)((FPSCamera)Engine.Services.GetService<Camera>()).PhysicsObject;

        public override bool ConsiderSkin(CollisionSkin skin0)
        {
            if (skin0.Owner != null && skin0.Owner != character.CharacterBody  && !skin0.Owner.Immovable)
                return true;

            else
                return false;
        }
    }
}
