using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Helix.ImageEffects
{
	[AddComponentMenu("Image Effects/Heatwave")]
	[ExecuteAlways]
	[ImageEffectAllowedInSceneView]
	public class Heatwave : MonoBehaviour
	{
		[Range(0.0f, 1f)]
		public float intensity = 0.5f;
		[Range(-10f, 10f)]
		public float speed = 0.5f;
		[Range(0.001f, 1f)]
		public float width = 0.5f;

		[Tooltip("If checked, the heatwave effect will amplify the farther away it is from the camera.")]
		public bool depthAware = true;

		[Tooltip("This distance is where the heatwave effect will reach its full intensity. If Depth Aware is enabled, this will be ignored.")]
		public float endDistance = 50f;

		Material material;

		void OnRenderImage(RenderTexture source, RenderTexture destination)
		{
			if(material == null)
			{
				material = new Material(Shader.Find("Hidden/IEB/Heatwave"));
			}
	
			material.SetFloat("_EndDist", endDistance);
			material.SetFloat("_Intensity", intensity);
			material.SetFloat("_Speed", speed);
			material.SetFloat("_Width", width);
			material.SetInt("_Depth", depthAware ? 1 : 0);
			Graphics.Blit(source, destination, material);
		}
	}
}