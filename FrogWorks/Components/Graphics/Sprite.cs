using System;
using System.Collections.Generic;

namespace FrogWorks
{
    public class Sprite<T> : Image
        where T : struct
    {
        bool _isPlaying;

        protected Texture[] Textures { get; set; }

        protected Dictionary<T, Animation> Animations { get; private set; }

        protected Animation Animation
        {
            get
            {
                Animation animation;
                Animations.TryGetValue(CurrentAnimation, out animation);
                return animation;
            }
        }

        public T CurrentAnimation { get; private set; }

        public int MaxFrames => Animation?.MaxFrames ?? 1;

        public int FrameIndex
        {
            get { return Animation?.FrameIndex ?? 0; }
            set
            {
                if (Animation != null)
                    Animation.FrameIndex = value;
            }
        }

        public float DelayPerFrame
        {
            get { return Animation?.DelayPerFrame ?? 0f; }
            set
            {
                if (Animation != null)
                    Animation.DelayPerFrame = value;
            }
        }

        public int MaxLoops
        {
            get { return Animation?.MaxLoops ?? 0; }
            set
            {
                if (Animation != null)
                    Animation.MaxLoops = value;
            }
        }

        public PlayMode PlayMode
        {
            get { return Animation?.PlayMode ?? PlayMode.Normal; }
            set
            {
                if (Animation != null)
                    Animation.PlayMode = value;
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
                Animation?.Update(deltaTime);
        }

        protected override void Draw(RendererBatch batch)
        {
            Texture = Animation?.GetFrame(Textures) ?? Textures[0];
            base.Draw(batch);
        }

        public void Play(T key, bool restart = false)
        {
            if (Animations.ContainsKey(key) && (!IsPlaying(key) || restart))
            {
                CurrentAnimation = key;
                Animation?.ResetChanges();
                _isPlaying = true;
            }
        }

        public bool IsPlaying()
        {
            return Animations.ContainsKey(CurrentAnimation) 
                && _isPlaying
                && Animation.IsPlaying;
        }

        public bool IsPlaying(T key)
        {
            return Animations.ContainsKey(key) 
                && key.Equals(CurrentAnimation) 
                && _isPlaying
                && Animation.IsPlaying;
        }

        public void Stop()
        {
            Animation?.Reset();
            _isPlaying = false;
        }

        public void Resume()
        {
            if (Animations.ContainsKey(CurrentAnimation) && !_isPlaying)
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
            PlayMode playMode,
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
            Animation?.SetFrames(frames);
        }

        public void SetFrames(T key, params int[] frames)
        {
            if (Animations.ContainsKey(key))
                Animations[key]?.SetFrames(frames);
        }
    }
}
