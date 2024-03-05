using Unity.Netcode;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode.Transports.UTP;

namespace Synthicate {

	// public abstract class GameManagerAbstractState : NetworkBehaviour, IManagerState 
	public abstract class GameManagerAbstractState : IManagerState 
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
		// protected GameNetworkManagerScriptableObject _gameNetworkManagerSO;
		
		protected Player _clientPlayer;
		
		// [SerializeReference]
		// GameNetworkManagerScriptableObject gameNetworkManagerSO;
		
		UnityTransport transport;

		public GameManagerAbstractState(GameManager owner)
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
			// _gameNetworkManagerSO = owner.gameNetworkManagerSO;
			
			_clientPlayer = _gameManagerSO.clientPlayer;
			
		}
		public abstract void Enter();
		public abstract void Execute();
		public abstract void Exit();
		protected void changeState(GameManagerAbstractState newState)
		{
			_stateMachine.ChangeState(newState);
		}
		public virtual void OnGUI()
		{
			GUILayout.BeginArea(new Rect(0, 500, 500, 500));
			GUILayout.Label($"Current GameManagerAbstractState: {this}");
			GUILayout.EndArea();
		}
	}
	
}