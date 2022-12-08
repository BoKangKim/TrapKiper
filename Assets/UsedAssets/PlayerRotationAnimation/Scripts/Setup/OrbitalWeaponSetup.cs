
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OrbitalWeaponSetup : MonoBehaviour
{
    [Header("Step 2:"), Tooltip("The weapon that you are setting up!")]
    public GameObject weaponPrefab;

    [Serializable]
    public class LocalValues
    {
        public int currentStep = 0;
        public List<GameObject> screens = new List<GameObject>();
        public bool editModeActive = false;

        public GameObject stepTwoButton;
        public Text stepTwoText;
        public GameObject spawnedWeapon;

        public OrbitalRemote orbitalRemote;
        public OrbitalWeaponProfile transformSettings;

        public TransformUIPanel rotationPanel;
        public List<Slider> rotationSliders = new List<Slider>();
        public TransformUIPanel positionPanel;
        public List<Slider> positionSliders = new List<Slider>();
    }
    [Header("Debug: (Read-Only)")]
    public LocalValues localValues;

    //Resets the UI
    private void Start()
    {
        localValues.currentStep = 0;
        localValues.screens[localValues.currentStep].SetActive(true);
        localValues.positionPanel.gameObject.SetActive(false);
        localValues.rotationPanel.gameObject.SetActive(false);
    }

    //Adjusts the profile based on step
    private void Update()
    {
        CheckStepTwo();
        EditUnlatched();
        EditLatched();
        EditLeftHand();
        EditRightHand();
        SaveChanges();
        DrawWeaponRay();
    }

    void SaveChanges()
    {
        if (localValues.transformSettings == null) return;
        localValues.transformSettings.SaveChanges();
    }

    //Adjusts the profile based on step
    void CheckStepTwo()
    {
        if (weaponPrefab == null)
        {
            localValues.stepTwoText.gameObject.SetActive(true);
            localValues.stepTwoText.text = "Complete this step before continuing.";
            localValues.stepTwoButton.SetActive(false);
            return;
        }
        OrbitalWeapon orbitalWeapon = weaponPrefab.GetComponent<OrbitalWeapon>();
        if (orbitalWeapon == null)
        {
            localValues.stepTwoText.gameObject.SetActive(true);
            localValues.stepTwoText.text = "Weapon prefab has not been setup correctly. Please refer to the documentation for help: www.imitationstudios.com";
            localValues.stepTwoButton.SetActive(false);
            return;
        }
        if (orbitalWeapon.weaponProfile == null)
        {
            localValues.stepTwoText.gameObject.SetActive(true);
            localValues.stepTwoText.text = "Weapon prefab has not been setup correctly. Please refer to the documentation for help: www.imitationstudios.com";
            localValues.stepTwoButton.SetActive(false);
            return;
        }
        localValues.transformSettings = orbitalWeapon.weaponProfile;
        localValues.stepTwoButton.SetActive(true);
        localValues.stepTwoText.gameObject.SetActive(false);
    }

    //Adjusts the profile based on step
    void EditUnlatched()
    {
        if (!localValues.editModeActive || localValues.currentStep != 2) return;

        Vector3 newPosition = new Vector3(localValues.rotationSliders[0].value, localValues.rotationSliders[1].value, localValues.rotationSliders[2].value);
        Vector3 newRotation = new Vector3(localValues.positionSliders[0].value, localValues.positionSliders[1].value, localValues.positionSliders[2].value);
        localValues.transformSettings.unlockedGunSettings.rotationOffset = newPosition;
        localValues.transformSettings.unlockedGunSettings.positionOffset = newRotation;
    }

    //Adjusts the profile based on step
    void EditLatched()
    {
        if (!localValues.editModeActive || localValues.currentStep != 3) return;

        Vector3 newPosition = new Vector3(localValues.rotationSliders[0].value, localValues.rotationSliders[1].value, localValues.rotationSliders[2].value);
        Vector3 newRotation = new Vector3(localValues.positionSliders[0].value, localValues.positionSliders[1].value, localValues.positionSliders[2].value);
        localValues.transformSettings.lockedGunSettings.rotationOffset = newPosition;
        localValues.transformSettings.lockedGunSettings.positionOffset = newRotation;
    }

    //Adjusts the profile based on step
    void EditLeftHand()
    {
        if (!localValues.editModeActive || localValues.currentStep != 4) return;

        Vector3 newPosition = new Vector3(localValues.rotationSliders[0].value, localValues.rotationSliders[1].value, localValues.rotationSliders[2].value);
        Vector3 newRotation = new Vector3(localValues.positionSliders[0].value, localValues.positionSliders[1].value, localValues.positionSliders[2].value);
        localValues.transformSettings.handIkSettings.leftHand.rotationOffset = newPosition;
        localValues.transformSettings.handIkSettings.leftHand.positionOffset = newRotation;
    }

    //Adjusts the profile based on step
    void EditRightHand()
    {
        if (!localValues.editModeActive || localValues.currentStep != 5) return;

        Vector3 newPosition = new Vector3(localValues.rotationSliders[0].value, localValues.rotationSliders[1].value, localValues.rotationSliders[2].value);
        Vector3 newRotation = new Vector3(localValues.positionSliders[0].value, localValues.positionSliders[1].value, localValues.positionSliders[2].value);
        localValues.transformSettings.handIkSettings.rightHand.rotationOffset = newPosition;
        localValues.transformSettings.handIkSettings.rightHand.positionOffset = newRotation;
    }

    void DrawWeaponRay()
    {
        if (localValues.currentStep == 3 || localValues.currentStep == 6)
        {
            Transform bindpoint = localValues.orbitalRemote.GetWeapon().bindPoint;
            Debug.DrawLine(bindpoint.position, bindpoint.position+ bindpoint.forward*250, Color.red);
        }
    }

    //Advances the step
    public void CompleteStep(int direction)
    {
        localValues.positionPanel.gameObject.SetActive(false);
        localValues.rotationPanel.gameObject.SetActive(false);
        localValues.editModeActive = false;
        localValues.screens[localValues.currentStep].SetActive(false);
        localValues.screens[localValues.currentStep+ direction].SetActive(true);
        localValues.currentStep += direction;

        if (localValues.currentStep == 2)
        {
            localValues.orbitalRemote.GetComponent<Animator>().SetBool("OrbitalAimMode", false);
            SpawnWeapon();
            SetUnlatchedPanelRotation();
            SetUnlatchedPanelPosition();
            localValues.editModeActive = true;
        }
        if (localValues.currentStep == 3)
        {
            localValues.orbitalRemote.GetComponent<Animator>().SetBool("OrbitalAimMode", true);
            localValues.orbitalRemote.SetSetupMode(true);
            localValues.orbitalRemote.iKEnabled = false;
            SetLatchedPanelRotation();
            SetLatchedPanelPosition();
            localValues.editModeActive = true;
        }
        if (localValues.currentStep == 4)
        {
            localValues.orbitalRemote.iKEnabled = true;
            localValues.orbitalRemote.SetSetupMode(true);
            SetLeftHandPanelPosition();
            SetLeftHandPanelRotation();
            localValues.editModeActive = true;
        }
        if (localValues.currentStep == 5)
        {
            localValues.orbitalRemote.iKEnabled = true;
            localValues.orbitalRemote.SetSetupMode(true);
            SetRightHandPanelPosition();
            SetRightHandPanelRotation();
            localValues.editModeActive = true;
        }
        if (localValues.currentStep == 6)
        {
            localValues.orbitalRemote.SetSetupMode(false);
        }
    }

    //Spawns the weapon
    public void SpawnWeapon()
    {
        localValues.orbitalRemote.SpawnWeaponFromPrefab(weaponPrefab, out localValues.spawnedWeapon);
    }
    
    //Populates the UI Panel
    public void SetUnlatchedPanelRotation()
    {
        localValues.rotationPanel.gameObject.SetActive(true);
        Vector3 xSettings = new Vector3(-180, 180, localValues.transformSettings.unlockedGunSettings.rotationOffset.x);
        Vector3 ySettings = new Vector3(-180, 180, localValues.transformSettings.unlockedGunSettings.rotationOffset.y);
        Vector3 zSettings = new Vector3(-180, 180, localValues.transformSettings.unlockedGunSettings.rotationOffset.z);
        localValues.rotationSliders.Clear();
        Slider xSlide;
        Slider yslide;
        Slider zSlide;
        localValues.rotationPanel.SetPanel("Rotation Settings:", xSettings, ySettings, zSettings, out xSlide, out yslide, out zSlide);
        localValues.rotationSliders.Add(xSlide);
        localValues.rotationSliders.Add(yslide);
        localValues.rotationSliders.Add(zSlide);
    }
    //Populates the UI Panel
    public void SetUnlatchedPanelPosition()
    {
        localValues.positionPanel.gameObject.SetActive(true);
        Vector3 xSettings = new Vector3(Mathf.Min(-1.5f, localValues.transformSettings.unlockedGunSettings.positionOffset.x), Mathf.Max(1.5f, localValues.transformSettings.unlockedGunSettings.positionOffset.x), localValues.transformSettings.unlockedGunSettings.positionOffset.x);
        Vector3 ySettings = new Vector3(Mathf.Min(-1.5f, localValues.transformSettings.unlockedGunSettings.positionOffset.y), Mathf.Max(1.5f, localValues.transformSettings.unlockedGunSettings.positionOffset.y), localValues.transformSettings.unlockedGunSettings.positionOffset.y);
        Vector3 zSettings = new Vector3(Mathf.Min(-1.5f, localValues.transformSettings.unlockedGunSettings.positionOffset.z), Mathf.Max(1.5f, localValues.transformSettings.unlockedGunSettings.positionOffset.z), localValues.transformSettings.unlockedGunSettings.positionOffset.z);
        localValues.positionSliders.Clear();
        Slider xSlide;
        Slider yslide;
        Slider zSlide;
        localValues.positionPanel.SetPanel("Position Settings:", xSettings, ySettings, zSettings, out xSlide, out yslide, out zSlide);
        localValues.positionSliders.Add(xSlide);
        localValues.positionSliders.Add(yslide);
        localValues.positionSliders.Add(zSlide);
    }
    //Populates the UI Panel
    public void SetLatchedPanelRotation()
    {
        localValues.rotationPanel.gameObject.SetActive(true);
        Vector3 worldRotation = localValues.transformSettings.lockedGunSettings.rotationOffset;
        Vector3 xSettings = new Vector3(-180, 180, worldRotation.x);
        Vector3 ySettings = new Vector3(-180, 180, worldRotation.y);
        Vector3 zSettings = new Vector3(-180, 180, worldRotation.z);
        localValues.rotationSliders.Clear();
        Slider xSlide;
        Slider yslide;
        Slider zSlide;
        localValues.rotationPanel.SetPanel("Rotation Settings:", xSettings, ySettings, zSettings, out xSlide, out yslide, out zSlide);
        localValues.rotationSliders.Add(xSlide);
        localValues.rotationSliders.Add(yslide);
        localValues.rotationSliders.Add(zSlide);
    }
    //Populates the UI Panel
    public void SetLatchedPanelPosition()
    {
        localValues.positionPanel.gameObject.SetActive(true);
        Vector3 xSettings = new Vector3(Mathf.Min(-10, localValues.transformSettings.lockedGunSettings.positionOffset.x), Mathf.Max(10, localValues.transformSettings.lockedGunSettings.positionOffset.x), localValues.transformSettings.lockedGunSettings.positionOffset.x);
        Vector3 ySettings = new Vector3(Mathf.Min(-10, localValues.transformSettings.lockedGunSettings.positionOffset.y), Mathf.Max(10, localValues.transformSettings.lockedGunSettings.positionOffset.y), localValues.transformSettings.lockedGunSettings.positionOffset.y);
        Vector3 zSettings = new Vector3(Mathf.Min(-10, localValues.transformSettings.lockedGunSettings.positionOffset.z), Mathf.Max(10, localValues.transformSettings.lockedGunSettings.positionOffset.z), localValues.transformSettings.lockedGunSettings.positionOffset.z);
        localValues.positionSliders.Clear();
        Slider xSlide;
        Slider yslide;
        Slider zSlide;
        localValues.positionPanel.SetPanel("Position Settings:", xSettings, ySettings, zSettings, out xSlide, out yslide, out zSlide);
        localValues.positionSliders.Add(xSlide);
        localValues.positionSliders.Add(yslide);
        localValues.positionSliders.Add(zSlide);
    }
    //Populates the UI Panel
    public void SetLeftHandPanelRotation()
    {
        localValues.rotationPanel.gameObject.SetActive(true);
        Vector3 xSettings = new Vector3(-180, 180, localValues.transformSettings.handIkSettings.leftHand.rotationOffset.x);
        Vector3 ySettings = new Vector3(-180, 180, localValues.transformSettings.handIkSettings.leftHand.rotationOffset.y);
        Vector3 zSettings = new Vector3(-180, 180, localValues.transformSettings.handIkSettings.leftHand.rotationOffset.z);
        localValues.rotationSliders.Clear();
        Slider xSlide;
        Slider yslide;
        Slider zSlide;
        localValues.rotationPanel.SetPanel("Rotation Settings:", xSettings, ySettings, zSettings, out xSlide, out yslide, out zSlide);
        localValues.rotationSliders.Add(xSlide);
        localValues.rotationSliders.Add(yslide);
        localValues.rotationSliders.Add(zSlide);
    }
    //Populates the UI Panel
    public void SetLeftHandPanelPosition()
    {
        localValues.positionPanel.gameObject.SetActive(true);
        Vector3 xSettings = new Vector3(Mathf.Min(-30, localValues.transformSettings.handIkSettings.leftHand.positionOffset.x), Mathf.Max(30, localValues.transformSettings.handIkSettings.leftHand.positionOffset.x), localValues.transformSettings.handIkSettings.leftHand.positionOffset.x);
        Vector3 ySettings = new Vector3(Mathf.Min(-30, localValues.transformSettings.handIkSettings.leftHand.positionOffset.y), Mathf.Max(30, localValues.transformSettings.handIkSettings.leftHand.positionOffset.y), localValues.transformSettings.handIkSettings.leftHand.positionOffset.y);
        Vector3 zSettings = new Vector3(Mathf.Min(-30, localValues.transformSettings.handIkSettings.leftHand.positionOffset.z), Mathf.Max(30, localValues.transformSettings.handIkSettings.leftHand.positionOffset.z), localValues.transformSettings.handIkSettings.leftHand.positionOffset.z);
        localValues.positionSliders.Clear();
        Slider xSlide;
        Slider yslide;
        Slider zSlide;
        localValues.positionPanel.SetPanel("Position Settings:", xSettings, ySettings, zSettings, out xSlide, out yslide, out zSlide);
        localValues.positionSliders.Add(xSlide);
        localValues.positionSliders.Add(yslide);
        localValues.positionSliders.Add(zSlide);
    }
    //Populates the UI Panel
    public void SetRightHandPanelRotation()
    {
        localValues.rotationPanel.gameObject.SetActive(true);
        Vector3 xSettings = new Vector3(-180, 180, localValues.transformSettings.handIkSettings.rightHand.rotationOffset.x);
        Vector3 ySettings = new Vector3(-180, 180, localValues.transformSettings.handIkSettings.rightHand.rotationOffset.y);
        Vector3 zSettings = new Vector3(-180, 180, localValues.transformSettings.handIkSettings.rightHand.rotationOffset.z);
        localValues.rotationSliders.Clear();
        Slider xSlide;
        Slider yslide;
        Slider zSlide;
        localValues.rotationPanel.SetPanel("Rotation Settings:", xSettings, ySettings, zSettings, out xSlide, out yslide, out zSlide);
        localValues.rotationSliders.Add(xSlide);
        localValues.rotationSliders.Add(yslide);
        localValues.rotationSliders.Add(zSlide);
    }
    //Populates the UI Panel
    public void SetRightHandPanelPosition()
    {
        localValues.positionPanel.gameObject.SetActive(true);
        Vector3 xSettings = new Vector3(Mathf.Min(-30, localValues.transformSettings.handIkSettings.rightHand.positionOffset.x), Mathf.Max(30, localValues.transformSettings.handIkSettings.rightHand.positionOffset.x), localValues.transformSettings.handIkSettings.rightHand.positionOffset.x);
        Vector3 ySettings = new Vector3(Mathf.Min(-30, localValues.transformSettings.handIkSettings.rightHand.positionOffset.y), Mathf.Max(30, localValues.transformSettings.handIkSettings.rightHand.positionOffset.y), localValues.transformSettings.handIkSettings.rightHand.positionOffset.y);
        Vector3 zSettings = new Vector3(Mathf.Min(-30, localValues.transformSettings.handIkSettings.rightHand.positionOffset.z), Mathf.Max(30, localValues.transformSettings.handIkSettings.rightHand.positionOffset.z), localValues.transformSettings.handIkSettings.rightHand.positionOffset.z);
        localValues.positionSliders.Clear();
        Slider xSlide;
        Slider yslide;
        Slider zSlide;
        localValues.positionPanel.SetPanel("Position Settings:", xSettings, ySettings, zSettings, out xSlide, out yslide, out zSlide);
        localValues.positionSliders.Add(xSlide);
        localValues.positionSliders.Add(yslide);
        localValues.positionSliders.Add(zSlide);
    }
}
