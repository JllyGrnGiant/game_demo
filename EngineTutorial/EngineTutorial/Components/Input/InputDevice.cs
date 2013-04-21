using System;

namespace Innovation
{
    // Base input device class. Other input device types will
    // inherit from it. The <T> generic allows us to specify what
    // type of input device state it manages. (MouseState, etc.)
    public abstract class InputDevice<T> : Component
    {
        public abstract T State { get; }
    }

    // Input device event argument class that can handle events for several types of
    // input devices. O specifies the type of object the event was triggered by (Key, Button,
    // MouseButton, etc.). S specifies the type of state provided (MouseState, KeyboardState, etc)
    public class InputDeviceEventArgs<O, S> : EventArgs
    {
        // The object that triggered the event
        public O Object;

        // The input device that manages the state that owns the triggered object
        public InputDevice<S> Device;

        // The state of the input device that was triggered
        public S State;

        public InputDeviceEventArgs(O Object, InputDevice<S> Device)
        {
            this.Object = Object;
            this.Device = Device;
            this.State = ((InputDevice<S>)Device).State;
        }
    }

    // An input device event handler delegate. This defines what type of method
    // may handle an event. In this case, it is a void that accepts the specified
    // input device arguments.
    public delegate void InputEventHandler<O, S>(object sender, InputDeviceEventArgs<O, S> e);
}