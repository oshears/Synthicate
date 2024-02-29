using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Synthicate
{
	public class GameManagerHackingState : GameManagerAbstractState
	{
		
		public GameManagerHackingState(GameManager owner) : base(owner) 
		{
			_hexManagerSO.hackEvent.AddListener(HackEventHandler);
		}

		public override void Enter()
		{
			// _clientPlayer.useHacker();
			_hexManagerSO.beginHackModeEvent.Invoke();
		}
		
		public override void Execute()
		{

		}

		public override void Exit()
		{


		}

		public override void OnGUI()
		{
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
			changeState(new GameManagerIdleState(_owner));
		}
		

	}
}