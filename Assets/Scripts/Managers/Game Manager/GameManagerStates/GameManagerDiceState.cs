using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace Synthicate
{
	
	public class GameManagerDiceState : GameManagerAbstractState
	{
		float m_DiceDelay = 0;
		
		// bool m_DiceRollingDone = false;
		
		// bool m_HashingValue = false;
		
		[SerializeField] 
		[Tooltip("Amount of time that the dice is rolled / hashed for.")]
		[Range(0, 5)]
		int m_DiceRollDelay = 1;
		
		[SerializeField] 
		[Range(0, 5)]
		[Tooltip("Amount of time that the dice value is displayed for.")]
		int m_DiceDisplayDelay = 1;
		
		// NetworkVariable<int> m_DiceValue = new NetworkVariable<int>();
		[SerializeField] int m_DiceValue = 1;
		
		[SerializeField]
		[Range(1, 12)]
		public int m_FixedDiceValue = 7;
		
		[SerializeField] public bool m_UsingFixedDice = true;
		
		[Header("Event Channels")]
		
		[SerializeField] StringEventChannel m_NotificationEventChannel;
		
		[SerializeField] StringEventChannel m_LocalNotificationEvent;
		
		[SerializeField] EventChannelSO m_UpdateUiEventChannel;
		
		[SerializeField] GameMenuStateEventChannel m_GameMenuStateEventChannel;
		
		enum DiceState
		{
			PendingDice,
			HashingValue,
			DisplayingDice,
			Done
		}
		
		DiceState m_State = DiceState.PendingDice;

		public override void Enter()
		{
			// m_DiceValue.OnValueChanged += DiceValueChangedEventHandler;
			
			m_GameMenuStateEventChannel.RaiseEvent(GameMenuType.PlayerPendingScreen);
			
			m_DiceValue = -1;
			m_DiceDelay = 0;
			m_State = DiceState.PendingDice;
			
			_owner.boardManagerSO.updatePointsResponseEvent.AddListener(UpdatePointsResponseEventHandler);
			
			if (_gameManagerSO.IsClientTurn())
			{
				if (m_UsingFixedDice)
				{
					UpdateDiceRollServerRpc(m_FixedDiceValue);
				}
				else
				{
					UpdateDiceRollServerRpc(_gameManagerSO.RollDice());
				}
			}
			
			
			// _hexManagerSO.hexSelectionEvent.Invoke(_diceValue);
			// _hexManagerSO.resourceRequest.Invoke(_diceValue);
			
			// GameEvent gameEvent = new GameEvent(GameEventType.Hack, "The game has begun! " + getCurrentPlayer().getName() + " hashed a " + diceValue);
			// playerEvent.Invoke(gameEvent);
			
			// update all player resources when board manager announces that player placements have been recorded
			// _owner.boardManagerSO.updatePointsResponseEvent.AddListener(UpdatePlayerResources); 
			
			// request updated stronghold/outpost/flyway points when the hex manager finishes updating its resources
			// _owner.hexManagerSO.updatePointsResponseEvent.AddListener(UpdatePlayerResources);
				
				

		}
		
		public override void Execute()
		{
			if (m_State == DiceState.HashingValue)
			{
				if(m_DiceDelay > m_DiceRollDelay)
				{
					_hexManagerSO.hexSelectionEvent.Invoke((uint) m_DiceValue);
					_hexManagerSO.resourceRequest.Invoke((uint) m_DiceValue);
					
					m_DiceDelay = 0;
					
					m_State = DiceState.DisplayingDice;
				}
				else
				{
					m_DiceDelay += Time.deltaTime;
				}
			}
			else if (m_State == DiceState.DisplayingDice)
			{
				if (m_DiceDelay > m_DiceDisplayDelay)
				{
					m_State = DiceState.Done;
				}
				else
				{
					m_DiceDelay += Time.deltaTime;
				}
			}
			else if (m_State == DiceState.Done)
			{
				// If current client's turn, then go to idle state.
				if (_gameManagerSO.IsClientTurn() && m_DiceValue == 7)
				{
					Debug.Log("Moving to hacking state.");
					changeState(_owner.m_DiceHackingState);
				}
				else if (_gameManagerSO.IsClientTurn())
				{
					Debug.Log("Moving to idle state.");
					changeState(_owner.idleState);
				}
				// If not current client's turn, then go to pending state.
				else
				{
					Debug.Log("Moving to pending state.");
					changeState(_owner.pendingState);
				}
			}
			
		}

		public override void Exit()
		{
			m_UpdateUiEventChannel.RaiseEvent();
			_owner.boardManagerSO.updatePointsResponseEvent.RemoveListener(UpdatePointsResponseEventHandler);
		}
		
		[ServerRpc(RequireOwnership = false)]
		void UpdateDiceRollServerRpc(int diceValue)
		{
			// m_DiceValue.Value = diceValue;
			UpdateDiceRollClientRpc(diceValue);
		}
		
		[ClientRpc]
		void UpdateDiceRollClientRpc(int diceValue)
		{
			m_DiceValue = diceValue;
			m_State = DiceState.HashingValue;
		}
		
		void UpdatePointsResponseEventHandler()
		{
			m_NotificationEventChannel.RaiseEvent($"Hashed value was: {m_DiceValue}!");
			
			// Get resources for this player at this dice roll
			int clientPlayerId = _gameManagerSO.GetClientPlayerId();
			int[] playerResources = _owner.boardManagerSO.GetResourcesForPlayer(clientPlayerId,  _owner.hexManagerSO.GetResources());
			
			// Update resources for this player at this dice roll
			_gameManagerSO.clientPlayer.UpdateResources(playerResources);
			
			// Update all player outpost and stronghold counts
			_gameManagerSO.clientPlayer.numOutposts =  _owner.boardManagerSO.GetNumOutpostsFor(clientPlayerId);
			_gameManagerSO.clientPlayer.numStrongholds =  _owner.boardManagerSO.GetNumStrongholdsFor(clientPlayerId);
			
			// update all player flyway counts when board manager announces that player flyway placements have been recorded
			_gameManagerSO.clientPlayer.numFlyways =  _owner.boardManagerSO.GetNumFlywaysFor(clientPlayerId);
			
			// Display notifications for collected resources			
			for(int i = 0; i < playerResources.Length; i++)
			{
				if (playerResources[i] > 0)
				{
					m_LocalNotificationEvent.RaiseEvent($"You recieved {playerResources[i]} {(ResourceType) i}");
				}
			}
			// m_UpdateUiEventChannel.RaiseEvent();
			m_UpdateUiEventChannel.RaiseEvent();
			
		}
		
		public override void OnGUI()
		{
			if (!IsActiveState()) return;
			
			// Rect diceArea = new Rect(0, Screen.height * 0.25f, Screen.width*0.25f, Screen.height*0.4f);
			// if (m_State == DiceState.HashingValue)
			// {
			// 	GUI.Box(diceArea, "");
			// 	GUILayout.BeginArea(diceArea);
			// 	GUILayout.Label("A new value is being hashed...");
			// 	GUILayout.EndArea();
			// }
			// else if (m_State == DiceState.DisplayingDice)
			// {
			// 	GUI.Box(diceArea, "");
			// 	GUILayout.BeginArea(diceArea);
			// 	GUILayout.Label($"Hashed value was: {(uint) m_DiceValue}!");
			// 	GUILayout.EndArea();
			// }
		}
		

	}
}