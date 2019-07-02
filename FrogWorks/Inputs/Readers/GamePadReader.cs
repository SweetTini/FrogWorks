using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;

namespace FrogWorks
{
    public class GamePadReader
    {
        protected GamePadState CurrentState { get; private set; }

        protected GamePadState LastState { get; private set; }

        public int PlayerIndex { get; private set; }

        public bool IsConnected { get; private set; }

        internal GamePadReader(int playerIndex)
        {
            PlayerIndex = playerIndex;
        }

        internal void Update(bool isActive)
        {
            LastState = CurrentState;
            CurrentState = isActive ? GamePad.GetState(PlayerIndex) : new GamePadState();
            IsConnected = GamePad.GetState(PlayerIndex).IsConnected;
        }

        #region Buttons
        public bool IsDown(GamePadButton button)
        {
            return !Input.IsDisabled && CurrentState.IsButtonDown((Buttons)button);
        }

        public bool IsPressed(GamePadButton button)
        {
            return !Input.IsDisabled
                && CurrentState.IsButtonDown((Buttons)button)
                && LastState.IsButtonUp((Buttons)button);
        }

        public bool IsReleased(GamePadButton button)
        {
            return !Input.IsDisabled
                && CurrentState.IsButtonUp((Buttons)button)
                && LastState.IsButtonDown((Buttons)button);
        }
        #endregion

        #region Left Analog
        public Vector2 GetLeftAnalog(float deadZone = 0f)
        {
            var axis = CurrentState.ThumbSticks.Left * new Vector2(1f, -1f);
            return !Input.IsDisabled && axis.LengthSquared() >= deadZone * deadZone ? axis : Vector2.Zero;
        }

        public bool IsLeftAnalogDown(AnalogAxis axis, float deadZone)
        {
            return !Input.IsDisabled && ValidateAnalog(axis, CurrentState.ThumbSticks.Left, deadZone);
        }

        public bool IsLeftAnalogPressed(AnalogAxis axis, float deadZone)
        {
            return !Input.IsDisabled
                && ValidateAnalog(axis, CurrentState.ThumbSticks.Left, deadZone)
                && !ValidateAnalog(axis, LastState.ThumbSticks.Left, deadZone);
        }

        public bool IsLeftAnalogReleased(AnalogAxis axis, float deadZone)
        {
            return !Input.IsDisabled
                && !ValidateAnalog(axis, CurrentState.ThumbSticks.Left, deadZone)
                && ValidateAnalog(axis, LastState.ThumbSticks.Left, deadZone);
        }
        #endregion

        #region Right Analog
        public Vector2 GetRightAnalog(float deadZone = 0f)
        {
            var axis = CurrentState.ThumbSticks.Right * new Vector2(1f, -1f);
            return !Input.IsDisabled && axis.LengthSquared() >= deadZone * deadZone ? axis : Vector2.Zero;
        }

        public bool IsRightAnalogDown(AnalogAxis axis, float deadZone)
        {
            return !Input.IsDisabled && ValidateAnalog(axis, CurrentState.ThumbSticks.Right, deadZone);
        }

        public bool IsRightAnalogPressed(AnalogAxis axis, float deadZone)
        {
            return !Input.IsDisabled
                && ValidateAnalog(axis, CurrentState.ThumbSticks.Right, deadZone)
                && !ValidateAnalog(axis, LastState.ThumbSticks.Right, deadZone);
        }

        public bool IsRightAnalogReleased(AnalogAxis axis, float deadZone)
        {
            return !Input.IsDisabled
                && !ValidateAnalog(axis, CurrentState.ThumbSticks.Right, deadZone)
                && ValidateAnalog(axis, LastState.ThumbSticks.Right, deadZone);
        }
        #endregion

        #region Left Trigger
        public float GetLeftTrigger(float threshold = 0f)
        {
            return CurrentState.Triggers.Left >= threshold ? CurrentState.Triggers.Left : 0f;
        }

        public bool IsLeftTriggerDown(float threshold)
        {
            return !Input.IsDisabled && ValidateTrigger(CurrentState.Triggers.Left, threshold);
        }

        public bool IsLeftTriggerPressed(float threshold)
        {
            return !Input.IsDisabled
                && ValidateTrigger(CurrentState.Triggers.Left, threshold)
                && !ValidateTrigger(LastState.Triggers.Left, threshold);
        }

        public bool IsLeftTriggerReleased(float threshold)
        {
            return !Input.IsDisabled
                && !ValidateTrigger(CurrentState.Triggers.Left, threshold)
                && ValidateTrigger(LastState.Triggers.Left, threshold);
        }
        #endregion

        #region Right Trigger
        public float GetRightTrigger(float threshold = 0f)
        {
            return CurrentState.Triggers.Right >= threshold ? CurrentState.Triggers.Right : 0f;
        }

        public bool IsRightTriggerDown(float threshold)
        {
            return !Input.IsDisabled && ValidateTrigger(CurrentState.Triggers.Right, threshold);
        }

        public bool IsRightTriggerPressed(float threshold)
        {
            return !Input.IsDisabled
                && ValidateTrigger(CurrentState.Triggers.Right, threshold)
                && !ValidateTrigger(LastState.Triggers.Right, threshold);
        }

        public bool IsRightTriggerReleased(float threshold)
        {
            return !Input.IsDisabled
                && !ValidateTrigger(CurrentState.Triggers.Right, threshold)
                && ValidateTrigger(LastState.Triggers.Right, threshold);
        }
        #endregion

        private bool ValidateAnalog(AnalogAxis axis, Vector2 analog, float deadZone)
        {
            analog *= new Vector2(1, -1);

            switch (axis)
            {
                case AnalogAxis.Left: return analog.X <= -deadZone;
                case AnalogAxis.Right: return analog.X >= deadZone;
                case AnalogAxis.Top: return analog.Y <= -deadZone;
                case AnalogAxis.Bottom: return analog.Y >= deadZone;
            }

            return false;
        }

        private bool ValidateTrigger(float trigger, float threshold)
        {
            return trigger >= threshold;
        }
    }

    [Flags]
    public enum GamePadButton
    {
        DPadUp = 1,
        DPadDown = 2,
        DPadLeft = 4,
        DPadRight = 8,
        Start = 16,
        Back = 32,
        LeftAnalog = 64,
        RightAnalog = 128,
        LeftShoulder = 256,
        RightShoulder = 512,
        BigButton = 2048,
        A = 4096,
        B = 8192,
        X = 16384,
        Y = 32768,
        LeftAnalogLeft = 2097152,
        RightTrigger = 4194304,
        LeftTrigger = 8388608,
        RightAnalogUp = 16777216,
        RightAnalogDown = 33554432,
        RightAnalogRight = 67108864,
        RightAnalogLeft = 134217728,
        LeftAnalogUp = 268435456,
        LeftAnalogDown = 536870912,
        LeftAnalogRight = 1073741824
    }

    public enum AnalogAxis
    {
        Left,
        Right,
        Top,
        Bottom
    }
}
