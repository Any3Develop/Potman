using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace Potman.Common.InputSystem.Utils
{
    public class InputHelper
    {
        #region Usage

        /// <summary>
        /// Raycast at the pointer position, looks for the first object that has raycast enabled and compares them to the target object.
        /// </summary>
        /// <param name="target">Reference to the object to check under the pointer</param>
        /// <returns></returns>
        public static bool IsPointerOver(GameObject target)
        {
            var focused = FirstObjectUnderPointer();
            if (!focused)
                return false;

            return focused == target || focused.transform.IsChildOf(target.transform);
        }

        /// <summary>
        /// Raycast at pointer position, looking for the first object that has raycast enabled.
        /// </summary>
        /// <returns>First found object under pointer</returns>
        public static GameObject FirstObjectUnderPointer()
        {
            try
            {
                return RaycastUnderPointer().gameObject;
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                return default;
            }
        }

        #endregion


        #region InputSystem

        /// <summary>
        /// Get device id currently used as pointer
        /// </summary>
        /// <returns>Device ID currently used as pointer</returns>
        public static int GetPointerId()
        {
            return Pointer.current.device.deviceId;
        }

        /// <summary>
        /// The position of the screen below the switch device from some input system.
        /// </summary>
        /// <returns>The position of the pointer in screen space.</returns>
        public static Vector2 GetPositionUnderPointer()
        {
            return Pointer.current.position.ReadValue();
        }

        #endregion


        #region Raycast

        private static readonly List<RaycastResult> CachedRaycastResults = new();
        private static PointerEventData cachedPointerEventData;
        private static int snapshotRaycastFrame;
        
        /// <summary>
        /// Raycast under the pointer, using the desired type of input system.
        /// </summary>
        /// <returns>Current raycast result</returns>
        public static RaycastResult RaycastUnderPointer()
        {
            if (EventSystem.current == null)
            {
                Debug.LogError("EventSystem not initialized");
                return default;
            }

            // raycast once per frame
            if (snapshotRaycastFrame == Time.frameCount)
                return cachedPointerEventData.pointerCurrentRaycast;

            if (cachedPointerEventData == null)
                cachedPointerEventData = new PointerEventData(EventSystem.current);

            snapshotRaycastFrame = Time.frameCount;
            cachedPointerEventData.position = GetPositionUnderPointer();
            EventSystem.current.RaycastAll(cachedPointerEventData, CachedRaycastResults);
            cachedPointerEventData.pointerCurrentRaycast = CachedRaycastResults.FirstOrDefault(raycast => raycast.gameObject != null);
            CachedRaycastResults.Clear();

            return cachedPointerEventData.pointerCurrentRaycast;
        }

        #endregion
    }
}