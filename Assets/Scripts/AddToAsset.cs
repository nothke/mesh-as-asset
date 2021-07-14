using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR
using static Nothke.Utils.AssetDatabaseUtils;
#endif

[ExecuteInEditMode]
public class AddToAsset : MonoBehaviour
{
    public Mesh source;

    public Mesh assetMesh;

    const string FOLDER_PATH = "Assets/Data/Meshes/";
    const string FILE_PATH_BASE = FOLDER_PATH + "mesh_";

    MeshFilter _mf;
    MeshFilter mf { get { if (!_mf) _mf = GetComponent<MeshFilter>(); return _mf; } }

#if UNITY_EDITOR
    private void Start()
    {
        bool validSource = source || (mf && mf.sharedMesh);

        if (validSource && !assetMesh)
        {
            MakeMeshFromSource();
        }
    }

    [ContextMenu("Make New Mesh")]
    public void MakeMeshFromSource()
    {
        Debug.Log("Creating new asset..");

        if (assetMesh)
            DestroyMeshAsset(assetMesh);

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


    void CreateMeshAsset(Mesh mesh)
    {
        int next = 0;
        string path;

        do
        {
            path = FILE_PATH_BASE + next + ".asset";
            next++;
        } while (AssetExists(path));

        CreateAsset(mesh, path);

        // Using a serialized property makes sure the change gets detected by the scene/prefab
        SerializedObject so = new SerializedObject(this);
        so.FindProperty("assetMesh").objectReferenceValue = mesh;
        so.ApplyModifiedProperties();
    }

    private void OnDestroy()
    {
        // isLoaded prevents it being run when scene is changed
        if (!Application.isPlaying && gameObject.scene.isLoaded)
        {
            if (assetMesh)
                DestroyMeshAsset(assetMesh);
        }
    }

    void DestroyMeshAsset(Mesh asset)
    {
        // Destroy the asset
        string path = AssetDatabase.GetAssetPath(asset);

        if (!string.IsNullOrEmpty(path))
        {
            bool success = AssetDatabase.DeleteAsset(path);

            if (!success)
                Debug.LogError("Did not destroy correctly");
        }

        // Destroying the mesh itself is also necessary
        DestroyImmediate(asset);

        Debug.Log("Destroyed " + path);
    }
#endif
}
