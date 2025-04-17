using Potman.Game.Context.Data.Spawn;
using UnityEngine;

namespace Potman.Game.Context.Spawn
{
    public class SpawnPoint : MonoBehaviour
    {
        [SerializeField] protected SpawnId id = SpawnId.WaveZone0;
        [SerializeField] protected Vector3 size = Vector3.one;
        [SerializeField] protected Transform root;

        public virtual SpawnId Id => id;
        public virtual Vector3 Bounds => root.localScale + size;
        public virtual Vector3 Position => root.position;

        public void SetParent(Transform other) => root.SetParent(other, true);

#if UNITY_EDITOR

        private void OnValidate()
        {
            if (!root)
                root = transform;
        }

        private void OnDrawGizmosSelected()
        {
            if (!root)
                return;
            
            var originalMatricx = Gizmos.matrix;
            Gizmos.color = Color.cyan;
            Gizmos.matrix = root.localToWorldMatrix;
            Gizmos.DrawWireCube(Vector3.zero, size);
            Gizmos.matrix = originalMatricx;
        }
#endif
    }
}