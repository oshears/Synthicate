using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Synthicate
{
    public class DebugMenuController : MonoBehaviour
    {
        enum DebugMenuState { Enabled, Disabled};
        DebugMenuState state = DebugMenuState.Disabled;

        //[SerializeField, Tooltip("Determines size of menu")]

        [SerializeField]
        private HexManagerScriptableObject hexManager;

        [SerializeField]
        private StrongholdManagerScriptableObject strongholdManager;

        [SerializeField]
        private FlywayManagerScriptableObject flywayManager;


        Vector2 scrollBarPosition = new Vector2();

        // private hex variables
        int hexSelection = Global.NUM_HEXES;
        bool hackerParticlesEnabled = false;
        bool hackerCagesEnabled = false;
        bool hexLightsEnable = false;

        // private stronghold variables
        int strongholdSelection = Global.NUM_STRONGHOLD_POINTS;
        uint playerSelection = 0;

        // private flyway
        int flywaySelection = Global.NUM_FLYWAY_EDGES;

        //string textField = "";


        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        private void OnGUI()
        {
            Rect debugMenuRect = new Rect();
            debugMenuRect.xMin = 4 * Screen.width / 5;
            debugMenuRect.xMax = Screen.width;
            debugMenuRect.yMin = 0;
            debugMenuRect.yMax = Screen.height;

            if (state == DebugMenuState.Disabled)
            {
                GUILayout.BeginArea(debugMenuRect);
                if (GUILayout.Button("Enable Debug Menu"))
                {
                    state = DebugMenuState.Enabled;
                }
                GUILayout.EndArea();
            }
            else {
                GUILayout.BeginArea(debugMenuRect);
                scrollBarPosition = GUILayout.BeginScrollView(scrollBarPosition);

                GUILayout.Label("Debug Menu");
                if (GUILayout.Button("Disable Debug Menu"))
                {
                    state = DebugMenuState.Disabled;
                }


                // Hex Settings
                GUILayout.Label("Hex Debug Settings");

                // Hacker Cages
                if (GUILayout.Button("Toggle Hacker Cages"))
                {
                    hexManager.setHackerCages((uint) hexSelection, hackerCagesEnabled);
                    hackerCagesEnabled = !hackerCagesEnabled;
                }

                // Particles
                if (GUILayout.Button("Toggle Hacker Particles"))
                {
                    hexManager.setHackerParticles((uint) hexSelection, hackerParticlesEnabled);
                    hackerParticlesEnabled = !hackerParticlesEnabled;
                }

                // Lights
                if (GUILayout.Button("Toggle Hex Lights"))
                {
                    hexManager.setHexLights((uint) hexSelection, hexLightsEnable);
                    hexLightsEnable = !hexLightsEnable;
                }

                // Hex Selection
                GUILayout.Label("Hex Selection");
                string[] hexSelections = new string[Global.NUM_HEXES + 1];
                for (int i = 0; i < Global.NUM_HEXES; i++) hexSelections[i] = i.ToString();
                hexSelections[Global.NUM_HEXES] = "All";
                int newHexSelection = GUILayout.SelectionGrid(hexSelection, hexSelections, 5);
                hexSelection = newHexSelection;


                // Stronghold Settings
                GUILayout.Label("Stronghold Debug Settings");

                // Stronghold Selection
                GUILayout.Label("Stronghold Selection");
                string[] strongholdSelections = new string[Global.NUM_STRONGHOLD_POINTS + 1];
                for (int i = 0; i < Global.NUM_STRONGHOLD_POINTS; i++) strongholdSelections[i] = i.ToString();
                strongholdSelections[Global.NUM_STRONGHOLD_POINTS] = "All";
                int newStrongholdSelection = GUILayout.SelectionGrid(strongholdSelection, strongholdSelections, 5);
                strongholdSelection = newStrongholdSelection;

                if (GUILayout.Button("Change to Vacant Space")) strongholdManager.changeToVacant((uint) strongholdSelection);
                if (GUILayout.Button("Change to Outpost Mesh")) strongholdManager.changeToOutpost((uint) strongholdSelection);
                if (GUILayout.Button("Change to Stronghold Mesh")) strongholdManager.changeToStronghold((uint) strongholdSelection);
                if (GUILayout.Button("Toggle Stronghold Player"))
                {
                    strongholdManager.changeToPlayer(strongholdSelection, playerSelection);
                    playerSelection = (playerSelection + 1) % Global.MAX_PLAYERS;
                }


                // Flyway Settings
                GUILayout.Label("Flyway Debug Settings");

                // Flyway Selection
                GUILayout.Label("Flyway Selection");
                string[] flywaySelections = new string[Global.NUM_FLYWAY_EDGES + 1];
                for (int i = 0; i < Global.NUM_FLYWAY_EDGES; i++) flywaySelections[i] = i.ToString();
                flywaySelections[Global.NUM_FLYWAY_EDGES] = "All";
                int newFlywaySelection = GUILayout.SelectionGrid(flywaySelection, flywaySelections, 5);
                flywaySelection = newFlywaySelection;

                if (GUILayout.Button("Change to Vacant Space")) flywayManager.changeToVacant((uint) flywaySelection);
                if (GUILayout.Button("Change to Flyway")) flywayManager.changeToFlyway((uint) flywaySelection);
                if (GUILayout.Button("Toggle Flyway Player"))
                {
                    flywayManager.changeToPlayer((uint) flywaySelection, playerSelection);
                    playerSelection = (playerSelection + 1) % Global.MAX_PLAYERS;
                }

                GUILayout.EndScrollView();
                GUILayout.EndArea();
            } 
            
        }
    }
}