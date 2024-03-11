using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIDragObject : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    protected Camera camera;
    private bool isDragging = false;
    private Vector3 offset;
    protected bool canDragging = false;

    protected virtual void Update()
    {
        if (isDragging)
        {
            Vector3 mousePosition = GetMouseWorldPosition();
            transform.position = new Vector3(mousePosition.x + offset.x, mousePosition.y + offset.y, transform.position.z);
        }
    }

    public void ApplyCamera(Camera _camera)
    {
        camera = _camera;
    }

    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePosition = Input.mousePosition;
        //mousePosition.z = -Camera.main.transform.position.z;
        return camera.ScreenToWorldPoint(mousePosition);
    }

    void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("OnPointerDown: " + canDragging);

        if (!canDragging)
            return;
        isDragging = true;
        offset = gameObject.transform.position - GetMouseWorldPosition();
        OnPointerDown();
    }

    void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
    {
        Debug.Log("OnPointerUp");
        isDragging = false;
        OnPointerUp();
    }

    protected virtual void OnPointerDown()
    {

    }

    protected virtual void OnPointerUp()
    {

    }
}
