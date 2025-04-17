using System;
using System.Linq;
using Potman.Common.UIService.Abstractions;
using Potman.Common.UIService.Abstractions.AnimationSource;

namespace Potman.Common.UIService.AnimationSource.Factories
{
    public class UIAnimationSourceFactory : IUIAnimationSourceFactory
    {
        protected readonly IUIAnimationsFactory AnimationsFactory;
        
        public UIAnimationSourceFactory(IUIAnimationsFactory animationsFactory) 
            => AnimationsFactory = animationsFactory;

        public IUIAnimationSource Create(IUIWindow window)
        {
            if (window == null)
                throw new NullReferenceException($"{nameof(IUIWindow)} missing window.");

            var configProvider = window.Root.GetComponentInChildren<IUIAnimationConfigsProvider>(true);
            if (configProvider == null)
                return new UINoAnimationSource();
            
            var animations = configProvider.LoadAll().Select(config => AnimationsFactory.Create(window, config)).ToArray();
            return new UIAnimationSource().Init(window, animations);
        }
    }
}