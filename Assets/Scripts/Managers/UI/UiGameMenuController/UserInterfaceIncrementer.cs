using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Synthicate
{
	public class UserInterfaceIncrementer : MonoBehaviour
	{
		
		[SerializeField]
		public Button m_IncrementButton;
		
		[SerializeField]
		public Button m_DecrementButton;
		
		[SerializeField]
		TextMeshProUGUI m_AmountText;
		
		[SerializeField]
		Image resourceIcon;
		
		
		public delegate void AmountChangedEventHandler();
		public event AmountChangedEventHandler e_AmountChanged;
		
		int m_Amount;
		
		void Awake()
		{
			
		}
		
		void Start()
		{
			m_IncrementButton.onClick.AddListener(IncrementAmount);
			m_DecrementButton.onClick.AddListener(DecrementAmount);
		}
		
		void OnEnable()
		{
			ResetAmount();
		}
		
		public void ResetAmount()
		{
			m_Amount = 0;
			m_AmountText.text = "0";
		}
		
		void IncrementAmount()
		{
			m_Amount++;
			e_AmountChanged.Invoke();
		}
		
		void DecrementAmount()
		{
			m_Amount = (m_Amount <= 0) ? 0 : m_Amount - 1;
			e_AmountChanged.Invoke();
		}
		
		public int GetAmount() => m_Amount;
		
		public void SetIcon(Sprite sprite) => resourceIcon.sprite = sprite;
		
	}
}