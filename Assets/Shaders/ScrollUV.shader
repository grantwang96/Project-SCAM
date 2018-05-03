Shader "Custom/ScrollUV" {
	Properties {
		_mainTex ("Texture", 2D) = "white" {}
		_scrollFactor ("Scroll Factor", float) = 5
	}

	SubShader {

		CGPROGRAM

		#pragma surface surf Lambert

		sampler2D _mainTex;
		float _scrollFactor;

		struct Input {
			float2 uv_mainTex;
		};

		void surf(Input IN, inout SurfaceOutput o) {
			fixed2 scroll = fixed2(_Time.x, cos(_Time.x)) * _scrollFactor;
			o.Albedo = tex2D(_mainTex, IN.uv_mainTex + scroll);
			o.Emission = tex2D(_mainTex, IN.uv_mainTex + scroll);
		}

		ENDCG
	}
}