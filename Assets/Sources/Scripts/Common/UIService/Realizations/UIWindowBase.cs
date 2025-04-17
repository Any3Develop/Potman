using System;
using System.Linq;
using Potman.Common.UIService.Abstractions;
using Potman.Common.UIService.Abstractions.AnimationSource;
using Potman.Common.UIService.Abstractions.AudioSource;
using UnityEngine;

namespace Potman.Common.UIService
{
    public abstract class UIWindowBase : MonoBehaviour, IUIWindow
    {
        [SerializeField] protected RectTransform root;
        [SerializeField] protected RectTransform content;
        protected bool Initialzied { get; private set; }

        public string Id { get; private set; }
        public event Action OnChanged;
        public IUIAnimationSource AnimationSource { get; private set; }
        public IUIAudioSource AudioSource { get; private set; }
        public RectTransform Root => root;
        public RectTransform Content => content;

        internal UIWindowBase Init(
	        string id,
            IUIAnimationSource animationSource,
            IUIAudioSource audioSource)
        {
	        Id = id;
            AnimationSource = animationSource;
            AudioSource = audioSource;
            Initialzied = true;

            OnFixContainers();
            OnInit();
            return this;
        }

        public virtual void Show() {}
        
        public virtual void Showed() {}

        public virtual void Hide() {}

        public virtual void Hidden() {}

        protected virtual void OnInit() {}

        protected virtual void OnDisposed() {}

        protected virtual void OnFixContainers()
        {
            if (!root)
                root = GetComponent<RectTransform>();

            if (!content && root)
                content = root.GetComponentsInChildren<RectTransform>(true)
                    .FirstOrDefault(x => x && x.name.ToLower().Contains("content")
                                         || x.name.ToLower().Contains("container")) ?? root;
        }

        protected void OnWindowChanged() => OnChanged?.Invoke();

        private void OnValidate() => OnFixContainers();

        protected void OnDestroy()
        {
            if (!Initialzied)
                return;

            OnDisposed();
            Initialzied = false;
            AnimationSource = null;
            AudioSource = null;
            content = null;
            OnChanged = null;
            root = null;
        }

        /// <summary>
        /// Instead, use <see cref="OnInit()"/>
        /// </summary>
        protected void Awake() {}

        /// <summary>
        /// Instead, use <see cref="OnInit()"/>
        /// </summary>
        protected void Start() {}

#if UNITY_EDITOR
	    
        [ContextMenu(nameof(AutoAssignFields))]
        private void AutoAssignFields()
        {
	        AutoComponentAssigner.AutoAssignComponents(this);
        }

        [ContextMenu(nameof(ClearAssignedFields))]
        private void ClearAssignedFields()
        {
	        AutoComponentAssigner.ClearAssignedFields(this);
        }
#endif
    }
}