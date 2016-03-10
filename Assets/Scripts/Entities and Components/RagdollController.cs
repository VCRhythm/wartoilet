#define SIMPLEFOOTIK
using UnityEngine;
using AnimFollow;

//	 This kind of a state machine that takes the character through the states: colliding, falling, matching the masters pose and getting back up
public class RagdollController : MonoBehaviour
{
    public bool isIdle = true;
    private AnimFollow_AF animFollow;

    #if SIMPLEFOOTIK
		    SimpleFootIK_AF simpleFootIK;
    #endif

    private Transform ragdollRootBone;              // A transform representative of the ragdoll position and rotation. ASSIGN IN INSPECTOR or it will be auto assigned to the first transform with a rigid body
    private GameObject master;                      // The master character that is originally controlled by animations. Auto assaigned
    private Rigidbody[] slaveRigidBodies;           // Contains all rigid bodies in the ragdoll. Only used to distribute the Limb scripts
    private Transform masterRootBone;               // A transform representative of the ragdoll position and rotation. Auto assaigned
    public Transform[] IceOnGetup;                  // Theese rigidbodies will get slipery during getup to avoid snagging
    public string[] ignoreCollidersWithTag = { "IgnoreMe" }; // Colliders with these tag will not affect the ragdolls strength

    [Range(10f, 170f)]
    public float getupAngularDrag = 50f;
    [Range(10f, 50f)]
    public float fallAngularDrag = 20f;
    [Range(5f, 85f)]
    public float getupDrag = 25f;

    [Range(.5f, 4.5f)]
    public float timeToLoseControlAfterColliding = 1.5f;
    [Range(0f, .2f)]
    public float torqueAfterCollision = 0f;
    [Range(0f, .2f)]
    public float residualForce = .1f;
    [Range(0f, 120f)]
    public float residualJointTorque = 120f;
    [Range(0f, 1f)]
    public float residualIdleFactor = 0f;   // Allows for lower residual strength if hit when in idle animation

    [Range(2f, 26f)]
    public float graceSpeed = 8f;           // The relative speed limit for a collision to make the character dose off
    [Range(.1f, 1.7f)]
    public float noGhostLimit = .5f;        // The Limit of limbError that is allowed before the character doses off, given certain conditions
    [Range(5f, 45f)]
    public float noGhostLimit2 = 15f;       // The Limit of limbError that is allowed before the character doses off, under all circumastances. This prevents you from going through walls like a ghost :)
    [Range(0f, 1.2f)]
    public float glideFree = .3f;           // makes the character glide free from an object if collision is not severe

    // These are shown in the inspector for you to get a feel for the states
    [ReadOnly] public bool falling = false;            // Is in falling state
    [ReadOnly] public bool gettingUp = false;          // Is in getUp state
    [ReadOnly] public bool jointLimits = false;

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    private Vector3 limbError;                      // Read from AnimFollow. Contains the total position error of the limbs

    // The parmeters below are parameters you might want to tune but they are not critical. Make them public if you want to mess with them. Uncomment first line of script. Changes not persistent if you undefine EXTRATUNING.
#if EXTRATUNING
	public bool fellOnSpeed = false;				// For tuning. Tells the reason the fall was triggered

	public float limbErrorMagnitude;				// This may be interesting to see if you are tuning the noGhostLimits. Read from AnimFollow. Contains the magnitude of the total position error of the limbs. When this is above the noGhostLimit falling is triggered

	[Range(0f, .4f)] public float settledSpeed = .2f;				// When ragdollRootBoone goes below this speed the falling state is through and the get up starts
	[Range(0f, .6f)] public float masterFallAnimatorSpeedFactor = .4f;	// Animator speed during transition to get up animations
	[Range(0f, .4f)] public float getup1AnimatorSpeedFactor = .25f; 	// Animation speed during the initial part of the get up state is getup1AnimatorSpeedFactor * animatorSpeed
	[Range(0f, 1f)] public float getup2AnimatorSpeedFactor = .65f; 	// Animation speed during the later part of the get up state is getup1AnimatorSpeedFactor * animatorSpeed

	[Range(0f, 10f)] public float contactTorque = 1f;				// The torque when in contact with other colliders
	[Range(0f, 10f)] public float contactForce = 2f;
	[Range(0f, 50000f)] public float contactJointTorque = 1000f;

