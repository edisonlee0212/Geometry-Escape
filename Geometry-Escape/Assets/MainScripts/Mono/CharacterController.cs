using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace GeometryEscape
{
    public class CharacterController : MonoBehaviour
    {
        [SerializeField]
        private Animator m_Animator;
        // Start is called before the first frame update
        public void MoveLeft()
        {
            var scale = transform.localScale;
            scale.x = 1;
            transform.localScale = scale;
            m_Animator.Play("Run");
        }

        public void MoveRight()
        {
            var scale = transform.localScale;
            scale.x = -1;
            transform.localScale = scale;
            m_Animator.Play("Run");
        }

        public void MoveUp()
        {
            m_Animator.Play("Run");
        }

        public void MoveDown()
        {
            m_Animator.Play("Run");
        }

        public void Idle()
        {
            m_Animator.Play("Idle");
        }

        public void Attack()
        {
            m_Animator.Play("Attack");
        }
    }
}
