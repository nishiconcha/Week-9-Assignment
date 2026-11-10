using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameUI : MonoBehaviour
{
    public PlayerController player;
    public Slider hpSlider;
    public TMP_Text hpText;
    public GameObject gameOverPanel;

    private void Start()
    {
        hpSlider.maxValue = player.maxHP;
        hpSlider.value = player.currentHP;

        gameOverPanel.SetActive(false);
    }

    private void Update()
    {
        hpSlider.value = player.currentHP;

        hpText.text = Mathf.CeilToInt(player.currentHP) + " / " + player.maxHP + " HP";

        if (player.IsDead && !gameOverPanel.activeSelf)
        {
            gameOverPanel.SetActive(true);
        }

        if (player.IsDead && Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}