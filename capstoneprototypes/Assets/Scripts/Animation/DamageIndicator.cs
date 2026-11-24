using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class DamageIndicator : MonoBehaviour
{
    Rigidbody2D rb;
    TextMeshProUGUI damageText;

    public float initialYVelocity = 7.5f;
    public float lifeTime = 0.5f;

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
        Destroy(gameObject, lifeTime);
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
            txt = "-"+damage.ToString();
            damageText.color = dmgColor;
            damageText.font = dmgFont;
        }

        damageText.text = txt;
    }
}
