using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace GeometryEscape
{
    public class CharacterHead : MonoBehaviour
    {
        private static SpriteRenderer rend;

        public void ChangeColor()
        {
            rend = GetComponent<SpriteRenderer>();
            rend.color = Color.red;
        }
    }
}