// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Screen Space Multiple Scattering for Unity
//
// Copyright (C) 2015, 2016 Keijiro Takahashi, OCASM
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
//

#include "UnityCG.cginc"

// Mobile: use RGBM instead of float/half RGB
#define USE_RGBM defined(SHADER_API_MOBILE)

sampler2D _MainTex; sampler2D _MainTexA;
sampler2D _BaseTex;
float2 _MainTex_TexelSize; float2 _MainTexA_TexelSize;
float2 _BaseTex_TexelSize;
half4 _MainTex_ST;
half4 _BaseTex_ST;

sampler2D _MainTexB;
float2 _MainTexB_TexelSize;
half4 _MainTexB_ST;

float _PrefilterOffs;
half _Threshold;
half3 _Curve;
float _SampleScale;
half3 _Intensity;


// SMSS
sampler2D _CameraDepthTexture;
sampler2D _FadeTex;
sampler2D _FogTex;
float _Radius;
half4 _BlurTint;
half _BlurWeight;

// Brightness function
half Brightness(half3 c)
{
    return max(max(c.r, c.g), c.b);
}

// 3-tap median filter
half3 Median(half3 a, half3 b, half3 c)
{
    return a + b + c - min(min(a, b), c) - max(max(a, b), c);
}

// Clamp HDR value within a safe range
half3 SafeHDR(half3 c) { return min(c, 65000); }
half4 SafeHDR(half4 c) { return min(c, 65000); }

// RGBM encoding/decoding
half4 EncodeHDR(float3 rgb)
{
#if USE_RGBM
    rgb *= 1.0 / 8;
    float m = max(max(rgb.r, rgb.g), max(rgb.b, 1e-6));
    m = ceil(m * 255) / 255;
    return half4(rgb / m, m);
#else
    return half4(rgb, 0);
#endif
}

float3 DecodeHDR(half4 rgba)
{
#if USE_RGBM
    return rgba.rgb * rgba.a * 8;
#else
    return rgba.rgb;
#endif
}

// Downsample with a 4x4 box filter
half3 DownsampleFilter(float2 uv)
{
   // return tex2D(_MainTexB, uv);

    float4 d = _MainTexB_TexelSize.xyxy * float4(-1, -1, +1, +1);

    half3 s;
    s  = DecodeHDR(tex2D(_MainTexB, uv + d.xy));
    s += DecodeHDR(tex2D(_MainTexB, uv + d.zy));
    s += DecodeHDR(tex2D(_MainTexB, uv + d.xw));
    s += DecodeHDR(tex2D(_MainTexB, uv + d.zw));

    return s * (1.0 / 4);
}

