using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace QDY.Drunk.URP
{
	public class DemoURP : MonoBehaviour
	{
		public RenderPipelineAsset m_Pipeline;
		public DrunkFeature m_Feature;
		public Material m_Mat;
		public enum EType { None = 0, Rotated, Splitted, Deform, Waggle };
		public EType m_Type = EType.Rotated;
		//[Range(0f, 1f)] public float m_DrunkIntensity = 1f;
		[Header("RGB Shift")]
		public bool m_RGBShift = false;
		[Range(0f, 0.05f)] public float m_RGBShiftFactor = 0.01f;
		[Range(1f, 16f)] public float m_RGBShiftPower = 3f;
		[Header("Rotate Ghost")]
		[Range(0f, 0.06f)] public float m_RotateRadius = 0.01f;
		[Range(0.01f, 1f)] public float m_RotateMix = 0.5f;
		[Header("Split Ghost")]
		[Range(0.01f, 0.2f)] public float m_SplitAmplitude = 0.05f;
		[Range(1f, 4f)] public float m_SplitSpeed = 1f;
		[Header("Deform")]
		[Range(0.01f, 0.15f)] public float m_DeformAmplitude = 0.08f;
		[Range(1f, 8f)] public float m_DeformSpeed = 2f;
		[Header("Waggle")]
		[Range(0.1f, 1f)] public float m_WaggleFocus = 0.75f;
		[Range(0.01f, 0.2f)] public float m_WaggleAmplitude = 0.05f;
		[Header("Distortion")]
		public bool m_Distortion = false;
		[Range(0.5f, 8f)] public float m_Frequency = 1f;
		[Range(0.1f, 4f)] public float m_Period = 1.5f;
		[Range(1f, 16f)] public float m_Amplitude = 1f;
		[Header("Radial Blur")]
		public bool m_Blur = false;
		[Range(0f, 1f)] public float m_BlurMin = 0.1f;
		[Range(0f, 1f)] public float m_BlurMax = 0.3f;
		[Range(1f, 6f)] public float m_BlurSpeed = 3f;
		[Header("SleepyEye")]
		public bool m_SleepyEye = false;
		[Range(0f, 0.9f)] public float m_EyeClose = 0.4f;

		void Start()
		{
			if (m_Pipeline != null)
				GraphicsSettings.renderPipelineAsset = m_Pipeline;
			else
				GraphicsSettings.renderPipelineAsset = null;
			m_Feature.SetActive(true);
			m_Feature.m_Mat = m_Mat;
		}
		void Update()
		{
			m_Feature.m_Tp = m_Type;
			m_Feature.m_SleepyEye = m_SleepyEye;
			m_Feature.m_Blur = m_Blur;
			m_Feature.m_Distortion = m_Distortion;
			m_Feature.m_RGBShift = m_RGBShift;

			m_Mat.SetFloat("_RGBShiftFactor", m_RGBShiftFactor);
			m_Mat.SetFloat("_RGBShiftPower", m_RGBShiftPower);
			m_Mat.SetFloat("_RotateRadius", m_RotateRadius);
			m_Mat.SetFloat("_RotateMix", m_RotateMix);
			m_Mat.SetFloat("_SplitAmplitude", m_SplitAmplitude);
			m_Mat.SetFloat("_SplitSpeed", m_SplitSpeed);
			m_Mat.SetFloat("_DeformAmplitude", m_DeformAmplitude);
			m_Mat.SetFloat("_DeformSpeed", m_DeformSpeed);
			m_Mat.SetFloat("_WaggleFocus", m_WaggleFocus);
			m_Mat.SetFloat("_WaggleAmplitude", m_WaggleAmplitude);
			//float strength = Mathf.Sin(Time.time) * 0.5f + 0.5f + 0.1f;
			m_Mat.SetVector("_Dimensions", new Vector4(0.8f, m_EyeClose, 0f, 0f));
			m_Mat.SetFloat("_Frequency", m_Frequency);
			m_Mat.SetFloat("_Period", m_Period);
			m_Mat.SetFloat("_Amplitude", m_Amplitude);
			m_Mat.SetFloat("_BlurMin", m_BlurMin);
			m_Mat.SetFloat("_BlurMax", m_BlurMax);
			m_Mat.SetFloat("_BlurSpeed", m_BlurSpeed);

			Shader.SetGlobalFloat("_Global_Fade", /*m_DrunkIntensity*/1f);
		}
	}
}