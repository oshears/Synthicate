using UnityEngine;

namespace Synthicate {

	public abstract class GameManagerState : IManagerState
	{
		protected GameManagerStateMachine _stateMachine;
		protected GameManager _owner;
		protected GameManagerSO _gameManagerSO;
		protected HexManagerScriptableObject _hexManagerSO;
		// protected PlayerScriptableObject[] _playerScriptableObjects;

		public GameManagerState(GameManager owner)
		{
			_owner = owner;
			_stateMachine = owner.stateMachine;
			// _uiScriptableObject = owner.uiScriptableObject;
			// _deckScriptableObject = owner.deckScriptableObject;
			// _playerScriptableObject = owner.playerScriptableObject;
			// _npcScriptableObjects = owner.npcScriptableObjects;
			// _gunScriptableObject = owner.gunScriptableObject;
		}
		public virtual void Enter() {}
		public virtual void Execute() {}
		public virtual void Exit() {}
		protected void changeState(GameManagerState newState)
		{
			_stateMachine.ChangeState(newState);
		}
		public virtual void OnGUI()
		{
			GUILayout.BeginArea(new Rect(0, 500, 500, 500));
			GUILayout.Label($"Current GameManagerState: {this}");
			GUILayout.EndArea();
		}
	}
	
}