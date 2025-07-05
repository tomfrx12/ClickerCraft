using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class CraftManager : MonoBehaviour
{
    public DropZone[] dropZones;           // 4 drop zones assignées dans l'Inspector
    public Image resultImage;              // Image du résultat (toujours visible)
    public List<CraftRecipe> recipes;     // Recettes assignées dans l'inspecteur

    public GameObject craftInventorySlotOakPlanks;
    public GameObject craftSavedSlotOakPlanks;
    public GameObject craftInventorySlotStick;
    public GameObject craftSavedSlotStick;

    private CraftRecipe currentDetectedRecipe = null;

    private bool unlockedOakPlanks = false;
    private bool unlockedStick = false;

    void Update()
    {
        DetectCraft();
    }

    void DetectCraft()
    {
        ItemType[] itemsInZones = new ItemType[dropZones.Length];
        for (int i = 0; i < dropZones.Length; i++)
            itemsInZones[i] = dropZones[i].HasItem() ? dropZones[i].GetItemType() : ItemType.None;

        foreach (var recipe in recipes)
        {
            // --- Flexible positions ---
            if (recipe.resultItem == ItemType.OakPlanks)
            {
                bool altPattern1 = itemsInZones[0] == ItemType.OakLog && itemsInZones[1] == ItemType.None && itemsInZones[2] == ItemType.None && itemsInZones[3] == ItemType.None;
                bool altPattern2 = itemsInZones[0] == ItemType.None && itemsInZones[1] == ItemType.OakLog && itemsInZones[2] == ItemType.None && itemsInZones[3] == ItemType.None;
                bool altPattern3 = itemsInZones[0] == ItemType.None && itemsInZones[1] == ItemType.None && itemsInZones[2] == ItemType.OakLog && itemsInZones[3] == ItemType.None;
                bool altPattern4 = itemsInZones[0] == ItemType.None && itemsInZones[1] == ItemType.None && itemsInZones[2] == ItemType.None && itemsInZones[3] == ItemType.OakLog;

                if (altPattern1 || altPattern2 || altPattern3 || altPattern4)
                {
                    resultImage.sprite = recipe.resultSprite;
                    resultImage.color = Color.white;
                    currentDetectedRecipe = recipe;
                    return;
                }
            }
            // --- Patterns alternatifs en dur pour les crafts spéciaux ---
            else if (recipe.resultItem == ItemType.Stick)
            {
                bool altPattern1 = itemsInZones[0] == ItemType.OakPlanks && itemsInZones[2] == ItemType.OakPlanks && itemsInZones[1] == ItemType.None && itemsInZones[3] == ItemType.None;
                bool altPattern2 = itemsInZones[1] == ItemType.OakPlanks && itemsInZones[3] == ItemType.OakPlanks && itemsInZones[0] == ItemType.None && itemsInZones[2] == ItemType.None;

                if (altPattern1 || altPattern2)
                {
                    resultImage.sprite = recipe.resultSprite;
                    resultImage.color = Color.white;
                    currentDetectedRecipe = recipe;
                    return;
                }
            }
            // --- Pattern strict classique ---
            else
            {
                if (recipe.patterns == null || recipe.patterns.Length != dropZones.Length)
                    continue;

                bool match = true;
                for (int i = 0; i < dropZones.Length; i++)
                {
                    if (recipe.patterns[i] != itemsInZones[i])
                    {
                        match = false;
                        break;
                    }
                }
                if (match)
                {
                    resultImage.sprite = recipe.resultSprite;
                    resultImage.color = Color.white;
                    currentDetectedRecipe = recipe;
                    return;
                }
            }
        }

        // Aucun craft valide → on nettoie
        resultImage.sprite = null;
        resultImage.color = new Color(1, 1, 1, 0);
        currentDetectedRecipe = null;
    }

    public void OnResultClicked()
    {
        if (currentDetectedRecipe == null)
            return;

        ItemType craftedType = GetItemTypeFromSprite(currentDetectedRecipe.resultSprite);
        int amount = 1;

        // Débloque à vie la zone correspondante si crafté au moins une fois
        if (craftedType == ItemType.OakPlanks)
        {
            amount = 4;
            unlockedOakPlanks = true;
        }
        else if (craftedType == ItemType.Stick)
        {
            amount = 4; // ou 1 si tu veux 1 stick par craft
            unlockedStick = true;
        }

        InventoryManager.instance.Add(craftedType, amount);

        // Vide les drop zones après craft
        foreach (var zone in dropZones)
        {
            zone.ClearItem();
        }

        // Nettoie l'image résultat
        resultImage.sprite = null;
        resultImage.color = new Color(1, 1, 1, 0);
        currentDetectedRecipe = null;

        Debug.Log("Craft effectué et ajouté : " + craftedType + " x" + amount);

        // Affiche UNIQUEMENT les zones débloquées à vie
        if (craftInventorySlotOakPlanks != null)
            craftInventorySlotOakPlanks.SetActive(unlockedOakPlanks);
        if (craftSavedSlotOakPlanks != null)
            craftSavedSlotOakPlanks.SetActive(unlockedOakPlanks);
        if (craftInventorySlotStick != null)
            craftInventorySlotStick.SetActive(unlockedStick);
        if (craftSavedSlotStick != null)
            craftSavedSlotStick.SetActive(unlockedStick);
    }

    // Fonction helper : retrouver l'ItemType à partir d'un sprite
    private ItemType GetItemTypeFromSprite(Sprite sprite)
    {
        string cleanName = CleanSpriteName(sprite.name);
        return GetItemTypeByName(cleanName);
    }

    // Exemple simple d'association nom de sprite -> ItemType
    private ItemType GetItemTypeByName(string name)
    {
        // Normalise le nom pour éviter les erreurs de correspondance
        string n = name.ToLower().Replace("_", "").Replace(" ", "");
        switch (n)
        {
            case "oaklog":
                return ItemType.OakLog;
            case "cobblestone":
                return ItemType.Cobblestone;
            case "oak":
            case "oakplank":
            case "oakplanks":
                return ItemType.OakPlanks;
            case "stick":
                return ItemType.Stick;
            default:
                Debug.LogWarning("GetItemTypeByName : nom inconnu = " + name);
                return default;
        }
    }

    private string CleanSpriteName(string spriteName)
    {
        int index = spriteName.IndexOf("_");
        if (index != -1)
            spriteName = spriteName.Substring(0, index);

        return spriteName;
    }
}
