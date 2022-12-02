using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.SocialPlatforms;

public class OrbitalTargeting : MonoBehaviour
{
    [Header("Basic Settings:"), Tooltip("Actually updates the aim target position when enabled.")]
    public bool aimCalculationEnabled = true;
    [Tooltip("Enables and Disables various features to allow for easier setup.")]
    public bool setupMode = false;

    [Serializable]
    public class RequiredTransforms
    {
        [Tooltip("The transform that represents the position that will be compared against the aim target position to actually calculate the gyro angles.")]
        public Transform aimOriginLocation;
        [Tooltip("The exoskeleton's left hand")]
        public Transform leftHand;
        [Tooltip("The exoskeleton's left hand contrast point. This is usually the base of the middle finger.")]
        public Transform leftHandFingerContrastPoint;
        [Tooltip("The exoskeleton's right hand.")]
        public Transform rightHand;
        [Tooltip("The exoskeleton's right hand contrast point. This is usually the first joint of the middle finger.")]
        public Transform rightHandFingerContrastPoint;
        [Tooltip("Refer to the documentation for more information.")]
        public Transform leftHandTwistTarget;
    }

    [Header("Required Settings:")]
    public RequiredTransforms requiredTransforms;

    [Serializable]
    public class MeshSettings
    {
        [Tooltip("Refer to the documentation for more information.")]
        public bool hideOnStart = true;
        [Tooltip("Refer to the documentation for more information.")]
        public SkinnedMeshRenderer meshRenderer;
        [Tooltip("Refer to the documentation for more information.")]
        public Material invisibleMaterial;
    }

    [Header("Mesh Settings:"), Tooltip("Refer to the documentation for more information.")]
    public MeshSettings meshSettings;

    [Serializable]
    public class GyroParameters
    {

        [Serializable]
        public class XRotationLimit
        {
            [Range(-67.5f, 89), Tooltip("The degrees the character will be able to aim down relative to the straight forward aiming stance.")]
            public float lowerLimit = -67.5f;
            [Range(-67.5f, 89), Tooltip("The degrees the character will be able to aim up relative to the straight forward aiming stance.")]
            public float upperLimit = 89;
        }

        [Serializable]
        public class YRotationLimit
        {
            [Range(-180, 0), Tooltip("The degrees the character will be able to aim right relative to the straight forward aiming stance.")]
            public float lowerLimit = -180;
            [Range(0, 180), Tooltip("The degrees the character will be able to aim left relative to the straight forward aiming stance. NOTE: Please be aware that 180 degrees is not the natural animation seam. Refer to the documentation for more information.")]
            public float upperLimit = 180;
            [Range(0.1f, 44), Tooltip("The degrees, in the positive direction, relative to the natural animation seam the character will not be able to aim at. Refer to the documentation for more information.")]
            public float deadZonePositiveLimit = 0.1f;
            [Range(-44, -0.1f), Tooltip("The degrees, in the negative direction, relative to the natural animation seam the character will not be able to aim at. Refer to the documentation for more information.")]
            public float deadZoneNegativeLimit = -0.1f;
            [Range(0, 20), Tooltip("The degrees surrounding the natural animation seam that the target's position will start to be averaged to prevent torso snapping. Refer to the documentation for more information.")]
            public float snapReductionAngleLimit = 5;
            [Range(0,120), Tooltip("How many target positions will be averaged when within the snap reduction angle limit. Refer to the documentation for more information.")]
            public int snapReductionIntensity = 20;
        }

        [Serializable]
        public class GyroSpeed
        {
            [Tooltip("The movement speed of side to side aiming")]
            public float yGyroSpeed = 5;
            [Tooltip("The movement speed of up and down aiming")]
            public float xGyroSpeed = 5;
        }
        [Tooltip("The actual aiming speed in both directions.")]
        public GyroSpeed gyroSpeed = new GyroSpeed();
        [Tooltip("The side to side aiming limits. Refer to the documentation for more information.")]
        public YRotationLimit yGyroLimits = new YRotationLimit();
        [Tooltip("The up and down aiming limits")]
        public XRotationLimit xGyroLimits = new XRotationLimit();
    }

    [Header("Gyro Settings:"), Tooltip("This section contains aiming limits and other settings.")]
    public GyroParameters gyroParameters = new GyroParameters();

    [Serializable]
    public class LocalValues
    {

        public float yTargetAngle = 0;
        public float xTargetAngle = 0;
        public Transform xGyro;

