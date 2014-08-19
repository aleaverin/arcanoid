using UnityEngine;
using System.Collections;
[System.Serializable]
public class AreaSettings
{
	[SerializeField]
	private float defaultBorderSize = 2.0f;

	public void CreateBorders(bool canCreateDown)
	{
		Transform transform;
		//left
		transform = GameObject.CreatePrimitive(PrimitiveType.Cube).transform;
		transform.localScale = new Vector3 (
			defaultBorderSize, DeviceSettings.Screen.y, defaultBorderSize);
		transform.position = new Vector3 (
			-DeviceSettings.Screen.x/2 - defaultBorderSize / 2, 0, 0);
		//right
		transform = GameObject.CreatePrimitive(PrimitiveType.Cube).transform;
		transform.localScale = new Vector3 (
			defaultBorderSize, DeviceSettings.Screen.y, defaultBorderSize);
		transform.position = new Vector3 (
			DeviceSettings.Screen.x/2 + defaultBorderSize / 2, 0, 0);
		//up
		transform = GameObject.CreatePrimitive (PrimitiveType.Cube).transform;
		transform.localScale = new Vector3 (
			DeviceSettings.Screen.x + 2*defaultBorderSize, defaultBorderSize, defaultBorderSize);
		transform.position = new Vector3 (
			0, DeviceSettings.Screen.y/2 + defaultBorderSize / 2,  0);
		//down
		if (canCreateDown)
		{
			CreateBottomBorder();
		}
	}

	private void CreateBottomBorder()
	{
		Transform transform = GameObject.CreatePrimitive (PrimitiveType.Cube).transform;
		transform.localScale = new Vector3 (
			DeviceSettings.Screen.x + 2*defaultBorderSize, defaultBorderSize, defaultBorderSize);
		transform.position = new Vector3 (
			0, -DeviceSettings.Screen.y/2 - defaultBorderSize / 2,  0);
	}
}
