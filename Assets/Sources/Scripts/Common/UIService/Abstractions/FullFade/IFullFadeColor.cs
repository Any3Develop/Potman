using UnityEngine;

namespace Potman.Common.UIService.Abstractions.FullFade
{
    public interface IFullFadeColor : IFullFadeTarget
    {
        Color FadeColor { get; }
    }
}