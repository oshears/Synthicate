using Unity.Netcode;
using UnityEngine;

namespace Synthicate
{
	public class GameManagerPendingSetupState : GameManagerAbstractState
	{
		[Header("Event Channels")]
		[SerializeField] BoolEventChannelSO m_EnablePlayerControllerEventChannel;
		
		[SerializeField] GameMenuStateEventChannel m_GameMenuStateEventChannel;
		[SerializeField] EventChannelSO m_InitializeUiEventChannel;
		[SerializeField] EventChannelSO m_UpdateUiEventChannel;

		public override void Enter()
		{
			// Enable player panning
			m_EnablePlayerControllerEventChannel.RaiseEvent(true);
			
			// Setup UI
			m_InitializeUiEventChannel.RaiseEvent();
			m_GameMenuStateEventChannel.RaiseEvent(GameMenuType.PlayerPendingScreen);
			m_UpdateUiEventChannel.RaiseEvent();
		}
		
		public override void Execute()
		{

		}

		public override void Exit()
		{
			
		}
		
		
		[ClientRpc]
		public void NextPlayerSetupClientRpc(int nextPlayerIndex)
		{
			if (!IsActiveState()) return;
			
			Debug.Log($"Moving to setup player: {nextPlayerIndex}");
			_gameManagerSO.SetCurrentPlayerTurn(nextPlayerIndex);
			
			if (nextPlayerIndex == _gameManagerSO.clientPlayer.GetId())
			{
				changeState(_owner.setupState);
			}
		}
		
		// [ClientRpc]
		// public void SetPendingStateClientRpc(int currentPlayerTurn)
		// {
		// 	if (!IsActiveState()) return;
			
		// 	changeState(_owner.pendingState);
		// }
		
		[ServerRpc(RequireOwnership = false)]
		public void SetDiceStateServerRpc() => SetDiceStateClientRpc();
		
		[ClientRpc]
		public void SetDiceStateClientRpc()
		{
			if (!IsActiveState()) return;
			
			Debug.Log("Moving to dice state!");
			
			changeState(_owner.diceState);
		}

	}
}