using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISystem : MonoBehaviour
{
    public static GameObject hit_300;
    public static GameObject miss;
    // Start is called before the first frame update
    void Start()
    {
        hit_300 = GameObject.Find("hit_300");
        miss = GameObject.Find("Miss");
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

}
