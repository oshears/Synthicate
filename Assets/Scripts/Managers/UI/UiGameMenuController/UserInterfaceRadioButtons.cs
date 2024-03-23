using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Synthicate
{
	public class UserInterfaceRadioButtons : MonoBehaviour
	{
		
		[SerializeField]
		Button[] m_RadioButtons;
		
		[SerializeField]
		Image[] m_ButtonImages;
		
		// [Header("Event Channels")]
		
		// [SerializeField] IntEventChannelSO m_ 
		
		public delegate void RadioButtonClickedEventHandler(int i); 
		
		public event RadioButtonClickedEventHandler e_RadioButtonClickedEvent;
		
		
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
			ResetButtons();
			for(int i = 0; i < m_RadioButtons.Length; i++)
			{
				SetupButton(i);
			}
		}
		
		void SetupButton(int i)
		{
			m_RadioButtons[i].onClick.AddListener(() => 
			{
				OnButtonClicked(i);
				// RadioButtonClickedEvent.Invoke(i);
			});
		}
		
		void OnButtonClicked(int buttonIndex)
		{
			if (e_RadioButtonClickedEvent != null) e_RadioButtonClickedEvent.Invoke(buttonIndex);
			
			for(int i = 0; i < m_RadioButtons.Length; i++)
			{
				if (i == buttonIndex)
				{
					m_ButtonImages[i].color = Color.green;
				}
				else
				{
					m_ButtonImages[i].color = Color.white;
				}
			}
		}
		
		void ResetButtons()
		{
			m_ButtonImages[0].color = Color.green;
			
			for(int i = 1; i < m_RadioButtons.Length; i++)
			{
				m_ButtonImages[i].color = Color.white;
			}
		}
	}
}