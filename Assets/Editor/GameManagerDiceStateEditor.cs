using UnityEngine;
using UnityEditor;
using Synthicate;

// [CustomEditor(typeof(DecalMeshHelper))]
#if (UNITY_EDITOR) 

[CustomEditor(typeof(GameManagerDiceState))]
[CanEditMultipleObjects]
public class GameManagerDiceStateEditor : Editor {
	
	// [Range(0,15)]
	// // SerializedProperty targetHex;
	// int targetHex;
	// [SerializeField]
	// bool 
	
	// HexController[] hexList;
	
	string m_FixedDiceValueString = "1";
	int m_FixedDiceValue = 1;
	bool m_UsingFixedDiceValue = false;
	
	void OnEnable()
	{
		// targetHex = serializedObject.FindProperty("targetHex");
	}
	
	public override void OnInspectorGUI() {
		
		// serializedObject.Update();
		serializedObject.UpdateIfRequiredOrScript();
		serializedObject.ApplyModifiedProperties();
		
		base.OnInspectorGUI();	
		
		
		GameManagerDiceState gameManagerDiceState = (GameManagerDiceState) target;
	
		GUILayout.Label("Debug Controls:");
		
		GUILayout.BeginHorizontal();
		GUILayout.Label("Dice Value");
		
		// Fixed dice value toggle.
		m_UsingFixedDiceValue = GUILayout.Toggle(m_UsingFixedDiceValue, "Fixed Dice Value");
		
		// Fixed dice value string to int.
		m_FixedDiceValueString = GUILayout.TextField(m_FixedDiceValueString);
		int.TryParse(m_FixedDiceValueString, out m_FixedDiceValue);
		
		GUILayout.EndHorizontal();
		
		gameManagerDiceState.m_FixedDiceValue = m_FixedDiceValue;
		gameManagerDiceState.m_UsingFixedDice = m_UsingFixedDiceValue;
		
		serializedObject.ApplyModifiedProperties();
	}
	
}

#endif