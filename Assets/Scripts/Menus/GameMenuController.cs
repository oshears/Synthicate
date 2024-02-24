using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Synthicate
{
    public class GameMenuController : MonoBehaviour
    {

        enum GameMenuState { Disabled, Setup, Inactive, Active, MenuOpen, BuildMode, HackMode, TradeRequest, TradePending, TradeOffer, DepotTradeMenu, PauseMenu, InvalidTrade }
        GameMenuState state = GameMenuState.Disabled;
        GameMenuState prePauseState;

        public GameManagerSO gameManagerSO;
        public GUISkin guiskin;

        int tradeRequestSelection = 0;
        float tradeRequestSlider = 1f;
        int tradeOfferSelection = 0;
        float tradeOfferSlider = 1f;
        int tradePlayerSelection = 0;

        Queue<GameEvent> eventQueue = new Queue<GameEvent>();
        GameEvent displayedEvent;
        float eventDisplayCounter = 0;

        DepotTradeRequest depotTradeReq;
        Trade currentTrade;

        // Start is called before the first frame update
        void Start()
        {
            gameManagerSO.endSetupEvent.AddListener(initPlayerUI);
            gameManagerSO.updateGUIEvent.AddListener(resetGUIState);
            gameManagerSO.playerEvent.AddListener((GameEvent gameEvent) => {
                eventQueue.Enqueue(gameEvent);
                //resetGUIState(); // DEBUG: I don't think this needs to be here
            });
            //gameManagerSO.playerHackEvent.AddListener(resetGUIState);
            gameManagerSO.depotTradeEvent.AddListener((DepotTradeRequest req) =>
            {
                depotTradeReq = req;
                state = GameMenuState.DepotTradeMenu;
            });

            //gameManagerSO.trade
            gameManagerSO.targetPlayerTradeRequestEvent.AddListener((Trade req) =>
            {
                currentTrade = req;
                state = GameMenuState.TradeOffer;
            });

            gameManagerSO.hackerRollEvent.AddListener(() =>
            {
                if (gameManagerSO.clientPlayerIsActive()) state = GameMenuState.HackMode;
                else state = GameMenuState.Inactive;
            });

            gameManagerSO.playerHackEvent.AddListener(() =>
            {
                resetGUIState();
            });

            gameManagerSO.playerBuildEvent.AddListener(() =>
            {
                resetGUIState();
            });

            
        }

        // Update is called once per frame
        void Update()
        {
            if (state == GameMenuState.Disabled) return;

            if (eventDisplayCounter <= 0 && eventQueue.Count > 0)
            {
                displayedEvent = eventQueue.Dequeue();
                eventDisplayCounter = 3;
            }
            else if (eventDisplayCounter > 0) eventDisplayCounter -= Time.deltaTime;

            processPauseButton(Input.GetKeyDown(KeyCode.Escape));
        }

        void processPauseButton(bool pressed)
        {
            if (!pressed) return;

            if (state != GameMenuState.PauseMenu)
            {
                prePauseState = state;
                state = GameMenuState.PauseMenu;
            }
            else if (prePauseState == GameMenuState.Inactive)
            {
                resetGUIState();
            }
            else
            {
                state = prePauseState;
            }
        }

        void resetGUIState()
        {
            if (gameManagerSO.clientPlayerIsActive())
               state = GameMenuState.Active;
            else if (gameManagerSO.isClientPlayerSetupActive())
                state = GameMenuState.Setup;
            else
                state = GameMenuState.Inactive;
        }


        void initPlayerUI(uint diceValue)
        {
            //Debug.
            state = GameMenuState.Inactive;

            resetGUIState();

            //this.playerManager = playerManager;
        }
       
        private void OnGUI()
        {
            if (state == GameMenuState.Disabled) return;

            GUI.skin = guiskin;

            int screenBlockWidth = Screen.width / 16;
            int screenBlockHeight = Screen.height / 16;

            // Entire Screen Area
            Rect screenArea = new Rect(0, 0, Screen.width, Screen.height);

            // Game Menu Area for Player Actions
            Rect gameMenuArea = new Rect(0, 14 * screenBlockHeight, 2 * screenBlockWidth, 2 * screenBlockHeight);

            // Expanded Game Menu Area for Player Actions
            Rect expandedGameMenu = new Rect(0, 11 * screenBlockHeight, 2 * screenBlockWidth, 5 * screenBlockHeight);

            // Expanded Game Menu Area for Player Actions
            Rect instructionArea = new Rect(0, 11 * screenBlockHeight, 4 * screenBlockWidth, 5 * screenBlockHeight);

            // Status Area Displaying Player Resources
            Rect playerResourcesArea = new Rect(13 * screenBlockWidth, 0, 3 * screenBlockWidth, 5 * screenBlockHeight);

            // Status Area Displaying Players' Influnece Points
            Rect playerStatusArea = new Rect(0,0,3 * screenBlockWidth,5 * screenBlockHeight);

            // Pop up area for event cards and general events
            Rect tradeEventArea = new Rect(6 * screenBlockWidth, 6 * screenBlockHeight, 4 * screenBlockWidth, 4 * screenBlockHeight);
            Rect playerEventArea = new Rect(11 * screenBlockWidth, 14 * screenBlockHeight, 5 * screenBlockWidth, screenBlockHeight);

            // Trade Menu Area
            Rect tradeMenuArea = new Rect(6 * screenBlockWidth, 3 * screenBlockHeight, 4 * screenBlockWidth, 7 * screenBlockHeight);

            // DEBUG AREA
            Rect debugArea = new Rect(6*screenBlockWidth,15 * screenBlockHeight, 3 * screenBlockWidth, screenBlockHeight);

            // Stop if game manager is not loaded
            if (!gameManagerSO.clientReady()) return;

            // Player Resource Area Always Gets Drawn
            GUI.Box(playerResourcesArea, "");
            GUILayout.BeginArea(playerResourcesArea);
            GUILayout.Label(gameManagerSO.getClientName());
            uint[] clientResources = gameManagerSO.getClientResources();
            for(int i = 0; i < Global.NUM_RESOURCE_TYPES; i++) GUILayout.Label(((ResourceType) i) + ": " + clientResources[i]);
            GUILayout.Label("Hackers Available: "  + gameManagerSO.getNumClientHackers());
            GUILayout.Label("Influnece Point Cards: "  + gameManagerSO.getNumClientInfluencePointCards());
            GUILayout.EndArea();

            // Player Status Area Always Gets Drawn
            GUI.Box(playerStatusArea, "");
            GUILayout.BeginArea(playerStatusArea);
            GUILayout.Label("Current Turn: " + gameManagerSO.getCurrentPlayer().getName());
            GUILayout.Label("Current Hash Value: " + gameManagerSO.getDiceValue());
            for(uint i = 0; i < gameManagerSO.getNumPlayers(); i++)
                GUILayout.Label(gameManagerSO.getPlayerName(i) + "'s Influence Points: " + gameManagerSO.getPublicInfluencePointsFor(i));
            GUILayout.EndArea();

            // DEBUG AREA
            GUILayout.BeginArea(debugArea);
            if (GUILayout.Button("DEBUG: GIVE RESOURCES"))
                gameManagerSO.debugGiveResourcesEvent.Invoke();
            GUILayout.EndArea();

            // display current event if available
            if (eventDisplayCounter > 0)
            {
                GUI.Box(playerEventArea, displayedEvent.message);
            }

            if (state == GameMenuState.Inactive)
            {
                //GUI.Box(gameMenuArea, "");
            }
            else if (state == GameMenuState.Setup)
            {
                GUI.Box(instructionArea, "");
                GUILayout.BeginArea(instructionArea);
                GUILayout.Label("Place a outpost on the board, then place a flyway on the board.");
                GUILayout.EndArea();
            }
            else if (state == GameMenuState.Active)
            {
                GUI.Box(gameMenuArea, "");

                // game menu box
                GUILayout.BeginArea(gameMenuArea);
                //GUILayout.Label("Current Turn: Player " + gameManagerSO.getGUITurn());
                if (GUILayout.Button("Menu")){
                    state = GameMenuState.MenuOpen;
                }
                if(GUILayout.Button("Next Turn")) gameManagerSO.nextTurn();
                GUILayout.EndArea();

                // resource box
            }
            else if (state == GameMenuState.MenuOpen)
            {
                GUI.Box(expandedGameMenu, "");

                // game menu box
                GUILayout.BeginArea(expandedGameMenu);
                //GUILayout.Label("Current Turn: Player " + gameManagerSO.getGUITurn());
                if (GUILayout.Button("Build"))
                {
                    state = GameMenuState.BuildMode;

                    gameManagerSO.beginClientBuildMode();
                }
                if (GUILayout.Button("Trade"))
                {
                    tradeRequestSelection = 0;
                    tradeRequestSlider = 1f;
                    tradeOfferSelection = 0;
                    tradeOfferSlider = 1f;
                    state = GameMenuState.TradeRequest;
                }
                if(GUILayout.Button("Event Card"))
                {
                    state = GameMenuState.Active;
                    gameManagerSO.clientRequestEventCard();
                }
                if (GUILayout.Button("Hack"))
                {
                    state = GameMenuState.HackMode;
                    gameManagerSO.beginClientHackMode();
                }
                if (GUILayout.Button("Close"))
                {
                    state = GameMenuState.Active;
                }
                if(GUILayout.Button("Next Turn")) gameManagerSO.nextTurn();
                GUILayout.EndArea();
            }
            else if (state == GameMenuState.BuildMode)
            {
                GUI.Box(gameMenuArea, "");

                GUILayout.BeginArea(gameMenuArea);
                if(GUILayout.Button("Exit Build Mode"))
                {
                    state = GameMenuState.Active;
                    gameManagerSO.endClientBuildMode();
                }
                GUILayout.EndArea();
            }
            else if (state == GameMenuState.HackMode)
            {
                GUI.Box(instructionArea, "Please select a territory to hack. This will prevent all players from acquiring resources from this territory.");
            }
            else if (state == GameMenuState.TradeRequest)
            {
                GUI.Box(tradeMenuArea, "");
                GUILayout.BeginArea(tradeMenuArea);
                GUILayout.Label("Trade Menu");
                GUILayout.Label("Target Player:");

                string[] playerNames = new string[gameManagerSO.getNumPlayers()];
                for(uint i = 0; i < gameManagerSO.getNumPlayers(); i++)
                    if (gameManagerSO.getClientId() != i) playerNames[i] = gameManagerSO.getPlayerName(i);
                tradePlayerSelection = GUILayout.SelectionGrid(tradePlayerSelection, playerNames, 4);
                if (tradePlayerSelection == gameManagerSO.getClientId()) tradePlayerSelection = (tradePlayerSelection + 1) % (int) gameManagerSO.getNumPlayers();
                
                GUILayout.Label("Trade Offer:");
                tradeOfferSelection = GUILayout.SelectionGrid(tradeOfferSelection, new string[] { "People", "Power", "Mech", "Metal", "Food" }, 5);
                GUILayout.Label("Amount Offered: " + tradeOfferSlider);
                tradeOfferSlider = Mathf.RoundToInt(GUILayout.HorizontalSlider(tradeOfferSlider, 0, 10));
                GUILayout.Label("Trade Request:");
                tradeRequestSelection = GUILayout.SelectionGrid(tradeRequestSelection, new string[] { "People", "Power", "Mech", "Metal", "Food" }, 5);
                GUILayout.Label("Amount Requested: "+ tradeRequestSlider);
                tradeRequestSlider = Mathf.RoundToInt(GUILayout.HorizontalSlider(tradeRequestSlider, 0, 10));
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Request"))
                {
                    if (gameManagerSO.clientHasEnough((ResourceType)tradeRequestSelection, (uint) tradeRequestSlider))
                    {
                        state = GameMenuState.TradeOffer;

                        currentTrade = new Trade((uint)tradePlayerSelection,
                                                        (ResourceType)tradeRequestSelection,
                                                        (uint)tradeRequestSlider,
                                                        gameManagerSO.getClientId(),
                                                        (ResourceType)tradeOfferSelection,
                                                        (uint)tradeOfferSlider);

                        gameManagerSO.playerTradeRequestEvent.Invoke(currentTrade);
                        state = GameMenuState.TradePending;
                    }
                    else
                    {
                        state = GameMenuState.InvalidTrade;
                    }
                }
                if (GUILayout.Button("Cancel"))
                {
                    state = GameMenuState.Active;
                }
                GUILayout.EndHorizontal();
                GUILayout.EndArea();
            }
            else if (state == GameMenuState.TradePending)
            {
                GUI.Box(tradeEventArea, "");
                GUILayout.BeginArea(tradeEventArea);
                GUILayout.Label("Trade Pending:");
                GUILayout.Label("Waiting on response from: "+gameManagerSO.getPlayerName(currentTrade.targetPlayer));
                GUILayout.BeginHorizontal();
                GUILayout.Label("Offering:");
                GUILayout.Label("Requesting:");
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                GUILayout.Label(currentTrade.offeredAmount + " " + currentTrade.offeredResource);
                GUILayout.Label(currentTrade.requestedAmount + " " + currentTrade.requestedResource);
                GUILayout.EndHorizontal();
                
                GUILayout.EndArea();
            }
            else if (state == GameMenuState.TradeOffer)
            {
                GUI.Box(tradeEventArea, "");

                GUILayout.BeginArea(tradeEventArea);
                GUILayout.Label("Incoming Trade Offer:");
                GUILayout.Label(gameManagerSO.getPlayerName(currentTrade.offeringPlayer) + " would like to trade:");
                GUILayout.BeginHorizontal();
                GUILayout.Label("Offering:");
                GUILayout.Label("Requesting:");
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                GUILayout.Label(currentTrade.offeredAmount + " " + currentTrade.offeredResource);
                GUILayout.Label(currentTrade.requestedAmount + " " + currentTrade.requestedResource);
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Accept"))
                {
                    if (gameManagerSO.clientHasEnough(currentTrade.requestedResource, currentTrade.requestedAmount))
                    {
                        gameManagerSO.playerTradeResponseEvent.Invoke(true);
                        state = GameMenuState.Inactive;
                    }
                    else
                    {
                        state = GameMenuState.InvalidTrade;
                    }
                    
                }
                if (GUILayout.Button("Reject"))
                {
                    gameManagerSO.playerTradeResponseEvent.Invoke(false);
                    state = GameMenuState.Inactive;
                }
                GUILayout.EndHorizontal();
                GUILayout.EndArea();
            }
            else if (state == GameMenuState.DepotTradeMenu)
            {
                GUI.Box(tradeMenuArea, "");

                GUILayout.BeginArea(tradeMenuArea);
                GUILayout.Label("Depot Trade Menu");
                if (depotTradeReq.requestedResource == ResourceType.Any)
                {
                    GUILayout.Label("Trade 3 of any resource at this depot to get any one resource.");
                    tradeOfferSelection = GUILayout.SelectionGrid(tradeOfferSelection, new string[] { "People", "Power", "Mech", "Metal", "Food" }, 5);
                }
                else
                {
                    GUILayout.Label("Trade 2 " + depotTradeReq.requestedResource + " resources at this depot to get any one resource.");
                }
                GUILayout.Label("Requested Resource: ");
                tradeRequestSelection = GUILayout.SelectionGrid(tradeRequestSelection, new string[] { "People", "Power", "Mech", "Metal", "Food" }, 5);
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Trade"))
                {
                    if ((depotTradeReq.requestedResource == ResourceType.Any && gameManagerSO.clientHasEnough((ResourceType)tradeOfferSelection, depotTradeReq.amount)) ||
                        (depotTradeReq.requestedResource != ResourceType.Any && gameManagerSO.clientHasEnough(depotTradeReq.requestedResource, depotTradeReq.amount)))
                    {
                        DepotTrade trade = new DepotTrade((ResourceType)tradeRequestSelection, depotTradeReq.requestedResource == ResourceType.Any ? (ResourceType)tradeOfferSelection : depotTradeReq.requestedResource, depotTradeReq.amount);
                        gameManagerSO.clientRequestDepotTrade(trade);
                        state = GameMenuState.Active;
                    }
                    else
                    {
                        state = GameMenuState.InvalidTrade;
                    }
                }
                if (GUILayout.Button("Cancel"))
                {
                    state = GameMenuState.Active;
                }
                GUILayout.EndHorizontal();
                GUILayout.EndArea();
            }
            else if (state == GameMenuState.InvalidTrade)
            {
                GUI.Box(tradeMenuArea, "");
                GUILayout.BeginArea(tradeMenuArea);
                GUILayout.Label("You do not have sufficient resources for this trade!");
                if (GUILayout.Button("Ok"))
                {
                    state = gameManagerSO.clientPlayerIsActive() ? GameMenuState.Active : GameMenuState.TradeOffer;
                }
                GUILayout.EndArea();
            }
            else if (state == GameMenuState.PauseMenu)
            {
                GUI.Box(screenArea, "");

                GUILayout.BeginArea(tradeMenuArea);

                GUILayout.Label("Paused");

                processPauseButton(GUILayout.Button("Resume"));

                GUILayout.Label("Enable Bloom:");
                GUILayout.SelectionGrid(0, new string[] { "On", "Off" },2);

                GUILayout.Label("Music Volume:");
                GUILayout.HorizontalSlider(0.5f, 0f, 1f);

                GUILayout.Label("Sound Effect Volume:");
                GUILayout.HorizontalSlider(0.5f, 0f, 1f);

                if (GUILayout.Button("Exit Game"))
                {
                    Application.Quit();
                }
                GUILayout.EndArea();
            }

        }
    }
}
