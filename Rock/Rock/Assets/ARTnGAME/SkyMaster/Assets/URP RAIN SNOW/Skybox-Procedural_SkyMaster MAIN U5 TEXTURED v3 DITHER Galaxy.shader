// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "SkyMaster/SkyMasterShaderE_SKYBOX U5 v3 DITHER Galaxy" 
{ 
 
 Properties {
        _MainTex ("Greyscale (R) Alpha (A)", 2D) = "white" {}
        _ColorRamp ("Colour Palette", 2D) = "gray" {}
        _Coloration ("Coloration Ammount", Float) = 0
        _TintColor("Color Tint", Color) = (0,0,0,0)
       
        
        fOuterRadius ("fOuterRadius", Float) = 0
        fOuterRadius2 ("fOuterRadius2", Float) = 0
        fInnerRadius ("fInnerRadius", Float) = 0
        fInnerRadius2 ("fInnerRadius2", Float) = 0
        
        fKrESun("fKrESun", Float) = 0			// Kr * ESun
		fKmESun("fKmESun", Float) = 0			// Km * ESun
		fKr4PI("fKr4PI", Float) = 0			// Kr * 4 * PI
		fKm4PI("fKm4PI", Float) = 0			// Km * 4 * PI
		fScale("fScale", Float) = 0			// 1 / (fOuterRadius - fInnerRadius)
		fScaleDepth("fScaleDepth", Float) = 0		//
		fScaleOverScaleDepth("fScaleOverScaleDepth", Float) = 0 // fScale / fScaleDepth
						
		fExposure ("fExposure", Float) = 0		// HDR 
    		    					
		v3CameraPos("v3CameraPos", Vector) = (0,0,0)		// camera
		v3LightDir("v3LightDir", Vector) = (0,0,0)		// light source
		v3LightDirMoon("v3LightDirMoon", Vector) = (0,0,0)		// moon light
		v3InvWavelength("v3InvWavelength", Vector) = (0,0,0)//  	

 		fCameraHeight("fCameraHeight", Float) = 0    // height
  		fCameraHeight2("fCameraHeight2", Float) = 0   // 	  	
	
    	
		fSamples ("fSamples", Float) = 2

		Bump_strenght("Bump_strenght", Float) = 0
		g("g", Float) = 0				// The Mie phase asymmetry factor
		g2("g2", Float) = 0				// The Mie phase asymmetry factor squared
		
		Horizon_adj("Horizon", Float) = 0
		HorizonY("HorizonY", Float) = 0
		GroundColor("GroundColor", Color) = (0,0,0,1)	
		
		_Obliqueness ("Obliqueness", Vector) = (0, 0, 0, 0)
		
		White_cutoff ("White replace factor", Float) =  1.1//0.9991
		SunBlend ("Sun blend", Float) = 26
		SunColor("Sun Tint", Color) = (0,0,0,0)


		//v0.1
		_Color("Light Color", Color) = (1,1,1,1)
		_MainTexGalaxy("Base (RGB)", 2D) = "white" {} 
		_Jitter("Jitter", 2D) = "white" {}
		_SunDir("Sun Direction", Vector) = (0,1,0,0)
		_CloudCover("Cloud Cover", Range(0,1)) = 0.5
		_CloudSharpness("Cloud Sharpness", Range(1,30)) = 8
		_CloudDensity("Density", Range(0,5)) = 1
		_CloudSpeed("Cloud Speed", Vector) = (0.001, 0, 0, 0)
		_Light("Sun Intensity", Range(0,10)) = 1
		_Bump("Bump", 2D) = "white" {}

		_galaxySpeedHor("Galaxy rotation Horizontal", Float) = 0
		_galaxySpeedVer("Galaxy rotation Vertical", Float) = 0

 }

	 SubShader
		{
			////////////////////////////Tags { "RenderType"="Transparent" }
				Tags { "Queue" = "Background" "RenderType" = "Background" "PreviewType" = "Skybox" }
				Cull Off ZWrite Off
			Pass
			{
				//Cull Front

				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#include "UnityCG.cginc"
				#include "Lighting.cginc"
				//#pragma exclude_renderers opengl 
				//#pragma target 3.0	

				//#pragma glsl


				//v0.1a
				float _galaxySpeedHor;
				float _galaxySpeedVer;

				//v0.1
				sampler2D _MainTexGalaxy;
				float4 _MainTexGalaxy_ST;
				//float4 _Bump_ST;
				sampler2D _Jitter;
				//sampler2D _Bump;
				float4 _Color;
				float4 _Ambient;
				float4 _SunDir;
				float4 _CloudSpeed;
				float _Light;
				float _CloudCover;
				uniform float _CloudDensity;
				float _CloudSharpness;



			samplerCUBE _CubeTex;
  			
  			uniform float fOuterRadius;		// outerradius
			uniform float fOuterRadius2;	// 
			uniform float fInnerRadius;		// inner radius
			uniform float fInnerRadius2;	// 
 			
  				  
      		uniform float fKrESun;			// Kr * ESun
			uniform float fKmESun;			// Km * ESun
			uniform float fKr4PI;			// Kr * 4 * PI
			uniform float fKm4PI;			// Km * 4 * PI
			uniform float fScale;			// 1 / (fOuterRadius - fInnerRadius)
			uniform float fScaleDepth;		//
			uniform float fScaleOverScaleDepth; // fScale / fScaleDepth
						
			uniform float fExposure = 0.7;		// HDR 
    		
    					
			uniform float3 v3CameraPos;		// camera
			uniform float3 v3LightDir;		// light source
			uniform float3 v3LightDirMoon;		// moon light
			uniform float3 v3InvWavelength; //  	

 		   	uniform float fCameraHeight;    // height
  			uniform float fCameraHeight2;   // 			  	
		 				 		
		 	//	
    		uniform int nSamples = 2;
			uniform float fSamples = 2.0 ;

			uniform float Bump_strenght=2;
			uniform float _Coloration =0; 
			
			uniform float Horizon_adj = 0;
			uniform float HorizonY = 0;
			
			uniform float g;				// The Mie phase asymmetry factor
			uniform float g2;				// The Mie phase asymmetry factor squared
			fixed4 _TintColor;
			fixed4 GroundColor;
			
			float4 _Obliqueness;
			float White_cutoff; 
			float SunBlend;
			float3 SunColor;


			//DITHER
			//https://github.com/cubedparadox/Cubeds-Unity-Shaders
			float3 dither(float2 p) {
				float3 p3 = frac(float3(p.xyx) * (float3(443.8975, 397.2973, 491.1871) + _Time.y));
				p3 += dot(p3, p3.yxz + 19.19);
				return (frac(float3((p3.x + p3.y)*p3.z, (p3.x + p3.z)*p3.y, (p3.y + p3.z)*p3.x)) - 0.5) * 2 * 0.00075;
			}


	
			struct fragIO 
			{
				float3 c0 : COLOR0;
    			float3 c1 : COLOR1;
    			float3 v3Direction : TEXCOORD0;
    			float4 pos : SV_POSITION;    			
    			float2 uv : TEXCOORD1; 
    			half4 normalAndSunExp : TEXCOORD2;  
				float3 v3DirectionA : TEXCOORD3;
				UNITY_VERTEX_OUTPUT_STEREO //VR1
			};
			
			float scale(float fCos)
			{
				float x = 1.0 - fCos;
				return fScaleDepth * exp(-0.00287 + x*(0.459 + x*(3.83 + x*(-6.80 + x*5.25))));
			}
			
			float getNearIntersection(float3 pos, float3 ray, float distance, float radius) {
	  			float B = 2.0 * dot(ray ,pos );
			 	float C = distance - radius;
			  	float det = max(0.0, B*B - 4.0 * C);
			  	return 0.5 * (-B - sqrt(det));
			}			
			uniform float4 _MainTex_ST;
			uniform float4 _ColorRamp_ST;
			
			fragIO vert(appdata_base v)	
			{
			
		
				fragIO OUTPUT;

				UNITY_SETUP_INSTANCE_ID(v); //v1.7.8 //VR1 UNITY_VERTEX_INPUT_INSTANCE_ID //v1.7.8 //VR1
				UNITY_INITIALIZE_OUTPUT(fragIO, OUTPUT); //Insert //VR1  UNITY_VERTEX_OUTPUT_STEREO //VR1
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUTPUT); //Insert //VR1
			
				float3 v3Pos = v.vertex.xyz;// - (_WorldSpaceCameraPos - v3CameraPos)/480000;
				
				
				float3 v3Ray = v3Pos;// + (_WorldSpaceCameraPos - v3CameraPos)/50000 ;						
		
				float fNear = getNearIntersection(v3Ray, v3Pos, fCameraHeight2, fOuterRadius2);					
																					
				float fFar = length(v3Ray);
				v3Ray /= fFar;	

				float3 v3Start =((v3Ray - 0) * (0+10.1))+ ( - v3CameraPos) -float3(0,-Horizon_adj,0) ; //v3.4.2 - removed _WorldSpaceCameraPos	in editor
				
				fCameraHeight = length(v3CameraPos);		//v3.4.2 - removed _WorldSpaceCameraPos	in editor		
				
				float3 AA =  normalize(mul((float3x3)unity_ObjectToWorld, v.vertex.xyz));	
				OUTPUT.normalAndSunExp.xyz = AA;
				OUTPUT.normalAndSunExp.w = (6 > 0)? (256.0/6) : 0.0;
			//OUTPUT.normalAndSunExp.w = 0;
						
//				if(AA.y <=-0.07){
//					fScaleOverScaleDepth = - fScaleOverScaleDepth;
//				}										
				float fInvScaleDepth  = -(fScaleOverScaleDepth * (fInnerRadius - fCameraHeight));
				
				float fStartDepth = exp(-fInvScaleDepth);
					float fStartAngle = dot(v3Ray, v3Start) / fOuterRadius; 
					float fStartOffset = fStartDepth *scale(fStartAngle);			
		
				float fSampleLength = fFar / fSamples;
				float fScaledLength = fSampleLength * fScale;
				float3 v3SampleRay = v3Ray * fSampleLength;
				float3 v3SamplePoint = v3Start + v3SampleRay * 0.5;
				
				float3 v3FrontColor = float3(0.0, 0.0, 0.0);
				
				nSamples = 2;				
				
				
				for(int i=0; i<nSamples; i++)
				{
					float fHeight = length(v3SamplePoint);
					float fDepth = exp(fScaleOverScaleDepth * (fInnerRadius - fHeight));
					float fLightAngle = dot(v3LightDir, v3SamplePoint) / fHeight;
					float fCameraAngle = dot(v3Ray, v3SamplePoint) / fHeight;
					float fScatter = (fStartOffset + fDepth*(scale(fLightAngle) - scale(fCameraAngle)))*1;
					float3 v3Attenuate = exp(-fScatter * (v3InvWavelength * fKr4PI + fKm4PI));
					v3FrontColor += v3Attenuate * (fDepth * fScaledLength);
					v3SamplePoint += v3SampleRay;
				}			
					
				//UNITY_INITIALIZE_OUTPUT(fragIO,OUTPUT);
			
    			
    	//		OUTPUT.pos = mul(UNITY_MATRIX_MVP, v.vertex);
    			OUTPUT.pos = UnityObjectToClipPos(v.vertex);
    			
    			//v.2.0.1
    			OUTPUT.pos.y += _Obliqueness.z * OUTPUT.pos.y;
    			OUTPUT.pos.x += _Obliqueness.x * OUTPUT.pos.x;
    			    			    			
    			OUTPUT.uv = v.texcoord.xy; 	    			
    			   						
    									
				OUTPUT.c0 = (v3FrontColor) * (v3InvWavelength * fKrESun  ); //* float4(distance(v3LightDir,v3Pos),0,0,1)
				OUTPUT.c1 = (v3FrontColor) * fKmESun;				
				//OUTPUT.v3Direction = (_WorldSpaceCameraPos - v3CameraPos) - v3Pos;
				
				OUTPUT.v3Direction = v.vertex.xyz;
							

				//v0.1
				OUTPUT.v3DirectionA = mul(unity_ObjectToWorld, v.vertex).xyz - _WorldSpaceCameraPos;

				
    			return OUTPUT;
    			
    			//NOTES - Play with Fexposure or G, to get an eclipse !!!!!!!!!!!
    			
			}
			
		 sampler2D _MainTex;
         sampler2D _ColorRamp;
         
			
			//  Mie 
			float getMiePhase(float fCos, float fCos2, float g, float g2)
			{
				return 1.5 * ((1.0 - g2) / (2.0 + g2)) * (1.0 + fCos2) / pow(1.0 + g2 - 2.0*g*fCos, 1.5);
			}

			//  Rayleigh
			float getRayleighPhase(float fCos2)
			{
				return 0.75 + 0.75*fCos2;
			}	




			//v0.1
			//https://docs.unity3d.com/Packages/com.unity.shadergraph@6.9/manual/Rotate-About-Axis-Node.html
			void RotateAboutAxis_Degrees_float(float3 In, float3 Axis, float Rotation, out float3 Out)
			{
				Rotation = radians(Rotation);
				float s = sin(Rotation);
				float c = cos(Rotation);
				float one_minus_c = 1.0 - c;

				Axis = normalize(Axis);
				float3x3 rot_mat =
				{ one_minus_c * Axis.x * Axis.x + c, one_minus_c * Axis.x * Axis.y - Axis.z * s, one_minus_c * Axis.z * Axis.x + Axis.y * s,
					one_minus_c * Axis.x * Axis.y + Axis.z * s, one_minus_c * Axis.y * Axis.y + c, one_minus_c * Axis.y * Axis.z - Axis.x * s,
					one_minus_c * Axis.z * Axis.x - Axis.y * s, one_minus_c * Axis.y * Axis.z + Axis.x * s, one_minus_c * Axis.z * Axis.z + c
				};
				Out = mul(rot_mat, In);
			}

										
#define PI 3.141592653589793								
	fixed4 frag(fragIO IN) : COLOR
    {        
               
      float3 NEW_D = normalize(IN.v3Direction);
      
                    float4 result;
                   
					result = tex2D(_MainTex, _MainTex_ST.xy *  dither(IN.uv));// IN.uv);//DITHER
                    ///////////
               
                float fCos = dot(-v3LightDir, NEW_D);
				float fCos2 = fCos*fCos;				  
				
		float3 Scolor = (getRayleighPhase(fCos2)+(( 1.5 * ((1.0 - g2) / (2.0 + g2)) * (1.0 + fCos2) / pow(1.0 + g2 - 2.0*g*fCos, 1.5) ))) * IN.c0  + getMiePhase(fCos, fCos2, g, g2) * IN.c1;				 
				 
				Scolor = 1.0 - exp((- fExposure/0.01) * (Scolor));				

                float4 Out_Color = (float4(Scolor,1.0)+(_Coloration*result))+_TintColor;  
                
                  if(IN.normalAndSunExp.y < HorizonY){
                  	Out_Color = GroundColor;
                  } 
                   
                    result = tex2D(_ColorRamp, _ColorRamp_ST.xy *dither(IN.uv));
                 
                    //v3.0
                    if(Out_Color.r  >= White_cutoff  ){ 
                     if(Out_Color.g  >= White_cutoff ){
                      	if(Out_Color.b  >= White_cutoff){

                      		float A =  distance(v3LightDir,IN.v3Direction) * SunBlend;//26;
                      
                      		Out_Color = float4(lerp(Out_Color.r,SunColor.r,2.2-A),lerp(Out_Color.g,SunColor.g,2.25-A)+result.g/3,lerp(Out_Color.b,SunColor.b,2.25-A)+result.g/1,1);
                     		
                     	}
                      }
                    } 
                       


					//v0.1
					//float2 TexC = float2(IN.uv.x, IN.uv.y);
					//float2 offset = _Time.y * _CloudSpeed.xy;     
					//float4 tex = tex2D(_MainTexGalaxy, TexC * _CloudDensity);
					//fixed Bumped_lightingN = 1;
					//fixed4 tex2N = tex2D(_Jitter, (TexC + offset / 15) * _CloudDensity * 11);
					//tex = max(tex - (1 - _CloudCover * 2), 0);
					//float4 res = _Color.a * lerp(pow(tex2N, 2), 0.6, _CloudSharpness) * float4 (0.95 * _Color.r * Bumped_lightingN * tex.r, 0.95 * _Color.g * Bumped_lightingN * tex.g, 0.95 * _Color.b * Bumped_lightingN * tex.b, _Color.a);
					//float4 galaxy = pow(abs(res), _Light);

					float3 pos = normalize(IN.v3DirectionA);

					//v0.1a
					RotateAboutAxis_Degrees_float(pos, float3(1, 0, 0), _galaxySpeedVer * 0.001 * _Time.y,  pos);

					float2 newUV;
					newUV.x = 0.5 + atan2(pos.z, pos.x) / (PI * 2);
					newUV.y = 0.5 - asin(pos.y) / PI;
					float2 dx = ddx(newUV);
					float2 dy = ddy(newUV);
					float2 du = float2(dx.x, dy.x);
					du -= (abs(du) > 0.5f) * sign(du);
					dx.x = du.x;
					dy.x = du.y ;
					newUV.x += _MainTexGalaxy_ST.z;
					//float4  galaxy = tex2Dgrad(_MainTexGalaxy, newUV, dx, dy);
					float2 offset = _Time.y * _CloudSpeed.xy;
					float4  tex = tex2Dgrad(_MainTexGalaxy, newUV* _MainTexGalaxy_ST.xy + float2(_galaxySpeedHor * 0.001, 0) * _Time.y, dx, dy);
					float Bumped_lightingN = 1;
					float4 tex2N = tex2D(_Jitter, (newUV + offset / 15) * _CloudDensity * 11);
					tex = max(tex - (1 - _CloudCover * 2), 0);
					float4 res = _Color.a * lerp(pow(tex2N, 2), 0.6, _CloudSharpness) 
						* float4 (0.95 * _Color.r * Bumped_lightingN * tex.r, 
							0.95 * _Color.g * Bumped_lightingN * tex.g, 
							0.95 * _Color.b * Bumped_lightingN * tex.b, 
							_Color.a);
					float4 galaxy = pow(abs(res), _Light);
					 
					return float4(dither(IN.uv) + Out_Color.rgb, 1) + galaxy;//


					//return float4(dither(IN.uv)+ Out_Color.rgb,1);

                 //return Out_Color;//+2*_Coloration*result1;                

                }
			
			ENDCG
    	}
	}
}