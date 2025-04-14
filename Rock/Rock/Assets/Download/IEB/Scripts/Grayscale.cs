using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Helix.ImageEffects
{
	[AddComponentMenu("Image Effects/Grayscale")]
	[ExecuteAlways]
	[ImageEffectAllowedInSceneView]
	public class Grayscale : MonoBehaviour
	{
		[Range(0.0f, 1f)]
		public float intensity = 1.0f;

		Material material;

		void OnRenderImage(RenderTexture source, RenderTexture destination)
		{
			if(material == null)
			{
				material = new Material(Shader.Find("Hidden/IEB/Grayscale"));
			}

			material.SetFloat("_Intensity", intensity);
			Graphics.Blit(source, destination, material);
		}
	}
}