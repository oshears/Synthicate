using UnityEngine;
using UnityEngine.Events;

namespace Synthicate
{
	public abstract class GenericEventChannelSO<T>: ScriptableObject
	{
		public delegate void EventHandler(T parameter);
		
		[Tooltip("The action to perform")]
		public event EventHandler OnEventRaised;

		public void RaiseEvent(T parameter)
		{
			OnEventRaised.Invoke(parameter);
		}
	}
}