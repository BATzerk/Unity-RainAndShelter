using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class UIDraggable : MonoBehaviour, IDragHandler, IBeginDragHandler {
    // Properties
    private Vector2 dragOffset; // set in OnBeginDrag.
    [SerializeField] private bool mayDragHorz=true;
    [SerializeField] private bool mayDragVert=true;
    
    // Events
    public void OnBeginDrag(PointerEventData eventData) {
        dragOffset = this.transform.position - Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }
    public void OnDrag(PointerEventData eventData) {
        //this.transform.position += (Vector3)eventData.delta;
        Vector2 targetPos = Camera.main.ScreenToWorldPoint(Input.mousePosition) + new Vector3(dragOffset.x,dragOffset.y);
        if (!mayDragHorz) {
            targetPos = new Vector2(transform.position.x, targetPos.y);
        }
        if (!mayDragVert) {
            targetPos = new Vector2(targetPos.x, transform.position.y);
        }
        this.transform.position = new Vector3(targetPos.x, targetPos.y, this.transform.position.z); // Note: Keep Z the same.
    }
}