// Downsample with a 4x4 box filter + anti-flicker filter
half3 DownsampleAntiFlickerFilter(float2 uv)
{
    float4 d = _MainTexB_TexelSize.xyxy * float4(-1, -1, +1, +1);

    half3 s1 = DecodeHDR(tex2D(_MainTexB, uv + d.xy));
    half3 s2 = DecodeHDR(tex2D(_MainTexB, uv + d.zy));
    half3 s3 = DecodeHDR(tex2D(_MainTexB, uv + d.xw));
    half3 s4 = DecodeHDR(tex2D(_MainTexB, uv + d.zw));

    // Karis's luma weighted average (using brightness instead of luma)
    half s1w = 1 / (Brightness(s1) + 1);
    half s2w = 1 / (Brightness(s2) + 1);
    half s3w = 1 / (Brightness(s3) + 1);
    half s4w = 1 / (Brightness(s4) + 1);
    half one_div_wsum = 1 / (s1w + s2w + s3w + s4w);

    return (s1 * s1w + s2 * s2w + s3 * s3w + s4 * s4w) * one_div_wsum;
}
half3 UpsampleFilterA(float2 uv)
{
#if HIGH_QUALITY
	// 9-tap bilinear upsampler (tent filter)
	float4 d = _MainTexA_TexelSize.xyxy * float4(1, 1, -1, 0) * _SampleScale;

	half3 s;
	s =  DecodeHDR(tex2D(_MainTexA, uv - d.xy));
	s += DecodeHDR(tex2D(_MainTexA, uv - d.wy)) * 2;
	s += DecodeHDR(tex2D(_MainTexA, uv - d.zy));

	s += DecodeHDR(tex2D(_MainTexA, uv + d.zw)) * 2;
	s += DecodeHDR(tex2D(_MainTexA, uv)) * 4;
	s += DecodeHDR(tex2D(_MainTexA, uv + d.xw)) * 2;

	s += DecodeHDR(tex2D(_MainTexA, uv + d.zy));
	s += DecodeHDR(tex2D(_MainTexA, uv + d.wy)) * 2;
	s += DecodeHDR(tex2D(_MainTexA, uv + d.xy));

	return s * (1.0 / 16);
#else
	// 4-tap bilinear upsampler
	float4 d = _MainTexA_TexelSize.xyxy * float4(-1, -1, +1, +1) * (_SampleScale * 0.5);

	half3 s;
	s =  DecodeHDR(tex2D(_MainTexA, uv + d.xy));
	s += DecodeHDR(tex2D(_MainTexA, uv + d.zy));
	s += DecodeHDR(tex2D(_MainTexA, uv + d.xw));
	s += DecodeHDR(tex2D(_MainTexA, uv + d.zw));

	return s * (1.0 / 4);
#endif
}
half3 UpsampleFilter(float2 uv)
{
#if HIGH_QUALITY
    // 9-tap bilinear upsampler (tent filter)
    float4 d = _MainTexB_TexelSize.xyxy * float4(1, 1, -1, 0) * _SampleScale;

    half3 s;
    s  = DecodeHDR(tex2D(_MainTexB, uv - d.xy));
    s += DecodeHDR(tex2D(_MainTexB, uv - d.wy)) * 2;
    s += DecodeHDR(tex2D(_MainTexB, uv - d.zy));

    s += DecodeHDR(tex2D(_MainTexB, uv + d.zw)) * 2;
    s += DecodeHDR(tex2D(_MainTexB, uv       )) * 4;
    s += DecodeHDR(tex2D(_MainTexB, uv + d.xw)) * 2;

    s += DecodeHDR(tex2D(_MainTexB, uv + d.zy));
    s += DecodeHDR(tex2D(_MainTexB, uv + d.wy)) * 2;
    s += DecodeHDR(tex2D(_MainTexB, uv + d.xy));

    return s * (1.0 / 16);
#else
    // 4-tap bilinear upsampler
    float4 d = _MainTexB_TexelSize.xyxy * float4(-1, -1, +1, +1) * (_SampleScale * 0.5);

    half3 s;
    s  = DecodeHDR(tex2D(_MainTexB, uv + d.xy));
    s += DecodeHDR(tex2D(_MainTexB, uv + d.zy));
    s += DecodeHDR(tex2D(_MainTexB, uv + d.xw));
    s += DecodeHDR(tex2D(_MainTexB, uv + d.zw));

    return s * (1.0 / 4);
#endif
}

//
// Vertex shader
//

v2f_img vert(appdata_img v)
{
    v2f_img o;
#if UNITY_VERSION >= 540
    o.pos = UnityObjectToClipPos(v.vertex);
    o.uv = UnityStereoScreenSpaceUVAdjust(v.texcoord, _MainTexB_ST);
#else
    o.pos = UnityObjectToClipPos(v.vertex);
    o.uv = v.texcoord;
#endif
    o.uv.y = 1 - o.uv.y;
    return o;
}

struct v2f_multitex
{
    float4 pos : SV_POSITION;
    float2 uvMain : TEXCOORD0;
    float2 uvBase : TEXCOORD1;
};

