using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
[RequireComponent(typeof(TextMeshProUGUI))]
public class SizeTargetToText : ContentSizeFitter
{
    protected SizeTargetToText()
    {
        m_HorizontalFit = FitMode.PreferredSize;
    }

    public RectTransform target;
    TMP_Text m_TextComponent;


    protected override void OnEnable()
    {
        base.OnEnable();
        
        m_TextComponent = GetComponent<TMP_Text>();

        if (target == null && transform.parent != null)
            target = transform.parent.GetComponentInParent<RectTransform>();

        TMPro_EventManager.TEXT_CHANGED_EVENT.Add(OnTextChanged);
    }

    protected override void OnDisable()
    {
        TMPro_EventManager.TEXT_CHANGED_EVENT.Remove(OnTextChanged);
        base.OnDisable();
    }


    void OnTextChanged(Object obj)
    {
        if (obj == m_TextComponent)
            StartCoroutine(ReSizeTarget());
    }

    IEnumerator ReSizeTarget()
    {
        yield return new WaitForNextFrameUnit();
        target.sizeDelta = new Vector2(m_TextComponent.rectTransform.rect.width, target.sizeDelta.y);
    }
}
