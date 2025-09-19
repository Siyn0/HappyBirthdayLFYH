using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public bool isActivated = false;
    private SpriteRenderer spriteRenderer;

    [Header("Optional")]
    public Color activatedColor = Color.green;
    public Color deactivatedColor = Color.gray;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        UpdateVisual();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isActivated)
        {
            Activate();
        }
    }

    public void Activate()
    {
        isActivated = true;
        UpdateVisual();
    }

    public void Deactivate()
    {
        isActivated = false;
        UpdateVisual();
    }

    private void UpdateVisual()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = isActivated ? activatedColor : deactivatedColor;
        }
    }
}