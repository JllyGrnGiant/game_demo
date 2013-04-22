using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Innovation
{
    public class GameScreen
    {
        // Keep track of all components we need to manage
        public ComponentCollection Components;

        // Whether or not to draw
        public bool Visible = true;

        // Whether or not this screen should block the update of
        // screens below
        public bool BlocksUpdate = false;

        // Whether or not this screen can override a blocked update
        // from an above screen (for a background screen)
        public bool OverrideUpdateBlocked = false;

        // Same for drawing
        public bool BlocksDraw = false;
        public bool OverrideDrawBlocked = false;

        // Same for input
        public bool BlocksInput = false;
        public bool OverrideInputBlocked = false;

        //public bool IsMouseVisible = false;
        //Texture2D cursorTexture = Engine.Content.Load<Texture2D>("Content/cursor");
        //SpriteBatch cursor = new SpriteBatch(Engine.GraphicsDevice);

        // Whether or not we want to block our own input so we can
        // do things like loading screens that will want to accept input
        // at some point, but not at startup
        public bool InputDisabled = false;

        // Set by the engine to tell us whether or not input is allowed.
        // We can still get input but we shouldn't. This is useful because
        // a ProcessInput() type of function would make it hard to manage
        // input (because we can't utilize events)
        public bool IsInputAllowed = true;

        // The name of our component, set in the constructor.
        // Used by the Engine because a GameScreen can be accessed by
        // name from Engine.GameScreens[Name]
        public string Name;

        // Fired when the component's Initialize() is finished. Can
        // be hooked for things like asynchronous loading screens
        public event EventHandler OnInitialized;

        // Whether or not the component is initialized.
        // Handles firing of OnInitialized
        bool initialized = false;
        public bool Initialized
        {
            get { return initialized; }
            set
            {
                initialized = value;
                if (OnInitialized != null)
                {
                    // Fire the OnInitialized event to let others
                    // know we're done initializing
                    OnInitialized(this, new EventArgs());
                }
            }
        }

        // Constructor takes name of component
        public GameScreen(string Name)
        {
            // Register with engine
            Components = new ComponentCollection(this);
            this.Name = Name;
            Engine.GameScreens.Add(this);

            // Initialize component
            if (!Initialized) { Initialize(); }
        }

        // Overridable function to initialize GameScreen
        public virtual void Initialize()
        {
            this.Initialized = true;
        }

        // Update screen and child components
        public virtual void Update()
        {
            // Create temporary list so we don't crash if a component
            // is added to the collection while updating
            List<Component> updating = new List<Component>();

            // Populate temp list
            foreach (Component c in Components)
                updating.Add(c);

            // Update all components that have been initialized
            foreach (Component Component in updating)
                if (Component.Initialized)
                    Component.Update();
        }

        // Draw screen and its components.
        // Accepts ComponentType to tell what kind of components to draw.
        // Useful for drawing a reflection into a render target without
        // 2D components getting in the way
        public virtual void Draw(ComponentPredicate DrawPredicate)
        {
            //position of cursor
            //if (IsMouseVisible)
            //{
                
            //    Vector2 position = new Vector2(Mouse.GetState().X, Mouse.GetState().Y);
            //    cursor.Begin();
            //    cursor.Draw(cursorTexture, position, Color.White);
            //    cursor.End();

            //}

            // Temp list
            List<Component> drawing = new List<Component>();

            foreach (Component component in Components.InDrawOrder)
                if (DrawPredicate.IsComponentEligible(component))
                    drawing.Add(component);

            // Keep list of components that are 2D so we can draw them
            // on top of 3D components
            List<Component> defer2D = new List<Component>();

            foreach (Component component in drawing)
                if (component.Visible && component.Initialized)
                {
                    if (component is I2DComponent)
                        defer2D.Add(component);
                    else
                        component.Draw();
                }

            // Draw 2D components
            foreach (Component component in defer2D)
                component.Draw();
        }

        // Disables GameScreen
        public virtual void Disable()
        {
            Components.Clear();

            Engine.GameScreens.Remove(this);

            if (Engine.DefaultScreen == this)
                Engine.DefaultScreen = Engine.BackgroundScreen;
        }

        // Override ToString() to return our name
        public override string ToString()
        {
            return Name;
        }
    }
}