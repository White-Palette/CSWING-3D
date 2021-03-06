using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Cinemachine;

namespace GameScene
{
    public class PlayerMove : MonoSingleton<PlayerMove>
    {
        private Vector3 _rotate;

        //[SerializeField] float _speed = 1f;
        public float Speed = 1f;
        [SerializeField] float _xRotateSpeed = 1f;
        [SerializeField] float _yRotateSpeed = 1f;
        [SerializeField] float _zRotateSpeed = 1f;
        [SerializeField] float _speedChange = 1f;
        [SerializeField] float _smooth = 3f;
        [SerializeField] float _maxSpeed = 10f;
        public float MaxSpeed
        {
            get { return _maxSpeed; }
        }
        [SerializeField] float _dashSpeed = 10f;
        [SerializeField] GameObject explosionEffect = null;
        [SerializeField] GameObject cameraPos = null;
        [SerializeField] CinemachineVirtualCamera virtualCamera;

        private Rigidbody rigid;
        public bool isTimeStop = false;

        bool isLeftDash = false;
        bool isRightDash = false;

        private void Start()
        {
            rigid = GetComponent<Rigidbody>();
            virtualCamera.m_Lens.FieldOfView = 25;
        }

        private void Update()
        {
            float horizontal = Input.GetAxis("Mouse X");
            float vertical = Input.GetAxis("Mouse Y");
            float rotate = Input.GetAxis("Horizontal");

            horizontal = Mathf.Clamp(horizontal, -1f, 1f);
            vertical = Mathf.Clamp(vertical, -1f, 1f);

            Vector3 lerpVector = new Vector3(vertical * _xRotateSpeed, -horizontal * _yRotateSpeed, 0).normalized * ((Mathf.Abs(horizontal) + Mathf.Abs(vertical)) / 2);
            _rotate = Vector3.Lerp(_rotate, lerpVector, _smooth * Time.deltaTime);
            if (!isTimeStop)
            {
                transform.Rotate(((-_rotate) + (new Vector3(0, 0, rotate) * _zRotateSpeed * -1)));
            }

            float speed = Input.GetAxis("Vertical");

            if (speed < 0 && Input.GetKey(KeyCode.Space))
            {
                Speed = Mathf.Lerp(Speed, 0f, Time.deltaTime * (1 / _speedChange));
                speed = 0;
            }
            else
            {
                Speed += speed * _speedChange * Time.deltaTime;
                Speed = Mathf.Clamp(Speed, -(_maxSpeed / 2), _maxSpeed);
            }

            Debug.DrawRay(transform.position, transform.forward * 100, Color.red);
            // transform.Translate(transform.forward * _speed * Time.deltaTime);
            rigid.velocity = transform.forward * Speed;

            if (Input.GetKeyDown(KeyCode.A) && !isLeftDash && !isRightDash)
            {
                StartCoroutine(LeftDash());
            }
            if (Input.GetKeyDown(KeyCode.D) && !isRightDash && !isLeftDash)
            {
                StartCoroutine(RightDash());
            }

            virtualCamera.m_Lens.FieldOfView = 50 + Mathf.Abs(Speed) / _maxSpeed * 10;

            if (cameraPos.transform.parent == null)
            {
                cameraPos.transform.localPosition = transform.position + new Vector3(0, 6.5f, -15);
            }
        }

        bool isPushedLeft = false;
        IEnumerator LeftDash()
        {
            if (isPushedLeft)
            {
                isLeftDash = true;
                Debug.Log("LeftDash");
                SoundManager.Instance.PlaySound("Dodge");
                transform.DOMove(transform.position + -transform.right * _dashSpeed + transform.forward * Speed, 1f);
                transform.DORotate(new Vector3(0, 0, 360), 1f, RotateMode.LocalAxisAdd);
                cameraPos.transform.SetParent(null);
                yield return new WaitForSeconds(1f);
                isLeftDash = false;
                cameraPos.transform.SetParent(transform);
                cameraPos.transform.localPosition = new Vector3(0, 3.5f, -15f);
            }
            else
            {
                isPushedLeft = true;
                yield return new WaitForSeconds(0.2f);
                isPushedLeft = false;
            }
        }

        bool isPushedRight = false;
        IEnumerator RightDash()
        {
            if (isPushedRight)
            {
                isRightDash = true;
                Debug.Log("RightDash");
                SoundManager.Instance.PlaySound("Dodge");
                transform.DOMove(transform.position + transform.right * _dashSpeed + transform.forward * Speed, 1f);
                transform.DORotate(new Vector3(0, 0, -360), 1f, RotateMode.LocalAxisAdd);
                cameraPos.transform.SetParent(null);
                yield return new WaitForSeconds(1f);
                isRightDash = false;
                cameraPos.transform.SetParent(transform);
                cameraPos.transform.localPosition = new Vector3(0, 3.5f, -15f);
            }
            else
            {
                isPushedRight = true;
                yield return new WaitForSeconds(0.2f);
                isPushedRight = false;
            }
        }

        public void ResetPosition()
        {
            transform.position = Vector3.zero;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("PursuitBullet") || other.gameObject.CompareTag("Enemy") || other.GetComponent<PursuitController>() != null)
            {
                StartCoroutine(PlayerExplosion());
            }

            if (other.gameObject.CompareTag("SafeZone"))
            {
                SafeZoneManager.Instance.OnEnterSafeZone();
                UIManager.Instance.OnSafeZoneCounterUpdate(0);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.CompareTag("SafeZone"))
            {
                SafeZoneManager.Instance.OnExitSafeZone();
            }
        }

        public IEnumerator PlayerExplosion()
        {
            GameObject effect = Instantiate(explosionEffect, transform.position, Quaternion.identity);
            yield return new WaitForSeconds(2f);
            Destroy(effect);
        }
    }
}