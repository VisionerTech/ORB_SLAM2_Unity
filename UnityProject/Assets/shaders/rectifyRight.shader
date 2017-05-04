Shader "Custom/rectifyRight" {
  Properties {
    _MainTex ("Base (RGB)", 2D) = "white" {}
    _Texture2 ("Texture 2 (RGFLOAT)", 2D) =  "white"{}
  }
  SubShader {
    Pass{
      CGPROGRAM
      #pragma vertex vert
      #pragma fragment frag
      #include "UnityCG.cginc"

      uniform sampler2D _MainTex;
      uniform sampler2D _Texture2;

      struct v2f {
        float4  pos : SV_POSITION;
        float2  uv : TEXCOORD0;
      };

      float4 _MainTex_ST;

      v2f vert (appdata_base v) {
        v2f o;
        o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
        o.uv = v.texcoord.xy;
        return o;
      }

      half4 frag(v2f i) : COLOR {
      	//coordinate encoded in texture pixel
      	float2 uv = float2(1.0-i.uv.x, i.uv.y);
       	float2 coord = tex2D (_Texture2, uv);
       	//why??? a 0.5 offset????
       	float2 offset = float2(0.5/960.0, 0.5/1080.0);
       	//the right texture have (0,0) in the buttom left while the left have in the top left. so it's flipped here
       	//coord.rg.x =  1.0 - coord.rg.x;
       	half4 c = tex2D(_MainTex, coord);
    	//half4 c = half4(1.0,0.0,0.0,1.0);
        return c;
      }
    ENDCG
    }
  }
  FallBack "Diffuse"
}
