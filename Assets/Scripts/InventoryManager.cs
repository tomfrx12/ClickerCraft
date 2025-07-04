using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager instance;

    private ItemType currentSelectedItem = ItemType.OakLog;

    // Déblocages
    private bool hasUnlockedOakLog = false;
    private bool hasUnlockedCobblestone = false;
    private bool hasUnlockedOakPlanks = false;   // pour planches
    private bool hasUnlockedStick = false;      // pour sticks

    // Ressources
    public int OakLog = 0;
    public int Cobblestone = 0;
    public int OakPlanks = 0;
    public int Stick = 0;

    // UI Texts
    public TextMeshProUGUI OakLogText;
    public TextMeshProUGUI CobblestoneText;
    public TextMeshProUGUI OakPlanksText;
    public TextMeshProUGUI StickText;
 
    // Messages
    public GameObject messagePopup;
    public TextMeshProUGUI messageText;

    // Boutons
    public Button clickButton;
    public Button plancheButton;
    public Button stickButton;

    // Menus & Inventaires
    public GameObject menuOakLog;
    public Button OakLogMenuButton;
    public GameObject InventoryOakLog;

    public GameObject menuStone;
    public Button StoneMenuButton;
    public GameObject InventoryCobblestone;

    public GameObject InventoryOakPlanks;

    public GameObject InventoryStick;

    void Start()
    {
        instance = this;

        // Initialisation UI
        UpdateOakLog();
        UpdateCobblestone();
        UpdateOakPlanks();
        UpdateStick();

        messagePopup.SetActive(false);
        messageText.text = "";

        // Boutons de craft
        plancheButton.onClick.AddListener(CraftOakPlanks);
        stickButton.onClick.AddListener(CraftStick);

        // Bouton de collecte
        clickButton.onClick.AddListener(Increment);
        clickButton.onClick.AddListener(ShowItems);

        // Menus
        OakLogMenuButton.onClick.AddListener(ShowOakLogMenu);
        menuOakLog.SetActive(false);
        InventoryOakLog.SetActive(false);

        StoneMenuButton.onClick.AddListener(ShowStoneMenu);
        menuStone.SetActive(false);
        InventoryCobblestone.SetActive(false);

        InventoryOakPlanks.SetActive(false);

        InventoryStick.SetActive(false);
    }

    // 🔽 Ajoute une ressource selon le type sélectionné
    void Increment()
    {
        switch (currentSelectedItem)
        {
            case ItemType.OakLog:
                OakLog++;
                UpdateOakLog();
                if (!hasUnlockedOakLog)
                {
                    hasUnlockedOakLog = true;
                    InventoryOakLog.SetActive(true);
                    InventoryOakLog.transform.SetAsFirstSibling(); // Pour l’afficher en haut
                }
                break;

            case ItemType.Cobblestone:
                Cobblestone++;
                UpdateCobblestone();
                if (!hasUnlockedCobblestone)
                {
                    hasUnlockedCobblestone = true;
                    InventoryCobblestone.SetActive(true);
                    InventoryCobblestone.transform.SetAsFirstSibling();
                }
                break;

            // Tu peux ajouter d'autres cas si besoin
        }
    }

    // 🔨 Craft : Oak Plank
    void CraftOakPlanks()
    {
        if (OakLog >= 1)
        {
            OakLog -= 1;
            OakPlanks += 4;
            UpdateOakLog();
            UpdateOakPlanks();

            if (!hasUnlockedOakPlanks)
            {
                hasUnlockedOakPlanks = true;
                InventoryOakPlanks.SetActive(true);
                InventoryOakPlanks.transform.SetAsFirstSibling(); // Affiche en haut
            }

            ShowMessage("Tu as crafté 4 planche de Oak Planks", Color.green);
        }
        else
        {
            ShowMessage("Il faut au moins 1 Oak Log", Color.red);
        }
    }

    // 🔨 Craft : Stick
    void CraftStick()
    {
        if (OakPlanks >= 2)
        {
            OakPlanks -= 2;
            Stick += 4;
            UpdateOakPlanks();
            UpdateStick();

            if (!hasUnlockedStick)
            {
                hasUnlockedStick = true;
                InventoryStick.SetActive(true);
                InventoryStick.transform.SetAsFirstSibling(); // Affiche en haut
            }

            ShowMessage("Tu as crafté 4 stick", Color.green);
        }
        else
        {
            ShowMessage("Il faut au moins 2 planches de Oak Plank pour un stick", Color.red);
        }
    }

    // 🔄 Mises à jour UI
    void UpdateOakLog() => OakLogText.text = "Oak Log : " + OakLog;
    void UpdateCobblestone() => CobblestoneText.text = "Cobblestone : " + Cobblestone;
    void UpdateOakPlanks() => OakPlanksText.text = "Oak Planks : " + OakPlanks;
    void UpdateStick() => StickText.text = "Stick : " + Stick;

    // 💬 Affichage des messages temporaires
    void ShowMessage(string msg, Color color)
    {
        StopAllCoroutines();
        StartCoroutine(ShowMessageCoroutine(msg, color));
    }

    IEnumerator ShowMessageCoroutine(string msg, Color color)
    {
        messageText.text = msg;
        messageText.color = color;
        messagePopup.SetActive(true);
        yield return new WaitForSeconds(1f);
        messagePopup.SetActive(false);
        messageText.text = "";
    }

    // 🧭 Navigation entre menus
    void ShowOakLogMenu()
    {
        menuOakLog.SetActive(true);
        menuStone.SetActive(false);
        currentSelectedItem = ItemType.OakLog;
    }

    void ShowStoneMenu()
    {
        menuStone.SetActive(true);
        menuOakLog.SetActive(false);
        currentSelectedItem = ItemType.Cobblestone;
    }

    void ShowItems()
    {
        if (hasUnlockedOakLog) InventoryOakLog.SetActive(true);
        if (hasUnlockedCobblestone) InventoryCobblestone.SetActive(true);
        if (hasUnlockedOakPlanks) InventoryOakPlanks.SetActive(true);
        if (hasUnlockedStick) InventoryStick.SetActive(true);
    }

    // 📦 Inventaire - Lecture
    public int Get(string itemName)
    {
        switch (itemName)
        {
            case "OakLog": return OakLog;
            case "Cobblestone": return Cobblestone;
            case "OakPlanks": return OakPlanks;
            case "Stick": return Stick;
            default:
                Debug.LogWarning("Get : item inconnu = " + itemName);
                return 0;
        }
    }

    // 🔄 Ajout selon ItemType
    public void Add(ItemType itemType, int amount)
    {
        switch (itemType)
        {
            case ItemType.OakLog:
                OakLog += amount;
                UpdateOakLog();
                break;
            case ItemType.Cobblestone:
                Cobblestone += amount;
                UpdateCobblestone();
                break;
            case ItemType.OakPlanks:
                OakPlanks += amount;
                UpdateOakPlanks();
                break;
            case ItemType.Stick:
                Stick += amount;
                UpdateStick();
                break;
            default:
                Debug.LogWarning("Add : itemType inconnu = " + itemType);
                break;
        }
    }

    // 📦 Inventaire - Suppression sécurisée
    public bool TryRemove(string itemName, int amount)
    {
        int current = Get(itemName);
        if (current >= amount)
        {
            switch (itemName)
            {
                case "OakLog":
                    OakLog -= amount;
                    UpdateOakLog();
                    break;
                case "Cobblestone":
                    Cobblestone -= amount;
                    UpdateCobblestone();
                    break;
                case "OakPlanks":
                    OakPlanks -= amount;
                    UpdateOakPlanks();
                    break;
                case "Stick":
                    Stick -= amount;
                    UpdateStick();
                    break;
                default:
                    Debug.LogWarning("TryRemove : item inconnu = " + itemName);
                    return false;
            }
            return true;
        }
        return false;
    }
}
