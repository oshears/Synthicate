using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Synthicate
{
    [CreateAssetMenu(fileName = "FlywayManagerScriptableObject", menuName = "ScriptableObjects/Flyway Manager")]
    public class FlywayManagerScriptableObject : ScriptableObject
    {

        public int NUM_FLYWAY_POINTS = 72;


        public Material[] playerMaterials = new Material[4];

        public Material fullyTranslucentMaterial;
        public Material semiTranslucentMaterial;


        public Color[] PLAYER_LIGHT_COLORS = new Color[4]{Color.yellow, Color.red, Color.blue, Color.cyan};

        List<PlayerEdge> edgeUpdates = new List<PlayerEdge>();
        uint numEdgeUpdateResponses = 0;

        [System.NonSerialized]
        public UnityEvent<uint, int> playerChangeEvent = new UnityEvent<uint, int>();

        [System.NonSerialized]
        public UnityEvent<uint> flywayBuildEvent = new UnityEvent<uint>();

        [System.NonSerialized]
        public UnityEvent<uint> vacantChangeEvent = new UnityEvent<uint>();

        [System.NonSerialized]
        public UnityEvent<int, List<uint>, BuildPermissions> beginBuildModeEvent = new UnityEvent<int, List<uint>, BuildPermissions>();

        [System.NonSerialized]
        public UnityEvent endBuildModeEvent = new UnityEvent();

        [System.NonSerialized]
        public UnityEvent<bool> playerBuildEvent = new UnityEvent<bool>();

        [System.NonSerialized]
        public UnityEvent edgeUpdateRequest = new UnityEvent();

        [System.NonSerialized]
        public UnityEvent<PlayerEdge> edgeUpdateResponse = new UnityEvent<PlayerEdge>();

        [System.NonSerialized]
        public UnityEvent<List<PlayerEdge>> managerEdgeUpdateResponse = new UnityEvent<List<PlayerEdge>>();

        private void OnEnable()
        {
            edgeUpdateRequest.AddListener( () => numEdgeUpdateResponses = 0 );

            edgeUpdateResponse.AddListener((PlayerEdge edge) => {
                edgeUpdates[(int)edge.id] = edge;
                numEdgeUpdateResponses++;
                if (numEdgeUpdateResponses == Global.NUM_FLYWAY_EDGES) managerEdgeUpdateResponse.Invoke(edgeUpdates);
            });

            for (int i = 0; i < Global.NUM_FLYWAY_EDGES; i++) edgeUpdates.Add(new PlayerEdge(0, false, 0));
        }

        public int getNumFlywayPoints() => NUM_FLYWAY_POINTS;

        public void changeToPlayer(uint strongholdSelection, int player) => playerChangeEvent.Invoke(strongholdSelection, player);

        public void changeToFlyway(uint strongholdSelection) => flywayBuildEvent.Invoke(strongholdSelection);

        public void changeToVacant(uint strongholdSelection) => vacantChangeEvent.Invoke(strongholdSelection);

        public void beginBuildModeForPlayer(int player, List<uint> buildEdges, BuildPermissions permissions) => beginBuildModeEvent.Invoke(player, buildEdges, permissions);

        public void endBuildMode() => endBuildModeEvent.Invoke();

    }
}
