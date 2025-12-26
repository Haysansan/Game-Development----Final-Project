using UnityEngine;
using TMPro;

public class AlwaysAnimateTextColor : MonoBehaviour
{
    public Color color1 = Color.white;
    public Color color2 = Color.cyan;
    public float speed = 2f;

    private TextMeshProUGUI text;

    void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        float t = Mathf.PingPong(Time.time * speed, 1f);
        text.color = Color.Lerp(color1, color2, t);
    }
}
