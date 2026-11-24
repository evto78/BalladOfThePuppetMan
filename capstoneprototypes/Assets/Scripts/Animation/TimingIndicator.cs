using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class TimingIndicator : MonoBehaviour
{
    Rigidbody2D rb;
    public TextMeshProUGUI feedbackText;

    public float initialYVelocity = 7.5f;
    public float lifeTime = 0.5f;
    bool still = false;

    public Color hitColor;
    public Color missColor;

    public TMP_FontAsset hitFont;
    public TMP_FontAsset missFont;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        feedbackText = GetComponentInChildren<TextMeshProUGUI>();
    }

    private void Start()
    {
        rb.velocity = new Vector2(0, initialYVelocity);
        Destroy(gameObject, lifeTime);
    }

    private void Update()
    {
        if (!still && rb.velocity.y <= 0.1f && rb.velocity.y >= -0.1f)
        {
            rb.bodyType = RigidbodyType2D.Static;
            still = true;
        }
    }

    public void SetFeedback(string feedback)
    {
        feedbackText.text = feedback;
    }
}
