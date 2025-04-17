using System;

namespace Potman.Common.ResourceManagament
{
    public interface IContentUpdateProgress : IProgress<float>
    {
        void OnSkip();
        void OnStarted();
        void OnEnded();
    }
}