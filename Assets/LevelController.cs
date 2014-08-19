using UnityEngine;
using System;
using System.Collections;
[Serializable]
public class LevelController
{	
	[SerializeField]
	private PrimitiveType nodeType = PrimitiveType.Sphere;
	[SerializeField]
	private Vector2 levelPath = new Vector2 (1.0f, 0.6f);
	[SerializeField]
	private int maxNodeOnLevelCount = 200;
	[SerializeField]
	private Shader barrierNodeShader;

	[SerializeField]
	private Texture[] levelTexures;

	private int currentLevel = -1;
	
	private ImageCreator ImageCreator = new ImageCreator();

	private void SetDefaultOptionsToImageCreator()
	{
		ImageCreator.SetOptions(nodeType,maxNodeOnLevelCount);
	}
	/*
	private Action destroyHandler;
	public void SetNodeDestroyHandler(Action handler)
	{
		destroyHandler = handler;
	}
	*/
	public void Initialize()
	{
		SetDefaultOptionsToImageCreator();
		ImageCreator.SetScreenPath (levelPath);
		ImageCreator.SetNodeShader (barrierNodeShader);
	}

	public IEnumerator LoadNextLevel(Action<int> callback)
	{
		return ImageCreator.GenerateLevel(levelTexures[++currentLevel],/* destroyHandler,*/ callback);
	}
}
