using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Synthicate
{
	public class GameManagerBuildingState : GameManagerAbstractState
	{
		
		public GameManagerBuildingState(GameManager owner) : base(owner) 
		{

		}

		public override void Enter()
		{
			BuildPermissions playerBuildPermissions = new BuildPermissions(_clientPlayer.canBuildFlyway(), _clientPlayer.canBuildOutpost(), _clientPlayer.canBuildStronghold());
			_strongholdManagerSO.beginBuildModeForPlayer(_clientPlayer.getId(), _boardManagerSO.getValidPointsFor(_clientPlayer.getId()), playerBuildPermissions);
			_flywayManagerSO.beginBuildModeForPlayer(_clientPlayer.getId(), _boardManagerSO.getValidEdgesFor(_clientPlayer.getId()), playerBuildPermissions);
		}
		
		public override void Execute()
		{

		}

		public override void Exit()
		{
			_strongholdManagerSO.endBuildMode();
			_flywayManagerSO.endBuildMode();
		}

		public override void OnGUI()
		{
			GUI.Box(UserInterface.s_gameMenuArea, "");

			GUILayout.BeginArea(UserInterface.s_gameMenuArea);
			if(GUILayout.Button("Exit Build Mode"))
			{
				changeState(new GameManagerIdleState(_owner));
			}
			GUILayout.EndArea();
		}
		
		

	}
}