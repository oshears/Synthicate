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
				
		public void Initialize()
		{
			playerList = new List<Player>();
			numPlayers = 0;
		}
		
		public void AddPlayer(Player newPlayer)
		{
			playerList.Add(newPlayer);
		}
		 
		public void SetNumPlayers(int numPlayers)
		{
			this.numPlayers = numPlayers;
		}
		
		public void SetClientPlayer(Player player)
		{
			clientPlayer = player;
		}
		
		public uint rollDice() => (uint)(Random.Range(1, 7) + Random.Range(1, 7));
		
		public void OnStartNextTurn()
		{
			startNextTurnEvent.Invoke();
		}
		


	}
}
