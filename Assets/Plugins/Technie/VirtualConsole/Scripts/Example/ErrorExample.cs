using UnityEngine;
using System.Collections;

namespace Technie.VirtualConsole
{

	public class ErrorExample : HandTrigger
	{
		private int numErrorsTriggered;

		public override void OnHandEntered()
		{
			Debug.LogError("This is an example error message!");

			numErrorsTriggered++;
			VrDebugStats.SetStat ("Errors", "Errors triggered", numErrorsTriggered);
			
			VrDebugStats.SetStat("Errors", "Test Stat 00", numErrorsTriggered);
			VrDebugStats.SetStat("Errors", "Test Stat 01", numErrorsTriggered);
			VrDebugStats.SetStat("Errors", "Test Stat 02", numErrorsTriggered);
			VrDebugStats.SetStat("Errors", "Test Stat 03", numErrorsTriggered);
			VrDebugStats.SetStat("Errors", "Test Stat 04", numErrorsTriggered);
			VrDebugStats.SetStat("Errors", "Test Stat 05", numErrorsTriggered);
			VrDebugStats.SetStat("Errors", "Test Stat 06", numErrorsTriggered);
			VrDebugStats.SetStat("Errors", "Test Stat 07", numErrorsTriggered);
			VrDebugStats.SetStat("Errors", "Test Stat 08", numErrorsTriggered);
			VrDebugStats.SetStat("Errors", "Test Stat 09", numErrorsTriggered);
			VrDebugStats.SetStat("Errors", "Test Stat 10", numErrorsTriggered);
			VrDebugStats.SetStat("Errors", "Test Stat 11", numErrorsTriggered);
			VrDebugStats.SetStat("Errors", "Test Stat 12", numErrorsTriggered);
			VrDebugStats.SetStat("Errors", "Test Stat 13", numErrorsTriggered);
			VrDebugStats.SetStat("Errors", "Test Stat 14", numErrorsTriggered);
			VrDebugStats.SetStat("Errors", "Test Stat 15", numErrorsTriggered);
			VrDebugStats.SetStat("Errors", "Test Stat 16", numErrorsTriggered);
			VrDebugStats.SetStat("Errors", "Test Stat 17", numErrorsTriggered);
			VrDebugStats.SetStat("Errors", "Test Stat 18", numErrorsTriggered);
			VrDebugStats.SetStat("Errors", "Test Stat 19", numErrorsTriggered);
			VrDebugStats.SetStat("Errors", "Test Stat 20", numErrorsTriggered);
			VrDebugStats.SetStat("Errors", "Test Stat 21", numErrorsTriggered);
			VrDebugStats.SetStat("Errors", "Test Stat 22", numErrorsTriggered);
			VrDebugStats.SetStat("Errors", "Test Stat 23", numErrorsTriggered);
			VrDebugStats.SetStat("Errors", "Test Stat 24", numErrorsTriggered);
			VrDebugStats.SetStat("Errors", "Test Stat 25", numErrorsTriggered);
			VrDebugStats.SetStat("Errors", "Test Stat 26", numErrorsTriggered);
			VrDebugStats.SetStat("Errors", "Test Stat 27", numErrorsTriggered);
			VrDebugStats.SetStat("Errors", "Test Stat 28", numErrorsTriggered);
			VrDebugStats.SetStat("Errors", "Test Stat 29", numErrorsTriggered);
			VrDebugStats.SetStat("Errors", "Test Stat 30", numErrorsTriggered);
			VrDebugStats.SetStat("Errors", "Test Stat 31", numErrorsTriggered);
			VrDebugStats.SetStat("Errors", "Test Stat 32", numErrorsTriggered);
			VrDebugStats.SetStat("Errors", "Test Stat 33", numErrorsTriggered);
			VrDebugStats.SetStat("Errors", "Test Stat 34", numErrorsTriggered);
			VrDebugStats.SetStat("Errors", "Test Stat 35", numErrorsTriggered);
			VrDebugStats.SetStat("Errors", "Test Stat 36", numErrorsTriggered);
			VrDebugStats.SetStat("Errors", "Test Stat 37", numErrorsTriggered);
			VrDebugStats.SetStat("Errors", "Test Stat 38", numErrorsTriggered);
			VrDebugStats.SetStat("Errors", "Test Stat 39", numErrorsTriggered);
			VrDebugStats.SetStat("Errors", "Test Stat 40", numErrorsTriggered);
		}
	}
}
