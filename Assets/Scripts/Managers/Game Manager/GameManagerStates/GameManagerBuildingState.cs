using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Synthicate
{
	public class GameManagerBuildingState : GameManagerAbstractState
	{
		
		[Header("Event Channels")]
		
		[SerializeField]
		GameMenuStateEventChannel m_GameMenuStateEventChannel;
		
		[SerializeField]
		EventChannelSO m_CancelButtonEventChannel;
		
		[SerializeField]
		StringEventChannel m_NotificationEventChannel;
		
		[SerializeField]
		StringEventChannel m_LocalNotificationEventChannel;
		
		public override void Enter()
		{
			_strongholdManagerSO.playerBuildEvent.AddListener(PlayerBuildStrongholdEventHandler);	
			_flywayManagerSO.playerBuildEvent.AddListener(PlayerBuildFlywayEventHandler);
			BuildPermissions playerBuildPermissions = new BuildPermissions(_gameManagerSO.clientPlayer.canBuildFlyway(), _gameManagerSO.clientPlayer.canBuildOutpost(), _gameManagerSO.clientPlayer.canBuildStronghold());
			_strongholdManagerSO.beginBuildModeForPlayer(_gameManagerSO.clientPlayer.GetId(), _boardManagerSO.GetValidPointsFor(_gameManagerSO.clientPlayer.GetId()), playerBuildPermissions);
			_flywayManagerSO.beginBuildModeForPlayer(_gameManagerSO.clientPlayer.GetId(), _boardManagerSO.GetValidEdgesFor(_gameManagerSO.clientPlayer.GetId()), playerBuildPermissions);
		

			m_GameMenuStateEventChannel.RaiseEvent(GameMenuType.PlayerBuildModeScreen);
			m_CancelButtonEventChannel.OnEventRaised += CancelButtonEventHandler;
		}
		
		public override void Execute()
		{

		}

		public override void Exit()
		{
			_strongholdManagerSO.endBuildMode();
			_flywayManagerSO.endBuildMode();
			_strongholdManagerSO.playerBuildEvent.RemoveListener(PlayerBuildStrongholdEventHandler);	
			_flywayManagerSO.playerBuildEvent.RemoveListener(PlayerBuildFlywayEventHandler);
		}

		public override void OnGUI()
		{
			// if (!IsActiveState()) return;
			
			// GUI.Box(UserInterface.s_gameMenuArea, "");

			// GUILayout.BeginArea(UserInterface.s_gameMenuArea);
			// if(GUILayout.Button("Exit Build Mode"))
			// {
			// 	changeState(_owner.idleState);
			// }
			// GUILayout.EndArea();
		}
		
		void CancelButtonEventHandler()
		{
			changeState(_owner.idleState);
		}
		
		void PlayerBuildStrongholdEventHandler(bool validBuild)
		{
			
			if (validBuild)
			{
				_strongholdManagerSO.endBuildMode();
				_gameManagerSO.playerBuildEvent.Invoke();
				_strongholdManagerSO.pointUpdateRequest.Invoke();
				// GameEvent gameEvent = new GameEvent(GameEventType.Build, _gameManagerSO.clientPlayer + " has built a new stronghold or outpost!");
				// _gameManagerSO.playerEvent.Invoke(gameEvent);
				m_NotificationEventChannel.RaiseEvent($"{_gameManagerSO.GetCurrentPlayer().GetName()} has built a new stronghold or outpost!!");

				changeState(_owner.idleState);
			}
			else
			{
				m_LocalNotificationEventChannel.RaiseEvent("Insufficient Resources!");
			}
		}
		
		void PlayerBuildFlywayEventHandler(bool validBuild)
		{
			
			if (validBuild)
			{
				_flywayManagerSO.endBuildMode();
				_gameManagerSO.playerBuildEvent.Invoke();
				_flywayManagerSO.edgeUpdateRequest.Invoke();
				// GameEvent gameEvent = new GameEvent(GameEventType.Build, _gameManagerSO.clientPlayer + " has built a new flyway!");
				// _gameManagerSO.playerEvent.Invoke(gameEvent);
				
				changeState(_owner.idleState);
			}
			else
			{
				m_LocalNotificationEventChannel.RaiseEvent("Insufficient Resources!");
			}
		}
		
		

	}
}