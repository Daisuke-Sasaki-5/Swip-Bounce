using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;

    [SerializeField] private float power = 10f;
    [SerializeField] private MobileInputVisualizer inputManager;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if(rb.linearVelocity.magnitude < 0.1f)
        {
            inputManager.SetCanShoot(true);
        }

#if UNITY_EDITOR
        if (Keyboard.current.spaceKey.wasPressedThisFrame)

#else
            if(Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
#endif
        {
            rb.linearVelocity = Vector2.zero;

            rb.AddForce(new Vector2(1, 1).normalized * power, ForceMode2D.Impulse);
        }
    }
};