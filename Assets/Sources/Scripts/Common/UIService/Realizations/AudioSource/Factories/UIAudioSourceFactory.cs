using System;
using System.Linq;
using Potman.Common.UIService.Abstractions;
using Potman.Common.UIService.Abstractions.AudioSource;

namespace Potman.Common.UIService.AudioSource.Factories
{
    public class UIAudioSourceFactory : IUIAudioSourceFactory
    {
        protected readonly IUIAudioHandlersFactory AudioHandlersFactory;
        
        public UIAudioSourceFactory(IUIAudioHandlersFactory audioHandlersFactory) 
            => AudioHandlersFactory = audioHandlersFactory;

        public virtual IUIAudioSource Create(IUIWindow window)
        {
            if (window == null)
                throw new NullReferenceException($"{nameof(IUIWindow)} missing window.");

            var configProvider = window.Root.GetComponentInChildren<IUIAudioConfigsProvider>(true);
            if (configProvider == null)
                return new UINoAudioSource();
            
            var handlers = configProvider.LoadAll().Select(config => AudioHandlersFactory.Create(window, config));

            return new UIAudioSource().Init(window, handlers);
        }
    }
}