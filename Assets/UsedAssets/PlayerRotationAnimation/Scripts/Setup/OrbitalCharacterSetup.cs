using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OrbitalCharacterSetup : MonoBehaviour
{
    [Tooltip ("The orbital remote component attached to the character being setup")]
    public OrbitalRemote orbitalRemote;


    //This script is for setup purposes only!

    [Serializable]
    public class LocalValues
    {
        public int currentStep = 0;

        public List<GameObject> panels = new List<GameObject>();

        public GameObject step1Button;
        public GameObject errorText;

        public TransformUIPanel rotationPanel;
        public List<Slider> rotationSliders = new List<Slider>();
        public TransformUIPanel positionPanel;
        public List<Slider> positionSliders = new List<Slider>();

        public bool editMode = false;
    }

    public LocalValues localValues;

    //Resets the UI
    private void Start()
    {
        localValues.currentStep = -1;
        ChangePanel(1);
    }

    //Chanegs the UI
    private void Update()
    {
        if (orbitalRemote != null)
        {
            orbitalRemote.characterSettings.SaveChanges();
        }
        if (localValues.currentStep == 1)
        {
            EditLeftHand();
            return;
        }
        if (localValues.currentStep == 2)
        {
            EditRightHand();
            return;
        }

        if (orbitalRemote == null)
        {
            localValues.step1Button.SetActive(false);
            localValues.errorText.SetActive(true);
        }
        else
        {
            localValues.step1Button.SetActive(true);
            localValues.errorText.SetActive(false);
        }
    }
    //Gives the UI controls to edit the weapon profile
    public void EditLeftHand()
    {
        if (!localValues.editMode) return;
        Vector3 newPosition = new Vector3(localValues.rotationSliders[0].value, localValues.rotationSliders[1].value, localValues.rotationSliders[2].value);
        Vector3 newRotation = new Vector3(localValues.positionSliders[0].value, localValues.positionSliders[1].value, localValues.positionSliders[2].value);
        orbitalRemote.characterSettings.handIkSettings.leftHand.rotationOffset = newPosition;
        orbitalRemote.characterSettings.handIkSettings.leftHand.positionOffset = newRotation;

    }

    //Gives the UI controls to edit the weapon profile
    public void EditRightHand()
    {
        if (!localValues.editMode) return;
        Vector3 newPosition = new Vector3(localValues.rotationSliders[0].value, localValues.rotationSliders[1].value, localValues.rotationSliders[2].value);
        Vector3 newRotation = new Vector3(localValues.positionSliders[0].value, localValues.positionSliders[1].value, localValues.positionSliders[2].value);
        orbitalRemote.characterSettings.handIkSettings.rightHand.rotationOffset = newPosition;
        orbitalRemote.characterSettings.handIkSettings.rightHand.positionOffset = newRotation;
    }

    //Button event that changes the UI
    public void ChangePanel(int direction)
    {
        localValues.editMode = false;
        if (localValues.currentStep >= 0)
        localValues.panels[localValues.currentStep].SetActive(false);
        localValues.currentStep += direction;
        localValues.panels[localValues.currentStep].SetActive(true);
        localValues.positionPanel.gameObject.SetActive(false);
        localValues.rotationPanel.gameObject.SetActive(false);

        if (orbitalRemote != null)
        {
            orbitalRemote.GetComponent<Animator>().SetBool("OrbitalAimMode", true);
            orbitalRemote.SetSetupMode(true);
            orbitalRemote.iKEnabled = true;
        }

        if (localValues.currentStep == 1)
        {
            SetLeftHandPanelPosition();
            SetLeftHandPanelRotation();
            localValues.editMode = true;
        }
        if (localValues.currentStep == 2)
        {
            SetRightHandPanelPosition();
            SetRightHandPanelRotation();
            localValues.editMode = true;
        }
        if (localValues.currentStep == 3)
        {
            orbitalRemote.SetSetupMode(false);
        }
    }

    //Adjusts a UI panel
    public void SetLeftHandPanelRotation()
    {
        localValues.rotationPanel.gameObject.SetActive(true);
        Vector3 xSettings = new Vector3(-180, 180, orbitalRemote.characterSettings.handIkSettings.leftHand.rotationOffset.x);
        Vector3 ySettings = new Vector3(-180, 180, orbitalRemote.characterSettings.handIkSettings.leftHand.rotationOffset.y);
        Vector3 zSettings = new Vector3(-180, 180, orbitalRemote.characterSettings.handIkSettings.leftHand.rotationOffset.z);
        localValues.rotationSliders.Clear();
        Slider xSlide;
        Slider yslide;
        Slider zSlide;
        localValues.rotationPanel.SetPanel("Rotation Settings:", xSettings, ySettings, zSettings, out xSlide, out yslide, out zSlide);
        localValues.rotationSliders.Add(xSlide);
        localValues.rotationSliders.Add(yslide);
        localValues.rotationSliders.Add(zSlide);
    }

    //Adjusts a UI panel
    public void SetLeftHandPanelPosition()
    {
        localValues.positionPanel.gameObject.SetActive(true);
        Vector3 xSettings = new Vector3(Mathf.Min(-10, orbitalRemote.characterSettings.handIkSettings.leftHand.positionOffset.x), Mathf.Max(10, orbitalRemote.characterSettings.handIkSettings.leftHand.positionOffset.x), orbitalRemote.characterSettings.handIkSettings.leftHand.positionOffset.x);
        Vector3 ySettings = new Vector3(Mathf.Min(-10, orbitalRemote.characterSettings.handIkSettings.leftHand.positionOffset.y), Mathf.Max(10, orbitalRemote.characterSettings.handIkSettings.leftHand.positionOffset.y), orbitalRemote.characterSettings.handIkSettings.leftHand.positionOffset.y);
        Vector3 zSettings = new Vector3(Mathf.Min(-10, orbitalRemote.characterSettings.handIkSettings.leftHand.positionOffset.z), Mathf.Max(10, orbitalRemote.characterSettings.handIkSettings.leftHand.positionOffset.z), orbitalRemote.characterSettings.handIkSettings.leftHand.positionOffset.z);
        localValues.positionSliders.Clear();
        Slider xSlide;
        Slider yslide;
        Slider zSlide;
        localValues.positionPanel.SetPanel("Position Settings:", xSettings, ySettings, zSettings, out xSlide, out yslide, out zSlide);
        localValues.positionSliders.Add(xSlide);
        localValues.positionSliders.Add(yslide);
        localValues.positionSliders.Add(zSlide);
    }

    //Adjusts a UI panel
    public void SetRightHandPanelRotation()
    {
        localValues.rotationPanel.gameObject.SetActive(true);
        Vector3 xSettings = new Vector3(-180, 180, orbitalRemote.characterSettings.handIkSettings.rightHand.rotationOffset.x);
        Vector3 ySettings = new Vector3(-180, 180, orbitalRemote.characterSettings.handIkSettings.rightHand.rotationOffset.y);
        Vector3 zSettings = new Vector3(-180, 180, orbitalRemote.characterSettings.handIkSettings.rightHand.rotationOffset.z);
        localValues.rotationSliders.Clear();
        Slider xSlide;
        Slider yslide;
        Slider zSlide;
        localValues.rotationPanel.SetPanel("Rotation Settings:", xSettings, ySettings, zSettings, out xSlide, out yslide, out zSlide);
        localValues.rotationSliders.Add(xSlide);
        localValues.rotationSliders.Add(yslide);
        localValues.rotationSliders.Add(zSlide);
    }

    //Adjusts a UI panel
    public void SetRightHandPanelPosition()
    {
        localValues.positionPanel.gameObject.SetActive(true);
        Vector3 xSettings = new Vector3(Mathf.Min(-10, orbitalRemote.characterSettings.handIkSettings.rightHand.positionOffset.x), Mathf.Max(10, orbitalRemote.characterSettings.handIkSettings.rightHand.positionOffset.x), orbitalRemote.characterSettings.handIkSettings.rightHand.positionOffset.x);
        Vector3 ySettings = new Vector3(Mathf.Min(-10, orbitalRemote.characterSettings.handIkSettings.rightHand.positionOffset.y), Mathf.Max(10, orbitalRemote.characterSettings.handIkSettings.rightHand.positionOffset.y), orbitalRemote.characterSettings.handIkSettings.rightHand.positionOffset.y);
        Vector3 zSettings = new Vector3(Mathf.Min(-10, orbitalRemote.characterSettings.handIkSettings.rightHand.positionOffset.z), Mathf.Max(10, orbitalRemote.characterSettings.handIkSettings.rightHand.positionOffset.z), orbitalRemote.characterSettings.handIkSettings.rightHand.positionOffset.z);
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
