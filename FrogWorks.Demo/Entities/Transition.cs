using Microsoft.Xna.Framework;

namespace FrogWorks.Demo.Entities
{
    public class Transition : Entity
    {
        public Transition()
            : base() { }

        protected override void AfterDraw(RendererBatch batch)
            => batch.DrawPrimitives(primitive => primitive.FillCircle(Center, 80f, Color.White, 32));
    }
}
