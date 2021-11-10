using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Technie.VirtualConsole
{
	public class AutoScreenshot : DebugPanel
	{
		// Public Properties

		public string screenshotOutputFolder = "{{PROJECT_FOLDER}}/Screenshots";

		public float screenshotTimerDelaySecs = 10.0f;
		public int supersamplingAmount = 1;
		public bool hidePanelsOnScreenshot = true;

		public Text timerDelayDisplay;
		public Text countdownDisplay;
		public Text lastScreenshotPathDisplay;

		public Text captureModeDisplay;

		public Toggle hidePanelsToggle;

		public GameObject[] captureModeUiElements;

		public HandAbstraction handAbstraction;
		public PanelManager panelManager;

		// Internal State

#if UNITY_2018_2_OR_NEWER
		private ScreenCapture.StereoScreenCaptureMode captureMode = ScreenCapture.StereoScreenCaptureMode.LeftEye;
#endif
		
		private void Start()
		{
			timerDelayDisplay.text = ((int)screenshotTimerDelaySecs).ToString();
			hidePanelsToggle.isOn = hidePanelsOnScreenshot;

			countdownDisplay.text = "";
			lastScreenshotPathDisplay.text = "";

#if UNITY_2018_2_OR_NEWER
			UpdateCaptureModeDisplay();
#else
			foreach (GameObject obj in captureModeUiElements)
			{
				obj.SetActive(false);
			}
#endif
		}

		public override void OnAttach()
		{

		}

		public override void OnDetach()
		{

		}

		public override void OnResized(VrDebugDisplay.State size)
		{

		}

		public void OnAutoBreak()
		{
			Debug.Break();
		}

		public void OnIncTimer()
		{
			screenshotTimerDelaySecs += 1.0f;

			timerDelayDisplay.text = ((int)screenshotTimerDelaySecs).ToString();
		}
		public void OnDecTimer()
		{
			screenshotTimerDelaySecs -= 1.0f;
			if (screenshotTimerDelaySecs < 1.0f)
				screenshotTimerDelaySecs = 1.0f;

			timerDelayDisplay.text = ((int)screenshotTimerDelaySecs).ToString();
		}

		public void OnIncSupersampling()
		{
			supersamplingAmount++;
			if (supersamplingAmount > 16)
				supersamplingAmount = 16;
		}

		public void OnDecSupersampling()
		{
			supersamplingAmount--;
			if (supersamplingAmount < 1)
				supersamplingAmount = 1;
		}

		public void OnHidePanelsToggled()
		{
			this.hidePanelsOnScreenshot = hidePanelsToggle.isOn;
		}

		public void OnNextCaptureMode()
		{
#if UNITY_2018_2_OR_NEWER
			if (captureMode == ScreenCapture.StereoScreenCaptureMode.LeftEye)
				captureMode = ScreenCapture.StereoScreenCaptureMode.RightEye;
			else if (captureMode == ScreenCapture.StereoScreenCaptureMode.RightEye)
				captureMode = ScreenCapture.StereoScreenCaptureMode.BothEyes;
			else if (captureMode == ScreenCapture.StereoScreenCaptureMode.BothEyes)
				captureMode = ScreenCapture.StereoScreenCaptureMode.LeftEye;
#endif
			UpdateCaptureModeDisplay();
		}

		public void OnPrevCaptureMode()
		{
#if UNITY_2018_2_OR_NEWER
			if (captureMode == ScreenCapture.StereoScreenCaptureMode.LeftEye)
				captureMode = ScreenCapture.StereoScreenCaptureMode.BothEyes;
			else if (captureMode == ScreenCapture.StereoScreenCaptureMode.RightEye)
				captureMode = ScreenCapture.StereoScreenCaptureMode.LeftEye;
			else if (captureMode == ScreenCapture.StereoScreenCaptureMode.BothEyes)
				captureMode = ScreenCapture.StereoScreenCaptureMode.RightEye;
#endif
			UpdateCaptureModeDisplay();
		}

		private void UpdateCaptureModeDisplay()
		{
#if UNITY_2018_2_OR_NEWER
			if (captureMode == ScreenCapture.StereoScreenCaptureMode.LeftEye)
				captureModeDisplay.text = "Left Eye";
			else if (captureMode == ScreenCapture.StereoScreenCaptureMode.RightEye)
				captureModeDisplay.text = "Right Eye";
			else if (captureMode == ScreenCapture.StereoScreenCaptureMode.BothEyes)
				captureModeDisplay.text = "Both Eyes";
#endif
		}

		public void OnStartTimer()
		{
			StopAllCoroutines();
			StartCoroutine(TimerRoutine());
		}

		private IEnumerator TimerRoutine()
		{
			float elapsed = 0.0f;
			while (elapsed < screenshotTimerDelaySecs)
			{
				countdownDisplay.text = "Screenshot in " + (screenshotTimerDelaySecs - elapsed).ToString("0.00") + "...";
				elapsed += Time.deltaTime;
				yield return null;
			}

			countdownDisplay.text = "Smile!";

			// Hide hands/panels
			if (hidePanelsOnScreenshot)
			{
				handAbstraction.SetStylusVisible(false);
				panelManager.SetDisplaysVisible(false);
			}
			yield return null;

			// Trigger screenshot
			
			// Build the output file path and create any directories needed

			string projectDir;
#if UNITY_EDITOR
			projectDir = Application.dataPath.Replace("/Assets", "");
#else
			projectDir = Application.persistentDataPath;
#endif
			string path = screenshotOutputFolder.Replace("{{PROJECT_FOLDER}}", projectDir);
			System.IO.Directory.CreateDirectory(path);
			
			string fileName = "/" + System.DateTime.Now.ToString("dd-MM-yyyy_HH-mm-ss") + ".png";
			path += fileName;

#if UNITY_2018_2_OR_NEWER
			ScreenCapture.CaptureScreenshot(path, captureMode);
#else
			ScreenCapture.CaptureScreenshot(path, supersamplingAmount); // NB: Supersampling parameter doesn't work for VR captures
#endif

			yield return null;

			// Show hands/panels again
			if (hidePanelsOnScreenshot)
			{
				handAbstraction.SetStylusVisible(true);
				panelManager.SetDisplaysVisible(true);
			}
			yield return null;

			// TODO: Burst mode? (take multiple screenshots)

			// Show the output file path to the user
			countdownDisplay.text = "";
			lastScreenshotPathDisplay.text = "Screenshot saved to:\n" + path;
		}
	}
} // Technie.VirtualConsole
