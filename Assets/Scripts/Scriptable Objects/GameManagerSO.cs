using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Synthicate
{
	[CreateAssetMenu(fileName = "GameManagerSO", menuName = "ScriptableObjects/Game Manager")]
	public class GameManagerSO : ScriptableObject
	{
		
		public List<Player> playerList;
		public Player clientPlayer;
		
		public UnityEvent<GameEvent> playerEvent;
		public UnityEvent playerBuildEvent;
		
		public UnityEvent startNextTurnEvent;
		public int currentPlayerTurn {get; private set;} = 0;
		
		[SerializeField]
		public BoardManagerSO boardManagerSO;
		
		[SerializeField]
		public HexManagerScriptableObject hexManagerScriptableObject;

		public void Initialize()
		{
			playerList = new List<Player>();
		}
		public void InitializeWithPlayerNames(string[] playerNames)
		{
			playerList = new List<Player>();
			for(int i = 0; i < playerNames.Length; i++)
			{
				playerList.Add(new Player(playerNames[i], i));
			}
		}
		
		public void AddPlayer(Player newPlayer)
		{
			playerList.Add(newPlayer);
		}
		
		public int GetNumPlayers()
		{
			return playerList.Count;
		}
		
		public bool IsClientTurn()
		{
			return currentPlayerTurn == clientPlayer.GetId();
		}
		 
		public void SetClientPlayer(int playerId)
		{
			clientPlayer = playerList[playerId];
		}
		
		public int RollDice() => Random.Range(1, 7) + Random.Range(1, 7);
		
		public void OnStartNextTurn()
		{
			startNextTurnEvent.Invoke();
		}
		
		public void OnDebugIncrementAlltResources()
		{
			clientPlayer.debugIncrementAllResources();
		}
		
		public bool DoneSetupPhase()
		{
			int playerCount = playerList.Count;
			int outpostsBuilt = 0;
			int flywaysBuilt = 0;
			for(int i = 0; i < playerCount; i++)
			{
				outpostsBuilt += playerList[i].numOutposts;
				flywaysBuilt += playerList[i].numFlyways;
			}
			return (outpostsBuilt == playerCount * 2) && (outpostsBuilt == playerCount * 2);
		}
		
		public int IncrementAndGetNextPlayerIndex()
		{
			currentPlayerTurn = (currentPlayerTurn + 1) % playerList.Count;
			return currentPlayerTurn;
		}
		
		public void UpdatePlayerBuildCounts()
		{
			for (int player = 0; player < GetNumPlayers(); player++){
				// int[] playerResources = boardManagerSO.getResourcesForPlayer(player, hexManagerSO.getResources());
				// playerList[player].updateResources(playerResources);
				playerList[player].numOutposts = boardManagerSO.GetNumOutpostsFor(player);
				playerList[player].numStrongholds = boardManagerSO.GetNumStrongholdsFor(player);
			}
		}
		
		/// <summary>
		/// Get the Player object for the player who's turn it is.
		/// </summary>
		/// <returns></returns>
		public Player GetCurrentPlayer()
		{
			return playerList[currentPlayerTurn];
		}
		
		
		// public void OnPlayerBuiltOutpost()
		// {
		// 	// playerList[currentPlayerTurn].numOutposts++;
		// 	for (int player = 0; player < GetNumPlayers(); player++){
		// 		int[] playerResources = boardManagerSO.getResourcesForPlayer(player, hexManagerSO.getResources());
		// 		playerList[player].updateResources(playerResources);
		// 		playerList[player].numOutposts = boardManagerSO.getNumOutpostsFor(player);
		// 		playerList[player].numStrongholds = boardManagerSO.getNumStrongholdsFor(player);
		// 	}
		// }
		// public void OnPlayerBuiltFlyway()
		// {
		// 	playerList[currentPlayerTurn].numFlyways++;
		// }
		
	}
}
