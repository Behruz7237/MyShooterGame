using UnityEngine;
using TMPro;

[RequireComponent(typeof(TMP_Text))]
public class HeavyMetalTextMaker : MonoBehaviour
{
    void Start()
    {
        MakeItHeavyMetal();
    }

    public void MakeItHeavyMetal()
    {
        TMP_Text textComponent = GetComponent<TMP_Text>();
        if (textComponent == null) return;

        // 1. GENERATE A SHARPER, MORE AGGRESSIVE CHROME TEXTURE
        Texture2D metalTex = new Texture2D(256, 256, TextureFormat.RGBA32, false);
        for (int x = 0; x < 256; x++)
        {
            // PingPong creates sharp zig-zags instead of smooth curves!
            float wave = Mathf.PingPong(x * 4f, 30f) + Mathf.PingPong(x * 1.5f, 15f);
            float jaggedEdge = 110 + wave;

            for (int y = 0; y < 256; y++)
            {
                Color pixelColor;
                if (y > jaggedEdge + 8)
                {
                    // Top half: Vibrant Orange to Bright Yellow
                    float t = (y - 128f) / 128f;
                    pixelColor = Color.Lerp(new Color(1f, 0.4f, 0f), new Color(1f, 0.9f, 0.2f), t);
                }
                else if (y > jaggedEdge)
                {
                    // The blinding white/chrome reflection line
                    pixelColor = Color.white;
                }
                else
                {
                    // Bottom half: Pitch Black to Crimson Red
                    float t = y / 128f;
                    pixelColor = Color.Lerp(new Color(0.1f, 0f, 0f), new Color(0.8f, 0f, 0f), t);
                }
                metalTex.SetPixel(x, y, pixelColor);
            }
        }
        metalTex.Apply();

        // 2. CLONE THE MATERIAL
        Material metalMat = Instantiate(textComponent.fontMaterial);

        // 3. APPLY THE TEXTURE
        metalMat.SetTexture("_FaceTex", metalTex);
        metalMat.SetColor("_FaceColor", Color.white);

        // 4. ADD THICK SILVER OUTLINE
        metalMat.EnableKeyword("OUTLINE_ON");
        metalMat.SetFloat("_OutlineWidth", 0.25f);
        metalMat.SetColor("_OutlineColor", new Color(0.8f, 0.8f, 0.85f));

        // 5. TURN ON 3D BEVEL & LIGHTING (Makes it shiny!)
        metalMat.EnableKeyword("BEVEL_ON");
        metalMat.SetFloat("_Bevel", 0.5f);
        metalMat.SetFloat("_BevelOffset", 0.1f);
        metalMat.SetFloat("_BevelWidth", 0.15f);

        // Specular settings make the silver edges catch the light and shine
        metalMat.SetFloat("_SpecularPower", 2f);
        metalMat.SetFloat("_Reflectivity", 10f);
        metalMat.SetColor("_SpecularColor", Color.white);

        // 6. ADD A FIERY GLOW AROUND THE LETTERS
        metalMat.EnableKeyword("GLOW_ON");
        metalMat.SetColor("_GlowColor", new Color(1f, 0.2f, 0f, 0.5f)); // Fiery orange/red
        metalMat.SetFloat("_GlowOuter", 0.8f);
        metalMat.SetFloat("_GlowInner", 0.1f);

        // 7. HEAVY 3D DROP SHADOW
        metalMat.EnableKeyword("UNDERLAY_ON");
        metalMat.SetFloat("_UnderlayOffsetX", 0.8f);
        metalMat.SetFloat("_UnderlayOffsetY", -0.8f);
        metalMat.SetFloat("_UnderlayDilate", 0.2f);
        metalMat.SetFloat("_UnderlaySoftness", 0.05f); // Slight blur for realism
        metalMat.SetColor("_UnderlayColor", new Color(0f, 0f, 0f, 1f));

        // 8. APPLY IT ALL
        textComponent.fontMaterial = metalMat;
        textComponent.fontStyle = FontStyles.Bold | FontStyles.Italic;
    }
}