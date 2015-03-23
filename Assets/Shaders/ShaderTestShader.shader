Shader "Custom/SolidColor" {
	Properties {
		_BlurLevel ("Blur Level", Range (0.0, 1.0)) = 1.0
		_BlurredTexture ("Blurred Texture", 2D) = "white" {}
		_OriginalTexture ("Original Texture", 2D) = "white" {}
	}

   SubShader {
        Pass {
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag

            #include "UnityCG.cginc"
            
            uniform sampler2D _BlurredTexture;
			uniform sampler2D _OriginalTexture;
			uniform float _BlurLevel;

            fixed4 frag(v2f_img i) : SV_Target {
                float4 blurred = tex2D(_BlurredTexture, i.uv);
				float4 original = tex2D(_OriginalTexture, i.uv);
				return lerp(original, blurred, _BlurLevel);
            }
            ENDCG
        }
    }
}