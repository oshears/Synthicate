using UnityEngine;

namespace Synthicate {

	public abstract class GameManagerState : IManagerState
	{
		protected GameManager _owner;
		private GameManagerStateMachine _stateMachine;
		protected GameManagerSO _gameManagerSO;
		protected HexManagerScriptableObject _hexManagerSO;
		protected UiScriptableObject _userInterfaceSO;
		protected PlayerManagerSO[] _playerManagerSOs;
		protected PlayerManagerSO _clientPlayerManagerSO;
		protected FlywayManagerScriptableObject _flywayManagerSO;
		protected StrongholdManagerScriptableObject _strongholdManagerSO;
		protected DepotManagerScriptableObject _depotManagerSO;
		protected BoardManagerSO _boardManagerSO;
		protected AudioManagerSO _audioManagerSO;

		public GameManagerState(GameManager owner)
		{
			_owner = owner;
			_stateMachine = owner.stateMachine;
			
			_gameManagerSO = owner.gameManagerSO;
			_hexManagerSO = owner.hexManagerSO;
			_userInterfaceSO = owner.userInterfaceSO;
			_playerManagerSOs = owner.playerManagerSOs;
			_clientPlayerManagerSO = owner.clientPlayerManagerSO;
			_flywayManagerSO = owner.flywayManagerSO;
			_strongholdManagerSO = owner.strongholdManagerSO;
			_depotManagerSO = owner.depotManagerSO;
			_boardManagerSO = owner.boardManagerSO;
			_audioManagerSO = owner.audioManagerSO;
			
		}
		public abstract void Enter();
		public abstract void Execute();
		public abstract void Exit();
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