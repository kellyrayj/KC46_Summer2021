using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

namespace Technie.VirtualConsole
{
	public class IsolatedEventSystem : EventSystem
	{
		protected override void OnEnable()
		{
			// Older Unity verions (~5.1) required this to be an empty function so that the base class doesn't assign EventSystem.current
			// Newer Unity versions (5.5+) require the base function to be called so that events are dispatched correctly
			// We could #ifdef this, but there's no reliable defines that work across all the versions we support
			// So we'll leave the base function called, and anyone on an ancient version of unity can take it back out again
			base.OnEnable();
		}
		
		protected override void Update()
		{
			EventSystem originalCurrent = EventSystem.current;
			current = this;
			base.Update();
			current = originalCurrent;
		}
	}
}
