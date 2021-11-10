using UnityEngine;
using System.Collections;

namespace Technie.VirtualConsole
{
	public class WarningExample : HandTrigger
	{
		private int numWarningsTriggered;
		
		public override void OnHandEntered()
		{
			numWarningsTriggered++;

			// Log this to two separate categories
			VrDebugStats.SetStat ("Gameplay", "Warnings triggered", numWarningsTriggered);
			VrDebugStats.SetStat ("Errors", "Warnings triggered", numWarningsTriggered);

			Debug.LogWarning("This is a warning message!");
		}
	}
}
