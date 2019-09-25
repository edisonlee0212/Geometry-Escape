// GENERATED AUTOMATICALLY FROM 'Assets/Controls.inputactions'

using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class Controls : IInputActionCollection
{
    private InputActionAsset asset;
    public Controls()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""Controls"",
    ""maps"": [
        {
            ""name"": ""Map Editor"",
            ""id"": ""975f58a4-4977-421c-a242-ce23bc7d24fb"",
            ""actions"": [
                {
                    ""name"": ""Move"",
                    ""type"": ""Button"",
                    ""id"": ""f3f98836-c8e3-4536-a3aa-f1a1c299bb68"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": ""Press""
                },
                {
                    ""name"": ""Zoom"",
                    ""type"": ""Button"",
                    ""id"": ""0ca8ace1-03e8-4c2e-93b9-9ba34e111245"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": ""Press""
                },
                {
                    ""name"": ""RemoveCenterTile"",
                    ""type"": ""Button"",
                    ""id"": ""394c4b70-522e-47b7-af25-2fc88a6818b2"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": ""Press""
                },
                {
                    ""name"": ""AddCenterTile"",
                    ""type"": ""Button"",
                    ""id"": ""cbe69035-6672-427e-add4-c973e80ee3f5"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": ""Press""
                },
                {
                    ""name"": ""DestroyAllTiles"",
                    ""type"": ""Button"",
                    ""id"": ""30721afd-8f6d-4b02-9290-6729597987f9"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": ""Press""
                },
                {
                    ""name"": ""SaveMap"",
                    ""type"": ""Button"",
                    ""id"": ""088143cc-ee3e-4e21-81af-d5722b5e917e"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": ""Press""
                },
                {
                    ""name"": ""LoadMap"",
                    ""type"": ""Button"",
                    ""id"": ""04ad959b-94fc-4559-b40b-2487d1624fb9"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": ""Press""
                }
            ],
            ""bindings"": [
                {
                    ""name"": ""WASD"",
                    ""id"": ""5e47b52d-653b-457b-a1ef-efd37350a256"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""PC"",
                    ""action"": ""Move"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""02167e3b-6c89-4d11-a69b-6d58d572f44a"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""PC"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""17fe07d6-5736-4f84-9020-0e02b5391a88"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""PC"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""20760cb0-0e41-4dea-a40a-c8b934705be6"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""PC"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""fbb8aa24-5274-4567-96eb-53fef720ad61"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""PC"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""Arrows"",
                    ""id"": ""a44b60c8-440d-4445-ada7-a27f53e1805c"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""PC"",
                    ""action"": ""Move"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""4898d5e2-f7b0-4d37-999a-252b4f35bad9"",
                    ""path"": ""<Keyboard>/upArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""PC"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""7a5e5822-8562-43af-ac40-3da6baa63977"",
                    ""path"": ""<Keyboard>/downArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""PC"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""0d02c384-d55d-4ec3-8388-1dea30598c19"",
                    ""path"": ""<Keyboard>/leftArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""PC"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""6b26849f-64a1-43ab-ae74-6ffef8a00789"",
                    ""path"": ""<Keyboard>/rightArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""PC"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""Scroll Wheel"",
                    ""id"": ""88e94179-13a4-4c4a-b105-cc007842144d"",
                    ""path"": ""1DAxis"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""PC"",
                    ""action"": ""Zoom"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""negative"",
                    ""id"": ""3804bad4-50f0-4665-87f3-169aa9385043"",
                    ""path"": ""<Keyboard>/minus"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""PC"",
                    ""action"": ""Zoom"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""positive"",
                    ""id"": ""21ea5bf1-14a8-461f-a2d8-754fcbbd4dd8"",
                    ""path"": ""<Keyboard>/equals"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""PC"",
                    ""action"": ""Zoom"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""8de313ef-0c8e-4663-8024-b180c958657e"",
                    ""path"": ""<Keyboard>/delete"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""PC"",
                    ""action"": ""RemoveCenterTile"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""61a284ab-7ef5-4af4-a6e8-cf69bf1d9367"",
                    ""path"": ""<Keyboard>/insert"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""PC"",
                    ""action"": ""AddCenterTile"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""8e121375-92f9-4ff4-bba0-2e055048c5f6"",
                    ""path"": ""<Keyboard>/backquote"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""DestroyAllTiles"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""c78681bf-fcd8-449e-89fd-81a1d8117432"",
                    ""path"": ""<Keyboard>/q"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""SaveMap"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""b1870cef-dd83-4757-b648-0f1673d4b33c"",
                    ""path"": ""<Keyboard>/e"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""LoadMap"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""In-Game"",
            ""id"": ""b75e16f5-78c6-43ae-8d6f-ea4704078fe3"",
            ""actions"": [
                {
                    ""name"": ""Move"",
                    ""type"": ""Button"",
                    ""id"": ""6702cc31-1dc5-44ef-a058-215fcad831ad"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": ""Press""
                },
                {
                    ""name"": ""Zoom"",
                    ""type"": ""Button"",
                    ""id"": ""91fa5e8f-2124-48ad-8172-e1d249906e20"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": ""Press""
                }
            ],
            ""bindings"": [
                {
                    ""name"": ""WASD"",
                    ""id"": ""afa983c2-ce8b-4ae0-89e7-ed114e49b964"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""PC"",
                    ""action"": ""Move"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""03e950a2-0f5a-493c-a2c0-db267c7e34ab"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""PC"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""ccf21b87-af37-4a1c-ae0f-0d99e4d57e6d"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""PC"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""36d1e240-bad4-43e8-b0b0-df55530eb27a"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""PC"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""d00b2672-901a-4a7f-b5f2-7150c333a42d"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""PC"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""Arrows"",
                    ""id"": ""70b12770-081e-4382-b1d1-88e18a6b79fd"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""PC"",
                    ""action"": ""Move"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""5bd7b973-4a99-46a7-9ba0-212078ca4a53"",
                    ""path"": ""<Keyboard>/upArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""PC"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""7b4e708f-9184-4d74-91e5-a75805126dbd"",
                    ""path"": ""<Keyboard>/downArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""PC"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""0d113272-0ff7-4e99-897c-eb2b0b7600e7"",
                    ""path"": ""<Keyboard>/leftArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""PC"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""7cd86c0f-cb19-47d0-96d2-e7861e53544c"",
                    ""path"": ""<Keyboard>/rightArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""PC"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""Scroll Wheel"",
                    ""id"": ""dcc3dee9-a145-4ef0-b839-add9b69bc5db"",
                    ""path"": ""1DAxis"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""PC"",
                    ""action"": ""Zoom"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""negative"",
                    ""id"": ""3c549430-a3ce-4404-8d65-376869586239"",
                    ""path"": ""<Keyboard>/minus"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""PC"",
                    ""action"": ""Zoom"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""positive"",
                    ""id"": ""ee205a8d-df70-4c27-aac8-c944a2c69189"",
                    ""path"": ""<Keyboard>/equals"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""PC"",
                    ""action"": ""Zoom"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                }
            ]
        },
        {
            ""name"": ""Menu"",
            ""id"": ""cdc4cdd4-d44b-49fb-a890-5c7ca64209ef"",
            ""actions"": [
                {
                    ""name"": ""New action"",
                    ""type"": ""Button"",
                    ""id"": ""d53baba8-629a-48df-91ad-b4bad1cc2aa5"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""b941238b-8ef5-4421-a345-7b984355fe39"",
                    ""path"": """",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""PC"",
                    ""action"": ""New action"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""Beats Editor"",
            ""id"": ""7746aa3a-7d91-4f64-8ee5-314994893c51"",
            ""actions"": [
                {
                    ""name"": ""StartRecording"",
                    ""type"": ""Button"",
                    ""id"": ""58f28ff7-b274-4ee6-896e-de50249eee00"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": ""Press""
                },
                {
                    ""name"": ""NewBeat"",
                    ""type"": ""Button"",
                    ""id"": ""e180b037-735f-4d5d-8956-075ef68ce3dd"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": ""Press""
                },
                {
                    ""name"": ""EndRecording"",
                    ""type"": ""Button"",
                    ""id"": ""1950efc9-9029-47e2-8455-1374a3d36f42"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": ""Press""
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""e3b45c8c-5dfc-4994-be39-67b63e9624aa"",
                    ""path"": ""<Keyboard>/f5"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""PC"",
                    ""action"": ""StartRecording"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""ad78e34e-4a7b-4dc1-ba1b-fa45eff38b3f"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""PC"",
                    ""action"": ""NewBeat"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""fecee741-068d-46f7-a4b6-81d6536b7dee"",
                    ""path"": ""<Keyboard>/f6"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""PC"",
                    ""action"": ""EndRecording"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": [
        {
            ""name"": ""PC"",
            ""bindingGroup"": ""PC"",
            ""devices"": [
                {
                    ""devicePath"": ""<Keyboard>"",
                    ""isOptional"": false,
                    ""isOR"": false
                },
                {
                    ""devicePath"": ""<Mouse>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        }
    ]
}");
        // Map Editor
        m_MapEditor = asset.FindActionMap("Map Editor", throwIfNotFound: true);
        m_MapEditor_Move = m_MapEditor.FindAction("Move", throwIfNotFound: true);
        m_MapEditor_Zoom = m_MapEditor.FindAction("Zoom", throwIfNotFound: true);
        m_MapEditor_RemoveCenterTile = m_MapEditor.FindAction("RemoveCenterTile", throwIfNotFound: true);
        m_MapEditor_AddCenterTile = m_MapEditor.FindAction("AddCenterTile", throwIfNotFound: true);
        m_MapEditor_DestroyAllTiles = m_MapEditor.FindAction("DestroyAllTiles", throwIfNotFound: true);
        m_MapEditor_SaveMap = m_MapEditor.FindAction("SaveMap", throwIfNotFound: true);
        m_MapEditor_LoadMap = m_MapEditor.FindAction("LoadMap", throwIfNotFound: true);
        // In-Game
        m_InGame = asset.FindActionMap("In-Game", throwIfNotFound: true);
        m_InGame_Move = m_InGame.FindAction("Move", throwIfNotFound: true);
        m_InGame_Zoom = m_InGame.FindAction("Zoom", throwIfNotFound: true);
        // Menu
        m_Menu = asset.FindActionMap("Menu", throwIfNotFound: true);
        m_Menu_Newaction = m_Menu.FindAction("New action", throwIfNotFound: true);
        // Beats Editor
        m_BeatsEditor = asset.FindActionMap("Beats Editor", throwIfNotFound: true);
        m_BeatsEditor_StartRecording = m_BeatsEditor.FindAction("StartRecording", throwIfNotFound: true);
        m_BeatsEditor_NewBeat = m_BeatsEditor.FindAction("NewBeat", throwIfNotFound: true);
        m_BeatsEditor_EndRecording = m_BeatsEditor.FindAction("EndRecording", throwIfNotFound: true);
    }

    ~Controls()
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

    // Map Editor
    private readonly InputActionMap m_MapEditor;
    private IMapEditorActions m_MapEditorActionsCallbackInterface;
    private readonly InputAction m_MapEditor_Move;
    private readonly InputAction m_MapEditor_Zoom;
    private readonly InputAction m_MapEditor_RemoveCenterTile;
    private readonly InputAction m_MapEditor_AddCenterTile;
    private readonly InputAction m_MapEditor_DestroyAllTiles;
    private readonly InputAction m_MapEditor_SaveMap;
    private readonly InputAction m_MapEditor_LoadMap;
    public struct MapEditorActions
    {
        private Controls m_Wrapper;
        public MapEditorActions(Controls wrapper) { m_Wrapper = wrapper; }
        public InputAction @Move => m_Wrapper.m_MapEditor_Move;
        public InputAction @Zoom => m_Wrapper.m_MapEditor_Zoom;
        public InputAction @RemoveCenterTile => m_Wrapper.m_MapEditor_RemoveCenterTile;
        public InputAction @AddCenterTile => m_Wrapper.m_MapEditor_AddCenterTile;
        public InputAction @DestroyAllTiles => m_Wrapper.m_MapEditor_DestroyAllTiles;
        public InputAction @SaveMap => m_Wrapper.m_MapEditor_SaveMap;
        public InputAction @LoadMap => m_Wrapper.m_MapEditor_LoadMap;
        public InputActionMap Get() { return m_Wrapper.m_MapEditor; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(MapEditorActions set) { return set.Get(); }
        public void SetCallbacks(IMapEditorActions instance)
        {
            if (m_Wrapper.m_MapEditorActionsCallbackInterface != null)
            {
                Move.started -= m_Wrapper.m_MapEditorActionsCallbackInterface.OnMove;
                Move.performed -= m_Wrapper.m_MapEditorActionsCallbackInterface.OnMove;
                Move.canceled -= m_Wrapper.m_MapEditorActionsCallbackInterface.OnMove;
                Zoom.started -= m_Wrapper.m_MapEditorActionsCallbackInterface.OnZoom;
                Zoom.performed -= m_Wrapper.m_MapEditorActionsCallbackInterface.OnZoom;
                Zoom.canceled -= m_Wrapper.m_MapEditorActionsCallbackInterface.OnZoom;
                RemoveCenterTile.started -= m_Wrapper.m_MapEditorActionsCallbackInterface.OnRemoveCenterTile;
                RemoveCenterTile.performed -= m_Wrapper.m_MapEditorActionsCallbackInterface.OnRemoveCenterTile;
                RemoveCenterTile.canceled -= m_Wrapper.m_MapEditorActionsCallbackInterface.OnRemoveCenterTile;
                AddCenterTile.started -= m_Wrapper.m_MapEditorActionsCallbackInterface.OnAddCenterTile;
                AddCenterTile.performed -= m_Wrapper.m_MapEditorActionsCallbackInterface.OnAddCenterTile;
                AddCenterTile.canceled -= m_Wrapper.m_MapEditorActionsCallbackInterface.OnAddCenterTile;
                DestroyAllTiles.started -= m_Wrapper.m_MapEditorActionsCallbackInterface.OnDestroyAllTiles;
                DestroyAllTiles.performed -= m_Wrapper.m_MapEditorActionsCallbackInterface.OnDestroyAllTiles;
                DestroyAllTiles.canceled -= m_Wrapper.m_MapEditorActionsCallbackInterface.OnDestroyAllTiles;
                SaveMap.started -= m_Wrapper.m_MapEditorActionsCallbackInterface.OnSaveMap;
                SaveMap.performed -= m_Wrapper.m_MapEditorActionsCallbackInterface.OnSaveMap;
                SaveMap.canceled -= m_Wrapper.m_MapEditorActionsCallbackInterface.OnSaveMap;
                LoadMap.started -= m_Wrapper.m_MapEditorActionsCallbackInterface.OnLoadMap;
                LoadMap.performed -= m_Wrapper.m_MapEditorActionsCallbackInterface.OnLoadMap;
                LoadMap.canceled -= m_Wrapper.m_MapEditorActionsCallbackInterface.OnLoadMap;
            }
            m_Wrapper.m_MapEditorActionsCallbackInterface = instance;
            if (instance != null)
            {
                Move.started += instance.OnMove;
                Move.performed += instance.OnMove;
                Move.canceled += instance.OnMove;
                Zoom.started += instance.OnZoom;
                Zoom.performed += instance.OnZoom;
                Zoom.canceled += instance.OnZoom;
                RemoveCenterTile.started += instance.OnRemoveCenterTile;
                RemoveCenterTile.performed += instance.OnRemoveCenterTile;
                RemoveCenterTile.canceled += instance.OnRemoveCenterTile;
                AddCenterTile.started += instance.OnAddCenterTile;
                AddCenterTile.performed += instance.OnAddCenterTile;
                AddCenterTile.canceled += instance.OnAddCenterTile;
                DestroyAllTiles.started += instance.OnDestroyAllTiles;
                DestroyAllTiles.performed += instance.OnDestroyAllTiles;
                DestroyAllTiles.canceled += instance.OnDestroyAllTiles;
                SaveMap.started += instance.OnSaveMap;
                SaveMap.performed += instance.OnSaveMap;
                SaveMap.canceled += instance.OnSaveMap;
                LoadMap.started += instance.OnLoadMap;
                LoadMap.performed += instance.OnLoadMap;
                LoadMap.canceled += instance.OnLoadMap;
            }
        }
    }
    public MapEditorActions @MapEditor => new MapEditorActions(this);

    // In-Game
    private readonly InputActionMap m_InGame;
    private IInGameActions m_InGameActionsCallbackInterface;
    private readonly InputAction m_InGame_Move;
    private readonly InputAction m_InGame_Zoom;
    public struct InGameActions
    {
        private Controls m_Wrapper;
        public InGameActions(Controls wrapper) { m_Wrapper = wrapper; }
        public InputAction @Move => m_Wrapper.m_InGame_Move;
        public InputAction @Zoom => m_Wrapper.m_InGame_Zoom;
        public InputActionMap Get() { return m_Wrapper.m_InGame; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(InGameActions set) { return set.Get(); }
        public void SetCallbacks(IInGameActions instance)
        {
            if (m_Wrapper.m_InGameActionsCallbackInterface != null)
            {
                Move.started -= m_Wrapper.m_InGameActionsCallbackInterface.OnMove;
                Move.performed -= m_Wrapper.m_InGameActionsCallbackInterface.OnMove;
                Move.canceled -= m_Wrapper.m_InGameActionsCallbackInterface.OnMove;
                Zoom.started -= m_Wrapper.m_InGameActionsCallbackInterface.OnZoom;
                Zoom.performed -= m_Wrapper.m_InGameActionsCallbackInterface.OnZoom;
                Zoom.canceled -= m_Wrapper.m_InGameActionsCallbackInterface.OnZoom;
            }
            m_Wrapper.m_InGameActionsCallbackInterface = instance;
            if (instance != null)
            {
                Move.started += instance.OnMove;
                Move.performed += instance.OnMove;
                Move.canceled += instance.OnMove;
                Zoom.started += instance.OnZoom;
                Zoom.performed += instance.OnZoom;
                Zoom.canceled += instance.OnZoom;
            }
        }
    }
    public InGameActions @InGame => new InGameActions(this);

    // Menu
    private readonly InputActionMap m_Menu;
    private IMenuActions m_MenuActionsCallbackInterface;
    private readonly InputAction m_Menu_Newaction;
    public struct MenuActions
    {
        private Controls m_Wrapper;
        public MenuActions(Controls wrapper) { m_Wrapper = wrapper; }
        public InputAction @Newaction => m_Wrapper.m_Menu_Newaction;
        public InputActionMap Get() { return m_Wrapper.m_Menu; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(MenuActions set) { return set.Get(); }
        public void SetCallbacks(IMenuActions instance)
        {
            if (m_Wrapper.m_MenuActionsCallbackInterface != null)
            {
                Newaction.started -= m_Wrapper.m_MenuActionsCallbackInterface.OnNewaction;
                Newaction.performed -= m_Wrapper.m_MenuActionsCallbackInterface.OnNewaction;
                Newaction.canceled -= m_Wrapper.m_MenuActionsCallbackInterface.OnNewaction;
            }
            m_Wrapper.m_MenuActionsCallbackInterface = instance;
            if (instance != null)
            {
                Newaction.started += instance.OnNewaction;
                Newaction.performed += instance.OnNewaction;
                Newaction.canceled += instance.OnNewaction;
            }
        }
    }
    public MenuActions @Menu => new MenuActions(this);

    // Beats Editor
    private readonly InputActionMap m_BeatsEditor;
    private IBeatsEditorActions m_BeatsEditorActionsCallbackInterface;
    private readonly InputAction m_BeatsEditor_StartRecording;
    private readonly InputAction m_BeatsEditor_NewBeat;
    private readonly InputAction m_BeatsEditor_EndRecording;
    public struct BeatsEditorActions
    {
        private Controls m_Wrapper;
        public BeatsEditorActions(Controls wrapper) { m_Wrapper = wrapper; }
        public InputAction @StartRecording => m_Wrapper.m_BeatsEditor_StartRecording;
        public InputAction @NewBeat => m_Wrapper.m_BeatsEditor_NewBeat;
        public InputAction @EndRecording => m_Wrapper.m_BeatsEditor_EndRecording;
        public InputActionMap Get() { return m_Wrapper.m_BeatsEditor; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(BeatsEditorActions set) { return set.Get(); }
        public void SetCallbacks(IBeatsEditorActions instance)
        {
            if (m_Wrapper.m_BeatsEditorActionsCallbackInterface != null)
            {
                StartRecording.started -= m_Wrapper.m_BeatsEditorActionsCallbackInterface.OnStartRecording;
                StartRecording.performed -= m_Wrapper.m_BeatsEditorActionsCallbackInterface.OnStartRecording;
                StartRecording.canceled -= m_Wrapper.m_BeatsEditorActionsCallbackInterface.OnStartRecording;
                NewBeat.started -= m_Wrapper.m_BeatsEditorActionsCallbackInterface.OnNewBeat;
                NewBeat.performed -= m_Wrapper.m_BeatsEditorActionsCallbackInterface.OnNewBeat;
                NewBeat.canceled -= m_Wrapper.m_BeatsEditorActionsCallbackInterface.OnNewBeat;
                EndRecording.started -= m_Wrapper.m_BeatsEditorActionsCallbackInterface.OnEndRecording;
                EndRecording.performed -= m_Wrapper.m_BeatsEditorActionsCallbackInterface.OnEndRecording;
                EndRecording.canceled -= m_Wrapper.m_BeatsEditorActionsCallbackInterface.OnEndRecording;
            }
            m_Wrapper.m_BeatsEditorActionsCallbackInterface = instance;
            if (instance != null)
            {
                StartRecording.started += instance.OnStartRecording;
                StartRecording.performed += instance.OnStartRecording;
                StartRecording.canceled += instance.OnStartRecording;
                NewBeat.started += instance.OnNewBeat;
                NewBeat.performed += instance.OnNewBeat;
                NewBeat.canceled += instance.OnNewBeat;
                EndRecording.started += instance.OnEndRecording;
                EndRecording.performed += instance.OnEndRecording;
                EndRecording.canceled += instance.OnEndRecording;
            }
        }
    }
    public BeatsEditorActions @BeatsEditor => new BeatsEditorActions(this);
    private int m_PCSchemeIndex = -1;
    public InputControlScheme PCScheme
    {
        get
        {
            if (m_PCSchemeIndex == -1) m_PCSchemeIndex = asset.FindControlSchemeIndex("PC");
            return asset.controlSchemes[m_PCSchemeIndex];
        }
    }
    public interface IMapEditorActions
    {
        void OnMove(InputAction.CallbackContext context);
        void OnZoom(InputAction.CallbackContext context);
        void OnRemoveCenterTile(InputAction.CallbackContext context);
        void OnAddCenterTile(InputAction.CallbackContext context);
        void OnDestroyAllTiles(InputAction.CallbackContext context);
        void OnSaveMap(InputAction.CallbackContext context);
        void OnLoadMap(InputAction.CallbackContext context);
    }
    public interface IInGameActions
    {
        void OnMove(InputAction.CallbackContext context);
        void OnZoom(InputAction.CallbackContext context);
    }
    public interface IMenuActions
    {
        void OnNewaction(InputAction.CallbackContext context);
    }
    public interface IBeatsEditorActions
    {
        void OnStartRecording(InputAction.CallbackContext context);
        void OnNewBeat(InputAction.CallbackContext context);
        void OnEndRecording(InputAction.CallbackContext context);
    }
}