        public Animator animator;

        public List<Vector3> targetLocations = new List<Vector3>();

        public GameObject gunLockLeft;
        public GameObject gunLockRight;
        [Range(0, 1)]
        public float leftHandContrastPercent = 0.5f;
        [Range(0, 1)]
        public float rightHandContrastPercent = 1f;
        public OrbitalWeapon orbitalWeapon;

        public float startupDelay = 0;

        public Vector2 apparentRecoil = Vector2.zero;
        public Vector2 currentRecoil= Vector2.zero;
    }

    public IOrbitalTargetProvider orbitalTargetProvider;

    [Serializable]
    public class DistanceCorrection
    {
        [Tooltip("Is distance correction enabled? Refer to the documentation for more information.")]
        public bool enabled = false;
        [Tooltip("The focal point of the distance correction calculation. Refer to the documentation for more information.")]
        public float FocalPoint = 20;
        [Tooltip("Distance correction intensity beyond the focal point. Refer to the documentation for more information.")]
        public float farsightedMultiplier = 0.03f;
        [Tooltip("Distance correction intensity in front of the focal point. Refer to the documentation for more information.")]
        public float nearsightedMultiplier = 0f;
    }
    [Header("Distance Correction Settings:"), Tooltip("Refer to the documentation for more information.")]
    public DistanceCorrection distanceCorrection = new DistanceCorrection();


    [Header("Debug: (Read-Only)"), Tooltip("Used for debug purposes only!")]
    public LocalValues localValues = new LocalValues();


    // Start is called before the first frame update
    void Start()
    {
        //Sets the animator if needed
        SetAnimator();
        //Finds the target provider on the same gameobject
        GetTargetProvider();
        //Creates the gameobject that will be used to calculate the X angles
        CreateXGyro();
        //Creates the weapon's bind points
        CreateGunLocks();
        //Sets the exoskeleton's material to make it invisible
        HideMesh();
        //Clears all recorded target locations
        localValues.targetLocations.Clear();
    }

    void SetAnimator()
    {
        //Sets the animator if needed
        localValues.animator = GetComponent<Animator>();
        if (localValues.animator == null)
        {
            Debug.LogError("Attached Gameobject is missing an 'Animator' component!");
        }
    }
    //Finds the target provider on the same gameobject
    void GetTargetProvider()
    {
        orbitalTargetProvider = GetComponent<IOrbitalTargetProvider>();
        if (orbitalTargetProvider == null)
        {
            Debug.LogError("Attached Gameobject is missing an 'Orbital Target Provider'!");
        }
    }
    //Creates the gameobject that will be used to calculate the X angles
    void CreateXGyro()
    {
        if (requiredTransforms.aimOriginLocation == null)
        {
            Debug.LogError("Set an 'Aim Origin Location'!");
            return;
        }

        GameObject newXGyro = new GameObject();
        newXGyro.name = "OrbitalAimGeneratedGyro";
        newXGyro.transform.position = requiredTransforms.aimOriginLocation.position;
        newXGyro.transform.rotation = requiredTransforms.aimOriginLocation.rotation;
        newXGyro.transform.parent = requiredTransforms.aimOriginLocation.parent;
        localValues.xGyro = newXGyro.transform;
    }
    //Creates the weapon's bind points
    void CreateGunLocks()
    {
        CreateGunLock(requiredTransforms.leftHand, requiredTransforms.leftHandFingerContrastPoint, localValues.leftHandContrastPercent, out localValues.gunLockLeft);
        CreateGunLock(requiredTransforms.rightHand, requiredTransforms.rightHandFingerContrastPoint, localValues.rightHandContrastPercent, out localValues.gunLockRight);
    }
    //Creates a weapon's bind point
    void CreateGunLock(Transform parent, Transform contrastPoint, float contrastPercent, out GameObject gLock)
    {
        GameObject newLock = new GameObject();
        newLock.name = "OrbitalAimGeneratedLock";
        Quaternion rotationFix = Quaternion.identity;
        rotationFix.eulerAngles = new Vector3(0, 0, 90);
        newLock.transform.rotation = parent.rotation *rotationFix;
        newLock.transform.position = Vector3.Lerp(parent.position, contrastPoint.position, contrastPercent);
        newLock.transform.parent = parent;
        gLock = newLock;
    }
    //Sets the exoskeleton's material to make it invisible
    void HideMesh()
    {
        if (meshSettings.hideOnStart)
        {
            if (meshSettings.meshRenderer == null)
            {
                Debug.LogWarning("Exoskeleton Mesh Render needs to be set!");
                return;
            }
            if (meshSettings.invisibleMaterial == null)
            {
                Debug.LogWarning("Exoskeleton invisible material needs to be set!");
                return;
            }
            meshSettings.meshRenderer.material = meshSettings.invisibleMaterial;
        }
    }
    //Updates the position and rotation of the gun lock
    void UpdateGunLock(Transform parent, Transform contrastPoint, float contrastPercent, GameObject gLock)
    {
        //Quaternion rotationFix = Quaternion.identity;
        //rotationFix.eulerAngles = new Vector3(0, -90, 0);
        //gLock.transform.localRotation = parent.rotation;


        gLock.transform.position = Vector3.Lerp(parent.position, contrastPoint.position, contrastPercent);
    }
    //Set's the gun's locked state. 
    public void SetGun(OrbitalWeapon weapon)
    {
        if (weapon == null) return;
        localValues.orbitalWeapon = weapon;
        if (localValues.gunLockLeft == null)
        {
            Debug.Log("Gun lock has not been spawned yet!");
            return;
        }
        localValues.orbitalWeapon.SetLockedGunTransform(localValues.gunLockLeft.transform);
    }
    //Set's the gun's unlocked state. 
    public void UnlatchGun(Transform newParent)
    {
        if (localValues.orbitalWeapon == null) return;
        localValues.orbitalWeapon.SetUnlockedGunTransform(newParent);
        localValues.orbitalWeapon = null;
    }

