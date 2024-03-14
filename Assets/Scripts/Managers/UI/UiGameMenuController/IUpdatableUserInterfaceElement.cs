using UnityEngine;

namespace Synthicate {

	/// <summary>
	/// A UI Element that can be refreshed to show newer values.
	/// </summary>
	public interface IUpdatableUserInterfaceElement
	{
		void UpdateUserInterfaceEventHandler();
		void InitilizeUserInterfaceEventHandler();
	}
}