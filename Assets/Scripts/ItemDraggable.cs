using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Image imageToDrag;

    private GameObject dragGhost;
    private CanvasGroup canvasGroup;

    public Sprite imageSprite => imageToDrag.sprite;

    private ItemsData data; // 🔽 Ajout de cette ligne pour stocker les données de l’item

    private void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>() ?? gameObject.AddComponent<CanvasGroup>();
        data = GetComponent<ItemsData>(); // 🔽 On récupère le composant ItemsData

        if (data == null)
            Debug.LogWarning("Aucun ItemsData assigné sur ce DraggableItem !");
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (data == null)
        {
            Debug.LogWarning("Impossible de drag : pas de ItemsData !");
            return;
        }

        // Vérifie si on a la ressource avant de permettre le drag
        if (InventoryManager.instance.Get(data.itemType.ToString()) <= 0)
        {
            Debug.Log("Drag interdit : quantité de " + data.itemType + " = 0");
            eventData.pointerDrag = null;
            return;
        }

        canvasGroup.blocksRaycasts = false;

        dragGhost = new GameObject("DragGhost");
        dragGhost.transform.SetParent(transform.root, false);

        Image ghostImage = dragGhost.AddComponent<Image>();
        ghostImage.sprite = imageToDrag.sprite;
        ghostImage.preserveAspect = true;

        RectTransform rt = ghostImage.rectTransform;
        rt.sizeDelta = new Vector2(80, 80);
        rt.position = imageToDrag.transform.position;

        dragGhost.AddComponent<CanvasGroup>().blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (dragGhost != null)
            dragGhost.transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;

        if (dragGhost != null)
            Destroy(dragGhost);

        if (data == null)
            return;

        if (InventoryManager.instance.Get(data.itemType.ToString()) <= 0)
            return;

        if (eventData.pointerEnter != null &&
            eventData.pointerEnter.TryGetComponent(out DropZone dropZone))
        {
            dropZone.TryDrop(this);
        }
    }

    // 🔽 Ajout d’un getter public si besoin
    public ItemType GetItemType() => data.itemType;
}
