using Microsoft.Xna.Framework;

namespace FrogWorks
{
    public class AnimatedPattern : Pattern
    {
        private float _timer, _duration;
        private int _index;

        protected Texture[] Textures { get; set; }

        protected int[] Frames { get; set; }

        public int Frame
        {
            get { return _index; }
            set
            {
                _index = value.Mod(Frames.Length);
                _timer = 0f;
            }
        }

        public int FrameCount => Frames.Length;

        public float Duration
        {
            get { return _duration; }
            set { _duration = value.Max(0); }
        }

        public AnimatedPattern(Texture[] textures, Point size, int[] frames, float duration)
            : base(textures[0], size, true)
        {
            Textures = textures;
            Frames = frames;
            Duration = duration;

            for (int i = 0; i < Frames.Length; i++)
                Frames[i] = Frames[i].Mod(Textures.Length);
        }

        public AnimatedPattern(Texture[] textures, int width, int height, int[] frames, float duration)
            : this(textures, new Point(width, height), frames, duration)
        {
        }

        public AnimatedPattern(Texture texture, Point frameSize, Point size, float duration)
            : base(texture, size, true)
        {
            Textures = Texture.Split(texture, frameSize);
            Texture = Textures[0];
            Frames = new int[Textures.Length];
            Duration = duration;

            for (int i = 0; i < Textures.Length; i++)
                Frames[i] = i;
        }

        public AnimatedPattern(Texture texture, int frameWidth, int frameHeight, int width, int height, float duration)
            : this(texture, new Point(frameWidth, frameHeight), new Point(width, height), duration)
        {
        }

        protected override void Update(float deltaTime)
        {
            if (_duration == 0f) return;

            _timer += deltaTime;

            if (_timer >= _duration)
            {
                _timer -= _duration;
                _index = (_index++).Mod(Frames.Length);
            }
        }

        protected override void Draw(RendererBatch batch)
        {
            Texture = Textures[Frames[_index]];
            base.Draw(batch);
        }

        public void SetFrames(params int[] frames)
        {
            for (int i = 0; i < frames.Length; i++)
                frames[i] = frames[i].Mod(Textures.Length);

            Frames = frames;
            Reset();
        }

        public void Reset()
        {
            _timer = 0f;
            _index = 0;
        }
    }
}
