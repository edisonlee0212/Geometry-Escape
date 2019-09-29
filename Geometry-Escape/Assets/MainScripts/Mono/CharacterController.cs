using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace GeometryEscape
{
    public class CharacterController : MonoBehaviour
    {
        [SerializeField]
        private Animator m_Animator;

        private UISystem m_UISystem;


        private int _HealthPoints;

        public int HealthPoints { get => _HealthPoints; }
        public UISystem UISystem { get => m_UISystem; set => m_UISystem = value; }

        public void Start()
        {
            _HealthPoints = 100;
        }

        public void Pause()
        {
            m_Animator.enabled = false;
        }

        public void Resume()
        {
            m_Animator.enabled = true;
        }

        public void MoveLeft()
        {
            var scale = transform.localScale;
            if (scale.x < 0) scale.x *= -1;
            transform.localScale = scale;
            m_Animator.Play("Run");
        }

        public void MoveRight()
        {
            var scale = transform.localScale;
            if (scale.x > 0) scale.x *= -1;
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

        public void ChangeHealth(int amount)
        {
            _HealthPoints += amount;
            UISystem.ChangeHealth(_HealthPoints);
        }
    }
}
