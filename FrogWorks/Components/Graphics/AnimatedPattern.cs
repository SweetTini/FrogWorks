using Microsoft.Xna.Framework;

namespace FrogWorks
{
    public class AnimatedPattern : Pattern
    {
        protected Texture[] Textures { get; set; }

        public Animation Animation { get; protected set; }

        public AnimatedPattern(Texture[] textures, Point size, Animation animation)
            : base(textures[0], size, true)
        {
            Textures = textures;
            Animation = animation;
        }

        public AnimatedPattern(
            Texture[] textures,
            int width,
            int height,
            Animation animation)
            : this(textures, new Point(width, height), animation)
        {
        }

        public AnimatedPattern(
            Texture[] textures,
            Point size,
            int[] frames,
            float frameStep,
            AnimationPlayMode playMode,
            int maxLoops = 0)
            : this(
                  textures,
                  size,
                  new Animation(frames, frameStep, playMode, maxLoops))
        {
        }

        public AnimatedPattern(
            Texture[] textures,
            int width,
            int height,
            int[] frames,
            float frameStep,
            AnimationPlayMode playMode,
            int maxLoops = 0)
            : this(
                  textures,
                  new Point(width, height),
                  new Animation(frames, frameStep, playMode, maxLoops))
        {
        }

        public AnimatedPattern(
            Texture texture,
            Point frameSize,
            Point size,
            Animation animation)
            : base(texture, size, true)
        {
            Textures = Texture.Split(texture, frameSize);
            Texture = Textures[0];
            Animation = animation;
            Resize();
        }

        public AnimatedPattern(
            Texture texture,
            int frameWidth,
            int frameHeight,
            int width,
            int height,
            Animation animation)
            : this(
                  texture,
                  new Point(frameWidth, frameHeight),
                  new Point(width, height), animation)
        {
        }

        public AnimatedPattern(
            Texture texture,
            Point frameSize,
            Point size,
            int[] frames,
            float frameStep,
            AnimationPlayMode playMode,
            int maxLoops = 0)
            : this(
                  texture,
                  frameSize,
                  size,
                  new Animation(frames, frameStep, playMode, maxLoops))
        {
        }

        public AnimatedPattern(
            Texture texture,
            int frameWidth,
            int frameHeight,
            int width,
            int height,
            int[] frames,
            float frameStep,
            AnimationPlayMode playMode,
            int maxLoops = 0)
            : this(
                  texture,
                  new Point(frameWidth, frameHeight),
                  new Point(width, height),
                  new Animation(frames, frameStep, playMode, maxLoops))
        {
        }

        protected override void Update(float deltaTime)
        {
            Animation.Update(deltaTime);
        }

        protected override void Draw(RendererBatch batch)
        {
            Texture = Animation.GetFrame(Textures);
            base.Draw(batch);
        }
    }
}
