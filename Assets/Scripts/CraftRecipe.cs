using UnityEngine;

[System.Serializable]
public class CraftRecipe
{
    public ItemType resultItem;
    public Sprite resultSprite;
    public int resultAmount = 1;

    public ItemType[] patterns; // 0=HG, 1=HD, 2=BG, 3=BD
}