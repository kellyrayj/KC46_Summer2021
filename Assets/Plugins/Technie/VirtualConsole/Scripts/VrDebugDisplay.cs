using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Technie.VirtualConsole
{
	/** Attaches a canvas to a target hand */
	public class VrDebugDisplay : MonoBehaviour
	{
		public enum State
		{
			Collapsed,
			NormalSize,
			LargeSize
		}

		// Public Properties

		public Canvas mainCanvas;
		public Canvas collapsedCanvas;
		
		public HandType handType;

		public Transform center;
		public Transform panelAttach;

		public float panelScale = 1.0f;

		public Text titleText;
		public Text maximiseText;

		public PanelManager panelManager;

		[ReadOnly]
		public GameObject targetHand;

		[ReadOnly]
		public Camera eventCamera;

		// Internal State

		private State state = State.NormalSize;
		private State prevState = State.NormalSize;

		private DebugPanel attachedPanel;

		private bool disableCanvasWhenNotInView = false;

		private void Start()
		{
			this.transform.localScale = new Vector3 (0.002f * panelScale, 0.002f * panelScale, 0.002f * panelScale);

			if (mainCanvas != null)
			{
				mainCanvas.worldCamera = eventCamera;
				mainCanvas.gameObject.SetActive(true);
			}
			if (collapsedCanvas != null)
			{
				collapsedCanvas.worldCamera = eventCamera;
				collapsedCanvas.gameObject.SetActive(false);
			}
		}

		void LateUpdate()
		{
			TrackTargetHand ();
		}

		private void TrackTargetHand()
		{
			if (targetHand != null)
			{
				this.transform.position = targetHand.transform.position;
				this.transform.rotation = targetHand.transform.rotation * Quaternion.Euler(90.0f, 0.0f, 0.0f);
			}

			// Only show the canvas if we're looking at it from the front
			if (disableCanvasWhenNotInView)
			{
				if (Camera.main != null)
				{
					float dot = Vector3.Dot(Camera.main.transform.forward, center.forward);
					bool isVisible = dot > 0.0f;

					if (mainCanvas != null)
						mainCanvas.enabled = isVisible;

					if (collapsedCanvas != null)
						collapsedCanvas.enabled = isVisible;
				}
			}
		}

		public void AllowDisableCanvasWhenNotInView(bool allow)
		{
			this.disableCanvasWhenNotInView = allow;
		}

		public void OnToggleSize()
		{
			if (state == State.Collapsed)
				return;

			if (state == State.NormalSize)
				state = State.LargeSize;
			else if (state == State.LargeSize)
				state = State.NormalSize;

			RectTransform canvasTransform = mainCanvas.gameObject.transform as RectTransform;
			if (state == State.LargeSize)
			{
				canvasTransform.sizeDelta = new Vector2(300, 200);
				canvasTransform.localPosition = canvasTransform.localPosition + new Vector3(0, 50, 0);

				maximiseText.text = "-";

				if (attachedPanel != null)
					attachedPanel.OnResized(state);
			}
			else
			{
				canvasTransform.sizeDelta = new Vector2(100, 100);
				canvasTransform.localPosition = canvasTransform.localPosition - new Vector3(0, 50, 0);

				maximiseText.text = "+";

				if (attachedPanel != null)
					attachedPanel.OnResized(state);
			}
		}

		public void OnCollapse()
		{
			// Shrink from normal panel to collapsed as a single icon

			prevState = state;
			state = State.Collapsed;

			mainCanvas.gameObject.SetActive (false);
			collapsedCanvas.gameObject.SetActive (true);
		}

		public void OnRestore()
		{
			// Restore to normal panel size from collapsed as a single icon

			if (state != State.Collapsed)
				return;

			state = prevState;

			mainCanvas.gameObject.SetActive (true);
			collapsedCanvas.gameObject.SetActive (false);
		}

		public void OnNextPanel()
		{
			panelManager.ChangePanel(+1, this, attachedPanel);
		}

		public void OnPrevPanel()
		{
			panelManager.ChangePanel(-1, this, attachedPanel);
		}

		public float DistanceTo(Vector3 worldPosition)
		{
			return Vector3.Distance(worldPosition, FindClosestPoint(worldPosition));
		}

		public Vector3 FindClosestPoint(Vector3 worldPosition)
		{
			if (state == State.NormalSize)
			{
				return ClosestPoint(worldPosition, mainCanvas, new Vector2(50, 50));
			}
			else if (state == State.LargeSize)
			{
				return ClosestPoint(worldPosition, mainCanvas, new Vector2(150, 150));
			}
			else if (state == State.Collapsed)
			{
				return ClosestPoint(worldPosition, collapsedCanvas, new Vector2(8, 8));
			}
			return Vector3.zero;
		}
		
		private Vector3 ClosestPoint(Vector3 worldPosition, Canvas canvas, Vector2 canvasSize)
		{
			Vector3 localPosition = canvas.transform.InverseTransformPoint(worldPosition);
			Vector3 closestPosition = new Vector3(localPosition.x, localPosition.y, 0.0f);

			closestPosition.x = Mathf.Clamp(closestPosition.x, -canvasSize.x, canvasSize.x);
			closestPosition.y = Mathf.Clamp(closestPosition.y, -canvasSize.y, canvasSize.y);
			
			Vector3 worldSpaceClosestPoint = canvas.transform.TransformPoint(closestPosition);

			return worldSpaceClosestPoint;
		}

		public void Attach(DebugPanel newPanel)
		{
			if (attachedPanel != null)
			{
				attachedPanel.gameObject.SetActive(false);

				titleText.text = "";

				attachedPanel.NotifyOnDetach();
				attachedPanel = null;
			}

			if (newPanel != null)
			{
				newPanel.transform.SetParent(panelAttach, false);
				newPanel.transform.localPosition = Vector3.zero;
				newPanel.gameObject.SetActive(true);

				RectTransform panelRect = newPanel.transform as RectTransform;
				panelRect.anchoredPosition = Vector3.zero;
				panelRect.anchorMin = Vector2.zero;
				panelRect.anchorMax = Vector2.one;
				panelRect.offsetMin = Vector2.zero;
				panelRect.offsetMax = Vector2.zero;

				titleText.text = newPanel.panelName;
				titleText.SetAllDirty();

				newPanel.NotifyOnAttach();
				attachedPanel = newPanel;
			}
		}

		// Temporarily disable visibility of the display
		public void SetDisplayVisible(bool visible)
		{
			if (state == State.NormalSize || state == State.LargeSize)
			{
				this.mainCanvas.enabled = visible;
			}
			else if (state == State.Collapsed)
			{
				this.collapsedCanvas.enabled = visible;
			}
		}
	}
}

