﻿using UnityEngine;
using UnityEngine.UI;

namespace FlatPixel.XR.Mock.Example
{
    public class FingerIndicator : MonoBehaviour
    {
        [SerializeField]
        Button m_Button;

        void Start()
        {
            Cursor.visible = false;
        }

        void Update()
        {
            var rectTransform = m_Button.transform as RectTransform;
            rectTransform.position = Input.mousePosition;
        }
    }
}