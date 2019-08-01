using Microsoft.Xna.Framework;
using System;

namespace FrogWorks.Demo.Entities
{
    public class MenuItem : Entity
    {
        protected SpriteText SpriteText { get; private set; }

        protected Color TextColor
        {
            get { return SpriteText.Color; }
            set { SpriteText.Color = value; }
        }

        protected Color BackgroundColor { get; set; }

        public Action OnSelected { get; set; }

        public MenuItem(string label, float x, float y, Action onSelected = null)
        {
            Position = new Vector2(x, y);
            Collider = new RectangleCollider(Engine.Display.Width - 64f, 24f);
            SpriteText = new SpriteText(DefaultFont.Font, label, (int)Collider.Width, (int)Collider.Height);
            SpriteText.HorizontalAlignment = HorizontalAlignment.Center;
            SpriteText.VerticalAlignment = VerticalAlignment.Center;
            AddComponents(SpriteText);
            OnSelected = onSelected;
        }

        public override void Draw(RendererBatch batch)
        {
            batch.DrawPrimitives((pb) => pb.FillRectangle(Collider.AbsolutePosition, Collider.Size, BackgroundColor));
            base.Draw(batch);
        }

        public bool IsHovered(Vector2 cursor)
        {
            var selected = Collider.Contains(cursor);

            TextColor = selected ? Color.Cyan : Color.Gray;
            BackgroundColor = selected ? Color.Navy : Color.Black;

            return selected;
        }
    }
}
