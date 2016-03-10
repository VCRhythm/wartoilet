#define RAGDOLLCONTROL
#define SIMPLEFOOTIK
using System;
using UnityEngine;

namespace AnimFollow
{
	public class AnimFollow_AF : MonoBehaviour
	{
		// Add this script to the ragdoll
		
#region Fields

#if RAGDOLLCONTROL
		RagdollController ragdollControl;
#endif
		public GameObject master; // ASSIGN IN INSPECTOR!
		private Transform[] masterTransforms; // These are all auto assigned
        private Transform[] masterRigidTransforms = new Transform[1];
        private Transform[] slaveTransforms;
        private Rigidbody[] slaveRigidbodies = new Rigidbody[1];
        private Vector3[] rigidbodiesPosToCOM;
        [SerializeField] private Transform[] slaveRigidTransforms = new Transform[1];
        public Transform[] slaveExcludeTransforms;

        private Quaternion[] localRotations1 = new Quaternion[1];
        private Quaternion[] localRotations2 = new Quaternion[1];

		public float fixedDeltaTime = 0.01f; // If you choose to go to longer times you need to lower PTorque, PLocalTorque and PForce or the system gets unstable. Can be done, longer time is better performance but worse mimicking of master.
        private float reciFixedDeltaTime; // 1f / fixedDeltaTime
				
		// The ranges are not set in stone. Feel free to extend the ranges
		[Range(0f, 100f)] public float maxTorque = 100f; // Limits the world space torque
		[Range(0f, 100f)] public float maxForce = 100f; // Limits the force
		[Range(0f, 10000f)] public float maxJointTorque = 10000f; // Limits the force
		[Range(0f, 10f)] public float jointDamping = .6f; // Limits the force

		public float[] maxTorqueProfile = {100f, 100f, 100f, 100f, 100f, 100f, 100f, 100f, 100f, 100f, 100f, 100f}; // Individual limits per limb
		public float[] maxForceProfile = {1f, .2f, .2f, .2f, .2f, 1f, 1f, .2f, .2f, .2f, .2f, .2f};
		public float[] maxJointTorqueProfile = {1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f};
		public float[] jointDampingProfile = {1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f};

		[Range(0f, .64f)] public float PTorque = .16f; // For all limbs Torque strength
		[Range(0f, 160f)] public float PForce = 30f;
		
		[Range(0f, .008f)] public float DTorque = .002f; // Derivative multiplier to PD controller
		[Range(0f, .064f)] public float DForce = .01f;
		
	//	public float[] PTorqueProfile = {20f, 30f, 10f, 30f, 10f, 30f, 30f, 30f, 10f, 30f, 10f}; // Per limb world space torque strength
		public float[] PTorqueProfile = {20f, 30f, 10f, 30f, 10f, 30f, 30f, 30f, 30f, 10f, 30f, 10f}; // Per limb world space torque strength for EthanRagdoll_12 (twelve rigidbodies)
		public float[] PForceProfile = {1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f};
		
		// The ranges are not set in stone. Feel free to extend the ranges
		[Range(0f, 340f)] public float angularDrag = 100f; // Rigidbodies angular drag. Unitys parameter
		[Range(0f, 2f)] public float drag = .5f; // Rigidbodies drag. Unitys parameter
        private float maxAngularVelocity = 1000f; // Rigidbodies maxAngularVelocity. Unitys parameter
		
		[SerializeField] bool torque = false; // Use World torque to controll the ragdoll (if true)
		[SerializeField] bool force = true; // Use force to controll the ragdoll
		[HideInInspector] public bool mimicNonRigids = true; // Set all local rotations of the transforms without rigidbodies to match the local rotations of the master
		[HideInInspector] [Range(2, 100)] public int secondaryUpdate = 2;
        private int frameCounter;
		public bool hideMaster = true;
		public bool useGravity = true; // Ragdoll is affected by Unitys gravity

