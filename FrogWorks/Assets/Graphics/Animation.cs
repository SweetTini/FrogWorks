using System;
using System.Collections.ObjectModel;

namespace FrogWorks
{
    public sealed class Animation
    {
        private int[] _frames;
        private float _timer, _lastTimer, _delayPerFrame, _origDelayPerFrame;
        private int _lastIndex, _loops, _maxLoops, _origMaxLoops;
        private bool _randUpdated;
        private AnimationPlayMode _playMode, _origPlayMode;

        public ReadOnlyCollection<int> Frames { get; private set; }

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
                if (_frames.Length < 1 || _delayPerFrame <= 0f) return 0;

                var maxFrames = MaxFrames;
                var index = (int)(_timer / Duration * maxFrames).Floor();

                switch (_playMode)
                {
                    case AnimationPlayMode.Normal:
                    case AnimationPlayMode.Loop:
                        index = IsLooping 
                            ? index.Mod(maxFrames) 
                            : index.Min(maxFrames - 1);
                        break;
                    case AnimationPlayMode.Reverse:
                    case AnimationPlayMode.LoopReverse:
                        index = IsLooping 
                            ? index.Mod(maxFrames) 
                            : index.Min(maxFrames - 1);
                        index = (maxFrames - 1) - index;
                        break;
                    case AnimationPlayMode.Yoyo:
                    case AnimationPlayMode.LoopYoyo:
                        maxFrames /= 2;
                        index = index > maxFrames
                            ? maxFrames - (index - maxFrames)
                            : index;
                        break;
                    case AnimationPlayMode.LoopRandom:
                        {
                            var lastIndex = (int)(_lastTimer / Duration * maxFrames).Floor();
                            index = lastIndex != index && !_randUpdated
                                ? Randomizer.Current.Next(maxFrames) 
                                : _lastIndex;
                            _randUpdated = lastIndex != index;
                        }
                        break;
                }

                _lastIndex = index;
                return index;
            }
            set
            {
                switch (_playMode)
                {
                    case AnimationPlayMode.Yoyo:
                    case AnimationPlayMode.LoopYoyo:
                        {
                            var maxFrames = (_frames.Length - 1) * 2;
                            value = value.Mod(maxFrames);
                            maxFrames /= 2;
                            _lastIndex = value > maxFrames
                                ? maxFrames - (value - maxFrames)
                                : value;
                        }
                        break;
                    default:
                        value = value.Mod(_frames.Length);
                        _lastIndex = value;
                        break;
                }

                _timer = value * _delayPerFrame;
            }
        }

        public float DelayPerFrame
        {
            get { return _delayPerFrame; }
            set { _delayPerFrame = value.Abs(); }
        }

        public float Duration => _delayPerFrame * MaxFrames;

        public int MaxLoops
        {
            get { return _maxLoops; }
            set { _maxLoops = value.Abs(); }
        }

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
                    switch (_origPlayMode)
                    {
                        case AnimationPlayMode.Normal: _playMode = AnimationPlayMode.Loop; break;
                        case AnimationPlayMode.Reverse: _playMode = AnimationPlayMode.LoopReverse; break;
                        case AnimationPlayMode.Yoyo: _playMode = AnimationPlayMode.LoopYoyo; break;
                        default: break;
                    }
                }
                else
                {
                    switch (_origPlayMode)
                    {
                        case AnimationPlayMode.Loop: _playMode = AnimationPlayMode.Normal; break;
                        case AnimationPlayMode.LoopReverse: _playMode = AnimationPlayMode.Reverse; break;
                        case AnimationPlayMode.LoopYoyo: _playMode = AnimationPlayMode.Yoyo; break;
                        case AnimationPlayMode.LoopRandom:
                            _playMode = AnimationPlayMode.Normal;
                            FrameIndex = _lastIndex;
                            break;
                        default: break;
                    }
                }
            }
        }

        public bool IsLooping => Loop && (_maxLoops < 1 || _loops < _maxLoops - 1);

        public Action OnFinished { get; set; }

        public Action OnLoop { get; set; }

        private Animation()
        {
        }

        public Animation(int[] frames, float delayPerFrame, AnimationPlayMode playMode, int maxLoops = 0,
                         Action onFinished = null, Action onLoop = null)
        {
            _frames = frames;
            _origDelayPerFrame = delayPerFrame.Abs();
            _origMaxLoops = maxLoops.Abs();
            _origPlayMode = playMode;
            OnFinished = onFinished;
            OnLoop = onLoop;

            Frames = new ReadOnlyCollection<int>(_frames);
            ResetChanges();
        }

        public void Update(float deltaTime)
        {
            _lastTimer = _timer;

            if (_delayPerFrame == 0 || _timer == Duration) return;

            if ((_timer += deltaTime) >= Duration)
            {
                _timer = IsLooping 
                    ? _timer.Mod(Duration) 
                    : _timer.Min(Duration);
                _loops++;

                if (IsLooping) OnLoop?.Invoke();
                else OnFinished?.Invoke();
            }
        }

        public void OffsetTimer(float timer)
        {
            var deltaTime = timer - _lastTimer;
            Update(timer);
        }

        public void SetFrames(params int[] frames)
        {
            _frames = frames;
            Reset();
        }

        public Texture GetFrame(Texture[] textures)
        {
            if (textures == null) return default(Texture);
            var index = _frames[FrameIndex].Mod(textures.Length);
            return textures[index];
        }

        public TextureAtlasTexture GetFrame(TextureAtlasTexture[] textures)
        {
            if (textures == null) return default(TextureAtlasTexture);
            var index = _frames[FrameIndex].Mod(textures.Length);
            return textures[index];
        }

        public Texture GetFrame(TileSet tileSet)
        {
            if (tileSet == null) return default(Texture);
            var index = _frames[FrameIndex].Mod(tileSet.Count);
            return tileSet[index];
        }

        public void Reset()
        {
            _timer = 0f;
            _lastTimer = 0f;
            _lastIndex = 0;
            _loops = 0;
            _randUpdated = false;
        }

        public void ResetChanges()
        {
            _delayPerFrame = _origDelayPerFrame;
            _maxLoops = _origMaxLoops;
            _playMode = _origPlayMode;
            Reset();
        }

        public Animation Clone()
        {
            return new Animation(_frames, _origDelayPerFrame, _origPlayMode, _origMaxLoops);
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
