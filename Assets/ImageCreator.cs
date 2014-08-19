using UnityEngine;
using System;
using System.Collections;

[System.Serializable]
public class ImageCreator 
{
	//Options
	PrimitiveType nodePrimitiveType = PrimitiveType.Sphere;
	private int maxNodeCount = 100;
	public void SetOptions(PrimitiveType nodePrimitiveType = PrimitiveType.Sphere,
	                       int maxNodeCount = 100,
	                       float waitingTimePastRow = 0.05f,
	                       float waitingTimeOnNode = 0.0f)
	{
		this.nodePrimitiveType = nodePrimitiveType;
		this.maxNodeCount = maxNodeCount;
	}
	/*
	private ParticleSystem nodeBoomParticleSystem;
	public void SetBoomParticles(ParticleSystem particleSystem)
	{
		nodeBoomParticleSystem = particleSystem;
	}
	*/
	private Shader barrierNodeShader;
	private Material barrierNodeMaterial;
	public void SetNodeShader(Shader barrierNodeShader)
	{
		this.barrierNodeShader = barrierNodeShader;
		barrierNodeMaterial = new Material(barrierNodeShader);
	}

	private Vector2 areaSize;
	private Vector2 centerOfArea;
	public void CalculateArea()
	{
		areaSize = new Vector2(
			DeviceSettings.Screen.x * screenPath.x,
			DeviceSettings.Screen.y * screenPath.y);
		centerOfArea = (DeviceSettings.Screen - areaSize) / 2;
	}
	private Vector2 screenPath = Vector2.one;
	public void SetScreenPath(Vector2 screenPath)
	{ 
		this.screenPath = screenPath;
		CalculateArea();
	}

	private Texture2D currentTexture;
	public int NodeOnLevelCount { get; private set; }


	Color GetColor(Texture2D texture, int i, int j, Vector2 pixelOnNode)
	{
		int x = (int)(i*pixelOnNode.x);
		int y = (int)(j*pixelOnNode.y);
		Color lastColor = texture.GetPixel(x, y);
		Color[] colors = texture.GetPixels(x, y, (int)pixelOnNode.x, (int)pixelOnNode.y);
		Color color = colors [0];
		for (int index = 1; i < colors.Length; i++)
		{
			color = Color.Lerp(color,colors[i],0.5f);
		}
		return color;
	}
	public IEnumerator GenerateLevel(Texture levelTexture, /* Action destroyNodeHandler,*/
	                                 Action<int> postGenerateCallback)
	{
		NodeOnLevelCount = 0;

		currentTexture = (Texture2D)levelTexture;

		Vector2 texSize = new Vector2 (currentTexture.width, currentTexture.height);
		
		float widthRatio = Screen.width * screenPath.x / texSize.x;
		float heightRatio = Screen.height * screenPath.y / texSize.y;
		
		float scale = Mathf.Min (widthRatio, heightRatio);
		
		Vector2 scaledImg = texSize * scale;
		
		float imgAspect = texSize.x / texSize.y;
		float imgH = Mathf.Sqrt (maxNodeCount / imgAspect);
		float imgW = imgH * imgAspect;
		int nodeCountOnHeight = (int)imgH;
		int nodeCountOnWidth = (int)imgW;
		// -- отношение уже разное может быть
		Vector2 pixelOnNode = new Vector2 (texSize.x / nodeCountOnWidth, texSize.y / nodeCountOnHeight);
		
		float nodeRadius = Mathf.Min (areaSize.x / nodeCountOnWidth, areaSize.y / nodeCountOnHeight);
		
		Vector3 startDrawPoint = new Vector3 (centerOfArea.x - (int)(nodeCountOnWidth/2) * nodeRadius,
		                              centerOfArea.y - (int)(nodeCountOnHeight/2) * nodeRadius, 0);
		
		for (int i = 0; i < nodeCountOnWidth; i++)
		{
			for (int j = 0; j < nodeCountOnHeight; j++)
			{
				int x = (int)(i*pixelOnNode.x);
				int y = (int)(j*pixelOnNode.y);
				
				Color color = GetColor(currentTexture,i,j,pixelOnNode);
				
				if (color == Color.white)
					continue;
				
				GameObject node = GameObject.CreatePrimitive(nodePrimitiveType);
				
				if (color == Color.red)
				{
					node.name = "barrier";
				}
				else
				{
					node.name = "node";
					NodeOnLevelCount++;
					node _node = node.AddComponent<node>();
					//_node.AddDestroyHandler(destroyNodeHandler);
					//_node.ParticleSystem = nodeBoomParticleSystem;
				}
				
				node.transform.localScale = Vector3.one * nodeRadius;
				float xOffset = -0.5f * (nodeCountOnWidth % 2 - 1);
				float yOffset = -0.5f * (nodeCountOnHeight % 2 - 1);
				node.transform.position = startDrawPoint + new Vector3(i+xOffset, j+yOffset, 0) * nodeRadius;

				node.transform.renderer.material = barrierNodeMaterial;
				node.renderer.material.color = color;
				/*
				barrierNodeMaterial.SetTexture("_MainTex", levelTexture);
				barrierNodeMaterial.SetTextureScale("_MainTex",
				           new Vector2(1.0f/nodeCountOnWidth,1.0f/nodeCountOnHeight));
				float x_ = 1.0f*i/(nodeCountOnWidth);
				float y_= 1.0f*j/nodeCountOnHeight;
				barrierNodeMaterial.SetTextureOffset("_MainTex",new Vector2(x_,y_));
				*/
				//node.transform.eulerAngles = new Vector3(0,0,180);
			}
			yield return new WaitForSeconds(0.05f);
		}
		if (postGenerateCallback!=null)
		{
			postGenerateCallback(NodeOnLevelCount);
		}
	}
}