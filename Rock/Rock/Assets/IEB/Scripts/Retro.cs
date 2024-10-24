using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Helix.ImageEffects
{
	[AddComponentMenu("Image Effects/Retro")]
	[ExecuteAlways]
	[ImageEffectAllowedInSceneView]

	public class Retro : MonoBehaviour
	{
		[Range(1, 100)]
		public float pixelation = 50;

		[Range(1, 128)]
		public int colorRange = 8;

		RenderTexture _target;
		Material material;

		void CheckTarget()
		{
			int targetWidth = Mathf.RoundToInt(Screen.width * (1.01f - (pixelation / 100.0f)));
			int targetHeight = Mathf.RoundToInt(Screen.height * (1.01f - (pixelation / 100.0f)));

			if(_target == null)
			{
				_target = new RenderTexture(targetWidth, targetHeight, 16);
				_target.filterMode = FilterMode.Point;
				_target.Create();
			}

			else if(_target.width != targetWidth || _target.height != targetHeight)
			{
				_target.Release();
				_target = new RenderTexture(targetWidth, targetHeight, 16);
				_target.filterMode = FilterMode.Point;
				_target.Create();
			}
		}

		void OnRenderImage(RenderTexture source, RenderTexture destination)
		{
			CheckTarget();

			if(material == null)
			{
				material = new Material(Shader.Find("Hidden/IEB/Retro"));
			}

			material.SetFloat("_Seed", Random.value);
			material.SetInt("_Range", colorRange);
			Graphics.Blit(source, _target);
			Graphics.Blit(_target, destination, material);
		}
	}
}