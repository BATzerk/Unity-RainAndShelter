using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToolIconTile : MonoBehaviour
{
    // Components
    [SerializeField] Image i_toolIcon;
    [SerializeField] Image i_healthBarFill;
    [SerializeField] Image i_selectedBorder;
    [SerializeField] RectTransform rt_healthBar;
    // References
    private Tool myTool;



    // ----------------------------------------------------------------
    //  Doers
    // ----------------------------------------------------------------
    public void SetVisualsFromTool(Tool myTool, bool isSelected) {
        this.myTool = myTool;

        i_selectedBorder.enabled = isSelected;

        if (this.myTool == null || myTool.MyType==ToolType.Hand) {
            i_toolIcon.enabled = false;
            rt_healthBar.gameObject.SetActive(false);
        }
        else {
            i_toolIcon.enabled = true;
            rt_healthBar.gameObject.SetActive(true);

            i_toolIcon.sprite = ResourcesHandler.Instance.GetToolIcon(myTool.MyType);
            UpdateHealthBarFill();
        }
    }


    private void UpdateHealthBarFill() {
        float barWidthFull = rt_healthBar.rect.width;
        float fillWidth = barWidthFull * myTool.HealthPercent();
        i_healthBarFill.rectTransform.sizeDelta = new Vector2(fillWidth, i_healthBarFill.rectTransform.sizeDelta.y);
    }




}