    // Update is called once per frame
    void Update()
    {
        //Makes sure some parameters have been correctly set or found
        if (!CheckPreReqs()) 
        {
            Debug.LogError("Orbital prerequisites are missing!");
            return;
        }
        //Updates the position and rotation of the gun lock
        UpdateGunLockTransforms();

        if (setupMode)
        {
            //Prevents the character from aiming in setup mode
            localValues.targetLocations.Insert(0, new Vector3(0, 0, -10000));
        } //Updates the target's location
        else if ( UpdateTargetLocation())
        {
            //Stops the aim calculation if the target provider failed to provide a location
            Debug.LogError("Orbital Provider failed to provide a target location!");
            aimCalculationEnabled = false;
            return;
        }
        //Checks if the character needs to be aiming
        if (localValues.targetLocations[0] != new Vector3(0,0,-10000))
        {
            //Calculates the gyro angles
            CalculateAimValues(localValues.targetLocations[0]);
            //Applies snap reduction if needed 
            Vector3 vectorOutput;
            if (ApplySnapReduction(out vectorOutput))
            {
                //Applies Distance Correction
                ApplyDistanceCorrection(vectorOutput);
                AimForBulletDrop(vectorOutput);
            }
            else 
            {
                //Applies Distance Correction
                ApplyDistanceCorrection(localValues.targetLocations[0]);
                AimForBulletDrop(localValues.targetLocations[0]);
            }

        }
        else
        {
            //Does not aim the character
            localValues.yTargetAngle = 0;
            localValues.xTargetAngle = 0;
        }
        
        //Adjusts for the dead zone limits
        AdjustForDeadZoneLimits();
        //Applies recoil if neccarssary
        if (localValues.apparentRecoil.magnitude != 0 || localValues.currentRecoil.magnitude != 0)
            AdjustForRecoil();
        //Adjusts for the offset animation seam
        AdjustAimRotationalValues();
        //Adjusts for a startup delay if needed
        AdjustForStartupDelay();
        //Sends the gyro angles to the animator
        SetAnimatorParameters();
    }

