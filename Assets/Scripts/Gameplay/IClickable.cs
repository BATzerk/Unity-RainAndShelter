using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IClickable {
    bool IsClickable(Tool tool);
    CursorType CurrCursorForMe(Tool tool);
    void OnHoverOver();
    void OnHoverOut();
    void OnLClickMe(Player player);
    void OnRClickMe(Player player);
}
