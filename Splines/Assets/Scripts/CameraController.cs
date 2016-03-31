using System;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;
using UnityStandardAssets.Utility;

public class CameraController : MonoBehaviour 
{
    [SerializeField] private CurveControlledBob m_HeadBob = new CurveControlledBob();
    [SerializeField] private MouseLook m_MouseLook;
    [SerializeField] private float m_StepInterval;
    [SerializeField] private bool m_UseHeadBob;

    private CharacterController m_CharacterController;
    private Camera m_Camera;
    private Vector3 m_OriginalCameraPosition;
   
    private Vector2 m_Input;
    private Vector3 m_MoveDir = Vector3.zero; 

    void Start () 
    {
        m_CharacterController = GetComponent<CharacterController>();
        m_Camera = Camera.main;
        m_OriginalCameraPosition = m_Camera.transform.localPosition;
        m_HeadBob.Setup(m_Camera, m_StepInterval);
        m_MouseLook.Init(transform, m_Camera.transform);
    }
	
	void Update () 
    {
        RotateView();
    }

    private void RotateView()
    {
        m_MouseLook.LookRotation(transform, m_Camera.transform);
    }

    private void FixedUpdate()
    {
        // always move along the camera forward as it is the direction that it being aimed at
        Vector3 desiredMove = transform.forward * m_Input.y + transform.right * m_Input.x;

        // get a normal for the surface that is being touched to move along it
        RaycastHit hitInfo;
        Physics.SphereCast(transform.position, m_CharacterController.radius, Vector3.down, out hitInfo,
                           m_CharacterController.height / 2f);
        desiredMove = Vector3.ProjectOnPlane(desiredMove, hitInfo.normal).normalized;

        m_MoveDir.x = desiredMove.x;
        m_MoveDir.z = desiredMove.z;
        
        UpdateCameraPosition();
    }

    private void GetInput(out float speed)
    {
        throw new NotImplementedException();
    }

    private void UpdateCameraPosition()
    {
        Vector3 newCameraPosition;
        if (!m_UseHeadBob)
        {
            return;
        }

        newCameraPosition = m_Camera.transform.localPosition;
        newCameraPosition.y = m_OriginalCameraPosition.y;
    
        m_Camera.transform.localPosition = newCameraPosition;
    }
}
