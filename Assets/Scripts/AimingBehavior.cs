using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class AimingBehavior : MonoBehaviour
{
    [SerializeField] private SpriteRenderer crosshair;
    [SerializeField] private float maxDistance;

    private Vector2 mousePos;
    private Controls controls;

    private void Start()
    {
        controls = new Controls();
        controls.Player.MousePosition.performed += SetMousePosition;
        controls.Player.Escape.started += (x) => { Debug.Break(); };
        controls.Enable();

        Cursor.visible = false;
    }
    private void SetMousePosition(InputAction.CallbackContext context)
    {
        mousePos = context.ReadValue<Vector2>();
    }
    private void Update()
    {
        Vector2 worldPos = Camera.main.ScreenToWorldPoint(mousePos);
        crosshair.transform.position = new Vector3(worldPos.x, worldPos.y, crosshair.transform.position.z);
    }
    public Vector2 GetAimDir()
    {
        Vector2 worldPos = Camera.main.ScreenToWorldPoint(mousePos);
        return (worldPos - (Vector2)transform.position).normalized;
    }
}
