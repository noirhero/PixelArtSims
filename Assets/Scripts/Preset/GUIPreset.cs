// Copyright 2018-2019 TAP, Inc. All Rights Reserved.

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

namespace Preset {
    public class GUIPreset : MonoBehaviour {
        public Image[] items = new Image[3];
        public List<Texture> itemImages = new List<Texture>();

        public GameObject forceStateBallon = null;

        public Slider sanGaugeSlider = null;

        private void Awake() {
#if UNITY_EDITOR
            foreach (var itemImage in items) {
                itemImage.sprite = null;
            }

            sanGaugeSlider.gameObject.SetActive(true);
#endif
        }
    }
}
