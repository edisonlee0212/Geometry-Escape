using GeometryEscape;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UISystem : MonoBehaviour {
    //This [SerializeField] will add a option in the inspector. Click on the UISystem in the scene to see the results.
    [SerializeField]
    private GameObject m_Hit_300;
    [SerializeField]
    private GameObject m_Miss;
    [SerializeField]
    private Slider m_HealthBar;
    [SerializeField]
    private TextMeshProUGUI m_HealthStatusText;
    // [SerializeField]
    // private TextMeshProUGUI m_MonsterHealthText;
    [SerializeField]
    private GameObject m_Popup;
    [SerializeField]
    private TextMeshProUGUI m_PopupText;
    [SerializeField]
    private Image m_ExitIndicator;

    private bool isShow = true;
    private float alpha = 0.1f;
    private float alphaSpeed = 0.01f;
    private static Sprite[] arrows;
    private static int last_arrow;

    public static TextMeshProUGUI MonsterHealthText;
    public static Slider health_bar;
    public static TextMeshProUGUI HealthStatusText;
    public static GameObject hit_300;
    public static GameObject miss;
    public static GameObject popup;
    public static Image exit_indicator;
    public static TextMeshProUGUI popupText;
    public static float _devision = 0;
    public CanvasGroup cg;
    
    

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
        exit_indicator = m_ExitIndicator;
        arrows = Resources.LoadAll<Sprite>("Textures/UI/arrows");
        last_arrow = -1;

        HealthStatusText = m_HealthStatusText;
        //   MonsterHealthText = m_MonsterHealthText;
        health_bar = m_HealthBar;
        HideHit_300();
        HideMiss();
    }

    // Update is called once per frame
    void Update() {
        if (_devision != 0)
        {
            alphaSpeed = Time.deltaTime / _devision * 2;
        }
        if (isShow)
        {
            if (alpha != cg.alpha)
            {
                cg.alpha = Mathf.Max(cg.alpha - alphaSpeed, alpha); //Mathf.Lerp(cg.alpha, alpha, alphaSpeed * Time.deltaTime);
                if (Mathf.Abs(alpha - cg.alpha) <= 0.01)
                {
                    cg.alpha = alpha; isShow = false;
                }
            }
        }
        else
        {
            if (1 != cg.alpha)
            {
                cg.alpha = Mathf.Min(cg.alpha + alphaSpeed, 1);
                if (Mathf.Abs(1 - cg.alpha) <= 0.01)
                {
                    cg.alpha = 1; isShow = true;
                }
            }
        }

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
            health_bar.value = points;
            HealthStatusText.text = points + "%";
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

    public static void UpdateExitIndicator(float x, float y) {
        if (ControlSystem.ControlMode == ControlMode.InGame) {
            if (x > 0.0f && y > 0.0f) {
                if (last_arrow == 6) {
                    return;
                }

                exit_indicator.overrideSprite = arrows[6];
                last_arrow = 6;
            } else if (x > 0.0f && y == 0.0f) {
                if (last_arrow == 4) {
                    return;
                }

                exit_indicator.overrideSprite = arrows[4];
                last_arrow = 4;
            } else if (x == 0.0f && y > 0.0f) {
                if (last_arrow == 7) {
                    return;
                }

                exit_indicator.overrideSprite = arrows[7];
                last_arrow = 7;
            } else if (x < 0.0f && y > 0.0f) {
                if (last_arrow == 5) {
                    return;
                }

                exit_indicator.overrideSprite = arrows[5];
                last_arrow = 5;
            } else if (x < 0.0f && y == 0.0f) {
                if (last_arrow == 3) {
                    return;
                }

                exit_indicator.overrideSprite = arrows[3];
                last_arrow = 3;
            } else if (x == 0.0f && y < 0.0f) {
                if (last_arrow == 2) {
                    return;
                }

                exit_indicator.overrideSprite = arrows[2];
                last_arrow = 2;
            } else if (x > 0.0f && y < 0.0f) {
                if (last_arrow == 1) {
                    return;
                }

                exit_indicator.overrideSprite = arrows[1];
                last_arrow = 1;
            } else {
                if (last_arrow == 0) {
                    return;
                }

                exit_indicator.overrideSprite = arrows[0];
                last_arrow = 0;
            }
        }
    }
}