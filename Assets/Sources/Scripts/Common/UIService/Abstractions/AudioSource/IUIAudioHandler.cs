using System;
using Potman.Common.UIService.Data;

namespace Potman.Common.UIService.Abstractions.AudioSource
{
    public interface IUIAudioHandler : IDisposable
    {
        event Action<UIAudioClipData> OnPayAudio;
        bool Enabled { get; }
        void Enable();
        void Disable();
    }
}