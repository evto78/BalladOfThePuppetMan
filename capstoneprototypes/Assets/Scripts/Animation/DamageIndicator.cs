using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DamageIndicator : MonoBehaviour
{
    Rigidbody2D rb;
    TextMeshProUGUI damageText;

    public float initialYVelocity = 7.5f;
    public float lifeTime = 0.5f;

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
        damageText.text = damage.ToString();
    }
}
