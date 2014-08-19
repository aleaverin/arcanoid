using UnityEngine;
using System.Collections;

public static class DeviceSettings
{
	public static void Reset()
	{
		camera = null;
		screen = Vector2.zero;
		unitOnPixel = float.NaN;
	}

	private static Camera camera = null;
	public static Camera Camera 
	{ 
		get 
		{ 
			if (camera == null)
			{
				camera = Camera.main;
			}
			return camera; 
		} 
	}
	public static float Aspect { get { return Camera.aspect; } }
	private static Vector2 screen = Vector2.zero;
	public static Vector2 Screen
	{
		get
		{
			if (screen == Vector2.zero)
			{
				screen = new Vector2 (
					Camera.orthographicSize * 2 * Camera.aspect,
					Camera.orthographicSize * 2);
			}
			return screen;
		}
	}
	private static float unitOnPixel = float.NaN;
	public static float UnitOnPixel
	{
		get
		{
			if (float.IsNaN(unitOnPixel))
			{
				unitOnPixel = Camera.orthographicSize * 2 / UnityEngine.Screen.height;
			}
			return unitOnPixel;
		}
	}
}
