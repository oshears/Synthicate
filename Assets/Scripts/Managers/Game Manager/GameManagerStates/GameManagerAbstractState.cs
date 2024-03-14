using Unity.Netcode;
using UnityEngine;
using Unity.Netcode.Transports.UTP;
using Codice.Client.BaseCommands;

namespace Synthicate {

	// public abstract class GameManagerAbstractState : NetworkBehaviour, IManagerState 
	public abstract class GameManagerAbstractState : NetworkBehaviour, IManagerState 
	{
		protected GameManager _owner;
		private GameManagerStateMachine _stateMachine;
		protected GameManagerSO _gameManagerSO;
		protected HexManagerScriptableObject _hexManagerSO;
		protected UiScriptableObject _userInterfaceSO;
		protected FlywayManagerScriptableObject _flywayManagerSO;
		protected StrongholdManagerScriptableObject _strongholdManagerSO;
		protected DepotManagerScriptableObject _depotManagerSO;
		protected BoardManagerSO _boardManagerSO;
		protected AudioManagerSO _audioManagerSO;
		protected UnityTransport _transport;

		public void SetOwner(GameManager owner)
		{
			_owner = owner;
			_stateMachine = owner.stateMachine;
			
			_gameManagerSO = owner.gameManagerSO;
			_hexManagerSO = owner.hexManagerSO;
			_userInterfaceSO = owner.userInterfaceSO;
			_flywayManagerSO = owner.flywayManagerSO;
			_strongholdManagerSO = owner.strongholdManagerSO;
			_depotManagerSO = owner.depotManagerSO;
			_boardManagerSO = owner.boardManagerSO;
			_audioManagerSO = owner.audioManagerSO;
		}
		public abstract void Enter();
		public abstract void Execute();
		public abstract void Exit();
		protected void changeState(GameManagerAbstractState newState)
		{
			_stateMachine.ChangeState(newState);
		}
		protected bool IsActiveState()
		{
			return _stateMachine._currentState.Equals(this);
		}
		public virtual void OnGUI()
		{
			if (!IsActiveState()) return;
			
			GUILayout.BeginArea(new Rect(0, Screen.height * 0.25f, Screen.width*0.1f, Screen.height*0.1f));
			GUILayout.Label($"Current GameManagerAbstractState: {this}");
			GUILayout.EndArea();
		}
	}
	
}