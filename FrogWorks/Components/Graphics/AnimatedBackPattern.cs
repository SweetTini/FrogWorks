using Microsoft.Xna.Framework;

namespace FrogWorks
{
    public class AnimatedBackPattern : BackPattern
    {
        protected Texture[] Textures { get; set; }

        public Animation Animation { get; protected set; }

        public AnimatedBackPattern(Texture[] textures, Animation animation)
            : base(textures[0], true)
        {
            Textures = textures;
            Animation = animation;
        }

        public AnimatedBackPattern(Texture[] textures, int[] frames, float delayPerFrame, 
                                   AnimationPlayMode playMode, int maxLoops = 0)
            : this(textures, new Animation(frames, delayPerFrame, playMode, maxLoops))
        {
        }

        public AnimatedBackPattern(Texture texture, Point frameSize, Animation animation)
            : base(texture, true)
        {
            Textures = Texture.Split(texture, frameSize);
            Texture = Textures[0];
            TileSize = Texture.Size;
            Animation = animation;
        }

        public AnimatedBackPattern(Texture texture, int frameWidth, int frameHeight, Animation animation)
            : this(texture, new Point(frameWidth, frameHeight), animation)
        {
        }

        public AnimatedBackPattern(Texture texture, Point frameSize, int[] frames, float delayPerFrame, 
                                   AnimationPlayMode playMode, int maxLoops = 0)
            : this(texture, frameSize, new Animation(frames, delayPerFrame, playMode, maxLoops))
        {
        }

        public AnimatedBackPattern(Texture texture, int frameWidth, int frameHeight, int[] frames, 
                                   float delayPerFrame, AnimationPlayMode playMode, int maxLoops = 0)
            : this(texture, new Point(frameWidth, frameHeight), 
                   new Animation(frames, delayPerFrame, playMode, maxLoops))
        {
        }

        protected override void Update(float deltaTime)
        {
            Animation.Update(deltaTime);
        }

        protected override Texture GetTile(int x, int y)
        {
            var texture = base.GetTile(x, y);

            if (texture != null)
                texture = Animation.GetFrame(Textures);

            return texture;
        }
    }
}
