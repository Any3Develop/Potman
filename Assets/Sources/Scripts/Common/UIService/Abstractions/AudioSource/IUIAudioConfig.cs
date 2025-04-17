namespace Potman.Common.UIService.Abstractions.AudioSource
{
    public interface IUIAudioConfig
    {
        public bool EnabledByDefault { get; }
        public bool ReInitWhenModified { get; }
    }
}