using Unity.Netcode;
using UnityEngine;

namespace Synthicate
{
	public class GameManagerPendingSetupState : GameManagerAbstractState
	{
		
		[SerializeField]
		BoolEventChannel m_EnablePlayerControllerEventChannel;

		public override void Enter()
		{
			m_EnablePlayerControllerEventChannel.RaiseEvent(true);
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
		
		[ClientRpc]
		public void SetDiceStateClientRpc()
		{
			if (!IsActiveState()) return;
			
			changeState(_owner.diceState);
		}

	}
}