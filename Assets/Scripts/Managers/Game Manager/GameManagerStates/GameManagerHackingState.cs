using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Synthicate
{
	public class GameManagerHackingState : GameManagerAbstractState
	{
		
		public override void Enter()
		{
			_hexManagerSO.hackEvent.AddListener(HackEventHandler);
			// _gameManagerSO.clientPlayer.useHacker();
			_hexManagerSO.beginHackModeEvent.Invoke();
		}
		
		public override void Execute()
		{

		}

		public override void Exit()
		{
			_hexManagerSO.hackEvent.RemoveListener(HackEventHandler);
		}

		public override void OnGUI()
		{
			if (!IsActiveState()) return;
			
			// GUI.Box(UserInterface.s_gameMenuArea, "");

			// GUILayout.BeginArea(UserInterface.s_gameMenuArea);
			// if(GUILayout.Button("Exit Build Mode"))
			// {
			// 	changeState(new GameManagerIdleState(_owner));
			// }
			// GUILayout.EndArea();
		}
		
		void HackEventHandler(uint id)
		{
			_hexManagerSO.endHackModeEvent.Invoke();
			changeState(_owner.idleState);
		}
		

	}
}