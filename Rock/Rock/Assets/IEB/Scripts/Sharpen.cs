using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Helix.ImageEffects
{
	[AddComponentMenu("Image Effects/Sharpen")]
	[ExecuteAlways]
	[ImageEffectAllowedInSceneView]
	public class Sharpen : MonoBehaviour
	{
		[Range(0.0f, 3f)]
		public float intensity = 1.0f;

		Material material;

		void OnRenderImage(RenderTexture source, RenderTexture destination)
		{
			if(material == null)
			{
				material = new Material(Shader.Find("Hidden/IEB/Sharpen"));
			}

			material.SetFloat("_Intensity", intensity);
			Graphics.Blit(source, destination, material);
		}
	}
}