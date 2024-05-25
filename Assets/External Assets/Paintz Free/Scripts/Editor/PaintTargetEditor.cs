using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PaintTarget))]
public class PaintTargetEditor : Editor
{
    private static Texture2D logo;
    private GUIStyle guiStyle = new GUIStyle(); //create a new variable

    public override void OnInspectorGUI()
    {
        PaintTarget script = (PaintTarget)target;
        GameObject go = (GameObject)script.gameObject;
        Renderer render = go.GetComponent<Renderer>();

        if (Application.isPlaying)
        {
            GUILayout.BeginVertical(GUI.skin.box);

            //EditorGUILayout.ObjectField((Object)script.splatTexPick, typeof(Texture2D), true);

            script.PaintAllSplats = GUILayout.Toggle(script.PaintAllSplats, "Paint All Splats");

            if (GUILayout.Button("Clear Paint"))
            {
                script.ClearPaint();
            }
            if (GUILayout.Button("Clear All Paint"))
            {
                PaintTarget.ClearAllPaint();
            }

            GUILayout.EndVertical();
        }
        else
        {
            GUILayout.BeginVertical(GUI.skin.box);

            script.paintTextureSize = (TextureSize)EditorGUILayout.EnumPopup("Paint Texture", script.paintTextureSize);
            script.renderTextureSize = (TextureSize)EditorGUILayout.EnumPopup("Render Texture", script.renderTextureSize);
            script.SetupOnStart = GUILayout.Toggle(script.SetupOnStart, "Setup On Start");
            script.PaintAllSplats = GUILayout.Toggle(script.PaintAllSplats, "Paint All Splats");

            GUILayout.EndVertical();

            if (render == null)
            {
                EditorGUILayout.HelpBox("Missing Render Component", MessageType.Error);
            }
            else
            {
                foreach (Material mat in render.sharedMaterials)
                {
                    if (!mat.shader.name.StartsWith("Paint/"))
                    {
                        EditorGUILayout.HelpBox(mat.name + "\nMissing Paint Shader", MessageType.Warning);
                    }
                }
            }

            bool foundCollider = false;
            bool foundMeshCollider = false;
            if (go.GetComponent<MeshCollider>())
            {
                foundCollider = true;
                foundMeshCollider = true;
            }
            if (go.GetComponent<BoxCollider>()) foundCollider = true;
            if (go.GetComponent<SphereCollider>()) foundCollider = true;
            if (go.GetComponent<CapsuleCollider>()) foundCollider = true;
            if (!foundCollider)
            {
                EditorGUILayout.HelpBox("Missing Collider Component", MessageType.Warning);
            }
            if (!foundMeshCollider)
            {
                EditorGUILayout.HelpBox("WARNING: Color Pick only works with Mesh Collider", MessageType.Warning);
            }
        }
    }
}