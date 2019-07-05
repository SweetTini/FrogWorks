using System;
using System.Collections.Generic;

namespace FrogWorks
{
    public class Sprite<T> : Image
    {
        private T _key;
        private Animation? _animation;
        private float _timer, _duration;
        private int _index, _loops, _loopFrom;
        private bool _isPlaying;

        protected Texture[] Textures { get; set; }

        protected Dictionary<T, Animation> Animations { get; private set; }

        public int Frame
        {
            get { return _index; }
            set
            {
                _index = value.Mod(FrameCount);
                _timer = 0f;
            }
        }

        public int FrameCount => _animation?.Frames.Length ?? 1;

        public float Duration
        {
            get { return _duration; }
            set { _duration = Math.Max(value, 0f); }
        }

        public int LoopFrom
        {
            get { return _loopFrom; }
            set { _loopFrom = value.Mod(FrameCount); }
        }

        public Action<T, Animation> OnLooped { get; set; }

        public Action<T, Animation> OnFinished { get; set; }

        public Sprite(Texture texture, int frameWidth, int frameHeight)
            : base(texture, true)
        {
            Animations = new Dictionary<T, Animation>();
            Textures = Texture.Split(texture, frameWidth, frameHeight);
            Texture = Textures[0];
        }

        public override void Update(float deltaTime)
        {
            if (!_animation.HasValue || !_isPlaying || _duration == 0f) return;

            _timer += deltaTime;

            if (_timer >= _duration)
            {
                _timer -= _duration;
                _index++;

                if (_index >= _animation.Value.Frames.Length)
                {
                    if (_animation.Value.Loop && (_animation.Value.LoopCount <= 0 || _loops < _animation.Value.LoopCount))
                    {
                        _index = _loopFrom;
                        if (_loops < _animation.Value.LoopCount) _loops++;
                        OnLooped?.Invoke(_key, _animation.Value);
                    }
                    else
                    {
                        _index = _animation.Value.Frames.Length - 1;
                        OnFinished?.Invoke(_key, _animation.Value);
                        _isPlaying = false;
                    }
                }
            }
        }

        public override void Draw(RendererBatch batch)
        {
            var index = _animation?.Frames[_index] ?? 0;
            Texture = Textures[index];
            base.Draw(batch);
        }

        public void Play(T key, bool restart = false)
        {
            if (!IsPlaying(key) || restart)
            {
                _key = key;
                _animation = Animations[key];
                _duration = _animation.Value.Duration;
                _loopFrom = _animation.Value.LoopFrom;
                _timer = 0f;
                _index = _loops = 0;
                _isPlaying = true;
            }
        }

        public bool IsPlaying()
        {
            return _isPlaying && _animation.HasValue;
        }

        public bool IsPlaying(T key)
        {
            return _isPlaying
                && Animations.ContainsKey(key)
                && _animation.HasValue
                && _animation.Value.Equals(Animations[key]);
        }

        public void Pause()
        {
            _isPlaying = false;
        }

        public void Resume()
        {
            if (!_isPlaying && _animation.HasValue)
                _isPlaying = true;
        }

        public void Stop()
        {
            _timer = 0f;
            _index = _loops = 0;
            _isPlaying = false;
        }

        public void Add(T key, int[] frames, float duration, bool loop = true, int loopCount = 0, int loopFrom = 0)
        {
            var isFirst = Animations.Count == 0;

            for (int i = 0; i < frames.Length; i++)
                frames[i] = frames[i].Mod(Textures.Length);

            var animation = new Animation()
            {
                Frames = frames,
                Duration = duration,
                Loop = loop,
                LoopCount = loopCount,
                LoopFrom = loopFrom
            };

            if (Animations.ContainsKey(key)) Animations[key] = animation;
            else Animations.Add(key, animation);

            if (isFirst || IsPlaying(key))
                Play(key, true);
        }

        public void Remove(T key)
        {
            if (Animations.ContainsKey(key))
            {
                if (IsPlaying(key))
                    Reset();

                Animations.Remove(key);
            }
        }

        public void Clear()
        {
            Reset();
            Animations.Clear();
        }

        private void Reset()
        {
            _key = default(T);
            _animation = null;
            _timer = _duration = 0f;
            _index = _loops = _loopFrom = 0;
            _isPlaying = false;
        }
    }
}
