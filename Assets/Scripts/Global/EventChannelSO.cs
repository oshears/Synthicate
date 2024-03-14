using UnityEngine;
using UnityEngine.Events;

namespace Synthicate
{
	
	[CreateAssetMenu(menuName = "Events/Event Channel", fileName = "EventChannel")]
	public class EventChannelSO : ScriptableObject
	{
		public delegate void EventHandler();
		
		[Tooltip("The action to perform")]
		public event EventHandler OnEventRaised;
		
		public void RaiseEvent()
		{
			OnEventRaised.Invoke();
		}
	}
}