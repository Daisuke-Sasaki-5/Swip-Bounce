using System;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    [SerializeField] private int hp = 3;

    private int maxHP;

    [Header("HPÉoÅ[")]
    [SerializeField] private Image hpFill;

    private void Start()
    {
        maxHP = hp;
        UpdateHPBar();
    }

    private void UpdateHPBar()
    {
        hpFill.fillAmount = (float)hp / maxHP;
    }

    public void TakeDamage(int damage)
    {
        hp -= damage;
        UpdateHPBar();

        if(hp <= 0 )
        {
            GameManager.instance.OnEnemyDefeated();
            Destroy(gameObject);
        }
    }
}
