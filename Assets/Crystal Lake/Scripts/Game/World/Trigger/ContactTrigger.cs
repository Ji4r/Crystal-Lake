using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace MyProj
{
    public class ContactTrigger : MonoBehaviour
    {
        [SerializeField]
        private string contactTag = "Player";
        [SerializeField, Tooltip("Одноразовый")] private bool isDisposableEnter = false;
        [SerializeField, Tooltip("Одноразовый")] private bool isDisposableExit = false;
        [SerializeField] private float delayEnter = 0f;
        [SerializeField] private float delayExit = 0f;

        public UnityEvent OnContactEnter;
        public UnityEvent OnContactExit;

        private Coroutine coroutineEnter;
        private Coroutine coroutineExit;
        private bool onCallEnter;
        private bool onCallExit;

        private void OnDisable()
        {
            if (coroutineEnter != null)
            {
                StopCoroutine(coroutineEnter);
                coroutineEnter = null;
            }
            if (coroutineExit != null)
            {
                StopCoroutine(coroutineExit);
                coroutineExit = null;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            Debug.Log($"ContactTrigger: OnTriggerEnter with {other.tag}");
            if (!other.CompareTag(contactTag))
                return;

            if (coroutineEnter != null || onCallEnter)
                return;

            coroutineEnter = StartCoroutine(CallWithDelay(
                delayEnter, OnContactEnter, isDisposableEnter,
                () => onCallEnter = true,
                () => coroutineEnter = null));
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.CompareTag(contactTag))
                return;

            if (coroutineExit != null || onCallExit)
                return;

            coroutineExit = StartCoroutine(CallWithDelay(
                delayExit, OnContactExit, isDisposableExit,
                () => onCallExit = true,
                () => coroutineExit = null));
        }

        private IEnumerator CallWithDelay(float delay, UnityEvent unityEvent, bool isDisposable,
                                            Action setDisposable, Action clearCoroutine)
        {
            yield return new WaitForSeconds(delay);

            unityEvent?.Invoke();

            if (isDisposable)
                setDisposable?.Invoke();

            clearCoroutine?.Invoke();
        }
    }
}
