using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XInputDotNetPure; // Required in C#

[RequireComponent(typeof(CharacterController))]
public class Player_Controller : MonoBehaviour
{

    public enum Control_Scheme
    {
        Dual_Analogue,
        Mouse_and_Keyboard
    }

    //System Variables
    private Quaternion targetRotation;
    public Control_Scheme _control_Scheme;

    [Header("Components")]
    public Gun[] Guns;
    public Transform _hand;
    private Gun _current_Gun;
    private CharacterController _controller;
    private Animator _animator;
    private Rigidbody _rigidBody;

    [Header("Movement")]
    public float rotationSpeed = 450;
    public float walkSpeed = 5;
    public float runSpeed = 8;
    private bool m_wasGrounded;
    private bool m_isGrounded;
    private bool m_isAxisInUse = false;
    private float m_jumpTimeStamp = 0;
    private float m_minJumpInterval = 0.25f;
    [SerializeField] private float m_jumpForce = 4;

    [Header("Actions")]
    private bool reloading;

    void Start()
    {
        _animator = GetComponent<Animator>();
        _controller = GetComponent<CharacterController>();

        Equip_Gun(0);

    }

    void Update()
    {
        _animator.SetBool("Grounded", m_isGrounded);

        //-------------------------Movement---------------------------------

        #region Movement



        switch (_control_Scheme)
        {
            case Control_Scheme.Dual_Analogue:

                Dual_Analogue();

                break;
            case Control_Scheme.Mouse_and_Keyboard:

                Mouse_and_Keyboard();


                break;
        }

        m_wasGrounded = m_isGrounded;

        #endregion

        //-------------------------Shooting---------------------------------

        #region Shooting

        if (_current_Gun)
        {

            //Controller
            if (_current_Gun.gunType == Gun.GunType.Semi)
            {
                if (Input.GetAxisRaw("Right Trigger") != 0)
                {
                    if (m_isAxisInUse == false)
                    {
                        _current_Gun.shoot();
                        m_isAxisInUse = true;
                    }
                }
                if (Input.GetAxisRaw("Right Trigger") == 0)
                {
                    m_isAxisInUse = false;
                }
            }
            else if (_current_Gun.gunType == Gun.GunType.Auto)
            {
                float rTriggerFloat = Input.GetAxisRaw("Right Trigger");

                if (rTriggerFloat > 0.0f)
                {
                    _current_Gun.Shoot_Continuous();
                }
            }

            //Mouse
            if (Input.GetButtonDown("Shoot")) // Shoot once
            {
                _current_Gun.shoot();
            }
            else if (Input.GetButton("Shoot")) //Continious fire
            {
                _current_Gun.Shoot_Continuous();
            }
        }


        #endregion

        //-------------------------Swapping weapons--------------------------

        #region Switching_weapons

        // Controller
        if (Input.GetButtonDown("Y Button"))
        {
            switch (_current_Gun.GunID)
            {
                case 0:

                    Equip_Gun(1);

                    break;

                case 1:

                    Equip_Gun(0);


                    break;
            }
        }

        //Mouse and Keyboard
        for (int i = 0; i < Guns.Length; i++)
        {
            if (Input.GetKeyDown((i + 1) + "") || Input.GetKeyDown("[" + (i + 1) + "]"))
            {
                Equip_Gun(i);
                break;
            }
        }

        #endregion

        //-------------------------Reloading---------------------------------

        #region Reloading

        //Mouse and Keyboard

        if (Input.GetButtonDown("Reload"))
        {
            if (_current_Gun.Reload())
            {
                //Trigger reload animation

                //----------------------------------------

                reloading = true;
            }
        }

        if (reloading)
        {
            _current_Gun.FinishReload();
            reloading = false;
        }

        #endregion


    }

    #region Equipment

    void Equip_Gun(int i)
    {
        if (_current_Gun)
        {
            Destroy(_current_Gun.gameObject);
        }

        _current_Gun = Instantiate(Guns[i], _hand.position, _hand.rotation) as Gun;
        _current_Gun.transform.parent = _hand;
    }

