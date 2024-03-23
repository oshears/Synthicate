using UnityEngine;

namespace Synthicate {

	/// <summary>
	/// A UI Element that can be refreshed to show newer values.
	/// </summary>
	public abstract class UiUpdatableElementMonoBehavior : MonoBehaviour
	{
		#region Scriptable Objects
		[SerializeField]
		protected GameManagerSO gameManagerSO;
		
		[SerializeField] protected EventChannelSO m_InitializeUserInterfaceEventChannel;
		[SerializeField] protected EventChannelSO m_UpdateUserInterfaceEventChannel;
		
		#endregion
		
		protected virtual void Awake() {
			m_InitializeUserInterfaceEventChannel.OnEventRaised += InitilizeUserInterfaceEventHandler;
			m_UpdateUserInterfaceEventChannel.OnEventRaised += UpdateUserInterfaceEventHandler;
		}
		
		protected abstract void UpdateUserInterfaceEventHandler();
		protected abstract void InitilizeUserInterfaceEventHandler();
	}
}