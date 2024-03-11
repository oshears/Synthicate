using System.Collections.Generic;
using UnityEngine;

namespace Synthicate
{
	
	public class GameManagerDiceState : GameManagerAbstractState
	{
		uint _diceValue;
		float _diceDelay = 0;
		bool _diceRollingDone = false;
		

		public override void Enter()
		{
			_diceValue = 0;
			_diceDelay = 0;
			_diceRollingDone = false;
			
			_diceValue = _gameManagerSO.rollDice();
			_hexManagerSO.hexSelectionEvent.Invoke(_diceValue);
			_hexManagerSO.resourceRequest.Invoke(_diceValue);
			// GameEvent gameEvent = new GameEvent(GameEventType.Hack, "The game has begun! " + getCurrentPlayer().getName() + " hashed a " + diceValue);
			// playerEvent.Invoke(gameEvent);
		}
		
		public override void Execute()
		{
			
			
			if (!_diceRollingDone)
			{
				if(_diceDelay > 3)
				{
					_hexManagerSO.hexSelectionEvent.Invoke(_diceValue);
					_hexManagerSO.resourceRequest.Invoke(_diceValue);
					
					_diceRollingDone = true;
					_diceDelay = 0;
				}
				else
				{
					_diceDelay += Time.deltaTime;
				}
			}
			else if(_diceDelay > 3)
			{
				Debug.Log("Moving to idle state.");
				changeState(_owner.idleState);
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
				GUILayout.Label($"Hashed value was: {_diceValue}!.");
				GUILayout.EndArea();
			}
			
		}
		
		

	}
}