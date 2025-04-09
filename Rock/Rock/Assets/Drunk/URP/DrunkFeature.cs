using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Experimental.Rendering;

namespace QDY.Drunk.URP
{
	public class PingPongRT
	{
		RTHandle m_RtA, m_RtB;
		int m_WhoIsNext = 0;

		RenderTextureDescriptor GetNeedDescriptor(RenderTextureDescriptor desc)
		{
			desc.depthBufferBits = (int)DepthBits.None;
			desc.msaaSamples = 1;
			desc.graphicsFormat = GraphicsFormat.R8G8B8A8_UNorm;
			return desc;
		}
		public void Init(RenderTextureDescriptor rtd)
		{
			RenderTextureDescriptor desc = GetNeedDescriptor(rtd);
			RenderingUtils.ReAllocateIfNeeded(ref m_RtA, desc, FilterMode.Bilinear, TextureWrapMode.Clamp, name:"_RtA");
			RenderingUtils.ReAllocateIfNeeded(ref m_RtB, desc, FilterMode.Bilinear, TextureWrapMode.Clamp, name:"_RtB");
			m_WhoIsNext = 0;
		}
		public void UnInit()
		{
			//RenderTexture.ReleaseTemporary(m_RtA);
			//RenderTexture.ReleaseTemporary(m_RtB);
		}
		public RTHandle CurrNext()
		{
			if (0 == m_WhoIsNext)
			{
				m_WhoIsNext = 1;
				return m_RtA;
			}
			if (1 == m_WhoIsNext)
			{
				m_WhoIsNext = 0;
				return m_RtB;
			}
			return null;
		}
		public RTHandle Curr()
		{
			if (0 == m_WhoIsNext)
				return m_RtA;
			if (1 == m_WhoIsNext)
				return m_RtB;
			return null;
		}
	}
	public class DrunkFeature : ScriptableRendererFeature
	{
		///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		public class DrunkPass : ScriptableRenderPass
		{
			Material m_Mat;
			RTHandle m_CameraColorRT;
			RenderTextureDescriptor m_Descriptor;
			public DemoURP.EType m_Tp;
			public bool m_RGBShift;
			public bool m_Distortion;
			public bool m_Blur;
			public bool m_SleepyEye;
			PingPongRT m_PingPongRT;

			public DrunkPass()
			{
				this.renderPassEvent = RenderPassEvent.AfterRenderingTransparents;
			}
			public void Setup(RTHandle handle, RenderTextureDescriptor rtDesc, Material mat, PingPongRT ppRt)
			{
				m_CameraColorRT = handle;
				m_Descriptor = rtDesc;
				m_Mat = mat;
				m_PingPongRT = ppRt;
			}
			public override void Execute(ScriptableRenderContext context, ref RenderingData data)
			{
				var cameraData = data.cameraData;
				if (cameraData.camera.cameraType != CameraType.Game)
					return;
				if (m_Mat == null)
					return;

				m_PingPongRT.Init(m_Descriptor);

				CommandBuffer cmd = CommandBufferPool.Get("DrunkFeature");
				cmd.SetGlobalTexture("_Global_OrigScene", m_CameraColorRT.nameID);

				// the initial pass
				Blitter.BlitCameraTexture(cmd, m_CameraColorRT, m_PingPongRT.Curr());
				
				// RGB split pass
				if (m_RGBShift) Blitter.BlitCameraTexture(cmd, m_PingPongRT.CurrNext(), m_PingPongRT.Curr(), m_Mat, 0);
				
				// main effect
				if (DemoURP.EType.Rotated == m_Tp)  Blitter.BlitCameraTexture(cmd, m_PingPongRT.CurrNext(), m_PingPongRT.Curr(), m_Mat, 1);
				if (DemoURP.EType.Splitted == m_Tp) Blitter.BlitCameraTexture(cmd, m_PingPongRT.CurrNext(), m_PingPongRT.Curr(), m_Mat, 2);
				if (DemoURP.EType.Waggle == m_Tp)   Blitter.BlitCameraTexture(cmd, m_PingPongRT.CurrNext(), m_PingPongRT.Curr(), m_Mat, 6);
				if (DemoURP.EType.Deform == m_Tp)   Blitter.BlitCameraTexture(cmd, m_PingPongRT.CurrNext(), m_PingPongRT.Curr(), m_Mat, 7);
				
				// additional effect pass: distortion, blur, sleepy eye
				if (m_Distortion) Blitter.BlitCameraTexture(cmd, m_PingPongRT.CurrNext(), m_PingPongRT.Curr(), m_Mat, 4);
				if (m_Blur) Blitter.BlitCameraTexture(cmd, m_PingPongRT.CurrNext(), m_PingPongRT.Curr(), m_Mat, 5);
				if (m_SleepyEye) Blitter.BlitCameraTexture(cmd, m_PingPongRT.CurrNext(), m_PingPongRT.Curr(), m_Mat, 3);
				
				// finally blit to screen
				Blitter.BlitCameraTexture(cmd, m_PingPongRT.CurrNext(), m_CameraColorRT);

				context.ExecuteCommandBuffer(cmd);
				cmd.Clear();
				CommandBufferPool.Release(cmd);
				
				m_PingPongRT.UnInit();
			}
		}
		///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		[HideInInspector] public DemoURP.EType m_Tp;
		[HideInInspector] public bool m_RGBShift = false;
		[HideInInspector] public bool m_SleepyEye = false;
		[HideInInspector] public bool m_Distortion = false;
		[HideInInspector] public bool m_Blur = false;
		[HideInInspector] public Material m_Mat;
		DrunkPass m_Pass;
		PingPongRT m_PingPongRT;

		public override void Create()
		{
			m_Pass = new DrunkPass();
			m_PingPongRT = new PingPongRT();
		}
		public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData data)
		{
			if (data.cameraData.cameraType != CameraType.Game)
				return;
			renderer.EnqueuePass(m_Pass);
		}
		public override void SetupRenderPasses(ScriptableRenderer renderer, in RenderingData data)
		{
			if (data.cameraData.cameraType != CameraType.Game)
				return;
			m_Pass.ConfigureInput(ScriptableRenderPassInput.Color);
			m_Pass.Setup(renderer.cameraColorTargetHandle, data.cameraData.cameraTargetDescriptor, m_Mat, m_PingPongRT);
			m_Pass.m_Tp = m_Tp;
			m_Pass.m_RGBShift = m_RGBShift;
			m_Pass.m_Blur = m_Blur;
			m_Pass.m_Distortion = m_Distortion;
			m_Pass.m_SleepyEye = m_SleepyEye;
		}
	}
}