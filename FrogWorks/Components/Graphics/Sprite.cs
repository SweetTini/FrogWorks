using System;
using System.Collections.Generic;

namespace FrogWorks
{
    public class Sprite<T> : Image
        where T : struct
    {
        private T _key;
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

        public int FrameCount
        {
            get { return Animations.ContainsKey(_key) ? Animations[_key].Frames.Length : 1; }
        }

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

        public Action<T, Animation> OnLoop { get; set; }

        public Action<T, Animation> OnFinished { get; set; }

        public Sprite(Texture texture, int frameWidth, int frameHeight)
            : base(texture, true)
        {
            Animations = new Dictionary<T, Animation>();
            Textures = Texture.Split(texture, frameWidth, frameHeight);
            Texture = Textures[0];
        }

        protected override void Update(float deltaTime)
        {
            Animation animation;

            if (_isPlaying && _duration > 0 && Animations.TryGetValue(_key, out animation))
            {
                _timer += deltaTime;

                if (_timer >= _duration)
                {
                    _timer -= _duration;
                    _index++;

                    if (_index >= animation.Frames.Length)
                    {
                        if (animation.Loop && (animation.LoopCount <= 0 || _loops < animation.LoopCount))
                        {
                            _index = _loopFrom;
                            if (_loops < animation.LoopCount) _loops++;
                            OnLoop?.Invoke(_key, animation);
                        }
                        else
                        {
                            _index = animation.Frames.Length - 1;
                            OnFinished?.Invoke(_key, animation);
                            _isPlaying = false;
                        }
                    }
                }
            }
        }

        protected override void Draw(RendererBatch batch)
        {
            Animation animation;
            var index = 0;

            if (Animations.TryGetValue(_key, out animation))
                index = animation.Frames[_index];

            Texture = Textures[index];
            base.Draw(batch);
        }

        public void Play(T key, bool restart = false)
        {
            if (Animations.ContainsKey(key) && (!IsPlaying(key) || restart))
            {
                _key = key;
                _duration = Animations[_key].Duration;
                _loopFrom = Animations[_key].LoopFrom;
                _timer = 0f;
                _index = _loops = 0;
                _isPlaying = true;
            }
        }

        public bool IsPlaying()
        {
            return Animations.ContainsKey(_key) && _isPlaying;
        }

        public bool IsPlaying(T key)
        {
            return Animations.ContainsKey(key) && key.Equals(_key) && _isPlaying;
        }

        public void Stop()
        {
            _timer = 0f;
            _index = _loops = 0;
            _isPlaying = false;
        }

        public void Resume()
        {
            if (Animations.ContainsKey(_key) && !_isPlaying)
                _isPlaying = true;
        }

        public void Pause()
        {
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
                if (IsPlaying(key)) Reset();
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
            _timer = _duration = 0f;
            _index = _loops = _loopFrom = 0;
            _isPlaying = false;
        }
    }
}