        private float torqueAngle; // Återanvänds för localTorque, därför ingen variabel localTorqueAngle
        private Vector3 torqueAxis;
        private Vector3 torqueError;
        private Vector3 torqueSignal;
        private Vector3[] torqueLastError = new Vector3[1];
        private Vector3 torqueVelError;
		[HideInInspector] public Vector3 totalTorqueError; // Total world space angular error of all limbs. This is a vector.

        private Vector3 forceAxis;
        private Vector3 forceSignal;
        private Vector3 forceError;
        private Vector3[] forceLastError = new Vector3[1];
        private Vector3 forceVelError;
		[HideInInspector] public Vector3 totalForceError; // Total world position error. a vector.
		public float[] forceErrorWeightProfile = {1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f}; // Per limb error weight

        private float masterAngVel;
        private Vector3 masterAngVelAxis;
        private float slaveAngVel;
        private Vector3 slaveAngVelAxis;
        private Quaternion masterDeltaRotation;
        private Quaternion slaveDeltaRotation;
        private Quaternion[] lastMasterRotation = new Quaternion[1];
        private Quaternion[] lastSlaveRotation = new Quaternion[1];
        private Quaternion[] lastSlavelocalRotation = new Quaternion[1];
        private Vector3[] lastMasterPosition = new Vector3[1];
        private Vector3[] lastSlavePosition = new Vector3[1];

        private Quaternion[] startLocalRotation = new Quaternion[1];
        private ConfigurableJoint[] configurableJoints = new ConfigurableJoint[1];
        private Quaternion[] localToJointSpace = new Quaternion[1];
        private JointDrive jointDrive = new JointDrive();

        public bool isStunned = false;
#endregion

		void Awake()
		{
			Time.fixedDeltaTime = fixedDeltaTime; // Set the physics loop update intervall
			reciFixedDeltaTime = 1f / fixedDeltaTime; // Cache the reciprocal
			
			if (hideMaster)
			{
                HideMaster();
			}

#if RAGDOLLCONTROL
            ragdollControl = GetComponent<RagdollController>();
#endif

            slaveTransforms = GetComponentsInChildren<Transform>(); // Get all transforms in ragdoll. THE NUMBER OF TRANSFORMS MUST BE EQUAL IN RAGDOLL AS IN MASTER!
			masterTransforms = master.GetComponentsInChildren<Transform>(); // Get all transforms in master. 
			Array.Resize(ref localRotations1, slaveTransforms.Length);
			Array.Resize(ref localRotations2, slaveTransforms.Length);
			Array.Resize(ref rigidbodiesPosToCOM, slaveTransforms.Length);

            ResizeArrays();
			
            SortTransformArrays();

            CheckSetup();
		}

		void Start()
		{
            SetRigidbodyParameters();
		}

#if RAGDOLLCONTROL && !SIMPLEFOOTIK
		void FixedUpdate ()
		{
			DoAnimFollow();
		}
#endif

		public void DoAnimFollow()
		{
#if RAGDOLLCONTROL
			ragdollControl.DoRagdollControl();

            if (ragdollControl.stayDown && ragdollControl.shotInHead)
            {
                return;
            }
#endif
			totalTorqueError = Vector3.zero;
			totalForceError = Vector3.zero;

			if (frameCounter % secondaryUpdate == 0)
			{
                if (mimicNonRigids)
                {
                    MatchLocalRotations2ToMaster();
                }
				SetJointTorque (maxJointTorque, jointDamping);
			}

			if (frameCounter % 2 == 0)
			{
                MatchSlaveTransformRotationsToMaster();
            }

			for (int i = 0; i < slaveRigidTransforms.Length; i++) // Do for all rigid bodies
			{
                if (isStunned && i < 8) continue;

				slaveRigidbodies[i].angularDrag = angularDrag; // Set rigidbody drag and angular drag in real-time
				slaveRigidbodies[i].drag = drag;

				if (torque)
				{
                    CalculateAndApplyWorldTorque(i);
				}

                CalculateForceError(i);
				
				if (force)
				{
                    CalculateAndApplyWorldForce(i);
				}

                if (i > 0)
                {
                    configurableJoints[i].targetRotation = Quaternion.Inverse(localToJointSpace[i]) * Quaternion.Inverse(masterRigidTransforms[i].localRotation) * startLocalRotation[i];
                }
			}
			frameCounter++;
            
		}

