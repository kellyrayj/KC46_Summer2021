using UnityEngine;
using System.Collections;

namespace Technie.VirtualConsole
{
	public class ExceptionExample : HandTrigger
	{
		public override void OnHandEntered()
		{
			VrDebugStats.SetStat ("Example", "Was exception triggered", true);

			throw new System.NullReferenceException("This is an example null reference exception");	
		}
	}
}
