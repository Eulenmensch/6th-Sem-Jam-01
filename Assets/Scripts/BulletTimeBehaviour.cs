using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;
using DG.Tweening;

public class BulletTimeBehaviour : MonoBehaviour
{
    public GameObject Camera;
    public CinemachineFreeLook BulletTimeCamera;
    public float BulletTimeScale;

    bool IsReceivingAimInput;

    private void Start()
    {
        BulletTimeCamera.enabled = false;
    }

    void LateUpdate()
    {
        BulletTime();
    }

    private void BulletTime()
    {
        if (IsReceivingAimInput)
        {
            Time.timeScale = BulletTimeScale;
            Time.fixedDeltaTime = Time.timeScale * 0.02f;
            Camera.GetComponent<CinemachineBrain>().m_UpdateMethod = CinemachineBrain.UpdateMethod.LateUpdate;
            BulletTimeCamera.enabled = true;
            //transform.DORotateQuaternion(Quaternion.LookRotation(Camera.transform.up), 0.4f).SetEase(Ease.OutCirc);
        }
        else
        {
            Time.timeScale = 1;
            Time.fixedDeltaTime = Time.timeScale * 0.02f;
            Camera.GetComponent<CinemachineBrain>().m_UpdateMethod = CinemachineBrain.UpdateMethod.FixedUpdate;
            BulletTimeCamera.enabled = false;
        }
    }

    public void GetAimInput(InputAction.CallbackContext context)
    {
        if (context.performed && !IsReceivingAimInput)
        {
            IsReceivingAimInput = true;
        }
        else if (context.performed && IsReceivingAimInput)
        {
            IsReceivingAimInput = false;
            //transform.DORotateQuaternion(Quaternion.LookRotation(new Vector3(0.0f, transform.rotation.eulerAngles.y, 0.0f), Vector3.up), 0.4f).SetEase(Ease.OutCirc);
        }
    }
}
