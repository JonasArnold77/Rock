Shader "Hidden/IEB/ColorPalette"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
	   
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
			float4 _Colors[128];
			int _ColorCount;
            sampler2D _MainTex;
			int _Brightness;

            fixed4 frag (v2f i) : SV_Target
            {
                float3 col = tex2D(_MainTex, i.uv).rgb;
				if (_Brightness == 0) {
					float4 bestColor = float4(1, 1, 1, 1);
					float3 bestDiff = float3(1000, 1000, 1000);
					for (float i = 0; i < _ColorCount; ++i) {
						float3 palCol = _Colors[i].rgb;
						float3 diff = abs(col.rgb - palCol.rgb);
						if (length(diff) < length(bestDiff)) {
							bestDiff = diff;
							bestColor.rgb = palCol;
						}
					}
					return bestColor;
				}
				else {
					float4 bestColor = float4(1, 1, 1, 1);
					float3 bestDiff = float3(1000, 1000, 1000);
					for (float i = 0; i < _ColorCount; ++i) {
						float3 palCol = _Colors[i].rgb;
						float3 diff = abs(length(col.rgb)/3 - length(palCol.rgb)/3);
						if (length(diff) < length(bestDiff)) {
							bestDiff = diff;
							bestColor.rgb = palCol;
						}
					}
					return bestColor;
				}
               
            }
            ENDCG
        }
    }
}
