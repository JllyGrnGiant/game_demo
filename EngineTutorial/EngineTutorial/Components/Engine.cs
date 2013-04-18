using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Xml;
using System;
using System.IO;

namespace Innovation
{
    public static class Engine
    {
        public static GraphicsDevice GraphicsDevice;
        public static SpriteBatch SpriteBatch;
        public static GameScreenCollection GameScreens = new GameScreenCollection();
        public static GameTime GameTime;
        public static IEServiceContainer Services;
        public static IEContentManager Content;

        public static GameScreen BackgroundScreen;
        public static GameScreen DefaultScreen;

        public static bool IsInitialized = false;

        // Initializes the engine
        public static void SetupEngine(IGraphicsDeviceService GraphicsDeviceService)
        {
            // Setup GraphicsDevice and SpriteBatch
            Engine.GraphicsDevice = GraphicsDeviceService.GraphicsDevice;
            Engine.SpriteBatch = new SpriteBatch(GraphicsDeviceService.GraphicsDevice);

            Engine.Services = new IEServiceContainer();
            Engine.Services.AddService(typeof(IGraphicsDeviceService), GraphicsDeviceService);
            Engine.Content = new IEContentManager(Services);

            // Setup background screen
            BackgroundScreen = new GameScreen("Engine.BackgroundScreen");
            BackgroundScreen.OverrideUpdateBlocked = false;
            BackgroundScreen.OverrideDrawBlocked = true;
            BackgroundScreen.OverrideInputBlocked = true;

            DefaultScreen = BackgroundScreen;

            Engine.IsInitialized = true;
        }

        // Update the engine, screens, and components
        public static void Update(GameTime gameTime)
        {
            Engine.GameTime = gameTime;
            List<GameScreen> updating = new List<GameScreen>();

            foreach (GameScreen screen in GameScreens)
                updating.Add(screen);

            // BlocksUpdate and OverrideUpdateBlocked login
            for (int i = GameScreens.Count - 1; i >= 0; --i)
                if (GameScreens[i].BlocksUpdate)
                {
                    if (i > 0)
                        for (int j = i - 1; j >= 0; --j)
                            if (!GameScreens[j].OverrideUpdateBlocked)
                                updating.Remove(GameScreens[j]);
                    break;
                }

            // Update remaining screens
            foreach (GameScreen screen in updating)
                if (screen.Initialized)
                    screen.Update();

            updating.Clear();

            foreach (GameScreen screen in GameScreens)
                updating.Add(screen);

            // BlocksInput and OverrideInputBlocked login
            for (int i = GameScreens.Count - 1; i >= 0; --i)
                if (GameScreens[i].BlocksInput)
                {
                    if (i > 0)
                        for (int j = i - 1; j >= 0; --j)
                            if (!GameScreens[j].OverrideInputBlocked)
                                updating.Remove(GameScreens[j]);
                    break;
                }

            // Set IsInputAllowed for all GameScreens
            foreach (GameScreen screen in GameScreens)
                if (!screen.InputDisabled)
                    screen.IsInputAllowed = updating.Contains(screen);
                else
                    screen.IsInputAllowed = false;
        }

        // Draws current collection of screens and components
        public static void Draw(GameTime gameTime, ComponentPredicate DrawPredicate)
        {
            Engine.GameTime = gameTime;
            List<GameScreen> drawing = new List<GameScreen>();

            // Clear back buffer
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // Populate temp list if screen is visible
            foreach (GameScreen screen in GameScreens)
                if (screen.Visible)
                    drawing.Add(screen);

            // BlocksDraw and OverrideDrawBlocked logic
            for (int i = GameScreens.Count - 1; i >= 0; i--)
                if (GameScreens[i].BlocksDraw)
                {
                    if (i > 0)
                        for (int j = i - 1; j >= 0; j--)
                        {
                            if (!GameScreens[j].OverrideDrawBlocked)
                                drawing.Remove(GameScreens[j]);
                        }
                    break;
                }

            // Draw the remaining screens
            foreach (GameScreen screen in drawing)
                if (screen.Initialized)
                    screen.Draw(DrawPredicate);
        }

        public static void Draw(GameTime gameTime, ComponentType DrawType)
        {
            Draw(gameTime, new ComponentTypePredicate(DrawType));
        }

        // Resets engine to initial state
        public static void Reset()
        {
            List<Component> destroy = new List<Component>();

            foreach (GameScreen screen in Engine.GameScreens)
                foreach (Component component in screen.Components)
                    destroy.Add(component);

            foreach (Component component in destroy)
                component.DisableComponent();

            List<GameScreen> screenDestroy = new List<GameScreen>();

            foreach (GameScreen screen in GameScreens)
                if (screen != Engine.BackgroundScreen)
                    screenDestroy.Add(screen);

            foreach (GameScreen screen in screenDestroy)
                screen.Disable();

            Engine.Services.Clear();
            Engine.Content.Unload();
        }

        // Save the current state of the engine to file
        public static void SerializeState(string Filename)
        {
            // Get the start time
            DateTime startTime = DateTime.Now;

            // Create an XmlWriter
            XmlWriterSettings set = new XmlWriterSettings();
            set.Indent = true;
            XmlWriter writer = XmlWriter.Create(new FileStream(Filename, FileMode.Create), set);

            // Create Serializer
            Serializer s = new Serializer();

            // Write start of document including root node and save time
            writer.WriteStartDocument();
            writer.WriteStartElement("EngineState");
            writer.WriteAttributeString("Time", startTime.ToString());

            s.WriteGameScreens(writer);

            writer.WriteStartElement("Components");

            // Serialize components
            foreach (GameScreen gameScreen in GameScreens)
                foreach (Component component in gameScreen.Components)
                    if (component.Serialize)
                    {
                        writer.WriteStartElement(component.GetType().FullName);
                        s.Serialize(writer, component.GetSerializationData(s, writer));
                        writer.WriteEndElement();
                    }

            writer.WriteEndElement();

            s.WriteDependencies(writer);

            writer.WriteEndElement();
            writer.WriteEndDocument();

            writer.Close();

            DateTime stopTime = DateTime.Now;
            TimeSpan elapsedTime = stopTime - startTime;
        }
    }
}