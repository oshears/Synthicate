using Unity.Netcode;
using UnityEngine;

namespace Synthicate
{
	public class GameManagerPendingState : GameManagerAbstractState
	{
		
		[Header("Event Channels")]
		
		[SerializeField] GameMenuStateEventChannel m_GameMenuStateEventChannel;
		[SerializeField] EventChannelSO m_UpdateUserInterfaceEventChannel;
		[SerializeField] EventChannelSO m_InitializeUiEventChannel;

		public override void Enter()
		{
			m_InitializeUiEventChannel.RaiseEvent();
			m_UpdateUserInterfaceEventChannel.RaiseEvent();
			
			m_GameMenuStateEventChannel.RaiseEvent(GameMenuType.PlayerPendingScreen);
		}
		
		public override void Execute()
		{

		}

		public override void Exit()
		{
			
		}
		
		[ServerRpc(RequireOwnership = false)]
		public void InitiateTradeRequestServerRpc(int clientId) => InitiateTradeRequestClientRpc(clientId);
		
		[ClientRpc]
		void InitiateTradeRequestClientRpc(int clientId)
		{
			if (clientId == _gameManagerSO.GetClientPlayerId())
			{
				changeState(_owner.m_PeerTradingState);
			}
		}

	}
}