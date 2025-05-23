﻿using System;
using System.Collections.Generic;
using System.Linq;
using Potman.Common.UIService.Abstractions;
using Potman.Common.UIService.Abstractions.FullFade;
using Potman.Common.UIService.Abstractions.Options;
using Potman.Common.UIService.Data;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Potman.Common.UIService
{
    public class UIDefaultService : IUIService
    {
        private readonly IUIFullFadePresenter fullFade;
        private readonly IUIWindowFactory windowFactory;
        private readonly IUIOptionFactory optionFactory;
        private readonly IUIServiceRepository repository;

        public object DefaultGroupId { get; }
        public IUIRoot UIRoot { get; }

        public UIDefaultService(
            IUIRoot uiRoot,
            IUIFullFadePresenter fullFade,
            IUIWindowFactory windowFactory,
            IUIOptionFactory optionFactory,
            IUIServiceRepository repository,
            object defaultGroupId)
        {
            if (string.IsNullOrWhiteSpace(defaultGroupId?.ToString()))
                throw new ArgumentNullException($"{nameof(defaultGroupId)} cannot be null, empty or white spaces.");
            
            UIRoot = uiRoot;
            this.fullFade = fullFade;
            this.windowFactory = windowFactory;
            this.optionFactory = optionFactory;
            this.repository = repository;
            DefaultGroupId = defaultGroupId;
            
            fullFade.Init(this);
            optionFactory.Init(this);
        }
        
        public void Destroy(IUIWindow window)
        {
	        if (window == null)
		        return;
	        
	        var windowItem = repository.Get<IUIWindow>(window.Id, null);
	        if (windowItem == null)
		        return;

	        repository.Remove(windowItem);
	        DestoryInternal(windowItem);
        }

        public void DestroyAll(object groupId = null)
        {
	        foreach (var windowItem in repository.GetAll<IUIWindow>(groupId?.ToString()).ToArray())
	        {
		        repository.Remove(windowItem);
		        DestoryInternal(windowItem);
	        }
        }

        public T Create<T>(object groupId) where T : IUIWindow
        {
	        var group = DefaultIfEmpty(groupId);
	        var window = windowFactory.Create<T>(group, UIRoot.DeactivatedContainer);
	        
	        repository.Add(new WindowItem(window, group));
	        return window;
        }

        public IEnumerable<IUIWindow> CreateAll(object groupId)
        {
	        var group = DefaultIfEmpty(groupId);
	        var windows = windowFactory.Create(group, UIRoot.DeactivatedContainer);
	        foreach (var window in windows)
		        repository.Add(new WindowItem(window, group));
	        
	        return windows;
        }

        public T Get<T>(string windowId = null, object groupId = null) where T : IUIWindow 
	        => (T)repository.Get<T>(windowId, groupId?.ToString())?.Window;

        public IEnumerable<T> GetAll<T>(object groupId = null) where T : IUIWindow 
	        => repository.GetAll<T>(groupId?.ToString()).Select(x => (T) x.Window).ToArray();

        public bool TryGet<T>(out T result, string windowId = null, object groupId = null) where T : IUIWindow 
	        => (result = Get<T>(windowId, groupId)) != null;

        public T Move<T>(T window, Transform parent = null, int? order = null) where T : IUIWindow
        {
            if (!window?.Root)
                return window;

            if (parent)
				window.Root.SetParent(parent, false);
            
            if (order.HasValue)
                window.Root.SetSiblingIndex(order.Value);
            else
                window.Root.SetAsLastSibling();
            
            return window;
        }

        public IUIOptions<IUIWindow> Begin() => optionFactory.Create<IUIWindow>();
        
        public IUIOptions<T> Begin<T>() where T : IUIWindow => optionFactory.Create<T>(); 
        
        private void DestoryInternal(WindowItem windowItem)
        {
	        if (windowItem?.Window?.Root == null)
		        return;
	        
	        fullFade.OnDeleted(windowItem.Window);
	        windowItem.ActiveTask?.Dispose();
	        windowItem.ActiveTask = null;
	        Object.Destroy(windowItem.Window.Root.gameObject);
        }
        
        private string DefaultIfEmpty(object groupId)
            => groupId?.ToString() ?? DefaultGroupId.ToString();
    }
}