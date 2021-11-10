using UnityEngine;
using System.Collections;

namespace Technie.VirtualConsole
{
	public class InfoExample : HandTrigger
	{
		private int nextBallNumber = 1;

		public override void OnHandEntered()
		{
			// This message will go to the console and the vr console
			Debug.Log ("Spawning ball number " + nextBallNumber);

			// This creates/updates a debug stat for the vr stats panel
			VrDebugStats.SetStat("Gameplay", "Num balls", nextBallNumber);

			// Now actually spawn the ball
			GameObject ball = GameObject.CreatePrimitive (PrimitiveType.Sphere);
			ball.name = "Ball " + nextBallNumber;
			ball.transform.position = this.transform.position;
			ball.transform.localScale = new Vector3 (0.1f, 0.1f, 0.1f);
			ball.AddComponent<Rigidbody> ();

			nextBallNumber++;
		}
	}
}