    //Makes sure some parameters have been correctly set or found
    bool CheckPreReqs()
    {
        if (localValues.xGyro == null || requiredTransforms.aimOriginLocation == null || orbitalTargetProvider == null)
        {
            aimCalculationEnabled = false;
            return false;
        }
        return true;
    }
    //Updates the target's location
    bool UpdateTargetLocation()
    {
        bool outFailed;
        localValues.targetLocations.Insert(0,orbitalTargetProvider.GetTargetLocation(out outFailed));
        while (localValues.targetLocations.Count > gyroParameters.yGyroLimits.snapReductionIntensity)
        {
            localValues.targetLocations.RemoveAt(gyroParameters.yGyroLimits.snapReductionIntensity);
        }
        return outFailed;
    }
    //Calculates the gyro angles
    void CalculateAimValues(Vector3 location)
    { 
        if (aimCalculationEnabled)
        {
            //Y Value

            Quaternion lookRotation = Quaternion.LookRotation(ExtendedVector3Functions.RemoveRelativeVector3Y(requiredTransforms.aimOriginLocation, location) - requiredTransforms.aimOriginLocation.position, requiredTransforms.aimOriginLocation.up);
            Vector3 relativePosition = requiredTransforms.aimOriginLocation.InverseTransformPoint(location);
            int directionMultiplier = -1;

            if (relativePosition.x < 0)
                directionMultiplier = 1;


            float angle = Mathf.Clamp(Quaternion.Angle(requiredTransforms.aimOriginLocation.rotation, lookRotation) * directionMultiplier, gyroParameters.yGyroLimits.lowerLimit, gyroParameters.yGyroLimits.upperLimit);
            localValues.yTargetAngle = angle;

            //X Value

            localValues.xGyro.transform.rotation = Quaternion.LookRotation(ExtendedVector3Functions.RemoveRelativeVector3Y(requiredTransforms.aimOriginLocation, location) - requiredTransforms.aimOriginLocation.position, requiredTransforms.aimOriginLocation.up);

            lookRotation = Quaternion.LookRotation(ExtendedVector3Functions.RemoveRelativeVector3X(localValues.xGyro, location) - localValues.xGyro.position, localValues.xGyro.up);
            relativePosition = localValues.xGyro.InverseTransformPoint(location);
            directionMultiplier = 1;

            if (relativePosition.y < 0)
                directionMultiplier = -1;


            localValues.xTargetAngle = Mathf.Clamp(Quaternion.Angle(localValues.xGyro.rotation, lookRotation) * directionMultiplier, gyroParameters.xGyroLimits.lowerLimit, gyroParameters.xGyroLimits.upperLimit);
        }
        else
        {
            localValues.yTargetAngle = 0;
            localValues.xTargetAngle = 0;
        }
    }
    //Applies Bullet Drop Correction
    void AimForBulletDrop(Vector3 location)
    {
        if (localValues.orbitalWeapon == null) return;
        if (setupMode == true) return;
        if (localValues.orbitalWeapon.weaponProfile.projectileDropCorrection == 0) return;
        float distance = Vector3.Distance(requiredTransforms.aimOriginLocation.position, location);
        float upperLimit = Mathf.Min(gyroParameters.xGyroLimits.upperLimit, localValues.orbitalWeapon.weaponProfile.correctionMaximumAngleLimit);
        localValues.xTargetAngle = Mathf.Clamp(localValues.xTargetAngle + distance*(localValues.orbitalWeapon.weaponProfile.projectileDropCorrection/100), gyroParameters.xGyroLimits.lowerLimit, upperLimit);
    }

