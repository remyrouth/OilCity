//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.7.0
//     from Assets/Scripts/InputSystem/PlayerControlls.inputactions
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public partial class @PlayerControlls: IInputActionCollection2, IDisposable
{
    public InputActionAsset asset { get; }
    public @PlayerControlls()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""PlayerControlls"",
    ""maps"": [
        {
            ""name"": ""UI"",
            ""id"": ""f7492c3b-3ed8-47ce-b276-4414ac5cbce5"",
            ""actions"": [
                {
                    ""name"": ""Toggle PauseMenu"",
                    ""type"": ""Button"",
                    ""id"": ""1030715f-5f2c-4e9c-b150-0476323a7816"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""ClickSelect"",
                    ""type"": ""Button"",
                    ""id"": ""5c81563c-f1c6-450b-9c05-1ce864c5eede"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""MoveHorizontal"",
                    ""type"": ""Value"",
                    ""id"": ""577e86f4-f5cf-4d0d-bfc0-cd4e5119afa7"",
                    ""expectedControlType"": ""Axis"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""MoveVertical"",
                    ""type"": ""Value"",
                    ""id"": ""e2fd64c2-297b-4cf6-9da4-0d5344e9afc1"",
                    ""expectedControlType"": ""Axis"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""b66e3efb-120d-4eb5-93e4-501dff1be2d0"",
                    ""path"": ""<Keyboard>/escape"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Toggle PauseMenu"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""2283fb52-90a0-4c26-9f2e-59ba045bd24d"",
                    ""path"": ""<Gamepad>/buttonSouth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""ClickSelect"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""c6d2bcbb-5d67-4632-9f32-e1e141d971e5"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""ClickSelect"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""7271971b-a2e2-45df-8965-b00731575539"",
                    ""path"": ""<Mouse>/delta/x"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MoveHorizontal"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""22889945-0593-4c35-ad47-c63dcc045007"",
                    ""path"": ""<Mouse>/delta/y"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MoveVertical"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // UI
        m_UI = asset.FindActionMap("UI", throwIfNotFound: true);
        m_UI_TogglePauseMenu = m_UI.FindAction("Toggle PauseMenu", throwIfNotFound: true);
        m_UI_ClickSelect = m_UI.FindAction("ClickSelect", throwIfNotFound: true);
        m_UI_MoveHorizontal = m_UI.FindAction("MoveHorizontal", throwIfNotFound: true);
        m_UI_MoveVertical = m_UI.FindAction("MoveVertical", throwIfNotFound: true);
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }

    public IEnumerable<InputBinding> bindings => asset.bindings;

    public InputAction FindAction(string actionNameOrId, bool throwIfNotFound = false)
    {
        return asset.FindAction(actionNameOrId, throwIfNotFound);
    }

    public int FindBinding(InputBinding bindingMask, out InputAction action)
    {
        return asset.FindBinding(bindingMask, out action);
    }

    // UI
    private readonly InputActionMap m_UI;
    private List<IUIActions> m_UIActionsCallbackInterfaces = new List<IUIActions>();
    private readonly InputAction m_UI_TogglePauseMenu;
    private readonly InputAction m_UI_ClickSelect;
    private readonly InputAction m_UI_MoveHorizontal;
    private readonly InputAction m_UI_MoveVertical;
    public struct UIActions
    {
        private @PlayerControlls m_Wrapper;
        public UIActions(@PlayerControlls wrapper) { m_Wrapper = wrapper; }
        public InputAction @TogglePauseMenu => m_Wrapper.m_UI_TogglePauseMenu;
        public InputAction @ClickSelect => m_Wrapper.m_UI_ClickSelect;
        public InputAction @MoveHorizontal => m_Wrapper.m_UI_MoveHorizontal;
        public InputAction @MoveVertical => m_Wrapper.m_UI_MoveVertical;
        public InputActionMap Get() { return m_Wrapper.m_UI; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(UIActions set) { return set.Get(); }
        public void AddCallbacks(IUIActions instance)
        {
            if (instance == null || m_Wrapper.m_UIActionsCallbackInterfaces.Contains(instance)) return;
            m_Wrapper.m_UIActionsCallbackInterfaces.Add(instance);
            @TogglePauseMenu.started += instance.OnTogglePauseMenu;
            @TogglePauseMenu.performed += instance.OnTogglePauseMenu;
            @TogglePauseMenu.canceled += instance.OnTogglePauseMenu;
            @ClickSelect.started += instance.OnClickSelect;
            @ClickSelect.performed += instance.OnClickSelect;
            @ClickSelect.canceled += instance.OnClickSelect;
            @MoveHorizontal.started += instance.OnMoveHorizontal;
            @MoveHorizontal.performed += instance.OnMoveHorizontal;
            @MoveHorizontal.canceled += instance.OnMoveHorizontal;
            @MoveVertical.started += instance.OnMoveVertical;
            @MoveVertical.performed += instance.OnMoveVertical;
            @MoveVertical.canceled += instance.OnMoveVertical;
        }

        private void UnregisterCallbacks(IUIActions instance)
        {
            @TogglePauseMenu.started -= instance.OnTogglePauseMenu;
            @TogglePauseMenu.performed -= instance.OnTogglePauseMenu;
            @TogglePauseMenu.canceled -= instance.OnTogglePauseMenu;
            @ClickSelect.started -= instance.OnClickSelect;
            @ClickSelect.performed -= instance.OnClickSelect;
            @ClickSelect.canceled -= instance.OnClickSelect;
            @MoveHorizontal.started -= instance.OnMoveHorizontal;
            @MoveHorizontal.performed -= instance.OnMoveHorizontal;
            @MoveHorizontal.canceled -= instance.OnMoveHorizontal;
            @MoveVertical.started -= instance.OnMoveVertical;
            @MoveVertical.performed -= instance.OnMoveVertical;
            @MoveVertical.canceled -= instance.OnMoveVertical;
        }

        public void RemoveCallbacks(IUIActions instance)
        {
            if (m_Wrapper.m_UIActionsCallbackInterfaces.Remove(instance))
                UnregisterCallbacks(instance);
        }

        public void SetCallbacks(IUIActions instance)
        {
            foreach (var item in m_Wrapper.m_UIActionsCallbackInterfaces)
                UnregisterCallbacks(item);
            m_Wrapper.m_UIActionsCallbackInterfaces.Clear();
            AddCallbacks(instance);
        }
    }
    public UIActions @UI => new UIActions(this);
    public interface IUIActions
    {
        void OnTogglePauseMenu(InputAction.CallbackContext context);
        void OnClickSelect(InputAction.CallbackContext context);
        void OnMoveHorizontal(InputAction.CallbackContext context);
        void OnMoveVertical(InputAction.CallbackContext context);
    }
}
