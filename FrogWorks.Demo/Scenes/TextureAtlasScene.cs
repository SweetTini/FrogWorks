using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FrogWorks.Demo
{
    public class TextureAtlasScene : DefaultScene
    {
        private TextureAltasTexture[] _textures;
        private int _index;
        private bool _flipX, _flipY;

        protected override void Begin()
        {
            var atlas = TexturePacker.Load(@"Textures/Test2.xml");
            _textures = atlas.ToArray();
        }

        protected override void BeforeUpdate(float deltaTime)
        {
            if (Input.Keyboard.IsPressed(Keys.LeftArrow)) _index--;
            else if (Input.Keyboard.IsPressed(Keys.RightArrow)) _index++;

            if (Input.Keyboard.IsPressed(Keys.Z)) _flipX = !_flipX;
            if (Input.Keyboard.IsPressed(Keys.X)) _flipY = !_flipY;

            _index = _index.Mod(_textures.Length);
        }

        protected override void AfterDraw(RendererBatch batch)
        {
            var texture = _textures[_index];
            var position = (Runner.Application.Size.ToVector2() - texture.Size) * .5f;
            var effects = SpriteEffects.None;

            if (_flipX) effects |= SpriteEffects.FlipHorizontally;
            if (_flipY) effects |= SpriteEffects.FlipVertically;

            batch.Begin();

            batch.DrawPrimitives(b => b.DrawRectangle(position, texture.Size, Color.Red));
            batch.DrawPrimitives(b => b.DrawRectangle(position + texture.Origin, texture.RealSize, Color.Gray));
            texture.Draw(batch, position, Vector2.Zero, Vector2.One, 0f, Color.White, effects);
            batch.DrawPrimitives(b => b.DrawDot(position + texture.Origin, Color.Yellow));
            batch.DrawPrimitives(b => b.DrawCircle(position + texture.Origin, 2f, Color.Cyan));

            batch.End();
        }
    }
}
