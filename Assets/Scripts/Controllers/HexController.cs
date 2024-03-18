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

		[Header("Scriptable Objects")]
		// private configuration
		[SerializeField]
		private HexManagerScriptableObject hexManager;


		enum HexControllerState {Normal, Hackable, Hacked, Static};
		HexControllerState state = HexControllerState.Normal;

		// MeshRenderer m_HexCageMeshRenderer;
		
		[SerializeField]
		Material m_TranslucentMaterial;
		[SerializeField]
		Material m_HackerMaterial;
		[SerializeField]
		Material m_SemiTranslucentMaterial;

		[SerializeField]
		BoxCollider m_HexCollider;
		
		[SerializeField]
		ParticleSystem m_HackerParticleSystem;
		
		[SerializeField]
		ParticleSystem m_SelectionParticleSystem;
		
		[SerializeField]
		Light m_HexLight;
		
		[SerializeField]
		GameObject m_HackerCage;
		
		[SerializeField]
		MeshRenderer m_HackerCageMeshRenderer;
		
		[SerializeField]
		TextMeshPro m_HexNumber;
		


		// Start is called before the first frame update
		void Start()
		{
			hexManager.BeginHackModeEvent.AddListener(configureHackOption);
			hexManager.EndHackModeEvent.AddListener(clearHackOption);
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
				m_HexNumber.text = hexValue.ToString();
			}
			else
			{
				m_HexNumber.text = "";
				state = HexControllerState.Static;
			}

			
			// Transform hexLoopTransform = Global.FindChildWithTag(transform,"hex_loop");
			if(m_HackerParticleSystem != null) m_HackerParticleSystem.Stop();

			// Transform hexLightTransform = Global.FindChildWithTag(transform, "hex_light");
			// if (hexLightTransform != null) m_HexLight.enabled = false;
			if(m_HexLight != null) m_HexLight.enabled = false;


			// Transform hexCageTransform = Global.FindChildTransformsWithTag(transform, "hex_cage");
			m_HackerCage.SetActive(false);
			
			// Transform hexSelectionTransform = Global.FindChildWithTag(transform, "hex_select");
			if(m_SelectionParticleSystem != null) m_SelectionParticleSystem.Stop();

			//for (int i = 0; i < transform.childCount; i++)
			//{
			//    Debug.Log(transform.GetChild(i).tag);
			//}

			// m_HexCollider = GetComponent<BoxCollider>();
			m_HexCollider.enabled = false;
		}

		private void OnEnable()
		{
			hexManager.hackerCageChangeEvent.AddListener(SetHackerCage);
			hexManager.hackerParticlesChangeEvent.AddListener(SetHackerParticles);
			hexManager.hexLightsChangeEvent.AddListener(SetHexLights);
			hexManager.hexSelectionEvent.AddListener(HexSelectionResponse);
		}

		private void OnDisable()
		{
			hexManager.hackerCageChangeEvent.RemoveListener(SetHackerCage);
			hexManager.hackerParticlesChangeEvent.RemoveListener(SetHackerParticles);
			hexManager.hexLightsChangeEvent.RemoveListener(SetHexLights);
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

		public void HexSelectionResponse(uint selection)
		{
			if (selection == hexValue)
			{
				EnableSelectionEffects(true);
			}
			else
			{
				EnableSelectionEffects(false);
			}
		}

		public void EnableSelectionEffects(bool enable)
		{

			if (enable)
				m_SelectionParticleSystem.Play();
			else
				m_SelectionParticleSystem.Stop();
		}

		public void SetHackerCage(uint selectedHex, bool enable)
		{
			if (selectedHex == id || selectedHex == Global.NUM_HEXES) m_HackerCage.SetActive(enable);
		}

		public void SetHackerCage(bool enable)
		{
			m_HackerCage.SetActive(enable);
		}

		public void ChangeHackCageMaterialToHacked()
		{
			m_HackerCageMeshRenderer.material = m_HackerMaterial;
		}

		public void ChangeHackCageMaterialToFullyTranslucent()
		{
			m_HackerCageMeshRenderer.material = m_TranslucentMaterial;
		}

		public void ChangeHackCageMaterialToSemiTranslucent()
		{
			m_HackerCageMeshRenderer.material = m_SemiTranslucentMaterial;

		}

		public void SetHackerParticles(bool enable)
		{
			// get child hex loop game object
			if (enable) m_HackerParticleSystem.Play();
			else if (!enable) m_HackerParticleSystem.Stop();
		}

		public void SetHackerParticles(uint selectedHex, bool enable)
		{
			// get child hex loop game object
			if (enable && (selectedHex == id || selectedHex == Global.NUM_HEXES)) m_HackerParticleSystem.Play();
			else if (!enable && (selectedHex == id || selectedHex == Global.NUM_HEXES)) m_HackerParticleSystem.Stop();
		}

		public void SetHexLights(bool enable)
		{
			m_HexLight.enabled = enable;
		}
		
		public void SetHexLights(uint selectedHex, bool enable)
		{
			if (selectedHex == id || selectedHex == Global.NUM_HEXES) m_HexLight.enabled = enable;
		}

		void configureHackOption()
		{
			if (state == HexControllerState.Normal)
			{
				SetHackerCage(true);
				ChangeHackCageMaterialToFullyTranslucent();
				state = HexControllerState.Hackable;
				m_HexCollider.enabled = true;
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
				SetHackerParticles(false);
				SetHackerCage(false);
				state = HexControllerState.Normal;
				
			}
			else if (state == HexControllerState.Hacked)
			{
				SetHackerParticles(true);
			}
			m_HexCollider.enabled = false;
		}

		private void OnMouseEnter()
		{
			if (state == HexControllerState.Hackable)
			{
				ChangeHackCageMaterialToSemiTranslucent();
			}
		}
		private void OnMouseExit()
		{
			if (state == HexControllerState.Hackable)
			{
				ChangeHackCageMaterialToFullyTranslucent();
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
			ChangeHackCageMaterialToHacked();
			hexManager.hackEvent.Invoke(id);

		}


	}
}

