using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Synthicate
{
	public class Player
	{
		private string playerName;

		private uint clientID;

		public uint numOutposts { get; set; }
		public uint numStrongholds { get; set; }
		public uint numFlyways { get; set; }

		public uint[] resources = new uint[Global.NUM_RESOURCE_TYPES] { 0, 0, 0, 0, 0 };

		uint numHackers;
		uint numInfluencePoints;
		uint numHackersUsed;

		public Player()
		{
			
		}

		public void updateResources(uint[] resources)
		{
			for (int i = 0; i < Global.NUM_RESOURCE_TYPES; i++) this.resources[i] += resources[i];
		}

		public uint[] getResourceCounts() => resources;
		public uint getResourceCount(ResourceType type) => resources[(int) type];

		public void Initialize()
		{
			resources = new uint[Global.NUM_RESOURCE_TYPES] { 0, 0, 0, 0, 0 };
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
		public uint getNumCards() => numHackers + numInfluencePoints;
		public uint getNumHackers() => numHackers;
		public uint getNumInfleuncePointCards() => numInfluencePoints;
		public uint getNumHackersUsed() => numHackersUsed;

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

		public uint getPublicInfluencePoints() => numOutposts + numStrongholds * 2;
		public uint getTotalInfluencePoints() => numOutposts + numStrongholds * 2 + numInfluencePoints;

		//public string getName() => "Player " + (clientID + 1);
		public string getName() => playerName;
		public void SetName(string name) => playerName = name;
		public uint getId() => clientID;
		public void SetId(uint id) => clientID = id;
		public bool hasExcess() => getNumResources() > 7;

		public uint getNumResources()
		{
			uint totalResources = 0;
			for(int i = 0; i < Global.NUM_RESOURCE_TYPES; i++) totalResources += resources[i];
			return totalResources;
		}

		public void randomlyRemoveHalf()
		{
			uint numToRemove = getNumResources() / 2;
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