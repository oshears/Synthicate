using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Synthicate
{
	public class GameManagerBuildingState : GameManagerAbstractState
	{
		
		public GameManagerBuildingState(GameManager owner) : base(owner) 
		{
			_strongholdManagerSO.playerBuildEvent.AddListener(PlayerBuildStrongholdEventHandler);	
			_flywayManagerSO.playerBuildEvent.AddListener(PlayerBuildFlywayEventHandler);
		}

		public override void Enter()
		{
			BuildPermissions playerBuildPermissions = new BuildPermissions(_clientPlayer.canBuildFlyway(), _clientPlayer.canBuildOutpost(), _clientPlayer.canBuildStronghold());
			_strongholdManagerSO.beginBuildModeForPlayer(_clientPlayer.GetId(), _boardManagerSO.getValidPointsFor(_clientPlayer.GetId()), playerBuildPermissions);
			_flywayManagerSO.beginBuildModeForPlayer(_clientPlayer.GetId(), _boardManagerSO.getValidEdgesFor(_clientPlayer.GetId()), playerBuildPermissions);
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
		
		void PlayerBuildStrongholdEventHandler(bool validBuild)
		{
			_strongholdManagerSO.endBuildMode();
			if (validBuild)
			{
				_gameManagerSO.playerBuildEvent.Invoke();
				_strongholdManagerSO.pointUpdateRequest.Invoke();
				GameEvent gameEvent = new GameEvent(GameEventType.Build, _gameManagerSO.clientPlayer + " has built a new stronghold or outpost!");
				_gameManagerSO.playerEvent.Invoke(gameEvent);

				changeState(new GameManagerIdleState(_owner));
			}
		}
		
		void PlayerBuildFlywayEventHandler(bool validBuild)
		{
			_flywayManagerSO.endBuildMode();
			if (validBuild)
			{
				_gameManagerSO.playerBuildEvent.Invoke();
				_flywayManagerSO.edgeUpdateRequest.Invoke();
				GameEvent gameEvent = new GameEvent(GameEventType.Build, _gameManagerSO.clientPlayer + " has built a new flyway!");
				_gameManagerSO.playerEvent.Invoke(gameEvent);
			}
		}
		
		

	}
}