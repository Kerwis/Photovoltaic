using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Slide : MonoBehaviour
{
    public static bool ShowHintIfNeed = false;
    public GameObject hint;
    public float timeToShowHint = 3;
    public RectTransform canvas;
    public List<RectTransform> PanelsTransforms;
    private int actualPage = 0;
    private float slideSpeed = 2;
    private bool moved;
    private float idleTime;
    private bool preventDrag = false;

    private void Start()
    {
        ShowHintIfNeed = true;
        HideHint();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Alpha1))
            SetPage(0);
        if(Input.GetKeyDown(KeyCode.Alpha2))
            SetPage(1);
        if(Input.GetKeyDown(KeyCode.Alpha3))
            SetPage(2);
        if(Input.GetKeyDown(KeyCode.Alpha4))
            SetPage(3);
        if (ShowHintIfNeed)
        {
            idleTime += Time.deltaTime;
        }
        if (!moved && idleTime > timeToShowHint)
        {
            ShowHint();
        }
        if(EventSystem.current.IsPointerOverGameObject() || preventDrag)
            return;
        
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            // Handle finger movements based on touch phase.
            switch (touch.phase)
            {
                // Determine direction by comparing the current touch position with the initial one.
                case TouchPhase.Moved:
                    ApplyPanelsMove(touch.deltaPosition);
                    break;

                // Report that a direction has been chosen when the finger is lifted.
                case TouchPhase.Ended:
                    SetPage();
                    break;
            }
        }
    }

    private void ShowHint()
    {
        hint.SetActive(true);
    }

    private void HideHint()
    {
        hint.SetActive(false);
    }

    private void SetPage()
    {
        for (int i = 0; i < PanelsTransforms.Count; i++)
        {
            if (PanelsTransforms[1].anchoredPosition.x >= -canvas.rect.width * i - (canvas.rect.width / 2))
            {
                actualPage = i;
                break;
            }
        }

        SetPage(actualPage);

        moved = true;
        HideHint();
    }

    public void SetPage(int page)
    {
        for (int i = 0; i < PanelsTransforms.Count; i++)
        {
            var tmp = PanelsTransforms[i].anchoredPosition;
            tmp.x = canvas.rect.width * (i - page - 1);
            PanelsTransforms[i].anchoredPosition = tmp;
        }
    }

    public void PreventDrag()
    {
        preventDrag = true;
    }

    public void AllowDrag()
    {
        preventDrag = false;
    }

    private void ApplyPanelsMove(Vector2 deltaPosition)
    {
        deltaPosition.y = 0;
        deltaPosition.x *= slideSpeed;
        Vector2 tmp;
        for (int i = 0; i < PanelsTransforms.Count; i++)
        {
            PanelsTransforms[i].anchoredPosition += deltaPosition;
            if (PanelsTransforms[i].anchoredPosition.x < -canvas.rect.width * (PanelsTransforms.Count - i - 1))
            {
                tmp = PanelsTransforms[i].anchoredPosition;
                tmp.x = -canvas.rect.width * (PanelsTransforms.Count - i - 1);
                PanelsTransforms[i].anchoredPosition = tmp;
            }

            if (PanelsTransforms[i].anchoredPosition.x > canvas.rect.width * (i - 1))
            {
                tmp = PanelsTransforms[i].anchoredPosition;
                tmp.x = canvas.rect.width * (i - 1);
                PanelsTransforms[i].anchoredPosition = tmp;
            }
        }
    }
}
