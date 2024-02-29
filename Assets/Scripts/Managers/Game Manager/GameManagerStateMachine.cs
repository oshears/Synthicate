using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

namespace Synthicate
{
    public class GameManagerStateMachine
    {
        GameManager _owner;
        GameManagerAbstractState _currentState;

        public GameManagerStateMachine(GameManager owner)
        {
            _owner = owner;
            // _currentState = new GameManagerInitState(owner);
        }

        public void ChangeState(GameManagerAbstractState newState)
        {
            if (_currentState != null)
                _currentState.Exit();

            _currentState = newState;
            _currentState.Enter();
        }

        public void Update()
        {
            if (_currentState != null) _currentState.Execute();
        }
        
        public void OnGUI()
        {
            if (_currentState != null) _currentState.OnGUI();
        }
    }
}
