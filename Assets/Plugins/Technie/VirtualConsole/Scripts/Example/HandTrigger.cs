using UnityEngine;
using System.Collections;

namespace Technie.VirtualConsole
{
	public class HandTrigger : MonoBehaviour
	{
		public BoxCollider area;

		private XrNodeRig xrRig;

		private bool wasInBox;

		void Start ()
		{
			xrRig = GameObject.FindObjectOfType<XrNodeRig>();
		}

		void Update ()
		{
			if (xrRig != null)
			{
				bool isInBox = IsInBox(xrRig.leftHandTransform.gameObject) || IsInBox(xrRig.rightHandTransform.gameObject);

				bool sendEvent = (isInBox && !wasInBox);
				wasInBox = isInBox;

				if (sendEvent)
				{
					OnHandEntered();
				}
			}
		}

		private bool IsInBox(GameObject obj)
		{
			if (obj == null)
				return false;

			return area.bounds.Contains (obj.transform.position);
		}

		public virtual void OnHandEntered()
		{
			
		}
	}
}
