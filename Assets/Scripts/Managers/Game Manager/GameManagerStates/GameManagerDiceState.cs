using System.Collections.Generic;
using UnityEngine;

namespace Synthicate
{
	
	public class GameManagerDiceState : GameManagerAbstractState
	{
		uint _diceValue;
		float _diceDelay = 0;
		bool _diceDone = false;
		
		public GameManagerDiceState(GameManager owner) : base(owner) 
		{
			_diceValue = 0;
			_diceDelay = 0;
			_diceDone = false;
		}

		public override void Enter()
		{
			_diceValue = _gameManagerSO.rollDice();
			_hexManagerSO.hexSelectionEvent.Invoke(_diceValue);
			_hexManagerSO.resourceRequest.Invoke(_diceValue);
			// GameEvent gameEvent = new GameEvent(GameEventType.Hack, "The game has begun! " + getCurrentPlayer().getName() + " hashed a " + diceValue);
			// playerEvent.Invoke(gameEvent);
		}
		
		public override void Execute()
		{
			_diceDelay += Time.deltaTime;
			
			if(_diceDelay > 3) _diceDone = true;
			else if(_diceDelay > 6){
				Debug.Log("Moving to idle state.");
				changeState(new GameManagerIdleState(_owner));
			}
		}

		public override void Exit()
		{
			
		}

		public override void OnGUI()
		{
			if (!_diceDone)
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