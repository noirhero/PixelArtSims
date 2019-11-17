// Copyright 2018-2019 TAP, Inc. All Rights Reserved.

using System.Text;
using System.Collections.Generic;
using UnityEngine;

namespace Preset {
    public class StatePreset : MonoBehaviour {
        public enum Type {
            Idle, Walk, SomethingDoIt, Count
        }

        private static readonly List<int> _hashes = new List<int>();

        private static void FillStates() {
            for (var i = 0; i < (int)Type.Count; ++i) {
                var hash = 0;
                foreach (var b in Encoding.ASCII.GetBytes(((Type) i).ToString())) {
                    hash += b;
                }
                _hashes.Add(hash);
            }
        }

        private void Awake() {
            FillStates();
        }

        public static int GetStateHash(Type inType) {
            return _hashes[(int) inType];
        }
    }
}
