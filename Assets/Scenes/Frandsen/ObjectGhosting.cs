using UnityEngine;
using System.Collections;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class ObjectGhostTrail : MonoBehaviour
{
    [Header("Ghost Trail Settings")]
    public Material ghostMaterial;   // Assign your GhostMaterial here
    public float spawnInterval = 0.05f;
    public float fadeDuration = 1f;
    public float startAlpha = 0.15f;
    public float minVelocity = 0.05f;

    private float timer;
    private Vector3 lastPosition;

    void Start()
    {
        lastPosition = transform.position;
    }

    void Update()
    {
        timer += Time.deltaTime;

        float velocity = (transform.position - lastPosition).magnitude / Mathf.Max(Time.deltaTime, 0.0001f);
        lastPosition = transform.position;

        if (velocity > minVelocity && timer >= spawnInterval)
        {
            SpawnGhost();
            timer = 0f;
        }
    }

    void SpawnGhost()
    {
        MeshFilter sourceMF = GetComponent<MeshFilter>();
        MeshRenderer sourceMR = GetComponent<MeshRenderer>();

        if (sourceMF == null || sourceMR == null || ghostMaterial == null)
            return;

        GameObject ghost = new GameObject("Ghost");
        ghost.transform.SetPositionAndRotation(transform.position, transform.rotation);
        ghost.transform.localScale = transform.localScale;

        MeshFilter mf = ghost.AddComponent<MeshFilter>();
        mf.sharedMesh = sourceMF.sharedMesh;

        MeshRenderer mr = ghost.AddComponent<MeshRenderer>();
        Material matInstance = new Material(ghostMaterial);

        // Copy base color tint if available
        Color c = sourceMR.material.HasProperty("_BaseColor") ?
            sourceMR.material.GetColor("_BaseColor") : Color.white;
        c.a = startAlpha;
        matInstance.color = c;

        mr.material = matInstance;

        StartCoroutine(FadeAndDestroy(ghost, matInstance, fadeDuration));
    }

    IEnumerator FadeAndDestroy(GameObject ghost, Material mat, float duration)
    {
        float elapsed = 0f;
        Color c = mat.color;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            c.a = Mathf.Lerp(startAlpha, 0f, elapsed / duration);
            mat.color = c;
            yield return null;
        }

        Destroy(ghost);
    }
}
