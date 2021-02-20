// GENERATED AUTOMATICALLY FROM 'Assets/InputMaster.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class @InputMaster : IInputActionCollection, IDisposable
{
    public InputActionAsset asset { get; }
    public @InputMaster()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""InputMaster"",
    ""maps"": [
        {
            ""name"": ""player"",
            ""id"": ""90b5f0ee-f77a-480e-a389-bcd9c5c24dfb"",
            ""actions"": [
                {
                    ""name"": ""Arm"",
                    ""type"": ""Button"",
                    ""id"": ""c9272878-d132-422f-b186-ed75c90182df"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": ""Tap""
                },
                {
                    ""name"": ""ThrottleYaw"",
                    ""type"": ""PassThrough"",
                    ""id"": ""c04dee8c-82af-460a-b851-b92ef2923bf2"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""PitchRoll"",
                    ""type"": ""PassThrough"",
                    ""id"": ""73263c04-108d-4b16-bb34-2983a86c70a4"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Disarm"",
                    ""type"": ""Button"",
                    ""id"": ""e0b5787e-17d9-4dd0-ac66-142e99f5d0e5"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Poshold"",
                    ""type"": ""Button"",
                    ""id"": ""f501f11d-ec13-4e61-9411-8db34e041a5b"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Stabilize"",
                    ""type"": ""Button"",
                    ""id"": ""c9ae9af9-e20b-41e8-b705-3d4548579d31"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""811c9949-e7b1-4c08-ac71-b875ea5d4772"",
                    ""path"": ""<XInputController>/buttonSouth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Xbox"",
                    ""action"": ""Arm"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""6a4b39f7-2190-4be8-9d9d-723ce850c604"",
                    ""path"": ""<XInputController>/leftStick"",
                    ""interactions"": """",
                    ""processors"": ""StickDeadzone"",
                    ""groups"": ""Xbox"",
                    ""action"": ""ThrottleYaw"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""2D Vector"",
                    ""id"": ""ee20be69-dfdd-4064-94f0-68b1f2325f5c"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""ThrottleYaw"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""6524aabd-35ab-4205-8956-ffc0c94dd68d"",
                    ""path"": """",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""ThrottleYaw"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""cafb56da-9c61-4c77-8d48-5ce574353421"",
                    ""path"": """",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""ThrottleYaw"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""53eb2a77-250c-424c-a6ff-edf8d871f156"",
                    ""path"": """",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""ThrottleYaw"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""667c0581-d13d-44bb-8d92-37167666bd1b"",
                    ""path"": """",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""ThrottleYaw"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""ae50e64f-6ebf-40a5-b817-9e211dc4dade"",
                    ""path"": ""<XInputController>/rightStick"",
                    ""interactions"": """",
                    ""processors"": ""StickDeadzone"",
                    ""groups"": ""Xbox"",
                    ""action"": ""PitchRoll"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""2D Vector"",
                    ""id"": ""991525d6-cd28-48f6-929a-9cbbaf630120"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""PitchRoll"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""6a2038cd-016f-401b-a191-66b3ddf8363a"",
                    ""path"": """",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""PitchRoll"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""a9e8a7a7-96ae-4d71-81f7-f93d29c4b4fe"",
                    ""path"": """",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""PitchRoll"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""fbc298d5-900f-43f3-b922-19dff7ddd4a1"",
                    ""path"": """",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""PitchRoll"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""619012db-3194-412c-b50b-ec25d72daef9"",
                    ""path"": """",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""PitchRoll"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""b3e2a30b-0cee-4644-8b02-ef4eeae3811e"",
                    ""path"": ""<XInputController>/buttonSouth"",
                    ""interactions"": ""Hold"",
                    ""processors"": """",
                    ""groups"": ""Xbox"",
                    ""action"": ""Disarm"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""2d1f2ce4-174c-4186-a555-eab4421679bd"",
                    ""path"": ""<XInputController>/buttonEast"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Xbox"",
                    ""action"": ""Poshold"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""be6c846e-6b89-4752-b2f8-35942ab5e6ca"",
                    ""path"": ""<XInputController>/buttonWest"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Xbox"",
                    ""action"": ""Stabilize"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": [
        {
            ""name"": ""Xbox"",
            ""bindingGroup"": ""Xbox"",
            ""devices"": [
                {
                    ""devicePath"": ""<XInputController>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        }
    ]
}");
        // player
        m_player = asset.FindActionMap("player", throwIfNotFound: true);
        m_player_Arm = m_player.FindAction("Arm", throwIfNotFound: true);
        m_player_ThrottleYaw = m_player.FindAction("ThrottleYaw", throwIfNotFound: true);
        m_player_PitchRoll = m_player.FindAction("PitchRoll", throwIfNotFound: true);
        m_player_Disarm = m_player.FindAction("Disarm", throwIfNotFound: true);
        m_player_Poshold = m_player.FindAction("Poshold", throwIfNotFound: true);
        m_player_Stabilize = m_player.FindAction("Stabilize", throwIfNotFound: true);
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

    // player
    private readonly InputActionMap m_player;
    private IPlayerActions m_PlayerActionsCallbackInterface;
    private readonly InputAction m_player_Arm;
    private readonly InputAction m_player_ThrottleYaw;
    private readonly InputAction m_player_PitchRoll;
    private readonly InputAction m_player_Disarm;
    private readonly InputAction m_player_Poshold;
    private readonly InputAction m_player_Stabilize;
    public struct PlayerActions
    {
        private @InputMaster m_Wrapper;
        public PlayerActions(@InputMaster wrapper) { m_Wrapper = wrapper; }
        public InputAction @Arm => m_Wrapper.m_player_Arm;
        public InputAction @ThrottleYaw => m_Wrapper.m_player_ThrottleYaw;
        public InputAction @PitchRoll => m_Wrapper.m_player_PitchRoll;
        public InputAction @Disarm => m_Wrapper.m_player_Disarm;
        public InputAction @Poshold => m_Wrapper.m_player_Poshold;
        public InputAction @Stabilize => m_Wrapper.m_player_Stabilize;
        public InputActionMap Get() { return m_Wrapper.m_player; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(PlayerActions set) { return set.Get(); }
        public void SetCallbacks(IPlayerActions instance)
        {
            if (m_Wrapper.m_PlayerActionsCallbackInterface != null)
            {
                @Arm.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnArm;
                @Arm.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnArm;
                @Arm.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnArm;
                @ThrottleYaw.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnThrottleYaw;
                @ThrottleYaw.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnThrottleYaw;
                @ThrottleYaw.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnThrottleYaw;
                @PitchRoll.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnPitchRoll;
                @PitchRoll.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnPitchRoll;
                @PitchRoll.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnPitchRoll;
                @Disarm.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnDisarm;
                @Disarm.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnDisarm;
                @Disarm.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnDisarm;
                @Poshold.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnPoshold;
                @Poshold.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnPoshold;
                @Poshold.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnPoshold;
                @Stabilize.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnStabilize;
                @Stabilize.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnStabilize;
                @Stabilize.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnStabilize;
            }
            m_Wrapper.m_PlayerActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Arm.started += instance.OnArm;
                @Arm.performed += instance.OnArm;
                @Arm.canceled += instance.OnArm;
                @ThrottleYaw.started += instance.OnThrottleYaw;
                @ThrottleYaw.performed += instance.OnThrottleYaw;
                @ThrottleYaw.canceled += instance.OnThrottleYaw;
                @PitchRoll.started += instance.OnPitchRoll;
                @PitchRoll.performed += instance.OnPitchRoll;
                @PitchRoll.canceled += instance.OnPitchRoll;
                @Disarm.started += instance.OnDisarm;
                @Disarm.performed += instance.OnDisarm;
                @Disarm.canceled += instance.OnDisarm;
                @Poshold.started += instance.OnPoshold;
                @Poshold.performed += instance.OnPoshold;
                @Poshold.canceled += instance.OnPoshold;
                @Stabilize.started += instance.OnStabilize;
                @Stabilize.performed += instance.OnStabilize;
                @Stabilize.canceled += instance.OnStabilize;
            }
        }
    }
    public PlayerActions @player => new PlayerActions(this);
    private int m_XboxSchemeIndex = -1;
    public InputControlScheme XboxScheme
    {
        get
        {
            if (m_XboxSchemeIndex == -1) m_XboxSchemeIndex = asset.FindControlSchemeIndex("Xbox");
            return asset.controlSchemes[m_XboxSchemeIndex];
        }
    }
    public interface IPlayerActions
    {
        void OnArm(InputAction.CallbackContext context);
        void OnThrottleYaw(InputAction.CallbackContext context);
        void OnPitchRoll(InputAction.CallbackContext context);
        void OnDisarm(InputAction.CallbackContext context);
        void OnPoshold(InputAction.CallbackContext context);
        void OnStabilize(InputAction.CallbackContext context);
    }
}
