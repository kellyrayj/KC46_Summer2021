using UnityEngine;
using System.Collections;

namespace Technie.VirtualConsole
{
	public class UiLaser : MonoBehaviour
	{
		[ReadOnly]
		public GameObject targetHand;

		private WandInputModule inputModule;

		private GameObject cylinder;

		private VrDebugDisplay[] displays;

		private float lastDistance = 0.4f;

		void Start()
		{
			displays = GameObject.FindObjectsOfType (typeof(VrDebugDisplay)) as VrDebugDisplay[];
		}

		public void CreateBeam(Material material)
		{
			cylinder = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
			cylinder.transform.SetParent (this.transform, false);
			
			GameObject.Destroy (cylinder.GetComponent<Collider>());
			
			cylinder.SetActive (false);

			cylinder.GetComponent<MeshRenderer> ().material = material;
		}

		public void OnHandDetected(int index, GameObject handObj, WandInputModule wandInputModule)
		{
			this.targetHand = handObj;
			this.inputModule = wandInputModule;
		}

		void Update ()
		{
			if (targetHand != null)
			{
			//	int inputIndex = inputModule.GetHandToLocalIndex(targetHand);
				int inputIndex = 0;
				bool hasPointer = inputModule.HasCurrentPointTarget(inputIndex);

				// What's the distance to the pointer (if we have one)
				float distance = lastDistance;
				if (hasPointer)
				{
					Vector3 cursorPos = inputModule.GetCurrentPointPosition(inputIndex);
					distance = (targetHand.transform.position - cursorPos).magnitude;

					lastDistance = Mathf.Clamp(distance, 0.1f, 0.6f);
				}

				// Set the laser as visible if we've got a pointer or if we're pointing in the right direction
				bool isPointingAtPanel = IsPointingAtPanel();
				cylinder.SetActive(hasPointer || isPointingAtPanel);

				// Position ourselves
				this.transform.position = targetHand.transform.position;
				this.transform.rotation = targetHand.transform.rotation;

				// Position the laser
				cylinder.transform.localScale = new Vector3(0.002f, distance * 0.5f, 0.002f);
				cylinder.transform.rotation = targetHand.transform.rotation * Quaternion.Euler(90.0f, 0.0f, 0.0f);
				cylinder.transform.position = targetHand.transform.position + targetHand.transform.forward * distance * 0.5f;
			}
			else
			{
				cylinder.SetActive(false);
			}
		}

		private float CalcOpacity()
		{
			float opacity = 0.0f;

			for (int i=0; i<displays.Length; i++)
			{
				opacity = Mathf.Max(CalcOpacity(displays[i]), opacity);
			}

			return opacity;
		}

		private float CalcOpacity(VrDebugDisplay display)
		{
			float distance = (this.transform.position - display.center.position).magnitude;
			distance = Mathf.Clamp (distance, 0.0f, 1.0f);

			return distance;
		}

		public bool IsPointingAtPanel()
		{
			bool isAtPanel = false;

			for (int i=0; i<displays.Length; i++)
			{
				isAtPanel |= IsPointingAtPanel(displays[i]);
			}

			return isAtPanel;
		}

		private bool IsPointingAtPanel(VrDebugDisplay display)
		{
			float angle = Vector3.Angle (targetHand.transform.forward, display.center.forward);
			float dot = Vector3.Dot (targetHand.transform.forward, display.center.forward);
			return Mathf.Abs (angle) < 30.0f && dot < 0.0f;
		}
	}
}
