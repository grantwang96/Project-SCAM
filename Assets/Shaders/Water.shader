Shader "Custom/Water" {
	Properties {
		_MainTex ("Tile", 2D) = "white" {}
		_WaveHeight ("Wave Height", float) = 0.1
		_Color ("Color", Color) = (1,1,1,1)
		_Color2 ("Wave Crest", Color) = (1,1,1,1)
		_Refract ("Index of Refraction", float) = 1
		_Bump ("Normal Map", 2D) = "bump" {}
	}
	SubShader {

		Tags {
			"Queue"="Transparent"
			"RenderType"="Transparent"
		}

		ZWrite Off
		Blend OneMinusDstColor OneMinusSrcAlpha

		CGPROGRAM

		#pragma surface surf Lambert vertex:vert
		#pragma target 3.0

		sampler2D _MainTex;
		sampler2D _Bump;
		half _WaveHeight;
		fixed4 _Color;
		fixed4 _Color2;
		half _Refract;

		struct Input {
			float2 uv_MainTex;
			float2 uv_Bump;
			float3 pos;
//			float3 initPos;
		};

		void vert(inout appdata_full v, out Input o) {
			UNITY_INITIALIZE_OUTPUT(Input, o);
//			o.initPos = v.vertex;
			v.vertex.y += (_WaveHeight * sin(v.vertex.x * _Time.z * 5) + _WaveHeight * cos(v.vertex.z * _Time.z * 5))/2;

			o.pos = v.vertex.xyz;
		}

		void surf(Input IN, inout SurfaceOutput o) {
			fixed2 scroll = fixed2(_Time.x, cos(_Time.x)) * 5;
			o.Normal = UnpackNormal(tex2D(_Bump, IN.uv_Bump + scroll));
//			float perturb = IN.pos.y +.25;
			float3 tex = tex2D(_MainTex, IN.uv_MainTex + scroll).rgb ;
			o.Albedo = tex * _Color;
//			o.Albedo = dot( tex, float3(1,1,1)) > 0.75 ? (1,1,1) : tex;
//			* _Color.rgb + lerp(_Color,_Color2, perturb*perturb);
//			o.Albedo += (IN.pos - IN.initPos) * _Color.rgb;
		}

		ENDCG
	}

	FallBack "Diffuse"
}



