using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class BlurImage : MonoBehaviour
{

    [Range(0,10)]
    public int blurStrength;

    [Range(0,4)]
    public int downRes;
    public Material blur;

    private void OnRenderImage(RenderTexture src, RenderTexture dest) {

        int width = src.width >> downRes;
        int height = src.height >> downRes;

        RenderTexture rt = RenderTexture.GetTemporary(width, height);
        Graphics.Blit(src, rt);

        Graphics.Blit(src, rt, blur);
        for(int i = 0; i < blurStrength; i++) {
            RenderTexture rt2 = RenderTexture.GetTemporary(width, height);
            Graphics.Blit(rt, rt2, blur);
            RenderTexture.ReleaseTemporary(rt);
            rt = rt2;
        }
        Graphics.Blit(rt, dest);

        RenderTexture.ReleaseTemporary(rt);

    }
}
