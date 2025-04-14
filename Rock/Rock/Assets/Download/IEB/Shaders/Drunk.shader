Shader "Hidden/IEB/Drunk"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
	    _Intensity ("Intensity",float) = 0.5
        _Speed ("Speed",float) = 1
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
			float _Intensity;
			float _Speed;
            fixed4 frag (v2f i) : SV_Target
            {   float2 uv = (i.uv - 0.5) * (1-_Intensity/5) + 0.5;
                fixed4 col = tex2D(_MainTex, (uv+(float2(sin(i.uv.x + _Time.r * _Speed*20),cos(i.uv.y+_Time.r*_Speed*20))*2-1)*(_Intensity/30)));
               
                return col;
            }
            ENDCG
        }
    }
}
