	Shader "Hidden/IEB/Retro"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
	    _Range ("Color Range",int) = 8
        _Seed ("Seed",float) = 0.5
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
			float rand(float2 co) {
				return frac(sin(_Seed+dot(co.xy, float2(12.9898, 78.233))) * 43758.5453);
			}
            sampler2D _MainTex;
			int _Range;
            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
			col = round(col * _Range) / _Range;
                return col+(rand(i.uv)*0.03)-0.03;
            }
            ENDCG
        }
    }
}
