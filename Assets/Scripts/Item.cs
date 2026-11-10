using UnityEngine;

public class Item : MonoBehaviour
{
    public Vector3 itemPosition;
    public float baseScale = 1f;

    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        ApplyPerspective();
    }
    
    public void ApplyPerspective()
    {
        float perspective = CameraComponent.focalLenth / (CameraComponent.focalLenth + itemPosition.z);

        transform.localScale = Vector3.one * (perspective * baseScale);
        transform.position = new Vector2(itemPosition.x, itemPosition.y) * perspective;

        // farther objects draw behind closer ones
        if (spriteRenderer != null)
        {
            spriteRenderer.sortingOrder = Mathf.RoundToInt(-itemPosition.z);
        }
    }
}