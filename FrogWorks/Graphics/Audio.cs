using Microsoft.Xna.Framework.Audio;
using System.Collections.Generic;
using System.IO;

namespace FrogWorks
{
    public sealed class Audio
    {
        private static Dictionary<string, SoundEffect> Cache { get; } = new Dictionary<string, SoundEffect>();

        private SoundEffect XnaSoundEffect { get; set; }

        public Audio(SoundEffect xnaSoundEffect)
        {
            XnaSoundEffect = xnaSoundEffect;
        }

        #region Static Methods
        public static Audio Load(string filePath)
        {
            var xnaSoundEffect = TryGetFromCache(filePath);
            return new Audio(xnaSoundEffect);
        }

        internal static SoundEffect TryGetFromCache(string filePath)
        {
            SoundEffect xnaSoundEffect;

            if (!Cache.TryGetValue(filePath, out xnaSoundEffect))
            {
                var absolutePath = Path.Combine(Runner.Application.ContentDirectory, filePath);

                using (var stream = File.OpenRead(absolutePath))
                {
                    xnaSoundEffect = SoundEffect.FromStream(stream);
                    Cache.Add(filePath, xnaSoundEffect);
                }
            }

            return xnaSoundEffect;
        }

        internal static void DisposeCache()
        {
            foreach (var xnaSoundEffect in Cache.Values)
                xnaSoundEffect.Dispose();

            Cache.Clear();
        }
        #endregion
    }
}
