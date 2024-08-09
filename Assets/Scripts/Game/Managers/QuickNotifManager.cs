using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuickNotifManager : Singleton<QuickNotifManager>
{
    public enum PingType
    {
        Connection,
        NoConnection,
        WhatTheHell
    }

    [System.Serializable]
    public struct SKVPair<T, K>
    {
        public T key;
        public K value;
    }

    [SerializeField] private List<SKVPair<PingType, GameObject>> m_objMap;

    public void PingSpot(PingType type, Vector3 pos)
    {
        var obj = Instantiate(FindObject(type), pos + Vector3.forward * 2f + Vector3.right * 0.5f + Vector3.up, Quaternion.identity);
        obj.transform.localScale = Vector3.zero;
        // obj.transform.DOScale(Vector3.one, 0.25f);
        Destroy(obj, 2f);

        var sequence = DOTween.Sequence();
        sequence.Append(obj.transform.DOScale(Vector3.one, 0.25f));
        sequence.AppendInterval(1.5f);
        sequence.Append(obj.transform.DOScale(Vector3.zero, 0.25f));
    }

    private GameObject FindObject(PingType type)
    {
        foreach (var pair in m_objMap)
        {
            if (pair.key == type)
                return pair.value;
        }

        return null;
    }
}
