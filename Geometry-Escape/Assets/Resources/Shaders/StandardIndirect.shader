Shader "Custom/StandardIndirect"
{
    Properties
    {
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows
		#pragma multi_compile_instancing
		#pragma instancing_options procedural:setup
        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;

        struct Input
        {
            float2 uv_MainTex;
        };

        half _Glossiness;
        half _Metallic;

		#ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED
		StructuredBuffer<float4x4> _LocalToWorldBuffer;
		StructuredBuffer<float4> _ColorBuffer;
		StructuredBuffer<float4> _TilingAndOffsetBuffer;
		#endif

		void setup()
		{
			#ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED
			float4x4 data = _LocalToWorldBuffer[unity_InstanceID];
			unity_ObjectToWorld = data;
			#endif
		}

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Albedo comes from a texture tinted by color
			#ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED
			float4 to = _TilingAndOffsetBuffer[unity_InstanceID];
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex * to.xy + to.zw);
			#else
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
			#endif
			#ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED
            c = c * _ColorBuffer[unity_InstanceID];
			#endif
            o.Albedo = c.rgb;
            // Metallic and smoothness come from slider variables
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
