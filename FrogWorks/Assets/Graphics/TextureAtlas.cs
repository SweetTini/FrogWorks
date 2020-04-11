using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace FrogWorks
{
    public sealed class TextureAtlas
    {
        Dictionary<string, TextureAtlasTexture> _textures;

        public ReadOnlyDictionary<string, TextureAtlasTexture> Textures
        {
            get; private set;
        }

        public TextureAtlasTexture this[int index]
        {
            get
            {
                var textures = _textures.Values.ToArray();

                return textures.WithinRange(index)
                    ? textures[index]
                    : null;
            }
        }

        public TextureAtlasTexture this[string key]
        {
            get
            {
                TextureAtlasTexture texture;
                _textures.TryGetValue(key, out texture);
                return texture;
            }
        }

        internal TextureAtlas()
        {
            _textures = new Dictionary<string, TextureAtlasTexture>();
            Textures = new ReadOnlyDictionary<string, TextureAtlasTexture>(_textures);
        }

        public void Add(string key, TextureAtlasTexture texture)
        {
            if (!string.IsNullOrWhiteSpace(key) && texture != null)
                _textures.Add(key, texture);
        }

        public void Add(TextureAtlas atlas)
        {
            foreach (var texture in atlas.Textures)
                Add(texture.Key, texture.Value);
        }

        public TextureAtlasTexture[] Get(params string[] keys)
        {
            return keys
                .Where(k => _textures.ContainsKey(k))
                .Select(k => _textures[k])
                .ToArray();
        }

        public int[] GetIndexes(params string[] keys)
        {
            var indexes = _textures
                .Select((kv, i) => new { Index = i, Pair = kv })
                .ToDictionary(k => k.Pair.Key, v => v.Index);

            return keys
                .Where(k => indexes.ContainsKey(k))
                .Select(k => indexes[k])
                .ToArray();
        }

        public TextureAtlasTexture[] ToArray()
        {
            return _textures.Values.ToArray();
        }
    }

    public sealed class TextureAtlasTexture
    {
        private Texture Texture { get; set; }

        public Vector2 RealSize { get; private set; }

        public float RealWidth => RealSize.X;

        public float RealHeight => RealSize.Y;

        public Vector2 Size { get; private set; }

        public float Width => Size.X;

        public float Height => Size.Y;

        public Vector2 Origin { get; private set; }

        public bool IsRotated { get; private set; }

        public TextureAtlasTexture(
            Texture texture,
            Vector2 size,
            Vector2 origin,
            bool isRotated)
        {
            Texture = texture;
            RealSize = isRotated
                ? texture.Size.ToVector2().Rotate().Abs()
                : texture.Size.ToVector2();
            Size = size;
            Origin = origin;
            IsRotated = isRotated;
        }

        public void Draw(
            RendererBatch batch,
            Vector2 position,
            Vector2 origin,
            Vector2 scale,
            float angle,
            Color color,
            SpriteEffects effects)
        {
            var offset = Origin;
            var origEffects = effects;

            if (IsRotated)
            {
                offset = offset.Rotate(true) - Texture.Width * Vector2.UnitX;
                origin = origin.Rotate(true);
                angle -= MathHelper.PiOver2;

                switch (effects)
                {
                    case SpriteEffects.FlipHorizontally:
                        effects = SpriteEffects.FlipVertically;
                        break;
                    case SpriteEffects.FlipVertically:
                        effects = SpriteEffects.FlipHorizontally;
                        break;
                }
            }

            if (origEffects.HasFlag(SpriteEffects.FlipHorizontally))
            {
                if (IsRotated)
                    offset.Y = (Width - RealWidth) - offset.Y;
                else offset.X = (Width - RealWidth) - offset.X;
            }

            if (origEffects.HasFlag(SpriteEffects.FlipVertically))
            {
                if (IsRotated)
                    offset.X = (Height - RealHeight) - (Height + offset.X)
                        * 2f + offset.X;
                else offset.Y = (Height - RealHeight) - offset.Y;
            }

            Texture.Draw(
                batch,
                position,
                origin - offset,
                scale,
                angle,
                color,
                effects);
        }
    }
}
