using UnityEngine;
using UnityEngine.XR.ARSubsystems;

namespace FlatPixel.XR.Mock.Example
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField]
        float m_MoveSpeed = 1f;

        [SerializeField]
        float m_TurnSpeed = 5f;

        [SerializeField]
        KeyCode m_KeyForward = KeyCode.W;

        [SerializeField]
        KeyCode m_KeyLeft = KeyCode.A;

        [SerializeField]
        KeyCode m_KeyBack = KeyCode.S;

        [SerializeField]
        KeyCode m_KeyRight = KeyCode.D;

        [SerializeField]
        KeyCode m_KeyUp = KeyCode.Q;

        [SerializeField]
        KeyCode m_KeyDown = KeyCode.E;

        [SerializeField]
        MouseButton m_LookMouseButton = MouseButton.Right;

        enum MouseButton
        {
            Left = 0,
            Middle = 2,
            Right = 1
        }

        Vector3 m_PreviousMousePosition;

        [System.NonSerialized]
        Feature m_lastCameraFacing = Feature.None;
        
        [System.NonSerialized]
        Color worldColor = new Color(0.5f, 0.25f, 0.6f, 1f);
        [System.NonSerialized]
        Color userColor = new Color(1f, 0.9f, 0.2f, 1f);

        void Move(Vector3 direction)
        {
            transform.position += direction * (Time.deltaTime * m_MoveSpeed);
        }

        void ChangeCameraFacing()
        {
            transform.RotateAround(transform.position, transform.up, 180);

            switch (CameraApi.currentFacingDirection)
            {
                case Feature.WorldFacingCamera:
                    gameObject.name = "Mock Camera (Wold Facing)";
                    Camera.main.backgroundColor = worldColor;
                    break;
                case Feature.UserFacingCamera:
                    gameObject.name = "Mock Camera (User Facing)";
                    Camera.main.backgroundColor = userColor;
                    break;
                default:
                    gameObject.name = "Mock Camera (None)";
                    break;
            }

            m_lastCameraFacing = CameraApi.currentFacingDirection;
        }

        void OnEnable()
        {
            m_lastCameraFacing = CameraApi.currentFacingDirection;

            switch (CameraApi.currentFacingDirection)
            {
                case Feature.WorldFacingCamera:
                    gameObject.name = "Mock Camera (Wold Facing)";
                    break;
                case Feature.UserFacingCamera:
                    gameObject.name = "Mock Camera (User Facing)";
                    break;
                default:
                    gameObject.name = "Mock Camera (None)";
                    break;
            }
        }

        void Update()
        {
            int mouseButton = (int) m_LookMouseButton;
            if (Input.GetMouseButton(mouseButton))
            {
                if (Input.GetMouseButtonDown(mouseButton))
                    m_PreviousMousePosition = Input.mousePosition;

                var delta = (Input.mousePosition - m_PreviousMousePosition) * Time.deltaTime * m_TurnSpeed;
                var euler = transform.rotation.eulerAngles;
                euler.x -= delta.y;
                euler.y += delta.x;
                transform.rotation = Quaternion.Euler(euler);
                m_PreviousMousePosition = Input.mousePosition;
            }

            var forwardXZ = new Vector3(transform.forward.x, 0, transform.forward.z);

            if (Input.GetKey(m_KeyForward))
                Move(forwardXZ);
            if (Input.GetKey(m_KeyLeft))
                Move(-transform.right);
            if (Input.GetKey(m_KeyRight))
                Move(transform.right);
            if (Input.GetKey(m_KeyBack))
                Move(-forwardXZ);
            if (Input.GetKey(m_KeyDown))
                Move(-transform.up);
            if (Input.GetKey(m_KeyUp))
                Move(transform.up);
            if (Input.GetAxis("Mouse ScrollWheel") > 0f)
                Move(transform.forward);
            if (Input.GetAxis("Mouse ScrollWheel") < 0f)
                Move(-transform.forward);

            if (CameraApi.currentFacingDirection != m_lastCameraFacing)
                ChangeCameraFacing();

            InputApi.pose = new Pose(transform.position, transform.rotation);
        }
    }
}
