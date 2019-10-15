// Copyright 2018-2019 TAP, Inc. All Rights Reserved.

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Preset {
    public class GUIPreset : MonoBehaviour {
        public Canvas canvas = null;

        public Image[] items = new Image[3];
        public List<Sprite> itemSprites = new List<Sprite>();

        public GameObject forceStateBalloon = null;

        public Slider sanGaugeSlider = null;

        private void Awake() {
#if UNITY_EDITOR
            sanGaugeSlider.gameObject.SetActive(true);
#endif
        }
    }
}
