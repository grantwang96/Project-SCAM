// Unity uses a programming language for Shaders called HLSL
// Similar, but distinct from C#

// SHADER CODE IS NOT OBJECT-ORIENTED! WELCOME TO DIE, BITCH!

Shader "Custom Shaders/MyNewShader"{
	
	Properties{ // These are like variables. These are like the options you get in Materials
		_Color("Color", Color) = (1, 1, 1, 1) // Arg1 is the name of the property, Arg2 is the type of property
		_MainTexture("Main Texture", 2D) = "white" {} // This will let you add a texture/image
		_Transparency("Transparency", Range(0.0, 0.5)) = 0.25 // Transparency...duh. Also you don't need f for floats in shader code...neat
		
		// the following properties will help produce a wave effect on the material
		_Distance("Distance", Float) = 1 // floats must be capitalized in the properties section. For some reason
		_Amplitude("Amplitude", Float) = 1
		_Speed("Speed", Float) = 1
		_Amount("Amount", Range(0.0, 1.0)) = 1
	}

	Subshader {

		Tags { // Provides strings to Unity so that it knows how to render it
			"Queue" = "Transparent" "RenderType" = "Transparent"
		}

		LOD 100 // Level Of Detail
		ZWrite Off // this makes appear on top of other objects
		Blend SrcAlpha OneMinusSrcAlpha// the method of blending

		Pass{ // where the actual code for the shader starts
			CGPROGRAM // write your code in between CGPROGRAM and ENDCG
			// Shader code consists of 2 parts

			// How to tell your gpu which is the vertex program and fragment program(you could name those functions anything)
			// Use pragma to determine vertex and fragment program
			// Used for various GPUs(Nvidia, AMD, Intel HD, etc.)
			#pragma vertex MyVertexProgram // This function is my Vertex program. Like Main
			#pragma fragment MyFragmentProgram // This function is my 

			#include "UnityCG.cginc" // some stuff you might need

			float4 _Color; // this will automatically be assigned to _Color just by its name.
			sampler2D _MainTexture; // sampler2D is similar to the sprite variable in C#
			float _Transparency; // regular float

			float _Distance;
			float _Amplitude;
			float _Speed;
			float _Amount;

			struct VertexData {
				float4 position : SV_POSITION;
				float2 uv : TEXCOORD0;
			};


			float rand(float3 co)
			{
				return frac(sin(dot(co.xyz, float3(12.9898, 78.233, 45.5432))) * 43758.5453);
			}
			/*
			// this should return float4. It's like a vector/matrix
			VertexData MyVertexProgram(float4 position : POSITION) : SV_POSITION { // SV_POSITION tells the computer to treat the float4 as a screen position
				// float4 newFloat4 = float4(1, 1, 1, 1); <--- declaring and initializing a float4
				// return position;
				return UnityObjectToClipPos(position);
			}*/

			VertexData MyVertexProgram(float4 position : POSITION, float2 uv : TEXCOORD0) { // since the vertexData already contains the SV_POSITION semantic, you don't need it here
				VertexData vertexData;

				position.x += sin(_Time.y * _Speed + position.y * _Amplitude) * _Distance * _Amount; // This will modify the x coordinate of the position passed in
				position.y += cos(_Time.y * _Speed + position.x * _Amplitude) * _Distance * _Amount;
				position.z += cos(_Time.y * _Speed + position.x * _Amplitude) * _Distance * _Amount;

				vertexData.position = UnityObjectToClipPos(position);
				vertexData.uv = uv;

				return vertexData;
			}
			
			// the float4 in this case will be used to determine color rather than position

			float4 MyFragmentProgram(VertexData vertexData) : SV_TARGET{ // also uses a semantic to determine purpose

				// OLD ITERATIONS
				// return _Color; // this will render a single color
				// return tex2D(_MainTexture, vertexData.uv) * _Color; // takes the vertex data from screen position and uv data
				
				fixed4 color = tex2D(_MainTexture, vertexData.uv) * _Color; // fixed4 is another group of 4 numbers. More restrictive float4 and more efficient for color
				color += sin(_Time.y * _Speed + (_Time.x * 0.5 + _Time.z * 0.5) * _Amplitude) * _Distance * _Amount;
				// color.a = _Transparency;
				color.a = rand(vertexData.uv.x);
				
				 return color;
			}

			// MyVertexProgram and MyFragmentProgram are called behind the scenes. Kinda like Start() or Update()

			ENDCG // Some shader code requires a semi-colon. Some don't. It's very inconsistent and weird
		}
	}
}