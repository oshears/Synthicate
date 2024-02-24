using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Synthicate
{ 
    [CreateAssetMenu(fileName = "HexManagerScriptableObject", menuName = "ScriptableObjects/Hex Manager")]
    public class HexManagerScriptableObject : ScriptableObject
    {

        //bool strongholdsEnabled =
        public Material outlandsMaterial;
        public Material scrapyardMaterial;
        public Material cityMaterial;
        public Material powerplantMaterial;
        public Material mineMaterial;

        public Material hexCageMaterial;
        public Material fullyTranslucentMaterial;
        public Material semiTranslucentMaterial;

        public const int NUM_HEXES = 19;

        public enum HexType { City, Powerplant, Scrapyard, Mine, Outlands, Desert };


        [System.NonSerialized]
        public UnityEvent<uint, bool> hackerCageChangeEvent = new UnityEvent<uint, bool>();
        public UnityEvent<uint, bool> hackerParticlesChangeEvent = new UnityEvent<uint, bool>();
        public UnityEvent<uint, bool> hexLightsChangeEvent = new UnityEvent<uint, bool>();
        public UnityEvent<uint> hexSelectionEvent = new UnityEvent<uint>();
        public UnityEvent beginHackModeEvent = new UnityEvent();
        public UnityEvent endHackModeEvent = new UnityEvent();
        public UnityEvent<uint> hackEvent = new UnityEvent<uint>();
        public UnityEvent<uint> resourceRequest = new UnityEvent<uint>();
        public UnityEvent<HexResource> hexResourceResponse = new UnityEvent<HexResource>();
        public UnityEvent managerResourceResponse = new UnityEvent();
        public UnityEvent setupResourceRequest = new UnityEvent();
        public UnityEvent<HexResource> setupResourceResponse = new UnityEvent<HexResource>();
        public UnityEvent<List<HexResource>> managerSetupResourceResponse = new UnityEvent<List<HexResource>>();

        //bool hexHacked = false;
        //uint hackedHex = 0;

        private List<HexResource> hexResources = new List<HexResource>();
        uint numHexResourceResponses = 0;


        private void OnEnable()
        {
            // reset number of hex responses when resources requested
            resourceRequest.AddListener((uint diceValue) => {
                numHexResourceResponses = 0;
            });

            // record hex response and check if all hexes have responded
            hexResourceResponse.AddListener((HexResource hexResource) => {
                hexResources[(int)hexResource.id] = hexResource;
                numHexResourceResponses++;
                if (numHexResourceResponses == NUM_HEXES) managerResourceResponse.Invoke();
            });

            // setup resource request
            setupResourceRequest.AddListener(() => {
                numHexResourceResponses = 0;
            });

            // send setup resources response to game manager
            setupResourceResponse.AddListener((HexResource hexResource) => {
                hexResources[(int)hexResource.id] = hexResource;
                numHexResourceResponses++;
                if (numHexResourceResponses == NUM_HEXES) managerSetupResourceResponse.Invoke(hexResources);
            });

            for (int i = 0; i < NUM_HEXES; i++) hexResources.Add(new HexResource());
        }

        public void setHackerCages(uint hexSelection, bool enable) =>
            hackerCageChangeEvent.Invoke(hexSelection, enable);

        public void setHackerParticles(uint hexSelection, bool enable) =>
            hackerParticlesChangeEvent.Invoke(hexSelection, enable);

        public void setHexLights(uint hexSelection, bool enable) =>
            hexLightsChangeEvent.Invoke(hexSelection, enable);

        //public void hackEventResponder(uint id)
        //{
        //    hexHacked = true;
        //    hackedHex = id;
        //}


        //public void respondToResourceRequest(uint diceValue) => ;

        //public void respondToResourceResponse(HexResource hexResource)
        //{
            
        //}

        public List<HexResource> getResources() => hexResources;
        //public List<HexResource> getSetupResources()
        //{
        //    setupResourceRequest.Invoke();
        //    //while()
        //    //hexResources[(int)hexResource.id] = hexResource;
        //    //numHexResourceResponses++;
        //    //if (numHexResourceResponses == NUM_HEXES) managerResourceResponse.Invoke();
        //}


    }
}
