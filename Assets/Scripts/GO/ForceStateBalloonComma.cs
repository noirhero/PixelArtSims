// Copyright 2018-2019 TAP, Inc. All Rights Reserved.

using UnityEngine;
using UnityEngine.UI;

namespace GO {
    public class ForceStateBalloonComma : MonoBehaviour {
        public Text commaText = null;
        public float updateTime = 0.1f;

        private const int _step = 3;
        private float _aniTime = 0.0f;
        private int _aniStep = 0;

        private readonly string[] _commaStrs = new string[_step] {
            ".",
            "..",
            "..."
        };

        public Slider madnessGauge = null;

        private void Awake() {
#if UNITY_EDITOR
            if (null != madnessGauge) {
                madnessGauge.enabled = true;
            }
#endif
        }

        void Update() {
            _aniTime += Time.deltaTime;
            if (updateTime <= _aniTime) {
                _aniTime = 0.0f;

                ++_aniStep;
                if (_step <= _aniStep) {
                    _aniStep = 0;
                }

                commaText.text = _commaStrs[_aniStep];
            }
        }
    }
}
