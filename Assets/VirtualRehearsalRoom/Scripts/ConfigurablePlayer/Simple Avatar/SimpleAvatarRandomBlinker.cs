using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleAvatarRandomBlinker : MonoBehaviour
{
    public float minBlinkTimeout = 10.0f;
    public float maxBlinkTimeout = 30.0f;

    Animator _animator;

    void Start()
    {
        _animator = GetComponent<Animator>();

        StartCoroutine(RandomBlinkRoutine());
    }

    IEnumerator RandomBlinkRoutine()
    {
        float randomTimeout;
        while (true)
        {
            randomTimeout = Random.Range(minBlinkTimeout, maxBlinkTimeout);
            yield return new WaitForSeconds(randomTimeout);
            _animator.SetTrigger("Blink");
        }
    }

}
