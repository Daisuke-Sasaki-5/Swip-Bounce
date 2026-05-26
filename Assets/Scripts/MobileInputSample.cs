using System;
using System.Collections;
using UnityEngine;

using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;

using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class MobileInputVisualizer : MonoBehaviour
{
    [SerializeField]
    private float swipeDistance = 80f;

    private Vector2 startPosition;
    private Vector2 currentPosition;

    private bool isTouching;

    private string currentState = "None";

    [SerializeField] private Rigidbody2D playerRb;

    [Header("発射パワー")]
    [SerializeField] float lanchPower = 0.05f;

    private bool canShoot = true;
    private bool canInput = false;
    private bool ignoreNextRelease = false;

    void OnEnable()
    {
        EnhancedTouchSupport.Enable();
    }

    void OnDisable()
    {
        EnhancedTouchSupport.Disable();
    }

    void Update()
    {
#if UNITY_EDITOR

        UpdateMouseInput();

#else

        UpdateTouchInput();

#endif
    }

    void UpdateMouseInput()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            startPosition = Mouse.current.position.ReadValue();
            currentPosition = startPosition;

            isTouching = true;
        }

        if (Mouse.current.leftButton.isPressed)
        {
            currentPosition = Mouse.current.position.ReadValue();
        }

        if (Mouse.current.leftButton.wasReleasedThisFrame)
        {

            if(ignoreNextRelease)
            {
                ignoreNextRelease = false;
                isTouching = false;
                return;
            }

            currentPosition = Mouse.current.position.ReadValue();

            CheckGesture();

            isTouching = false;
        }
    }

    void UpdateTouchInput()
    {
        if (Touch.activeTouches.Count == 0)
        {
            return;
        }

        Touch touch = Touch.activeTouches[0];

        if (touch.phase == UnityEngine.InputSystem.TouchPhase.Began)
        {
            startPosition = touch.screenPosition;
            currentPosition = startPosition;

            isTouching = true;
        }

        if (touch.phase == UnityEngine.InputSystem.TouchPhase.Moved)
        {
            currentPosition = touch.screenPosition;
        }

        if (touch.phase == UnityEngine.InputSystem.TouchPhase.Ended)
        {
            if (ignoreNextRelease)
            {
                ignoreNextRelease = false;
                isTouching = false;
                return;
            }
            currentPosition = touch.screenPosition;

            CheckGesture();

            isTouching = false;
        }
    }

    void CheckGesture()
    {
        if (!canInput) return;

        if (!canShoot) return;

        Vector2 diff = startPosition - currentPosition;

        if (diff.magnitude < 50f) return;

        diff = Vector2.ClampMagnitude(diff, 200f);

        Vector2 force = diff * lanchPower;

        playerRb.linearVelocity = Vector2.zero;

        playerRb.AddForce(force, ForceMode2D.Impulse);

        canShoot = false;

        GameManager.instance.UseShot();
    }

    void OnGUI()
    {
        GUIStyle style = new GUIStyle(GUI.skin.label);

        style.fontSize = 32;
        style.normal.textColor = Color.white;

        if (isTouching)
        {
            DrawPoint(currentPosition, 30, Color.white);

            DrawLine(
                startPosition,
                currentPosition,
                Color.yellow,
                5f
            );
        }
    }

    void DrawPoint(Vector2 position, float size, Color color)
    {
        Rect rect = new Rect(
            position.x - size / 2,
            Screen.height - position.y - size / 2,
            size,
            size
        );

        GUI.color = color;
        GUI.Box(rect, "");
    }

    void DrawLine(Vector2 start, Vector2 end, Color color, float width)
    {
        Matrix4x4 matrix = GUI.matrix;

        Vector2 guiStart = new Vector2(
            start.x,
            Screen.height - start.y
        );

        Vector2 guiEnd = new Vector2(
            end.x,
            Screen.height - end.y
        );

        float angle =
            Vector3.Angle(
                guiEnd - guiStart,
                Vector2.right
            );

        if (guiStart.y > guiEnd.y)
        {
            angle = -angle;
        }

        float length =
            (guiEnd - guiStart).magnitude;

        GUI.color = color;

        GUIUtility.RotateAroundPivot(
            angle,
            guiStart
        );

        GUI.DrawTexture(
            new Rect(
                guiStart.x,
                guiStart.y,
                length,
                width
            ),
            Texture2D.whiteTexture
        );

        GUI.matrix = matrix;
    }

    public void SetCanShoot(bool value)
    {
        canShoot = value;
    }

    public void EnableInput()
    {
        ignoreNextRelease = true;

        isTouching = false;

        startPosition = Vector2.zero;
        currentPosition = Vector2.zero;

        StartCoroutine(EnableInputCoroutine());
    }

    private IEnumerator EnableInputCoroutine()
    {
        canInput = false;

        yield return null; // 2フレーム待つ
        yield return null; 

        canInput = true;
    }
}