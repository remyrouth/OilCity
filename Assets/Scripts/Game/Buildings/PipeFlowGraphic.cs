using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PipeFlowGraphic : MonoBehaviour
{
    [SerializeField] private Color m_keroseneColor;
    [SerializeField] private Color m_oilColor;
    [SerializeField] private ParticleSystem m_flowSystem;
    private (ParticleSystem lhs, ParticleSystem rhs) m_systems;

    private bool m_lhsState = false;
    private bool m_rhsState = false;
    private FlowType m_flowType;

    private bool m_lockoutDisable = false;
    private bool m_doLhsDisableAfterLockout = false;
    private bool m_doRhsDisableAfterLockout = false;

    private bool m_invalidLhs = false;
    private bool m_invalidRhs = false;

    public void SetupSystems(Vector2Int lhs, Vector2Int rhs, PipeFlowDirection lhs_dir, PipeFlowDirection rhs_dir)
    {
        m_doLhsDisableAfterLockout = false;
        m_doRhsDisableAfterLockout = false;
        m_flowType = FlowType.None;

        m_invalidLhs = false;
        m_invalidRhs = false;
        
        var lhs_dir_offset = Utilities.GetPipeFlowDirOffset(lhs_dir);
        var rhs_dir_offset = Utilities.GetPipeFlowDirOffset(rhs_dir);

        var offset = new Vector2(0.5f, 0.5f);

        var lhs_pos = lhs + offset - 0.5f * new Vector2(lhs_dir_offset.x, lhs_dir_offset.y);
        var rhs_pos = rhs + offset + 0.5f * new Vector2(rhs_dir_offset.x, rhs_dir_offset.y);

        var lhs_go = Instantiate(m_flowSystem);
        var rhs_go = Instantiate(m_flowSystem);

        lhs_go.transform.position = lhs_pos;
        rhs_go.transform.position = rhs_pos;

        m_systems = (lhs_go, rhs_go);

        float lhs_offset_angle = Vector2.SignedAngle(lhs_dir_offset, Vector2.right);
        float rhs_offset_angle = Vector2.SignedAngle(rhs_dir_offset, Vector2.right);

        lhs_go.transform.Rotate(lhs_offset_angle, 0f, 0f);
        rhs_go.transform.Rotate(rhs_offset_angle, 0f, 0f);

        StartCoroutine(Pulse());
    }

    public void ToggleSystem(bool is_lhs, bool state)
    {
        if (m_lockoutDisable && state == false)
        {
            if (is_lhs) m_doLhsDisableAfterLockout = true;
            else m_doRhsDisableAfterLockout = true;
            return;
        }

        if (is_lhs)
        {
            if (state == m_lhsState) return;

            if (state)
            {
                m_systems.lhs.Play();

                m_lhsState = true;
            }
            else
            {
                m_systems.lhs.Stop();

                m_lhsState = false;
            }
        }
        else
        {
            if (state == m_rhsState) return;

            if (state)
            {
                m_systems.rhs.Play();

                m_rhsState = true;
            }
            else
            {
                m_systems.rhs.Stop();

                m_rhsState = false;
            }
        }
    }

    public void SetFlow(FlowType flow)
    {
        if (flow == m_flowType && (!m_invalidRhs && !m_invalidLhs)) return;

        if (flow == FlowType.Kerosene) SetColor(m_keroseneColor);
        if (flow == FlowType.Oil) SetColor(m_oilColor);

        m_flowType = flow;
    }

    public void SetColor(Color color)
    {
        var cache = m_systems.rhs.main;
        cache.startColor = color;

        cache = m_systems.lhs.main;
        cache.startColor = color;

        m_invalidLhs = m_invalidRhs = false;
    }

    public void SetColor(bool is_start, Color color)
    {
        if (!is_start)
        {
            var cache = m_systems.rhs.main;
            cache.startColor = color;

            m_invalidRhs = color.Equals(Color.red);
        }
        else
        {
            var cache = m_systems.lhs.main;
            cache.startColor = color;

            m_invalidLhs = color.Equals(Color.red);
        }
    }

    public void ClearObjs()
    {
        if (m_systems.lhs != null) Destroy(m_systems.lhs.gameObject);
        if (m_systems.rhs != null) Destroy(m_systems.rhs.gameObject);
    }

    private IEnumerator Pulse()
    {
        ToggleSystem(true, true);
        ToggleSystem(false, true);

        // prevent systems from disabling the graphic during the pulse
        m_lockoutDisable = true;
        yield return new WaitForSeconds(1.5f);
        m_lockoutDisable = false;

        // if a disable was enqueued, disable them.
        if (m_doLhsDisableAfterLockout) ToggleSystem(true, false);
        if (m_doRhsDisableAfterLockout) ToggleSystem(false, false);

    }

    private void OnDestroy()
    {
        ClearObjs();
    }
}
