using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class DamageIndicator : MonoBehaviour
{
    Rigidbody2D rb;
    TextMeshProUGUI damageText;
    public TimelineManager tl;

    public float initialYVelocity = 7.5f;
    public float lifeTime = 0.5f;
    public int currentDamage = 0;
    public bool untilRoundEnd = true;
    bool lifeTimerStarted = false;
    bool still = false;

    public Color dmgColor;
    public Color healColor;

    public TMP_FontAsset dmgFont;
    public TMP_FontAsset healFont;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        damageText = GetComponentInChildren<TextMeshProUGUI>();
    }

    private void Start()
    {
        rb.velocity = new Vector2(0, initialYVelocity);
        tl = GameObject.Find("Timeline").GetComponent<TimelineManager>();

        tl.onRoundEnd.AddListener(DestoryNumbers);
    }

    private void Update()
    {
        if (!untilRoundEnd && !lifeTimerStarted) { Destroy(gameObject, lifeTime); lifeTimerStarted = true; }

        if (!still && rb.velocity.y <= 0.1f && rb.velocity.y >= -0.1f)
        {
            rb.bodyType = RigidbodyType2D.Static;
            still = true;
        }
    }

    public void SetDamage(int damage)
    {
        string txt;
        if (damage < 0)
        {
            txt = "+"+(-damage).ToString();
            damageText.color = healColor;
            damageText.font = healFont;
        }
        else
        {
            txt = damage.ToString();
            damageText.color = dmgColor;
            damageText.font = dmgFont;
        }

        damageText.text = txt;
    }

    public void UpdateDamageIndicator(int damage)
    {
        currentDamage += damage;
        SetDamage(currentDamage);
    }

    void DestoryNumbers()
    {
        Destroy(gameObject, 2f);
    }

    public void OnDestroy()
    {
        tl.onRoundEnd.RemoveListener(DestoryNumbers);
    }
}
