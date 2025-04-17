using System.Collections.Generic;
using System.Linq;
using Potman.Common.InputSystem;
using Potman.Common.InputSystem.Abstractions;
using Potman.Common.InputSystem.DragDrop;
using Potman.Common.InputSystem.Hovering;
using Potman.Common.InputSystem.Selection;
using Potman.Game.Entities.Player.Data;
using UnityEngine.InputSystem;
using Zenject;

namespace Potman.Infrastructure.Common.InputSystem
{
	public class InputSystemInstaller : Installer<InputSystemInstaller>
	{
		public override void InstallBindings()
		{
			BindInput();
			BindSystems();
		}

		private void BindSystems()
		{
			Container
				.BindInterfacesTo<SelectionSystem>()
				.AsSingle();
				
			Container
				.BindInterfacesTo<HoverSystem>()
				.AsSingle();
			
			Container
				.BindInterfacesTo<DragDropSystem>()
				.AsSingle();

			Container
				.BindInterfacesTo<DragDropInputHandler>()
				.AsSingle();
		}

		private void BindInput()
		{
			var contextProcessor = new InputContextProcessor();
			var gamePlayInput = new InputActions();
			gamePlayInput.Enable(); // before binding input actions
			
			Container
				.Bind<InputActions>()
				.FromInstance(gamePlayInput)
				.AsSingle()
				.NonLazy();

			Container
				.BindInterfacesTo<InputContextProcessor>()
				.FromInstance(contextProcessor)
				.AsSingle()
				.NonLazy();

			BindInputController<DragDropActions>(gamePlayInput.DragDropActions.Get(), contextProcessor);
			BindInputController<HoverActions>(gamePlayInput.HoverActions.Get(), contextProcessor);
			BindInputController<SelectionActions>(gamePlayInput.SelectionActions.Get(), contextProcessor);
			BindInputController<PlayerActions>(gamePlayInput.Player.Get(), contextProcessor);
		}

		private void BindInputController<TAction>(InputActionMap actionMap, IInputContextProcessor contextProcessor)
		{
			// Generics in assembly with IL2CPP - runtime error, deterministic generation of Generics required.
			// https://forum.unity.com/threads/is-there-any-limitations-to-deserializing-json-on-webgl.1250356/#post-7951618
			var args = MapInputActions(actionMap, contextProcessor);
			var inputController = new InputController<TAction>(args);
			Container
				.Bind<IInputController<TAction>>()
				.FromInstance(inputController)
				.AsSingle()
				.NonLazy();
		}

		private static IEnumerable<IInputAction> MapInputActions(InputActionMap actionMap, IInputContextProcessor contextProcessor)
		{
			return actionMap.actions.Select(inputAction => new InputSystemInputAction(inputAction, contextProcessor));
		}
	}
}