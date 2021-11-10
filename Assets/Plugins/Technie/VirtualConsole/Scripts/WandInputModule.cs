using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace Technie.VirtualConsole
{
	public class WandInputModule : BaseInputModule
	{
		public static WandInputModule Instance;
		
		[Header(" [Cursor setup]")]
		public Sprite CursorSprite;
		public Material CursorMaterial;
		public float NormalCursorScale = 0.00025f;

		[Space(10)]
		
		[Header(" [Runtime variables]")]
		[Tooltip("Indicates whether or not the gui was hit by any controller this frame")]
		public bool guiHit;
		
		[Tooltip("Indicates whether or not a button was used this frame")]
		public bool ButtonUsed;
		
		[Tooltip("Generated cursors")]
		public RectTransform[] Cursors;
		
		[Tooltip("Generated non rendering camera (used for raycasting ui)")]
		public Camera ControllerCamera;

		public Color[] cursorColours;

		/** Does raycasting ignore canvases set to render as 'overlay'? */
		public bool ignoreOverlayCanvases = true;

		// Internal State

	//	private GameObject leftHand, rightHand;

		private Transform[] raycastOrigin;
		private bool[] raycastActive;

		private GameObject[] CurrentPoint;
		private GameObject[] CurrentPressed;
		private GameObject[] CurrentDragging;
		private Vector3[]    CurrentRaycastPosition;
		private Ray[] LastRays;

		private PointerEventData[] PointEvents;

		private bool[] latchedDownEdges;
		private bool[] latchedUpEdges;

		private bool Initialized = false;

		protected override void Awake()
		{
			base.Awake ();
		}
		
		protected override void OnDestroy()
		{
			base.OnDestroy ();
		}

		public void OnHandsDetected(HandAbstraction hands)
		{
			Initialise ();

	//		leftHand = hands.GetLeftHand ();
	//		rightHand = hands.GetRightHand ();
		}

		public void SetRaycastOrigins(Transform leftOrigin, Transform rightOrigin)
		{
			this.raycastOrigin = new Transform[2];
			this.raycastActive = new bool[2];

			this.raycastOrigin[0] = leftOrigin;
			this.raycastOrigin[1] = rightOrigin;
		}

		public void SetRaycastActive(HandType hand, bool isActive)
		{
			if (hand == HandType.Left)
				raycastActive[0] = isActive;
			else
				raycastActive[1] = isActive;
		}

		private void Initialise()
		{	
			if (Initialized == false)
			{
				Instance = this;

				if (cursorColours == null || cursorColours.Length != 2)
				{
					cursorColours = new Color[2];
					cursorColours[0] = Color.white;
					cursorColours[1] = Color.white;
				}
				
				ControllerCamera = new GameObject("Virtual Console UI Camera").AddComponent<Camera>();
				ControllerCamera.enabled = false;
				ControllerCamera.clearFlags = CameraClearFlags.Nothing;
				ControllerCamera.cullingMask = 0;
				ControllerCamera.nearClipPlane = 0.01f;
#if UNITY_5_4_OR_NEWER
				ControllerCamera.stereoTargetEye = StereoTargetEyeMask.None;
#endif
				ControllerCamera.transform.SetParent(this.transform, false);

				Cursors = new RectTransform[2]; // for left hand, right hand
				
				for (int index = 0; index < Cursors.Length; index++)
				{
					GameObject cursor = new GameObject("Virtual Console Cursor " + index);
					cursor.transform.SetParent(this.transform, false);
					Canvas canvas = cursor.AddComponent<Canvas>();
					cursor.AddComponent<CanvasRenderer>();
					cursor.AddComponent<CanvasScaler>();
					cursor.AddComponent<UiIgnoreRaycast>();
					cursor.AddComponent<GraphicRaycaster>();
					
					canvas.renderMode = RenderMode.WorldSpace;
					canvas.sortingOrder = 1000; //set to be on top of everything
					
					Image image = cursor.AddComponent<Image>();
					image.sprite = CursorSprite;
					image.material = new Material(CursorMaterial);

					if (CursorSprite == null)
						Debug.LogWarning("Set CursorSprite on " + this.gameObject.name + " to the sprite you want to use as your cursor.", this.gameObject);

					image.material.SetColor("_Color", cursorColours[index]);

					Cursors[index] = cursor.GetComponent<RectTransform>();
				}
				
				CurrentPoint = new GameObject[Cursors.Length];
				CurrentPressed = new GameObject[Cursors.Length];
				CurrentDragging = new GameObject[Cursors.Length];
				PointEvents = new PointerEventData[Cursors.Length];
				CurrentRaycastPosition = new Vector3[Cursors.Length];
				LastRays = new Ray[Cursors.Length];

				latchedDownEdges = new bool[Cursors.Length];
				latchedUpEdges = new bool[Cursors.Length];
				
				Canvas[] canvases = GameObject.FindObjectsOfType<Canvas> (); // NB: This gets all canvases, including the game's canvases
				foreach (Canvas canvas in canvases)
				{
					// Is this one of our canvases?
					GameObject canvasParent = canvas.transform.parent != null ? canvas.transform.parent.gameObject : null;
					if (canvasParent != null && canvasParent.GetComponent<VrDebugDisplay> () != null)
					{
						// This is one of our canvases on a vr debug panel
						canvas.worldCamera = ControllerCamera;
					}
				}
				
				Initialized = true;
			}
		}

		public Camera GetControllerCamera()
		{
			return ControllerCamera;
		}
		
		// use screen midpoint as locked pointer location, enabling look location to be the "mouse"
		private bool GetLookPointerEventData(int index)
		{
			if (PointEvents[index] == null)
				PointEvents[index] = new PointerEventData(base.eventSystem);
			else
				PointEvents[index].Reset();
			
			PointEvents[index].delta = Vector2.zero;
			PointEvents[index].position = new Vector2(Screen.width / 2, Screen.height / 2);
			PointEvents[index].scrollDelta = Vector2.zero;

			Ray ray = ControllerCamera.ScreenPointToRay(PointEvents[index].position);
			LastRays [index] = ray;

			base.eventSystem.RaycastAll(PointEvents[index], m_RaycastResultCache);

			if (ignoreOverlayCanvases)
			{
				// Search through the raycast results and discard any that are actually on an overlay canvas

				for (int i=m_RaycastResultCache.Count-1; i>=0; i--)
				{
					Canvas objCanvas = FindParentCanvas(m_RaycastResultCache[i].gameObject);
					if (objCanvas != null && (objCanvas.renderMode == RenderMode.ScreenSpaceOverlay || objCanvas.renderMode == RenderMode.ScreenSpaceCamera))
					{
						m_RaycastResultCache.RemoveAt(i);
					}
					else if (objCanvas.transform.parent != null)
					{
						// Hack: Remove any hits that aren't part of one of our debug panels

						GameObject canvasParent = objCanvas.transform.parent.gameObject;
						VrDebugDisplay display = canvasParent.GetComponent<VrDebugDisplay>();
						if (display == null)
						{
							m_RaycastResultCache.RemoveAt(i);
						}
					}
				}
			}

			PointEvents[index].pointerCurrentRaycast = FindFirstRaycast(m_RaycastResultCache);
			if (PointEvents[index].pointerCurrentRaycast.gameObject != null)
			{
				guiHit = true; //gets set to false at the beginning of the process event
			}
			
			m_RaycastResultCache.Clear();
			
			return true;
		}

		private static Canvas FindParentCanvas(GameObject obj)
		{
			if (obj == null)
				return null;

			Transform current = obj.transform;

			do
			{
				Canvas parentCanvas = current.GetComponent<Canvas> ();
				if (parentCanvas != null)
					return parentCanvas;

				current = current.parent;
			}
			while (current != null);

			return null;
		}

		// update the cursor location and whether it is enabled
		// this code is based on Unity's DragMe.cs code provided in the UI drag and drop example
		private void UpdateCursor(int index, PointerEventData pointData)
		{
			Camera mainCamera = Camera.main;
			if (mainCamera == null)
				return;

			if (PointEvents[index].pointerCurrentRaycast.gameObject != null)
			{
				Cursors[index].gameObject.SetActive(true);
				
				if (pointData.pointerEnter != null)
				{
					RectTransform draggingPlane = pointData.pointerEnter.GetComponent<RectTransform>();
					Vector3 globalLookPos;
					if (RectTransformUtility.ScreenPointToWorldPointInRectangle(draggingPlane, pointData.position, pointData.enterEventCamera, out globalLookPos))
					{
						Cursors[index].position = globalLookPos;
						Cursors[index].rotation = draggingPlane.rotation;
						
						// scale cursor based on distance to camera
						float lookPointDistance = (Cursors[index].position - mainCamera.transform.position).magnitude;
						float cursorScale = lookPointDistance * NormalCursorScale;
						if (cursorScale < NormalCursorScale)
						{
							cursorScale = NormalCursorScale;
						}
						
						Cursors[index].localScale = Vector3.one * cursorScale;
					}
				}
			}
			else
			{
				Cursors[index].gameObject.SetActive(false);
			}
		}
		
		// clear the current selection
		public void ClearSelection()
		{
			if (base.eventSystem.currentSelectedGameObject)
			{
				base.eventSystem.SetSelectedGameObject(null);
			}
		}
		
		// select a game object
		private void Select(GameObject go)
		{
			ClearSelection();
			
			if (ExecuteEvents.GetEventHandler<ISelectHandler>(go))
			{
				base.eventSystem.SetSelectedGameObject(go);
			}
		}
		
		// send update event to selected object
		// needed for InputField to receive keyboard input
		private bool SendUpdateEventToSelectedObject()
		{
			if (base.eventSystem.currentSelectedGameObject == null)
				return false;
			
			BaseEventData data = GetBaseEventData();
			
			ExecuteEvents.Execute(base.eventSystem.currentSelectedGameObject, data, ExecuteEvents.updateSelectedHandler);
			
			return data.used;
		}

		private void UpdateCameraPosition(int index)
		{
		//	GameObject hand = index == 0 ? leftHand : rightHand;
		//	ControllerCamera.transform.position = hand.transform.position;
		//	ControllerCamera.transform.forward = hand.transform.forward;

			Transform origin = raycastOrigin[index];
			ControllerCamera.transform.position = origin.position;
			ControllerCamera.transform.rotation = origin.rotation;
		}
		
		// Process is called by UI system to process events
		public override void Process()
		{
			if (!Initialized)
				return;

			guiHit = false;
			ButtonUsed = false;
			
			// send update events if there is a selected object - this is important for InputField to receive keyboard events
			SendUpdateEventToSelectedObject();

			// see if there is a UI element that is currently being looked at
			for (int index = 0; index < Cursors.Length; index++)
			{
				/*
				GameObject handObj = index == 0 ? leftHand : rightHand;

				if (handObj == null || !handObj.activeInHierarchy)
				{
					if (Cursors[index].gameObject.activeInHierarchy)
					{
						Cursors[index].gameObject.SetActive(false);
					}
					continue;
				}
				*/
				
				Transform origin = raycastOrigin[index];
				if (!raycastActive[index] || origin == null || !origin.gameObject.activeInHierarchy)
				{
					Cursors[index].gameObject.SetActive(false);
					continue;
				}


				UpdateCameraPosition(index);
				
				bool hit = GetLookPointerEventData(index);
				if (hit == false)
					continue;

				// PointEvents.pointerCurrentRaycast.worldPosition is always (0,0,0) so we must manually recalculate it via the 'distance' returned
			//	Vector3 rayOrigin = handObj.transform.position;
			//	Vector3 rayDir = handObj.transform.forward;
				Vector3 rayOrigin = origin.position;
				Vector3 rayDir = origin.forward;
				Vector3 worldPosition = rayOrigin + rayDir * (PointEvents[index].pointerCurrentRaycast.distance + ControllerCamera.nearClipPlane);

				CurrentPoint[index] = PointEvents[index].pointerCurrentRaycast.gameObject;
				CurrentRaycastPosition[index] = worldPosition;

				// handle enter and exit events (highlight)
				base.HandlePointerExitAndEnter(PointEvents[index], CurrentPoint[index]);
				
				// update cursor
				UpdateCursor(index, PointEvents[index]);

			//	if (handObj != null)
				{
					if (ButtonDown(index))
					{
						ClearSelection();
						
						PointEvents[index].pressPosition = PointEvents[index].position;
						PointEvents[index].pointerPressRaycast = PointEvents[index].pointerCurrentRaycast;
						PointEvents[index].pointerPress = null;
						
						if (CurrentPoint[index] != null)
						{
							CurrentPressed[index] = CurrentPoint[index];
							
							GameObject newPressed = ExecuteEvents.ExecuteHierarchy(CurrentPressed[index], PointEvents[index], ExecuteEvents.pointerDownHandler);
							
							if (newPressed == null)
							{
								// some UI elements might only have click handler and not pointer down handler
								newPressed = ExecuteEvents.ExecuteHierarchy(CurrentPressed[index], PointEvents[index], ExecuteEvents.pointerClickHandler);
								if (newPressed != null)
								{
									CurrentPressed[index] = newPressed;
								}
							}
							else
							{
								CurrentPressed[index] = newPressed;
								// we want to do click on button down at same time, unlike regular mouse processing
								// which does click when mouse goes up over same object it went down on
								// reason to do this is head tracking might be jittery and this makes it easier to click buttons
								ExecuteEvents.Execute(newPressed, PointEvents[index], ExecuteEvents.pointerClickHandler);
							}
							
							if (newPressed != null)
							{
								PointEvents[index].pointerPress = newPressed;
								CurrentPressed[index] = newPressed;
								Select(CurrentPressed[index]);
								ButtonUsed = true;
							}
							
							ExecuteEvents.Execute(CurrentPressed[index], PointEvents[index], ExecuteEvents.beginDragHandler);
							PointEvents[index].pointerDrag = CurrentPressed[index];
							CurrentDragging[index] = CurrentPressed[index];
						}
					}
					
					if (ButtonUp(index))
					{
						if (CurrentDragging[index])
						{
							ExecuteEvents.Execute(CurrentDragging[index], PointEvents[index], ExecuteEvents.endDragHandler);
							if (CurrentPoint[index] != null)
							{
								ExecuteEvents.ExecuteHierarchy(CurrentPoint[index], PointEvents[index], ExecuteEvents.dropHandler);
							}
							PointEvents[index].pointerDrag = null;
							CurrentDragging[index] = null;
						}
						if (CurrentPressed[index])
						{
							ExecuteEvents.Execute(CurrentPressed[index], PointEvents[index], ExecuteEvents.pointerUpHandler);
							PointEvents[index].rawPointerPress = null;
							PointEvents[index].pointerPress = null;
							CurrentPressed[index] = null;
						}
					}
					
					// drag handling
					if (CurrentDragging[index] != null)
					{
						ExecuteEvents.Execute(CurrentDragging[index], PointEvents[index], ExecuteEvents.dragHandler);
					}
				}
			}
		}
		
		private bool ButtonDown(int index)
		{
			if (latchedDownEdges [index])
			{
				latchedDownEdges[index] = false;
				return true;
			}
			return false;
		}
		
		private bool ButtonUp(int index)
		{
			if (latchedUpEdges [index])
			{
				latchedUpEdges[index] = false;
				return true;
			}
			return false;
		}

		public void LatchButtonDown(int index)
		{
			latchedDownEdges [index] = true;
		}

		public void LatchButtonUp(int index)
		{
			latchedUpEdges [index] = true;
		}

		public bool HasCurrentPointTarget(int index)
		{
			return CurrentPoint != null ? CurrentPoint[index] != null : false;
		}

		public Vector3 GetCurrentPointPosition(int index)
		{
			return CurrentRaycastPosition [index];
		}

		public bool HasLeftPointTarget()
		{
			return CurrentPoint [0] != null;
		}
		public bool HasRightPointTarget()
		{
			return CurrentPoint [1] != null;
		}

		public void SetCursorsVisible(bool visible)
		{
			foreach (RectTransform cursor in this.Cursors)
			{
				cursor.gameObject.SetActive(visible);
			}
		}
	}

}