using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace Synthicate
{
	
	public class GameManagerDiceState : GameManagerAbstractState
	{
		float _diceDelay = 0;
		bool _diceRollingDone = false;
		
		NetworkVariable<int> m_diceValue = new NetworkVariable<int>();
		
		[SerializeField]
		StringEventChannel m_NotificationEventChannel;

		public override void Enter()
		{
			
			if (_gameManagerSO.IsClientTurn())
			{
				m_diceValue.Value = _gameManagerSO.RollDice();
				_diceDelay = 0;
				_diceRollingDone = false;
			}
			
			
			// _hexManagerSO.hexSelectionEvent.Invoke(_diceValue);
			// _hexManagerSO.resourceRequest.Invoke(_diceValue);
			
			// GameEvent gameEvent = new GameEvent(GameEventType.Hack, "The game has begun! " + getCurrentPlayer().getName() + " hashed a " + diceValue);
			// playerEvent.Invoke(gameEvent);
		}
		
		public override void Execute()
		{
			
			
			if (!_diceRollingDone)
			{
				if(_diceDelay > 3)
				{
					_hexManagerSO.hexSelectionEvent.Invoke((uint) m_diceValue.Value);
					_hexManagerSO.resourceRequest.Invoke((uint) m_diceValue.Value);
					
					_diceRollingDone = true;
					_diceDelay = 0;
					
					m_NotificationEventChannel.RaiseEvent($"Hashed value was: {m_diceValue.Value}!");
				}
				else
				{
					_diceDelay += Time.deltaTime;
				}
			}
			else if(_diceDelay > 3)
			{
				Debug.Log("Moving to idle state.");
				
				// If current client's turn, then go to idle state.
				if (_gameManagerSO.IsClientTurn())
				{
					changeState(_owner.idleState);
				}
				// If not current client's turn, then go to pending state.
				else
				{
					changeState(_owner.pendingState);
				}
				
			}
			else
			{
				_diceDelay += Time.deltaTime;
			}
		}

		public override void Exit()
		{
			_userInterfaceSO.OnUpdateUserInterface();
		}

		public override void OnGUI()
		{
			if (!IsActiveState()) return;
			
			if (!_diceRollingDone)
			{
				GUI.Box(UserInterface.s_instructionArea, "");
				GUILayout.BeginArea(UserInterface.s_instructionArea);
				GUILayout.Label("A new value is being hashed...");
				GUILayout.EndArea();
			}
			else
			{
				GUI.Box(UserInterface.s_instructionArea, "");
				GUILayout.BeginArea(UserInterface.s_instructionArea);
				GUILayout.Label($"Hashed value was: {(uint) m_diceValue.Value}!");
				GUILayout.EndArea();
			}
		}
		

	}
}