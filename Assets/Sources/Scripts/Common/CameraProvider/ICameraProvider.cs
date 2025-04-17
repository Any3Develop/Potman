using UnityEngine;

namespace Potman.Common.CameraProvider
{
    public interface ICameraProvider
    {
        Camera Camera { get; }
        void Overlay(Camera other);
    }
}