v2f_multitex vert_multitex(appdata_img v)
{
    v2f_multitex o;
#if UNITY_VERSION >= 540
    o.pos = UnityObjectToClipPos(v.vertex);
    o.uvMain = UnityStereoScreenSpaceUVAdjust(v.texcoord, _MainTexB_ST);
    o.uvBase = UnityStereoScreenSpaceUVAdjust(v.texcoord, _BaseTex_ST);
#else
    o.pos = UnityObjectToClipPos(v.vertex);
    o.uvMain = v.texcoord;
    o.uvBase = v.texcoord;
#endif
#if UNITY_UV_STARTS_AT_TOP
    if (_BaseTex_TexelSize.y < 0.0)
        o.uvBase.y = 1.0 - v.texcoord.y;
#endif
    o.uvBase.y = 1.0 - v.texcoord.y;
    o.uvMain.y = 1.0 - v.texcoord.y;
    return o;
}

//
// fragment shader
//

// SMSS
half AdjustDepth(half d){
	d = tex2D(_FadeTex, half2(d, 0.5));
	return saturate(d);
}

half4 frag_prefilter(v2f_img i) : SV_Target
{

    //return tex2D(_FogTex, i.uv);
    //return tex2D(_MainTexB, i.uv);
   

	float2 uv = i.uv + _MainTexB_TexelSize.xy * _PrefilterOffs;

#if ANTI_FLICKER
    float3 d = _MainTexB_TexelSize.xyx * float3(1, 1, 0);
    half4 s0 = SafeHDR(tex2D(_MainTexB, uv));
    half3 s1 = SafeHDR(tex2D(_MainTexB, uv - d.xz).rgb);
    half3 s2 = SafeHDR(tex2D(_MainTexB, uv + d.xz).rgb);
    half3 s3 = SafeHDR(tex2D(_MainTexB, uv - d.zy).rgb);
    half3 s4 = SafeHDR(tex2D(_MainTexB, uv + d.zy).rgb);
    half3 m = Median(Median(s0.rgb, s1, s2), s3, s4);
#else
    half4 s0 = SafeHDR(tex2D(_MainTexB, uv));
    half3 m = s0.rgb;
#endif

#if UNITY_COLORSPACE_GAMMA
    m = GammaToLinearSpace(m);
#endif

   

    // Pixel brightness
    half br = Brightness(m);

    // Under-threshold part: quadratic curve
    half rq = clamp(br - _Curve.x, 0, _Curve.y);
    rq = _Curve.z * rq * rq;

    // Combine and apply the brightness response curve.
    m *= max(rq, br - _Threshold) / max(br, 1e-5);

    //return float4(m,1)*101;

    // SMSS 
    half depth = tex2D(_FogTex, i.uv); // Deferred
    // half depth = tex2D(_FogTex, float2(i.uv.x, 1 - i.uv.y)); // Forward
   	depth = AdjustDepth(depth);

    return EncodeHDR(m * depth) * _BlurTint*1;
}

half4 frag_downsample1(v2f_img i) : SV_Target
{
#if ANTI_FLICKER
    return EncodeHDR(DownsampleAntiFlickerFilter(i.uv));
#else
    return EncodeHDR(DownsampleFilter(i.uv));
#endif
}

half4 frag_downsample2(v2f_img i) : SV_Target
{
    return EncodeHDR(DownsampleFilter(i.uv));
}
sampler2D _BaseTexA;
half4 frag_upsample(v2f_multitex i) : SV_Target
{
    half3 base = DecodeHDR(tex2D(_BaseTexA, i.uvBase));
    half3 blur = UpsampleFilter(i.uvMain);

    return EncodeHDR(base + blur * (1 + _BlurWeight)) / (1 + (_BlurWeight * 0.735));
}




