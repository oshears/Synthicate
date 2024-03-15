using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace Synthicate
{
	public class GameManagerSetupState : GameManagerAbstractState
	{

		[Header("Event Channels")]
		
		[SerializeField]
		GameMenuStateEventChannel m_GameMenuStateEventChannel;
		
		[SerializeField]
		StringEventChannel m_NotificationEventChannel;
		
		[SerializeField]
		BoolEventChannel m_EnablePlayerControllerEventChannel;
		
		public override void Enter()
		{
			Debug.Log($"Entering Setup State with Player: {_gameManagerSO.currentPlayerTurn}");
			
			_strongholdManagerSO.playerBuildEvent.AddListener(PlayerBuildStrongholdEventHandler);	
			_flywayManagerSO.playerBuildEvent.AddListener(PlayerBuildFlywayEventHandler);
			
			// Configure outpost building spots
			List<uint> buildPoints = _boardManagerSO.GetValidSetupPointsFor();
			BuildPermissions playerBuildPermissions = new BuildPermissions(false, true, false);
			_strongholdManagerSO.beginBuildModeForPlayer(_gameManagerSO.clientPlayer.GetId(), buildPoints, playerBuildPermissions);

			// Setup UI
			_userInterfaceSO.OnInitializeUserInterface();
			m_GameMenuStateEventChannel.RaiseEvent(GameMenu.Screens.PlayerSetupTurnScreen);
			_userInterfaceSO.OnUpdateUserInterface();
			
			// Enable player panning
			m_EnablePlayerControllerEventChannel.RaiseEvent(true);
			
			// Send notification to the UI
			m_NotificationEventChannel.RaiseEvent($"{_gameManagerSO.GetCurrentPlayer().GetName()} is setting up!");
			
			// 
			_owner.hexManagerSO.managerSetupResourceResponse.AddListener(SetupResourceResponseEventHandler);

		}
		
		public override void Execute()
		{

		}

		public override void Exit()
		{
			_strongholdManagerSO.playerBuildEvent.RemoveListener(PlayerBuildStrongholdEventHandler);	
			_flywayManagerSO.playerBuildEvent.RemoveListener(PlayerBuildFlywayEventHandler);
			_owner.hexManagerSO.managerSetupResourceResponse.RemoveListener(SetupResourceResponseEventHandler);
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
				// GameEvent gameEvent = new GameEvent(GameEventType.Build, _gameManagerSO.clientPlayer + " has built a new stronghold or outpost!");
				// _gameManagerSO.playerEvent.Invoke(gameEvent);
				OnUpdatePlayerBuildCountsServerRpc();
				
				// TODO: Maybe the actual flyway can raise this event (avoid the middle man)
				m_NotificationEventChannel.RaiseEvent("Player built an outpost!");

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
			
			List<uint> buildEdges = _boardManagerSO.GetValidEdgesFor(_gameManagerSO.clientPlayer.GetId());
			BuildPermissions playerBuildPermissions = new BuildPermissions(true, false, false);
			_flywayManagerSO.beginBuildModeForPlayer(_gameManagerSO.clientPlayer.GetId(), buildEdges, playerBuildPermissions);
		}
		
		[ServerRpc(RequireOwnership = false)]
		public void OnUpdatePlayerBuildCountsServerRpc() => OnUpdatePlayerBuildCountsClientRpc();
		[ClientRpc]
		public void OnUpdatePlayerBuildCountsClientRpc()
		{
			_gameManagerSO.UpdatePlayerBuildCounts();
		}
		
		public void PlayerBuildFlywayEventHandler(bool validBuild)
		{
			_flywayManagerSO.endBuildMode();
			if (validBuild)
			{
				_gameManagerSO.playerBuildEvent.Invoke();
				_flywayManagerSO.edgeUpdateRequest.Invoke();
				// GameEvent gameEvent = new GameEvent(GameEventType.Build, _gameManagerSO.clientPlayer + " has built a new flyway!");
				// _gameManagerSO.playerEvent.Invoke(gameEvent);
				OnUpdatePlayerBuildCountsServerRpc();
				
				// TODO: Maybe the actual flyway can raise this event (avoid the middle man)
				m_NotificationEventChannel.RaiseEvent("Player built a flyway!");
			}
			
			
			_hexManagerSO.setupResourceRequest.Invoke();
			
			// if (_gameManagerSO.DoneSetupPhase())
			// {
			// 	// changeState(_owner.diceState);
			// 	Debug.Log("Done with Setup Phase!");
			// 	_owner.pendingSetupState.SetDiceStateServerRpc();
			// 	changeState(_owner.diceState);
			// }
			// else
			// {
			// 	int nextPlayerTurn =  _gameManagerSO.IncrementAndGetNextPlayerIndex();
			// 	Debug.Log($"Performing Setup for Player ID: {nextPlayerTurn}");
			// 	if (_gameManagerSO.clientPlayer.GetId() != nextPlayerTurn)
			// 	{
			// 		NextPlayerSetupServerRpc(nextPlayerTurn);
			// 		// _owner.pendingSetupState.NextPlayerSetupClientRpc(nextPlayerTurn);
			// 		changeState(_owner.pendingSetupState);
			// 	}
			// 	else
			// 	{
			// 		changeState(_owner.setupState);
			// 	}
				
				
			// }
		}
		
		void SetupResourceResponseEventHandler(List<HexResource> setupResources)
		{
			// For each player
			// for (int player = 0; player < gameManagerSO.GetNumPlayers(); player++)
			// {
			// 	// Get resources for the current player
			// 	int[] playerResources = boardManagerSO.GetResourcesForPlayer(player, setupResources);
			
			// 	// Add the resources to the player's inventory
			// 	gameManagerSO.playerList[player].updateResources(playerResources);
			// }
			
			int[] playerResources = _owner.boardManagerSO.GetResourcesForPlayer(_gameManagerSO.clientPlayer.GetId(), setupResources);
			// _gameManagerSO.clientPlayer.updateResources(playerResources);
			// _gameManagerSO.clientPlayer.UpdateResources(playerResources);
			_gameManagerSO.clientPlayer.SetResources(playerResources);
			
			GoToNextPlayerTurn();
		}
		
		void GoToNextPlayerTurn()
		{
			if (_gameManagerSO.DoneSetupPhase())
			{
				// changeState(_owner.diceState);
				Debug.Log("Done with Setup Phase!");
				_owner.pendingSetupState.SetDiceStateServerRpc();
				changeState(_owner.diceState);
			}
			else
			{
				int nextPlayerTurn =  _gameManagerSO.IncrementAndGetNextPlayerIndex();
				Debug.Log($"Performing Setup for Player ID: {nextPlayerTurn}");
				if (_gameManagerSO.clientPlayer.GetId() != nextPlayerTurn)
				{
					NextPlayerSetupServerRpc(nextPlayerTurn);
					// _owner.pendingSetupState.NextPlayerSetupClientRpc(nextPlayerTurn);
					changeState(_owner.pendingSetupState);
				}
				else
				{
					changeState(_owner.setupState);
				}
			}
		}
		
		[ServerRpc(RequireOwnership = false)]
		void NextPlayerSetupServerRpc(int nextPlayerIndex)
		{
			_owner.pendingSetupState.NextPlayerSetupClientRpc(nextPlayerIndex);	
		} 
		
	}
}