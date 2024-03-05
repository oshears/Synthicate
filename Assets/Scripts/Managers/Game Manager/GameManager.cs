using System.Collections.Generic;
using UnityEngine;


namespace Synthicate
{
	public class GameManager : MonoBehaviour
	{
		public GameManagerStateMachine stateMachine; 
		
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
		// public GameNetworkManagerScriptableObject gameNetworkManagerSO;
		
		// UnityTransport transport;

		void Awake()
		{
			stateMachine = new GameManagerStateMachine(this);
			stateMachine.ChangeState(new GameManagerInitState(this));
		}
		
		// Start is called before the first frame update
		void Start()
		{
			// transport = NetworkManager.Singleton.gameObject.GetComponent<UnityTransport>();
			
			// current player builds a stronghold
			// strongholdManagerSO.playerBuildEvent.AddListener((bool validBuild) => {
			// 	endClientBuildMode();
			// 	if (validBuild)
			// 	{
			// 		playerBuildEvent.Invoke();
			// 		strongholdManagerSO.pointUpdateRequest.Invoke();
			// 		GameEvent gameEvent = new GameEvent(GameEventType.Build, getCurrentPlayer().getName() + " has built a new stronghold or outpost!");
			// 		playerEvent.Invoke(gameEvent);

			// 		if (!inSetupPhase()) getCurrentPlayer().buildStronghold();

			// 		// if in setup mode, let the client build a flyway now
			// 		if (isClientPlayerSetupActive()) beginClientSetupFlywayBuildMode();
			// 	}
			// 	else if (clientPlayerIsActive())
			// 	{
			// 		GameEvent gameEvent = new GameEvent(GameEventType.Build, "You do not have enough resources to build an outpost or stronghold here!");
			// 		playerEvent.Invoke(gameEvent);
			// 	}
			// });

			// current player builds a flyway
			// flywayManagerSO.playerBuildEvent.AddListener((bool validBuild) => {
			// 	// TODO: We need to send this response back to the server so that it can command the change on all clients
			// 	// NOTE: This may already be happening if the stronghold point triggers the event across the network...
			// 	endClientBuildMode();
			// 	if (validBuild) {
			// 		playerBuildEvent.Invoke();
			// 		flywayManagerSO.edgeUpdateRequest.Invoke();
			// 		GameEvent gameEvent = new GameEvent(GameEventType.Build, getCurrentPlayer().getName() + " has built a new flyway!");
			// 		playerEvent.Invoke(gameEvent);

			// 		if (!inSetupPhase()) getCurrentPlayer().buildFlyway();

			// 		// if in setup mode, indicate that the player has finished their turn
			// 		if (isClientPlayerSetupActive()) nextSetupTurnEvent.Invoke();
			// 	}
			// 	else if (clientPlayerIsActive()) {
			// 		GameEvent gameEvent = new GameEvent(GameEventType.Build, "You do not have enough resources to build a flyway!");
			// 		playerEvent.Invoke(gameEvent);
			// 	}
			// });

			// Event that is triggered when a Hex is hacked
			// hexManagerSO.hackEvent.AddListener((uint id) => {
			// 	hexManagerSO.endHackModeEvent.Invoke();
			// 	playerHackEvent.Invoke();

			// 	GameEvent gameEvent = new GameEvent(GameEventType.Hack, getCurrentPlayer().getName() + " has hacked a territory!");
			// 	playerEvent.Invoke(gameEvent);

			// 	// re-enable GUI after hack
			// 	//if (isClientPlayerActive()) enableGUIEvent.Invoke();
			// });
			
			// player attempts to trade at depot
			// depotManagerSO.playerClickEvent.AddListener((uint id, ResourceType resource, uint amount) => {
			// 	if (!inSetupPhase())
			// 	{
			// 		if (boardManagerSO.validDepotForPlayer(clientPlayerManager.getId(), id) && clientPlayerIsActive()) depotTradeEvent.Invoke(new DepotTradeRequest(resource, amount));
			// 		else if (boardManagerSO.validDepotForPlayer(clientPlayerManager.getId(), id)) playerEvent.Invoke(new GameEvent(GameEventType.Info, "You cannot trade at this time."));
			// 		else playerEvent.Invoke(new GameEvent(GameEventType.Info, "You cannot trade at this depot."));
			// 		endClientBuildMode();
			// 	}
			// 	else
			// 	{
			// 		playerEvent.Invoke(new GameEvent(GameEventType.Info, "You cannot trade during the setup phase."));
			// 	}
			// });

			// request updated stronghold/outpost/flyway points when the hex manager finishes updating its resources
			hexManagerSO.managerResourceResponse.AddListener(() => {
				strongholdManagerSO.pointUpdateRequest.Invoke();
				flywayManagerSO.edgeUpdateRequest.Invoke();
			});

			// request updated stronghold/outpost/flyway points when the hex manager finishes updating its resources
			hexManagerSO.managerSetupResourceResponse.AddListener((List<HexResource> setupResources) => {
				for (int player = 0; player < gameManagerSO.numPlayers; player++)
				{
					int[] playerResources = boardManagerSO.getResourcesForPlayer(player, setupResources);
					gameManagerSO.playerList[player].updateResources(playerResources);
				}
				// beginFirstTurn();
			});

			// request the board manager to update player stronghold/outpost placements after the stronghold manager collects them
			strongholdManagerSO.managerPointUpdateResponse.AddListener((List<PlayerPoint> points) => boardManagerSO.updatePointsRequestEvent.Invoke(points));

			// request the board manager to update player flyway placements after the flyway manager collects them
			flywayManagerSO.managerEdgeUpdateResponse.AddListener((List<PlayerEdge> edges) => boardManagerSO.updateEdgesRequestEvent.Invoke(edges));

			// update all player resources when board manager announces that player placements have been recorded
			boardManagerSO.updatePointsResponseEvent.AddListener(() => {
				for (int player = 0; player < gameManagerSO.numPlayers; player++)
				{
					int[] playerResources = boardManagerSO.getResourcesForPlayer(player, hexManagerSO.getResources());
					gameManagerSO.playerList[player].updateResources(playerResources);
					gameManagerSO.playerList[player].numOutposts = boardManagerSO.getNumOutpostsFor(player);
					gameManagerSO.playerList[player].numStrongholds = boardManagerSO.getNumStrongholdsFor(player);
				}
			});

			// update all player flyway counts when board manager announces that player flyway placements have been recorded
			boardManagerSO.updatePointsResponseEvent.AddListener(() => {
				for (int player = 0; player < gameManagerSO.numPlayers; player++) gameManagerSO.playerList[player].numFlyways = boardManagerSO.getNumFlywaysFor(player);
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
