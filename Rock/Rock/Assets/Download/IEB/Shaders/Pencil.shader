Shader "Hidden/IEB/Pencil"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_Size("Size",float) = 0.1
		_Color("Color",Color) = (0,0,0,1)
		_BGColor("Color",Color) = (1,1,1,1)
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
				float _Size;
				float4 _Color;
				float4 _BGColor;
				float _Tolerance;
				int _Depth;
				int _IgnoreSkybox;
				float outline(float2 uv) {
					fixed4 x1 = tex2D(_MainTex, uv + float2(_Size, 0));
					fixed4 x2 = tex2D(_MainTex, uv - float2(_Size, 0));
					float outX = length(x1 - x2);
					fixed4 y1 = tex2D(_MainTex, uv + float2(0, _Size));
					fixed4 y2 = tex2D(_MainTex, uv - float2(0, _Size));
					float outY = length(y1 - y2);
					return (outX + outY) / 2;
				}
				sampler2D _CameraDepthTexture;
				float outlineDepth(float2 uv) {
					fixed4 x1 = 1 - Linear01Depth(tex2D(_CameraDepthTexture, uv + float2(_Size, 0)));
					fixed4 x2 = 1 - Linear01Depth(tex2D(_CameraDepthTexture, uv - float2(_Size, 0)));
					float outX = length(x1 - x2);
					fixed4 y1 = 1 - Linear01Depth(tex2D(_CameraDepthTexture, uv + float2(0, _Size)));
					fixed4 y2 = 1 - Linear01Depth(tex2D(_CameraDepthTexture, uv - float2(0, _Size)));
					float outY = length(y1 - y2);
					return (outX + outY) / 2;
				}
			
				fixed4 frag(v2f i) : SV_Target
				{ float outLine = 0;
					if (_Depth == 0) {
					outLine = outline(i.uv);
					}
					else if(_Depth==1){
					outLine = outlineDepth(i.uv);
					}
					else {
						outLine = outlineDepth(i.uv)+outline(i.uv);
						outLine /= 2;
					}
					float4 final = _BGColor;
					if (_IgnoreSkybox == 1) {
						float depth = Linear01Depth(tex2D(_CameraDepthTexture, i.uv));
						if (depth == 1) {
							final = tex2D(_MainTex, i.uv);	
							outLine = 0;
						}
					}
					if (outLine > _Tolerance) {
						final = _Color;
					}
					
					return final;
				}
				ENDCG
			}
		}
}
