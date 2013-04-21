using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Innovation
{
    // An input device that keeps track of the GamePad and GamePadState
    public class GamepadDevice : InputDevice<GamePadState>
    {
        // The last and current MouseStates
        GamePadState last;
        GamePadState current;

        // The MouseButtons that are currently down
        Buttons[] currentButtons;

        // Public properties for the above members
        public override GamePadState State { get { return current; } }
        public Buttons[] PressedButtons { get { return currentButtons; } }

        // The position in (X,Y) coordinates of the thumbsticks
        public Vector2 LeftStickPosition 
            { get { return current.ThumbSticks.Left; } }
        public Vector2 RightStickPosition
            { get { return current.ThumbSticks.Right; } }

        // The positions of the triggers
        public float LeftTrigger
            { get { return current.Triggers.Left; } }
        public float RightTrigger
            { get { return current.Triggers.Right; } }

        // The change in position of the thumbstricks between the last two frames
        public Vector2 LeftStickDelta = Vector2.Zero;
        public Vector2 RightStickDelta = Vector2.Zero;

        // The change in position of the triggers between the last two frames
        public float LeftTriggerDelta = 0;
        public float RightTriggerDelta = 0;

        // The GamePad's PlayerIndex
        public PlayerIndex PlayerIndex;

        // Whether the controller is connected
        public bool IsConnected { get { return current.IsConnected; } }

        // Events for when a button is pressed, released, or held
        public event InputEventHandler<Buttons, GamePadState> ButtonPressed;
        public event InputEventHandler<Buttons, GamePadState> ButtonReleased;
        public event InputEventHandler<Buttons, GamePadState> ButtonHeld;

        // Events for when the controller is connected and disconnected
        public event EventHandler Connected;
        public event EventHandler Disconnected;

        // Constructor gets the initial GamePadState, and does the first update.
        // It also takes the PlayerIndex of the controller (One, Two, Three, Four)
        public GamepadDevice(PlayerIndex PlayerIndex) : base()
        {
            this.PlayerIndex = PlayerIndex;
            current = GamePad.GetState(PlayerIndex);
            Update();
        }

        // List of pressed buttons used to setup currentButtons
        List<Buttons> pressed = new List<Buttons>();

        public override void Update()
        {
            // Update the last state
            last = current;

            // Update the current state and clear the list of pressed
            // buttons
            current = GamePad.GetState(PlayerIndex);
            pressed.Clear();

            // Only update if the controller is still connected
            if (IsConnected)
            {
                // If the controller connected this frame, fire the event
                if (!last.IsConnected)
                    if (Connected != null)
                        Connected(this, new EventArgs());

                LeftStickDelta = current.ThumbSticks.Left - last.ThumbSticks.Left;
                RightStickDelta = current.ThumbSticks.Right - last.ThumbSticks.Right;
                LeftTriggerDelta = current.Triggers.Left - last.Triggers.Left;
                RightTriggerDelta = current.Triggers.Right - last.Triggers.Right;

                // For each mouse button...
                foreach (Buttons button in Util.GetEnumValues<Buttons>())
                {
                    // If it is down, add it to the list
                    if (IsButtonDown(button))
                        pressed.Add(button);

                    // If it was just pressed, fire the event
                    if (WasButtonPressed(button))
                        if (ButtonPressed != null)
                            ButtonPressed(this, new InputDeviceEventArgs
                                <Buttons, GamePadState>(button, this));

                    // If it was just released, fire the event
                    if (WasButtonReleased(button))
                        if (ButtonReleased != null)
                            ButtonReleased(this, new InputDeviceEventArgs
                                <Buttons, GamePadState>(button, this));

                    // If it was held, fire the event
                    if (WasButtonHeld(button))
                        if (ButtonHeld != null)
                            ButtonHeld(this, new InputDeviceEventArgs
                                <Buttons, GamePadState>(button, this));
                }

                // Update the currentButtons array from the list of buttons
                // that are down
                currentButtons = pressed.ToArray();
            }
            else
                // If the controller was just disconnected, fire the event
                if (last.IsConnected)
                    if (Disconnected != null)
                        Disconnected(this, new EventArgs());
        }

        // Whether the specified button is currently down
        public bool IsButtonDown(Buttons Button)
        {
            return current.IsButtonDown(Button);
        }

        // Whether the specified button is currently up
        public bool IsButtonUp(Buttons Button)
        {
            return current.IsButtonUp(Button);
        }

        // Whether the specified button is down for the time this frame
        public bool WasButtonPressed(Buttons Button)
        {
            if (last.IsButtonUp(Button) && current.IsButtonDown(Button))
                return true;

            return false;
        }

        // Whether the specified button is up for the first this frame
        public bool WasButtonReleased(Buttons Button)
        {
            if (last.IsButtonDown(Button) && current.IsButtonUp(Button))
                return true;

            return false;
        }

        // Whether the specified button has been down for more than one frame
        public bool WasButtonHeld(Buttons Button)
        {
            if (last.IsButtonDown(Button) && current.IsButtonDown(Button))
                return true;

            return false;
        }
    }
}
