using System.Collections;
using System.Collections.Generic;
using Com.Immersive.Cameras;
using UnityEditor;
using UnityEngine;

namespace Com.Immersive.Hotspots
{
    [ExecuteInEditMode]
    public class RegionHotspot : MonoBehaviour
    {
        public enum LineType
        {
            Solid,
            Dotted
        }

        public LineRenderer lineRenderer;

        [Range(0.1f, 5.0f)]
        public float lineThikness = 1;
        public LineType lineType;
        public Color lineColor = Color.white;
        public Gradient lineColorGradient;
        public Texture dotTexture;

        public Material SolidMaterial;
        public Material DottedMaterial;
        public BoxCollider boxCollider;

        void Awake()
        {
            Init();
        }

        public void Init()
        {
            lineThikness = 1.0f;
            SolidMaterial = new Material(Shader.Find("Sprites/Default"));
            DottedMaterial = new Material(Shader.Find("Unlit/UnlitAlphaWithColor"));
            DottedMaterial.mainTextureOffset = new Vector2(-0.5f, 0);

            lineRenderer = GetComponentInChildren<LineRenderer>();

            DrawLine();
        }

        public void DrawLine()
        {
            Bounds bounds;
            boxCollider = GetComponent<BoxCollider>();

            if (boxCollider != null)
                bounds = boxCollider.bounds;
            else
                return;

            if (lineType == LineType.Solid)
            {
                lineRenderer.material = SolidMaterial;
                lineRenderer.startColor = lineColor;
                lineRenderer.endColor = lineColor;
            }
            else
            {
                lineRenderer.material = DottedMaterial;
                DottedMaterial.mainTexture = dotTexture;
                DottedMaterial.color = lineColor;

                //DottedMaterial.mainTextureScale = new Vector2(Mathf.Round((1f/lineThikness) * 10f) / 10f, 1);
            }

            Vector3 v3Center = bounds.center;
            Vector3 v3Extents = bounds.extents;

            List<Vector3> linePos = new List<Vector3>();
            v3Center = Vector3.zero;
            linePos.Add(new Vector3(v3Center.x - v3Extents.x, v3Center.y + v3Extents.y, 0) * 100);//Top-Left corner
            linePos.Add(new Vector3(v3Center.x + v3Extents.x, v3Center.y + v3Extents.y, 0) * 100);//Top-Right corner
            linePos.Add(new Vector3(v3Center.x + v3Extents.x, v3Center.y - v3Extents.y, 0) * 100);//Bottom-Right corner
            linePos.Add(new Vector3(v3Center.x - v3Extents.x, v3Center.y - v3Extents.y, 0) * 100);//Bottom-Left corner            

            float dis = 0;
            for (int i = 0; i < linePos.Count - 1; i++)
            {
                dis += Vector2.Distance(linePos[i], linePos[i + 1]);
            }

            dis += Vector2.Distance(linePos[1], linePos[2]);

            DottedMaterial.mainTextureScale = new Vector2(dis / lineThikness, 1);

            lineRenderer.widthMultiplier = lineThikness;

            lineRenderer.SetPositions(linePos.ToArray());
        }

        private void Update()
        {
            if (!Application.isPlaying || transform.hasChanged)
                DrawLine();
        }
    }

#if UNITY_EDITOR

    /// <summary>
    /// This implements a custom editor for a RegionHotspot
    /// </summary>
    [CustomEditor(typeof(RegionHotspot))]
    public class RegionHotspotEditor : Editor
    {
        private RegionHotspot regionHotspot;

        private SerializedProperty lineThikness;
        private SerializedProperty lineType;
        private SerializedProperty lineColor;
        private SerializedProperty lineColorGradient;
        private SerializedProperty dotTexture;

        private Vector3 startSize;
        private Vector2 startMousePos;

        private void OnEnable()
        {
            lineThikness = serializedObject.FindProperty("lineThikness");
            lineType = serializedObject.FindProperty("lineType");
            lineColor = serializedObject.FindProperty("lineColor");
            lineColorGradient = serializedObject.FindProperty("lineColorGradient");
            dotTexture = serializedObject.FindProperty("dotTexture");

            regionHotspot = (RegionHotspot)target;
        }

        private void OnSceneGUI()
        {
            if (Tools.current == Tool.Rect)
            {
                Event current = Event.current;
                HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));

                switch (current.type)
                {
                    case EventType.MouseDown:
                        MouseDown();
                        break;

                    case EventType.MouseDrag:
                        MouseDrag();
                        break;

                    case EventType.MouseUp:
                        HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Keyboard));
                        current.Use();
                        break;
                }
            }
        }

        void MouseDown()
        {
            previouPos = Event.current.mousePosition;
        }

        void MouseDrag()
        {
            if (Tools.current == Tool.Rect)
            {
                ScaleHotspot(Event.current.mousePosition);
            }
        }

        public float scaleFactor = 0.01f;
        Vector3 previouPos;
        void ScaleHotspot(Vector3 currentPos)
        {
            Vector2 startX = new Vector2(previouPos.x, 0);
            Vector2 currentX = new Vector2(currentPos.x, 0);

            Vector2 startY = new Vector2(0, previouPos.y);
            Vector2 currentY = new Vector2(0, currentPos.y);

            float x = Vector2.Distance(startX, currentX) * scaleFactor; //Distance for x value
            float y = Vector2.Distance(startY, currentY) * scaleFactor; //Distance for y value

            float currentDistance = Vector2.Distance(regionHotspot.transform.position, currentPos);
            float previousDistance = Vector2.Distance(regionHotspot.transform.position, previouPos);

            int direction = 0;
            if (currentDistance > previousDistance)
            {
                direction = 1;
            }
            else
            {
                direction = -1;
            }

            if (Input.GetKey(KeyCode.LeftControl))
            {
                //set unified scale
                regionHotspot.boxCollider.size += new Vector3(x, x, 0) * direction;
            }
            else
            {
                //set independent scale
                regionHotspot.boxCollider.size += new Vector3(x, y, 0) * direction;
            }

            previouPos = currentPos;
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.PropertyField(lineThikness, new GUIContent("Line Thikness"), true);
            EditorGUILayout.PropertyField(lineType, new GUIContent("Line Type"), true);

            EditorGUILayout.PropertyField(lineColor, new GUIContent("Color"), true);

            if (regionHotspot.lineType == RegionHotspot.LineType.Dotted)
            {

                EditorGUILayout.PropertyField(dotTexture, new GUIContent("Texture"), true);
            }
            //else
            //    EditorGUILayout.PropertyField(lineColorGradient, new GUIContent("Color"), true);

            serializedObject.ApplyModifiedProperties();
        }
    }
#endif
}