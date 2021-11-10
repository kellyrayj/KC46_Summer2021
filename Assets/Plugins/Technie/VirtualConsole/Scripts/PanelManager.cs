using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Technie.VirtualConsole
{
	public class PanelManager : MonoBehaviour
	{
		// Public Properties

		public XrNodeRig xrRig;

		public VrDebugDisplay templateDisplay;

		public DebugPanel[] panels;

		// Internal State

		// The display windows (one per hand) which panels are attached to
		private List<VrDebugDisplay> displays = new List<VrDebugDisplay>();

		private bool isInitialised;

		private void Awake()
		{
			templateDisplay.gameObject.SetActive(false);

			foreach (DebugPanel panel in panels)
			{
				panel.gameObject.SetActive(false);
			}
		}

		void Start()
		{

		}

		public void OnHandsDetected(Camera eventCamera)
		{
			if (isInitialised)
				return;

			foreach (DebugPanel panel in panels)
			{
				GameObject.Destroy(panel.GetComponent<GraphicRaycaster>());
				GameObject.Destroy(panel.GetComponent<CanvasScaler>());
				GameObject.Destroy(panel.GetComponent<Canvas>());
			}

			CreateDisplay(HandType.Left, xrRig.leftHandTransform, eventCamera);
			CreateDisplay(HandType.Right, xrRig.rightHandTransform, eventCamera);

			AttachPanel(FindDisplay(HandType.Left), FindPanel("Vr Console"));
			AttachPanel(FindDisplay(HandType.Right), FindPanel("Debug Stats"));

			foreach (VrDebugDisplay display in displays)
			{
				display.gameObject.SetActive(true);
			}

			isInitialised = true;
		}

		private void CreateDisplay(HandType handType, Transform targetHand, Camera eventCamera)
		{
			GameObject newDisplayObj = GameObject.Instantiate(templateDisplay.gameObject);
			newDisplayObj.name = handType == HandType.Left ? "Left Display" : "Right Display";
			newDisplayObj.transform.SetParent(this.transform.parent, false);
			newDisplayObj.gameObject.SetActive(true);

			VrDebugDisplay newDisplay = newDisplayObj.GetComponent<VrDebugDisplay>();
			newDisplay.panelManager = this;
			newDisplay.handType = handType;
			newDisplay.eventCamera = eventCamera;
			newDisplay.targetHand = targetHand.gameObject;

			displays.Add(newDisplay);
		}

		private VrDebugDisplay FindDisplay(HandType type)
		{
			foreach (VrDebugDisplay display in displays)
			{
				if (display.handType == type)
					return display;
			}
			return null;
		}

		private DebugPanel FindPanel(string panelName)
		{
			foreach (DebugPanel panel in panels)
			{
				if (panel.panelName == panelName)
					return panel;
			}
			return null;
		}

		private void AttachPanel(VrDebugDisplay display, DebugPanel panel)
		{
			if (display == null || panel == null)
				return;
			
			display.Attach(panel);
		}

		public void ChangePanel(int direction, VrDebugDisplay display, DebugPanel currentPanel)
		{
			if (display == null)
				return;

			int currentIndex = 0;
			for (int i=0; i<panels.Length; i++)
			{
				if (panels[i] == currentPanel)
				{
					currentIndex = i;
					break;
				}
			}

			DebugPanel nextPanel = null;
			int numPanelsChecked = 0;

			int nextIndex = currentIndex;
			do
			{
				nextIndex += direction;
				if (nextIndex < 0)
					nextIndex = panels.Length - 1;
				if (nextIndex >= panels.Length)
					nextIndex = 0;

				if (!panels[nextIndex].IsAttached())
				{
					nextPanel = panels[nextIndex];
					break;
				}

				numPanelsChecked++;
			}
			while (numPanelsChecked < panels.Length);

			if (nextPanel != null)
			{
				display.Attach(nextPanel);
			}
		}

		public void SetDisplaysVisible(bool visible)
		{
			foreach (VrDebugDisplay display in displays)
			{
				display.SetDisplayVisible(visible);
			}
		}
	}
}
