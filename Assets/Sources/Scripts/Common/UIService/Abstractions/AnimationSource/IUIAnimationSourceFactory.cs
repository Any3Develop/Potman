namespace Potman.Common.UIService.Abstractions.AnimationSource
{
    public interface IUIAnimationSourceFactory
    {
        IUIAnimationSource Create(IUIWindow window);
    }
}