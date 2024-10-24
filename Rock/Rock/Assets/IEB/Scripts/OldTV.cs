using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Helix.ImageEffects
{
	[AddComponentMenu("Image Effects/Old TV")]
	[ExecuteAlways]
	[ImageEffectAllowedInSceneView]
	public class OldTV : MonoBehaviour
	{
		[Range(0.0f, 1f)]
		public float grayscaleIntensity = 1.0f;

        [Range(0.0f, 1f)]
        public float noiseIntensity = 1.0f;

        [Range(0.0f, 2.0f)]
		public float bandScale = 1.0f;

		[Range(0.0f, 2.0f)]
		public float bandSpeed = 1.0f;

		[Range(0.0f, 2.0f)]
		public float bandColorIntensity = 1.0f;

		[Range(0.0f, 2.0f)]
		public float bandJitterIntensity = 1.0f;

		Material material;

		void OnRenderImage(RenderTexture source, RenderTexture destination)
		{
			if(material == null)
			{
				material = new Material(Shader.Find("Hidden/IEB/OldTV"));
			}

			material.SetFloat("_Seed", Random.value);
			material.SetFloat("_Intensity1", grayscaleIntensity);
            material.SetFloat("_Intensity2", noiseIntensity);
            material.SetFloat("_BandScale", bandScale);
			material.SetFloat("_BandSpeed", bandSpeed);
			material.SetFloat("_BandIntensity1", bandColorIntensity);
			material.SetFloat("_BandIntensity2", bandJitterIntensity);
			Graphics.Blit(source, destination, material);
		}
	}
}