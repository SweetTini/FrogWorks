using System;

namespace FrogWorks
{
    public class AnimatedTiledImage : TiledImage
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
            set { _duration = Math.Max(value, 0); }
        }

        public AnimatedTiledImage(Texture[] textures, int[] frames, float duration, int width, int height)
            : base(textures[0], width, height, true)
        {
            Textures = textures;
            Frames = frames;
            Duration = duration;
        }

        public AnimatedTiledImage(Texture texture, int frameWidth, int frameHeight, float duration, int width, int height)
            : base(texture, width, height, true)
        {
            Textures = Texture.Split(texture, frameWidth, frameHeight);
            Texture = Textures[0];
            Frames = new int[Textures.Length];
            Duration = duration;

            for (int i = 0; i < Textures.Length; i++)
                Frames[i] = i;
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
            var index = Frames[_index];
            Texture = Textures[index];
            base.Draw(batch);
        }

        public void SetFrames(params int[] frames)
        {
            for (int i = 0; i < frames.Length; i++)
                if (!Textures.WithinRange(frames[i]))
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