		public void SetJointTorque (float positionSpring, float positionDamper)
		{
			for (int i = 1; i < configurableJoints.Length; i++) // Do for all configurable joints
			{
				jointDrive.positionSpring = positionSpring * maxJointTorqueProfile[i];
				jointDrive.positionDamper = positionDamper * jointDampingProfile[i];
				configurableJoints[i].slerpDrive = jointDrive;
			}
			maxJointTorque = positionSpring;
			jointDamping = positionDamper;
		}
		
		public void SetJointTorque (float positionSpring)
		{
			for (int i = 1; i < configurableJoints.Length; i++) // Do for all configurable joints
			{
				jointDrive.positionSpring = positionSpring * maxJointTorqueProfile[i];
				configurableJoints[i].slerpDrive = jointDrive;
			}
			maxJointTorque = positionSpring;
		}
			
		public void EnableJointLimits (bool jointLimits)
		{
			for (int i = 1; i < configurableJoints.Length; i++) // Do for all configurable joints
			{
				if (jointLimits)
				{
					configurableJoints[i].angularXMotion = ConfigurableJointMotion.Limited;
					configurableJoints[i].angularYMotion = ConfigurableJointMotion.Limited;
					configurableJoints[i].angularZMotion = ConfigurableJointMotion.Limited;
				}
				else
				{
					configurableJoints[i].angularXMotion = ConfigurableJointMotion.Free;
					configurableJoints[i].angularYMotion = ConfigurableJointMotion.Free;
					configurableJoints[i].angularZMotion = ConfigurableJointMotion.Free;
				}
			}
		}

		private float FixEuler (float angle) // For the angle in angleAxis, to make the error a scalar
		{
            if (angle > 180f)
            {
                return angle - 360f;
            }
            else
            {
                return angle;
            }
		}
		
		public static void PDControl (float P, float D, out Vector3 signal, Vector3 error, ref Vector3 lastError, float reciDeltaTime) // A PD controller
		{
			// theSignal = P * (theError + D * theDerivative) This is the implemented algorithm.
			signal = P * (error + D * ( error - lastError ) * reciDeltaTime);
			lastError = error;
		}

        private void ResizeArrays()
        {
            slaveRigidbodies = GetComponentsInChildren<Rigidbody>();
            int j = slaveRigidbodies.Length;

            System.Array.Resize(ref masterRigidTransforms, j);
            System.Array.Resize(ref slaveRigidTransforms, j);

            System.Array.Resize(ref maxTorqueProfile, j);
            System.Array.Resize(ref maxForceProfile, j);
            System.Array.Resize(ref maxJointTorqueProfile, j);
            System.Array.Resize(ref jointDampingProfile, j);
            System.Array.Resize(ref PTorqueProfile, j);
            System.Array.Resize(ref PForceProfile, j);
            System.Array.Resize(ref forceErrorWeightProfile, j);

            System.Array.Resize(ref torqueLastError, j);
            System.Array.Resize(ref forceLastError, j);

            System.Array.Resize(ref lastMasterRotation, j);
            System.Array.Resize(ref lastSlaveRotation, j);
            System.Array.Resize(ref lastSlavelocalRotation, j);
            System.Array.Resize(ref lastMasterPosition, j);
            System.Array.Resize(ref lastSlavePosition, j);

            System.Array.Resize(ref startLocalRotation, j);
            System.Array.Resize(ref configurableJoints, j);
            System.Array.Resize(ref localToJointSpace, j);
        }

