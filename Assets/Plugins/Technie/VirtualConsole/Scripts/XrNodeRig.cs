using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Technie.VirtualConsole
{
	/* 'Shadow' camera rig based entirely on UnityEngine.XR interface
	 * Keeps track of world-space position and rotation of hmd/left hand/right hand without needing any external plugins (eg. SteamVR or Rift)
	 */
	public class XrNodeRig : MonoBehaviour
	{
		public Transform headTransform;
		public Transform leftHandTransform;
		public Transform rightHandTransform;

		// Internal State

		private CameraDetectionMode cameraDetectionMode = CameraDetectionMode.UseExplicitCameraReference;
		private Camera explicitVrCameraReference;
		private string explicitVrCameraName;

		// The current vr camera
		// We cache this between frames and only refetch it if it's been deleted or is no longer valid
		private Camera vrCamera;

		// Only trigger the warning about explicit camera setup once so we don't spam the user with messages
		private bool hasShowExplicitCameraWarning;

		public void SetCameraDetection(CameraDetectionMode mode, Camera explicitRef, string explicitName)
		{
			this.cameraDetectionMode = mode;
			this.explicitVrCameraReference = explicitRef;
			this.explicitVrCameraName = explicitName;
		}

		void Update()
		{
			// We know the VR camera's world position/rotation and it's locally tracked position/rotation
			// Using these we can work out the tracking space's origin and set ourselves to that
			// 
			// Then we can easily set the hand transforms as they're just relative to the ourselves/tracking origin

			if (!IsVrCameraValid())
				FindVrCamera();

			if (IsVrCameraValid())
			{
				// Query all of the vr node states
				List<UnityEngine.XR.XRNodeState> states = new List<UnityEngine.XR.XRNodeState>();
				UnityEngine.XR.InputTracking.GetNodeStates(states);

				// Find and process the HMD node first
				foreach (UnityEngine.XR.XRNodeState state in states)
				{
					if (state.nodeType == UnityEngine.XR.XRNode.Head)
					{
						// First align ourselves with the camera rig's rotation
						// Note we also zero our position to make the next step easier
						Quaternion trackedRotation;
						state.TryGetRotation(out trackedRotation);
						Quaternion rigRotation = vrCamera.transform.rotation * Quaternion.Inverse(trackedRotation);
						this.transform.position = Vector3.zero;
						this.transform.rotation = rigRotation;

						// Now we know the rig rotation, figure out what world-space offset we need to align a local camera offset with the actual world camera position
						Vector3 trackedPosition;
						state.TryGetPosition(out trackedPosition);
						Vector3 rigPosition = vrCamera.transform.position - this.transform.TransformPoint(trackedPosition);
						this.transform.position = rigPosition;
					}
					else if (state.nodeType == UnityEngine.XR.XRNode.LeftHand)
					{
						// Apply the local position/rotation to the left hand transform
						Sync(state, leftHandTransform);
					}
					else if (state.nodeType == UnityEngine.XR.XRNode.RightHand)
					{
						// Apply the local position/rotation to the right hand transform
						Sync(state, rightHandTransform);
					}
				}
			}
		}

		private void Sync(UnityEngine.XR.XRNodeState node, Transform destTransform)
		{
			Vector3 position;
			if (node.TryGetPosition(out position))
			{
				destTransform.localPosition = position;
			}
			Quaternion rotation;
			if (node.TryGetRotation(out rotation))
			{
				destTransform.localRotation = rotation;
			}

			destTransform.gameObject.SetActive(node.tracked);
		}

		private bool IsVrCameraValid()
		{
			return vrCamera != null && vrCamera.isActiveAndEnabled && vrCamera.stereoTargetEye == StereoTargetEyeMask.Both;
		}

		private void FindVrCamera()
		{
			Camera[] allCameras = Camera.allCameras;

			if (cameraDetectionMode == CameraDetectionMode.UseExplicitCameraReference)
			{
				vrCamera = explicitVrCameraReference;

				if (vrCamera == null && !hasShowExplicitCameraWarning)
				{
					Debug.LogWarning("[Virtual Console] Camera detection is set to 'explicit camera reference' but camera reference is null");
					hasShowExplicitCameraWarning = true;
				}
			}
			else if (cameraDetectionMode == CameraDetectionMode.UseExplicitCameraName)
			{
				foreach (Camera cam in allCameras)
				{
					if (cam.name == explicitVrCameraName)
					{
						vrCamera = cam;
						break;
					}
				}

				if (vrCamera == null && !hasShowExplicitCameraWarning)
				{
					Debug.LogWarning("[Virtual Console] Camera detection is set to 'explicit camera name' but no camera with the name '" + explicitVrCameraName + "' found");
					hasShowExplicitCameraWarning = true;
				}
			}

			if (cameraDetectionMode == CameraDetectionMode.Automatic || vrCamera == null) // Do automatic find if neither of the above have actually found anything
			{
				foreach (Camera cam in allCameras)
				{
					if (cam != null && cam.isActiveAndEnabled && cam.stereoTargetEye == StereoTargetEyeMask.Both)
					{
						vrCamera = cam;
						break;
					}
				}
			}
		}
	}

} // namespace Technie.VirtualConsole