    #endregion

    #region Movement

    void Mouse_and_Keyboard()
    {
        //player input
        Vector3 input = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));

        //Make Player look in direction of movement
        if (input != Vector3.zero)
        {
            targetRotation = Quaternion.LookRotation(input);

            //smooth rotation
            transform.eulerAngles = Vector3.up * Mathf.MoveTowardsAngle(transform.eulerAngles.y, targetRotation.eulerAngles.y, rotationSpeed * Time.deltaTime);
        }

        //Movement (.move constrains for collisions)
        Vector3 _motion = input.normalized;
        _motion *= (Input.GetButton("Run")) ? runSpeed : walkSpeed;
        _motion += Vector3.up * -8;

        _controller.Move(_motion * Time.deltaTime);

        _animator.SetFloat("MoveSpeed", Mathf.Sqrt(_motion.x * _motion.x + _motion.z * _motion.z));

        JumpingAndLanding();
    }

    void Dual_Analogue()
    {
        //player input
        Vector3 input = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));

        //rotation
        Vector3 playerDirection = Vector3.right * Input.GetAxisRaw("Mouse X") + Vector3.forward * -Input.GetAxisRaw("Mouse Y");
        if (playerDirection.sqrMagnitude > 0.0f)
        {
            transform.rotation = Quaternion.LookRotation(playerDirection, Vector3.up);
        }

        //Movement (.move constrains for collisions)
        Vector3 _motion = input.normalized;
        _motion *= (Input.GetButton("A Button")) ? runSpeed : walkSpeed;
        _motion += Vector3.up * -8;

        _controller.Move(_motion * Time.deltaTime);

        _animator.SetFloat("MoveSpeed", Mathf.Sqrt(_motion.x * _motion.x + _motion.z * _motion.z));

        JumpingAndLanding();
    }

    private void JumpingAndLanding()
    {
        bool jumpCooldownOver = (Time.time - m_jumpTimeStamp) >= m_minJumpInterval;

        if (jumpCooldownOver && m_isGrounded && Input.GetKey(KeyCode.Space))
        {
            m_jumpTimeStamp = Time.time;
            _rigidBody.AddForce(Vector3.up * m_jumpForce, ForceMode.Impulse);
        }

        if (!m_wasGrounded && m_isGrounded)
        {
            _animator.SetTrigger("Land");
        }

        if (!m_isGrounded && m_wasGrounded)
        {
            _animator.SetTrigger("Jump");
        }
    }

    private List<Collider> m_collisions = new List<Collider>();

    private void OnCollisionEnter(Collision collision)
    {
        ContactPoint[] contactPoints = collision.contacts;
        for (int i = 0; i < contactPoints.Length; i++)
        {
            if (Vector3.Dot(contactPoints[i].normal, Vector3.up) > 0.5f)
            {
                if (!m_collisions.Contains(collision.collider))
                {
                    m_collisions.Add(collision.collider);
                }
                m_isGrounded = true;
            }
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        ContactPoint[] contactPoints = collision.contacts;
        bool validSurfaceNormal = false;
        for (int i = 0; i < contactPoints.Length; i++)
        {
            if (Vector3.Dot(contactPoints[i].normal, Vector3.up) > 0.5f)
            {
                validSurfaceNormal = true; break;
            }
        }

        if (validSurfaceNormal)
        {
            m_isGrounded = true;
            if (!m_collisions.Contains(collision.collider))
            {
                m_collisions.Add(collision.collider);
            }
        }
        else
        {
            if (m_collisions.Contains(collision.collider))
            {
                m_collisions.Remove(collision.collider);
            }
            if (m_collisions.Count == 0) { m_isGrounded = false; }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (m_collisions.Contains(collision.collider))
        {
            m_collisions.Remove(collision.collider);
        }
        if (m_collisions.Count == 0) { m_isGrounded = false; }
    }

    #endregion
}
