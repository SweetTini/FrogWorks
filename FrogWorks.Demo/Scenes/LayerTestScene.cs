using FrogWorks.Demo.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrogWorks.Demo.Scenes
{
    public class LayerTestScene : Scene
    {
        AlphaMaskLayer _layer;
        Transition _transition;

        public LayerTestScene()
            : base() { }

        protected override void Begin()
        {
            _layer = new AlphaMaskLayer();
            Layers.Add(_layer);

            _transition = new Transition() { X = 160f, Y = 120f };
            _layer.Entities.Add(_transition);
        }
    }
}
