Shader "Hidden/IEB/OldTV"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
	    _Seed("Seed",float) = 0.5
	    _Intensity("Intensity",float) = 0.5
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
			float _Seed;
			float _Intensity1;
			float _Intensity2;
			float _BandScale;
			float _BandSpeed;
			float _BandIntensity1;
			float _BandIntensity2;
			float rand(float2 co) {
				return frac(sin(_Seed + dot(co.xy, float2(12.9898, 78.233))) * 43758.5453);
			}
            sampler2D _MainTex;

            fixed4 frag (v2f i) : SV_Target
			{ float4 color = tex2D(_MainTex,i.uv);
			float2 uv = i.uv;
			float colFactor =1; 
			if (cos(uv.y + _Time.y * 66*_BandSpeed) > lerp(1,0,_BandScale/2)) {
				uv.y += rand(i.uv) * 0.012*_BandIntensity2;
				colFactor = lerp(1,1.15,_BandIntensity1);
			}

                float4 col = colFactor*lerp(color,tex2D(_MainTex, uv).r,_Intensity1)*lerp(rand(i.uv)/1.3+0.1,1,1-_Intensity2);
		
			return col;
            }
            ENDCG
        }
    }
}
