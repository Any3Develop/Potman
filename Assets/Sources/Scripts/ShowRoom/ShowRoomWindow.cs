#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Potman.ShowRoom
{
    [DefaultExecutionOrder(-100)]
    public class ShowRoomWindow : MonoBehaviour
    {
        [SerializeField] private RectTransform layout;
        [SerializeField] private RectTransform contentPrototype;
        [SerializeField] private Button buttonPrototype;
        [SerializeField] private Slider sliderPrototype;
        public static ShowRoomWindow Instance { get; private set; }
        private readonly Dictionary<string, RectTransform> contents = new();

        private void Awake()
        {
            if (!Instance)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
                return;
            }

            buttonPrototype.gameObject.SetActive(false);
            sliderPrototype.gameObject.SetActive(false);
        }

        public void CreateButton(string label, string contentId, Action click)
        {
            if (!contents.TryGetValue(contentId, out var content))
            {
                contentPrototype.gameObject.SetActive(true);
                contents[contentId] = content = Instantiate(contentPrototype.gameObject, layout).GetComponent<RectTransform>();
                contentPrototype.gameObject.SetActive(false);
            }
            
            buttonPrototype.gameObject.SetActive(true);
            var button = Instantiate(buttonPrototype, content);
            var text = button.GetComponentInChildren<TextMeshProUGUI>();
            
                if (text)
                    text.SetText(label);
                
            button.onClick.AddListener(() => click?.Invoke());
            buttonPrototype.gameObject.SetActive(false);
        }

        public void CreateSlider(string label, string contentId, Action<float> changed)
        {
            if (!contents.TryGetValue(contentId, out var content))
            {
                contentPrototype.gameObject.SetActive(true);
                contents[contentId] = content = Instantiate(contentPrototype.gameObject, layout).GetComponent<RectTransform>();
                contentPrototype.gameObject.SetActive(false);
            }

            sliderPrototype.gameObject.SetActive(true);
            var slider = Instantiate(sliderPrototype, content);
            var sliderText = slider.GetComponentInChildren<TextMeshProUGUI>();
            slider.onValueChanged.AddListener(OnValueChanged);
            sliderPrototype.gameObject.SetActive(false);
            slider.value = 1f;
            
            return;
            void OnValueChanged(float value)
            {
                if (sliderText)
                    sliderText.SetText($"{label} : {value:F2}");
                
                changed?.Invoke(value);
            }
        }
    }
}
#endif