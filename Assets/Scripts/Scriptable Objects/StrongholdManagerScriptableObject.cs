using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Synthicate
{
    [CreateAssetMenu(fileName = "StrongholdManagerScriptableObject", menuName = "ScriptableObjects/Stronghold Manager")]
    public class StrongholdManagerScriptableObject : ScriptableObject
    {

        //public int NUM_STRONGHOLD_POINTS = 54;


        public Material[] playerMaterials = new Material[4];
        public Material externalMaterial;

        public Material fullyTranslucentMaterial;
        public Material semiTranslucentMaterial;

        public Mesh outpostMesh;
        public Mesh strongHoldMesh;

        public Color[] PLAYER_LIGHT_COLORS = new Color[4]{Color.yellow, Color.red, Color.blue, Color.cyan};

        [System.NonSerialized]
        public UnityEvent<int, int> playerChangeEvent;

        [System.NonSerialized]
        public UnityEvent<uint> outpostChangeEvent;

        [System.NonSerialized]
        public UnityEvent<uint> strongholdChangeEvent;

        [System.NonSerialized]
        public UnityEvent<uint> vacantChangeEvent;

        public UnityEvent<int, List<uint>, BuildPermissions> beginBuildModeEvent;
        public UnityEvent endBuildModeEvent;
        public UnityEvent<bool> playerBuildEvent;

        public UnityEvent pointUpdateRequest;
        public UnityEvent<PlayerPoint> pointUpdateResponse;
        public UnityEvent<List<PlayerPoint>> managerPointUpdateResponse;

        List<PlayerPoint> pointUpdates = new List<PlayerPoint>();
        uint numPointUpdateResponses = 0;

        private void OnEnable()
        {
            // if (playerChangeEvent == null) playerChangeEvent = new UnityEvent<int, int>();
            // if (outpostChangeEvent == null) outpostChangeEvent = new UnityEvent<uint>();
            // if (strongholdChangeEvent == null) strongholdChangeEvent = new UnityEvent<uint>();
            // if (vacantChangeEvent == null) vacantChangeEvent = new UnityEvent<uint>();
            // if (beginBuildModeEvent == null) beginBuildModeEvent = new UnityEvent<int, List<uint>, BuildPermissions>();
            // if (endBuildModeEvent == null) endBuildModeEvent = new UnityEvent();
            // if (playerBuildEvent == null) playerBuildEvent = new UnityEvent<bool>();
            // if (pointUpdateRequest == null) pointUpdateRequest = new UnityEvent();
            // if (pointUpdateResponse == null) pointUpdateResponse = new UnityEvent<PlayerPoint>();
            // if (managerPointUpdateResponse == null) managerPointUpdateResponse = new UnityEvent<List<PlayerPoint>>();

            pointUpdateRequest.AddListener(respondToPointUpdateRequest);
            pointUpdateResponse.AddListener(respondToPointUpdateResponse);

            for (int i = 0; i < Global.NUM_STRONGHOLD_POINTS; i++) pointUpdates.Add(new PlayerPoint(0,false,0,false));
        }

        public int getNumStrongholdPoints() => Global.NUM_STRONGHOLD_POINTS;

        public void changeToPlayer(int strongholdSelection, int player) => playerChangeEvent.Invoke(strongholdSelection,player);

        public void changeToOutpost(uint strongholdSelection) => outpostChangeEvent.Invoke(strongholdSelection);

        public void changeToStronghold(uint strongholdSelection) => strongholdChangeEvent.Invoke(strongholdSelection);

        public void changeToVacant(uint strongholdSelection) => vacantChangeEvent.Invoke(strongholdSelection);

        public void beginBuildModeForPlayer(int player, List<uint> buildPoints, BuildPermissions permissions) => beginBuildModeEvent.Invoke(player, buildPoints, permissions);

        public void endBuildMode() => endBuildModeEvent.Invoke();

        public void respondToPointUpdateRequest() => numPointUpdateResponses = 0;

        public void respondToPointUpdateResponse(PlayerPoint point)
        {
            //Debug.Log(point.id + " " + point.player + " " + point.placed +" " + point.isStronghold);
            pointUpdates[(int)point.id] = point;
            numPointUpdateResponses++;
            if (numPointUpdateResponses == Global.NUM_STRONGHOLD_POINTS)
            {
                managerPointUpdateResponse.Invoke(pointUpdates);
            }
        }

    }
}
