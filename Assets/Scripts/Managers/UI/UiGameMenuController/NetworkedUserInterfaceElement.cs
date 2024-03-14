using UnityEngine;
using Unity.Netcode;

namespace Synthicate {

	/// <summary>
	/// A UI Element that can be refreshed to show newer values.
	/// </summary>
	public abstract class NetworkedUserInterfaceElement : NetworkBehaviour
	{
		[Header("Scriptable Objects")]
		
		[SerializeField]
		protected GameManagerSO gameManagerSO;
		
		[SerializeField]
		protected UiScriptableObject userInterfaceSO;
		
		// protected virtual void Awake() {
			
		// }
		
		// protected virtual void IUpdatableUserInterfaceElement.UpdateUserInterfaceEventHandler()
		// {
			
		// }
		// protected abstract void IUpdatableUserInterfaceElement.InitilizeUserInterfaceEventHandler()
		// {
			
		// }
	}
}