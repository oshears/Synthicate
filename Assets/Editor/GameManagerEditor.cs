
using UnityEngine;
using UnityEditor;
using Synthicate;

// [CustomEditor(typeof(DecalMeshHelper))]
#if (UNITY_EDITOR) 

[CustomEditor(typeof(GameManager))]
[CanEditMultipleObjects]
public class GameManagerEditor : Editor {
	
	// [Range(0,15)]
	// // SerializedProperty targetHex;
	// int targetHex;
	// [SerializeField]
	// bool 
	
	// HexController[] hexList;
	
	[SerializeField]
	bool m_HackerEnabled = false;
	[SerializeField]
	bool m_TranslucentViewEnabled = false;
	
	string targetHex = "0";
	uint targetHexNum = 0;
	
	void OnEnable()
	{
		// targetHex = serializedObject.FindProperty("targetHex");
	}
	
	public override void OnInspectorGUI() {
		base.OnInspectorGUI();
		
		serializedObject.UpdateIfRequiredOrScript();
		
		GameManager gameManager = (GameManager) target;
		
		GUILayout.Label("Debug Controls:");
		
		GUILayout.BeginHorizontal();
		GUILayout.Label("Target Hex Number");
		targetHex = GUILayout.TextField(targetHex);
		uint.TryParse(targetHex,out targetHexNum);
		GUILayout.EndHorizontal();
		
		GUILayout.BeginHorizontal();
		if(GUILayout.Button("Trigger Hack Event"))
		{
			gameManager.hexManagerSO.SetHackerCages(targetHexNum,true);
		}
		if(GUILayout.Button("Clear Hack Event"))
		{
			gameManager.hexManagerSO.SetHackerCages(targetHexNum,false);
		}
		GUILayout.EndHorizontal();
		
		GUILayout.BeginHorizontal();
		if(GUILayout.Button("Trigger Hex Selection Event"))
		{
			gameManager.hexManagerSO.hexSelectionEvent.Invoke(targetHexNum);
		}
		GUILayout.EndHorizontal();
		
		// if(GUILayout.Button("Toggle Hacker"))
		// {
		// 	m_HackerEnabled = !m_HackerEnabled;
		// 	hexController.SetHackerParticles(m_HackerEnabled);
		// 	hexController.SetHackerCage(m_HackerEnabled);
		// }
		
		// // m_HackerEnabled = !m_HackerEnabled;
		// // hexController.SetHackerParticles(m_HackerEnabled);
		// // hexController.SetHackerCage(m_HackerEnabled);
			
		// if(GUILayout.Button("Toggle Translucent Views"))
		// {
		// 	m_TranslucentViewEnabled = !m_TranslucentViewEnabled;
		// 	if (m_TranslucentViewEnabled) hexController.ChangeHackCageMaterialToSemiTranslucent(); 
		// 	else hexController.ChangeHackCageMaterialToHacked();
		// }
		
		// GUILayout.EndHorizontal();
		
		// // m_TranslucentViewEnabled = !m_TranslucentViewEnabled;
		// // if (m_TranslucentViewEnabled) hexController.ChangeHackCageMaterialToSemiTranslucent(); 
		// // else hexController.ChangeHackCageMaterialToHacked();
		
			
		serializedObject.ApplyModifiedProperties();
	}
	
}

#endif