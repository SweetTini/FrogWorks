using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Framework.Content.Pipeline.Builder;
using System;
using System.Collections.Generic;
using System.IO;

namespace FrogWorks
{
    public abstract class Shader
    {
        private static Dictionary<string, Effect> Cache { get; } = new Dictionary<string, Effect>();

        public Effect Effect { get; private set; }

        protected Shader()
        {
        }

        protected Shader(Effect effect)
            : this()
        {
            Initialize(effect);
        }

        protected virtual void Initialize()
        {
        }

        private void Initialize(Effect effect)
        {
            if (effect == null)
                throw new NullReferenceException("XNA effect cannot be null.");

            Effect = effect;
            Initialize();
        }

        public abstract Shader Clone();

        #region Static Methods
        public static T Load<T>(string filePath)
            where T : Shader, new()
        {
            var effect = TryGetFromCache(filePath);
            var shader = new T();
            shader.Initialize(effect);
            return shader;
        }

        internal static Effect TryGetFromCache(string filePath)
        {
            Effect effect;

            if (!Cache.TryGetValue(filePath, out effect))
            {
                var absolutePath = Path.Combine(Runner.Application.ContentDirectory, filePath);

                try
                {
                    var importer = new EffectImporter();
                    var processor = new EffectProcessor();
                    var pipelineManager = new PipelineManager(string.Empty, string.Empty, string.Empty)
                    {
                        Profile = Runner.Application.Game.Graphics.GraphicsProfile,
                        Platform = TargetPlatform.DesktopGL
                    };

                    var processorContext = new PipelineProcessorContext(pipelineManager, new PipelineBuildEvent());
                    var content = importer.Import(absolutePath, null);
                    var compiledContent = processor.Process(content, processorContext);
                    var graphicsDevice = Runner.Application.Game.GraphicsDevice;

                    effect = new Effect(graphicsDevice, compiledContent.GetEffectCode());
                    Cache.Add(filePath, effect);
                }
                catch
                {
                    effect = null;
                }
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
