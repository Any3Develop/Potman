using Potman.Common.DependencyInjection;
using Potman.Common.UIService.Abstractions;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Potman.Common.CameraProvider
{
    public class GlobalCameraProvider : ICameraProvider // TODO organize
    {
        public Camera Camera { get;}
        private readonly UniversalAdditionalCameraData universalData;

        public GlobalCameraProvider(IUIRoot uiRoot, IAbstractFactory abstractFactory)
        {
            Camera = abstractFactory.CreateUnityObject<Camera>(Resources.Load("Common/GlobalCamera"));
            Camera.TryGetComponent(out universalData);
            Overlay(uiRoot.UICamera);
        }

        public void Overlay(Camera other)
        {
            universalData.cameraStack.Add(other);
            Debug.Log("Successful setup overlay camera.");
        }
    }
}