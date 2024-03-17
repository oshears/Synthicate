
using UnityEngine;
using UnityEditor;
using Synthicate;

// [CustomEditor(typeof(DecalMeshHelper))]
#if (UNITY_EDITOR) 

[CustomEditor(typeof(HexController))]
[CanEditMultipleObjects]
public class HexControllerEditor : Editor {
	
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
	
	[SerializeField]
	bool m_SelectionEnabled = false;
	
	void OnEnable()
	{
		// targetHex = serializedObject.FindProperty("targetHex");
	}
	
	public override void OnInspectorGUI() {
		base.OnInspectorGUI();
		
		serializedObject.UpdateIfRequiredOrScript();
		
		HexController hexController = (HexController) target;
		
		GUILayout.Label("Debug Controls:");
		
		// GUILayout.BeginHorizontal();
		
		m_HackerEnabled = GUILayout.Toggle(m_HackerEnabled,"Toggle Hacker");
		hexController.SetHackerParticles(m_HackerEnabled);
		hexController.SetHackerCage(m_HackerEnabled);
		
		m_SelectionEnabled = GUILayout.Toggle(m_SelectionEnabled,"Toggle Selection");
		hexController.EnableSelectionEffects(m_SelectionEnabled);
		
		// m_TranslucentViewEnabled = GUILayout.Toggle(m_HackerEnabled,"Toggle Translucent View");
		// if (m_TranslucentViewEnabled) hexController.ChangeHackCageMaterialToSemiTranslucent(); 
		// else hexController.ChangeHackCageMaterialToHacked();
		
		GUILayout.Label("Hacker Cage Material: ");
		GUILayout.BeginHorizontal();
		if(GUILayout.Button("Hacker (Red)"))
		{
			hexController.ChangeHackCageMaterialToHacked();
		}
		if(GUILayout.Button("Semi-Translucent"))
		{
			hexController.ChangeHackCageMaterialToSemiTranslucent();
		}
		if(GUILayout.Button("Translucent"))
		{
			hexController.ChangeHackCageMaterialToFullyTranslucent();
		}
		GUILayout.EndHorizontal();
		
		// if(GUILayout.Button("Toggle Hacker"))
		// {
			
		// }
		
		// m_HackerEnabled = !m_HackerEnabled;
		// hexController.SetHackerParticles(m_HackerEnabled);
		// hexController.SetHackerCage(m_HackerEnabled);
			
		// if(GUILayout.Button("Toggle Translucent Views"))
		// {
		// 	m_TranslucentViewEnabled = !m_TranslucentViewEnabled;
		// 	if (m_TranslucentViewEnabled) hexController.ChangeHackCageMaterialToSemiTranslucent(); 
		// 	else hexController.ChangeHackCageMaterialToHacked();
		// }
		
		// GUILayout.EndHorizontal();
		
		// m_TranslucentViewEnabled = !m_TranslucentViewEnabled;
		// if (m_TranslucentViewEnabled) hexController.ChangeHackCageMaterialToSemiTranslucent(); 
		// else hexController.ChangeHackCageMaterialToHacked();
		
			
		serializedObject.ApplyModifiedProperties();
	}
	
}

#endif