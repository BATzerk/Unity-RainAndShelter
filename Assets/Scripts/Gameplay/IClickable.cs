using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IClickable {
    bool IsClickable();
    void OnHoverOver();
    void OnHoverOut();
    void OnLClickMe(Player player);
    void OnRClickMe(Player player);
    CursorType CurrCursorForMe();
}
