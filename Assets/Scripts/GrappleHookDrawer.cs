using UnityEngine;

public class GrappleHookDrawer : MonoBehaviour
{
    [SerializeField] private GameObject endCap;
    [SerializeField] private float length;
    [SerializeField] private float lineOffset;
    [SerializeField] private float endCapOffset;
    [SerializeField] private LineRenderer line;
    [SerializeField] private Material lineMat;
    [SerializeField] private Texture grappleSprite;
    [SerializeField] private LayerMask lazerCollisions;

    private void Awake()
    {
        endCap = Instantiate(endCap, transform.position, Quaternion.identity);
        endCap.SetActive(false);
        ClearLines();
    }
    private void Start()
    {
        lineMat.SetTexture("Main_Tex", grappleSprite);
    }
    private void OnDestroy()
    {
        Destroy(endCap);
    }
    public void ShootLazer(Vector2 direction)
    {
        DrawLazer(direction);
    }
    public void UpdateLines(Vector2 endPos)
    {
        Vector2 dir = (endPos - (Vector2)transform.position).normalized;
        SetLine(transform.position, endPos + dir * lineOffset);
        SetEndCap(endPos + dir * endCapOffset, dir);
    }
    public void SetLines(Vector2 endPos)
    {
        Vector2 dir = (endPos - (Vector2)transform.position).normalized;
        SetEndCap(endPos + dir * endCapOffset, dir);
        SetLine(transform.position, endPos + dir * lineOffset);
    }
    private void DrawLazer(Vector2 direction)
    {
        Vector2 startPosition = transform.position;
        Vector2 endPosition = new Vector2();
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, length, lazerCollisions);
        if (hit)
        {
            endPosition = hit.point;

            SetEndCap(endPosition, direction);
            SetLine(startPosition, endPosition);
        }
    }

    private void SetEndCap(Vector2 position, Vector2 direction)
    {
        endCap.SetActive(true);

        endCap.transform.position = position;
        endCap.transform.up = -direction;
    }
    private void SetLine(Vector2 startPosition, Vector2 endPosition)
    {
        if (line.positionCount != 2)
            line.positionCount = 2;

        line.SetPosition(0, startPosition);
        line.SetPosition(1, endPosition);
    }
    public void ClearLines()
    {
        line.positionCount = 0;
        endCap.SetActive(false);
    }
}
