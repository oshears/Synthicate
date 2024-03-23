using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Synthicate
{
	public class UserInterfaceAnyDepotTradeWindow : MonoBehaviour
	{
		
		[SerializeField] UserInterfaceRadioButtons m_GivingResourceRadioButtons;
		[SerializeField] UserInterfaceRadioButtons m_ReceivingResourceRadioButtons;
		
		[Header("Text")]
		[SerializeField] TextMeshProUGUI m_InvalidTradeText;
		
		[Header("Buttons")]
		[SerializeField] Button m_ConfirmTradeButton;
		[SerializeField] Button m_CancelTradeButton;
		
		[Header("Event Channels")]
		
		[SerializeField] EventChannelSO m_TradeExecutedEventChannel;
		[SerializeField] EventChannelSO m_CancelButtonEventChannel;
		[SerializeField] DepotSelectedEventChannelSO m_DepotSelectedEventChannel;
		
		[Header("Scriptable Objects")]
		[SerializeField] GameManagerSO m_GameManagerSO;
		
		ResourceType m_GivingResourceSelection = ResourceType.Power;
		ResourceType m_ReceivingResourceSelection = ResourceType.Power;
		
		void Awake()
		{
			
		}
		
		void Start()
		{
			// m_IncrementButton.onClick.AddListener(IncrementAmount);
			// m_DecrementButton.onClick.AddListener(DecrementAmount);
		}
		
		void OnEnable()
		{
			m_DepotSelectedEventChannel.OnEventRaised += DepotSelectedEventHandler;
			
			m_GivingResourceRadioButtons.e_RadioButtonClickedEvent += GivingRadioButtonClickedEventHandler;
			m_ReceivingResourceRadioButtons.e_RadioButtonClickedEvent += ReceivingRadioButtonClickedEventHandler;
			
			m_ConfirmTradeButton.onClick.AddListener(ConfirmTradeButtonEventHandler);
			
			m_CancelTradeButton.onClick.AddListener(CancelTradeButtonEventHandler);
			
			m_InvalidTradeText.text = "";
			
		}
		
		void ConfirmTradeButtonEventHandler()
		{
			// ResetCounts();
			// Debug.Log($"Giving: {m_GivingAmount}");
			// Debug.Log($"Receiving: {m_ReceivingAmounts}");
			
			// If trading at a standard depot
			// Calculate total number of resources a player must give
			// int totalRequiredAmount = 0;
			// foreach (int receivingAmount in m_ReceivingAmounts)
			// {
			// 	totalRequiredAmount += receivingAmount;
			// }
			
			// Check that the player has the required number of resources for the trade
			if (m_GameManagerSO.clientPlayer.resources[(int) m_GivingResourceSelection] >= 3)
			{
				
				// Remove the resources from the player's inventory
				// Debug.Log("Requested Resource: " + m_TradeResource);
				m_GameManagerSO.clientPlayer.RemoveResources(m_GivingResourceSelection,3);
				
				// Give the new resources to the player
				m_GameManagerSO.clientPlayer.AddResources(m_ReceivingResourceSelection,1);
				
				// Raise trade executed event
				// ResetCounts();
				m_TradeExecutedEventChannel.RaiseEvent();
			}
			else
			{
				Debug.Log("Client does not have enough resources for this trade!");
				m_InvalidTradeText.text = "Insufficient Resources!";
			}
			
			m_InvalidTradeText.text = "Insufficient Resources!";
		}
		
		void DepotSelectedEventHandler(DepotSelection selection)
		{
			// ResetCounts();
			// m_TradeResource = selection.Resource;
			// tradeOfferAmount.text = $"{selection.RequiredAmount}";
			// m_DepotResourceImage.sprite = m_UserInterfaceScriptableObject.ResourceSprites[(int) selection.Resource];
		}
		
		void CancelTradeButtonEventHandler()
		{
			m_CancelButtonEventChannel.RaiseEvent();
		}
		
		void GivingRadioButtonClickedEventHandler(int i)
		{
			m_GivingResourceSelection = (ResourceType) i;
		}
		
		void ReceivingRadioButtonClickedEventHandler(int i)
		{
			m_ReceivingResourceSelection = (ResourceType) i;
		}
		
		
	}
}