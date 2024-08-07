using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

/// <summary>
/// A class in charge of controlling the firing of game ticks at real-time intervals. Additionally in charge of registering
/// and deregistering objects that are meant to receive these ticks.
/// </summary>
public class TimeManager : Singleton<TimeManager>
{
#if UNITY_EDITOR
    [SerializeField] private Gradient m_debugGradient;
#endif


    /// <summary>
    /// How many ticks occur in a minute. Setting this property changes the ticks per minute, the amount of time per tick (60f / new_ticks),
    /// and the time that has passed since a new tick began (reset to 0).
    /// </summary>
    public int TicksPerMinute
    {
        get { return m_ticksPerMinute; }
        set
        {
            m_ticksPerMinute = value;
            m_timePerTick = 60f / m_ticksPerMinute;
            m_timeElaspedSinceTick = 0;
            OnTicksPerMinuteChanged?.Invoke(value);
            OnTimePerTickChanged?.Invoke(TimePerTick);
        }
    }
    [SerializeField] private int m_ticksPerMinute = 60;
    public event Action<int> OnTicksPerMinuteChanged;
    public event Action<float> OnTimePerTickChanged;
    // a collection of all the nodes that require an individual OnTick invokation
    // loggers and train stations, for example. individual trees (like a pipe to a refinery) 
    // also exist in this list.
    public Collection<ITickReceiver> m_tickableForest = new();

    // invariant: contains every tree node component in the game
    private readonly Collection<Game.New.IFlowable> m_nodes = new();

    public float TimePerTick => m_timePerTick;
    private float m_timePerTick;
    private float m_timeElaspedSinceTick;

    private void Awake()
    {
        TicksPerMinute = m_ticksPerMinute;
    }

    /// <summary>
    /// Tracks the time elapsed and invokes a tick if necessary
    /// </summary>
    private void FixedUpdate()
    {
        // if we've elapsed more time than the time it takes for a tick to evaluate, start the tick
        if (m_timeElaspedSinceTick >= m_timePerTick)
        {
            // reset the tick. This doesn't set to 0 because then we might lose some time.
            m_timeElaspedSinceTick -= m_timePerTick;

            // activate all tick listeners
            for (int i = m_tickableForest.Count - 1; i >= 0; i--)
                m_tickableForest[i].OnTick();
        }

        // increment time
        m_timeElaspedSinceTick += Time.fixedDeltaTime;
    }

    #region Registration and Deregistration

    /// <summary>
    /// Registers the given gameobject as a tickreciever if it has a component that inherents from the interface.
    /// Additionally, manages the combination of trees that are connected to the input object when called.
    /// </summary>
    /// <param name="receiver"></param>
    public void RegisterReceiver(MonoBehaviour receiver)
    {
        // if no tickable item is found on the given object, nothing is done
        if (receiver as ITickReceiver == null)
        {
            Debug.Log(string.Format("No tick receiver given. Skipping..."));
            return;
        }


        // flowables handle their own relations in the timemanager
        if (receiver is not Game.New.IFlowable)
            m_tickableForest.Add(receiver as ITickReceiver);
        else
            Debug.Log("Flowables handle their own registration. Skipping...");
    }

    /// <summary>
    /// Removes a given gameobject from the tick collection if it has a component that was registered. Depending on
    /// the itme remove, sometimes splits the tree into multiple new trees that are added to the forest.
    /// </summary>
    /// <param name="receiver"></param>
    public void DeregisterReceiver(MonoBehaviour receiver)
    {
        // if no tickable item is found on the given object, nothing is done
        if (receiver as ITickReceiver == null)
        {
            Debug.Log(string.Format("No tick receiver given. Skipping..."));
            return;
        }

        if (receiver is not Game.New.IFlowable)
            m_tickableForest.Remove(receiver as ITickReceiver);
        else
            Debug.Log("Flowables handle their own deregistration. Skipping...");

    }

    /// <summary>
    /// Removes the object from the forest without disconnecting any parents or children.
    /// </summary>
    /// <param name="obj"></param>
    public void LiteDeregister(ITickReceiver obj)
    {
        m_tickableForest.Remove(obj);
    }

    #region old code
    /// <summary>
    /// Helper method that, given a flowable, binds it bidirectionally to its reported children and parent,
    /// potentially reducing the number of trees in the forest.
    /// </summary>
    /// <param name="node"></param>
    private void HandleRegister(Game.New.IFlowable node)
    {
        var relations = node.GetRelations();
        var children = relations.GetRelationFlowables(Relation.Input);

        foreach (var (flowable, _) in children)
        {
            if (m_tickableForest.Contains(flowable))
            {
                m_tickableForest.Remove(flowable);
            }
        }

        // if we dont have a parent
        if (relations.GetRelationFlowables(Relation.Output).Count == 0)
        {
            m_tickableForest.Add(node);
        }

        // add us to the building list of all active nodes
        m_nodes.Add(node);
    }

    /// <summary>
    /// Helper method that, given a flowable, attempts to remove it from the node list and tickable
    /// forest. Unregisters children and parents and splits trees into the forest if there are children.
    /// </summary>
    /// <param name="node"></param>
    private void HandleDeregister(Game.New.IFlowable node)
    {
        var relations = node.GetRelations();
        var children = relations.GetRelationFlowables(Relation.Input);

        foreach (var (flowable, _) in children)
        {
            m_tickableForest.Add(flowable);
        }

        if (m_tickableForest.Contains(node)) m_tickableForest.Remove(node);

        node.Remove();

        // remove us from the building list of all active nodes
        m_nodes.Remove(node);
    }
    #endregion

    #endregion

    #region Debug
#if UNITY_EDITOR
    public void OnDrawGizmosSelected()
    {
        if (m_tickableForest == null) return;

        for (int i = 0; i < m_tickableForest.Count; ++i)
        {
            Gizmos.color = m_debugGradient.Evaluate(i / (float)m_tickableForest.Count);

            if (!ConvertToClassType<MonoBehaviour>(m_tickableForest[i], out var mono)) continue;

            Gizmos.DrawSphere(mono.transform.position + Vector3.up * 0.5f + Vector3.right * 0.5f, 0.4f);

            if (!ConvertToClassType<IConnection>(m_tickableForest[i], out var tree_node)) continue;

            var children = new List<IConnection>();
            children.AddRange(tree_node.GetRelations().GetRelationFlowables(Relation.Input).ConvertAll(a => a.flowable));

            while (children.Count > 0)
            {
                var item = children[0];
                children.RemoveAt(0);

                if (!ConvertToClassType<MonoBehaviour>(item, out var imono)) continue;
                if (imono == null)
                {
                    Debug.Log("Mono has vanished..? " + mono.gameObject.name);
                    continue;
                }

                Gizmos.DrawCube(imono.transform.position + Vector3.up * 0.5f + Vector3.right * 0.5f, Vector3.one * 0.35f);

                children.AddRange(item.GetRelations().GetRelationFlowables(Relation.Input).ConvertAll(a => a.flowable));
            }
        }
    }

    private bool ConvertToClassType<T>(object obj, out T mono) where T : class
    {
        mono = obj as T;
        if (mono == null) return false;
        return true;
    }
#endif
    #endregion
}
