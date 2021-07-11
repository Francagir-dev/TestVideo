using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    public float Speed = 5f;
    public float JumpHeight = 2f;
    public float GroundDistance = 0.2f;
    public LayerMask Ground;
    public Transform Camera;
    public AudioSource FootAudioSource;
    public AudioClip[] FootstepSounds;
    public float FootstepLength;
    public AudioClip JumpSound;
    public AudioClip LandingSound;

    private Rigidbody _body;
    private Vector3 _inputs = Vector3.zero;
    private Vector3 _offset = Vector3.zero;
    private bool _isGrounded = true;
    private bool _wasGrounded = true;
    private Transform _groundChecker;
    private float _distanceWalked;

    void Start()
    {
        _body = GetComponent<Rigidbody>();
        _groundChecker = transform.GetChild(0);
	_distanceWalked = 0f;
    }

    void Update()
    {
        _isGrounded = Physics.CheckSphere(_groundChecker.position, GroundDistance, Ground, QueryTriggerInteraction.Ignore);
	if (_isGrounded && !_wasGrounded)
	{
	    FootAudioSource.PlayOneShot(LandingSound);
	    _wasGrounded = true;
	}

	_offset = Vector3.zero;
	_offset.y = Camera.rotation.eulerAngles.y;

        _inputs = Vector3.zero;	
        _inputs.x = Input.GetAxis("Horizontal");
        _inputs.z = Input.GetAxis("Vertical");	

	_inputs = Quaternion.Euler(_offset) * _inputs;
	
        if (Input.GetButtonDown("Jump") && _isGrounded)
        {
            _body.AddForce(Vector3.up * Mathf.Sqrt(JumpHeight * -2f * Physics.gravity.y), ForceMode.VelocityChange);
	    FootAudioSource.PlayOneShot(JumpSound);
	    _wasGrounded = false;
        }
    }

    void FixedUpdate()
    {
        _body.MovePosition(_body.position + _inputs * Speed * Time.fixedDeltaTime);
	_distanceWalked += Vector3.Distance(_inputs, Vector3.zero) * Time.fixedDeltaTime;
	if (_distanceWalked > FootstepLength && _isGrounded)
	{
	    _distanceWalked = _distanceWalked % FootstepLength;
	    PlayFootstep();
	}
    }

    void PlayFootstep()
    {
	int n = Random.Range(1, FootstepSounds.Length);
	FootAudioSource.clip = FootstepSounds[n];
	FootAudioSource.PlayOneShot(FootAudioSource.clip);
	FootstepSounds[n] = FootstepSounds[0];
	FootstepSounds[0] = FootAudioSource.clip;
    }
}
