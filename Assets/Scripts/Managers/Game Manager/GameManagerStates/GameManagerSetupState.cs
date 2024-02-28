using System.Collections.Generic;
using UnityEngine;

namespace Synthicate
{
	public class GameManagerSetupState : GameManagerState
	{
		public GameManagerSetupState(GameManager owner) : base(owner) 
		{

		}

		public override void Enter()
		{
			List<uint> buildPoints = _boardManagerSO.getValidSetupPointsFor();
			BuildPermissions playerBuildPermissions = new BuildPermissions(false, true, false);
			_strongholdManagerSO.beginBuildModeForPlayer(0, buildPoints, playerBuildPermissions);
		}
		
		public override void Execute()
		{

		}

		public override void Exit()
		{
			
		}

		public override void OnGUI()
		{
			GUI.Box(UserInterface.s_instructionArea, "");
			GUILayout.BeginArea(UserInterface.s_instructionArea);
			GUILayout.Label("Place a outpost on the board, then place a flyway on the board.");
			GUILayout.EndArea();
		}

	}
}