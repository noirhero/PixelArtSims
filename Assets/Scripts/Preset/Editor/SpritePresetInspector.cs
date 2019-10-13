// Copyright 2018-2019 TAP, Inc. All Rights Reserved.

using System.Collections.Generic;
using System.Text;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;

namespace Preset.Editor {
    [CustomEditor(typeof(SpritePreset)), CanEditMultipleObjects]
    public class SpritePresetInspector : UnityEditor.Editor {
        public override void OnInspectorGUI() {
            base.OnInspectorGUI();

            GUILayout.Space(20);
            if (GUILayout.Button("Set clips and then click.")) {
                InitializePreset(target as SpritePreset);
            }
        }

        private static void InitializePreset(SpritePreset preset) {
            preset.datas.Clear();

            foreach (var clip in preset.clips) {
                var sprites = new List<Sprite>();
                foreach (var binding in AnimationUtility.GetObjectReferenceCurveBindings(clip)) {
                    foreach (var frame in AnimationUtility.GetObjectReferenceCurve(clip, binding)) {
                        sprites.Add((Sprite) frame.value);
                    }
                }

                if (0 == sprites.Count) {
                    continue;
                }

                var firstSprite = sprites[0];
                var pixelRatioX = firstSprite.texture.width / 100.0f;
                var pixelRatioY = firstSprite.texture.height / 100.0f;
                var scaleX = firstSprite.rect.width / firstSprite.texture.width * pixelRatioX;
                var scaleY = firstSprite.rect.height / firstSprite.texture.height * pixelRatioY;

                var presetData = new SpritePresetData() {
                    texture = sprites[0].texture,
                    length = clip.length,
                    frameRate = clip.length / sprites.Count,
                    scale = new float3(scaleX, scaleY, 1.0f),
                    posOffset = new float3(0.0f, scaleY * 0.5f, 0.0f),
                    rects = new List<Vector4>()
                };
                foreach (var sprite in sprites) {
                    presetData.rects.Add(new Vector4(
                        sprite.rect.width / sprite.texture.width,
                        sprite.rect.height / sprite.texture.height,
                        sprite.rect.x / sprite.texture.width,
                        sprite.rect.y / sprite.texture.height));
                }

                var hash = 0;
                foreach (var b in Encoding.ASCII.GetBytes(clip.name)) {
                    hash += b;
                }

                preset.datas.Add(hash, presetData);
            }
        }
    }
}
