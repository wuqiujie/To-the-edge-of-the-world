Shader "Custom/Water"
{
    Properties
    {
        _ColorA ("Color", Color) = (1,1,1,1)
        _ColorB ("ColorB", Color) = (1,1,1,1)
        _ColorChangeDistance("Color Change Distance", Float) = 1
        _FoamDistance("Foam Distance", Float) = 1
        _HeightMap ("Wave Height Map", 2D) = "white" {}
        _HeightMap2 ("Wave2 Height Map", 2D) = "white" {}
        _FoamTexture("Foam Texture", 2D) = "white" {}
        _Glossiness("Smoothness", Range(0,1)) = 0.5
        _Metallic("Metallic", Range(0,1)) = 0.0
        _WaveHeight("Wave Height", Float) = 1
        _WaveHeight2("Wave Height 2", Float) = 1
        _WorldSpaceTilingScale("World Space Tiling Scale", Float) = 1
        _WorldSpaceTilingScale2("World Space Tiling Scale 2", Float) = 1
        _FoamTextureScale("Foam Texture Scale", Float) = 1
        _FlowDirection("Wave flow direction", Vector) = (0,1,1,0)
        _FlowDirection2("Wave flow direction 2", Vector) = (0,1,1,0)
        _FlowSpeed("Wave flow speed", Float) = 1
        _FlowSpeed2("Wave flow speed 2", Float) = 1
        _FoamSpeed("Foam speed", Float) = 1
    }
    SubShader
    {
        Tags {"Queue" = "Transparent" "RenderType" = "Transparent" }
        LOD 200
        ZWrite Off

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows alpha:fade
        #pragma vertex vert

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _HeightMap;
        sampler2D _HeightMap2;
        sampler2D _FoamTexture;

        uniform float _TimeCustom;

        struct Input
        {
            float2 uv_HeightMap : TEXCOORD0;
            float2 uv_HeightMap2 : TEXCOORD1;
            float2 uv_FoamTexture : TEXCOORD1;
            float3 worldPos : TEXCOORD3;
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _ColorA;
        fixed4 _ColorB;
        float _ColorChangeDistance;
        half _WaveHeight;
        half _WaveHeight2;
        float _WorldSpaceTilingScale;
        float _WorldSpaceTilingScale2;
        float _FoamTextureScale;
        float4 _FlowDirection;
        float4 _FlowDirection2;
        float _FlowSpeed;
        float _FlowSpeed2;
        float _FoamSpeed;
        float _FoamDistance;

        float heightFunction(float2 uv, sampler2D tex, float waveHeight) {
            return (tex2Dlod(tex, float4(uv, 0, 0)).x * waveHeight);
        }

        float2 heightFunctionUV(float3 worldPos, float4 direction, float speed, float scale) {
            float2 uv = (worldPos + _TimeCustom * 3 * direction * speed).xz;
            uv /= scale;
            uv = float2(uv.x % 1, uv.y % 1);
            return uv;
        }

        float heightFunction(float3 worldPos) {
            float2 uv = heightFunctionUV(worldPos, _FlowDirection, _FlowSpeed, _WorldSpaceTilingScale);

            return heightFunction(uv, _HeightMap, _WaveHeight);
        }

        float heightNormalFunction(float2 uv, sampler2D tex, float waveHeight) {
            return (tex2Dlod(tex, float4(uv, 0, 0)).x * waveHeight);
        }

        float2 heightNormalFunctionUV(float3 worldPos, float4 direction, float speed, float scale) {
            float2 uv = (worldPos + _TimeCustom * 3 * direction * speed).xz;
            uv /= scale;
            uv = float2(uv.x % 1, uv.y % 1);
            return uv;
        }

        float heightNormalFunction(float3 worldPos) {
            float2 uv = heightFunctionUV(worldPos, _FlowDirection, _FlowSpeed, _WorldSpaceTilingScale);
            float2 uv2 = heightFunctionUV(worldPos, _FlowDirection2, _FlowSpeed2, _WorldSpaceTilingScale2);

            return heightNormalFunction(uv, _HeightMap, _WaveHeight) + heightNormalFunction(uv2, _HeightMap2, _WaveHeight2);
        }

        void vert(inout appdata_full v, out Input o) {
            UNITY_INITIALIZE_OUTPUT(Input, o);
            o.worldPos = mul(unity_ObjectToWorld, v.vertex);
            v.vertex.y += heightFunction(o.worldPos) - ((_WaveHeight + _WaveHeight2)/ 2);
             
        }

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Sample height function at adjacent points to determine normal
            float ADJACENCY_DIFFERENCE = 0.3f;
            float tangent1 = heightNormalFunction(IN.worldPos.xyz + float3(1, 0, 0) * ADJACENCY_DIFFERENCE) - heightNormalFunction(IN.worldPos.xyz - float3(1, 0, 0) * ADJACENCY_DIFFERENCE);
            float tangent2 = heightNormalFunction(IN.worldPos.xyz + float3(0, 0, 1) * ADJACENCY_DIFFERENCE) - heightNormalFunction(IN.worldPos.xyz - float3(0, 0, 1) * ADJACENCY_DIFFERENCE);
            float3 normal = cross(float3(1,tangent1,0),float3(0, tangent2,1));

            // Use distance to camera to determine color and alpha
            float distToCam = clamp(distance(_WorldSpaceCameraPos, IN.worldPos) / _ColorChangeDistance, 0, 1);//0-1
            float distToFoam = clamp(distance(_WorldSpaceCameraPos, IN.worldPos) / _FoamDistance, 0, 1);//0-1
            fixed4 c = lerp(_ColorA, _ColorB, distToCam);
            c += tex2D(_FoamTexture, heightFunctionUV(IN.worldPos, float4(0,0,1,0), _FoamSpeed, _FoamTextureScale)) * (1- distToFoam);
            o.Normal = normal;
            o.Albedo = c.rgb;
            // Metallic and smoothness come from slider variables
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = max(distToCam, 0.3f);

        }
        ENDCG
    }
    FallBack "Diffuse"
}
