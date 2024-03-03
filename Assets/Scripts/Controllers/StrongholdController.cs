using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;


namespace Synthicate
{

    public class StrongholdController : NetworkBehaviour
    {
        public uint id;
        //public Vector3 position;

        [SerializeField]
        private StrongholdManagerScriptableObject strongholdManager;

        int player;

        int pendingPlayer;
        BuildPermissions pendingPlayerPermissions;

        enum StrongholdControllerState {Vacant, OutpostAvailable, Outpost, StrongholdAvailable, Stronghold};

        StrongholdControllerState state = StrongholdControllerState.Vacant;

        MeshRenderer meshRenderer;

        Light[] strongholdLights;

        TrailRenderer strongholdTrail;

        Vector3 strongholdGlobalPosition;
        Vector3 outpostGlobalPosition;

        BoxCollider strongholdCollider;

        // Start is called before the first frame update
        void Start()
        {
            strongholdManager.vacantChangeEvent.AddListener(changeToVacant);
            strongholdManager.outpostChangeEvent.AddListener(changeToOutpost);
            strongholdManager.strongholdChangeEvent.AddListener(changeToStronghold);
            strongholdManager.playerChangeEvent.AddListener(changeToPlayer);
            strongholdManager.beginBuildModeEvent.AddListener((int player, List<uint> buildPoints, BuildPermissions permissions) => {
                if (!buildPoints.Contains(id)) return;

                pendingPlayer = player;
                pendingPlayerPermissions = permissions;

                if (state == StrongholdControllerState.Vacant)
                {
                    state = StrongholdControllerState.OutpostAvailable;
                    setChildrenActive(true);
                    changeMeshToOutpost();
                    changeMaterialToFullyTranslucent();
                    strongholdCollider.enabled = true;
                }
                else if (state == StrongholdControllerState.Outpost && this.player == player)
                {
                    state = StrongholdControllerState.StrongholdAvailable;
                    strongholdCollider.enabled = true;
                }
            });
            strongholdManager.endBuildModeEvent.AddListener(clearBuildMode);

            strongholdManager.pointUpdateRequest.AddListener(respondToPointUpdate);

            meshRenderer = Global.FindChildWithTag(transform, "stronghold_mesh").GetComponent<MeshRenderer>();

            List<Transform> children = Global.FindChildrenWithTag(transform, "stronghold_light");
            strongholdLights = new Light[children.Count];
            for(int i = 0; i < children.Count; i++)
            {
                if (children[i] != null) strongholdLights[i] = children[i].GetComponent<Light>();
                strongholdLights[i].enabled = false;
            }

            Transform trailTransform = Global.FindChildWithTag(transform, "stronghold_trail");
            if (trailTransform != null) strongholdTrail = trailTransform.GetComponent<TrailRenderer>();

            changeToVacant(id);

            strongholdGlobalPosition = transform.position;
            outpostGlobalPosition = transform.position + new Vector3(0,-4);

            strongholdCollider = GetComponent<BoxCollider>();
            strongholdCollider.enabled = false;
        }

        // Update is called once per frame
        void Update()
        {
            
        }

        void respondToPointUpdate()
        {
            PlayerPoint point = new PlayerPoint(id, state != StrongholdControllerState.Vacant, player, state == StrongholdControllerState.Stronghold);
            strongholdManager.pointUpdateResponse.Invoke(point);
        }
        

        void setChildrenActive(bool active)
        {
            for (int i = 0; i < transform.childCount; i++) transform.GetChild(i).gameObject.SetActive(active);
        }

        

        void changeMaterialToPlayer(int player)
        {
            Material[] materials = meshRenderer.materials;
            materials[0] = strongholdManager.externalMaterial;
            materials[1] = strongholdManager.playerMaterials[player];
            meshRenderer.materials = materials;


            strongholdTrail.material = strongholdManager.playerMaterials[player];
            for (int i = 0; i < strongholdLights.Length; i++) strongholdLights[i].color = strongholdManager.PLAYER_LIGHT_COLORS[player];
        }

        void changeMaterialToFullyTranslucent()
        {
            Material[] materials = meshRenderer.materials;
            materials[0] = strongholdManager.fullyTranslucentMaterial;
            materials[1] = strongholdManager.fullyTranslucentMaterial;
            meshRenderer.materials = materials;

            strongholdTrail.material = strongholdManager.fullyTranslucentMaterial;
            for (int i = 0; i < strongholdLights.Length; i++) strongholdLights[i].color = Color.white;
        }

        void changeMaterialToSemiTranslucent()
        {
            Material[] materials = meshRenderer.materials;
            materials[0] = strongholdManager.semiTranslucentMaterial;
            materials[1] = strongholdManager.semiTranslucentMaterial;
            meshRenderer.materials = materials;

            strongholdTrail.material = strongholdManager.semiTranslucentMaterial;
            for (int i = 0; i < strongholdLights.Length; i++) strongholdLights[i].color = Color.white;
        }

        void changeMeshToOutpost()
        {
            Transform meshTransform = Global.FindChildWithTag(transform, "stronghold_mesh");

            if (meshTransform == null) return;

            // Change Mesh to Outpost
            meshTransform.GetComponent<MeshFilter>().mesh = strongholdManager.outpostMesh;

            // Shift Downwards
            //transform.Translate(0, -4f, 0);
            //transform.position
            transform.position = outpostGlobalPosition;

            // Enable Trail
            strongholdTrail.enabled = false;
        }

