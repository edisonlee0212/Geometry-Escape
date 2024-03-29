﻿using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.InputSystem;
using static Unity.Mathematics.math;

namespace GeometryEscape
{
    public enum ControlMode
    {
        NoControl,
        Menu,
        InGame,
        MapEditor,
        BeatsEditor
    }

    /// <summary>
    /// This system receive user control and perform actions.
    /// </summary>
    public class ControlSystem
    {
        #region Private
        #endregion

        #region Public
        private static ControlMode _ControlMode;
        private static Controls _InputSystem;
        public static Controls InputSystem { get => _InputSystem; set => _InputSystem = value; }
        public static ControlMode ControlMode
        {
            get => _ControlMode;
            set
            {
                if (_InputSystem != null)
                {
                    switch (_ControlMode)
                    {
                        case ControlMode.Menu:
                            _InputSystem.Menu.Disable();
                            break;
                        case ControlMode.InGame:
                            _InputSystem.InGame.Disable();
                            break;
                        case ControlMode.MapEditor:
                            _InputSystem.MapEditor.Disable();
                            break;
                        case ControlMode.BeatsEditor:
                            _InputSystem.BeatsEditor.Disable();
                            break;
                        default:
                            break;
                    }
                    switch (value)
                    {
                        case ControlMode.Menu:
                            _InputSystem.Menu.Enable();
                            break;
                        case ControlMode.InGame:
                            _InputSystem.InGame.Enable();
                            break;
                        case ControlMode.MapEditor:
                            _InputSystem.MapEditor.Enable();
                            break;
                        case ControlMode.BeatsEditor:
                            _InputSystem.BeatsEditor.Enable();
                            break;
                        default:
                            break;
                    }
                }
               
                _ControlMode = value;
            }
        }
        #endregion


        public ControlSystem()
        {
            _InputSystem = new Controls();
            //In-Game
            _InputSystem.InGame.Disable();
            _InputSystem.InGame.Move.performed += ctx => Move(ctx);
            _InputSystem.InGame.Zoom.performed += ctx => ZoomMap(ctx);
            //Map Editor
            _InputSystem.MapEditor.Disable();
            _InputSystem.MapEditor.Move.performed += ctx => Move(ctx);
            _InputSystem.MapEditor.Zoom.performed += ctx => ZoomMap(ctx);
            _InputSystem.MapEditor.AddCenterTile.performed += ctx => MapEditorAddCenterTile(ctx);
            _InputSystem.MapEditor.RemoveCenterTile.performed += ctx => MapEditorRemoveCenterTile(ctx);
            _InputSystem.MapEditor.DestroyAllTiles.performed += ctx => MapEditorDestroyAllTiles(ctx);
            _InputSystem.MapEditor.AddMonster.performed += ctx => MapEditorAddCenterMonster(ctx);
            _InputSystem.MapEditor.SaveMap.performed += ctx => MapEditorSaveMap(ctx);
            _InputSystem.MapEditor.LoadMap.performed += ctx => MapEditorLoadMap(ctx);
            _InputSystem.MapEditor.SelectLeft.performed += ctx => MapEditorSelectLeft(ctx);
            _InputSystem.MapEditor.SelectRight.performed += ctx => MapEditorSelectRight(ctx);

            //Menu
            _InputSystem.Menu.Disable();

            _InputSystem.BeatsEditor.Disable();
            _InputSystem.BeatsEditor.StartRecording.performed += ctx => BeatsEditorStartRecording(ctx);
            _InputSystem.BeatsEditor.NewBeat.performed += ctx => BeatsEditorNewBeat(ctx);
            _InputSystem.BeatsEditor.EndRecording.performed += ctx => BeatsEditorEndRecording(ctx);
        }

        private void MapEditorSelectLeft(InputAction.CallbackContext ctx)
        {
            Debug.Log("selectleft");
        }

        private static void MapEditorSelectRight(InputAction.CallbackContext ctx)
        {
            Debug.Log("selectright");
        }

        private static void MapEditorSaveMap(InputAction.CallbackContext ctx)
        {
            WorldSystem.SaveMapByName("TestGround");
        }

        private static void MapEditorLoadMap(InputAction.CallbackContext ctx)
        {
            WorldSystem.LoadMapByName("TestGround");
        }

        private static void MapEditorDestroyAllTiles(InputAction.CallbackContext ctx)
        {
            WorldSystem.UnloadMap();
        }

        private static void BeatsEditorStartRecording(InputAction.CallbackContext ctx)
        {
            AudioSystem.StartRecording();
        }

        private static void BeatsEditorNewBeat(InputAction.CallbackContext ctx)
        {
            AudioSystem.RecordBeat();
        }

        private static void BeatsEditorEndRecording(InputAction.CallbackContext ctx)
        {
            AudioSystem.EndRecording();
        }

        private static void Move(InputAction.CallbackContext ctx)
        {
            AudioSystem.PlayKeySound();
            // UISystem.Displaypopup(1);
            CentralSystem.Move(ctx.ReadValue<Vector2>());
        }

        private static void ZoomMap(InputAction.CallbackContext ctx)
        {
            CentralSystem.Zoom(ctx.ReadValue<float>());
        }

        private static void MapEditorAddCenterTile(InputAction.CallbackContext ctx)
        {
            WorldSystem.AddCenterTile();
        }

        private static void MapEditorAddCenterMonster(InputAction.CallbackContext ctx)
        {
            WorldSystem.AddCenterMonster();
        }

        private static void MapEditorRemoveCenterTile(InputAction.CallbackContext ctx)
        {
            WorldSystem.RemoveCenterTile();
        }
    }
}