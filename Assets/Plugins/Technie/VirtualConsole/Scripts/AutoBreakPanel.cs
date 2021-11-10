using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Technie.VirtualConsole
{
	public class AutoBreakPanel : DebugPanel
	{
		// Public Properties

		public float breakTimerDelaySecs = 10.0f;

		public Text timerDelayDisplay;
		public Text countdownDisplay;

		// Internal State

		private void Start()
		{
			timerDelayDisplay.text = ((int)breakTimerDelaySecs).ToString();

			countdownDisplay.text = "";
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

		public void OnIncBreakTimer()
		{
			breakTimerDelaySecs += 1.0f;

			timerDelayDisplay.text = ((int)breakTimerDelaySecs).ToString();
		}
		public void OnDecBreakTimer()
		{
			breakTimerDelaySecs -= 1.0f;
			if (breakTimerDelaySecs < 1.0f)
				breakTimerDelaySecs = 1.0f;

			timerDelayDisplay.text = ((int)breakTimerDelaySecs).ToString();
		}

		public void OnStartBreakTimer()
		{
			StopAllCoroutines();
			StartCoroutine(BreakTimerRoutine());
		}

		private IEnumerator BreakTimerRoutine()
		{
			float elapsed = 0.0f;
			while (elapsed < breakTimerDelaySecs)
			{
				countdownDisplay.text = "Pause in " + (breakTimerDelaySecs - elapsed).ToString("0.00") + "...";
				elapsed += Time.deltaTime;
				yield return null;
			}

			countdownDisplay.text = "";

			Debug.Break();
		}
	}
}