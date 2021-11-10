using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace Technie.VirtualConsole
{
	public class VrDebugStats : DebugPanel
	{
		// Structures

		private class StatPage
		{
			private Dictionary<string, string> stats = new Dictionary<string, string>();

			public StatPage(string cat)
			{

			}

			public void SetStat(string name, string value)
			{
				stats [name] = value;
			}

			public void GetStats(List<string> keys, List<string> values)
			{
				foreach (string key in stats.Keys)
				{
					keys.Add(key);
					values.Add(stats[key]);
				}
			}
		}

		// Public Properties

	//	public Text[] statLines;

		public Text pageTitle;
		public Text fullPanelText;
		public Text leftCollumnText;
		public Text rightCollumnText;

		// Internal State

		private static bool allowLogging = false;

		private static Dictionary<string, StatPage> pages = new Dictionary<string, StatPage>();
		private static List<string> categories = new List<string> ();

		private static bool hasStatsChanged;

		private string currentCategory = "";

		private bool useTwoCollumns;

		void Start ()
		{
		//	for (int i=0; i<statLines.Length; i++)
		//	{
		//		Text textLine = statLines[i];
		//		textLine.text = "";
		//	}

			pageTitle.text = "";
			fullPanelText.text = "";
			leftCollumnText.text = "";
			rightCollumnText.text = "";
		}

		void Update ()
		{
			if (hasStatsChanged)
			{
				UpdateStats();

				hasStatsChanged = false;
			}
		}

		public override void OnAttach()
		{
			hasStatsChanged = true;
		}

		public override void OnDetach()
		{

		}

		public override void OnResized(VrDebugDisplay.State size)
		{
			hasStatsChanged = true;
			useTwoCollumns = (size == VrDebugDisplay.State.LargeSize);

			if (useTwoCollumns)
			{
				leftCollumnText.enabled = true;
				rightCollumnText.enabled = true;
				fullPanelText.enabled = false;
			}
			else
			{
				leftCollumnText.enabled = false;
				rightCollumnText.enabled = false;
				fullPanelText.enabled = true;
			}
		}

		private void UpdateStats()
		{
			if (pages.Count == 0)
			{
			//	for (int i=0; i<statLines.Length; i++)
			//		statLines[i].text = "";

				fullPanelText.text = "";
				leftCollumnText.text = "";
				rightCollumnText.text = "";
				return;
			}

			if (currentCategory == "" && categories.Count > 0)
				currentCategory = categories [0];

			StatPage page = FindPage (currentCategory);

			List<string> keys = new List<string> ();
			List<string> values = new List<string> ();
			page.GetStats (keys, values);

		//	statLines [0].text = " [ " + currentCategory + " ]";
			pageTitle.text = " [ " + currentCategory + " ] ";

			List<string> lines = new List<string>();
			for (int i=0; i<keys.Count; i++)
			{
				string line = string.Format("{0}: {1}", keys[i], values[i]);
				lines.Add(line);
			}

			// Sort alphabetically?
			lines.Sort();

			if (useTwoCollumns)
			{
				// Fit as much text as we can in the first collumn

				string bestText = "";
				int numLines = 1;
				while (numLines <= lines.Count)
				{
					string newText;
					if (TryGenerateConsoleText(lines, leftCollumnText, numLines, out newText))
					{
						bestText = newText;
						numLines++;
					}
					else
					{
						break;
					}
				}
				leftCollumnText.text = bestText;

				if (numLines < lines.Count)
				{
					// Stil more to display! Put the remaining text on the other collumn

					string colText = "";
					for (int i=numLines; i<lines.Count; i++)
					{
						colText += lines[i] + "\n";
					}
					rightCollumnText.text = colText;
				}
				else
				{
					rightCollumnText.text = "";
				}
			}
			else
			{
				string allLines = "";
				foreach (string line in lines)
				{
					allLines += line + "\n";
				}
				fullPanelText.text = allLines;
			}

			/*
			string bestText = "";
			int numLines = 1;
			while (numLines <= lines.Count)
			{
				string newText;
				if (TryGenerateConsoleText(lines, fullPanelText, numLines, out newText))
				{
					bestText = newText;
					numLines++;
				}
				else
				{
					break;
				}
			}
			fullPanelText.text = bestText;
			*/

			/*
			for (int i=0; i<statLines.Length-1; i++)
			{
				Text textLine = statLines[i+1];
				if (i < keys.Count)
				{
					textLine.text = keys[i] + ": "+ values[i];
				}
				else
				{
					textLine.text = "";
				}
			}
			*/
		}

		private bool TryGenerateConsoleText(List<string> lines, Text targetTextArea, int numLines, out string newText)
		{
			newText = "";

			// First build the string with the most recent numLines

			string txt = "";
		//	for (int i = lines.Count - 2; i > lines.Count - numLines - 1; i--)
			for (int i = 0; i < numLines; i++)
			{
				txt += lines[i] + "\n";
			}

			// Check if it fits!

			targetTextArea.text = txt;
			float h = LayoutUtility.GetPreferredHeight(targetTextArea.rectTransform);
			if (h > targetTextArea.rectTransform.rect.height)
				return false;

			newText = txt;
			return true;
		}

		public void ClearStats()
		{
			pages.Clear ();
			categories.Clear ();

			currentCategory = "";

			hasStatsChanged = true;
		}

		public void PrevCategory()
		{
			ChangeCategory (-1);
		}

		public void NextCateogry()
		{
			ChangeCategory (1);
		}

		private void ChangeCategory(int direction)
		{
			if (categories.Count <= 1)
				return;
			
			int currentIndex = -1;
			for (int i=0; i<categories.Count; i++)
			{
				if (categories[i] == currentCategory)
				{
					currentIndex = i;
					break;
				}
			}
			
			if (currentIndex != -1)
			{
				currentIndex += direction;
				
				if (currentIndex >= categories.Count)
					currentIndex = 0;
				if (currentIndex < 0)
					currentIndex = categories.Count-1;
			}

			currentCategory = categories [currentIndex];

			hasStatsChanged = true;
		}

		public static void AllowLogging(bool allow)
		{
			VrDebugStats.allowLogging = allow;
		}

		public static void SetStat(string category, string name, bool value)
		{
			SetStat (category, name, value.ToString ());
		}

		public static void SetStat(string category, string name, int value)
		{
			SetStat (category, name, value.ToString ());
		}

		public static void SetStat(string category, string name, string value)
		{
			if (!allowLogging)
				return;

			StatPage page = FindPage (category);

			page.SetStat (name, value);

			hasStatsChanged = true;
		}

		private static StatPage FindPage(string categoryName)
		{
			if (categoryName == null)
				categoryName = "";

			if (pages.ContainsKey(categoryName))
			{
				// Return the existing page
				return pages[categoryName];
			}
			else
			{
				// Add a new page for this category
				StatPage page = new StatPage(categoryName);
				pages[categoryName] = page;

				// Add the category to the category list
				categories.Add(categoryName);

				categories.Sort();

				return page;
			}
		}
	}
}
