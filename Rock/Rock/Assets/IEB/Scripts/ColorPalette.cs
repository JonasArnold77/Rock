using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Helix.ImageEffects
{
	[AddComponentMenu("Image Effects/Color Palette")]
	[ExecuteAlways]
	[ImageEffectAllowedInSceneView]
	public class ColorPalette : MonoBehaviour
	{
		public Color[] colors = new Color[4];

		[Tooltip("RGB: Chooses the closest color based on RGB values.\nBrightness: Chooses the closest color based on brightness.")]
		public ColorAssignMode colorAssignMode;

		public enum ColorAssignMode
		{
			RGB,
			Brightness
		}

		Material material;

		void OnRenderImage(RenderTexture source, RenderTexture destination)
		{
			if(material == null)
			{
				material = new Material(Shader.Find("Hidden/IEB/ColorPalette"));
			}

			if(colors.Length == 0 || colors == null)
			{
				Debug.LogWarning("Color Palette does not have any colors.");
				Graphics.Blit(source, destination);
			}
			else
			{
				material.SetColorArray("_Colors", colors);
				material.SetInt("_ColorCount", colors.Length);
				material.SetInt("_Brightness", (int)colorAssignMode);

				Graphics.Blit(source, destination, material);
			}
		}
	}
}