Shader "Hidden/IEB/Heatwave"
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
				float2 screenPos: TEXCOORD1;
			};

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.screenPos = ComputeScreenPos(o.vertex);
				o.uv = v.uv;
				return o;
			}
			sampler2D _MainTex;
			sampler2D _CameraDepthTexture;
			float _Intensity;
			float _Speed;
			float _StartDist;
			float _EndDist;
			float _Width;
			int _Depth;
			float3 WorldPosFromDepth(float depth,float2 uv) {
				float z = depth * 2.0 - 1.0;

				float4 clipSpacePosition = float4(uv * 2.0 - 1.0, z, 1.0);
				float4 viewSpacePosition = mul(unity_CameraInvProjection,clipSpacePosition);

			
				viewSpacePosition /= viewSpacePosition.w;

			    float4 worldSpacePosition = mul(UNITY_MATRIX_V,viewSpacePosition);

				return worldSpacePosition.xyz;
			}
			fixed4 frag(v2f i) : SV_Target
			{ float4 color = 0;
				if (_Depth == 1) {
				float depth = tex2D(_CameraDepthTexture,i.uv).r;
				float3 worldPos = WorldPosFromDepth(depth,i.uv);
			float dist = length(_WorldSpaceCameraPos - worldPos);
				 color = tex2D(_MainTex,i.uv + lerp(0,1,(dist - _StartDist) / (_EndDist - _StartDist)) * (float2(0,sin(((i.uv.x / _Width) * depth * 120 + _Time.x * _Speed) * 120)) * (1 - Linear01Depth(depth)) / 100) * _Intensity / 2);
				}
				else {
					color = tex2D(_MainTex, i.uv +  (float2(0, sin(((i.uv.x*(1-_Width)/10)  * 120 + _Time.x * _Speed) * 120)) * _Intensity / 2));

				}
				return color;
			}
			ENDCG
		}
	}
}
