using System;
using System.Collections.ObjectModel;

namespace FrogWorks
{
    public sealed class Animation
    {
        const float _defaultFrameRate = 60f;
        private int[] _frames;
        private float
            _timer, 
            _delayPerFrame, 
            _initialDelayPerFrame;
        private int 
            _index, 
            _randomIndex,
            _loops, 
            _maxLoops, 
            _initialMaxLoops;
        private AnimationPlayMode 
            _playMode, 
            _initialPlayMode;

        public ReadOnlyCollection<int> Frames { get; }

        public int MaxFrames
        {
            get
            {
                switch (_playMode)
                {
                    case AnimationPlayMode.Yoyo:
                    case AnimationPlayMode.LoopYoyo:
                        return (_frames.Length - 1) * 2;
                    default:
                        return _frames.Length;
                }
            }
        }

        public int FrameIndex
        {
            get
            {
                switch (_playMode)
                {
                    case AnimationPlayMode.Reverse:
                    case AnimationPlayMode.LoopReverse:
                        return (MaxFrames - 1) - _index;
                    case AnimationPlayMode.Yoyo:
                    case AnimationPlayMode.LoopYoyo:
                        var half = MaxFrames / 2;
                        return _index > half
                            ? half - (_index - half)
                            : _index;
                    case AnimationPlayMode.LoopRandom:
                        return _randomIndex;
                    default:
                        return _index;
                }
            }
            set
            {
                value = value.Mod(MaxFrames);

                switch (_playMode)
                {
                    case AnimationPlayMode.Reverse:
                    case AnimationPlayMode.LoopReverse:
                        _index = (MaxFrames - 1) - value;
                        break;
                    case AnimationPlayMode.LoopRandom:
                        _index = value;
                        _randomIndex = _index;
                        break;
                    default:
                        _index = value;
                        break;
                }
            }
        }

        public float DelayPerFrame
        {
            get { return _delayPerFrame; }
            set { _delayPerFrame = value.Abs(); }
        }

        public float FrameStep
        {
            get { return _delayPerFrame * _defaultFrameRate; }
            set { _delayPerFrame = value.Abs() / _defaultFrameRate; }
        }

        public float Duration => _delayPerFrame * MaxFrames;

        public AnimationPlayMode PlayMode
        {
            get { return _playMode; }
            set
            {
                if (_playMode == value) return;
                _playMode = value;
                Reset();
            }
        }

        public int MaxLoops
        {
            get { return _maxLoops; }
            set { _maxLoops = value.Abs(); }
        }

        public bool Loop
        {
            get
            {
                return _playMode == AnimationPlayMode.Loop
                    || _playMode == AnimationPlayMode.LoopReverse
                    || _playMode == AnimationPlayMode.LoopYoyo
                    || _playMode == AnimationPlayMode.LoopRandom;
            }
            set
            {
                if (value)
                {
                    switch (_initialPlayMode)
                    {
                        case AnimationPlayMode.Normal:
                            _playMode = AnimationPlayMode.Loop;
                            break;
                        case AnimationPlayMode.Reverse:
                            _playMode = AnimationPlayMode.LoopReverse;
                            break;
                        case AnimationPlayMode.Yoyo:
                            _playMode = AnimationPlayMode.LoopYoyo;
                            break;
                    }
                }
                else
                {
                    switch (_initialPlayMode)
                    {
                        case AnimationPlayMode.Loop:
                            _playMode = AnimationPlayMode.Normal;
                            break;
                        case AnimationPlayMode.LoopReverse:
                            _playMode = AnimationPlayMode.Reverse;
                            break;
                        case AnimationPlayMode.LoopYoyo:
                            _playMode = AnimationPlayMode.Yoyo;
                            break;
                        case AnimationPlayMode.LoopRandom:
                            _playMode = AnimationPlayMode.Normal;
                            break;
                    }
                }
            }
        }

        public bool IsPlaying => _maxLoops < 1 || _loops < _maxLoops - 1;

        public Action OnFinished { get; set; }

        public Action OnLoop { get; set; }

        public Animation(
            int[] frames,
            float frameStep,
            AnimationPlayMode playMode,
            int maxLoops = 0,
            Action onFinished = null,
            Action onLoop = null)
        {
            _frames = frames;
            _initialDelayPerFrame = frameStep.Abs() / _defaultFrameRate;
            _initialMaxLoops = maxLoops.Abs();
            _initialPlayMode = playMode;

            Frames = new ReadOnlyCollection<int>(_frames);
            OnFinished = onFinished;
            OnLoop = onLoop;
            ResetChanges();
        }

        public void Update(float deltaTime)
        {
            if (_delayPerFrame == 0 || !IsPlaying) return;

            if ((_timer += deltaTime) >= _delayPerFrame)
            {
                _timer -= _delayPerFrame;
                _index++;

                if (IsPlaying)
                    _randomIndex = Randomizer
                        .Current.Next(_frames.Length);

                if (_index >= MaxFrames)
                {
                    _loops++;

                    if (Loop)
                    {
                        _index -= MaxFrames;
                        OnLoop?.Invoke();
                    }
                    else
                    {
                        _index = MaxFrames - 1;
                        OnFinished?.Invoke();
                    }
                }
            }
        }

        public void OffsetByTimer(float timer)
        {
            var maxFrames = MaxFrames;
            var lastIndex = _index;

            _timer = timer.Mod(_delayPerFrame);
            _index = (int)(timer / Duration * maxFrames)
                .Floor().Mod(maxFrames);

            if (_index != lastIndex)
                _randomIndex = Randomizer
                    .Current.Next(_frames.Length);
        }

        public void SetFrames(params int[] frames)
        {
            _frames = frames;
            Reset();
        }

        public Texture GetFrame(Texture[] textures)
        {
            if (textures == null)
                return default(Texture);

            var index = _frames[FrameIndex].Mod(textures.Length);
            return textures[index];
        }

        public TextureAtlasTexture GetFrame(TextureAtlasTexture[] textures)
        {
            if (textures == null)
                return default(TextureAtlasTexture);

            var index = _frames[FrameIndex].Mod(textures.Length);
            return textures[index];
        }

        public Texture GetFrame(TileSet tileSet)
        {
            if (tileSet == null)
                return default(Texture);

            var index = _frames[FrameIndex].Mod(tileSet.Count);
            return tileSet[index];
        }

        public void Reset()
        {
            _timer = 0f;
            _index = 0;
            _randomIndex = 0;
            _loops = 0;
        }

        public void ResetChanges()
        {
            _delayPerFrame = _initialDelayPerFrame;
            _maxLoops = _initialMaxLoops;
            _playMode = _initialPlayMode;
            Reset();
        }

        public Animation Clone()
        {
            return new Animation(
                _frames, 
                _delayPerFrame * _defaultFrameRate, 
                _playMode, 
                _maxLoops);
        }
    }

    public enum AnimationPlayMode
    {
        Normal,
        Reverse,
        Yoyo,
        Loop,
        LoopReverse,
        LoopYoyo,
        LoopRandom
    }
}
