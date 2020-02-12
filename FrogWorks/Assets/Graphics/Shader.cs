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
            var effect = AssetManager.GetFromCache(filePath, ReadAndCompile);

            if (effect != null)
            {
                var shader = new T();
                shader.Initialize(effect);
                return shader;
            }

            return null;
        }

        internal static Effect ReadAndCompile(string filePath)
        {
            var fullPath = AssetManager.GetFullPath(filePath, ".fx");

            if (!string.IsNullOrEmpty(fullPath))
            {
                var importer = new EffectImporter();
                var processor = new EffectProcessor();
                var pipelineManager = new PipelineManager(string.Empty, string.Empty, string.Empty)
                {
                    Profile = Runner.Application.Game.Graphics.GraphicsProfile,
                    Platform = TargetPlatform.DesktopGL
                };

                var processorContext = new PipelineProcessorContext(pipelineManager, new PipelineBuildEvent());
                var content = importer.Import(fullPath, null);
                var compiledContent = processor.Process(content, processorContext);
                var graphicsDevice = Runner.Application.Game.GraphicsDevice;

                return new Effect(graphicsDevice, compiledContent.GetEffectCode());
            }

            return null;
        }
        #endregion
    }
}
