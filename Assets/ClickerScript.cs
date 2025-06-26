using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class ClickerScript : MonoBehaviour
{
    public int score = 0;
    public int bois = 0;
    public int stick = 0;

    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI boisText;
    public TextMeshProUGUI stickText;
    public GameObject messagePopup;
    public TextMeshProUGUI messageText;

    public Button clickButton;
    public Button plancheButton;
    public Button stickButton;

    // 🔽 Ajout pour la gestion des menus de catégories
    public GameObject menuBois;
    public GameObject menuCobble;
    public Button boisMenuButton;
    public Button cobbleMenuButton;

    void Start()
    {
        UpdateScore();
        UpdateBois();
        UpdateStick();

        messagePopup.SetActive(false);
        messageText.text = "";

        clickButton.onClick.AddListener(IncrementScore);
        plancheButton.onClick.AddListener(CraftBois);
        stickButton.onClick.AddListener(CraftStick);

        boisMenuButton.onClick.AddListener(ShowBoisMenu);
        cobbleMenuButton.onClick.AddListener(ShowCobbleMenu);

        // Affiche Bois par défaut
        menuBois.SetActive(true);
        menuCobble.SetActive(false);
    }

    void IncrementScore()
    {
        score++;
        UpdateScore();
    }

    void CraftBois()
    {
        if (score >= 1)
        {
            score -= 1;
            bois += 1;
            UpdateScore();
            UpdateBois();
            ShowMessage("Tu as crafté une planche de bois", Color.green);
        }
        else
        {
            ShowMessage("Il faut au moins 1 de score", Color.red);
        }
    }

    void CraftStick()
    {
        if (bois >= 2)
        {
            bois -= 2;
            stick += 1;
            UpdateBois();
            UpdateStick();
            ShowMessage("Tu as crafté un stick", Color.green);
        }
        else
        {
            ShowMessage("Il faut au moins 2 planches de bois pour un stick", Color.red);
        }
    }

    void UpdateScore()
    {
        scoreText.text = "Score : " + score;
    }

    void UpdateBois()
    {
        boisText.text = "Bois : " + bois;
    }

    void UpdateStick()
    {
        stickText.text = "Stick : " + stick;
    }

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

    // 🔽 Fonctions pour changer de menu
    void ShowBoisMenu()
    {
        menuBois.SetActive(true);
        menuCobble.SetActive(false);
    }

    void ShowCobbleMenu()
    {
        menuBois.SetActive(false);
        menuCobble.SetActive(true);
    }
}
