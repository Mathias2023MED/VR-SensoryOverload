using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class ObjectGhosting : MonoBehaviour
{
    [Header("Ghosting Settings")]
    public int ghostCount = 3;            // Number of ghost copies
    public float offsetStrength = 0.05f;  // Max offset distance
    public float alpha = 0.2f;            // Base transparency
    public float ghostSmooth = 2f;        // How quickly ghosts drift to new positions
    public float changeInterval = 0.5f;   // How often to pick a new random offset

    private Mesh mesh;
    private MeshRenderer rend;

    private Vector3[] ghostOffsets;
    private Vector3[] targetOffsets;
    private float timer;

    void Start()
    {
        mesh = GetComponent<MeshFilter>().sharedMesh;
        rend = GetComponent<MeshRenderer>();

        ghostOffsets = new Vector3[ghostCount];
        targetOffsets = new Vector3[ghostCount];
        PickNewTargets();
    }

    void Update()
    {
        // Every interval, pick new random offsets
        timer += Time.deltaTime;
        if (timer > changeInterval)
        {
            timer = 0f;
            PickNewTargets();
        }

        // Smoothly move ghost offsets toward targets
        for (int i = 0; i < ghostCount; i++)
        {
            ghostOffsets[i] = Vector3.Lerp(
                ghostOffsets[i],
                targetOffsets[i],
                Time.deltaTime * ghostSmooth
            );
        }
    }

    void LateUpdate()
    {
        if (!mesh || !rend) return;

        for (int i = 0; i < ghostCount; i++)
        {
            Matrix4x4 matrix = Matrix4x4.TRS(
                transform.position + ghostOffsets[i],
                transform.rotation,
                transform.lossyScale
            );

            for (int sub = 0; sub < mesh.subMeshCount; sub++)
            {
                // Make a transparent copy of the material
                Material mat = new Material(rend.sharedMaterials[sub]);

                if (mat.HasProperty("_BaseColor"))
                {
                    Color c = mat.GetColor("_BaseColor");

                    // Stronger transparency falloff
                    float fade = alpha / Mathf.Pow(i + 1, 2f);
                    c.a = Mathf.Clamp01(fade);

                    mat.SetColor("_BaseColor", c);

                    // Force transparency mode for URP Lit
                    if (mat.HasProperty("_Surface"))
                    {
                        mat.SetFloat("_Surface", 1f); // Transparent
                    }
                    mat.renderQueue = (int)RenderQueue.Transparent;
                }


                Graphics.DrawMesh(
                    mesh,
                    matrix,
                    mat,
                    gameObject.layer,
                    null,
                    sub
                );
            }
        }
    }

    void PickNewTargets()
    {
        for (int i = 0; i < ghostCount; i++)
        {
            targetOffsets[i] = Random.insideUnitSphere * offsetStrength;
        }
    }
}

