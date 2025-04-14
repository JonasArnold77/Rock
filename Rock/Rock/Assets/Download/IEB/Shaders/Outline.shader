Shader "Hidden/IEB/Outline"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
	    _Size ("Size",float) = 0.1
        _Color ("Color",Color) = (0,0,0,1)
	    _Depth ("Depth",int) = 1
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

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            sampler2D _MainTex;
			sampler2D _CameraDepthTexture;
			float _Size;
			float4 _Color;
			int _HQ;
			float outline(float2 uv) {
				fixed4 x1 = 1-Linear01Depth(tex2D(_CameraDepthTexture, uv + float2(_Size, 0)));
				fixed4 x2 = 1 - Linear01Depth(tex2D(_CameraDepthTexture, uv - float2(_Size, 0)));
				float outX = length(x1 - x2);
				fixed4 y1 = 1 - Linear01Depth(tex2D(_CameraDepthTexture, uv + float2(0, _Size)));
				fixed4 y2 = 1 - Linear01Depth(tex2D(_CameraDepthTexture, uv - float2(0, _Size)));
				float outY = length(y1 - y2);
				return (outX + outY) / 2;
			}
			float outlineHQ(float2 uv) {
				fixed4 x1 = 1 - Linear01Depth(tex2D(_CameraDepthTexture, uv + float2(_Size, 0)));
				fixed4 x2 = 1 - Linear01Depth(tex2D(_CameraDepthTexture, uv - float2(_Size, 0)));
				fixed4 x3 = 1 - Linear01Depth(tex2D(_CameraDepthTexture, uv + float2(_Size, _Size)));
				fixed4 x4 = 1 - Linear01Depth(tex2D(_CameraDepthTexture, uv - float2(_Size, _Size)));
				float outX = length(x1 - x2) + length(x3 - x4);
				fixed4 y1 = 1 - Linear01Depth(tex2D(_CameraDepthTexture, uv + float2(0, _Size)));
				fixed4 y2 = 1 - Linear01Depth(tex2D(_CameraDepthTexture, uv - float2(0, _Size)));
				fixed4 y3 = 1 - Linear01Depth(tex2D(_CameraDepthTexture, uv + float2(_Size, _Size)));
				fixed4 y4 = 1 - Linear01Depth(tex2D(_CameraDepthTexture, uv - float2(_Size, _Size)));
				float outY = length(y1 - y2) + length(y3 - y4);
				return (outX + outY) / 4;
			}
			float outlineColor(float2 uv) {
				fixed4 x1 = tex2D(_MainTex, uv + float2(_Size, 0));
				fixed4 x2 = tex2D(_MainTex, uv - float2(_Size, 0));
				float outX = length(x1 - x2);
				fixed4 y1 = tex2D(_MainTex, uv + float2(0, _Size));
				fixed4 y2 = tex2D(_MainTex, uv - float2(0, _Size));
				float outY = length(y1 - y2);
				return (outX + outY) / 2;
			}
			float outlineColorHQ(float2 uv) {
				fixed4 x1 = tex2D(_MainTex, uv + float2(_Size, 0));
				fixed4 x2 = tex2D(_MainTex, uv - float2(_Size, 0));
				fixed4 x3 = tex2D(_MainTex, uv + float2(_Size, _Size));
				fixed4 x4 = tex2D(_MainTex, uv - float2(_Size, _Size));
				float outX = length(x1 - x2)+ length(x3 - x4);
				fixed4 y1 = tex2D(_MainTex, uv + float2(0, _Size));
				fixed4 y2 = tex2D(_MainTex, uv - float2(0, _Size));
				fixed4 y3 = tex2D(_MainTex, uv + float2(0, _Size));
				fixed4 y4 = tex2D(_MainTex, uv - float2(0, _Size));
				float outY = length(y1 - y2) + length(y3 - y4);
				return (outX + outY) / 4;
			}
			int _Depth;
			float _Tolerance;
            fixed4 frag (v2f i) : SV_Target
			{ float outLine = 0;
				if (_Depth == 1) {
				fixed4 col = tex2D(_MainTex, i.uv);
				float depth = Linear01Depth(tex2D(_CameraDepthTexture,i.uv));
				if (_HQ == 1) {
					outLine = outlineHQ(i.uv);
				}
				else {
					 outLine = outline(i.uv);
				}
				float4 final = col;
				if (outLine > _Tolerance) {
					final = _Color;
				}
				return final;
				}
			else if(_Depth==0) {
				if (_HQ == 1) {
				 outLine = outlineColorHQ(i.uv);
				}
				else {
			 outLine = outlineColor(i.uv);
				}
                   
					float4 final = tex2D(_MainTex,i.uv);
					if (outLine > _Tolerance) {
						final = _Color;
					}

					return final;
} 
			
			else {
					if (_HQ == 1) {
						outLine = outlineColorHQ(i.uv)+ outlineHQ(i.uv);
					}
					else {
						outLine = outlineColor(i.uv)+outline(i.uv);
					}
					outLine /= 2;
					float4 final = tex2D(_MainTex, i.uv);
					if (outLine > _Tolerance) {
						final = _Color;
					}

					return final;
				}
			}
            
            ENDCG
        }
    }
}
