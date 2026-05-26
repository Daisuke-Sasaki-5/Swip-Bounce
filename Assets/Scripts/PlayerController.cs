using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;

    [SerializeField] private float power = 10f;
    [SerializeField] private MobileInputVisualizer inputManager;

    private float stopTimer = 0f;
    [SerializeField] private float stopTime = 0.3f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (rb.linearVelocity.magnitude < 0.1f)
        {
            stopTimer += Time.deltaTime;

            if (stopTimer >= stopTime)
            {

                inputManager.SetCanShoot(true);

                // 全ショット使い切っていたら
                if (GameManager.instance.IsShotEmpty())
                {
                    GameManager.instance.GameOver();
                }
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Enemy enemy = collision.gameObject.GetComponent<Enemy>();

        if (enemy != null)
        {
            enemy.TakeDamage(1);
        }
    }
};