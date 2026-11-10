
using UnityEngine;

public class StarField : MonoBehaviour
{
    public SpriteRenderer starPrefab;
    public Transform parent;
    public int starCount = 80;

    public float spreadX = 40f;
    public float spreadY = 25f;
    public float spawnDistance = 1200f;

    public float starSpeed = 200f;
    public RunnerSpawner runnerSpawner;
    public float speedMultiplier = 0.5f;

    public float minSize = 0.3f;
    public float maxSize = 1f;
    public int sortingOrder = -5000;

    private Vector3[] positions;
    private float[] sizes;
    private SpriteRenderer[] stars;

    private void Start()
    {
        positions = new Vector3[starCount];
        sizes = new float[starCount];
        stars = new SpriteRenderer[starCount];

        for (int i = 0; i < starCount; i++)
        {
            stars[i] = Instantiate(
                starPrefab,
                parent != null ? parent : transform
            );

            stars[i].sortingOrder = sortingOrder;
            RespawnStar(i, Random.Range(1f, spawnDistance));
        }
    }

    private void Update()
    {
        float speed = runnerSpawner != null
            ? runnerSpawner.obstacleSpeed * speedMultiplier
            : starSpeed;

        for (int i = 0; i < starCount; i++)
        {
            positions[i].z -= speed * Time.deltaTime;

            if (positions[i].z <= 1f)
                RespawnStar(i, spawnDistance);

            ApplyPerspective(i);
        }
    }

    private void RespawnStar(int i, float z)
    {
        positions[i] = new Vector3(
            Random.Range(-spreadX, spreadX),
            Random.Range(-spreadY, spreadY),
            z
        );

        sizes[i] = Random.Range(minSize, maxSize);
        ApplyPerspective(i);
    }

    // similar to obstacles pers
    private void ApplyPerspective(int i)
    {
        float perspective = CameraComponent.focalLenth /
            (CameraComponent.focalLenth + positions[i].z);

        stars[i].transform.localScale =
            Vector3.one * (perspective * sizes[i]);

        stars[i].transform.position =
            new Vector2(positions[i].x, positions[i].y) * perspective;

        Color color = stars[i].color;
        color.a = Mathf.Clamp01(1f - positions[i].z / spawnDistance);
        stars[i].color = color;
    }
}
