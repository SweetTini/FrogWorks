using Microsoft.Xna.Framework.Input;
using XnaKeys = Microsoft.Xna.Framework.Input.Keys;

namespace FrogWorks
{
    public class KeyboardReader
    {
        protected KeyboardState CurrentState { get; private set; }

        protected KeyboardState LastState { get; private set; }

        internal KeyboardReader()
        {
        }

        internal void Update(bool isActive)
        {
            LastState = CurrentState;
            CurrentState = isActive ? Keyboard.GetState() : new KeyboardState();
        }

        public bool IsDown(Keys key)
        {
            return !Input.IsDisabled && CurrentState.IsKeyDown((XnaKeys)key);
        }

        public bool IsPressed(Keys key)
        {
            return !Input.IsDisabled 
                && CurrentState.IsKeyDown((XnaKeys)key) 
                && LastState.IsKeyUp((XnaKeys)key);
        }

        public bool IsReleased(Keys key)
        {
            return !Input.IsDisabled 
                && CurrentState.IsKeyUp((XnaKeys)key) 
                && LastState.IsKeyDown((XnaKeys)key);
        }
    }

    public enum Keys
    {
        None = 0,
        Backspace = 8,
        Tab = 9,
        Enter = 13,
        Pause = 19,
        CapsLock = 20,
        Escape = 27,
        Space = 32,
        PageUp = 33,
        PageDown = 34,
        End = 35,
        Home = 36,
        LeftArrow = 37,
        UpArrow = 38,
        RightArrow = 39,
        DownArrow = 40,
        Select = 41,
        Print = 42,
        Execute = 43,
        PrintScreen = 44,
        Insert = 45,
        Delete = 46,
        Help = 47,
        Alpha0 = 48,
        Alpha1 = 49,
        Alpha2 = 50,
        Alpha3 = 51,
        Alpha4 = 52,
        Alpha5 = 53,
        Alpha6 = 54,
        Alpha7 = 55,
        Alpha8 = 56,
        Alpha9 = 57,
        A = 65,
        B = 66,
        C = 67,
        D = 68,
        E = 69,
        F = 70,
        G = 71,
        H = 72,
        I = 73,
        J = 74,
        K = 75,
        L = 76,
        M = 77,
        N = 78,
        O = 79,
        P = 80,
        Q = 81,
        R = 82,
        S = 83,
        T = 84,
        U = 85,
        V = 86,
        W = 87,
        X = 88,
        Y = 89,
        Z = 90,
        NumPad0 = 96,
        NumPad1 = 97,
        NumPad2 = 98,
        NumPad3 = 99,
        NumPad4 = 100,
        NumPad5 = 101,
        NumPad6 = 102,
        NumPad7 = 103,
        NumPad8 = 104,
        NumPad9 = 105,
        Multiply = 106,
        Add = 107,
        Separator = 108,
        Subtract = 109,
        Decimal = 110,
        Divide = 111,
        F1 = 112,
        F2 = 113,
        F3 = 114,
        F4 = 115,
        F5 = 116,
        F6 = 117,
        F7 = 118,
        F8 = 119,
        F9 = 120,
        F10 = 121,
        F11 = 122,
        F12 = 123,
        NumLock = 144,
        Scroll = 145,
        LeftShift = 160,
        RightShift = 161,
        LeftControl = 162,
        RightControl = 163,
        LeftAlt = 164,
        RightAlt = 165
    }
}
