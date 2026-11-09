using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 5f;
    public float sprintSpeed = 9f;
    public float jumpHeight = 1.5f;
    public float gravity = -9.81f;

    [Header("Stamina")]
    public float maxStamina = 100f;
    public float staminaDrain = 25f;
    public float staminaRegen = 15f;
    public float currentStamina;

    [Header("UI")]
    public Slider staminaBar;

    [Header("Breathing")]
    public AudioSource breathingAudio;
    public float staminaThreshold = 30f;

    [Header("Footstep")]
    public AudioSource footstepSource;
    public AudioClip walkClip;
    public AudioClip runClip;

    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        currentStamina = maxStamina;

        staminaBar.maxValue = maxStamina;
        staminaBar.value = currentStamina;

        breathingAudio.Stop();
        footstepSource.Stop();
    }

    void Update()
    {
        // ===== Ground =====
        isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0)
            velocity.y = -2f;

        // ===== Input =====
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        bool isMoving = new Vector3(x, 0, z).magnitude > 0.1f;

        // ===== Sprint =====
        bool isSprinting = Input.GetKey(KeyCode.LeftShift) && currentStamina > 0;
        float currentSpeed = isSprinting ? sprintSpeed : speed;

        // ===== Move =====
        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(move * currentSpeed * Time.deltaTime);

        // ===== Jump =====
        if (Input.GetButtonDown("Jump") && isGrounded)
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);

        // ===== Stamina =====
        if (isSprinting && isMoving)
            currentStamina -= staminaDrain * Time.deltaTime;
        else
            currentStamina += staminaRegen * Time.deltaTime;

        currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);

        // ===== Gravity =====
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        // ===== UI =====
        staminaBar.value = currentStamina;

        // ===== Breathing Sound (ผูกกับ stamina อย่างเดียว) 
        if (currentStamina <= staminaThreshold)
        {
            if (!breathingAudio.isPlaying)
            {
                breathingAudio.loop = true;
                breathingAudio.Play();
            }
        }
        else
        {
            if (breathingAudio.isPlaying)
                breathingAudio.Stop();
        }

        // ===== Footstep Sound (อิสระจาก stamina) =====
        if (isGrounded && isMoving)
        {
            AudioClip targetClip = isSprinting ? runClip : walkClip;

            if (footstepSource.clip != targetClip)
            {
                footstepSource.clip = targetClip;
                footstepSource.loop = true;
                footstepSource.Play();
            }
            else if (!footstepSource.isPlaying)
            {
                footstepSource.Play();
            }
        }
        else
        {
            if (footstepSource.isPlaying)
                footstepSource.Stop();
        }
    }
}
