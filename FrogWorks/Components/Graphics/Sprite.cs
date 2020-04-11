using System;
using System.Collections.Generic;

namespace FrogWorks
{
    public class Sprite<T> : Image
        where T : struct
    {
        T _key;
        bool _isPlaying;

        protected Texture[] Textures { get; set; }

        protected Dictionary<T, Animation> Animations { get; private set; }

        protected Animation Current
        {
            get
            {
                Animation animation;
                Animations.TryGetValue(_key, out animation);
                return animation;
            }
        }

        public int MaxFrames => Current?.MaxFrames ?? 1;

        public int FrameIndex
        {
            get { return Current?.FrameIndex ?? 0; }
            set
            {
                if (Current != null)
                    Current.FrameIndex = value;
            }
        }

        public float DelayPerFrame
        {
            get { return Current?.DelayPerFrame ?? 0f; }
            set
            {
                if (Current != null)
                    Current.DelayPerFrame = value;
            }
        }

        public int MaxLoops
        {
            get { return Current?.MaxLoops ?? 0; }
            set
            {
                if (Current != null)
                    Current.MaxLoops = value;
            }
        }

        public AnimationPlayMode PlayMode
        {
            get { return Current?.PlayMode ?? AnimationPlayMode.Normal; }
            set
            {
                if (Current != null)
                    Current.PlayMode = value;
            }
        }

        public Sprite(Texture texture, int frameWidth, int frameHeight)
            : base(texture, true)
        {
            Animations = new Dictionary<T, Animation>();
            Textures = Texture.Split(texture, frameWidth, frameHeight);
            Texture = Textures[0];
        }

        protected override void Update(float deltaTime)
        {
            if (_isPlaying)
                Current?.Update(deltaTime);
        }

        protected override void Draw(RendererBatch batch)
        {
            Texture = Current?.GetFrame(Textures) ?? Textures[0];
            base.Draw(batch);
        }

        public void Play(T key, bool restart = false)
        {
            if (Animations.ContainsKey(key) && (!IsPlaying(key) || restart))
            {
                _key = key;
                Current?.ResetChanges();
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
            Current?.Reset();
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

        public void AddOrUpdate(T key, Animation animation)
        {
            var isFirst = Animations.Count == 0;

            if (Animations.ContainsKey(key)) Animations[key] = animation;
            else Animations.Add(key, animation);

            if (isFirst || IsPlaying(key))
                Play(key, true);
        }

        public void AddOrUpdate(
            T key,
            int[] frames,
            float frameStep,
            AnimationPlayMode playMode,
            int maxLoops = 0,
            Action onFinished = null,
            Action onLoop = null)
        {
            AddOrUpdate(
                key,
                new Animation(
                    frames,
                    frameStep,
                    playMode,
                    maxLoops,
                    onFinished,
                    onLoop));
        }

        public void Remove(T key)
        {
            if (Animations.ContainsKey(key))
                Animations.Remove(key);
        }

        public void Clear()
        {
            Animations.Clear();
        }

        public void SetFrames(params int[] frames)
        {
            Current?.SetFrames(frames);
        }

        public void SetFrames(T key, params int[] frames)
        {
            if (Animations.ContainsKey(key))
                Animations[key]?.SetFrames(frames);
        }
    }
}
