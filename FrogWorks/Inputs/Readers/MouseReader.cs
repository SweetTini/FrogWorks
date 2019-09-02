using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace FrogWorks
{
    public class MouseReader
    {
        protected MouseState CurrentState { get; private set; }

        protected MouseState LastState { get; private set; }

        #region Cursor
        public Vector2 Position
        {
            get { return Runner.Application.Game.Display.ToView(CurrentState.Position.ToVector2()); }
            set
            {
                var position = Runner.Application.Game.Display.FromView(value).ToPoint();
                Mouse.SetPosition(position.X, position.Y);
            }
        }

        public float X
        {
            get { return Position.X; }
            set { Position = new Vector2(value, Position.Y); }
        }

        public float Y
        {
            get { return Position.Y; }
            set { Position = new Vector2(Position.X, value); }
        }
        
        public bool IsMoving => CurrentState.Position != LastState.Position;
        #endregion

        #region Mouse Wheel
        public int Wheel => CurrentState.ScrollWheelValue;

        public int WheelDelta => CurrentState.ScrollWheelValue - LastState.ScrollWheelValue;
        #endregion

        internal MouseReader()
        {
        }

        internal void Update(bool isActive)
        {
            LastState = CurrentState;
            CurrentState = isActive ? Mouse.GetState() : new MouseState();
        }

        public bool IsDown(MouseButton button)
        {
            return !Input.IsDisabled && Validate(button, ButtonState.Pressed);
        }

        public bool IsClicked(MouseButton button)
        {
            return !Input.IsDisabled 
                && Validate(button, ButtonState.Pressed) 
                && Validate(button, ButtonState.Released, true);
        }

        public bool IsReleased(MouseButton button)
        {
            return !Input.IsDisabled 
                && Validate(button, ButtonState.Released) 
                && Validate(button, ButtonState.Pressed, true);
        }

        private bool Validate(MouseButton button, ButtonState status, bool checkLastState = false)
        {
            var state = checkLastState ? LastState : CurrentState;

            switch (button)
            {
                case MouseButton.Left: return state.LeftButton == status;
                case MouseButton.Middle: return state.MiddleButton == status;
                case MouseButton.Right: return state.RightButton == status;
            }

            return false;
        }
    }

    public enum MouseButton
    {
        Left,
        Middle,
        Right
    }
}
