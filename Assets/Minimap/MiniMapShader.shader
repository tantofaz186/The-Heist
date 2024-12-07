Shader "Unlit/MiniMapShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Radius ("Radius", Range(0,1)) = 0.5
        _BorderCol ("Border Color", Color) = (0,0,1,1)
        _BorderThickness ("Border Thickness", Range(0,0.05)) = 0.01
        _AlphaControl ("AlphaControl", Range(0,1)) = 1
    }
    SubShader
    {
        Tags
        {
            "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"
        }
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float1 _Radius;
            float4 _BorderCol;
            float1 _BorderThickness;
            float1 _AlphaControl;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // sample the texture
                fixed4 transparent = 0; 
                fixed4 col = tex2D(_MainTex, i.uv);
                col.a = _AlphaControl;
                // make border color
                float2 center = 0.5 * float2(1, 1);
                float2 uv = i.uv.xy - center;
                float radius = length(uv);

                //make outside of border transparent
                col = col * (1 - ceil(radius - _Radius));

                // apply border

                if (sqrt(pow(radius - _Radius, 2)) <= _BorderThickness)
                {
                    col = _BorderCol;
                    col.a = _AlphaControl;
                }
                return col;
            }
            ENDCG
        }
    }
}