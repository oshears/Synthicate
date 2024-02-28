using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.VisualScripting;

namespace Synthicate
{
	public class GameManager : NetworkBehaviour
	{
		public GameManagerStateMachine stateMachine; 
		
		[SerializeField]
		public GameManagerSO gameManagerSO;
		
		[SerializeField]
		public UiScriptableObject userInterfaceSO;
		
		[SerializeField]
		public HexManagerScriptableObject hexManagerSO;
		
		public PlayerManagerSO[] playerManagerSOs;
        public PlayerManagerSO clientPlayerManagerSO;

        public FlywayManagerScriptableObject flywayManagerSO;
        public StrongholdManagerScriptableObject strongholdManagerSO;
        public DepotManagerScriptableObject depotManagerSO;
        public BoardManagerSO boardManagerSO;
        public AudioManagerSO audioManagerSO;
		
		

		void Awake()
		{
			stateMachine = new GameManagerStateMachine(this);
			stateMachine.ChangeState(new GameManagerInitState(this));
		}
		
		// Start is called before the first frame update
		void Start()
		{

		}
		
		void Update() {
			stateMachine.Update();
		}
		
		void OnGUI()
		{
			stateMachine.OnGUI();
		}
		

	}

}