        private void SortTransformArrays()
        {
            int i = 0;
            int j = 0;
            int k = 0;
            int numOfConfigurableJoints = 0;

            foreach (Transform slaveTransform in slaveTransforms) // Sort the transform arrays
            {
                if (slaveTransform.GetComponent<Rigidbody>())
                {
                    slaveRigidTransforms[j] = slaveTransform;
                    masterRigidTransforms[j] = masterTransforms[i];

                    if (slaveTransform.GetComponent<ConfigurableJoint>())
                    {
                        configurableJoints[j] = slaveTransform.GetComponent<ConfigurableJoint>();
                        Vector3 forward = Vector3.Cross(configurableJoints[j].axis, configurableJoints[j].secondaryAxis);
                        Vector3 up = configurableJoints[j].secondaryAxis;
                        localToJointSpace[j] = Quaternion.LookRotation(forward, up);
                        startLocalRotation[j] = slaveTransform.localRotation * localToJointSpace[j];
                        jointDrive = configurableJoints[j].slerpDrive;
                        configurableJoints[j].slerpDrive = jointDrive;
                        numOfConfigurableJoints++;
                    }
                    else if (j > 0)
                    {
                        Debug.LogWarning("Rigidbody " + slaveTransform.name + " on " + name + " is not connected to a configurable joint" + "\n");
                        return;
                    }

                    rigidbodiesPosToCOM[j] = Quaternion.Inverse(slaveTransform.rotation) * (slaveTransform.GetComponent<Rigidbody>().worldCenterOfMass - slaveTransform.position);
                    j++;
                }
                else
                {
                    bool excludeBool = false;

                    foreach (Transform exclude in slaveExcludeTransforms)
                    {
                        if (slaveTransform == exclude)
                        {
                            excludeBool = true;
                            break;
                        }
                    }

                    if (!excludeBool)
                    {
                        slaveTransforms[k] = slaveTransform;
                        masterTransforms[k] = masterTransforms[i];
                        localRotations1[k] = slaveTransform.localRotation;
                        k++;
                    }
                }
                i++;
            }

            localRotations2 = localRotations1;
            System.Array.Resize(ref masterTransforms, k);
            System.Array.Resize(ref slaveTransforms, k);
            System.Array.Resize(ref localRotations1, k);
            System.Array.Resize(ref localRotations2, k);

            if (numOfConfigurableJoints == 0)
            {
                Debug.LogWarning("There are no configurable joints on the ragdoll " + name + "\nDrag and drop the ReplaceJoints script on the ragdoll." + "\n");
                return;
            }
            else
            {
                SetJointTorque(maxJointTorque);
                EnableJointLimits(false);
            }
        }

        private void SetRigidbodyParameters()
        {
            for(int i = 0; i < slaveRigidTransforms.Length; i++)
            {
                slaveRigidTransforms[i].GetComponent<Rigidbody>().useGravity = useGravity;
                slaveRigidTransforms[i].GetComponent<Rigidbody>().angularDrag = angularDrag;
                slaveRigidTransforms[i].GetComponent<Rigidbody>().drag = drag;
                slaveRigidTransforms[i].GetComponent<Rigidbody>().maxAngularVelocity = maxAngularVelocity;
            }
        }

        private void MatchLocalRotations2ToMaster()
        {
            for (int i = 2; i < slaveTransforms.Length - 1; i++)
            {
                localRotations2[i] = masterTransforms[i].localRotation;
            }
        }

        private void CheckSetup()
        {
            if (slaveRigidTransforms.Length == 0)
            {
                Debug.LogWarning("There are no rigid body components on the ragdoll " + name + "\n");
            }
            else if (slaveRigidTransforms.Length < 12)
            {
                Debug.Log("This version of AnimFollow works better with one extra colleder in the spine on " + name + "\n");
            }

            if (PTorqueProfile[PTorqueProfile.Length - 1] == 0f)
            {
                Debug.Log("The last entry in the PTorqueProfile is zero on " + name + ".\nIs that intentional?\nDrop ResizeProfiles on the ragdoll and adjust the values." + "\n");
            }

            if (slaveExcludeTransforms.Length == 0)
            {
                Debug.Log("Should you not assign some slaveExcludeTransforms to the AnimFollow script on " + this.name + "\n");
            }
        }

