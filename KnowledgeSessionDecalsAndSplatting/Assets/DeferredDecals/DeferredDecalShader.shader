// http://www.popekim.com/2012/10/siggraph-2012-screen-space-decals-in.html

Shader "Decal/DeferredDecal"
{
	Properties
	{
		_Transparency ("Transparency", Range(0.0, 1.0)) = 1.0
		_AlbedoMap ("Albedo + Alpha", 2D) = "white" {}
		_AlbedoStrength ("Albedo Strength", Range(0.0, 1.0)) = 1.0
		_NormalMap ("Normal", 2D) = "bump" {}
		_NormalStrength ("Normal Strength", Range(0.0, 1.0)) = 1.0
		_SmoothnessMap ("Smoothness Map (r)", 2D) = "white" {}
		_SmoothnessStrength ("Smoothness Strength", Range(0.0, 1.0)) = 1.0

		_AngleFadeStart ("Angle Fade Start", Range(0.0, 1.0)) = 0.5
		_AngleFadeEnd ("Angle Fade End", Range(0.0, 1.0)) = 1.0
	}
	SubShader
	{
		Pass
		{
			Fog { Mode Off }
			ZWrite Off
			Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM
			#pragma target 3.0
			#pragma vertex vert
			#pragma fragment frag
			#pragma exclude_renderers nomrt
			
			#include "UnityCG.cginc"

			struct v2f
			{
				float4 pos : SV_POSITION;
				half2 uv : TEXCOORD0;
				float4 screenUV : TEXCOORD1;
				float3 ray : TEXCOORD2;
				half3 orientation : TEXCOORD3;
				half3 orientationX : TEXCOORD4;
				half3 orientationZ : TEXCOORD5;
			};

			v2f vert (float3 v : POSITION)
			{
				v2f o;
				o.pos = UnityObjectToClipPos (float4(v,1));
				o.uv = v.xz+0.5;
				o.screenUV = ComputeScreenPos (o.pos);
				o.ray = mul (UNITY_MATRIX_MV, float4(v,1)).xyz * float3(-1,-1,1);
				o.orientation = mul ((float3x3)unity_ObjectToWorld, float3(0,1,0));
				o.orientationX = mul ((float3x3)unity_ObjectToWorld, float3(1,0,0));
				o.orientationZ = mul ((float3x3)unity_ObjectToWorld, float3(0,0,1));
				return o;
			}

			float _Transparency;
			sampler2D _AlbedoMap;
			float _AlbedoStrength;
			sampler2D _NormalMap;
			float _NormalStrength;
			sampler2D _SmoothnessMap;
			float _SmoothnessStrength;

			float _AngleFadeStart;
			float _AngleFadeEnd;

			sampler2D _GbufferNormals;

			sampler2D_float _CameraDepthTexture;


			float remap (float value, float low1, float high1, float low2, float high2) {
				return low2 + (value - low1) * (high2 - low2) / (high1 - low1);
			}

			void frag(v2f i, out half4 outDiffuse : COLOR0, out half4 outParam : COLOR1, out half4 outNormal : COLOR2)
			{
				i.ray = i.ray * (_ProjectionParams.z / i.ray.z);
				float2 uv = i.screenUV.xy / i.screenUV.w;

				// read depth and reconstruct world position
				float depth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, uv);
				depth = Linear01Depth (depth);
				float4 viewPos = float4(i.ray * depth,1);
				float3 worldPos = mul (unity_CameraToWorld, viewPos).xyz;
				float3 objectPos = mul (unity_WorldToObject, float4(worldPos,1)).xyz;

				clip (float3(0.5, 0.5, 0.5) - abs(objectPos.xyz));
				i.uv = objectPos.xz + 0.5;

				fixed4 albedo = tex2D (_AlbedoMap, i.uv);
				float smoothness = tex2D (_SmoothnessMap, i.uv).r;

				float alpha = albedo.a * _Transparency;
				
				fixed3 normal = UnpackNormal(tex2D(_NormalMap, i.uv));
				half3x3 norMat = half3x3(i.orientationX, i.orientationZ, i.orientation);
				normal = mul (normal, norMat);

				// Get angle between decal projector and surface normal
				half3 gbufferNormal = tex2D(_GbufferNormals, uv).rgb * 2.0 - 1.0;

				half3 forward = mul(half3(0, 0, 1), norMat);
				float surfaceAngle = dot(forward, gbufferNormal);
				
				float angleFade = remap (surfaceAngle, _AngleFadeStart, _AngleFadeEnd, 0.0, 1.0);
				angleFade = clamp(angleFade, 0.0, 1.0);
				alpha *= angleFade;
				
				outDiffuse = half4(albedo.rgb, alpha * _AlbedoStrength);
				outParam = half4(1.0, 1.0, 1.0, smoothness * alpha * _SmoothnessStrength);
				outNormal = fixed4(normal * 0.5 + 0.5, alpha * _NormalStrength);
			}
			ENDCG
		}		

	}

	Fallback Off
}
