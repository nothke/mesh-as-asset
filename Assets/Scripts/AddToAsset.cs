using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
public class AddToAsset : MonoBehaviour
{
    public Mesh source;

    public Mesh assetMesh;

    private void Start()
    {
    }

    MeshFilter _mf;
    MeshFilter mf { get { if (!_mf) _mf = GetComponent<MeshFilter>(); return _mf; } }

    [ContextMenu("Make Mesh")]
    public void MakeMeshFromSource()
    {
        if (!source)
            source = mf.sharedMesh;

        // Copy mesh
        Mesh mesh = new Mesh();
        mesh.vertices = source.vertices;
        mesh.triangles = source.triangles;
        mesh.normals = source.normals;
        mesh.uv = source.uv;
        mesh.bounds = source.bounds;

        CreateMeshAsset(mesh);

        mf.sharedMesh = mesh;
    }

    bool MeshAssetExists(in string path)
    {
        return AssetDatabase.LoadAssetAtPath<Mesh>(path) != null;
    }

    void CreateMeshAsset(Mesh mesh)
    {
        if (!AssetDatabase.IsValidFolder("Assets/Data"))
            AssetDatabase.CreateFolder("Assets", "Data");

        if (!AssetDatabase.IsValidFolder("Assets/Data/Meshes"))
            AssetDatabase.CreateFolder("Assets/Data", "Meshes");

        //string[] guids = AssetDatabase.FindAssets("", new[] { "Assets/Data/Meshes" });
        //Debug.Log("Found guids: " + guids.Length);

        int next = 0;
        //bool found = false;
        string path;
        do
        {
            path = "Assets/Data/Meshes/mesh_" + next + ".asset";
            //found = MeshAssetExists(path);
            next++;
        } while (MeshAssetExists(path));

        /*
        for (int i = 0; i < guids.Length; i++)
        {
            var str = AssetDatabase.GUIDToAssetPath(guids[i]);

            Debug.Log("Found asset: " + str);

            if (!str.Contains("mesh_" + i))
            {
                Debug.Log("Doesn't have " + i);
                next = i;
                break;
            }
        }*/

        //string path = "Assets/Data/Meshes/mesh_" + next + ".asset";

        //assetMesh = AssetDatabase.LoadAssetAtPath<Mesh>(path);
        AssetDatabase.CreateAsset(mesh, path);

        assetMesh = mesh;
    }

    private void OnDestroy()
    {
        if (!Application.isPlaying)
        {
            if (assetMesh)
            {
                string path = AssetDatabase.GetAssetPath(assetMesh);

                if (!string.IsNullOrEmpty(path))
                {
                    bool success = AssetDatabase.DeleteAsset(path);

                    if (!success)
                        Debug.LogError("Did not destroy correctly");
                }

                DestroyImmediate(assetMesh);

                Debug.Log("Destroyed " + path);
            }
        }
    }
}
