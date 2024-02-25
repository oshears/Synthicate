using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Netcode;

namespace Synthicate
{

    public class HexController : NetworkBehaviour
    {
        // public configuration
        public uint id;
        public int hexValue;
        public Vector3 position;
        public HexType hexType;


        // private configuration
        [SerializeField]
        private HexManagerScriptableObject hexManager;

        bool hackerParticlesEnabled = false;
        bool hackerCageEnabled = false;
        bool hexLightsEnabled = false;

        enum HexControllerState {Normal, Hackable, Hacked, Static};
        HexControllerState state = HexControllerState.Normal;

        MeshRenderer hexCageMeshRenderer;

        BoxCollider hexCollider;


        // Start is called before the first frame update
        void Start()
        {
            hexManager.beginHackModeEvent.AddListener(configureHackOption);
            hexManager.endHackModeEvent.AddListener(clearHackOption);
            hexManager.resourceRequest.AddListener((uint diceValue) =>
            {
                HexResource resourceStruct = new HexResource(id, hexType, (state != HexControllerState.Hacked && diceValue == hexValue) ? 1u : 0u);
                hexManager.hexResourceResponse.Invoke(resourceStruct);
            });
            hexManager.setupResourceRequest.AddListener(() =>
            {
                HexResource resourceStruct = new HexResource(id, hexType,1u);
                hexManager.setupResourceResponse.Invoke(resourceStruct);
            });

            // set text mesh pro text to hex value
            if (hexType != HexType.Desert)
            {
                GetComponentInChildren<TextMeshPro>().text = hexValue.ToString();
            }
            else
            {
                GetComponentInChildren<TextMeshPro>().text = "";
                state = HexControllerState.Static;
            }

            
            Transform hexLoopTransform = Global.FindChildWithTag(transform,"hex_loop");
            hexLoopTransform.GetComponent<ParticleSystem>().Stop();

            Transform hexLightTransform = Global.FindChildWithTag(transform, "hex_light");
            if (hexLightTransform != null) hexLightTransform.GetComponent<Light>().enabled = false;


            Transform hexCageTransform = Global.FindChildTransformsWithTag(transform, "hex_cage");
            if (hexCageTransform != null)
            {
                hexCageTransform.gameObject.SetActive(false);
                hexCageMeshRenderer = hexCageTransform.GetComponent<MeshRenderer>();
            }

            Transform hexSelectionTransform = Global.FindChildWithTag(transform, "hex_select");
            if (hexSelectionTransform != null) hexSelectionTransform.GetComponent<ParticleSystem>().Stop();

            //for (int i = 0; i < transform.childCount; i++)
            //{
            //    Debug.Log(transform.GetChild(i).tag);
            //}

            hexCollider = GetComponent<BoxCollider>();
            hexCollider.enabled = false;
        }

        private void OnEnable()
        {
            hexManager.hackerCageChangeEvent.AddListener(setHackerCage);
            hexManager.hackerParticlesChangeEvent.AddListener(setHackerParticles);
            hexManager.hexLightsChangeEvent.AddListener(setHexLights);
            hexManager.hexSelectionEvent.AddListener(hexSelectionResponse);
        }

        private void OnDisable()
        {
            hexManager.hackerCageChangeEvent.RemoveListener(setHackerCage);
            hexManager.hackerParticlesChangeEvent.RemoveListener(setHackerParticles);
            hexManager.hexLightsChangeEvent.RemoveListener(setHexLights);
        }

        // Update is called once per frame
        void Update()
        {

        }

        //void respondToResourceRequest(uint diceValue)
        //{
        //    HexResource resourceStruct = new HexResource(id, hexType, (state != HexControllerState.Hacked && diceValue == hexValue) ? 1u : 0u);
        //    hexManager.hexResourceResponse.Invoke(resourceStruct);
        //}

        void hexSelectionResponse(uint selection)
        {
            if (selection == hexValue)
            {
                enableSelectionEffects(true);
            }
            else
            {
                enableSelectionEffects(false);
            }
        }

        void enableSelectionEffects(bool enable)
        {
            Transform hexSelectionTransform = Global.FindChildWithTag(transform, "hex_select");

            if (hexSelectionTransform == null) return;

            if (enable)
                hexSelectionTransform.GetComponent<ParticleSystem>().Play();
            else
                hexSelectionTransform.GetComponent<ParticleSystem>().Stop();
        }

