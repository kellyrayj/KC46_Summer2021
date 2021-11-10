using UnityEngine;
using System.Collections;

namespace Technie.VirtualConsole
{
	public class UiIgnoreRaycast : MonoBehaviour, ICanvasRaycastFilter 
	{
		public bool IsRaycastLocationValid(Vector2 sp, Camera eventCamera)
		{
			return false;
		}
	}
}