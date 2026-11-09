using UnityEngine;

public class HeadBob : MonoBehaviour
{
    public float walkBobSpeed = 8f;
    public float walkBobAmount = 0.08f;

    public float runBobSpeed = 12f;
    public float runBobAmount = 0.15f;

    private float defaultYPos;
    private float timer;

    void Start()
    {
        defaultYPos = transform.localPosition.y;
    }

    void Update()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        bool isMoving = new Vector3(x, 0, z).magnitude > 0.1f;
        bool isSprinting = Input.GetKey(KeyCode.LeftShift);

        if (isMoving)
        {
            float bobSpeed = isSprinting ? runBobSpeed : walkBobSpeed;
            float bobAmount = isSprinting ? runBobAmount : walkBobAmount;

            timer += Time.deltaTime * bobSpeed;
            float newY = defaultYPos + Mathf.Sin(timer) * bobAmount;

            transform.localPosition = new Vector3(
                transform.localPosition.x,
                newY,
                transform.localPosition.z
            );
        }
        else
        {
            timer = 0f;
            transform.localPosition = Vector3.Lerp(
                transform.localPosition,
                new Vector3(
                    transform.localPosition.x, defaultYPos, transform.localPosition.z),
                Time.deltaTime * 5f
            );
        }
    }
}