        void setHackerCage(uint selectedHex, bool enable)
        {
            // get child cage mesh game object
            Transform hackerCageTransform = transform.Find("hex_cage");

            if (hackerCageTransform != null && enable != hackerCageEnabled && (selectedHex == id || selectedHex == Global.NUM_HEXES)) hackerCageTransform.gameObject.SetActive(enable);

            hackerCageEnabled = enable;
        }

        void setHackerCage(bool enable)
        {
            // get child cage mesh game object
            Transform hackerCageTransform = transform.Find("hex_cage");

            if (hackerCageTransform != null && enable != hackerCageEnabled) hackerCageTransform.gameObject.SetActive(enable);

            hackerCageEnabled = enable;
        }

        void changeHackCageMaterialToHacked()
        {
            if (hexCageMeshRenderer != null) hexCageMeshRenderer.material = hexManager.hexCageMaterial;
        }

        void changeHackCageMaterialToFullyTranslucent()
        {
            if (hexCageMeshRenderer != null) hexCageMeshRenderer.material = hexManager.fullyTranslucentMaterial;
        }

        void changeHackCageMaterialToSemiTranslucent()
        {
            if (hexCageMeshRenderer != null) hexCageMeshRenderer.material = hexManager.semiTranslucentMaterial;

        }

        void setHackerParticles(bool enable)
        {
            // get child hex loop game object
            Transform hexLoopTransform = transform.Find("hex_loop");

            if (hexLoopTransform != null && enable && !hackerParticlesEnabled) hexLoopTransform.GetComponent<ParticleSystem>().Play();
            else if (hexLoopTransform != null && !enable && hackerParticlesEnabled) hexLoopTransform.GetComponent<ParticleSystem>().Stop();

            hackerParticlesEnabled = enable;
        }

        void setHackerParticles(uint selectedHex, bool enable)
        {
            // get child hex loop game object
            Transform hexLoopTransform = transform.Find("hex_loop");

            if (hexLoopTransform != null && enable && !hackerParticlesEnabled && (selectedHex == id || selectedHex == Global.NUM_HEXES)) hexLoopTransform.GetComponent<ParticleSystem>().Play();
            else if (hexLoopTransform != null && !enable && hackerParticlesEnabled && (selectedHex == id || selectedHex == Global.NUM_HEXES)) hexLoopTransform.GetComponent<ParticleSystem>().Stop();

            hackerParticlesEnabled = enable;
        }

        void setHexLights(uint selectedHex, bool enable)
        {
            Transform hexLightTransform = Global.FindChildWithTag(transform,"hex_light");

            if (hexLightTransform != null && enable != hexLightsEnabled && (selectedHex == id || selectedHex == Global.NUM_HEXES)) hexLightTransform.GetComponent<Light>().enabled = enable;

            hexLightsEnabled = enable;
        }

        void configureHackOption()
        {
            if (state == HexControllerState.Normal)
            {
                setHackerCage(true);
                changeHackCageMaterialToFullyTranslucent();
                state = HexControllerState.Hackable;
                hexCollider.enabled = true;
            }
            else if (state == HexControllerState.Hacked)
            {
                state = HexControllerState.Normal;
            }
        }

        void clearHackOption()
        {
            if (state == HexControllerState.Normal || state == HexControllerState.Hackable)
            {
                setHackerParticles(false);
                setHackerCage(false);
                state = HexControllerState.Normal;
                
            }
            else if (state == HexControllerState.Hacked)
            {
                setHackerParticles(true);
            }
            hexCollider.enabled = false;
        }

        private void OnMouseEnter()
        {
            if (state == HexControllerState.Hackable)
            {
                changeHackCageMaterialToSemiTranslucent();
            }
        }
        private void OnMouseExit()
        {
            if (state == HexControllerState.Hackable)
            {
                changeHackCageMaterialToFullyTranslucent();
            }
        }

        private void OnMouseDown()
        {
            if (state == HexControllerState.Hackable)
            {
                requestHackServerRpc();
            }
        }

        [ServerRpc(RequireOwnership = false)]
        private void requestHackServerRpc() => hackHexClientRpc();

        [ClientRpc]
        private void hackHexClientRpc()
        {
            //this.player = player;
            //setChildrenActive(true);
            //changeMaterialToPlayer(player);
            //state = FlywayControllerState.FlywayPlaced;
            //flywayManager.playerBuildEvent.Invoke();
            state = HexControllerState.Hacked;
            changeHackCageMaterialToHacked();
            hexManager.hackEvent.Invoke(id);

        }


    }
}

