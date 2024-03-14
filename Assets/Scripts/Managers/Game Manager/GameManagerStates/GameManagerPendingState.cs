using Unity.Netcode;
using UnityEngine;

namespace Synthicate
{
	public class GameManagerPendingState : GameManagerAbstractState
	{
		
		[Header("Event Channels")]
		
		[SerializeField]
		GameMenuStateEventChannel m_GameMenuStateEventChannel;
		

		public override void Enter()
		{
			_userInterfaceSO.OnUpdateUserInterface();
			
			m_GameMenuStateEventChannel.RaiseEvent(GameMenu.Screens.PlayerWaitScreen);
		}
		
		public override void Execute()
		{

		}

		public override void Exit()
		{
			
		}

	}
}