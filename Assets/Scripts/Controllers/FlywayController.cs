using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

namespace Synthicate
{
    public class FlywayController : NetworkBehaviour
    {
        public uint id;

        [SerializeField]
        private FlywayManagerScriptableObject flywayManager;

        int player;

        int pendingPlayer;
        BuildPermissions pendingPlayerPermissions;

        enum FlywayControllerState { Vacant, FlywayAvailable, FlywayPlaced };

        FlywayControllerState state = FlywayControllerState.Vacant;

        MeshRenderer meshRenderer;

        BoxCollider flywayCollider;

        // Start is called before the first frame update
        void Start()
        {
            flywayManager.vacantChangeEvent.AddListener(changeToVacant);
            flywayManager.flywayBuildEvent.AddListener(changeToPlaced);
            flywayManager.playerChangeEvent.AddListener(changeToPlayer);
            flywayManager.beginBuildModeEvent.AddListener((int player, List<uint> buildEdges, BuildPermissions permissions) => {
                
                if (!buildEdges.Contains(id)) return;

                pendingPlayer = player;
                pendingPlayerPermissions = permissions;

                if (state == FlywayControllerState.Vacant)
                {
                    state = FlywayControllerState.FlywayAvailable;
                    setChildrenActive(true);
                    changeMaterialToFullyTranslucent();
                    flywayCollider.enabled = true;
                }
            });
            flywayManager.endBuildModeEvent.AddListener(() => {
                if (state == FlywayControllerState.FlywayAvailable)
                {
                    setChildrenActive(false);
                    state = FlywayControllerState.Vacant;
                }
                flywayCollider.enabled = false;
            });

            meshRenderer = Global.FindChildWithTag(transform, "flyway").GetComponent<MeshRenderer>();

            changeToVacant(id);

            flywayCollider = GetComponent<BoxCollider>();
            flywayCollider.enabled = false;

            flywayManager.edgeUpdateRequest.AddListener( 
                () => {
                    PlayerEdge edge = new PlayerEdge(id, state != FlywayControllerState.Vacant, player);
                    flywayManager.edgeUpdateResponse.Invoke(edge);
                }
            );
        }

        // Update is called once per frame
        void Update()
        {

        }


        void setChildrenActive(bool active)
        {
            for (int i = 0; i < transform.childCount; i++) transform.GetChild(i).gameObject.SetActive(active);
        }



        void changeMaterialToPlayer(int player)
        {
            Material[] materials = meshRenderer.materials;
            materials[0] = flywayManager.playerMaterials[player];
            materials[1] = flywayManager.playerMaterials[player];
            meshRenderer.materials = materials;
        }

        void changeMaterialToFullyTranslucent()
        {
            Material[] materials = meshRenderer.materials;
            materials[0] = flywayManager.fullyTranslucentMaterial;
            materials[1] = flywayManager.fullyTranslucentMaterial;
            meshRenderer.materials = materials;
        }

        void changeMaterialToSemiTranslucent()
        {
            Material[] materials = meshRenderer.materials;
            materials[0] = flywayManager.semiTranslucentMaterial;
            materials[1] = flywayManager.semiTranslucentMaterial;
            meshRenderer.materials = materials;
        }


        bool matchesSelection(uint flywaySelection) => flywaySelection == id || flywaySelection == flywayManager.NUM_FLYWAY_POINTS;


        ////////////////////////

        void changeToVacant(uint strongholdSelection)
        {
            if (matchesSelection(strongholdSelection)) setChildrenActive(false);

            state = FlywayControllerState.Vacant;
        }

        void changeToPlaced(uint strongholdSelection)
        {

            if (matchesSelection(strongholdSelection))
            {
                // if the stronghold was previously vacant, activate the components and children
                if (state == FlywayControllerState.Vacant) setChildrenActive(true);

                // Stronghold Mesh Material Change
                changeMaterialToPlayer(player);

            }

            state = FlywayControllerState.FlywayPlaced;
        }

        

        void changeToPlayer(uint strongholdSelection, int player)
        {
            //Debug.Log(strongholdSelection + " : " + player);
            if (strongholdSelection == id || strongholdSelection == flywayManager.NUM_FLYWAY_POINTS)
            {
                // if the stronghold was previously vacant, activate the components and children
                if (state == FlywayControllerState.Vacant) setChildrenActive(true);

                // change stronghold to designated player
                this.player = player;

                // Stronghold Mesh Material Change
                changeMaterialToPlayer(player);
            }
        }

        private void OnMouseEnter()
        {

            if (state == FlywayControllerState.FlywayAvailable)
            {
                changeMaterialToSemiTranslucent();
            }

        }

        private void OnMouseExit()
        {

            if (state == FlywayControllerState.FlywayAvailable)
            {
                changeMaterialToFullyTranslucent();
            }

        }

        private void OnMouseDown()
        {
            if (state == FlywayControllerState.FlywayAvailable && pendingPlayerPermissions.canBuildFlyway)
            {
                requestBuildFlywayServerRpc(pendingPlayer);
            }
            else
            {
                flywayManager.playerBuildEvent.Invoke(false);
            }
        }

        [ServerRpc(RequireOwnership = false)]
        private void requestBuildFlywayServerRpc(int player) => buildFlywayClientRpc(player);

        [ClientRpc]
        private void buildFlywayClientRpc(int player)
        {
            this.player = player;
            setChildrenActive(true);
            changeMaterialToPlayer(player);
            state = FlywayControllerState.FlywayPlaced;
            flywayManager.playerBuildEvent.Invoke(true);
        }

    }
}