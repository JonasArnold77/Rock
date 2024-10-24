using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Helix.ImageEffects
{
	[AddComponentMenu("Image Effects/Pencil")]
	[ExecuteAlways]
	[ImageEffectAllowedInSceneView]
	public class Pencil : MonoBehaviour
	{
		[Range(0.01f, 1f)]
		public float thickness = 0.01f;

		[Range(0.0001f, 1f)]
		public float tolerance = 0.01f;

		public Color pencilColor = Color.black;
		public Color bgColor = Color.white;

		public EdgeDetectionType edgeDetection;
		
		public bool ignoreSkybox = false;

		public enum EdgeDetectionType
		{
			Depth = 1,
			Color = 0,
			Both = 2
		}

		Material material;

		void OnRenderImage(RenderTexture source, RenderTexture destination)
		{
			if(material == null)
			{
				material = new Material(Shader.Find("Hidden/IEB/Pencil"));
			}

			material.SetFloat("_Size", thickness * 0.01f);
			material.SetFloat("_Tolerance", tolerance);
			material.SetColor("_Color", pencilColor);
			material.SetColor("_BGColor", bgColor);
			material.SetInt("_Depth", (int)edgeDetection);
			material.SetInt("_IgnoreSkybox", ignoreSkybox ? 1 : 0);
			Graphics.Blit(source, destination, material);
		}
	}
}