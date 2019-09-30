using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UISystem : MonoBehaviour
{
    //This [SerializeField] will add a option in the inspector. Click on the UISystem in the scene to see the results.
    [SerializeField]
    private GameObject m_Hit_300;
    [SerializeField]
    private GameObject m_Miss;
    [SerializeField]
    private TextMeshProUGUI m_HealthStatusText;
   // [SerializeField]
   // private TextMeshProUGUI m_MonsterHealthText;

    public static TextMeshProUGUI MonsterHealthText;
    public static TextMeshProUGUI HealthStatusText;
    public static GameObject hit_300;
    public static GameObject miss;
    // Start is called before the first frame update
    void Start()
    {
        /*Don't use GameObject.Find, its not safe and its slow. Consider multiple "miss"
         * Consider you renamed "Miss"
         * Consider you have too many game objects
         */
        //hit_300 = GameObject.Find("hit_300");
        //miss = GameObject.Find("Miss");
        hit_300 = m_Hit_300;
        miss = m_Miss;

        HealthStatusText = m_HealthStatusText;
     //   MonsterHealthText = m_MonsterHealthText;

        Debug.Log("init");
        HideHit_300();
        HideMiss();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public static void ShowMiss()
    {

        miss.SetActive(true);
        HideHit_300();
    }

    public static void HideMiss()
    {

        miss.SetActive(false);
    }

    public static void ShowHit_300()
    {

        hit_300.SetActive(true);
        HideMiss();
    }

    public static void HideHit_300()
    {
        hit_300.SetActive(false);
    }

    public static void ChangeHealth(int points)
    {
        HealthStatusText.text = "Health: " + points;
    }
    public static void ChangeMonsterHealth()
    {
   //     MonsterHealthText.text = "Health: "+50;
    }
}