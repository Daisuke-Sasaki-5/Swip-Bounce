using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private int hp = 1;

    public void TakeDamage(int damage)
    {
        hp -= damage;

        if(hp <= 0 )
        {
            GameManager.instance.OnEnemyDefeated();
            Destroy(gameObject);
        }
    }
}
