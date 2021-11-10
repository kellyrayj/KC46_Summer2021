using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Technie.VirtualConsole
{

	public enum HandType
	{
		Invalid,
		Left,
		Right
	}
	
	public class Hand : MonoBehaviour
	{
		public HandType type = HandType.Invalid;
		public Transform trackedTransform = null;

		void LateUpdate()
		{
			TrackTargetHand ();
		}
		
		private void TrackTargetHand()
		{
			if (trackedTransform != null)
			{
				this.transform.position = trackedTransform.position;
				this.transform.rotation = trackedTransform.rotation * Quaternion.Euler(90.0f, 0.0f, 0.0f);
			}
		}
	}
}
