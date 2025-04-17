using System;
using System.Text.RegularExpressions;
using Potman.Common.UIService;
using R3;
using TMPro;
using UnityEngine;

namespace Potman.Game.UI.AbilityList
{
    public class AbilityListWindow : UIWindowBase
    {
        [SerializeField] private AbilityItemView prototype;
        [SerializeField] private TextMeshProUGUI inputLeftText;
        [SerializeField] private TextMeshProUGUI inputRightText;
        
        private IAbilityListViewModel viewModel;
        private IDisposable binds;
        
        protected override void OnInit()
        {
            base.OnInit();
            prototype.gameObject.SetActive(false);
        }
        
        public void Bind(IAbilityListViewModel model)
        {
            Clear();
            viewModel = model;
            
            if (viewModel == null)
                return;

            using var builder = new DisposableBuilder();
            prototype.SetActive(true);
            foreach (var ability in viewModel.Datas)
            {
                var itemView = Instantiate(prototype, Content);
                builder.Add(viewModel.Current.Subscribe(x => itemView.SetSelected(x == ability)));
                itemView.Button.onClick.AddListener(() => viewModel.Current.OnNext(ability));
                itemView.Label.SetText(Regex.Replace(ability.Config.id.ToString(), "(?<!^)([A-Z][a-z]|\\d)", " $1").Replace("Battle ", ""));
                itemView.SetSelected(viewModel.Current.Value == ability);
            }
            prototype.SetActive(false);
            binds = builder.Build();
        }
        
        public override void Hidden()
        {
            base.Hidden();
            Clear();
        }

        public void Clear()
        {
            binds?.Dispose();
            binds = null;
            
            foreach (Transform child in Content) 
                if (inputLeftText.transform != child && inputRightText.transform != child) 
                    Destroy(child.gameObject);

            viewModel = null;
        }
    }
}