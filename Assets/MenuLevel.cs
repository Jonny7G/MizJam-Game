using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using TMPro;
public class MenuLevel : MonoBehaviour
{
    public UnityEvent OnEnter;
    public TMP_Text descriptionText;
    public SpriteRenderer lockSprite;
    [TextArea] public string description;
    public bool Unlocked;

    private Controls controls;
    private Vector2 moveAxis;
    private bool entered;
    private void Start()
    {
        controls = new Controls();
        controls.Enable();
        controls.Player.Movement.performed += (x) => { moveAxis = x.ReadValue<Vector2>(); };
    }
    public void SetUnlocked()
    {
        Unlocked = true;
        lockSprite.enabled = false;
        descriptionText.SetText(description);
    }
    private void Update()
    {
        if (Unlocked && moveAxis.y > 0 && entered)
        {
            OnEnter?.Invoke();
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        entered = true;
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        entered = false;
    }
}
