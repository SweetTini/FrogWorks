﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace FrogWorks
{
    public sealed class TextureAtlas
    {
        Dictionary<string, TextureAltasTexture> _textures;

        public ReadOnlyDictionary<string, TextureAltasTexture> Textures { get; private set; }

        public TextureAltasTexture this[string key]
        {
            get
            {
                TextureAltasTexture texture;
                _textures.TryGetValue(key, out texture);
                return texture;
            }
        }

        internal TextureAtlas()
        {
            _textures = new Dictionary<string, TextureAltasTexture>();
            Textures = new ReadOnlyDictionary<string, TextureAltasTexture>(_textures);
        }

        public void Add(string key, TextureAltasTexture texture)
        {
            if (!string.IsNullOrWhiteSpace(key) && texture != null)
                _textures.Add(key, texture);
        }

        public void Add(TextureAtlas atlas)
        {
            foreach (var texture in atlas.Textures)
                Add(texture.Key, texture.Value);
        }

        public TextureAltasTexture[] Get(params string[] keys)
        {
            return keys.Distinct()
                .Where(k => _textures.ContainsKey(k))
                .Select(k => _textures[k])
                .ToArray();
        }

        public TextureAltasTexture[] ToArray()
        {
            return _textures.Values.ToArray();
        }
    }

    public sealed class TextureAltasTexture
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

        public TextureAltasTexture(Texture texture, Vector2 size, Vector2 origin, bool isRotated)
        {
            Texture = texture;
            RealSize = isRotated
                ? texture.Size.ToVector2().Perpendicular().Abs()
                : texture.Size.ToVector2();
            Size = size;
            Origin = origin;
            IsRotated = isRotated;
        }

        public void Draw(RendererBatch batch, Vector2 position, Vector2 origin,
            Vector2 scale, float angle, Color color, SpriteEffects effects)
        {
            var offset = Origin;
            var origEffects = effects;

            if (IsRotated)
            {
                offset = offset.Perpendicular(true) - Texture.Width * Vector2.UnitX;
                angle -= MathHelper.PiOver2;

                switch (effects)
                {
                    case SpriteEffects.FlipHorizontally: effects = SpriteEffects.FlipVertically; break;
                    case SpriteEffects.FlipVertically: effects = SpriteEffects.FlipHorizontally; break;
                }
            }

            if (origEffects.HasFlag(SpriteEffects.FlipHorizontally))
            {
                if (IsRotated) offset.Y = (Width - RealWidth) - offset.Y;
                else offset.X = (Width - RealWidth) - offset.X;
            }

            if (origEffects.HasFlag(SpriteEffects.FlipVertically))
            {
                if (IsRotated) offset.X = (Height - RealHeight) - (Height + offset.X) * 2f + offset.X;
                else offset.Y = (Height - RealHeight) - offset.Y;
            }

            Texture.Draw(batch, position, origin - offset, scale, angle, color, effects);
        }
    }
}
