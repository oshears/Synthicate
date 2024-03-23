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
		
		[ServerRpc(RequireOwnership = false)]
		public ResourceType TakeRandomResourceFromPeerServerRpc(int clientId) {
			ClientRpcParams clientRpcParams = new ClientRpcParams
			{
				Send = new ClientRpcSendParams
				{
					TargetClientIds = new ulong[]{(ulong) clientId}
				}
			};
		
			return TakeRandomResourceFromPeerClientRpc(clientRpcParams);
		}
		
		[ClientRpc]
		public ResourceType TakeRandomResourceFromPeerClientRpc(ClientRpcParams clientRpcParams = default)
		{
			if(!IsActiveState()){
				Debug.LogError("Error! This client received the RPC and wasn't in the pending state!");
				return ResourceType.None;
			}
			
			return _gameManagerSO.clientPlayer.RemoveRandomResource();
		}
		

	}
}