using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrogWorks.Demo
{
    public class TileMapCollisionScene : Scene
    {
        Spider _spider;
        TileMapField _field;

        public TileMapCollisionScene()
            : base()
        {
        }

        protected override void Begin()
        {
            ClearColor = ColorEX.FromRGB(96, 96, 96);

            _field = new TileMapField(8, 7, 32, 32);
            _spider = new Spider(_field, 40, 40);
            Add(_spider, _field);

            Audio.Play<SoundTrack>("Music\\GameOver");
        }
    }
}
