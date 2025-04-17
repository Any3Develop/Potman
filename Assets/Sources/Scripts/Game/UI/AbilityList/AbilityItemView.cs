using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Potman.Game.UI.AbilityList
{
    public class AbilityItemView : MonoBehaviour
    {
        [field: SerializeField] public TextMeshProUGUI Label { get; private set; }
        [field: SerializeField] public Button Button { get; private set; }
        [field: SerializeField] public RectTransform Layout { get; private set; }

        public void SetSelected(bool value)
        {
            Layout.DOKill();
            Layout.DOAnchorPosY(value ? 50 : 0, 0.25f);
        }

        private void OnDestroy()
        {
            if (Layout)
                Layout.DOKill();
        }

        public void SetActive(bool value)
        {
            gameObject.SetActive(value);
        }
    }
}