half3 UpsampleFilterA1(float2 uv)
{
#if HIGH_QUALITY
    // 9-tap bilinear upsampler (tent filter)
    float4 d = _MainTexA_TexelSize.xyxy * float4(1, 1, -1, 0) * _SampleScale;

    half3 s;
    s = DecodeHDR(tex2D(_MainTexA, uv - d.xy));
    s += DecodeHDR(tex2D(_MainTexA, uv - d.wy)) * 2;
    s += DecodeHDR(tex2D(_MainTexA, uv - d.zy));

    s += DecodeHDR(tex2D(_MainTexA, uv + d.zw)) * 2;
    s += DecodeHDR(tex2D(_MainTexA, uv)) * 4;
    s += DecodeHDR(tex2D(_MainTexA, uv + d.xw)) * 2;

    s += DecodeHDR(tex2D(_MainTexA, uv + d.zy));
    s += DecodeHDR(tex2D(_MainTexA, uv + d.wy)) * 2;
    s += DecodeHDR(tex2D(_MainTexA, uv + d.xy));

    return s * (1.0 / 16);
#else
    // 4-tap bilinear upsampler
    float4 d = _MainTexA_TexelSize.xyxy * float4(-1, -1, +1, +1) * (_SampleScale * 0.5);

    half3 s;
    s = DecodeHDR(tex2D(_MainTexA, uv + d.xy));
    s += DecodeHDR(tex2D(_MainTexA, uv + d.zy));
    s += DecodeHDR(tex2D(_MainTexA, uv + d.xw));
    s += DecodeHDR(tex2D(_MainTexA, uv + d.zw));

    return s * (1.0 / 4);
#endif
}
half4 frag_upsample_final(v2f_multitex i) : SV_Target
{
    //return  tex2D(_MainTexA, i.uvMain)*0.1;
    //return  tex2D(_FogTex, i.uvBase);
    //return  tex2D(_BaseTexA, i.uvBase)*2;

	half4 base = tex2D(_BaseTexA, i.uvBase);
    half3 blur = UpsampleFilterA1(i.uvMain);

   // return float4(blur.rgb, 1);
   // return float4(tex2D(_MainTexA, i.uvMain).rgb, 1);
    //blur = tex2D(_MainTexA, i.uvBase).rgb*4;

#if UNITY_COLORSPACE_GAMMA
    base.rgb = GammaToLinearSpace(base.rgb);
#endif

    // SMSS
    half3 depth = tex2D(_FogTex, i.uvBase); // Deferred
    // half depth = tex2D(_FogTex, float2(i.uvBase.x, 1 - i.uvBase.y)); // Forward
    float depth_r = AdjustDepth(depth.r);
	
	//ERROR - get proper depth for URP here - FIX
	//depth = 1;

    //return float4(depth, 1);

	//return float4(blur, 1);
	//return float4(depth, depth, depth,1);
    return lerp(base + depth.rgbb * _Intensity.y * base, half4(blur + _Intensity.z * depth, 1) * (1 / _Radius), clamp(depth_r, 0, _Intensity.x));
	//return lerp(base, half4(blur, 1) * (1 / _Radius), clamp(depth_r, 0, _Intensity));
	//return float4(blur,1);// lerp(base, half4(blur, 1) * (1 / _Radius), clamp(depth, 0, _Intensity));
}


half4 frag_upsample_final2(v2f_multitex i) : SV_Target
{
	half4 fog = tex2D(_FogTex, i.uvBase);
	half4 main = tex2D(_BaseTex, i.uvBase);
//#if UNITY_COLORSPACE_GAMMA
//	fog.rgb = GammaToLinearSpace(base.rgb);
//#endif
	//return fog;
	//return main;
	return float4( fog.rgb*4 + main.rgb*2,1)/3;
	//return float4(main.rgb*fog.rgb+fog.rgb + main.rgb,1);
}

half4 frag_test(v2f_multitex i) : SV_Target
{
    //half4 fog = tex2D(_FogTex, i.uvBase);
    half4 main = tex2D(_BaseTex, i.uvBase);
    //#if UNITY_COLORSPACE_GAMMA
    //	fog.rgb = GammaToLinearSpace(base.rgb);
    //#endif
        //return fog;
        //return main;
        return main;
        //return float4(main.rgb*fog.rgb+fog.rgb + main.rgb,1);
}