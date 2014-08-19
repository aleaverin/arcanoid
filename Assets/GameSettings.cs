using UnityEngine;
using System.Collections;

[System.Serializable]
public class GameSettings
{
	[SerializeField]
	private float platformOffsetOnBottomInUnit = 0.5f;
	[SerializeField]
	private bool canCreateDown = false;

	private static GameSettings singleton = new GameSettings();

	private GameSettings(){}

	public static float PlatformOffsetOnBottomInUnit
	{
		get { return singleton.platformOffsetOnBottomInUnit; }
	}
	public static bool CanCreateDown 
	{
		get { return singleton.canCreateDown; }
	}
}
