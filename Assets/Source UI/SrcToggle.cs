using System;
using System.ComponentModel;
using Unity.VisualScripting;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace UnityEngine.UI
{
    /// <summary>
    /// A modded toggle to work with basic behaviour
    /// </summary>
    /// <remarks>
    /// If you change or add a property that should be displayed in the inspector you have to handle that property in
    /// <see cref="UnityEditor.UI.SrcToggleEditor"/>
    /// </remarks>
    [AddComponentMenu("UI/SrcToggle", 30)]
    [RequireComponent(typeof(RectTransform))]
    public class SrcToggle : Toggle
    {
        private bool useCheckBoxOld = false;
        [SerializeField]
        private bool m_useCheckBox = false;
        public bool UseCheckBox {
            get { return m_useCheckBox; }
            set
            {
                m_useCheckBox = value;
                HandleGraphics();
            }
        }

        public Image targetImage;
        public Sprite normalSprite;
        public Sprite selectedSprite;


        protected override void OnEnable()
        {
            base.OnEnable();

            onValueChanged.AddListener(IsOnChanged);
        }

        protected override void OnDisable()
        {
            onValueChanged.RemoveListener(IsOnChanged);

            base.OnDisable();
        }



        private void IsOnChanged(bool isOn)
        {
            targetImage.sprite = isOn ? selectedSprite : normalSprite;

            var rectTransform = GetComponent<RectTransform>();
            float height = isOn ? 44 : 40;
            float yPos = isOn ? -18 : -20;
            rectTransform.sizeDelta = new Vector2(rectTransform.rect.width, height);
            rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, yPos);
        }

        private void HandleGraphics()
        {
            if (transition != Transition.None)
                transition = Transition.None;

            if (useCheckBoxOld != UseCheckBox)
            {
                graphic.enabled = UseCheckBox ? true : false;
                useCheckBoxOld = UseCheckBox;
            }
        }


        private void InternalToggle()
        {
            if (!IsActive() || !IsInteractable())
                return;

            isOn = !isOn;

            IsOnChanged(isOn);
        }

        /// <summary>
        /// React to clicks.
        /// </summary>
        public override void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
                return;

            InternalToggle();
        }


        protected override void OnValidate()
        {
            HandleGraphics();

            base.OnValidate();
        }
    }
}
