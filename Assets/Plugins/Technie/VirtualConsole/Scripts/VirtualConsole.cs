using UnityEngine;
using System.Collections;

namespace Technie.VirtualConsole
{
	public enum StylusPosition
	{
		Bottom,
		Left,
		Right,
		Top,
		Custom
	}

	public enum CameraDetectionMode
	{
		Automatic,
		UseExplicitCameraReference,
		UseExplicitCameraName,
	}
	
	/** This class serves to expose the public API for a host game to interact with the Virtual Console.
	 *  Note to self - assume all public functions in here are callable by the host game.
	 *  Note to users - *only* call public functions in this class. Calling other functions will result in undefined behaviour.
	 */
	public class VirtualConsole : MonoBehaviour
	{
		public bool onlyInDebugBuilds = true;

		[Header("Stylus Locations")]
		public StylusPosition leftStylusPosition;
		public StylusPosition rightStylusPosition;

		[Header("Custom Locations")]
		public Vector3 customLeftStylusPosition;
		public Vector3 customRightStylusPosition;

		[Header("Camera Detection")]
		public CameraDetectionMode cameraDetectionMode = CameraDetectionMode.Automatic;

		public Camera explicitVrCameraReference;
		public string explicitVrCameraName;
		
		// Singleton

		private static VirtualConsole instance;
		public static VirtualConsole Instance { get { return instance; } }

		// Internal State

		private HandAbstraction handAbstraction;

		private XrNodeRig xrNodeRig;
		
		void Start ()
		{
			if (onlyInDebugBuilds && !Debug.isDebugBuild)
			{
				GameObject.Destroy(this.gameObject);
			}
			else
			{
				this.handAbstraction = GetComponentInChildren<HandAbstraction> ();

				this.xrNodeRig = GetComponentInChildren<XrNodeRig>();
				this.xrNodeRig.SetCameraDetection(cameraDetectionMode, explicitVrCameraReference, explicitVrCameraName);
			}
		}

		public void OnEnable()
		{
			VrDebugStats.AllowLogging (true);

			VirtualConsole.instance = this;
		}

		public void OnDisable()
		{
			VrDebugStats.AllowLogging (false);
		}

		/** External API for host game.
		 *  Retrives the GameObject for a given hand.
		 */
		public GameObject GetHand(HandType hand)
		{
			if (handAbstraction == null)
				return null;
			
			return hand == HandType.Left ? handAbstraction.GetLeftHand () : handAbstraction.GetRightHand ();
		}

		/** External API for host game.
		 *  Detect if a specified hand is pointing at a virtual console panel.
		 *  Useful if the host game wants to redirect input to the virtual console only when nessesary.
		 */
		public bool HasTarget(HandType targetHand)
		{
			if (handAbstraction == null)
				return false;
			
			return handAbstraction.HasTarget (targetHand);
		}

		/** External API for host game.
		 *  Host game can call this to manually trigger an input press for the specified hand.
		 *  Useful for when the game is already doing it's own input handling and doesn't want our own input handling active.
		 */
		public void TriggerInput(HandType targetHand)
		{
			if (handAbstraction == null)
				return;
			
			handAbstraction.TriggerInput (targetHand);
		}
	}
}
