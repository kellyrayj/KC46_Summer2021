using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Technie.VirtualConsole
{
	public class HandAbstraction : MonoBehaviour
	{
		public VirtualConsole virtualConsole;

		public Material ballMaterial;
		public Material laserMaterial;

		public Sprite cursorSprite;
		public Material cursorMaterial;

		public XrNodeRig xrRig;
		public PanelManager panelManager;
		
		// Internal State

		private WandInputModule wandInputModule;
		
		private Hand leftHand, rightHand;
		
		private UiStylus leftStylus;
		private UiStylus rightStylus;

		private bool wasLeftDown;
		private bool wasRightDown;
		
		private float findHandsTimer;

		void OnEnable()
		{

		}
		
		void OnDisable()
		{

		}

		void Start()
		{
			// Always call this on start so we refetch hands if we're dynamically loaded as a prefab after SteamVR has sent all of it's controller events

			FindHands();
		}

		private void FindHands()
		{
			// Lazily create the wand input module

			if (wandInputModule == null)
			{
				GameObject eventSystemObj = new GameObject("Isolated Event System");
				eventSystemObj.transform.SetParent(this.transform, false);

				eventSystemObj.gameObject.AddComponent<IsolatedEventSystem>();

				wandInputModule = eventSystemObj.gameObject.AddComponent<WandInputModule>();
				wandInputModule.CursorSprite = cursorSprite;
				wandInputModule.CursorMaterial = cursorMaterial;
			}

			// Lazily create the styluses

			if (leftStylus == null)
			{
				leftStylus = CreateStylus(HandType.Left, xrRig.leftHandTransform, wandInputModule);
				
				SetStylusPosition(leftStylus, HandType.Left, virtualConsole.leftStylusPosition, virtualConsole.customLeftStylusPosition);
			}

			if (rightStylus == null)
			{
				rightStylus = CreateStylus(HandType.Right, xrRig.rightHandTransform, wandInputModule);

				SetStylusPosition(rightStylus, HandType.Right, virtualConsole.rightStylusPosition, virtualConsole.customRightStylusPosition);
			}

			wandInputModule = GameObject.FindObjectOfType(typeof(WandInputModule)) as WandInputModule;
			wandInputModule.SetRaycastOrigins(leftStylus.transform, rightStylus.transform);

			wandInputModule.OnHandsDetected(this); // Needed to initialise input module

			panelManager.OnHandsDetected(wandInputModule.GetControllerCamera());
		}

		private UiLaser CreateLaser()
		{
			GameObject laser = new GameObject ("Ui Laser");
			laser.transform.SetParent (this.transform, false);

			UiLaser laserComponent = laser.AddComponent<UiLaser> ();
			laserComponent.CreateBeam (laserMaterial);

			return laserComponent;
		}

		private UiStylus CreateStylus(HandType type, Transform handTransform, WandInputModule inputModule)
		{
			GameObject obj = new GameObject("Ui Stylus");
			obj.transform.SetParent(handTransform, false);

			UiStylus stylus = obj.AddComponent<UiStylus>();
			stylus.laserMaterial = laserMaterial;
			stylus.ballMaterial = ballMaterial;
			stylus.handType = type;
			stylus.inputModule = inputModule;

			return stylus;
		}

		private void SetStylusPosition(UiStylus stylus, HandType hand, StylusPosition stylusPosition, Vector3 customStylusPosition)
		{
			switch (stylusPosition)
			{
				case StylusPosition.Top:
				{
					stylus.transform.localPosition = new Vector3(0.0f, 0.0f, 0.05f);
					break;
				}
				case StylusPosition.Bottom:
				{
					stylus.transform.localPosition = new Vector3(0.0f, -0.01f, -0.18f);
					break;
				}
				case StylusPosition.Left:
				{
					stylus.transform.localPosition = new Vector3(-0.085f, -0.01f, -0.01f);
					break;
				}
				case StylusPosition.Right:
				{
					stylus.transform.localPosition = new Vector3( 0.085f, -0.01f, -0.01f);
					break;
				}
				case StylusPosition.Custom:
				{
					stylus.transform.localPosition = customStylusPosition;
					break;
				}
			}
		}
		
		public GameObject GetLeftHand()
		{
			return null;
		}

		public GameObject GetRightHand()
		{
			return null;
		}

		public bool HasTarget(HandType targetHand)
		{
			if (targetHand == HandType.Left)
			{
				return leftStylus != null ? leftStylus.IsPointingAtPanel() : false;
			}
			else if (targetHand == HandType.Right)
			{
				return rightStylus != null ? rightStylus.IsPointingAtPanel() : false;
			}
			return false;
		}

		public void TriggerInput(HandType targetHand)
		{
			// No longer used
		}

		public void SetStylusVisible(bool visible)
		{
			if (leftStylus != null)
				leftStylus.gameObject.SetActive(visible);
			if (rightStylus != null)
				rightStylus.gameObject.SetActive(visible);

			if (wandInputModule != null)
				wandInputModule.SetCursorsVisible(visible);
		}
	}
}
