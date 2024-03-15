using Unity.Netcode;
using UnityEngine;
using System.Collections.Generic;
using UnityEditor;

namespace Synthicate
{
	public class GameManager : NetworkBehaviour
	{
		public GameManagerStateMachine stateMachine; 
		
		[Header("Scriptable Objects")]
		[SerializeField]
		public GameManagerSO gameManagerSO;
		
		[SerializeField]
		public UiScriptableObject userInterfaceSO;
		
		[SerializeField]
		public HexManagerScriptableObject hexManagerSO;
		
		// public PlayerManagerSO[] playerManagerSOs;
		// public PlayerManagerSO clientPlayerManagerSO;

		public FlywayManagerScriptableObject flywayManagerSO;
		public StrongholdManagerScriptableObject strongholdManagerSO;
		public DepotManagerScriptableObject depotManagerSO;
		public BoardManagerSO boardManagerSO;
		public AudioManagerSO audioManagerSO;
		
		// [Header("Game Manager States")]
		[System.NonSerialized] public GameManagerInitState initState;
		[System.NonSerialized] public GameManagerMainMenuState mainMenuState;
		[System.NonSerialized] public GameManagerClientLobbyState clientLobbyState;
		[System.NonSerialized] public GameManagerHostLobbyState hostLobbyState;
		[System.NonSerialized] public GameManagerSetupState setupState;
		[System.NonSerialized] public GameManagerPendingSetupState pendingSetupState;
		[System.NonSerialized] public GameManagerDiceState diceState;
		[System.NonSerialized] public GameManagerIdleState idleState;
		[System.NonSerialized] public GameManagerBuildingState buildingState;
		[System.NonSerialized] public GameManagerHackingState hackingState;
		[System.NonSerialized] public GameManagerPendingState pendingState;
		[System.NonSerialized] public GameManagerTradingState tradingState;
		
		

		void Awake()
		{
			
		}
		
		
		// Start is called before the first frame update
		void Start()
		{
			stateMachine = new GameManagerStateMachine(this);
			
			initState = GetComponent<GameManagerInitState>();
			initState.SetOwner(this);
			
			mainMenuState = GetComponent<GameManagerMainMenuState>();
			mainMenuState.SetOwner(this);
			
			setupState = GetComponent<GameManagerSetupState>();
			setupState.SetOwner(this);
			
			pendingSetupState = GetComponent<GameManagerPendingSetupState>();
			pendingSetupState.SetOwner(this);
			
			diceState = GetComponent<GameManagerDiceState>();
			diceState.SetOwner(this);
			
			idleState = GetComponent<GameManagerIdleState>();
			idleState.SetOwner(this);
			
			buildingState = GetComponent<GameManagerBuildingState>();
			buildingState.SetOwner(this);
			
			hackingState = GetComponent<GameManagerHackingState>();
			hackingState.SetOwner(this);
			
			hostLobbyState = GetComponent<GameManagerHostLobbyState>();
			hostLobbyState.SetOwner(this);
			
			clientLobbyState = GetComponent<GameManagerClientLobbyState>();
			clientLobbyState.SetOwner(this);
			
			pendingState = GetComponent<GameManagerPendingState>();
			pendingState.SetOwner(this);
			
			tradingState = GetComponent<GameManagerTradingState>();
			tradingState.SetOwner(this);
			
			stateMachine.ChangeState(initState);

			// request updated stronghold/outpost/flyway points when the hex manager finishes updating its resources
			hexManagerSO.managerResourceResponse.AddListener(() => {
				strongholdManagerSO.pointUpdateRequest.Invoke();
				flywayManagerSO.edgeUpdateRequest.Invoke();
			});

			// // request updated stronghold/outpost/flyway points when the hex manager finishes updating its resources
			// hexManagerSO.managerSetupResourceResponse.AddListener((List<HexResource> setupResources) => {
				
			// 	// For each player
			// 	for (int player = 0; player < gameManagerSO.GetNumPlayers(); player++)
			// 	{
			// 		// Get resources for the current player
			// 		int[] playerResources = boardManagerSO.GetResourcesForPlayer(player, setupResources);
					
			// 		// Add the resources to the player's inventory
			// 		gameManagerSO.playerList[player].updateResources(playerResources);
			// 	}

			// });

			// request the board manager to update player stronghold/outpost placements after the stronghold manager collects them
			strongholdManagerSO.managerPointUpdateResponse.AddListener((List<PlayerPoint> points) => boardManagerSO.updatePointsRequestEvent.Invoke(points));

			// request the board manager to update player flyway placements after the flyway manager collects them
			flywayManagerSO.managerEdgeUpdateResponse.AddListener((List<PlayerEdge> edges) => boardManagerSO.updateEdgesRequestEvent.Invoke(edges));

			// update all player resources when board manager announces that player placements have been recorded
			boardManagerSO.updatePointsResponseEvent.AddListener(() => {
				for (int player = 0; player < gameManagerSO.GetNumPlayers(); player++)
				{
					// Get resources for this player at this dice roll
					int[] playerResources = boardManagerSO.GetResourcesForPlayer(player, hexManagerSO.getResources());
					
					// Update resources for this player at this dice roll
					gameManagerSO.playerList[player].UpdateResources(playerResources);
					
					// Update all player outpost and stronghold counts
					gameManagerSO.playerList[player].numOutposts = boardManagerSO.GetNumOutpostsFor(player);
					gameManagerSO.playerList[player].numStrongholds = boardManagerSO.GetNumStrongholdsFor(player);
					
					// update all player flyway counts when board manager announces that player flyway placements have been recorded
					gameManagerSO.playerList[player].numFlyways = boardManagerSO.GetNumFlywaysFor(player);
				}
			});

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
