namespace Potman.Common.UIService.Abstractions.AudioSource
{
    public interface IUIAudioSourceFactory
    {
        IUIAudioSource Create(IUIWindow window);
    }
}