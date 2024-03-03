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
		[SerializeField]
		protected UiScriptableObject userInterfaceSO;
		#endregion
		
		protected virtual void Awake() {
			userInterfaceSO.initializeUserInterfaceEvent += InitilizeUserInterfaceEventHandler;
			userInterfaceSO.updateUserInterfaceEvent += UpdateUserInterfaceEventHandler;
		}
		
		protected abstract void UpdateUserInterfaceEventHandler();
		protected abstract void InitilizeUserInterfaceEventHandler();
	}
}