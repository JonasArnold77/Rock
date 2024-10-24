Shader "Hidden/IEB/Distort"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_Noise("Texture", 2D) = "white" {}
	}
		SubShader
		{
			// No culling or depth
			Cull Off ZWrite Off ZTest Always

			Pass
			{
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag

				#include "UnityCG.cginc"

				struct appdata
				{
					float4 vertex : POSITION;
					float2 uv : TEXCOORD0;
				};

				struct v2f
				{
					float2 uv : TEXCOORD0;
					float4 vertex : SV_POSITION;
				};

				v2f vert(appdata v)
				{
					v2f o;
					o.vertex = UnityObjectToClipPos(v.vertex);
					o.uv = v.uv;
					return o;
				}
				sampler2D _MainTex;
				sampler2D _Noise;
				vector _Speed;
				float _Intensity;
				float _Scale;
				fixed4 frag(v2f i) : SV_Target
				{ 
					float4 noise = tex2D(_Noise,(_Speed*_Time.x)+(_Scale*2)*(i.uv * _ScreenParams.xy / 1000));
					float4 color = tex2D(_MainTex,i.uv+(noise.rg*2-1)*_Intensity/5);
				
					return color;
				}
				ENDCG
			}
		}
}
