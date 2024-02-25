using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Synthicate
{
    [CreateAssetMenu(fileName = "GameManagerSO", menuName = "ScriptableObjects/Game Manager")]
    public class GameManagerSO : ScriptableObject
    {
        public UnityEvent<uint, List<string>> hostStartGameEvent = new UnityEvent<uint, List<string>>();
        //public UnityEvent<uint> clientStartGameEvent = new UnityEvent<uint>();
        public UnityEvent<uint> nextTurnEvent = new UnityEvent<uint>();
        public UnityEvent updateGUIEvent = new UnityEvent();
        public UnityEvent clientBuildModeEvent = new UnityEvent();
        //public UnityEvent playerBuildEvent = new UnityEvent();
        public UnityEvent playerHackEvent = new UnityEvent();
        public UnityEvent<CardType> playerCardEvent = new UnityEvent<CardType>();
        public UnityEvent<Trade> playerTradeRequestEvent = new UnityEvent<Trade>();
        public UnityEvent<Trade> targetPlayerTradeRequestEvent = new UnityEvent<Trade>();
        public UnityEvent<bool> playerTradeResponseEvent = new UnityEvent<bool>();
        public UnityEvent<bool> updateMainMenuEvent = new UnityEvent<bool>();
        public UnityEvent<uint> endSetupEvent = new UnityEvent<uint>();
        public UnityEvent nextSetupTurnEvent = new UnityEvent();
        public UnityEvent<GameEvent> playerEvent = new UnityEvent<GameEvent>();
        public UnityEvent<DepotTradeRequest> depotTradeEvent = new UnityEvent<DepotTradeRequest>();
        public UnityEvent<DepotTrade> depotTradeExecuteEvent = new UnityEvent<DepotTrade>();
        //public UnityEvent<DepotTrade> depotTradeExecuteEve = new UnityEvent<DepotTrade>();
        public UnityEvent disableGUIEvent = new UnityEvent();
        public UnityEvent enableGUIEvent = new UnityEvent();
        //public UnityEvent eventCardRequest
        public UnityEvent hackerRollEvent = new UnityEvent();
        public UnityEvent debugGiveResourcesEvent = new UnityEvent();
        public UnityEvent playerBuildEvent = new UnityEvent();


        private PlayerManagerSO[] playerManagers;
        private PlayerManagerSO clientPlayerManager;

        public FlywayManagerScriptableObject flywayManager;
        public StrongholdManagerScriptableObject strongholdManager;
        public HexManagerScriptableObject hexManager;
        public DepotManagerScriptableObject depotManager;
        public BoardManagerSO boardManager;
        public AudioManagerSO audioManager;

        private uint currentTurn = 0;
        private uint currentSetupTurn = 0;
        private uint numPlayers = Global.MAX_PLAYERS;
        private uint diceValue;
        private Trade pendingTrade;
        private uint HACKER_NUM = 7;

        public enum GameState { Inactive, Setup_0, Setup_1, Active, BuildingFlyway, BuildingStronghold, BuildingOutpost, TradingPlayer, TradingDepot, Hacking }
        public GameState state = GameState.Inactive;

        private void OnEnable()
        {
            // current player builds a stronghold
            strongholdManager.playerBuildEvent.AddListener((bool validBuild) => {
                endClientBuildMode();
                if (validBuild)
                {
                    playerBuildEvent.Invoke();
                    strongholdManager.pointUpdateRequest.Invoke();
                    GameEvent gameEvent = new GameEvent(GameEventType.Build, getCurrentPlayer().getName() + " has built a new stronghold or outpost!");
                    playerEvent.Invoke(gameEvent);

                    if (!inSetupPhase()) getCurrentPlayer().buildStronghold();

                    // if in setup mode, let the client build a flyway now
                    if (isClientPlayerSetupActive()) beginClientSetupFlywayBuildMode();
                }
                else if (clientPlayerIsActive())
                {
                    GameEvent gameEvent = new GameEvent(GameEventType.Build, "You do not have enough resources to build an outpost or stronghold here!");
                    playerEvent.Invoke(gameEvent);
                }
            });

            // current player builds a flyway
            flywayManager.playerBuildEvent.AddListener((bool validBuild) => {
                // TODO: We need to send this response back to the server so that it can command the change on all clients
                // NOTE: This may already be happening if the stronghold point triggers the event across the network...
                endClientBuildMode();
                if (validBuild) {
                    playerBuildEvent.Invoke();
                    flywayManager.edgeUpdateRequest.Invoke();
                    GameEvent gameEvent = new GameEvent(GameEventType.Build, getCurrentPlayer().getName() + " has built a new flyway!");
                    playerEvent.Invoke(gameEvent);

                    if (!inSetupPhase()) getCurrentPlayer().buildFlyway();

                    // if in setup mode, indicate that the player has finished their turn
                    if (isClientPlayerSetupActive()) nextSetupTurnEvent.Invoke();
                }
                else if (clientPlayerIsActive()) {
                    GameEvent gameEvent = new GameEvent(GameEventType.Build, "You do not have enough resources to build a flyway!");
                    playerEvent.Invoke(gameEvent);
                }
            });

            // Event that is triggered when a Hex is hacked
            hexManager.hackEvent.AddListener((uint id) => {
                hexManager.endHackModeEvent.Invoke();
                playerHackEvent.Invoke();

                GameEvent gameEvent = new GameEvent(GameEventType.Hack, getCurrentPlayer().getName() + " has hacked a territory!");
                playerEvent.Invoke(gameEvent);

                // re-enable GUI after hack
                //if (isClientPlayerActive()) enableGUIEvent.Invoke();
            });
            // player attempts to trade at depot
            depotManager.playerClickEvent.AddListener((uint id, ResourceType resource, uint amount) => {
                if (!inSetupPhase())
                {
                    if (boardManager.validDepotForPlayer(clientPlayerManager.getId(), id) && clientPlayerIsActive()) depotTradeEvent.Invoke(new DepotTradeRequest(resource, amount));
                    else if (boardManager.validDepotForPlayer(clientPlayerManager.getId(), id)) playerEvent.Invoke(new GameEvent(GameEventType.Info, "You cannot trade at this time."));
                    else playerEvent.Invoke(new GameEvent(GameEventType.Info, "You cannot trade at this depot."));
                    endClientBuildMode();
                }
                else
                {
                    playerEvent.Invoke(new GameEvent(GameEventType.Info, "You cannot trade during the setup phase."));
                }
            });

            // request updated stronghold/outpost/flyway points when the hex manager finishes updating its resources
            hexManager.managerResourceResponse.AddListener(() => {
                strongholdManager.pointUpdateRequest.Invoke();
                flywayManager.edgeUpdateRequest.Invoke();
            });

            // request updated stronghold/outpost/flyway points when the hex manager finishes updating its resources
            hexManager.managerSetupResourceResponse.AddListener((List<HexResource> setupResources) => {
                for (uint player = 0; player < numPlayers; player++)
                {
                    uint[] playerResources = boardManager.getResourcesForPlayer(player, setupResources);
                    playerManagers[player].updateResources(playerResources);
                }
                beginFirstTurn();
            });

            // request the board manager to update player stronghold/outpost placements after the stronghold manager collects them
            strongholdManager.managerPointUpdateResponse.AddListener((List<PlayerPoint> points) => boardManager.updatePointsRequestEvent.Invoke(points));

            // request the board manager to update player flyway placements after the flyway manager collects them
            flywayManager.managerEdgeUpdateResponse.AddListener((List<PlayerEdge> edges) => boardManager.updateEdgesRequestEvent.Invoke(edges));

            // update all player resources when board manager announces that player placements have been recorded
            boardManager.updatePointsResponseEvent.AddListener(() => {
                for (uint player = 0; player < numPlayers; player++)
                {
                    uint[] playerResources = boardManager.getResourcesForPlayer(player, hexManager.getResources());
                    playerManagers[player].updateResources(playerResources);
                    playerManagers[player].numOutposts = boardManager.getNumOutpostsFor(player);
                    playerManagers[player].numStrongholds = boardManager.getNumStrongholdsFor(player);
                }
            });

            // update all player flyway counts when board manager announces that player flyway placements have been recorded
            boardManager.updatePointsResponseEvent.AddListener(() => {
                for (uint player = 0; player < numPlayers; player++) playerManagers[player].numFlyways = boardManager.getNumFlywaysFor(player);
            });

        } // END onEnable()


        /// <summary>
        /// Tell the host's game manager to start the game and send the message to the clients
        /// </summary>
        /// <param name="numPlayers">test</param>
        public void hostStartGame(uint numPlayers, List<string> playerNames) => hostStartGameEvent.Invoke(numPlayers, playerNames);
        /// <summary>
        /// The clients tell all game objects that the game has started
        /// </summary>
        /// <param name="numPlayers"></param>
        public void clientStartGame(uint[] clientIds, ulong localClientId, string[] playerNames)
        {
            this.numPlayers = (uint)clientIds.Length;
            state = GameState.Setup_0;
            currentSetupTurn = 0;
            currentTurn = 0;
            diceValue = 0;

            playerManagers = new PlayerManagerSO[numPlayers];

            for (int i = 0; i < numPlayers; i++)
            {
                playerManagers[i] = CreateInstance("PlayerManagerSO") as PlayerManagerSO;
                playerManagers[i].setName(playerNames[i]);
                playerManagers[i].setId(clientIds[i]);
                playerManagers[i].init();
            }

            clientPlayerManager = playerManagers[localClientId];

            if (clientPlayerManager.getId() == currentSetupTurn)
                beginClientSetupStrongholdBuildMode();

            updateGUIEvent.Invoke();
            updateMainMenuEvent.Invoke(false);
        } // END clientStartGame()

        public bool clientReady() => clientPlayerManager != null;
        public string getClientName() => clientPlayerManager.getName();
        public uint getClientId() => clientPlayerManager.getId();
        public uint[] getClientResources() => clientPlayerManager.getResourceCounts();
        public bool inSetupPhase() => state == GameState.Setup_0 || state == GameState.Setup_1;
        public uint getGUITurn() => currentTurn + 1;
        public bool clientPlayerIsActive() => currentTurn == clientPlayerManager.getId() && state == GameState.Active;
        public bool isClientPlayerSetupActive() => currentSetupTurn == clientPlayerManager.getId() && inSetupPhase();
        public uint getPublicInfluencePointsFor(uint player) => playerManagers[player].getPublicInfluencePoints();
        public string getPlayerName(uint player) => playerManagers[player].getName();
        public uint rollDice() => (uint)(Random.Range(1, 7) + Random.Range(1, 7));
        public uint getDiceValue() => diceValue;
        public PlayerManagerSO getCurrentPlayer() => playerManagers[currentTurn];
        public uint getNumClientHackers() => clientPlayerManager.getNumHackers();
        public uint getNumClientInfluencePointCards() => clientPlayerManager.getNumInfleuncePointCards();
        public uint getNumPlayers() => numPlayers;
        public bool clientHasEnough(ResourceType resource, uint amount) => clientPlayerManager.resources[(uint)resource] >= amount;

        //public string getPlayerName(uint i) => playerManagers[i].getName();

        // Let all clients know that a new turn has begun
        public void nextTurn() => nextTurnEvent.Invoke(rollDice());

        public void beginClientSetupStrongholdBuildMode()
        {
            // identify which strongholds can be built by the current player

            // invoke the stronghold manager event with the current client and the list of possible locations
            List<uint> buildPoints = inSetupPhase() ? boardManager.getValidSetupPointsFor() : boardManager.getValidPointsFor(getClientId());
            BuildPermissions playerBuildPermissions = new BuildPermissions(false, true, false);
            strongholdManager.beginBuildModeForPlayer(getClientId(), buildPoints, playerBuildPermissions);
        }

        public void beginClientSetupFlywayBuildMode()
        {
            List<uint> buildEdges = boardManager.getValidEdgesFor(getClientId());
            BuildPermissions playerBuildPermissions = new BuildPermissions(true, false, false);
            flywayManager.beginBuildModeForPlayer(clientPlayerManager.getId(), buildEdges, playerBuildPermissions);
        }



        public void nextSetupTurn()
        {
            if (state == GameState.Setup_0)
            {
                if (currentSetupTurn != numPlayers - 1) currentSetupTurn++;
                else state = GameState.Setup_1;
            }
            else if (state == GameState.Setup_1)
            {
                if (currentSetupTurn != 0) currentSetupTurn--;
                else
                {
                    endSetupEvent.Invoke(rollDice());
                    //endSetupEvent.Invoke(7); //DEBUG INITIAL HACKER ROLL
                    return;
                }
            }
            if (clientPlayerManager.getId() == currentSetupTurn && inSetupPhase())
            {
                beginClientSetupStrongholdBuildMode();
            }
            updateGUIEvent.Invoke();
        } // END nextSetupTurn()

        public void endSetup(uint diceValue)
        {
            state = GameState.Active;
            this.diceValue = diceValue;
            hexManager.setupResourceRequest.Invoke();

            // TODO: get resources from hexes and distribute to players (2 turns worth of materials for every hex)
            //hexManager
            //hexManager.resourceRequest.Invoke(diceValue);
            //hexManager.resourceRequest.Invoke(diceValue);
            //hexManager.setupResourceRequest.Invoke();
            //List<HexResource> setupHexResources = hexManager.getSetupResources();
            //for (int i = 0; i < numPlayers; i++)
            //    boardManager.getResourcesForPlayer(clientPlayerManager.getId(), setupHexResources);



        } // END endSetup()

        public void beginFirstTurn()
        {
            hexManager.hexSelectionEvent.Invoke(diceValue);
            hexManager.resourceRequest.Invoke(diceValue);
            updateGUIEvent.Invoke();

            if (diceValue == HACKER_NUM)
            {
                GameEvent gameEvent = new GameEvent(GameEventType.Hack, getCurrentPlayer().getName() + " hashed a 7! The hacker will be placed.");
                playerEvent.Invoke(gameEvent);

                hackerRollEvent.Invoke();
                
                // let active player choose a spot to hack
                if (clientPlayerIsActive()) hexManager.beginHackModeEvent.Invoke();
            }
            else
            {
                GameEvent gameEvent = new GameEvent(GameEventType.Hack, "The game has begun! " + getCurrentPlayer().getName() + " hashed a " + diceValue);
                playerEvent.Invoke(gameEvent);
            }

        } // END beginFirstTurn()

        public void initNextTurn(uint diceValue)
        {
            currentTurn = (currentTurn + 1) % numPlayers;
            //diceValue = 7; //DEBUG HACKER EVENT
            this.diceValue = diceValue;
            if (diceValue == HACKER_NUM)
            {
                hackerRollEvent.Invoke();

                GameEvent gameEvent = new GameEvent(GameEventType.Hack, getCurrentPlayer().getName() + " hashed a 7! The hacker will be placed.");
                playerEvent.Invoke(gameEvent);

                // remove half resources from all players with excess
                for (int i = 0; i < numPlayers; i++)
                {
                    if (playerManagers[i].hasExcess())
                    {
                        playerManagers[i].randomlyRemoveHalf();
                        GameEvent hackGameEvent = new GameEvent(GameEventType.Hack, playerManagers[i].getName() + " lost half of their resources to hackers.");
                        playerEvent.Invoke(hackGameEvent);
                    }
                }

                // let active player choose a spot to hack
                if (clientPlayerIsActive()) hexManager.beginHackModeEvent.Invoke();

                // TODO: let active player pick a target to remove resources from
            }
            else
            {
                GameEvent gameEvent = new GameEvent(GameEventType.Hack, getCurrentPlayer().getName() + " hashed a " + diceValue);
                playerEvent.Invoke(gameEvent);

                hexManager.hexSelectionEvent.Invoke(diceValue);
                hexManager.resourceRequest.Invoke(diceValue);
                updateGUIEvent.Invoke();
            }
        }

        public void beginClientBuildMode()
        {
            BuildPermissions playerBuildPermissions = new BuildPermissions(clientPlayerManager.canBuildFlyway(), clientPlayerManager.canBuildOutpost(), clientPlayerManager.canBuildStronghold());
            strongholdManager.beginBuildModeForPlayer(getClientId(), boardManager.getValidPointsFor(getClientId()), playerBuildPermissions);
            flywayManager.beginBuildModeForPlayer(getClientId(), boardManager.getValidEdgesFor(getClientId()), playerBuildPermissions);
        }
        public void endClientBuildMode()
        {
            strongholdManager.endBuildMode();
            flywayManager.endBuildMode();
        }

        public void beginClientHackMode()
        {
            if (clientPlayerManager.canUseHacker())
            {
                //disableGUIEvent.Invoke();
                clientPlayerManager.useHacker();
                hexManager.beginHackModeEvent.Invoke();
            }
            else
            {
                GameEvent invalidHackRequest = new GameEvent(GameEventType.Info, "You do not have any hackers.");
                playerEvent.Invoke(invalidHackRequest);
            }
        }

        public void clientRequestDepotTrade(DepotTrade trade)
        {
            if (clientPlayerManager.getResourceCount(trade.offeredResource) >= trade.offeredAmount)
            {
                depotTradeExecuteEvent.Invoke(trade);
            }
            else
            {
                GameEvent invalidTrade = new GameEvent(GameEventType.Info, "Insufficient resources for this trade.");
                playerEvent.Invoke(invalidTrade);
            }
        }

        public void executeDepotTrade(DepotTrade trade)
        {
            getCurrentPlayer().resources[(int)trade.offeredResource] -= trade.offeredAmount;
            getCurrentPlayer().resources[(int)trade.requestedResource] += 1;
            GameEvent invalidTrade = new GameEvent(GameEventType.Trade, getCurrentPlayer().getName() + " performed a trade at a depot.");
            playerEvent.Invoke(invalidTrade);
        }

        public void clientRequestEventCard()
        {
            if (clientPlayerManager.canBuyCard())
            {
                clientPlayerManager.buyEventCard();

                uint cardType = (uint)Random.Range(0, 2);

                if (cardType == 1)
                {
                    GameEvent cardEvent = new GameEvent(GameEventType.Influence, "You recieved an influence point card!");
                    playerEvent.Invoke(cardEvent);
                }
                else
                {
                    GameEvent cardEvent = new GameEvent(GameEventType.Hack, "You recieved a hacker!");
                    playerEvent.Invoke(cardEvent);
                }
                playerCardEvent.Invoke((CardType)cardType);
            }
            else
            {
                GameEvent cardEvent = new GameEvent(GameEventType.Info, "You cannot buy any event cards!");
                playerEvent.Invoke(cardEvent);
            }

        }
        public void addPlayerCard(CardType cardType)
        {
            switch (cardType)
            {
                case CardType.Hacker: clientPlayerManager.addHacker(); break;
                case CardType.Influence: clientPlayerManager.addInfluencePoint(); break;
            }
            GameEvent cardEvent = new GameEvent(GameEventType.Card, getCurrentPlayer().getName() + " has acquired an event card.");
            playerEvent.Invoke(cardEvent);
        }

        // DEBUG: Give one of all resources to the active player
        public void debugGiveResources()
        {
            getCurrentPlayer().debugIncrementAllResources();
        }

        public void requestTrade(Trade tradeReq)
        {
            pendingTrade = tradeReq;

            targetPlayerTradeRequestEvent.Invoke(tradeReq);

            GameEvent tradeEvent = new GameEvent(GameEventType.Trade, getCurrentPlayer() + " requested a trade with " + playerManagers[pendingTrade.targetPlayer].getName());
            playerEvent.Invoke(tradeEvent);
        }

        public void tradeResponse(bool response)
        {
            //
            if (response)
            {
                getCurrentPlayer().resources[(int)pendingTrade.offeredResource] -= pendingTrade.offeredAmount;
                playerManagers[pendingTrade.targetPlayer].resources[(int)pendingTrade.requestedResource] -= pendingTrade.requestedAmount;

                getCurrentPlayer().resources[(int)pendingTrade.requestedResource] += pendingTrade.offeredAmount;
                playerManagers[pendingTrade.targetPlayer].resources[(int)pendingTrade.offeredResource] += pendingTrade.requestedAmount;

                GameEvent tradeEvent = new GameEvent(GameEventType.Trade, getCurrentPlayer() + " traded with " + playerManagers[pendingTrade.targetPlayer].getName());
                playerEvent.Invoke(tradeEvent);
            }
            else
            {
                GameEvent tradeEvent = new GameEvent(GameEventType.Trade, playerManagers[pendingTrade.targetPlayer].getName() + " rejected the trade request from " + getCurrentPlayer());
                playerEvent.Invoke(tradeEvent);
            }
        }


    }
}