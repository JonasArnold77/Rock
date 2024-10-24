using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Helix.ImageEffects
{
	[AddComponentMenu("Image Effects/Drunk")]
	[ExecuteAlways]
	public class Drunk : MonoBehaviour
	{
		[Range(0.1f, 1f)]
		public float intensity = 0.5f;

		[Range(0.05f, 10f)]
		public float speed = 1.0f;

		Material material;

		void OnRenderImage(RenderTexture source, RenderTexture destination)
		{
			if(material == null)
			{
				material = new Material(Shader.Find("Hidden/IEB/Drunk"));
			}

			material.SetFloat("_Intensity", intensity);
			material.SetFloat("_Speed", speed);
			Graphics.Blit(source, destination, material);
		}
	}
}