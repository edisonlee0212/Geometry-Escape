﻿Shader "Custom/TileIndirect"
{
	Properties
	{
		_MainTex("Albedo (RGB)", 2D) = "white" {}
		_EmissionColor("Emission Color", Color) = (0,0,0)
	}
	SubShader
	{
		Tags{ "RenderType" = "Opaque" }
		LOD 200

		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows
		#pragma multi_compile_instancing
		#pragma instancing_options procedural:setup
		// Use shader model 3.0 target, to get nicer looking lighting
		//#pragma target 3.0

		sampler2D _MainTex;

		struct Input
		{
			float2 uv_MainTex;
		};



		#ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED
			StructuredBuffer<float4x4> localToWorldBuffer;
			StructuredBuffer<float4> colorBuffer;
		#endif

		void setup()
		{
			#ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED
				float4x4 data = localToWorldBuffer[unity_InstanceID];
				unity_ObjectToWorld = data;
			#endif
		}

		void surf(Input IN, inout SurfaceOutputStandard o)
		{
			float4 color = 1.0f;
			#ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED
				color = colorBuffer[unity_InstanceID];
			#endif
			o.Emission = color;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
