using DG.Tweening;
using UnityEngine;

public class AnimalController : MonoBehaviour, ITickReceiver
{
    private Animator _anim;

    private int _timer;
    public void OnTick()
    {
        _timer--;
        if (_timer < 0)
        {
            _timer = Random.Range(10, 20);
            GoToRandomPos();
        }
    }
    private float range = 2;
    private void GoToRandomPos()
    {
        Vector3 pos = transform.position + (Random.insideUnitCircle * range).ToVector3();
        transform.localScale = new Vector3((transform.position - pos).x > 0 ? 1 : -1, 1, 1);
        _anim.SetBool("IsWalking", true);
        transform.DOKill();
        transform.DOMove(pos, Vector3.Distance(pos, transform.position) * 4).OnComplete(() => ResetVisual());
    }
    private void ResetVisual()
    {
        _anim.SetBool("IsWalking", false);
    }

    private void Awake()
    {
        _anim = GetComponent<Animator>();
        TimeManager.Instance.RegisterReceiver(this);
    }
    private void OnDestroy()
    {
        TimeManager.Instance.DeregisterReceiver(this);
    }





}
