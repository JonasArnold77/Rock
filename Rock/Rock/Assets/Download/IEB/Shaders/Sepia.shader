Shader "Hidden/IEB/Sepia"
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
			float _Intensity;
            sampler2D _MainTex;

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 original = tex2D(_MainTex, i.uv);
			    fixed Y = dot(fixed3(0.299, 0.587, 0.114), original.rgb);
			    fixed4 sepiaConvert = float4 (0.191, -0.054, -0.221, 0.0);
			    fixed4 output = sepiaConvert + Y;
                return lerp(original,output,_Intensity);
            }
            ENDCG
        }
    }
}
