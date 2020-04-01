using System.IO;
using System.Text;
using System.Xml;

namespace FrogWorks
{
    public static class Memory
    {
        public static bool Save<T>(string filePath, T instance)
            where T : IMemory
        {
            try
            {
                var fullPath = Path.Combine(Runner.Application.ContentDirectory, filePath);

                using (var stream = File.Create(fullPath))
                {
                    instance.WriteToStream(stream);
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        public static bool Load<T>(string filePath, out T instance)
            where T : IMemory, new()
        {
            try
            {
                instance = default;

                var fullPath = Path.Combine(Runner.Application.ContentDirectory, filePath);

                if (File.Exists(fullPath))
                {
                    using (var stream = File.OpenRead(fullPath))
                    {
                        instance = new T();
                        instance.ReadFromStream(stream);
                        return true;
                    }
                }

                return false;
            }
            catch
            {
                instance = default;
                return false;
            }
        }
    }

    public abstract class BinaryMemory : IMemory
    {
        protected Encoding Encoding { get; private set; }

        protected BinaryMemory()
            : this(Encoding.Default)
        {
        }

        protected BinaryMemory(Encoding encoding)
        {
            Encoding = encoding;
        }

        protected abstract void WriteToStream(BinaryWriter writer);

        protected abstract void ReadFromStream(BinaryReader reader);

        public void WriteToStream(Stream stream)
        {
            using (var writer = new BinaryWriter(stream, Encoding))
            {
                WriteToStream(writer);
            }
        }

        public void ReadFromStream(Stream stream)
        {
            using (var reader = new BinaryReader(stream, Encoding))
            {
                ReadFromStream(reader);
            }
        }
    }

    public abstract class XmlMemory : IMemory
    {
        protected XmlWriterSettings WriterSettings { get; private set; }

        protected XmlReaderSettings ReaderSettings { get; private set; }

        protected XmlMemory()
        {
            WriterSettings = new XmlWriterSettings();
            ReaderSettings = new XmlReaderSettings();
        }

        protected abstract void WriteToStream(XmlWriter writer);

        protected abstract void ReadFromStream(XmlReader reader);

        public void WriteToStream(Stream stream)
        {
            using (var writer = XmlWriter.Create(stream, WriterSettings))
            {
                WriteToStream(writer);
            }
        }

        public void ReadFromStream(Stream stream)
        {
            using (var reader = XmlReader.Create(stream, ReaderSettings))
            {
                ReadFromStream(reader);
            }
        }
    }

    public interface IMemory
    {
        void WriteToStream(Stream stream);

        void ReadFromStream(Stream stream);
    }
}
