using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Synthicate
{
	[CreateAssetMenu(fileName = "GameManagerSO", menuName = "ScriptableObjects/Game Manager")]
	public class GameManagerSO : ScriptableObject
	{
		public int numPlayers {get; private set;} = 0;
		
		public List<Player> playerList;
		public Player clientPlayer;
		
		public UnityEvent<GameEvent> playerEvent;
		public UnityEvent playerBuildEvent;
		
		public UnityEvent startNextTurnEvent;
		public int currentPlayerTurn {get; private set;} = 0;

		public void Initialize()
		{
			playerList = new List<Player>();
			numPlayers = 0;
		}
		
		public void AddPlayer(Player newPlayer)
		{
			playerList.Add(newPlayer);
			numPlayers++;
		}
		 
		public void SetNumPlayers(int numPlayers)
		{
			this.numPlayers = numPlayers;
		}
		
		public void SetClientPlayer(int playerId)
		{
			clientPlayer = playerList[playerId];
		}
		
		public uint rollDice() => (uint)(Random.Range(1, 7) + Random.Range(1, 7));
		
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
		
		public void OnPlayerBuiltOutpost()
		{
			playerList[currentPlayerTurn].numOutposts++;
		}
		public void OnPlayerBuiltFlyway()
		{
			playerList[currentPlayerTurn].numFlyways++;
		}
		
	}
}