	[Range(.04f, .48f)] public float getupLerp1 = .15f;		// Determines the initial regaining of strength after the character fallen to ease the ragdoll to the masters pose
	[Range(.5f, 6.5f)] public float getupLerp2 = 2f;			// Determines the regaining of strength during the later part of the get up state
	[Range(.05f, .65f)] public float wakeUpStrength = .2f;		// A number that defines the degree of strength the ragdoll must reach before it is assumed to match the master pose and start the later part of the get up state

	[Range(0f, 700f)] public float toContactLerp = 70f;				// Determines how fast the character loses strength when in contact
	[Range(0f, 10f)] public float fromContactLerp = 1f;				// Determines how fast the character gains strength after freed from contact
		
	[Range(0f, 100f)] public float maxTorque = 100f;				// The torque when not in contact with other colliders
	[Range(0f, 100f)] public float maxForce = 100f;
	[Range(0f, 10000f)] public float maxJointTorque = 10000f;

	[Range(0f, 1f)] public float maxErrorWhenMatching = .1f;		// The limit of error acceptable to consider the ragdoll to be matching the master. Is condition for going to normal operation after getting up
#else
    [ReadOnly] public bool fellBecauseOfSpeed = false;
    private float limbErrorMagnitude;               // This may be interesting to see if you are tuning the noGhostLimits. Read from AnimFollow. Contains the magnitude of the total position error of the limbs. When this is above the noGhostLimit falling is triggered

    private const float settledSpeed = .1f;               // When ragdollRootBoone goes below this speed the falling state is through and the get up starts
    private const float masterFallAnimatorSpeedFactor = .4f;  // Animator speed during transition to get up animations
    private const float getup1AnimatorSpeedFactor = .35f;     // Animation speed during the initial part of the get up state is getup1AnimatorSpeedFactor * animatorSpeed
    private const float getup2AnimatorSpeedFactor = .85f;     // Animation speed during the later part of the get up state is getup1AnimatorSpeedFactor * animatorSpeed

    private const float contactTorque = 1f;               // The torque when in contact with other colliders
    private const float contactForce = 2f;
    private const float contactJointTorque = 1000f;

    private const float getupLerp1 = .15f;        // Determines the initial regaining of strength after the character fallen to ease the ragdoll to the masters pose
    private const float getupLerp2 = 2f;          // Determines the regaining of strength during the later part of the get up state
    private const float wakeUpStrength = .2f;     // A number that defines the degree of strength the ragdoll must reach before it is assumed to match the master pose and start the later part of the get up state

    private const float toContactLerp = 70f;              // Determines how fast the character loses strength when in contact
    private const float fromContactLerp = 1f;             // Determines how fast the character gains strength after freed from contact

    private const float maxTorque = 100f;                 // The torque when not in contact with other colliders
    private const float maxForce = 100f;
    private const float maxJointTorque = 10000f;

    private const float maxErrorWhenMatching = .1f;       // The limit of error acceptable to consider the ragdoll to be matching the master. Is condition for going to normal operation after getting up
#endif
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    [HideInInspector]
    public float orientateY = 0f;       // The world y-coordinate the master transform will be at after a fall. If you move your character vertically you want to set this to match
    [HideInInspector]
    public float collisionSpeed;        // The relative speed of the colliding collider
    [HideInInspector]
    public int numberOfCollisions;  // Number of colliders currently in contact with the ragdoll
    private float animatorSpeed;                    // Read from the PlayerMovement script
    private int secondaryUpdateSet;                 // Read from the AnimFollow script
    private float[] noIceDynFriction;               // To save user settings of friction
    private float[] noIceStatFriction;
    private float drag;                             // Read from the AnimFollow script
    private float angularDrag;

