Shader "Hyperparticle/Waves" {

Properties {
	_PrimaryColor("Primary color", Color) = (1,1,1,1)
	_SecondaryColor("Secondary color", Color) = (1,1,1,1)
    
    [Space]
    _SecondaryColorAngle("Secondary color angle", Range(0, 5)) = 1.0
    _SecondaryColorImpact("Secondary color impact", Range(0, 2)) = 1.0
    
    [Space]
    _Alpha("Opacity", Range(0, 1)) = 0.8
    
    [Space]
    _Intensity("Intensity", Range(0, 15)) = 1
    _Speed("Speed", Range(0, 15)) = 1
    _WaveLength("Wave length", Range(0.1, 5)) = 1
    _Direction("Direction", Range(0, 6.282 /*2PI*/)) = 0
    [Toggle] _SquareWaves("Square waves", Float) = 0
    
    _WaveLength2("Wave length", Float) = 0.5
    _WaveHeight("Wave height", Float) = 0.5
    _WaveSpeed("Wave speed", Float) = 1.0
    _RandomHeight("Random height", Float) = 0.5
    _RandomSpeed("Random Speed", Float) = 0.5
}

Category {
	Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Opaque" }

	AlphaTest Off
	Blend One OneMinusSrcAlpha
	Cull Back
	Fog { Mode Off }
	Lighting Off
	ZTest LEqual
	ZWrite On
	
	SubShader {
		Pass {
			CGPROGRAM
			
            #pragma vertex vert
			#pragma fragment frag
            #pragma multi_compile_fog
            
			#include "UnityCG.cginc"

			struct appdata {
			    half4 position : POSITION;
			    half2 texcoord : TEXCOORD0;
			    half3 normal : NORMAL;
			};

			struct v2f {
			    half4 position : SV_POSITION;
			    half4 color : TEXCOORD0;
                UNITY_FOG_COORDS(1)
			};

			half4 _PrimaryColor;
			half4 _SecondaryColor;
            
            half _SecondaryColorImpact;
            half _SecondaryColorAngle;
            
            half _Alpha;

            half _Intensity;
            half _Speed;
            half _Direction;
            half _WaveLength;
            half _SquareWaves;
            
            ///
            float _WaveLength2;
			float _WaveHeight;
			float _WaveSpeed;
			float _RandomHeight;
			float _RandomSpeed;
            
            float rand(float3 co)
			{
				return frac(sin(dot(co.xyz ,float3(12.9898,78.233,45.5432))) * 43758.5453);
			}

			float rand2(float3 co)
			{
				return frac(sin(dot(co.xyz ,float3(19.9128,75.2,34.5122))) * 12765.5213);
			}
            ///
            
			v2f vert(appdata vertex) {
			    v2f output;

                // vertex.position.y += sin((_Time.y * _Speed + vertex.position.x * sin(_Direction) + vertex.position.z * cos(_Direction)) / _WaveLength) * 
                //         _Intensity * 0.05 * sin(1.57 + _SquareWaves * (vertex.position.x * sin(_Direction + 1.57) + vertex.position.z * cos(_Direction + 1.57) / _WaveLength));
                
                ///
                float3 v0 = mul(_Object2World, vertex.position).xyz;

				float phase0 = (_WaveHeight)* sin((_Time[1] * _WaveSpeed) + (v0.x * _WaveLength2) + (v0.z * _WaveLength2) + rand2(v0.xzz));
				float phase0_1 = (_RandomHeight)*sin(cos(rand(v0.xzz) * _RandomHeight * cos(_Time[1] * _RandomSpeed * sin(rand(v0.xxz)))));
				
				v0.y += phase0 + phase0_1;
                
                vertex.position.xyz = mul((float3x3)_World2Object, v0);
                ///
                
			    half3 worldPosition = mul(_Object2World, vertex.position).xyz;
			    half3 cameraVector = normalize(worldPosition.xyz - _WorldSpaceCameraPos);
			    half3 worldNormal = normalize(mul(_Object2World, half4(vertex.normal,0)).xyz);
			    half blend = dot(worldNormal, cameraVector) * _SecondaryColorAngle + _SecondaryColorImpact;

			    output.color = _PrimaryColor * (1 - blend) + _SecondaryColor * blend;
                output.color.a = _Alpha;
			    output.color.rgb *= output.color.a;
                
                output.position = mul(UNITY_MATRIX_MVP, vertex.position);
                
                UNITY_TRANSFER_FOG(output, output.position);

			    return output;
			}
			
			half4 frag(v2f fragment) : COLOR {
                UNITY_APPLY_FOG(fragment.fogCoord, fragment.color);
				return fragment.color;
			}
            
			ENDCG
		}
	}
	
}
}
