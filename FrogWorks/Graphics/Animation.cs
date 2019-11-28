using System;
using System.Collections.ObjectModel;

namespace FrogWorks
{
    public sealed class Animation
    {
        private int[] _frames;
        private int _lastFrameIndex;
        private float _timer, _lastTimer, _delayPerFrame, _origDelayPerFrame;
        private AnimationPlayMode _playMode;

        public ReadOnlyCollection<int> Frames { get; private set; }

        public int FrameIndex
        {
            get
            {
                if (_frames.Length < 1 || _delayPerFrame <= 0f) return 0;

                var maxFrames = _playMode == AnimationPlayMode.Yoyo || _playMode == AnimationPlayMode.LoopYoyo 
                    ? (_frames.Length - 1) * 2 : _frames.Length;
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
                            index = lastIndex != index
                                ? Randomizer.Current.Next(maxFrames)
                                : _lastFrameIndex;
                        }
                        break;
                }

                _lastFrameIndex = index;
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
                            _lastFrameIndex = value > maxFrames
                                ? maxFrames - (value - maxFrames)
                                : value;
                        }
                        break;
                    default:
                        value = value.Mod(_frames.Length);
                        _lastFrameIndex = value;
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

        public float Duration
        {
            get
            {
                switch (_playMode)
                {
                    case AnimationPlayMode.Yoyo:
                    case AnimationPlayMode.LoopYoyo:
                        return _delayPerFrame * (_frames.Length - 1) * 2;
                    default:
                        return _delayPerFrame * _frames.Length;
                }
            }
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

        public bool IsLooping
        {
            get
            {
                return _playMode == AnimationPlayMode.Loop
                    || _playMode == AnimationPlayMode.LoopReverse
                    || _playMode == AnimationPlayMode.LoopYoyo
                    || _playMode == AnimationPlayMode.LoopRandom;
            }
        }

        public Action OnFinished { get; set; }

        public Action OnLoop { get; set; }

        private Animation()
        {
        }

        public Animation(int[] frames, float delayPerFrame, AnimationPlayMode playMode,
                         Action onFinished = null, Action onLoop = null)
        {
            _frames = frames;
            _origDelayPerFrame = delayPerFrame.Abs();
            _playMode = playMode;
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

                if (IsLooping) OnLoop?.Invoke();
                else OnFinished?.Invoke();
            }
        }

        public void SetFrames(params int[] frames)
        {
            _frames = frames;
            Reset();
        }

        public T GetFrame<T>(T[] array)
        {
            if (array == null) return default(T);
            var index = _frames[FrameIndex].Mod(array.Length);
            return array[index];
        }

        public void Reset()
        {
            _timer = 0f;
            _lastTimer = 0f;
            _lastFrameIndex = 0;
        }

        public void ResetChanges()
        {
            _delayPerFrame = _origDelayPerFrame;
            Reset();
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
