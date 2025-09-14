using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class WallPixelateComposite : MonoBehaviour
{
    [Header("Wall Camera (renders only Walls layer)")]
    public Camera wallCamera;            // drag your wall-only Camera here
    public Material pixelateMaterial;    // uses Hidden/WallPixelateOutline
    public Material compositeMaterial;   // uses Hidden/WallCompositeBlend
    [Range(1, 128)]
    public int pixelSize = 16;

    private RenderTexture wallRT;

    void OnEnable()
    {
        SetupRT();
    }

    void OnDisable()
    {
        if (wallRT != null)
        {
            if (wallCamera != null) wallCamera.targetTexture = null;
            wallRT.Release();
            DestroyImmediate(wallRT);
            wallRT = null;
        }
    }

    void SetupRT()
    {
        if (wallCamera == null) return;
        if (wallRT != null) return;
        if (Screen.width <= 0 || Screen.height <= 0) return; // Prevent zero-size RT
        wallRT = new RenderTexture(Screen.width, Screen.height, 16, RenderTextureFormat.ARGB32);
        wallRT.Create();
        wallCamera.targetTexture = wallRT;
    }

    void Update()
    {
        if (Screen.width <= 0 || Screen.height <= 0) return; // Prevent zero-size RT

        if (wallRT == null || wallRT.width != Screen.width || wallRT.height != Screen.height)
        {
            if (wallRT != null) { if (wallCamera != null) wallCamera.targetTexture = null; wallRT.Release(); DestroyImmediate(wallRT); }
            if (wallCamera != null)
            {
                wallRT = new RenderTexture(Screen.width, Screen.height, 16, RenderTextureFormat.ARGB32);
                wallRT.Create();
                wallCamera.targetTexture = wallRT;
            }
        }

        if (pixelateMaterial != null) pixelateMaterial.SetFloat("_PixelSize", pixelSize);
    }

    // This runs on the main camera as a final image effect. It composites the pixelated walls over the main scene.
    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if (wallRT == null || pixelateMaterial == null || compositeMaterial == null)
        {
            Graphics.Blit(src, dest);
            return;
        }

        // pixelate wallRT into a temporary RT
        RenderTexture tmp = RenderTexture.GetTemporary(wallRT.width, wallRT.height, 0, wallRT.format);
        Graphics.Blit(wallRT, tmp, pixelateMaterial);

        // set overlay texture on composite material
        compositeMaterial.SetTexture("_Overlay", tmp);

        // composite scene (src) with overlay and output to dest
        Graphics.Blit(src, dest, compositeMaterial);
        
        RenderTexture.ReleaseTemporary(tmp);
    }
}
