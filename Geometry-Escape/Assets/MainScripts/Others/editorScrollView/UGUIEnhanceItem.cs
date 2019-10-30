using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UGUIEnhanceItem : EnhanceItem
{
    private Button uButton;
    private Image image;

    protected override void OnStart()
    {
        image = GetComponent<Image>();
        uButton = GetComponent<Button>();
        uButton.onClick.AddListener(OnClickUGUIButton);
    }

    private void OnClickUGUIButton()
    {
        OnClickEnhanceItem();
    }

    // Set the item "depth" 2d or 3d
    protected override void SetItemDepth(float depthCurveValue, int depthFactor, float itemCount)
    {
        int newDepth = (int)(depthCurveValue * itemCount);
        this.transform.SetSiblingIndex(newDepth);
    }

    public override void SetSelectState(bool isCenter)
    {
        if (image == null)
            image = GetComponent<Image>();
        image.color = isCenter ? Color.white : Color.gray;
    }
}