using System.Collections.Generic;
using UnityEngine;

namespace Synthicate
{
	public class GameManagerSetupState : GameManagerAbstractState
	{
		public GameManagerSetupState(GameManager owner) : base(owner) 
		{
			_strongholdManagerSO.playerBuildEvent.AddListener(PlayerBuildStrongholdEventHandler);	
			_flywayManagerSO.playerBuildEvent.AddListener(PlayerBuildFlywayEventHandler);
		}

		public override void Enter()
		{
			List<uint> buildPoints = _boardManagerSO.getValidSetupPointsFor();
			BuildPermissions playerBuildPermissions = new BuildPermissions(false, true, false);
			_strongholdManagerSO.beginBuildModeForPlayer(_clientPlayer.GetId(), buildPoints, playerBuildPermissions);
		
			_userInterfaceSO.OnSetGameMenuActive(true);
		}
		
		public override void Execute()
		{

		}

		public override void Exit()
		{
			_strongholdManagerSO.playerBuildEvent.RemoveListener(PlayerBuildStrongholdEventHandler);	
			_flywayManagerSO.playerBuildEvent.RemoveListener(PlayerBuildFlywayEventHandler);
			_hexManagerSO.setupResourceRequest.Invoke();
		}

		public override void OnGUI()
		{
			GUI.Box(UserInterface.s_instructionArea, "");
			GUILayout.BeginArea(UserInterface.s_instructionArea);
			GUILayout.Label("Place a outpost on the board, then place a flyway on the board.");
			GUILayout.EndArea();
		}
		
		public void PlayerBuildStrongholdEventHandler(bool validBuild)
		{
			_strongholdManagerSO.endBuildMode();
			if (validBuild)
			{
				_gameManagerSO.playerBuildEvent.Invoke();
				_strongholdManagerSO.pointUpdateRequest.Invoke();
				GameEvent gameEvent = new GameEvent(GameEventType.Build, _gameManagerSO.clientPlayer + " has built a new stronghold or outpost!");
				_gameManagerSO.playerEvent.Invoke(gameEvent);

				// if (!inSetupPhase()) getCurrentPlayer().buildStronghold();
				//_gameManagerSO.clientPlayer.buildStronghold();

				// if in setup mode, let the client build a flyway now
				// if (isClientPlayerSetupActive()) beginClientSetupFlywayBuildMode();
			}
			// else if (clientPlayerIsActive())
			// {
			// 	GameEvent gameEvent = new GameEvent(GameEventType.Build, "You do not have enough resources to build an outpost or stronghold here!");
			// 	playerEvent.Invoke(gameEvent);
			// }
			
			List<uint> buildEdges = _boardManagerSO.getValidEdgesFor(_clientPlayer.GetId());
			BuildPermissions playerBuildPermissions = new BuildPermissions(true, false, false);
			_flywayManagerSO.beginBuildModeForPlayer(_clientPlayer.GetId(), buildEdges, playerBuildPermissions);
		}
		
		public void PlayerBuildFlywayEventHandler(bool validBuild)
		{
			_flywayManagerSO.endBuildMode();
			if (validBuild)
			{
				_gameManagerSO.playerBuildEvent.Invoke();
				_flywayManagerSO.edgeUpdateRequest.Invoke();
				GameEvent gameEvent = new GameEvent(GameEventType.Build, _gameManagerSO.clientPlayer + " has built a new flyway!");
				_gameManagerSO.playerEvent.Invoke(gameEvent);
			}
			
			changeState(new GameManagerDiceState(_owner));
		}

	}
}