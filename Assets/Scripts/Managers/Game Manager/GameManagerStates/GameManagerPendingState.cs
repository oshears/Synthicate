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
		public void TakeRandomResourceFromPeerServerRpc(int clientId) {
			ClientRpcParams clientRpcParams = new ClientRpcParams
			{
				Send = new ClientRpcSendParams
				{
					TargetClientIds = new ulong[]{(ulong) clientId}
				}
			};
		
			TakeRandomResourceFromPeerClientRpc(clientRpcParams);
		}
		
		[ClientRpc]
		public void TakeRandomResourceFromPeerClientRpc(ClientRpcParams clientRpcParams = default)
		{
			if(!IsActiveState()){
				Debug.LogError("Error! This client received the RPC and wasn't in the pending state!");
				// return ResourceType.None;
			}
			
			ResourceType resourceToGive = _gameManagerSO.clientPlayer.RemoveRandomResource();
			
			_owner.m_DiceHackingState.GiveResourceToCurrentPlayerServerRpc(resourceToGive);
			_owner.hackingState.GiveResourceToCurrentPlayerServerRpc(resourceToGive);
		}
		

	}
}