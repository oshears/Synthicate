using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace Synthicate
{
	public class GameManagerSetupState : GameManagerAbstractState
	{

		public override void Enter()
		{
			_strongholdManagerSO.playerBuildEvent.AddListener(PlayerBuildStrongholdEventHandler);	
			_flywayManagerSO.playerBuildEvent.AddListener(PlayerBuildFlywayEventHandler);
			
			List<uint> buildPoints = _boardManagerSO.getValidSetupPointsFor();
			BuildPermissions playerBuildPermissions = new BuildPermissions(false, true, false);
			_strongholdManagerSO.beginBuildModeForPlayer(_clientPlayer.GetId(), buildPoints, playerBuildPermissions);
		
			_userInterfaceSO.OnInitializeUserInterface();
			_userInterfaceSO.OnSetGameMenuActive(true);
			_userInterfaceSO.OnUpdateUserInterface();
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
			if (!IsActiveState()) return;
			
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
				OnPlayerBuiltOutpostServerRpc();
				

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
		
		[ServerRpc(RequireOwnership = false)]
		public void OnPlayerBuiltOutpostServerRpc() => OnPlayerBuiltOutpostClientRpc();
		[ClientRpc]
		public void OnPlayerBuiltOutpostClientRpc()
		{
			_gameManagerSO.OnPlayerBuiltOutpost();
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
				OnPlayerBuiltFlywayServerRpc();
			}
			
			if (_gameManagerSO.DoneSetupPhase())
			{
				// changeState(_owner.diceState);
				Debug.Log("Done with Setup Phase!");
			}
			else
			{
				_owner.pendingSetupState.NextPlayerSetupClientRpc(_gameManagerSO.IncrementAndGetNextPlayerIndex());
				changeState(_owner.pendingSetupState);
			}
		}
		
		[ServerRpc(RequireOwnership = false)]
		public void OnPlayerBuiltFlywayServerRpc() => OnPlayerBuiltOutpostClientRpc();
		[ClientRpc]
		public void OnPlayerBuiltFlywayClientRpc()
		{
			_gameManagerSO.OnPlayerBuiltFlyway();
		}

	}
}