#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.U2D.Sprites;
using UnityEngine;

public class SpriteSheetMaker : EditorWindow
{
    public Texture2D[] inputTextures;
    public string outputFileName = "SpriteSheet.png";
    public bool horizontal = false; // packing direction
    public int padding = 0;
    public Vector2 pivot = new(0.5f, 0.5f);

    [MenuItem("Tools/Sprite Sheet Maker")]
    private static void Init()
    {
        SpriteSheetMaker window = (SpriteSheetMaker)EditorWindow.GetWindow(typeof(SpriteSheetMaker));
        window.titleContent = new GUIContent("Sprite Sheet Maker");
        window.Show();
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("Sprite Sheet Settings", EditorStyles.boldLabel);

        SerializedObject so = new(this);
        SerializedProperty texProp = so.FindProperty("inputTextures");
        _ = EditorGUILayout.PropertyField(texProp, true);
        _ = so.ApplyModifiedProperties();

        outputFileName = EditorGUILayout.TextField("Output File", outputFileName);
        horizontal = EditorGUILayout.Toggle("Pack Horizontal", horizontal);
        padding = EditorGUILayout.IntField("Padding", padding);
        pivot = EditorGUILayout.Vector2Field("Pivot", pivot);

        if (GUILayout.Button("Create Sprite Sheet"))
        {
            if (inputTextures == null || inputTextures.Length == 0)
            {
                Debug.LogError("No textures provided!");
                return;
            }
            CreateSpriteSheet();
        }
    }

    private void CreateSpriteSheet()
    {
        int spriteWidth = inputTextures[0].width;
        int spriteHeight = inputTextures[0].height;

        // validate all inputs are same size
        foreach (Texture2D tex in inputTextures)
        {
            if (tex == null)
            {
                continue;
            }

            if (tex.width != spriteWidth || tex.height != spriteHeight)
            {
                Debug.LogError("All textures must have the same dimensions!");
                return;
            }
        }

        int width = horizontal ? ((spriteWidth + padding) * inputTextures.Length) - padding : spriteWidth;
        int height = horizontal ? spriteHeight : ((spriteHeight + padding) * inputTextures.Length) - padding;

        Texture2D output = new(width, height, TextureFormat.RGBA32, false)
        {
            filterMode = FilterMode.Point
        };

        List<Rect> spriteRects = new();
        List<string> spriteNames = new();

        int offsetX = 0;
        int offsetY = 0;

        foreach (Texture2D tex in inputTextures)
        {
            if (tex == null)
            {
                continue;
            }

            Color[] pixels = tex.GetPixels();
            output.SetPixels(offsetX, offsetY, tex.width, tex.height, pixels);

            spriteRects.Add(new Rect(offsetX, offsetY, tex.width, tex.height));
            spriteNames.Add(tex.name);

            if (horizontal)
            {
                offsetX += tex.width + padding;
            }
            else
            {
                offsetY += tex.height + padding;
            }
        }

        output.Apply();

        // save PNG
        string path = Path.Combine(Application.dataPath, outputFileName);
        File.WriteAllBytes(path, output.EncodeToPNG());
        AssetDatabase.Refresh();

        string relativePath = "Assets/" + outputFileName;
        TextureImporter importer = AssetImporter.GetAtPath(relativePath) as TextureImporter;
        importer.textureType = TextureImporterType.Sprite;
        importer.spriteImportMode = SpriteImportMode.Multiple;
        importer.filterMode = FilterMode.Point;
        importer.mipmapEnabled = false;

        AssetDatabase.ImportAsset(relativePath, ImportAssetOptions.ForceUpdate);

        // slicing with ISpriteEditorDataProvider
        if (AssetImporter.GetAtPath(relativePath) is ISpriteEditorDataProvider dataProvider)
        {
            dataProvider.InitSpriteEditorDataProvider();
            List<SpriteRect> rects = new();

            for (int i = 0; i < spriteRects.Count; i++)
            {
                SpriteRect sr = new()
                {
                    name = spriteNames[i],
                    rect = spriteRects[i],
                    pivot = pivot,
                    alignment = (SpriteAlignment)9 // 9 = custom pivot
                };
                rects.Add(sr);
            }

            dataProvider.SetSpriteRects(rects.ToArray());
            dataProvider.Apply();
            AssetDatabase.ImportAsset(relativePath, ImportAssetOptions.ForceUpdate);
        }

        Debug.Log("Sprite sheet created and sliced: " + relativePath);
    }
}
#endif