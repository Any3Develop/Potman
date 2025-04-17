﻿using System.Collections.Generic;
using System.Linq;
using Potman.Common.UIService.AudioSource.Configs;
using UnityEngine.UI;

namespace Potman.Common.UIService.AudioSource.Handlers
{
    public class UIDropDownAudioHandler : UIAudioHandlerBase<UIDropDownAudioConfig>
    {
        protected List<Dropdown> ListenComponents = new();
        
        protected override void OnInit() => GetComponents(ref ListenComponents);
        
        protected override void OnDisposed()
        {
            if (ListenComponents != null)
                OnDisabled();
            
            ListenComponents = null;
        }

        protected override void OnEnabled()
        {
            foreach (var component in ListenComponents.Where(component => component))
                component.onValueChanged.AddListener(OnValueChanged);
        }

        protected override void OnDisabled()
        {
            foreach (var component in ListenComponents.Where(component => component))
                component.onValueChanged.RemoveListener(OnValueChanged);
        }

        protected virtual void OnValueChanged(int value)
        {
            if (!Initialized || !Enabled)
                return;

            PlayAudioClip(Config.SelectAudio);
        }

        protected virtual void GetComponents<T>(ref List<T> components)
        {
            components ??= new List<T>();
            if (Config.IncludeInherited)
            {
                components.AddRange(Window.Content.GetComponentsInChildren<T>(Config.IncludeDisabled));
                return;
            }

            var specifiedType = typeof(T);  // to ensure the components are not inherited.
            components.AddRange(Window.Content
	            .GetComponentsInChildren<T>(Config.IncludeDisabled)
	            .Where(x => x.GetType() == specifiedType));
        }
    }
}