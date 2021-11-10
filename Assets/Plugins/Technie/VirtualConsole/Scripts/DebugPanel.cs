using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Technie.VirtualConsole
{
	public class DebugPanel : MonoBehaviour
	{
		// Public Properties

		public string panelName;

		// Internal State

		private bool isAttached;

		public bool IsAttached()
		{
			return isAttached;
		}

		public void NotifyOnAttach()
		{
			isAttached = true;

			OnAttach();
		}

		public void NotifyOnDetach()
		{
			isAttached = false;

			OnDetach();
		}

		public virtual void OnAttach() { }
		public virtual void OnDetach() { }
		public virtual void OnResized(VrDebugDisplay.State size) { }
	}
}