    // These parameters are not for tuning
    private float contactTime;                      // The game time since last collision (zero when not in contact with other colliders)
    private float noContactTime = 10f;                  // The game time since last the ragdoll was in contact with another collider (zero when in contact)
    public bool shotByBullet = false;
    [ReadOnly]
    public bool userNeedsToAssignStuff = false; // If this is true then ....
    private bool delayedGetupDone = false;          // Used to delay setting gettingUp to false if still in contakt after get up state
    private bool localTorqUserSetting;              // Saves the user setting from AnimFollow
    private bool orientate = false;                 // starts the process of matching the ragdoll to the master
    private bool orientated = true;                 // starts the process of matching the ragdoll to the master
    private bool getupState = false;                // Not used in this version
    private bool isInTransitionToGetup = false;
    private bool wasInTransitionToGetup = false;
    public bool stayDown = false;
    [HideInInspector]
    public bool shotInHead = false;
    private ulong i;

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    void Awake() // Initialize
    {
        SetUpComponents();

        secondaryUpdateSet = animFollow.secondaryUpdate;

        animFollow.maxTorque = maxTorque; // Set the maxTorque in the AnimFollow script. This overrides the settings in AnimFollow
        animFollow.maxForce = maxForce;
        animFollow.maxJointTorque = maxJointTorque;

        slaveRigidBodies = GetComponentsInChildren<Rigidbody>(); // Get all rigid bodies
        foreach (Rigidbody slaveRigidBody in slaveRigidBodies) // Distribute limb script
        {
            slaveRigidBody.gameObject.AddComponent<Limb_AF>(); // Destribute a collision scripts to all limbs. This script reports to RagdollControll if any limb is in contact with another collider
        }

        System.Array.Resize(ref noIceDynFriction, IceOnGetup.Length);
        System.Array.Resize(ref noIceStatFriction, IceOnGetup.Length);
        for (int m = 0; m < IceOnGetup.Length; m++)
        {
            noIceDynFriction[m] = IceOnGetup[m].GetComponent<Collider>().material.dynamicFriction;
            noIceStatFriction[m] = IceOnGetup[m].GetComponent<Collider>().material.staticFriction;
        }
        drag = animFollow.drag;
        angularDrag = animFollow.angularDrag;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    public void DoRagdollControl() // Needs to be synced with AnimFollow
    {
        if (stayDown && shotInHead)
        {
            StayDeadOnHeadShot();
            return;
        }

        i++;

        limbError = animFollow.totalForceError; // Get Ragdoll distortion from AnimFollow
        limbErrorMagnitude = limbError.magnitude;

        // The code below first checks if we are hit with enough force to fall and then do:
        // inhibit movements in PlayerMovement script, falling, orientate master. ease ragdoll to master pose, play getup animation, go to full strength and anable movements again. 

        // Fall if: we hit with high enough speed or if the distortion of the character to large
        if (shotByBullet || numberOfCollisions > 0 && (collisionSpeed > graceSpeed || (!(gettingUp || falling) && limbErrorMagnitude > noGhostLimit)) || (limbErrorMagnitude > noGhostLimit2 && orientated))
        {
            if (!falling)
            {
                Fall();
            }

            shotByBullet = false;
            falling = true;
            gettingUp = false;
            orientated = false;
            delayedGetupDone = false;
            fellBecauseOfSpeed = numberOfCollisions > 0 && collisionSpeed > graceSpeed; // For tuning. If you want to know if the fall was triggered by the speed of the collision
        }
        else if (falling || gettingUp) // Code does not run in normal operation
        {
            if (gettingUp)
            {
                GetUp();
            }
            else if(falling)
            {
                StartGettingUp();
            }
        }

        collisionSpeed = 0f; // Reset to zero

        // The code below is run also in normal operation (not falling or getting up)

        // Check if we are in contact with other colliders
        if (numberOfCollisions == 0) // Not in contact
        {
            noContactTime += Time.fixedDeltaTime;
            contactTime = 0f;

            // When not in contact character has maxStrenth strength
            if (!(gettingUp || falling) || delayedGetupDone)
            {
                animFollow.maxTorque = Mathf.Lerp(animFollow.maxTorque, maxTorque, fromContactLerp * Time.fixedDeltaTime);
                animFollow.maxForce = Mathf.Lerp(animFollow.maxForce, maxForce, fromContactLerp * Time.fixedDeltaTime);
                animFollow.maxJointTorque = Mathf.Lerp(animFollow.maxJointTorque, maxJointTorque, fromContactLerp * Time.fixedDeltaTime);
            }
        }
        else // In contact
        {
            contactTime += Time.fixedDeltaTime;
            noContactTime = 0f;

            // When in contact character has only contact strength
            if (!(gettingUp || falling) || delayedGetupDone)
            {
                animFollow.maxTorque = Mathf.Lerp(animFollow.maxTorque, contactTorque, toContactLerp * Time.fixedDeltaTime);
                animFollow.maxForce = Mathf.Lerp(animFollow.maxForce, contactForce, toContactLerp * Time.fixedDeltaTime);
                animFollow.maxJointTorque = Mathf.Lerp(animFollow.maxJointTorque, contactJointTorque, toContactLerp * Time.fixedDeltaTime);
            }
        }

        //playerMovement.glideFree = AdjustPlayerMovementsForLargeDistortion();
    }

    private void SetUpComponents()
    {
        animFollow = GetComponent<AnimFollow_AF>();
        master = animFollow.master;

#if SIMPLEFOOTIK
        simpleFootIK = master.GetComponent<SimpleFootIK_AF>();
#endif

        if (!ragdollRootBone)
        {
            ragdollRootBone = GetComponentInChildren<Rigidbody>().transform;
        }
        else if (!ragdollRootBone.GetComponent<Rigidbody>() || !(ragdollRootBone.root == this.transform.root))
        {
            ragdollRootBone = GetComponentInChildren<Rigidbody>().transform;
        }

        int i = 0;
        Transform[] transforms = GetComponentsInChildren<Transform>();
        foreach (Transform transformen in transforms)  // Find the masterRootBoone
        {
            if (transformen == ragdollRootBone)
            {
                masterRootBone = master.GetComponentsInChildren<Transform>()[i];
                break;
            }
            i++;
        }
    }

    private Vector3 AdjustPlayerMovementsForLargeDistortion()
    {
        // Adjust player movements if ragdoll distortion is large, e.g. if we are walking into a wall
        if (noContactTime < .3f && !(gettingUp || falling))
        {
            return new Vector3(-limbError.x, 0f, -limbError.z) * glideFree;
        }

        return Vector3.zero;
    }

    private void StayDeadOnHeadShot()
    {
        animFollow.maxTorque = 0f; // Go total ragdoll
        animFollow.maxForce = 0f;
        animFollow.maxJointTorque = 0f;
        animFollow.SetJointTorque(animFollow.maxJointTorque); // Do not wait for animfollow.secondaryUpdate
        animFollow.angularDrag = angularDrag;
        animFollow.drag = drag;
        simpleFootIK.userNeedsToFixStuff = true; // Just disabling footIK

        Renderer ragdollRenderer = transform.GetComponentInChildren<Renderer>();
        if (ragdollRenderer && !ragdollRenderer.isVisible)
        {
            Destroy(transform.root.gameObject);
        }
    }

    private void Fall()
    {
        // The initial strength immediately after the impact
        if (!isIdle && !getupState) // If not in idle state
        {
            animFollow.maxTorque = torqueAfterCollision;
            animFollow.maxForce = residualForce;
            animFollow.maxJointTorque = residualJointTorque;
            animFollow.SetJointTorque(residualJointTorque); // Do not wait for animfollow.secondaryUpdate
        }
        else // If was in Idle state
        {
            animFollow.maxTorque = torqueAfterCollision * residualIdleFactor;
            animFollow.maxForce = residualForce * residualIdleFactor;
            animFollow.maxJointTorque = residualJointTorque * residualIdleFactor;
            animFollow.SetJointTorque(animFollow.maxJointTorque); // Do not wait for animfollow.secondaryUpdate
        }

        animFollow.EnableJointLimits(true);
        jointLimits = true;
        animFollow.secondaryUpdate = 100;
        for (int m = 0; m < IceOnGetup.Length; m++) // turn of iceOnGetup
        {
            IceOnGetup[m].GetComponent<Collider>().material.dynamicFriction = noIceDynFriction[m];
            IceOnGetup[m].GetComponent<Collider>().material.staticFriction = noIceStatFriction[m];
        }
        animFollow.angularDrag = fallAngularDrag;
        animFollow.drag = drag;
    }

    private void GetUp()
    {
        // Wait until transition to getUp is done so that the master animation is lying down before orientating the master to the ragdoll rotation and position
        if (orientate && !isInTransitionToGetup && wasInTransitionToGetup)
        {
            falling = false;

            // Here the master gets reorientated to the ragdoll which could have ended its fall in any direction and position
            master.transform.rotation = ragdollRootBone.rotation * Quaternion.Inverse(masterRootBone.rotation) * master.transform.rotation;
            master.transform.rotation = Quaternion.LookRotation(new Vector3(master.transform.forward.x, 0f, master.transform.forward.z), Vector3.up);
            master.transform.Translate(ragdollRootBone.position - masterRootBone.position, Space.World);
#if SIMPLEFOOTIK
            simpleFootIK.extraYLerp = .02f;
            simpleFootIK.leftFootPosition = ragdollRootBone.position + Vector3.up;
            simpleFootIK.rightFootPosition = ragdollRootBone.position + Vector3.up;
#else
            master.transform.position = new Vector3(master.transform.position.x, orientateY, master.transform.position.z);
#endif
            orientate = false; // Orientation is now done
            orientated = true;
            for (int m = 0; m < IceOnGetup.Length; m++) // Turn On iceOnGetup
            {
                IceOnGetup[m].GetComponent<Collider>().material.dynamicFriction = 0f;
                IceOnGetup[m].GetComponent<Collider>().material.staticFriction = 0f;
            }
            animFollow.angularDrag = getupAngularDrag;
            animFollow.drag = getupDrag;
        }

        if (orientated)
        {
            if (animFollow.maxTorque < wakeUpStrength) // Ease the ragdoll to the master pose. WakeUpStrength limit should be set so that the radoll just has reached the master pose
            {
                master.transform.Translate((ragdollRootBone.position - masterRootBone.position) * .5f, Space.World);

                animFollow.maxTorque = Mathf.Lerp(animFollow.maxTorque, contactTorque, getupLerp1 * Time.fixedDeltaTime); // We now start lerping the strength back to the ragdoll. Do until strength is wakeUpStrength. Animation is running wery slowly
                animFollow.maxForce = Mathf.Lerp(animFollow.maxForce, contactForce, getupLerp1 * Time.fixedDeltaTime);
                animFollow.maxJointTorque = Mathf.Lerp(animFollow.maxJointTorque, contactJointTorque, getupLerp1 * Time.fixedDeltaTime);
                animFollow.secondaryUpdate = 20;
            }
            else if (!(isInTransitionToGetup || getupState)) // Getting up is done. We are back in Idle (if not delayed)
            {
#if SIMPLEFOOTIK
                simpleFootIK.extraYLerp = 1f;
#endif
                animFollow.angularDrag = angularDrag;
                animFollow.drag = drag;
                animFollow.secondaryUpdate = secondaryUpdateSet;

                for (int m = 0; m < IceOnGetup.Length; m++) // turn of iceOnGetup
                {
                    IceOnGetup[m].GetComponent<Collider>().material.dynamicFriction = noIceDynFriction[m];
                    IceOnGetup[m].GetComponent<Collider>().material.staticFriction = noIceStatFriction[m];
                }

                if (limbErrorMagnitude < maxErrorWhenMatching) // Do not go to full strength unless ragdoll is matching master (delay)
                {
                    gettingUp = false; // Getting up is done
                    delayedGetupDone = false;
                }
                else
                {
                    delayedGetupDone = true;
                }
            }
            else // Lerp the ragdoll to contact strength during get up
            {
                animFollow.maxTorque = Mathf.Lerp(animFollow.maxTorque, contactTorque, getupLerp2 * Time.fixedDeltaTime);
                animFollow.maxForce = Mathf.Lerp(animFollow.maxForce, contactForce, getupLerp2 * Time.fixedDeltaTime);
                animFollow.maxJointTorque = Mathf.Lerp(animFollow.maxJointTorque, contactJointTorque, getupLerp2 * Time.fixedDeltaTime);
                animFollow.secondaryUpdate = secondaryUpdateSet * 2;
                if (jointLimits)
                {
                    animFollow.EnableJointLimits(false);
                    jointLimits = false;
                }
            }
        }
    }

    private void StartGettingUp()
    {
        // Lerp force to zero from residual values
        animFollow.maxTorque = Mathf.Lerp(animFollow.maxTorque, 0f, timeToLoseControlAfterColliding * Time.fixedDeltaTime);
        animFollow.maxForce = Mathf.Lerp(animFollow.maxForce, 0f, timeToLoseControlAfterColliding * Time.fixedDeltaTime);
        animFollow.maxJointTorque = Mathf.Lerp(animFollow.maxJointTorque, 0f, timeToLoseControlAfterColliding * Time.fixedDeltaTime);
        animFollow.SetJointTorque(animFollow.maxJointTorque); // Do not wait for animfollow.secondaryUpdate

        // Orientate master to ragdoll and start transition to getUp when settled on the ground. Falling is over, getting up commences
        if (ragdollRootBone.GetComponent<Rigidbody>().velocity.magnitude < settledSpeed) // && contactTime + noContactTime > .4f)
        {
            gettingUp = true;
            orientate = true;
            //playerMovement.inhibitMove = true;
            animFollow.maxTorque = 0f; // These strengths shold be zero to avoid twitching during orientation
            animFollow.maxForce = 0f;
            animFollow.maxJointTorque = 0f;
            //animator.SetFloat(hash.speedFloat, 0f, 0f, Time.fixedDeltaTime);
        }
    }
}