namespace FrogWorks
{
    public sealed class DesktopRunner : Runner
    {
        public string Title
        {
            get { return Game.Window.Title; }
            set { Game.Window.Title = value; }
        }

        public bool AllowAltF4
        {
            get { return Game.Window.AllowAltF4; }
            set { Game.Window.AllowAltF4 = value; }
        }

        public bool AllowUserResizing
        {
            get { return Game.Window.AllowUserResizing; }
            set { Game.Window.AllowUserResizing = value; }
        }

        public bool IsBorderless
        {
            get { return Game.Window.IsBorderless; }
            set { Game.Window.IsBorderless = value; }
        }

        public DesktopRunner(int width, int height, bool fullscreen = false) 
            : base(width, height, 1, fullscreen)
        {
            Title = "New Game";
        }

        public DesktopRunner(int width, int height, int scale)
            : base(width, height, scale, false)
        {
            Title = "New Game";
        }

        public void ToFixedSize(int scale = 1) => Display.ToFixedScale(scale);

        public void ToFullscreen() => Display.ToFullscreen(); 
    }
}
