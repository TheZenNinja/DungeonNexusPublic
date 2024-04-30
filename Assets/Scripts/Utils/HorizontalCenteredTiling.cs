using System.Collections;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Collections.Generic;

namespace Utils
{
    public class HorizontalCenteredTiling : MonoBehaviour
    {
        public float spacing = 10;

        [Button("Align")]
        public void Align()
        {
            var chilren = new List<RectTransform>();

            foreach (Transform c in transform)
                if (c.TryGetComponent(out RectTransform rect))
                    chilren.Add(rect);

            float totalWidth = 0;
            foreach (RectTransform rect in chilren)
                totalWidth += rect.sizeDelta.x;
            totalWidth += (chilren.Count - 1) * spacing;

            float currentX = -totalWidth / 2 + 
                //account for center offset, is jank please fix
                chilren[0].sizeDelta.x/2;
            for (int i = 0; i < chilren.Count; i++) {
                var c = chilren[i];
                c.localPosition = new Vector2(currentX, 0);
                currentX += c.sizeDelta.x + spacing;
            }
        }
    }
}