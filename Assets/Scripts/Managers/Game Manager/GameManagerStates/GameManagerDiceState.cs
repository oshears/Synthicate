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
		static int s_DiceRollDelay = 1;
		static int s_DiceDisplayDelay = 1;
		
		// NetworkVariable<int> m_DiceValue = new NetworkVariable<int>();
		int m_DiceValue = -1;
		
		[SerializeField]
		StringEventChannel m_NotificationEventChannel;
		
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
			
			m_DiceValue = -1;
			m_DiceDelay = 0;
			m_State = DiceState.PendingDice;
			
			
			if (_gameManagerSO.IsClientTurn())
			{
				UpdateDiceRollServerRpc(_gameManagerSO.RollDice());
			}
			
			
			// _hexManagerSO.hexSelectionEvent.Invoke(_diceValue);
			// _hexManagerSO.resourceRequest.Invoke(_diceValue);
			
			// GameEvent gameEvent = new GameEvent(GameEventType.Hack, "The game has begun! " + getCurrentPlayer().getName() + " hashed a " + diceValue);
			// playerEvent.Invoke(gameEvent);
		}
		
		public override void Execute()
		{
			if (m_State == DiceState.HashingValue)
			{
				if(m_DiceDelay > s_DiceRollDelay)
				{
					_hexManagerSO.hexSelectionEvent.Invoke((uint) m_DiceValue);
					_hexManagerSO.resourceRequest.Invoke((uint) m_DiceValue);
					
					m_DiceDelay = 0;
					
					m_NotificationEventChannel.RaiseEvent($"Hashed value was: {m_DiceValue}!");
					
					m_State = DiceState.DisplayingDice;
				}
				else
				{
					m_DiceDelay += Time.deltaTime;
				}
			}
			else if (m_State == DiceState.DisplayingDice)
			{
				if (m_DiceDelay > s_DiceDisplayDelay)
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
				if (_gameManagerSO.IsClientTurn())
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
			_userInterfaceSO.OnUpdateUserInterface();
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
		
		public override void OnGUI()
		{
			if (!IsActiveState()) return;
			
			Rect diceArea = new Rect(0, Screen.height * 0.25f, Screen.width*0.25f, Screen.height*0.4f);
			if (m_State == DiceState.HashingValue)
			{
				GUI.Box(diceArea, "");
				GUILayout.BeginArea(diceArea);
				GUILayout.Label("A new value is being hashed...");
				GUILayout.EndArea();
			}
			else if (m_State == DiceState.DisplayingDice)
			{
				GUI.Box(diceArea, "");
				GUILayout.BeginArea(diceArea);
				GUILayout.Label($"Hashed value was: {(uint) m_DiceValue}!");
				GUILayout.EndArea();
			}
		}
		

	}
}