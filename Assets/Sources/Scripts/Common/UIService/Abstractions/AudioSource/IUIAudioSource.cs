using System;
using System.Collections.Generic;
using Potman.Common.UIService.Data;

namespace Potman.Common.UIService.Abstractions.AudioSource
{
    public interface IUIAudioSource : IDisposable
    {
        event Action<UIAudioClipData> OnPlayAudio; 
        IEnumerable<IUIAudioHandler> Handlers { get; }
        bool Enabled { get; }
        
        void Enable();
        void Disable();
    }
}