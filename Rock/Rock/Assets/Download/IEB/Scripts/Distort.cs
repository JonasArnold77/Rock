using System.Collections;
using System.Collections.Generic;

using UnityEngine;
namespace Helix.ImageEffects
{
	[AddComponentMenu("Image Effects/Distort")]
	[ExecuteAlways]
	[ImageEffectAllowedInSceneView]
	public class Distort : MonoBehaviour
	{
		[Range(0f, 1f)]
		public float intensity = 0.5f;

		[Range(0.1f, 4f)]
		public float scale = 0.5f;

		public Vector2 UVSpeed = new Vector2(0.5f, 0);

		public Texture2D texture;

		Material material;

		void OnRenderImage(RenderTexture source, RenderTexture destination)
		{
			if(material == null)
			{
				material = new Material(Shader.Find("Hidden/IEB/Distort"));
			}

			if(texture == null)
			{
				texture = Resources.Load<Texture2D>("Noise/Perlin");
			}

			material.SetFloat("_Intensity", intensity);
			material.SetFloat("_Scale", scale);
			material.SetTexture("_Noise", texture);
			material.SetVector("_Speed", UVSpeed);
			Graphics.Blit(source, destination, material);
		}
	}
}