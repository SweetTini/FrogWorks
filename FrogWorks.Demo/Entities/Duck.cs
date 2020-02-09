using FrogWorks;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrogWorks.Demo
{
    public class Duck : Entity
    {
        private TextureAtlas _atlas;
        private AtlasSprite<int> _sprite;

        public Duck(TextureAtlas altas)
        {
            Position = new Vector2(0f, 232f);

            _atlas = altas;
            _sprite = new AtlasSprite<int>(_atlas.ToArray());
            _sprite.SetOrigin(Origin.Bottom);

            Add(_sprite);
            SetUpAnimations();
        }

        private void SetUpAnimations()
        {
            var runKeys = _atlas.Textures
                .Where(t => t.Key.StartsWith("HeroRun"))
                .Select(t => t.Key)
                .ToArray();

            var indexes = _atlas.GetIndexes(runKeys);

            _sprite.AddOrUpdate(0, new Animation(indexes, .1f, AnimationPlayMode.Loop));
            _sprite.Scale = Vector2.One * .5f;
            _sprite.Color = Color.LightCoral;
        }

        protected override void BeforeUpdate(float deltaTime)
        {
            X += 2f;

            var halfWidth = _sprite.Bounds.Width * _sprite.Scale.X;

            if (X > 320f + halfWidth)
                X = -halfWidth;
        }

        protected override void AfterDraw(RendererBatch batch)
        {
            batch.DrawPrimitives(x => x.DrawDot(Position, Color.Red));
            batch.DrawPrimitives(x => x.DrawCircle(Position, 2f, Color.Blue));
        }
    }
}
