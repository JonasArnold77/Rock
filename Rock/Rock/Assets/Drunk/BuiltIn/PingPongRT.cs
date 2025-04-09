using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace QDY.Drunk.BuiltIn
{
	public class PingPongRT
	{
		RenderTexture m_RtA;
		RenderTexture m_RtB;
		int m_WhoIsNext = 0;

		public void Init()
		{
			m_RtA = RenderTexture.GetTemporary(Screen.width, Screen.height, 0);
			m_RtB = RenderTexture.GetTemporary(Screen.width, Screen.height, 0);
			m_WhoIsNext = 0;
		}
		public void UnInit()
		{
			RenderTexture.ReleaseTemporary(m_RtA);
			RenderTexture.ReleaseTemporary(m_RtB);
		}
		public RenderTexture CurrNext()
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
		public RenderTexture Curr()
		{
			if (0 == m_WhoIsNext)
				return m_RtA;
			if (1 == m_WhoIsNext)
				return m_RtB;
			return null;
		}
	}
}