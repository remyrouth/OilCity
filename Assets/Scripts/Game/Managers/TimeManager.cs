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
    private Collection<ITickReceiver> m_tickableForest = new();

    // invariant: contains every tree node component in the game
    private readonly Collection<ITreeNode> m_nodes = new();

    public float TimePerTick => m_timePerTick;
    private float m_timePerTick;
    private float m_timeElaspedSinceTick;

    private readonly Queue<(GameObject obj, bool to_register)> m_objectsToModifyRegistration = new();

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
            if (m_tickableForest.Count != 0) {
                for (int i = m_tickableForest.Count - 1; i >= 0; i--) {
                    m_tickableForest[i].OnTick();
                }
            }
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

        // this isnt very great bc we aren't using just TreeNodes, but the issue is that we need a flowable at
        // some level before adding it as a child, which is something we can't do with just a tree node. Oops!
        //
        // otherwise, if no flowable component is found, we know that it's something that doesn't deal with flow
        // but needs ticks. Like a logger cabin or a geologist's hut. Hence, we'll just add it to the tickable forest
        // as its own singleton tree.
        if (receiver is IFlowable flow_node)
            HandleRegister(flow_node);
        else
            m_tickableForest.Add(receiver as ITickReceiver);
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

        // this isnt very great bc we aren't using just TreeNodes, but the issue is that we need a flowable at
        // some level before adding it as a child, which is something we can't do with just a tree node. Oops!
        //
        // otherwise, if no flowable component is found, we know that it's something that doesn't deal with flow
        // but needs ticks. Like a logger cabin or a geologist's hut. Hence, we'll just add it to the tickable forest
        // as its own singleton tree.
        if (receiver is IFlowable flow_node)
            HandleDeregister(flow_node);
        else
            m_tickableForest.Remove(receiver as ITickReceiver);

    }

    /// <summary>
    /// Removes the object from the forest without disconnecting any parents or children.
    /// </summary>
    /// <param name="obj"></param>
    public void LiteDeregister(ITickReceiver obj)
    {
        m_tickableForest.Remove(obj);
    }

    /// <summary>
    /// Helper method that, given a flowable, binds it bidirectionally to its reported children and parent,
    /// potentially reducing the number of trees in the forest.
    /// </summary>
    /// <param name="node"></param>
    private void HandleRegister(IFlowable node)
    {
        var children = node.GetChildren(); // cache the node's reported children (its input)
        var parent = node.GetParent(); // cache the node's reported parent (its output)

        // the parents and the children, by the invariants, will never NOT be in the node list.
        // therefore we can update their connections with no consequence
        foreach (var child in children)
        {
            // 
            child.SetParent(node);

            // if the child was in the forest but now has a parent, remove them from the forest
            if (m_tickableForest.Contains(child))
            {
                m_tickableForest.Remove(child);
            }
        }

        // if we have a parent, set us as their child. Otherwise, we are the root of our tree and
        // belong in the tickable forest.
        if (parent != null)
        {
            parent.AddChild(node);
        }
        else
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
    private void HandleDeregister(IFlowable node)
    {
        var children = node.GetChildren(); // cache the node's children
        var parent = node.GetParent(); // cache the node's parent

        // the parents and the children, by the invariants, will never NOT be in the node list.
        // therefore we can update their connections with no consequence
        foreach (var child in children)
        {
            // disown children
            child.SetParent(null);

            // since we disowned them, they have to make it on their own in the vast world
            // i.e. we have to add them to the tickable forest
            //
            // we dont need to check if the current node is in the forest because every node has one
            // parent. Since we removed that parent, the child node HAS to be a root now.
            m_tickableForest.Add(child);
        }

        // if we have a parent, have them disown us because we've been a terrible kid to them
        // and have failed at every possible opportunity in life.
        //
        // otherwise, remove ourselves from the tickable forest, since if we didn't have a parent
        // we'd've had to have been a root.
        if (parent != null)
        {
            parent.DisownChild(node);
        }
        else
        {
            m_tickableForest.Remove(node);
        }

        // remove us from the building list of all active nodes
        m_nodes.Remove(node);
    }

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

            if (!ConvertToClassType<ITreeNode>(m_tickableForest[i], out var tree_node)) continue;

            List<ITreeNode> children = new List<ITreeNode>();
            children.AddRange(tree_node.GetChildren());

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

                children.AddRange(item.GetChildren());
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
