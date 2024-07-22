using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IFlowable
{
    (FlowType, float) SendFlow();
}

public enum FlowType { Oil, Kerosene }
