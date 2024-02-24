using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Synthicate
{
    public enum HexType { City, Powerplant, Scrapyard, Mine, Outlands, Desert };
    public enum ResourceType { People, Power, Mech, Metal, Food, Any };
    public enum GameEventType { Build, Trade, Hack, Roll, Info, Influence, Card };
    public enum CardType { Hacker, Influence};

    public struct HexResource
    {
        public uint id;
        public HexType hexType;
        //public ResourceType resource;
        public uint amount;

        public HexResource(uint id, HexType hexType, uint amount)
        {
            this.id = id;
            this.hexType = hexType;
            this.amount = amount;
        }

        public ResourceType getResource()
        {
            switch (hexType)
            {
                case HexType.City:
                    return ResourceType.People;
                case HexType.Powerplant:
                    return ResourceType.Power;
                case HexType.Scrapyard:
                    return ResourceType.Mech;
                case HexType.Mine:
                    return ResourceType.Metal;
                case HexType.Outlands:
                    return ResourceType.Food;
                default:
                    return ResourceType.Any;
            }
        }
    }

    public struct PlayerPoint
    {
        public uint id;
        public bool placed;
        public uint player;
        public bool isStronghold;

        public PlayerPoint(uint id, bool placed, uint player, bool isStronghold)
        {
            this.id = id;
            this.placed = placed;
            this.player = player;
            this.isStronghold = isStronghold;
        }
    }

    public struct PlayerEdge
    {
        public uint id;
        public bool placed;
        public uint player;

        public PlayerEdge(uint id, bool placed, uint player)
        {
            this.id = id;
            this.placed = placed;
            this.player = player;
        }
    }

    public struct GameEvent
    {
        public GameEventType type;
        public string message;

        public GameEvent(GameEventType type, string message)
        {
            this.type = type;
            this.message = message;
        }
    }

    public struct DepotTradeRequest
    {
        public ResourceType requestedResource;
        public uint amount;

        public DepotTradeRequest(ResourceType requestedResource, uint amount)
        {
            this.requestedResource = requestedResource;
            this.amount = amount;
        }
    }
    public struct DepotTrade
    {
        public ResourceType requestedResource;
        public ResourceType offeredResource;
        public uint offeredAmount;

        public DepotTrade(ResourceType requestedResource, ResourceType offeredResource, uint offeredAmount)
        {
            this.requestedResource = requestedResource;
            this.offeredResource = offeredResource;
            this.offeredAmount = offeredAmount;
        }
    }
    public struct Trade
    {
        public ResourceType requestedResource;
        public ResourceType offeredResource;
        
        public uint requestedAmount;
        public uint offeredAmount;

        public uint offeringPlayer;
        public uint targetPlayer;

        public Trade(uint targetPlayer, ResourceType requestedResource, uint requestedAmount, uint offeringPlayer, ResourceType offeredResource,  uint offeredAmount)
        {
            this.requestedResource = requestedResource;
            this.requestedAmount = requestedAmount;
            this.offeredResource = offeredResource;
            this.offeredAmount = offeredAmount;
            this.offeringPlayer = offeringPlayer;
            this.targetPlayer = targetPlayer;
        }
    }

    public struct BuildPermissions
    {
        public bool canBuildFlyway;
        public bool canBuildOutpost;
        public bool canBuildStronghold;

        public BuildPermissions(bool canBuildFlyway, bool canBuildOutpost, bool canBuildStronghold)
        {
            this.canBuildFlyway = canBuildFlyway;
            this.canBuildOutpost = canBuildOutpost;
            this.canBuildStronghold = canBuildStronghold;
        }
    }

    struct NetworkStringArray
    {
        public string[] array;
        public NetworkStringArray(string[] array) => this.array = array;
    }

    public class Global
    {
        //public enum HexType { City, Powerplant, Scrapyard, Mine, Outlands, Desert };

        public const int MAX_PLAYERS = 4;

        public const int NUM_HEXES = 19;

        public const int NUM_STRONGHOLD_POINTS = 54;
        public const int NUM_FLYWAY_EDGES = 72;
        public const int NUM_RESOURCE_TYPES = 5;
        public const int NUM_DEPOT_POINTS = 9;

        public static string[] PLAYER_MATERIALS = new string[4] {
        "Materials/Player/Neon_P1",
        "Materials/Player/Neon_P2",
        "Materials/Player/Neon_P3",
        "Materials/Player/Neon_P4"
        };

        public static string STRONGHOLD_EXTERNAL_MATERIAL = "Materials/Structures/Glossy_Black";

        public static string TRANSLUCENT_WHITE = "Materials/Translucent/Translucent_White";

        public static Color[] PLAYER_LIGHT_COLORS = new Color[4]
        {
            Color.yellow,
            Color.red,
            Color.blue,
            Color.cyan
        };

        public static string STRONGHOLD_MESH = "Meshes/stronghold";
        public static string OUTPOST_MESH = "Meshes/outpost";
        public static string FLYWAY_MESH = "Meshes/flyway";

        public static Transform FindChildWithTag(Transform root, string tag)
        {
            foreach (Transform t in root.GetComponentsInChildren<Transform>())
            {
                if (t.CompareTag(tag)) return t;
            }
            return null;
        }

        public static List<Transform> FindChildrenWithTag(Transform root, string tag)
        {
            List<Transform> children = new List<Transform>();

            foreach (Transform t in root.GetComponentsInChildren<Transform>())
            {
                if (t.CompareTag(tag))
                {
                    children.Add(t);
                }
            }
            return children;
        }

        public static Transform FindChildTransformsWithTag(Transform root, string tag)
        {
            for (int i = 0; i < root.childCount; i++)
            {
                if (root.GetChild(i).CompareTag(tag))
                {
                    return root.GetChild(i);
                }
            }
            return null;
        }

        public static List<Transform> FindChildenTransformsWithTag(Transform root, string tag)
        {
            List<Transform> children = new List<Transform>();

            for(int i = 0; i < root.childCount; i++)
            {
                if (root.GetChild(i).CompareTag(tag))
                {
                    children.Add(root.GetChild(i));
                }
            }
            return children;
        }

    }
}