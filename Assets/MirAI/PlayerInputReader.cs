﻿using Assets.MirAI.UI;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Assets.MirAI {

    public class PlayerInputReader : MonoBehaviour {

        [SerializeField] private EditorCanvasController _controller;

        public void OnMouseWheel(InputAction.CallbackContext context) {
            var v = context.ReadValue<Vector2>();
            _controller.ChangeCanvasScale(v);
        }
    }
}