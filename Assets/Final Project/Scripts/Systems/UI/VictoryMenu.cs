using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class VictoryMenu : MonoBehaviour
{
    [Header("Victory Menu")]
    public GameObject victoryMenu;

    [Header("Button Colors")]
    public Color normalButtonColor = new Color(1, 1, 1, 0);
    public Color hoverButtonColor = Color.white;
    public Color normalTextColor = Color.white;
    public Color hoverTextColor = Color.black;

    [Header("Text Scale")]
    public float normalScale = 1f;
    public float hoverScale = 1.15f;
    public float scaleSpeed = 12f;

    void Start()
    {
        victoryMenu.SetActive(true);

        Button[] buttons = victoryMenu.GetComponentsInChildren<Button>(true);
        foreach (Button btn in buttons)
        {
            SetupButton(btn);
        }
    }

    void SetupButton(Button btn)
    {
        Image btnImage = btn.GetComponent<Image>();
        TextMeshProUGUI btnText = btn.GetComponentInChildren<TextMeshProUGUI>();
        RectTransform textRect = btnText.GetComponent<RectTransform>();

        btnImage.color = normalButtonColor;
        btnText.color = normalTextColor;
        textRect.localScale = Vector3.one * normalScale;

        EventTrigger trigger = btn.GetComponent<EventTrigger>();
        if (trigger == null)
            trigger = btn.gameObject.AddComponent<EventTrigger>();

        trigger.triggers.Clear();

        EventTrigger.Entry enter = new EventTrigger.Entry
        {
            eventID = EventTriggerType.PointerEnter
        };
        enter.callback.AddListener(_ =>
        {
            btnImage.color = hoverButtonColor;
            btnText.color = hoverTextColor;
            StopAllCoroutines();
            StartCoroutine(ScaleText(textRect, hoverScale));
        });

        EventTrigger.Entry exit = new EventTrigger.Entry
        {
            eventID = EventTriggerType.PointerExit
        };
        exit.callback.AddListener(_ =>
        {
            btnImage.color = normalButtonColor;
            btnText.color = normalTextColor;
            StopAllCoroutines();
            StartCoroutine(ScaleText(textRect, normalScale));
        });

        trigger.triggers.Add(enter);
        trigger.triggers.Add(exit);
    }

    IEnumerator ScaleText(RectTransform target, float targetScale)
    {
        while (Mathf.Abs(target.localScale.x - targetScale) > 0.01f)
        {
            float scale = Mathf.Lerp(
                target.localScale.x,
                targetScale,
                Time.unscaledDeltaTime * scaleSpeed
            );

            target.localScale = Vector3.one * scale;
            yield return null;
        }

        target.localScale = Vector3.one * targetScale;
    }

    // -------- BUTTON ACTIONS --------

    public void ContinueToCredits()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Credits");
    }

    public void OpenStatistics()
    {
        // Open stats panel or load stats scene
        SceneManager.LoadScene("Statistics");
    }

    public void RewindBossFight()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("BossFight");
    }

    public void MainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}
