using Microsoft.Xna.Framework.Input;
using System;

namespace Innovation
{
    public class KeyboardDevice : InputDevice<KeyboardState>
    {
        // The last and current KeyboardStates
        KeyboardState last;
        KeyboardState current;

        // The keys that were pressed in the current state
        Keys[] currentKeys;

        public override KeyboardState State { get { return current; } }
        public Keys[] PressedKeys { get { return currentKeys; } }

        // Events for when a key is pressed, released, and held.
        // This event can be handled by our InputEventHandler class.
        public event InputEventHandler<Keys, KeyboardState> KeyPressed;
        public event InputEventHandler<Keys, KeyboardState> KeyReleased;
        public event InputEventHandler<Keys, KeyboardState> KeyHeld;

        public KeyboardDevice()
        {
            current = Keyboard.GetState();
            Update();
        }

        public override void Update()
        {
            last = current;
            current = Keyboard.GetState();
            currentKeys = current.GetPressedKeys();

            foreach (Keys key in Util.GetEnumValues<Keys>())
            {
                // New key press
                if (WasKeyPressed(key))
                    if (KeyPressed != null)
                        KeyPressed(this, new InputDeviceEventArgs<Keys, KeyboardState>(key, this));

                // Key release
                if (WasKeyReleased(key))
                    if (KeyReleased != null)
                        KeyReleased(this, new InputDeviceEventArgs<Keys, KeyboardState>(key, this));

                // Held key
                if (WasKeyHeld(key))
                    if (KeyHeld != null)
                        KeyHeld(this, new InputDeviceEventArgs<Keys, KeyboardState>(key, this));
            }
        }

        // Whether specified key is currently down
        public bool IsKeyDown(Keys Key)
        {
            return current.IsKeyDown(Key);
        }

        // Whether specified key is currently up
        public bool IsKeyUp(Keys Key)
        {
            return current.IsKeyUp(Key);
        }

        // Whether the key is down for the first time this frame
        public bool WasKeyPressed(Keys Key)
        {
            if (last.IsKeyUp(Key) && current.IsKeyDown(Key))
                return true;

            return false;
        }

        // Whether the key is up for the first time this frame
        public bool WasKeyReleased(Keys Key)
        {
            if (last.IsKeyDown(Key) && current.IsKeyUp(Key))
                return true;

            return false;
        }

        // Whether the specified key has been down for more than one frame
        public bool WasKeyHeld(Keys Key)
        {
            if (last.IsKeyDown(Key) && current.IsKeyDown(Key))
                return true;

            return false;
        }
    }
}