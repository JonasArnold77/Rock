using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Helix.ImageEffects
{
	[AddComponentMenu("Image Effects/Outline")]
	[ExecuteAlways]
	[ImageEffectAllowedInSceneView]
	public class Outline : MonoBehaviour
	{
		[Range(0.01f, 1f)]
		public float thickness = 0.01f;

		[Range(0.0001f, 1f)]
		public float tolerance = 0.01f;

		public Color outlineColor = Color.black;

		public bool highQuality = false;

		public EdgeDetectionType edgeDetection;

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
				material = new Material(Shader.Find("Hidden/IEB/Outline"));
			}

			Camera.current.depthTextureMode = DepthTextureMode.Depth;
			material.SetFloat("_Size", thickness * 0.01f);
			material.SetFloat("_Tolerance", tolerance);
			material.SetColor("_Color", outlineColor);
			material.SetInt("_Depth", (int)edgeDetection);
			material.SetInt("_HQ", highQuality ? 0 : 1);
			Graphics.Blit(source, destination, material);
		}
	}
}