Shader "Custom/rectify" {
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
      	//revert x coord 
      	float2 uv = float2(1.0-i.uv.x, i.uv.y);
       	float2 coord = tex2D (_Texture2, uv);
       	//why??? a 0.5 offset????
       	//float2 offset = float2(0.5/960.0, 0.5/1080.0);
		//coord.rg.x =  1.0 - coord.rg.x;
       	half4 c = tex2D(_MainTex, coord);
       	//half4 c = half4(0.0,1.0,0.0,1.0);
        return c;

      }
    ENDCG
    }


    //pencil effects shader
   




  }
  FallBack "Diffuse"
}



















//Shader "Custom/rectify" {
//  Properties {
//    _MainTex ("Base (RGB)", 2D) = "white" {}
//    _Texture2 ("Texture 2 (RGFLOAT)", 2D) =  "white"{}
//
//
//    	_MainTex ("Base (RGB)", 2D) = "white" {}
//		_PencilTex0("Pencil Texture0",2D) = "white" {}
//		_PencilTex1("Pencil Texture1",2D) = "white" {}
//		_PencilTex2("Pencil Texture2",2D) = "white" {}
//		_PencilTex3("Pencil Texture3",2D) = "white" {}
//		_PencilTex4("Pencil Texture4",2D) = "white" {}
//		_PencilTex5("Pencil Texture5",2D) = "white" {}
//		_PaperTex("Paper Texture",2D) = "white" {}
//		_TileFactor ("Tile Factor", Float) = 1
//
//  }
//
//
//
//
//  SubShader {
//    Pass{
//      CGPROGRAM
//      #pragma vertex vert
//      #pragma fragment frag
//      #include "UnityCG.cginc"
//
//      uniform sampler2D _MainTex;
//      uniform sampler2D _Texture2;
//
//      struct v2f {
//        float4  pos : SV_POSITION;
//        float2  uv : TEXCOORD0;
//      };
//
//      float4 _MainTex_ST;
//
//      v2f vert (appdata_base v) {
//        v2f o;
//        o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
//        o.uv = v.texcoord.xy;
//        return o;
//      }
//
//      half4 frag(v2f i) : COLOR {
//      	//coordinate encoded in texture pixel
//       	float2 coord = tex2D (_Texture2, i.uv);
//       	//why??? a 0.5 offset????
//       	//float2 offset = float2(0.5/960.0, 0.5/1080.0);
//		coord.rg.x =  1.0 - coord.rg.x;
//       	half4 c = tex2D(_MainTex, coord);
//       	//half4 c = half4(0.0,1.0,0.0,1.0);
//        return c;
//
//      }
//    ENDCG
//    }
//
//
//
//    Pass {  
//            CGPROGRAM  
//            #pragma exclude_renderers gles
//            #pragma vertex vert 
//            #pragma fragment frag  
//            #include "UnityCG.cginc" 
//            #pragma target 3.0 
//            uniform sampler2D _MainTex;
//            uniform sampler2D _PencilTex0;
//            uniform sampler2D _PencilTex1;
//            uniform sampler2D _PencilTex2;
//            uniform sampler2D _PencilTex3;
//            uniform sampler2D _PencilTex4;
//            uniform sampler2D _PencilTex5;
//            uniform sampler2D _PaperTex;
//            fixed _TileFactor;
//            fixed4 _MainTex_ST; 
//            half4 _MainTex_TexelSize;
//            //half _GamaAmount;
//            struct v2f 
//            {
//		        float4 pos : POSITION;
//		        half2 uv : TEXCOORD0;
//		        half2 MapOffset : TEXCOORD1; 
//	        };
//	        v2f vert( appdata_img v ) 
//	        {
//		       v2f o; 
//		       o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
//		       o.uv = v.texcoord;
//		       o.MapOffset= o.uv + half2(1.00,1.00)*_MainTex_TexelSize.xy;
//		       return o;
//	        }
//            fixed4 frag(v2f i) : COLOR  
//            {  
//                
//                fixed3 c = tex2D(_MainTex,i.uv);
//                fixed4 Paper =tex2D(_PaperTex,i.uv);
//                fixed grey0 = Luminance(c);
//                //InnerHSV
////                fixed MaxColor = max(c.r,c.g);
////                MaxColor = max(MaxColor,c.b);
////                fixed MinColor = min(c.r,c.g);
////                MinColor = min(MinColor,c.b);
////                fixed ColorDiff = MaxColor - MinColor;
////                fixed ColorSum = MaxColor + MinColor;
////                fixed L = sign(grey0 - 0.5);
////                fixed s =  lerp(ColorDiff / ColorSum  , ColorDiff / ( 2 - ColorSum ),L);
////                fixed percent0 = s - floor(s);
////                fixed percent1  = 1 - percent1;
////                fixed Over4 = step(4,s);
////                fixed Over3 = step(3,s) * step(s,4);
////                fixed Over2 = step(2,s) * step(s,3);
////                fixed Over1 = step(1,s) * step(s,2);
////                fixed Over0 = s * step(s,1);
////                fixed4 hatchTex0 = tex2D(_PencilTex0, i.uv * _TileFactor) * Over4 * percent0;
////				fixed4 hatchTex1 = tex2D(_PencilTex1, i.uv * _TileFactor) * Over3 * percent0 + Over4 * percent1;
////				fixed4 hatchTex2 = tex2D(_PencilTex2, i.uv * _TileFactor) * Over2 * percent0 + Over3 * percent1;
////				fixed4 hatchTex3 = tex2D(_PencilTex3, i.uv * _TileFactor) * Over1 * percent0 + Over2 * percent1;
////				fixed4 hatchTex4 = tex2D(_PencilTex4, i.uv * _TileFactor) * Over0 * percent0 + Over1 * percent1;
////				fixed4 hatchTex5 = tex2D(_PencilTex5, i.uv * _TileFactor) * Over0 * percent1;
////                PencilTex = hatchTex0 + hatchTex1 + hatchTex2 + hatchTex3 + hatchTex4 + hatchTex5;
//                //InnerLum
//                fixed LastPercent = 1.0;
//                fixed Hatch0Percent = saturate(( grey0 - 0.8 ) / 0.2);
//                LastPercent -= Hatch0Percent;
//                fixed Hatch1Percent = (1-saturate(abs( grey0 - 0.8 )/ 0.2)) * max( LastPercent,0 );
//                LastPercent -= Hatch1Percent;
//                fixed Hatch2Percent = (1-saturate(abs( grey0 - 0.6 )/ 0.2)) * max( LastPercent,0 );
//                LastPercent -= Hatch2Percent;
//                fixed Hatch3Percent = (1-saturate(abs( grey0 - 0.4 )/ 0.2)) * max( LastPercent,0 );
//                LastPercent -= Hatch3Percent;
//                fixed Hatch4Percent = (1-saturate(abs( grey0 - 0.2 )/ 0.2)) * max( LastPercent,0 );
//                LastPercent -= Hatch4Percent;
//                fixed Hatch5Percent = (1-saturate(abs( grey0 - 0.0 )/ 0.2)) * max( LastPercent,0 );
//                LastPercent -= Hatch5Percent;
//                fixed4 hatchTex0 = tex2D(_PencilTex0, i.uv * _TileFactor) ;
//				fixed4 hatchTex1 = tex2D(_PencilTex1, i.uv * _TileFactor) ;
//				fixed4 hatchTex2 = tex2D(_PencilTex2, i.uv * _TileFactor) ;
//				fixed4 hatchTex3 = tex2D(_PencilTex3, i.uv * _TileFactor) ;
//				fixed4 hatchTex4 = tex2D(_PencilTex4, i.uv * _TileFactor) ;
//				fixed4 hatchTex5 = tex2D(_PencilTex5, i.uv * _TileFactor) ;
//                
//                
//                //Line
//                fixed3 cOffset = tex2D(_MainTex,i.MapOffset);
//                fixed3 RGBDiff = abs(cOffset-c);
//                fixed greys1 = Luminance(RGBDiff);
//                greys1 = min(greys1,1);
//                fixed4 FinalColor = fixed4((1-greys1).xxx,1);
//                FinalColor *= FinalColor;
//                FinalColor *= FinalColor;
//                FinalColor *= FinalColor;
//                FinalColor *= FinalColor;
//
//                return FinalColor;
//                fixed4 PencilColor = hatchTex0 * Hatch0Percent + hatchTex1 * Hatch1Percent + hatchTex2 * Hatch2Percent + hatchTex3 * Hatch3Percent + hatchTex4 * Hatch4Percent + hatchTex5 * Hatch5Percent;
//                return FinalColor * Paper * PencilColor;
//            }  
//              
//            ENDCG  
//        }  
//
//
//
//
//  }
//  FallBack "Diffuse"
//}
