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
        [SerializeField]
        private bool m_useCheckBox = false;
        private bool isCheckBoxOld = false;
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


        private void HandleGraphics()
        {
            if (transition != Transition.None)
                transition = Transition.None;

            if (isCheckBoxOld != UseCheckBox)
            {
                graphic.enabled = UseCheckBox ? true : false;
                isCheckBoxOld = UseCheckBox;
            }
        }


        private void InternalToggle()
        {
            if (!IsActive() || !IsInteractable())
                return;

            isOn = !isOn;

            var test = targetGraphic as Image;
            targetImage.sprite = isOn ? selectedSprite : normalSprite;
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
