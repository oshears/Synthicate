using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Synthicate
{
	public class DepotController : MonoBehaviour
	{
		//public enum ResourceType { People, Power, Mech, Metal, Food, Any };
		public ResourceType depotResource = ResourceType.People;
		public uint id;

		public DepotManagerScriptableObject depotManager;

		public enum DepotState {Active,Inactive};
		DepotState depotState = DepotState.Inactive;

		[SerializeField]
		Transform depotIndicatorTransform;
		
		[SerializeField]
		MeshRenderer depotIndicatorRenderer;
		
		[SerializeField]
		Transform selectionCubeTransform;
		//BoxCollider depotCollider;

		private uint tradeAmount = 2;
		
		[SerializeField]
		DepotSelectedEventChannelSO m_DepotSelectedEventChannel;
		
		[SerializeField]
		BoolEventChannelSO m_EnableDepotSelectionEventChannel;
		
		void Awake()
		{
			m_EnableDepotSelectionEventChannel.OnEventRaised += EnableDepotSelectionEventHandler;
			depotState = DepotState.Inactive;
		}


		// Start is called before the first frame update
		void Start()
		{
			//for (int i = 0; i < transform.childCount; i++) Debug.Log(transform.GetChild(i).tag);


			// depotIndicatorTransform = transform.GetChild(0);
			// Debug.Assert(depotIndicatorTransform != null, "Could not find indicator transform for depot" + id);
			// depotIndicatorRenderer = depotIndicatorTransform.GetChild(0).GetComponent<MeshRenderer>();
			// Debug.Assert(depotIndicatorRenderer != null, "Could not find indicator mesh renderer for depot" + id);

			Material[] mats = depotIndicatorRenderer.materials;
			switch (depotResource) { 
				case ResourceType.People:
					mats[1] = depotManager.cityMaterial;
					break;
				case ResourceType.Power:
					mats[1] = depotManager.powerplantMaterial;
					break;
				case ResourceType.Mech:
					mats[1] = depotManager.scrapyardMaterial;
					break;
				case ResourceType.Metal:
					mats[1] = depotManager.mineMaterial;
					break;
				case ResourceType.Food:
					mats[1] = depotManager.outlandsMaterial;
					break;
				case ResourceType.Any:
					mats[1] = depotManager.randomMaterial;
					break;
				default:
					mats[1] = depotManager.cityMaterial;
					break;
			}
			depotIndicatorRenderer.materials = mats;

			// selectionCubeTransform = Global.FindChildTransformsWithTag(transform, "selection_cube");

			//depotCollider = GetComponent<BoxCollider>();
			if (depotResource == ResourceType.Any) tradeAmount = 3;
			else tradeAmount = 2;

		}

		// Update is called once per frame
		void Update()
		{

			// update depot indicator rotation

			GameObject mainCamera = GameObject.FindGameObjectWithTag("MainCamera");

			if (mainCamera != null) 
			{
				Quaternion rotateToCamera = Quaternion.LookRotation((depotIndicatorTransform.position - mainCamera.transform.position) * Time.deltaTime);
				Quaternion newRotation = Quaternion.Euler(rotateToCamera.eulerAngles + Vector3.right * 180);
				depotIndicatorTransform.rotation = newRotation;
			}
			
		}

		void SetSelectionCube(bool enable) => selectionCubeTransform.gameObject.SetActive(enable);

		private void OnMouseEnter()
		{
			if (depotState == DepotState.Active) SetSelectionCube(true);
		}

		private void OnMouseExit()
		{
			if (depotState == DepotState.Active) SetSelectionCube(false);
		}

		private void OnMouseUpAsButton()
		{
			// depotManager.playerClickEvent.Invoke(id, depotResource, tradeAmount);
			if (depotState == DepotState.Active)
			{
				m_DepotSelectedEventChannel.RaiseEvent(new DepotSelection((int) id, depotResource, (int) tradeAmount));
			}
		}
		
		void EnableDepotSelectionEventHandler(bool enable)
		{
			depotState = enable ? DepotState.Active : DepotState.Inactive;
			if (enable == false) SetSelectionCube(false);
		}
	}
}
