using UnityEngine;

[CreateAssetMenu]
public class ScriptableStats : ScriptableObject
{
    [Header("LAYERS")]
    [Tooltip("Set this to the layer your player is on")]
    public LayerMask PlayerLayer;

    [Header("INPUT")]
    [Tooltip("Makes all Input snap to an integer. Prevents gamepads from walking slowly. Recommended value is true to ensure gamepad/keybaord parity.")]
    public bool SnapInput = true;

    [Tooltip("Minimum input required before you mount a ladder or climb a ledge. Avoids unwanted climbing using controllers"), Range(0.01f, 0.99f)]
    public float VerticalDeadZoneThreshold = 0.3f;

    [Tooltip("Minimum input required before a left or right is recognized. Avoids drifting with sticky controllers"), Range(0.01f, 0.99f)]
    public float HorizontalDeadZoneThreshold = 0.1f;

    [Header("RUN")]
    [Tooltip("The top horizontal movement speed")]
    public float MaxSpeed = 14;

    [Tooltip("The player's capacity to gain horizontal speed")]
    public float Acceleration = 120;

    [Tooltip("The pace at which the player comes to a stop")]
    public float GroundDeceleration = 60;

    [Tooltip("The amount of friction applied when no input is given")]
    public float FrictionAmount = 0.15f;

    [Tooltip("How much the current velocity affects the acceleration. Higher values make it harder to change direction quickly"), Range(0f, 1f)]
    public float VelocityPower = 0.9f;

    [Tooltip("The detection distance for grounding and roof detection"), Range(0f, 0.5f)]
    public float GrounderDistance = 0.05f;

    [Header("JUMP")]
    [Tooltip("The immediate velocity applied when jumping")]
    public float JumpPower = 36;

    [Tooltip("The immediate downward velocity applied when performing a jump cut (releasing jump early)")]
    public float JumpCutForce = 4;

    [Tooltip("How much weaker the double jump is compared to the first one (velocity = JumpPower * DoubleJumpPower)")]
    public float DoubleJumpPower = 0.75f;

    [Tooltip("The player's base gravity multiplier")]
    public float GravityScale = 6;

    [Tooltip("The gravity multiplier added when falling")]
    public float FallGravityMultiplier = 2.5f;

    [Tooltip("The time before coyote jump becomes unusable. Coyote jump allows jump to execute even after leaving a ledge")]
    public float CoyoteTime = .15f;

    [Tooltip("The amount of time we buffer a jump. This allows jump input before actually hitting the ground")]
    public float JumpBuffer = .2f;
}

