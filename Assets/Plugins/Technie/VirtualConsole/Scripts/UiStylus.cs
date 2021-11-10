using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Technie.VirtualConsole
{
	public class UiStylus : MonoBehaviour
	{
		// Public Properties

		public Material laserMaterial;
		public Material ballMaterial;
		public HandType handType;
		public WandInputModule inputModule;

		// Internal State

		private VrDebugDisplay[] displays;

		private GameObject sphereObj;
		private GameObject laserObj;

		private bool isButtonDown;

		void Start()
		{
			displays = GameObject.FindObjectsOfType(typeof(VrDebugDisplay)) as VrDebugDisplay[];

			sphereObj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
			sphereObj.transform.SetParent(this.transform, false);
			sphereObj.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
			GameObject.Destroy(sphereObj.GetComponent<Collider>());

			sphereObj.GetComponent<Renderer>().sharedMaterial = ballMaterial;


			laserObj = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
			laserObj.transform.SetParent(this.transform, false);
			laserObj.transform.localPosition = new Vector3(0.0f, 0.0f, 0.1f);
			laserObj.transform.localRotation = Quaternion.Euler(90.0f, 0.0f, 0.0f);
			laserObj.transform.localScale = new Vector3(0.001f, 0.1f, 0.001f);
			GameObject.Destroy(laserObj.GetComponent<Collider>());

			laserObj.GetComponent<Renderer>().sharedMaterial = laserMaterial;
		}

		void Update()
		{
			VrDebugDisplay display = FindClosestDisplay();
			if (display != null)
			{
				inputModule.SetRaycastActive(handType, true);
				

				Vector3 closestPos = display.FindClosestPoint(this.transform.position);
				Vector3 dir = (closestPos - this.transform.position).normalized;
				this.transform.rotation = Quaternion.LookRotation(dir);

				float height = display.DistanceTo(this.transform.position);
				if (isButtonDown)
				{
					if (height > 0.022f)
					{
						inputModule.LatchButtonUp(handType == HandType.Left ? 0 : 1);
						isButtonDown = false;
					}
				}
				else
				{
					if (height < 0.02f)
					{
						inputModule.LatchButtonDown(handType == HandType.Left ? 0 : 1);
						isButtonDown = true;
					}

				}
				
				bool hasPointer = inputModule.HasCurrentPointTarget(handType == HandType.Left ? 0 : 1);
				if (hasPointer)
				{
					float halfHeight = height / 2.0f;
					laserObj.transform.localPosition = new Vector3(0.0f, 0.0f, halfHeight);
					laserObj.transform.localScale = new Vector3(0.001f, halfHeight, 0.001f);

					SetVisualEnabled(true);
				}
				else
				{
					SetVisualEnabled(false);
				}
			}
			else
			{
				inputModule.SetRaycastActive(handType, false);
				SetVisualEnabled(false);
			}
		}

		private void SetVisualEnabled(bool visible)
		{
		//	sphereObj.SetActive(visible);
			laserObj.SetActive(visible);
		}

		private VrDebugDisplay FindClosestDisplay()
		{
			VrDebugDisplay closestDisplay = null;
			float closestDistance = float.MaxValue;

			foreach (VrDebugDisplay display in displays)
			{
				if (display.handType == this.handType)
					continue;

				float distance = display.DistanceTo(this.transform.position);
				if (distance < 0.08f && distance < closestDistance)
				{
					closestDisplay = display;
					closestDistance = distance;
				}
			}
			return closestDisplay;
		}

		public bool IsPointingAtPanel()
		{
			return FindClosestDisplay() != null;
		}
	}
}
