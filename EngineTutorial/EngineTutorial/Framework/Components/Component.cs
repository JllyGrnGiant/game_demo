using Microsoft.Xna.Framework;
using System;
using System.Xml;

namespace Innovation
{
    public class Component
    {
        // The GameScreen object that owns this component
        public GameScreen Parent;

        // Whether or not this component has been initialized
        public bool Initialized = false;

        // Whether or not the GameScreen should draw the component
        public bool Visible = true;

        // The draw order of the component. Lower values draw first
        int drawOrder = 1;
        
        // Draw order changed event
        public event ComponentDrawOrderChangedEventHandler DrawOrderChanged;

        static int count = 0;
        public string Name;
        public bool Serialize = true;

        // Overloaded constructor allows us to specify the parent
        public Component(GameScreen Parent)
        {
            InitializeComponent(Parent);
        }

        // Constructor sets the parent to the default GameScreen
        public Component()
        {
            InitializeComponent(Engine.DefaultScreen);
        }

        // Called by the constructor to initialize the component
        protected virtual void InitializeComponent(GameScreen Parent)
        {
            // Check if engine is initialized before setting up component
            // otherwise the Engine will crash when the component tries
            // to add itself to the list
            if (!Engine.IsInitialized)
            {
                throw new Exception("Engine must be initialized with 'SetupEngine()'"
                    + "before components can be initialized");
            }

            Parent.Components.Add(this);
            Initialized = true;

            count++;
            Name = this.GetType().FullName + count;
        }

        // Public draw order. If the value is changed, we fire the draw order change event
        public int DrawOrder
        {
            get { return drawOrder; }
            set
            {
                // Save a copy of the old value and set the new one
                int last = drawOrder;
                drawOrder = value;

                // Fire DrawOrderChanged
                if (DrawOrderChanged != null)
                {
                    DrawOrderChanged(this, new ComponentDrawOrderChangedEventArgs(this, last, this.Parent.Components));
                }
            }
        }

        // Updates the component - Called by the owner
        public virtual void Update() {}

        // Draws the component - Called by the owner
        public virtual void Draw() {}

        // Unregisters the component with its parent
        public virtual void DisableComponent()
        {
            Parent.Components.Remove(this);
        }

        // Returns a SerializationData a Serializer can use to save the state
        // of the object to an Xml file
        public SerializationData GetSerializationData(Serializer Serializer, XmlWriter Writer)
        {
            // Create a new SerializationData
            SerializationData data = new SerializationData(Serializer, Writer);

            // Add the basic Component values
            data.AddData("Component.DrawOrder", DrawOrder);
            data.AddData("Component.ParentScreen", Parent.Name);
            data.AddData("Component.Visible", Visible);
            data.AddData("Component.Name", this.Name);

            // Tell serializer that it will need to know the type of component
            data.AddDependency(this.GetType());

            // Construct a ServiceData
            ServiceData sd = new ServiceData();

            // If this object is a service, find out what the provider type is
            // (the type used to look up the services)
            Type serviceType;
            if (Engine.Services.IsService(this, out serviceType))
            {
                // Tell serializer about provider type
                data.AddDependency(serviceType);

                // Set data to ServiceData
                sd.IsService = true;
                sd.Type = serviceType.FullName;
            }

            // Add the ServiceData to the SerializationData
            data.AddData("Component.ServiceData", sd);

            // Call the overridable function that allows components to provide data
            SaveSerializationData(data);

            return data;
        }

        public void RecieveSerializationData(SerializationData Data)
        {
            // Set the basic Component values
            this.DrawOrder = Data.GetData<int>("Component.DrawOrder");
            this.Visible = Data.GetData<bool>("Component.Visible");
            this.Name = Data.GetData<string>("Component.Name");

            // Get the ServiceData from the data
            ServiceData sd = Data.GetData<ServiceData>("Component.ServiceData");

            // If the conponent was a service
            if (sd.IsService)
            {
                // Get the type back from the serializer
                Type t = Data.GetTypeFromDependency(sd.Type);

                // Add the service to the Engine
                Engine.Services.AddService(t, this);
            }

            // Set the owner GameScreen
            string parent = Data.GetData<string>("Component.ParentScreen");
            this.Parent = Engine.GameScreens[parent];

            // Call the overridable function that allow components to load from data
            LoadFromSerializationData(Data);
        }

        // Overridable function to allow components to save data during serialization
        public virtual void SaveSerializationData(SerializationData Data)
        {
        }

        // Overridable function to allow components to load data during deserialization
        public virtual void LoadFromSerializationData(SerializationData Data)
        {
        }

        public override string ToString()
        {
            return this.Name;
        }
    }

    public class ComponentDrawOrderChangedEventArgs : EventArgs
    {
        // Component modified
        public Component Component;

        // Old draw order
        public int LastDrawOrder;

        // Collection that owns the component
        public ComponentCollection ParentCollection;

        public ComponentDrawOrderChangedEventArgs(Component Component, int LastDrawOrder, ComponentCollection ParentCollection)
        {
            this.Component = Component;
            this.LastDrawOrder = LastDrawOrder;
            this.ParentCollection = ParentCollection;
        }
    }

    // Event handler for draw order change on a component
    public delegate void ComponentDrawOrderChangedEventHandler(object sender, ComponentDrawOrderChangedEventArgs e);
}