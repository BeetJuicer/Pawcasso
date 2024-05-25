using UnityEngine;

public class PaintExample : MonoBehaviour
{
    public Brush brush;
    public bool RandomChannel = false;
    public bool SingleShotClick = false;
    public bool ClearOnClick = false;
    public bool IndexBrush = false;

    private float cameraSensitivity = 360;
    private float translateSpeed = 50;
    private float climbSpeed = 4;
    private float normalMoveSpeed = 10;
    private float slowMoveFactor = 0.25f;
    private float fastMoveFactor = 3;

    private float rotationX = 0.0f;
    private float rotationY = 0.0f;

    private bool HoldingButtonDown = false;

    //private Vector3 rotatePoint = Vector3.zero;

    private bool Help = false;

    private void Start()
    {
        colorTex = new Texture2D(1, 1);

        rotationX = transform.eulerAngles.y;
        rotationY = -transform.eulerAngles.x;

        if (brush.splatTexture == null)
        {
            brush.splatTexture = Resources.Load<Texture2D>("splats");
            brush.splatsX = 4;
            brush.splatsY = 4;
        }
    }

    private void Update()
    {
        CameraControl();

        if (Input.GetKeyDown(KeyCode.Alpha1)) brush.splatChannel = 0;
        if (Input.GetKeyDown(KeyCode.Alpha2)) brush.splatChannel = 1;
        if (Input.GetKeyDown(KeyCode.Alpha5)) brush.splatChannel = 4;

        if (RandomChannel) brush.splatChannel = Random.Range(0, 2);

        if (Input.GetMouseButton(0))
        {
            if (!SingleShotClick || (SingleShotClick && !HoldingButtonDown))
            {
                if (ClearOnClick) PaintTarget.ClearAllPaint();
                PaintTarget.PaintCursor(brush);
                if (IndexBrush) brush.splatIndex++;
            }
            HoldingButtonDown = true;
        }
        else
        {
            HoldingButtonDown = false;
        }
    }


    private Texture2D colorTex;
    private bool ShowMenu = true;

    private void OnGUI()
    {
        ShowMenu = GUILayout.Toggle(ShowMenu,"");
        if (!ShowMenu) return;

        GUILayout.BeginVertical(GUI.skin.box);

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Channel 0")) brush.splatChannel = 0;
        if (GUILayout.Button("Channel 1")) brush.splatChannel = 1;
        if (GUILayout.Button("Erase"))
        {
            brush.splatChannel = 4;
            RandomChannel = false;
            ClearOnClick = false;
        }

        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        RandomChannel = GUILayout.Toggle(RandomChannel, "Random");
        SingleShotClick = GUILayout.Toggle(SingleShotClick, "Single Click");
        ClearOnClick = GUILayout.Toggle(ClearOnClick, "Clear on Click");
        IndexBrush = GUILayout.Toggle(IndexBrush, "Index Brush");
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Paint Size");
        brush.splatScale = GUILayout.HorizontalSlider(brush.splatScale, .1f, 5f);
        GUILayout.EndHorizontal();

        if (GUILayout.Button("Clear ALL")) PaintTarget.ClearAllPaint();

        //Texture2D c = new Texture2D(1, 1);
        colorTex.SetPixel(0, 0, PaintTarget.CursorColor());
        colorTex.Apply();

        //GUILayout.Box(colorTex, GUILayout.Width(128), GUILayout.Height(32));
        //GUILayout.Box("CURSOR COLOR:" + PaintTarget.CursorColor());

        GUI.DrawTexture(new Rect(0, Screen.height - 32, 32, 32), colorTex);
        GUILayout.Box("CURSOR CHANNEL:" + PaintTarget.CursorChannel());

        Help = GUILayout.Toggle(Help, "Show Help");

        if (Help)
        {
            GUILayout.Label("Movement: (Hold Right Mouse Button)");
            GUILayout.Label("W/S/A/D - Move Forward/Back/Left/Right");
            GUILayout.Label("* SHIFT = FASTER / CTRL = SLOWER");
            GUILayout.Label("Q/E - Move Up/Down");
            GUILayout.Label("Scroll Wheel - Zoom");
            GUILayout.Label("Paint: Left Mouse Button");
        }

        GUILayout.EndVertical();
    }

    private void CameraControl()
    {
        float zoom = Input.GetAxis("Mouse ScrollWheel");
        if (zoom != 0)
        {
            transform.Translate(transform.forward * 5 * zoom, Space.World);
        }

        if (Input.GetMouseButton(2))
        {
            transform.Translate(-Input.GetAxis("Mouse X") * translateSpeed * Time.deltaTime, -Input.GetAxis("Mouse Y") * translateSpeed * Time.deltaTime, 0, Space.Self);
        }

        if (Input.GetMouseButton(1))
        {
            rotationX += Input.GetAxis("Mouse X") * cameraSensitivity * Time.deltaTime;
            rotationY += Input.GetAxis("Mouse Y") * cameraSensitivity * Time.deltaTime;
            rotationY = Mathf.Clamp(rotationY, -90, 90);

            transform.localRotation = Quaternion.AngleAxis(rotationX, Vector3.up);
            transform.localRotation *= Quaternion.AngleAxis(rotationY, Vector3.left);

            float speed = normalMoveSpeed;
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) speed = normalMoveSpeed * fastMoveFactor;
            if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) speed = normalMoveSpeed * slowMoveFactor;
            transform.position += transform.forward * speed * Input.GetAxis("Vertical") * Time.deltaTime;
            transform.position += transform.right * speed * Input.GetAxis("Horizontal") * Time.deltaTime;

            if (Input.GetKey(KeyCode.Q)) { transform.position += transform.up * climbSpeed * Time.deltaTime; }
            if (Input.GetKey(KeyCode.E)) { transform.position -= transform.up * climbSpeed * Time.deltaTime; }
        }
    }
}