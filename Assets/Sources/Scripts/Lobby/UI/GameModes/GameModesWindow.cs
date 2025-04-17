using Potman.Common.UIService;
using UnityEngine;

namespace Potman.Lobby.UI.GameModes
{
    public class GameModesWindow : UIWindowBase
    {
        [SerializeField] private ModeItemView prototype;
        private IGameModesViewModel viewModel;

        protected override void OnInit()
        {
            base.OnInit();
            prototype.gameObject.SetActive(false);
        }

        public void Bind(IGameModesViewModel model)
        {
            Clear();
            viewModel = model;
            
            if (viewModel == null)
                return;
            
            foreach (var data in viewModel.Datas)
            {
                var itemView = Instantiate(prototype, Content);
                itemView.Name.text = data.Name;
                itemView.Button.onClick.AddListener(() => viewModel.Current.OnNext(data));
                itemView.gameObject.SetActive(true);
            }
        }

        public override void Hidden()
        {
            base.Hidden();
            Clear();
        }

        public void Clear()
        {
            foreach (Transform child in Content)
                Destroy(child.gameObject);

            viewModel = null;
        }
    }
}