        private void MatchSlaveTransformRotationsToMaster()
        {
            for (int i = 2; i < slaveTransforms.Length - 1; i++)
            {
                if (secondaryUpdate > 2)
                {
                    localRotations1[i] = Quaternion.Lerp(localRotations1[i], localRotations2[i], 2f / secondaryUpdate);
                    slaveTransforms[i].localRotation = localRotations1[i];
                }
                else
                {
                    slaveTransforms[i].localRotation = localRotations2[i];
                }
            }
        }

        private void CalculateAndApplyWorldTorque(int i)
        {
            Quaternion targetRotation = masterRigidTransforms[i].rotation * Quaternion.Inverse(slaveRigidTransforms[i].rotation);
            targetRotation.ToAngleAxis(out torqueAngle, out torqueAxis);
            torqueError = FixEuler(torqueAngle) * torqueAxis;

            if (torqueAngle != 360f)
            {
                totalTorqueError += torqueError;
                PDControl(PTorque * PTorqueProfile[i], DTorque, out torqueSignal, torqueError, ref torqueLastError[i], reciFixedDeltaTime);
            }
            else
            {
                torqueSignal = new Vector3(0f, 0f, 0f);
            }

            torqueSignal = Vector3.ClampMagnitude(torqueSignal, maxTorque * maxTorqueProfile[i]);
            slaveRigidbodies[i].AddTorque(torqueSignal, ForceMode.VelocityChange); // Add torque to the limbs
        }

        private void CalculateAndApplyWorldForce(int i)
        {
            PDControl(PForce * PForceProfile[i], DForce, out forceSignal, forceError, ref forceLastError[i], reciFixedDeltaTime);
            forceSignal = Vector3.ClampMagnitude(forceSignal, maxForce * maxForceProfile[i]);
            slaveRigidbodies[i].AddForce(forceSignal, ForceMode.VelocityChange);
        }

        private void CalculateForceError(int i)
        {
            Vector3 masterRigidTransformsWCOM = masterRigidTransforms[i].position + masterRigidTransforms[i].rotation * rigidbodiesPosToCOM[i];
            forceError = masterRigidTransformsWCOM - slaveRigidTransforms[i].GetComponent<Rigidbody>().worldCenterOfMass; // Doesn't work if collider is trigger
            totalForceError += forceError * forceErrorWeightProfile[i];
        }

        private void HideMaster()
        {
            SkinnedMeshRenderer visible;
            MeshRenderer visible2;
            if (visible = master.GetComponentInChildren<SkinnedMeshRenderer>())
            {
                visible.enabled = false;
                SkinnedMeshRenderer[] visibles;
                visibles = master.GetComponentsInChildren<SkinnedMeshRenderer>();
                foreach (SkinnedMeshRenderer visiblen in visibles)
                {
                    visiblen.enabled = false;
                }
            }
            if (visible2 = master.GetComponentInChildren<MeshRenderer>())
            {
                visible2.enabled = false;
                MeshRenderer[] visibles2;
                visibles2 = master.GetComponentsInChildren<MeshRenderer>();
                foreach (MeshRenderer visiblen2 in visibles2)
                {
                    visiblen2.enabled = false;
                }
            }
        }

        public void UpdateHealth(int health)
        {
        }

        public void Stun(Vector3 position, int health)
        {
            isStunned = true;
        }

        public void UnStun()
        {
            isStunned = false;
        }

        public void Die(Vector3 position) {}

        public void Move(Vector3 moveVector) {}
    }
}