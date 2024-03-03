using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Synthicate
{
	public class Player
	{
		private string playerName;

		private ulong networkClientId;
		private int playerId;

		public int numOutposts { get; set; }
		public int numStrongholds { get; set; }
		public int numFlyways { get; set; }

		public int[] resources = new int[Global.NUM_RESOURCE_TYPES] { 0, 0, 0, 0, 0 };

		int numHackers;
		int numInfluencePoints;
		int numHackersUsed;

		public Player()
		{
			InitializeResourceCounts();
		}
		
		public Player(string playerName, int playerId)
		{
			this.playerName = playerName;
			this.playerId = playerId;
			networkClientId = 0;
			InitializeResourceCounts();
		}
		
		public void SetNetworkClientId(ulong clientId)
		{
			networkClientId = clientId;
		}

		public void updateResources(int[] resources)
		{
			for (int i = 0; i < Global.NUM_RESOURCE_TYPES; i++) this.resources[i] += resources[i];
		}

		// public int[] getResourceCounts() => resources;
		public int GetResourceCount(ResourceType type) => resources[(int) type];

		public void InitializeResourceCounts()
		{
			resources = new int[Global.NUM_RESOURCE_TYPES] { 0, 0, 0, 0, 0 };
			numOutposts = 0;
			numStrongholds = 0;
			numFlyways = 0;
			numHackers = 0;
			numInfluencePoints = 0;
			numHackersUsed = 0;
		}
		
		public void addInfluencePoint() => numInfluencePoints++;
		public void addHacker() => numHackers++;
		//public void removeHacker() => numHackers--;

		public bool canBuildFlyway() => resources[(int)ResourceType.Metal] >= 1 && resources[(int)ResourceType.Power] >= 1;
		public bool canBuildOutpost() => resources[(int)ResourceType.Metal] >= 1 && resources[(int)ResourceType.Power] >= 1 && resources[(int)ResourceType.Food] >= 1 && resources[(int)ResourceType.People] >= 1;
		public bool canBuildStronghold() => resources[(int)ResourceType.People] >= 2 && resources[(int)ResourceType.Mech] >= 3;
		public bool canBuyCard() => resources[(int)ResourceType.Mech] >= 1 && resources[(int)ResourceType.People] >= 1 && resources[(int)ResourceType.Food] >= 1;
		public bool canUseHacker() => numHackers >= 1;
		public int getNumCards() => numHackers + numInfluencePoints;
		public int getNumHackers() => numHackers;
		public int getNumInfleuncePointCards() => numInfluencePoints;
		public int GetNumHackersUsed() => numHackersUsed;

		public void buildFlyway()
		{
			resources[(int)ResourceType.Metal]--;
			resources[(int)ResourceType.Power]--;
			numFlyways++;
		}
		public void buildOutpost()
		{
			resources[(int)ResourceType.Metal]--;
			resources[(int)ResourceType.Power]--; 
			resources[(int)ResourceType.Food]--;
			resources[(int)ResourceType.People]--;
			numOutposts++;
		}

		public void buildStronghold()
		{
			resources[(int)ResourceType.Mech] -= 3;
			resources[(int)ResourceType.People] -= 2;
			numStrongholds++;
			numOutposts--;
		}
		public void useHacker()
		{
			numHackers--;
		}

		public void buyEventCard()
		{
			resources[(int)ResourceType.Mech]--;
			resources[(int)ResourceType.People]--;
			resources[(int)ResourceType.Food]--;
		}

		public int getPublicInfluencePoints() => numOutposts + numStrongholds * 2;
		public int getTotalInfluencePoints() => numOutposts + numStrongholds * 2 + numInfluencePoints;

		//public string getName() => "Player " + (clientID + 1);
		public string GetName() => playerName;
		public void SetName(string name) => playerName = name;
		public int GetId() => playerId;
		public void SetId(int id) => playerId = id;
		public bool hasExcess() => getNumResources() > 7;

		public int getNumResources()
		{
			int totalResources = 0;
			for(int i = 0; i < Global.NUM_RESOURCE_TYPES; i++) totalResources += (int) resources[i];
			return totalResources;
		}

		public void randomlyRemoveHalf()
		{
			int numToRemove = getNumResources() / 2;
			for (int i = 0; i < numToRemove; i++)
			{
				int resourceToRemove = Random.Range(0, Global.NUM_RESOURCE_TYPES);
				bool removed = false;
				while (!removed)
				{
					if (resources[resourceToRemove] > 0)
					{
						resources[resourceToRemove]--;
						removed = true;
					}
					else
						resourceToRemove = (resourceToRemove + 1) % Global.NUM_RESOURCE_TYPES;
				}
				
				
			}
		}
		public void debugIncrementAllResources()
		{
			for (int i = 0; i < Global.NUM_RESOURCE_TYPES; i++) resources[i] += 1;
		}
	}

	
}