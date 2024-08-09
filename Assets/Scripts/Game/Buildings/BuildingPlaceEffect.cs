using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingPlaceEffect : MonoBehaviour
{
    [SerializeField] private SpriteRenderer m_renderer;

    private void Awake()
    {
        m_renderer.gameObject.transform.localScale = new Vector3(0.75f, 1f, 1f);

        var seq = DOTween.Sequence();
        seq.Append(m_renderer.transform.DOScale(new Vector3(1.25f, 0.75f, 1f), 0.5f));
        seq.Append(m_renderer.transform.DOScale(new Vector3(.9f, 1.1f, 1f), 0.5f));
        seq.Append(m_renderer.transform.DOScale(new Vector3(1, 1f, 1f), 0.05f));
    }
}
