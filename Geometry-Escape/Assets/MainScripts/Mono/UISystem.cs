using GeometryEscape;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UISystem : MonoBehaviour {
    //This [SerializeField] will add a option in the inspector. Click on the UISystem in the scene to see the results.
    [SerializeField]
    private GameObject m_Hit_300;
    [SerializeField]
    private GameObject m_Miss;
    [SerializeField]
    private TextMeshProUGUI m_HealthStatusText;
    // [SerializeField]
    // private TextMeshProUGUI m_MonsterHealthText;
    [SerializeField]
    private GameObject m_Popup;
    [SerializeField]
    private TextMeshProUGUI m_PopupText;

    public static TextMeshProUGUI MonsterHealthText;
    public static TextMeshProUGUI HealthStatusText;
    public static GameObject hit_300;
    public static GameObject miss;
    public static GameObject popup;
    public static TextMeshProUGUI popupText;

    #region Popups
    private static string[] popupString = {
@"How to move your character:
Use either WSAD or navigation keys to make your first move!",
@"Remember tp follow the beat!
You'll get informed when you hit or missed the beat!",
@"Nail Trap!
Your character took damage every time you stepped on them!",
@"MusicAcclerator Trap!
The music playback and the pace of beat are accelerated!",
@"Freeze Trap!
Your character are immobilized for the next 5 movement!",
@"Inverse Trap!
The movement pattern are inversed!
Up is now down and left is now right, etc."
    };

    private static bool[] popupFlags = new bool[] { true, true, true, true, true, true };
    #endregion

    // Start is called before the first frame update
    void Start() {
        /*Don't use GameObject.Find, its not safe and its slow. Consider multiple "miss"
         * Consider you renamed "Miss"
         * Consider you have too many game objects
         */
        //hit_300 = GameObject.Find("hit_300");
        //miss = GameObject.Find("Miss");
        hit_300 = m_Hit_300;
        miss = m_Miss;
        popup = m_Popup;
        popupText = m_PopupText;

        HealthStatusText = m_HealthStatusText;
        //   MonsterHealthText = m_MonsterHealthText;

        HideHit_300();
        HideMiss();
    }

    // Update is called once per frame
    void Update() {

    }

    public static void ShowMiss() {

        miss.SetActive(true);
        HideHit_300();
    }

    public static void HideMiss() {

        miss.SetActive(false);
    }

    public static void ShowHit_300() {

        hit_300.SetActive(true);
        HideMiss();
    }

    public static void HideHit_300() {
        hit_300.SetActive(false);
    }

    public static void ChangeHealth(int points) {
        if (ControlSystem.ControlMode == ControlMode.InGame)
        {
            HealthStatusText.text = "Health: " + points;
        }
    }
    public static void ChangeMonsterHealth() {
        //     MonsterHealthText.text = "Health: "+50;
    }

    public static void Displaypopup(int index) {
        if (ControlSystem.ControlMode == ControlMode.InGame)
        {
            if (popupFlags[index])
            {
                popupFlags[index] = false;
                popupText.text = popupString[index];
                popup.SetActive(true);
            }
        }
    }
}