using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Framework.Content.Pipeline.Builder;
using System.Collections.Generic;
using System.IO;

namespace FrogWorks
{
    public class Shader
    {
        private static Dictionary<string, Effect> Cache = new Dictionary<string, Effect>();

        public Effect Effect { get; private set; }

        protected Shader(Effect effect)
            : base()
        {
            Effect = effect;
        }

        #region Static Methods
        public static Shader Load(string filePath)
        {
            var effect = TryGetFromCache(filePath);
            return new Shader(effect);
        }

        internal static Effect TryGetFromCache(string filePath)
        {
            Effect effect;

            if (!Cache.TryGetValue(filePath, out effect))
            {
                var importer = new EffectImporter();
                var processor = new EffectProcessor();
                var pipelineManager = new PipelineManager(string.Empty, string.Empty, string.Empty);

                pipelineManager.Profile = GraphicsProfile.Reach;
                pipelineManager.Platform = TargetPlatform.DesktopGL;

                var processorContext = new PipelineProcessorContext(pipelineManager, new PipelineBuildEvent());
                var absolutePath = Path.Combine(Runner.Application.ContentDirectory, filePath);
                var content = importer.Import(absolutePath, null);
                var compiledContent = processor.Process(content, processorContext);
                var graphicsDevice = Runner.Application.Game.GraphicsDevice;

                effect = new Effect(graphicsDevice, compiledContent.GetEffectCode());
                Cache.Add(filePath, effect);
            }

            return effect;
        }

        internal static void Dispose()
        {
            foreach (var effect in Cache.Values)
                effect.Dispose();

            Cache.Clear();
        }
        #endregion
    }
}
