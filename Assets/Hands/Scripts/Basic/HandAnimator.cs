using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Passes floating point animation scalers
/// to an animation based on the float
/// values returned by the XR trigger and 
/// grip values. 
/// 
/// Script updated from Muddy Wolf tutorial
/// https://www.youtube.com/watch?v=pI8l42F6ZVc
/// </summary>
public class HandAnimator : MonoBehaviour
{
    public enum Hand { left, right}

    [Tooltip("Changes the speed of the hand animation")]
    [SerializeField] private float m_transitionSpeed = 5f;
    [Tooltip("The hand that input will be taken from")]
    [SerializeField] private Hand m_hand;

    [Header("Input Actions")]
    private Animator m_animator;
    [SerializeField] private InputAction m_gripAction;
    [SerializeField] private InputAction m_pointAction;

    [Header("Optional")]
    [Tooltip("If assigned, will change collider to trigger when gripping")]
    [SerializeField] private Collider m_collider;

    //List of fingers that should be animated when the grip button is pressed
    private readonly List<Finger> m_gripFingers = new List<Finger>()
    {
        new Finger(Finger.FingerType.Middle),
        new Finger(Finger.FingerType.Ring),
        new Finger(Finger.FingerType.Pinky)
    };

    //List of fingers that should be animated then the trigger is pressed
    private readonly List<Finger> m_pointFingers = new List<Finger>()
    {
        new Finger(Finger.FingerType.Index),
        new Finger(Finger.FingerType.Thumb)
    };

    /// <summary>
    /// Attempts to get the animator that is
    /// attached to the same object as this script
    /// </summary>
    private void Awake()
    {
        // Only override the controls if one is unassigned
        if (m_pointAction == null || m_pointAction == null)
        {
            switch (m_hand)
            {
                case Hand.left:
                    m_pointAction.AddBinding("<XRController>{LeftHand}/trigger");
                    m_gripAction.AddBinding("<XRController>{LeftHand}/grip");
                    break;
                case Hand.right:
                    m_pointAction.AddBinding("<XRController>{RightHand}/trigger");
                    m_gripAction.AddBinding("<XRController>{RightHand}/grip");
                    break;
            }
        }
        
        m_gripAction.Enable();
        m_pointAction.Enable();

        m_animator = GetComponent<Animator>();
    }

    /// <summary>
    /// Runs input and animate functions
    /// 
    /// note: could be changed to fixed update?
    /// </summary>
    private void Update()
    {
        SetFingerTargets(m_pointFingers, m_pointAction.ReadValue<float>());
        AnimateFinger(m_pointFingers);

        SetFingerTargets(m_gripFingers, m_gripAction.ReadValue<float>());
        AnimateFinger(m_gripFingers);

        if (m_gripAction.ReadValue<float>() > 0 && m_collider != null)
        {
            m_collider.isTrigger = true;
        }
        else
        {
            m_collider.isTrigger = false;
        }
    }

    /// <summary>
    /// Sends the fload values to the list of fingers
    /// </summary>
    /// <param name="fingers">The fingers the values are passed to</param>
    /// <param name="value">The float modifier that defines transition state of the finger</param>
    private void SetFingerTargets(List<Finger> fingers, float value)
    {
        foreach (Finger f in fingers)
        {
            f.TargetMod = value;
        }
    }

    /// <summary>
    /// Smooths the input values and sends the modifiers
    /// to the animator using the target fingers as
    /// keys for the animation masks 
    /// </summary>
    /// <param name="fingers"> A list of fingers to be animated </param>
    private void AnimateFinger(List<Finger> fingers)
    {
        if (m_animator == null)
        {
            Debug.LogError($"ERROR: Animator not assigned on object {gameObject.name}. Hands will not animate");
            return;
        }

        foreach (Finger f in fingers)
        {
            float time = m_transitionSpeed * Time.deltaTime;
            f.CurrentMod = Mathf.MoveTowards(f.CurrentMod, f.TargetMod, time);

            m_animator.SetFloat(f.GetFingerType(), f.CurrentMod);
        }
    }
}

/// <summary>
/// Class that represents an individual finger
/// </summary>
public partial class Finger
{
    public enum FingerType
    {
        None,
        Thumb,
        Index,
        Middle,
        Ring,
        Pinky
    }
    // The type of finger to modify
    private FingerType m_fingerType = FingerType.None;

    //The target animation modifier for the finger
    public float TargetMod { get; set; }

    //the current target modifier for the finger
    public float CurrentMod { get; set; }

    //instantiating the finger
    public Finger(FingerType ft)
    {
        m_fingerType = ft;
    }

    /// <summary>
    /// Gets a string value of the finger type
    /// </summary>
    /// <returns>String name of the finger type</returns>
    public string GetFingerType()
    {
        return m_fingerType.ToString();
    }
}