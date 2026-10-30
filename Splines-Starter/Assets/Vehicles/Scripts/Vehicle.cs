using UnityEngine;
using UnityEngine.InputSystem;

/*
   A raycast car: one Rigidbody pushed by four hubs, one at each corner.

   Each physics step, every hub casts a ray down from itself to find the road. If it hits, the hub
   works out three forces, each along one of its own axes:
     suspension  along up       holds this corner off the road (a spring plus a damper)
     grip        along right    pushes against the tire sliding sideways
     drive       along forward  pushes the car where the hub points
   It stores them on the hub (hub.SuspensionForce and so on) for VehicleVisualizations to draw,
   then pushes the car once with their sum, at the hub's position. Pushing at a corner instead of
   the center is what tilts and turns the body; there is no rotation code.

   Steering adds no force. Update turns the front hubs, which turns their right and forward, and
   grip does the turning.

   Every force goes in FixedUpdate. Physics moves the car in its own steps, not once per frame, and
   FixedUpdate runs right before each step, so every step gets exactly one push.
*/
public class Vehicle : MonoBehaviour
{
    [Header("Car")]
    [SerializeField] Rigidbody carRigidBody;
    [SerializeField] VehicleHubData[] hubDataList;

    [Header("Suspension")]
    [SerializeField] float suspensionRestDist = 1f;
    [SerializeField] float raycastLength = 1.5f;
    [SerializeField] float stiffness = 30000f;
    [SerializeField] float damping = 1000f;
    [SerializeField] LayerMask groundLayers = ~0;

    [Header("Steering")]
    [SerializeField] float maxSteerAngle = 30f;

    [Header("Grip")]
    [SerializeField] float gripStrength = 750f;

    [Header("Drive")]
    [SerializeField] float driveForce = 2000f;

    float _driveInput;

    public VehicleHubData[] Hubs => hubDataList;

    void Update()
    {
        _driveInput = 0f;
        if (Keyboard.current.wKey.isPressed) _driveInput += 1f;
        if (Keyboard.current.sKey.isPressed) _driveInput -= 1f;

        float steerInput = 0f;
        if (Keyboard.current.dKey.isPressed) steerInput += 1f;
        if (Keyboard.current.aKey.isPressed) steerInput -= 1f;

        foreach (VehicleHubData hub in hubDataList)
        {
            if (hub.Steers)
                hub.transform.localRotation = Quaternion.Euler(0f, steerInput * maxSteerAngle, 0f);
        }
    }

    void FixedUpdate()
    {
        foreach (VehicleHubData hub in hubDataList)
        {
            // TODO Part 1, setup: zero all three hub forces, then cast a ray along the hub's down.
            // On a miss, skip to the next hub with no forces left from the previous step.

            // TODO Part 2, spring: set the wheel visual on the road, then push along the hub's up with the spring.

            // TODO Part 3, damper: also resist how fast this hub moves along its up.

            // TODO Part 4, drive: push along the hub's forward.

            // TODO Part 5, wheels: find the distance rolled this step from the hub's forward speed.
            // Divide by the wheel circumference for revolutions, then multiply by 360 for degrees and spin the wheel visual.

            // TODO Part 6, grip and steering: push against how fast this hub slides along its right.
            // gripStrength affects how strong the anti-slide force is (a damper turned sideways).
        }
    }
}