    //Applies Distance Correction
    void ApplyDistanceCorrection(Vector3 location)
    {
        if (!distanceCorrection.enabled) return;
        if (setupMode == true) return;

        float distance = Vector3.Distance(requiredTransforms.aimOriginLocation.position, location);


        if (distance > distanceCorrection.FocalPoint)
        {
            //-
            localValues.yTargetAngle -= (distance-distanceCorrection.FocalPoint) * distanceCorrection.farsightedMultiplier;
        }
        if (distance < distanceCorrection.FocalPoint)
        {
            //+
            localValues.yTargetAngle += Mathf.Abs(distanceCorrection.FocalPoint- distance) * distanceCorrection.nearsightedMultiplier;
        }
    }
    //Applies snap reduction if needed, averages the target locations 
    bool ApplySnapReduction(out Vector3 averagedVector)
    {
        if (gyroParameters.yGyroLimits.snapReductionAngleLimit != 0 && localValues.yTargetAngle > -135 - gyroParameters.yGyroLimits.snapReductionAngleLimit && localValues.yTargetAngle < -135 + gyroParameters.yGyroLimits.snapReductionAngleLimit)
        {
            averagedVector = ExtendedVector3Functions.AverageVector3List(localValues.targetLocations);
            CalculateAimValues(averagedVector);
            return true;
        }
        averagedVector = Vector3.zero;
        return false;
    }
    //Adjusts for the dead zone limits
    void AdjustForDeadZoneLimits()
    {
        if (gyroParameters.yGyroLimits.upperLimit != 180 && gyroParameters.yGyroLimits.lowerLimit < -135)
        {
            gyroParameters.yGyroLimits.lowerLimit = -135;
        }

        if (gyroParameters.yGyroLimits.deadZoneNegativeLimit != 0 && localValues.yTargetAngle > -135 + gyroParameters.yGyroLimits.deadZoneNegativeLimit && localValues.yTargetAngle <= -135 && gyroParameters.yGyroLimits.upperLimit == 180)
        {
            localValues.yTargetAngle = -135 + gyroParameters.yGyroLimits.deadZoneNegativeLimit;
        }

        if (gyroParameters.yGyroLimits.deadZonePositiveLimit != 0 && localValues.yTargetAngle < -135 + gyroParameters.yGyroLimits.deadZonePositiveLimit && localValues.yTargetAngle > -135)
        {
            localValues.yTargetAngle = -135 + gyroParameters.yGyroLimits.deadZonePositiveLimit;
        }
    }
    //Applies recoil if neccarssary
    void AdjustForRecoil()
    {
        if (localValues.orbitalWeapon == null) return;

        if (localValues.currentRecoil.x > 0)
        {
            localValues.currentRecoil.x = Mathf.Clamp(localValues.currentRecoil.x - localValues.orbitalWeapon.weaponProfile.recoil.recoverySpeed * Time.deltaTime, 0, 360);
        }
        else
        {
            localValues.currentRecoil.x = Mathf.Clamp(localValues.currentRecoil.x + localValues.orbitalWeapon.weaponProfile.recoil.recoverySpeed * Time.deltaTime, -360, 0);
        }

        if (localValues.currentRecoil.y > 0)
        {
            localValues.currentRecoil.y = Mathf.Clamp(localValues.currentRecoil.y - localValues.orbitalWeapon.weaponProfile.recoil.recoverySpeed * Time.deltaTime, 0, 360);
        }
        else
        {
            localValues.currentRecoil.y = Mathf.Clamp(localValues.currentRecoil.y + localValues.orbitalWeapon.weaponProfile.recoil.recoverySpeed * Time.deltaTime, -360, 0);
        }

        if (localValues.currentRecoil.magnitude > localValues.apparentRecoil.magnitude)
        {
            localValues.apparentRecoil = Vector2.Lerp(localValues.apparentRecoil, localValues.currentRecoil, localValues.orbitalWeapon.weaponProfile.adjustmentSpeed * Time.deltaTime);
        }
        else
        {
            localValues.apparentRecoil = localValues.currentRecoil;
        }

        localValues.xTargetAngle = Mathf.Clamp(localValues.xTargetAngle + localValues.apparentRecoil.y, gyroParameters.xGyroLimits.lowerLimit, gyroParameters.xGyroLimits.upperLimit);

        if (gyroParameters.yGyroLimits.upperLimit != 180 && localValues.yTargetAngle + localValues.apparentRecoil.x > gyroParameters.yGyroLimits.upperLimit)
        {
            localValues.yTargetAngle = gyroParameters.yGyroLimits.upperLimit;
            return;
        }
        if (gyroParameters.yGyroLimits.upperLimit == 180 && localValues.yTargetAngle + localValues.apparentRecoil.x > -135 + gyroParameters.yGyroLimits.deadZoneNegativeLimit && localValues.yTargetAngle < -135)
        {
            localValues.yTargetAngle = -135 + gyroParameters.yGyroLimits.deadZoneNegativeLimit;
            return;
        }

        if (localValues.yTargetAngle + localValues.apparentRecoil.x < gyroParameters.yGyroLimits.lowerLimit)
        {
            localValues.yTargetAngle = gyroParameters.yGyroLimits.lowerLimit;
            return;
        }

        if (localValues.yTargetAngle + localValues.apparentRecoil.x < -135 + gyroParameters.yGyroLimits.deadZonePositiveLimit && localValues.yTargetAngle > -135)
        {
            localValues.yTargetAngle = -135 + gyroParameters.yGyroLimits.deadZonePositiveLimit;
            return;
        }

        localValues.yTargetAngle = Mathf.Clamp(localValues.yTargetAngle + localValues.apparentRecoil.x, gyroParameters.yGyroLimits.lowerLimit, gyroParameters.yGyroLimits.upperLimit);

    }
    //Adjusts for the offset animation seam
    void AdjustAimRotationalValues()
    {
        localValues.yTargetAngle += 135;
        if (localValues.yTargetAngle > 360)
        {
            localValues.yTargetAngle -= 360;
        }
        if (localValues.yTargetAngle < 0)
        {
            localValues.yTargetAngle += 360;
        }
    }
    //Adjusts for a startup delay if needed
    void AdjustForStartupDelay()
    {
        if (localValues.startupDelay != 0)
        {
            localValues.startupDelay = Mathf.Clamp(localValues.startupDelay - Time.deltaTime, 0, 10);
            localValues.yTargetAngle = Mathf.Lerp(localValues.yTargetAngle, 135, localValues.startupDelay);
            localValues.xTargetAngle = Mathf.Lerp(localValues.xTargetAngle, 0, localValues.startupDelay);

            if (localValues.yTargetAngle > 225)
            {
                localValues.yTargetAngle = 225f;
            }

            if (localValues.yTargetAngle < 45)
            {
                localValues.yTargetAngle = 45f;
            }
        }
    }
    //Sends the gyro angles to the animator
    void SetAnimatorParameters()
    {
        localValues.animator.SetFloat("OrbitalYGyroRotation", Mathf.Lerp(localValues.animator.GetFloat("OrbitalYGyroRotation"), localValues.yTargetAngle / 360, gyroParameters.gyroSpeed.yGyroSpeed * Time.deltaTime));
        localValues.animator.SetFloat("OrbitalXGyroRotation", Mathf.Lerp(localValues.animator.GetFloat("OrbitalXGyroRotation"), localValues.xTargetAngle, gyroParameters.gyroSpeed.xGyroSpeed * Time.deltaTime));
    }
    //Updates the gun lock transforms
    void UpdateGunLockTransforms()
    {
        UpdateGunLock(requiredTransforms.leftHand, requiredTransforms.leftHandFingerContrastPoint, localValues.rightHandContrastPercent, localValues.gunLockLeft);
        UpdateGunLock(requiredTransforms.rightHand, requiredTransforms.rightHandFingerContrastPoint, localValues.leftHandContrastPercent, localValues.gunLockRight);
    }
    //Returns the aim origin position
    public Vector3 GetAimOriginLocation()
    {
        return requiredTransforms.aimOriginLocation.position;
    }
    //Sends information to the orbital remote
    public void GetSystemInformation(ref OrbitalRemoteSignal orbitalRemoteSignal, bool fullPrintout = false)
    {
        if (localValues.animator != null)
        {
            orbitalRemoteSignal.yGyroRotation = localValues.animator.GetFloat("OrbitalYGyroRotation");
            orbitalRemoteSignal.xGyroRotation = localValues.animator.GetFloat("OrbitalXGyroRotation");
        }
        if (localValues.orbitalWeapon != null)
        {
            orbitalRemoteSignal.setOrbitalWeapon = localValues.orbitalWeapon;
        }

        if (!fullPrintout) return;
        orbitalRemoteSignal.leftHandContrast = localValues.leftHandContrastPercent;
        orbitalRemoteSignal.rightHandContrast = localValues.rightHandContrastPercent;
    }
    //Returns the weapon's transform
    public Transform GetWeaponTransform()
    {
        if (localValues.orbitalWeapon == null)
        {
            return null;
        }
        return localValues.orbitalWeapon.transform;
    }
    //Play's the exoskeleton's fire animation
    public void PlayFireAnimation()
    {
        if (localValues.orbitalWeapon == null) return;

        localValues.animator.SetFloat("OrbitalFireSpeed", localValues.orbitalWeapon.weaponProfile.recoil.animationSpeed);
        localValues.animator.SetFloat("OrbitalFireIntensity", localValues.orbitalWeapon.weaponProfile.recoil.recoilWeight.kick);
        localValues.animator.SetTrigger("OrbitalFire");
    }
    //Play's the characters's fire animation
    public void PlayFireAnimation(Animator animator)
    {
        if (localValues.orbitalWeapon == null) return;

        animator.SetFloat("OrbitalFireSpeed", localValues.orbitalWeapon.weaponProfile.recoil.animationSpeed);
        animator.SetFloat("OrbitalFireIntensity", localValues.orbitalWeapon.weaponProfile.recoil.recoilWeight.kick);
        animator.SetTrigger("OrbitalFire");
    }
    //Applies a new recoil event
    public void ApplyRecoil()
    {
        Vector2 newRecoilRoll = Vector2.zero;
        int randomX = 3;
        if (localValues.orbitalWeapon.weaponProfile.recoil.recoilWeight.left.enabled && localValues.orbitalWeapon.weaponProfile.recoil.recoilWeight.right.enabled)
            randomX = UnityEngine.Random.Range(0, 2);


        int randomY = 3;
        if (localValues.orbitalWeapon.weaponProfile.recoil.recoilWeight.up.enabled && localValues.orbitalWeapon.weaponProfile.recoil.recoilWeight.down.enabled)
            randomY = UnityEngine.Random.Range(0, 2);

        if (randomX == 0 || randomX == 3)
        {
            if (localValues.orbitalWeapon.weaponProfile.recoil.recoilWeight.right.enabled)
                newRecoilRoll.x = UnityEngine.Random.Range(-localValues.orbitalWeapon.weaponProfile.recoil.recoilWeight.right.upperLimit, -localValues.orbitalWeapon.weaponProfile.recoil.recoilWeight.right.lowerLimit);
        }
        if (randomX == 1 || randomX == 3)
        {
            if (localValues.orbitalWeapon.weaponProfile.recoil.recoilWeight.left.enabled)
                newRecoilRoll.x = UnityEngine.Random.Range(localValues.orbitalWeapon.weaponProfile.recoil.recoilWeight.left.lowerLimit, localValues.orbitalWeapon.weaponProfile.recoil.recoilWeight.left.upperLimit);
        }
        if (randomY == 0 || randomY == 3)
        {
            if (localValues.orbitalWeapon.weaponProfile.recoil.recoilWeight.down.enabled)
            newRecoilRoll.y = UnityEngine.Random.Range(-localValues.orbitalWeapon.weaponProfile.recoil.recoilWeight.down.upperLimit, -localValues.orbitalWeapon.weaponProfile.recoil.recoilWeight.down.lowerLimit);
        }
        if (randomY == 1 || randomY == 3)
        {
            if (localValues.orbitalWeapon.weaponProfile.recoil.recoilWeight.up.enabled)
                newRecoilRoll.y = UnityEngine.Random.Range(localValues.orbitalWeapon.weaponProfile.recoil.recoilWeight.up.upperLimit, localValues.orbitalWeapon.weaponProfile.recoil.recoilWeight.up.upperLimit);
        }

        if (newRecoilRoll.magnitude < localValues.orbitalWeapon.weaponProfile.recoil.minimumShotMagnitude)
        {
            newRecoilRoll = newRecoilRoll.normalized * localValues.orbitalWeapon.weaponProfile.recoil.minimumShotMagnitude;
        }
        if (newRecoilRoll.magnitude > localValues.orbitalWeapon.weaponProfile.recoil.maximumShotMagnitude)
        {
            newRecoilRoll = newRecoilRoll.normalized * localValues.orbitalWeapon.weaponProfile.recoil.maximumShotMagnitude;
        }

        newRecoilRoll += localValues.currentRecoil;

        if (localValues.orbitalWeapon.weaponProfile.recoil.persistentHardCap.enabled && newRecoilRoll.magnitude > localValues.orbitalWeapon.weaponProfile.recoil.persistentHardCap.value)
        {
            newRecoilRoll = newRecoilRoll.normalized * localValues.orbitalWeapon.weaponProfile.recoil.persistentHardCap.value;
        }
        else
        {
            if (localValues.orbitalWeapon.weaponProfile.recoil.persistentSoftCap.enabled && newRecoilRoll.magnitude > localValues.orbitalWeapon.weaponProfile.recoil.persistentSoftCap.value)
            {
                newRecoilRoll = newRecoilRoll.normalized * ((newRecoilRoll.magnitude - localValues.orbitalWeapon.weaponProfile.recoil.persistentSoftCap.value) * localValues.orbitalWeapon.weaponProfile.recoil.persistentSoftCap.softness + localValues.orbitalWeapon.weaponProfile.recoil.persistentSoftCap.value);
            }
        }

        localValues.currentRecoil = newRecoilRoll;
    }
    //Sets a startup delay
    public void SetStartupDelay(float time)
    {
        localValues.startupDelay = time;
    }
    //Returns true if there is a weapon
    public bool CheckForWeapon()
    {
        if (localValues.orbitalWeapon != null)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    //Returns the left forearm twist target
    public Transform GetTwistTarget()
    {
        return requiredTransforms.leftHandTwistTarget;
    }
}
