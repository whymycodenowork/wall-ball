using UnityEngine;

public class TextureManager : MonoBehaviour
{
    public static Sprite[] sprites;
    private void Awake()
    {
        sprites = Resources.LoadAll<Sprite>("SpriteSheet");
        foreach (Sprite sprite in sprites)
        {
            Debug.Log($"sprite loaded: {sprite}");
        }
    }
}
