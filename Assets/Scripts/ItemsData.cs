using UnityEngine;
using System.Collections.Generic;

public class ItemsData : MonoBehaviour
{
    public ItemType itemType;

    // Associe ici tous tes sprites à leurs ItemType dans l’inspecteur ou via code
    public Sprite itemSprite;

    // Si tu préfères une méthode statique pour récupérer ItemType par sprite, tu peux faire ça :

    private static Dictionary<Sprite, ItemType> spriteToTypeMap;

    private void Awake()
    {
        if (spriteToTypeMap == null)
        {
            spriteToTypeMap = new Dictionary<Sprite, ItemType>();
            // Ici tu peux ajouter manuellement toutes les associations
            // Exemple (remplace par tes références de sprites)
            // spriteToTypeMap.Add(oakPlanksSprite, ItemType.OakPlanks);
            // spriteToTypeMap.Add(stickSprite, ItemType.Stick);
        }
    }

    public static ItemType? GetItemTypeFromSprite(Sprite sprite)
    {
        if (sprite == null) return null;
        if (spriteToTypeMap != null && spriteToTypeMap.TryGetValue(sprite, out var type))
            return type;
        return null;
    }
}
