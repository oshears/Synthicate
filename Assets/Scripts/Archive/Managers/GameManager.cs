using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

namespace Synthicate
{
    public class GameManager : NetworkBehaviour
    {
        public GameManagerSO gameManagerSO;

        // Start is called before the first frame update
        void Start()
        {
            gameManagerSO.hostStartGameEvent.AddListener((uint numPlayers, List<string> playerNames) => {
                uint[] clientIds = new uint[numPlayers];
                for (int i = 0; i < numPlayers; i++) clientIds[i] = (uint)NetworkManager.ConnectedClientsIds[i];
                initClientSOsClientRpc(clientIds,new NetworkStringArray(playerNames.ToArray()));
            });

            gameManagerSO.nextTurnEvent.AddListener((uint diceValue) => {
                if (NetworkManager.Singleton.IsServer) incrementTurnClientRpc(diceValue);
                else requestIncrementTurnServerRpc(diceValue);
            });

            gameManagerSO.nextSetupTurnEvent.AddListener(() => {
                if (NetworkManager.Singleton.IsServer) setupTurnUpdateClientRpc();
                else requestSetupTurnUpdateServerRpc();
            });

            gameManagerSO.endSetupEvent.AddListener((uint diceValue) =>
            {
                if (NetworkManager.Singleton.IsServer) endSetupClientRpc(diceValue);
                else requestEndSetupServerRpc(diceValue);
            });

            gameManagerSO.depotTradeExecuteEvent.AddListener((DepotTrade trade) =>
            {
                if (NetworkManager.Singleton.IsServer) depotTradeClientRpc(trade);
                else requestDepotTradeServerRpc(trade);
            });

            gameManagerSO.playerCardEvent.AddListener((CardType cardType) =>
            {
                if (NetworkManager.Singleton.IsServer) addPlayerCardClientRpc(cardType);
                else requestAddPlayerCardServerRpc(cardType);
            });

            gameManagerSO.debugGiveResourcesEvent.AddListener(() =>
            {
                if (NetworkManager.Singleton.IsServer) debugGiveResourcesClientRpc();
                else requestDebugGiveResourcesServerRpc();
            });

            gameManagerSO.playerTradeRequestEvent.AddListener((Trade trade) =>
            {
                if (NetworkManager.Singleton.IsServer) requestTradeClientRpc(trade);
                else requestTradeServerRpc(trade);
            });

            gameManagerSO.playerTradeResponseEvent.AddListener((bool response) =>
            {
                if (NetworkManager.Singleton.IsServer) tradeResponseClientRpc(response);
                else requestTradeResponseServerRpc(response);
            });
        }
        [ServerRpc(RequireOwnership = false)]
        void requestTradeResponseServerRpc(bool response) => tradeResponseClientRpc(response);
        [ClientRpc]
        void tradeResponseClientRpc(bool response) => gameManagerSO.tradeResponse(response);

        [ServerRpc(RequireOwnership = false)]
        void requestTradeServerRpc(Trade trade) => requestTradeClientRpc(trade);
        [ClientRpc]
        void requestTradeClientRpc(Trade trade) => gameManagerSO.requestTrade(trade);

        [ServerRpc(RequireOwnership = false)]
        void requestDebugGiveResourcesServerRpc() => debugGiveResourcesClientRpc();
        [ClientRpc]
        void debugGiveResourcesClientRpc() => gameManagerSO.debugGiveResources();


        [ServerRpc (RequireOwnership = false)]
        void requestAddPlayerCardServerRpc(CardType cardType) => addPlayerCardClientRpc(cardType);
        [ClientRpc]
        void addPlayerCardClientRpc(CardType cardType) => gameManagerSO.addPlayerCard(cardType);


        // Update is called once per frame
        void Update()
        {

        }

        [ClientRpc]
        void initClientSOsClientRpc(uint[] clientIds, NetworkStringArray playerNames) => gameManagerSO.clientStartGame(clientIds, NetworkManager.LocalClientId, playerNames.array);

        /// <summary>
        /// Server RPC to increment the turn on all clients
        /// </summary>
        [ServerRpc(RequireOwnership = false)]
        void requestIncrementTurnServerRpc(uint diceValue) => incrementTurnClientRpc(diceValue);

        [ClientRpc]
        void incrementTurnClientRpc(uint diceValue) => gameManagerSO.initNextTurn(diceValue);

        
        [ServerRpc(RequireOwnership = false)]
        void requestSetupTurnUpdateServerRpc() => setupTurnUpdateClientRpc();

        [ClientRpc]
        void setupTurnUpdateClientRpc() => gameManagerSO.nextSetupTurn();

        [ServerRpc(RequireOwnership = false)]
        void requestEndSetupServerRpc(uint diceValue) => endSetupClientRpc(diceValue);

        [ClientRpc]
        void endSetupClientRpc(uint diceValue) => gameManagerSO.endSetup(diceValue);

        [ServerRpc(RequireOwnership = false)]
        void requestDepotTradeServerRpc(DepotTrade trade) => depotTradeClientRpc(trade);
        [ClientRpc]
        void depotTradeClientRpc(DepotTrade trade) => gameManagerSO.executeDepotTrade(trade);

    }

}
