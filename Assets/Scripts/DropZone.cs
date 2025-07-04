using UnityEngine;
using UnityEngine.UI;

public class DropZone : MonoBehaviour
{
    // 🔹 Nouveau : pour suivre le type de l'objet déposé
    private ItemType? currentItemType = null;

    public void TryDrop(DraggableItem item)
    {
        // Récupère le type de l'item qu'on essaie de déposer
        ItemsData itemData = item.GetComponent<ItemsData>();
        if (itemData == null)
        {
            Debug.LogWarning("L'item n'a pas de ItemsData attaché !");
            return;
        }

        string itemName = itemData.itemType.ToString();

        // Vérifie qu'on a assez de cet item dans l'inventaire
        if (InventoryManager.instance.Get(itemName) <= 0)
        {
            Debug.LogWarning($"Tu n'as pas assez de {itemName} pour déposer !");
            return;
        }

        // Crée le nouvel objet visuel
        GameObject newImageGO = new GameObject("DroppedItem", typeof(Image));
        newImageGO.transform.SetParent(transform, false);

        Image img = newImageGO.GetComponent<Image>();
        img.sprite = item.imageSprite;
        img.preserveAspect = true;

        RectTransform rt = newImageGO.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(240, 240);
        rt.localPosition = Vector3.zero;

        // Donne l'information de type de l'item déposé
        var data = newImageGO.AddComponent<ItemsData>();
        data.itemType = itemData.itemType;

        // 🔹 Nouveau : on stocke le type dans cette DropZone
        currentItemType = itemData.itemType;

        // Ajoute un bouton pour supprimer l'objet et rendre la ressource
        Button btn = newImageGO.AddComponent<Button>();
        btn.onClick.AddListener(() =>
        {
            InventoryManager.instance.Add(data.itemType, 1); // On rend 1 item
            currentItemType = null; // 🔹 Nouveau : on efface le type stocké
            Destroy(newImageGO);
        });

        // Consomme 1 item
        InventoryManager.instance.TryRemove(itemName, 1);
        Debug.Log($"Objet déposé. {itemName} restant : {InventoryManager.instance.Get(itemName)}");
    }

    // 🔹 Pour savoir si cette DropZone contient un objet
    public bool HasItem()
    {
        return currentItemType.HasValue;
    }

    // 🔹 Pour obtenir le type d'objet actuellement dans la zone
    public ItemType GetItemType()
    {
        if (currentItemType.HasValue)
            return currentItemType.Value;

        throw new System.Exception("La DropZone ne contient pas d'item !");
    }

    // 🔹 Pour vider la DropZone (visuel + données)
    public void ClearItem()
    {
        // Supprime uniquement l'objet déposé (s'il y en a un)
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        currentItemType = null;
    }
}
