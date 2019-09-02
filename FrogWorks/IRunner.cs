using System;

namespace FrogWorks
{
    public interface IRunner : IDisposable
    {
        void Run();

        void RunOnce();
    }
}