        void changeMeshToStronghold()
        {
            Transform meshTransform = Global.FindChildWithTag(transform, "stronghold_mesh");

            if (meshTransform == null) return;

            // Change Mesh to Outpost
            meshTransform.GetComponent<MeshFilter>().mesh = strongholdManager.strongHoldMesh;

            // Shift Downwards
            //transform.Translate(0, 4f, 0);
            transform.position = strongholdGlobalPosition;

            // Enable Trail
            strongholdTrail.enabled = true;
        }

        bool matchesSelection(uint strongholdSelection) => strongholdSelection == id || strongholdSelection == Global.NUM_STRONGHOLD_POINTS;


        ////////////////////////

        void changeToVacant(uint strongholdSelection)
        {
            if (matchesSelection(strongholdSelection)) setChildrenActive(false);

            state = StrongholdControllerState.Vacant;
        }

        void changeToOutpost(uint strongholdSelection) {

            if (matchesSelection(strongholdSelection))
            {
                // if the stronghold was previously vacant, activate the components and children
                if (state == StrongholdControllerState.Vacant) setChildrenActive(true);

                // update mesh
                changeMeshToOutpost();

                // Stronghold Mesh Material Change
                changeMaterialToPlayer(player);

            }

            state = StrongholdControllerState.Outpost;
        }

        void changeToStronghold(uint strongholdSelection)
        {
            if (matchesSelection(strongholdSelection))
            {
                // if the stronghold was previously vacant, activate the components and children
                if (state == StrongholdControllerState.Vacant) setChildrenActive(true);

                // update mesh
                changeMeshToStronghold();

                // Stronghold Mesh Material Change
                changeMaterialToPlayer(player);
            }

            state = StrongholdControllerState.Stronghold;
        }

        void changeToPlayer(int strongholdSelection, int player)
        {
            //Debug.Log(strongholdSelection + " : " + player);
            if (strongholdSelection == id || strongholdSelection == Global.NUM_STRONGHOLD_POINTS)
            {
                // if the stronghold was previously vacant, activate the components and children
                if (state == StrongholdControllerState.Vacant) setChildrenActive(true);

                // change stronghold to designated player
                this.player = player;

                // Stronghold Mesh Material Change
                changeMaterialToPlayer(player);
            }
        }

        void configureBuildMode(int player, List<uint> buildPoints)
        {
            if (!buildPoints.Contains(id)) return;

            pendingPlayer = player;

            if (state == StrongholdControllerState.Vacant)
            {
                state = StrongholdControllerState.OutpostAvailable;
                setChildrenActive(true);
                changeMeshToOutpost();
                changeMaterialToFullyTranslucent();
                strongholdCollider.enabled = true;
            }
            else if (state == StrongholdControllerState.Outpost && this.player == player)
            {
                state = StrongholdControllerState.StrongholdAvailable;
                strongholdCollider.enabled = true;
            }
        }

        void clearBuildMode()
        {
            if (state == StrongholdControllerState.OutpostAvailable)
            {
                setChildrenActive(false);
                state = StrongholdControllerState.Vacant;
            }
            else if(state == StrongholdControllerState.StrongholdAvailable)
            {
                changeMeshToOutpost();
                changeMaterialToPlayer(player);
                state = StrongholdControllerState.Outpost;
            }
            strongholdCollider.enabled = false;
        }


        private void OnMouseEnter()
        {

            if (state == StrongholdControllerState.OutpostAvailable)
            {
                changeMaterialToSemiTranslucent();
            }
            else if (state == StrongholdControllerState.StrongholdAvailable)
            {
                changeMaterialToSemiTranslucent();
                changeMeshToStronghold();
            }

        }

        private void OnMouseExit()
        {

            if (state == StrongholdControllerState.OutpostAvailable)
            {
                changeMaterialToFullyTranslucent();
            }
            else if (state == StrongholdControllerState.StrongholdAvailable)
            {
                changeMaterialToPlayer(player);
                changeMeshToOutpost();
            }

        }

        private void OnMouseDown()
        {
            if (state == StrongholdControllerState.OutpostAvailable)
            {
                if (pendingPlayerPermissions.canBuildOutpost)
                    requestBuildOutpostServerRpc(pendingPlayer);
                else
                    strongholdManager.playerBuildEvent.Invoke(false);
            }
            else if (state == StrongholdControllerState.StrongholdAvailable){
                if (pendingPlayerPermissions.canBuildStronghold)
                    requestBuildStrongholdServerRpc(pendingPlayer);
                else
                    strongholdManager.playerBuildEvent.Invoke(false);
            }
        }

        [ServerRpc(RequireOwnership = false)]
        private void requestBuildOutpostServerRpc(int player) => buildOutpostClientRpc(player);

        [ClientRpc]
        private void buildOutpostClientRpc(int player)
        {
            this.player = player;
            setChildrenActive(true);
            changeMeshToOutpost();
            changeMaterialToPlayer(player);
            state = StrongholdControllerState.Outpost;
            strongholdManager.playerBuildEvent.Invoke(true);
        }

        [ServerRpc(RequireOwnership = false)]
        private void requestBuildStrongholdServerRpc(int player) => buildStrongholdClientRpc(player);

        [ClientRpc]
        private void buildStrongholdClientRpc(int player)
        {
            setChildrenActive(true);
            changeMeshToStronghold();
            changeMaterialToPlayer(player);
            state = StrongholdControllerState.Stronghold;
            strongholdManager.playerBuildEvent.Invoke(true);
        }

    }
}