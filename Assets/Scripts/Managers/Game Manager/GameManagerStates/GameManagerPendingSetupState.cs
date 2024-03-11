using Unity.Netcode;
using UnityEngine;

namespace Synthicate
{
	public class GameManagerPendingSetupState : GameManagerAbstractState
	{

		public override void Enter()
		{

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
			if (nextPlayerIndex == _gameManagerSO.clientPlayer.GetId())
			{
				changeState(_owner.setupState);
			}
		}

	}
}