Shader "[QDY]Drunk/DrunkURP" {
	Properties {
		[HideInInspector]_MainTex  ("Main", 2D) = "white" {}
	}
	HLSLINCLUDE
		#include "Utils.hlsl"
		sampler2D _Global_OrigScene;
		float _Global_Fade;
	ENDHLSL
	SubShader {
		ZTest Off Cull Off ZWrite Off Blend Off Fog { Mode Off }
		Pass {   // pass 0 - RGB split
			HLSLPROGRAM
			#pragma vertex Vert
			#pragma fragment frag

			float _RGBShiftFactor, _RGBShiftPower;

			float4 frag (Varyings input) : SV_TARGET
			{
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
				float2 uv = input.texcoord;

				float2 dist = 0.5 - uv;
				float2 unit = dist / length(dist);

				float ol = length(dist) * _RGBShiftFactor;
				ol = 1.0 - pow(1.0 - ol, _RGBShiftPower);
				float2 offset = unit * ol;
 			
				float4 cr  = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, uv + offset);
				float4 cga = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, uv);
				float4 cb  = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, uv - offset);
				return float4(cr.r, cga.g, cb.b, cga.a);
			}
			ENDHLSL
		}
		Pass {   // pass 1 - rotate view
			HLSLPROGRAM
			#pragma vertex Vert
			#pragma fragment frag

			float _RotateRadius, _RotateMix;

			float4 frag (Varyings input) : SV_TARGET
			{
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
				float2 uv = input.texcoord;
				
				float angle = _Time.y;
				float2 offset = float2(cos(angle), sin(angle * 2.0)) * _RotateRadius;
				float4 shifted = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, uv + offset);
				float4 orig = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, uv);
				return lerp(orig, shifted, _RotateMix);
			}
			ENDHLSL
		}
		Pass {   // pass 2 - split view
			HLSLPROGRAM
			#pragma vertex Vert
			#pragma fragment frag

			float _SplitAmplitude, _SplitSpeed;

			float4 frag (Varyings input) : SV_TARGET
			{
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
				float2 uv = input.texcoord;

				float sq = sin(_Time.y * _SplitSpeed) * _SplitAmplitude;
				float4 tc = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, uv);
				float4 tl = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, uv - float2(sin(sq), 0));
				float4 tR = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, uv + float2(sin(sq), 0));
				float4 tD = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, uv - float2(0, sin(sq)));
				float4 tU = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, uv + float2(0, sin(sq)));
				return (tc + tl + tR + tD + tU) / 5.0;
			}
			ENDHLSL
		}
		Pass {   // pass 3 - sleepy eye
			HLSLPROGRAM
			#pragma vertex Vert
			#pragma fragment frag

			float2 _Dimensions;

			half4 frag (Varyings input) : SV_TARGET
			{
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
				float2 uv = input.texcoord;
				
				float2 gradient = 0.5 - uv;
				gradient.x = gradient.x * (1.0 / _Dimensions.x);
				gradient.y = gradient.y * (1.0 / _Dimensions.y);
				float dist = length(gradient);
				half4 tc = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, uv);
				tc = lerp(tc, 0.0, dist);

				half4 orig = tex2D(_Global_OrigScene, uv);
				return lerp(orig, tc, _Global_Fade);
			}
			ENDHLSL
		}
		Pass {   // pass 4 - distort with noise
			HLSLPROGRAM
			#pragma vertex Vert
			#pragma fragment frag

			float _Frequency, _Period, _Amplitude;

			float4 frag (Varyings input) : SV_TARGET
			{
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
				float2 uv = input.texcoord;

				float n = snoise(float3((uv * _Frequency), _Period + _Time.y)) * PI;
				float2 offset = float2(cos(n), sin(n)) * _Amplitude * _BlitTexture_TexelSize.xy;
				return SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, uv + offset);
			}
			ENDHLSL
		}
		Pass {   // pass 5 - dynamic radial blur
			HLSLPROGRAM
			#pragma vertex Vert
			#pragma fragment frag

			float _BlurMin, _BlurMax, _BlurSpeed;

			half4 frag (Varyings input) : SV_TARGET
			{
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
				float2 uv = input.texcoord;

				half4 c = 0;
				float t = lerp(_BlurMin, _BlurMax, (sin(_Time.y * _BlurSpeed) + 1) / 2);
				for (int n = 0; n <= 25; n++)
				{
					float q = n / 25.0;
					c += SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, uv + (0.5 - uv) * q * t) / 25.0;
				}

				half4 orig = tex2D(_Global_OrigScene, uv);
				return lerp(orig, c, _Global_Fade);
			}
			ENDHLSL
		}
		Pass {   // pass 6 - waggle
			HLSLPROGRAM
			#pragma vertex Vert
			#pragma fragment frag
			
			float _WaggleFocus, _WaggleAmplitude;

			half4 frag (Varyings input) : SV_TARGET
			{
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
				float2 uv = input.texcoord;

				float t = _Time.y;
				float2 o = float2(
					sin(t * 1.25 + 75.0 + uv.y * 0.5) + sin(t * 2.75 - 18.0 - uv.x * 0.25),
					sin(t * 1.75 - 125.0 + uv.x * 0.25) + sin(t * 2.25 + 4.0 - uv.y * 0.5)) * 0.25 + 0.5;

				float z = sin((t + 234.5) * 3.0) * _WaggleAmplitude + _WaggleFocus;
				float2 uv2 = ((uv - o) * z + o);
				half4 c = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, uv2);

				half4 orig = tex2D(_Global_OrigScene, uv);
				return lerp(orig, c, _Global_Fade);
			}
			ENDHLSL
		}
		Pass {   // pass 7 - deform
			HLSLPROGRAM
			#pragma vertex Vert
			#pragma fragment frag

			float _DeformAmplitude, _DeformSpeed;

			half4 frag (Varyings input) : SV_TARGET
			{
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
				float2 uv = input.texcoord;

				float2 waved_uv = uv + (sin(uv * 2.0 * PI) * _DeformAmplitude * sin(_Time.y * _DeformSpeed));
				return SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, waved_uv);
			}
			ENDHLSL
		}
	}
	FallBack Off
}