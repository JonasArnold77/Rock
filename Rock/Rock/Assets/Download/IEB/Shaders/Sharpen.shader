Shader "Hidden/IEB/Sharpen"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
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
			float outlineColor(float2 uv) {
				float _Size = 0.0015f;
				fixed4 x1 = tex2D(_MainTex, uv + float2(_Size, 0));
				fixed4 x2 = tex2D(_MainTex, uv - float2(_Size, 0));
				float outX = length(x1 - x2);
				fixed4 y1 = tex2D(_MainTex, uv + float2(0, _Size));
				fixed4 y2 = tex2D(_MainTex, uv - float2(0, _Size));
				float outY = length(y1 - y2);
				return (outX + outY) / 2;
			}
			float _Intensity;
			fixed4 frag(v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv);
			float sharpening = outlineColor(i.uv);
				col.rgb = lerp(col.rgb,pow(col.rgb,1+sharpening),_Intensity);
				return col;
			}
			ENDCG
		}
	}
}
