using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Synthicate
{ 
    [CreateAssetMenu(fileName = "DepotManagerScriptableObject", menuName = "ScriptableObjects/Depot Manager")]
    public class DepotManagerScriptableObject : ScriptableObject
    {

        //bool strongholdsEnabled =
        public Material outlandsMaterial;
        public Material scrapyardMaterial;
        public Material cityMaterial;
        public Material powerplantMaterial;
        public Material mineMaterial;
        public Material randomMaterial;

        public Material fullyTranslucentMaterial;
        public Material semiTranslucentMaterial;

        [System.NonSerialized]
        public UnityEvent<uint, ResourceType, uint> playerClickEvent = new UnityEvent<uint, ResourceType, uint>();
        //public UnityEvent<bool> playerResponseEvent;
        //public UnityEvent<uint, uint> initDepotTradeEvent;

        private void OnEnable()
        {
            //if (playerClickEvent == null) playerClickEvent = new UnityEvent<uint, uint, uint>();
        }
    }
}
