using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkerVisualAnimator : MonoBehaviour
{
    private Animator _anim;
    public Animator Anim
    {
        get
        {
            if (_anim == null)
                _anim = GetComponent<Animator>();
            return _anim;
        }
    }
    private void Start()
    {
        UpdateSpeed(TimeManager.Instance.TicksPerMinute);
    }
    private void OnEnable()
    {
        TimeManager.Instance.OnTicksPerMinuteChanged += UpdateSpeed;
    }
    private void OnDisable()
    {
        TimeManager.Instance.OnTicksPerMinuteChanged += UpdateSpeed;
    }
    protected void UpdateSpeed(int newTickRate)
    {
        if (newTickRate == 0)
        {
            Anim.speed = 0;
            transform.DOPause();
        }
        else
        {
            Anim.speed = 1;
            transform.DOPlay();
        }

    }
}
