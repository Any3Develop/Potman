#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Potman.ShowRoom
{
    [RequireComponent(typeof(ShowRoomCameraTarget))]
    public class AnimationTest : MonoBehaviour
    {
        [Tooltip("Увеличивает силу слайдера скорости анимации. Обычно от 0 до 1.00, с множителем можно сделать от 0 до 1.00 * на множитель.")]
        [SerializeField] private float speedMultiplier = 1f;
        [Tooltip("Список Анимаций можно установить в ручную перетаскиванием ссылок, а так же можно воспользоваться контекстым меню ПКМ по скрипту: 'Загрузить анимации из папки исходника' ")]
        [SerializeField] private AnimationClip[] playClips;
        private static AnimatorController[] controllers;
        private static int taken;

        private Animator animator;

        private void Awake()
        {
            animator = GetComponentInChildren<Animator>();
            if (!animator)
                animator = gameObject.AddComponent<Animator>();

            OccupyController();

            if (playClips is null or {Length: 0})
                LoadClipsFromSourceLocation();
        }

        private void Start()
        {
            ShowRoomWindow.Instance.CreateSlider("Speed", name, value => { animator.speed = value * speedMultiplier; });
            foreach (var clip in playClips)
            {
                var clipId = AddAnimationToController(clip);
                Debug.Log($"{clipId} seconds: {clip.length}");
                ShowRoomWindow.Instance.CreateButton(clipId, name, () =>
                {
                    animator.Play(clipId, -1, 0f);
                });
            }
        }

        private void LoadControllersPool()
        {
            if (controllers != null)
                return;
            
            if (!TryGetSourcePath(this, out var path))
                return;

            controllers = FindAssets<AnimatorController>("", path);
        }

        private void OccupyController()
        {
            LoadControllersPool();

            var controller = controllers[taken];
            taken = (taken + 1) % controllers.Length;
            CleanupController(controller);
            animator.runtimeAnimatorController = controller;
        }

        private void CleanupController(AnimatorController controller)
        {
            var defaultLayer = controller.layers[0];
            foreach (var childState in defaultLayer.stateMachine.states.ToArray())
                defaultLayer.stateMachine.RemoveState(childState.state);
        }

        private bool TryGetSourcePath<T>(T obj, out string path) where T : Object
        {
            path = null;
    
            if (obj is MonoBehaviour mono)
            {
                MonoScript script = MonoScript.FromMonoBehaviour(mono);
                path = AssetDatabase.GetAssetPath(script);
            }
            else if (obj is ScriptableObject scriptable)
            {
                MonoScript script = MonoScript.FromScriptableObject(scriptable);
                path = AssetDatabase.GetAssetPath(script);
            }
            else
            {
                var source = PrefabUtility.GetCorrespondingObjectFromSource(obj);
                path = source != null ? AssetDatabase.GetAssetPath(source) : null;
            }

            if (string.IsNullOrEmpty(path))
            {
                Debug.LogError($"Can't load this object: {obj.name} it's source location.");
                return false;
            }

            path = Path.GetDirectoryName(path);
            return true;
        }

        [ContextMenu("Загрузить анимации из папки исходника")]
        private void LoadClipsFromSourceLocation()
        {
            if (!TryGetSourcePath(gameObject, out var path))
                return;
            
            playClips = FindAssets<AnimationClip>(string.Empty, path);
        }

        private string AddAnimationToController(AnimationClip clip)
        {
            if (animator.runtimeAnimatorController is AnimatorController controller)
            {
                var stateMachine = controller.layers[0].stateMachine;
                var newState = stateMachine.AddState(clip.name);
                newState.motion = clip;
                return newState.name;
            }

            return string.Empty;
        }

        private T[] FindAssets<T>(string searchName, params string[] searchFolders) where T : Object
        {
            var hasSuffix = !string.IsNullOrEmpty(searchName);
            var foundAssets = new List<T>();
            var guids = AssetDatabase.FindAssets($"t:{typeof(T).Name}",searchFolders);

            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var assets = AssetDatabase.LoadAllAssetsAtPath(path);

                foreach (var asset in assets)
                {
                    if (asset == null)
                        continue;
                    
                    if (asset is T tAsset && !asset.name.ContainsInvariantCultureIgnoreCase("preview") && (!hasSuffix || asset.name.ContainsInvariantCultureIgnoreCase(searchName)))
                        foundAssets.Add(tAsset);
                }
            }

            return foundAssets.ToArray();
        }

        private void Reset()
        {
            LoadClipsFromSourceLocation();
        }

        private void OnDestroy()
        {
            if (controllers != null)
            {
                foreach (var controller in controllers)
                {
                    CleanupController(controller);
                }

                controllers = null;
                taken = 0;
            }
        }
    }
}
#endif