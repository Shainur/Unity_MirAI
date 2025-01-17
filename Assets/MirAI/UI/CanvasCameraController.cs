﻿using Assets.MirAI.AiEditor;
using Assets.MirAI.Utils;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.MirAI.UI {

    [RequireComponent(typeof(Canvas))]
    public class CanvasCameraController : MonoBehaviour, IBeginDragHandler, IDragHandler, IPointerDownHandler, IPointerUpHandler {

        [SerializeField] private UnityEvent _onClick;
        [Header("Selection tool")]
        [SerializeField] private RectTransform _selection;
        [SerializeField] private Image _selectionButtonImage;

        private RectTransform _canvasRectTransform;
        private float _newCameraSize;
        private Vector2 _cameraFlyDirection;
        private readonly float _cameraFlyInertia = 1.05f;
        private readonly float _cameraResizeInertia = 30f;
        private readonly float _deltaMoveCameraDivider = 300f;   // Magic value ??
        private bool _isDragging;
        private bool _selectionMode = false;
        private Camera _camera;

        private void Start() {
            _camera = GetComponent<Canvas>().worldCamera;
            _canvasRectTransform = GetComponent<RectTransform>();
            _newCameraSize = (int)_camera.orthographicSize;
            _isDragging = false;
        }

        private void Update() {
            if ((int)_newCameraSize != (int)_camera.orthographicSize) {
                var dSize = (_newCameraSize - _camera.orthographicSize) / _cameraResizeInertia;
                _camera.orthographicSize += dSize;
                ConfineCameraInCanvas();
            }
            if (_cameraFlyDirection != Vector2.zero) {
                _camera.transform.position -= (Vector3)_cameraFlyDirection;
                _cameraFlyDirection /= _cameraFlyInertia;
                ConfineCameraInCanvas();
            }
        }

        public void SetCameraViewport(Rect rect) {
            _camera.transform.position = new Vector3(rect.center.x, rect.center.y, _camera.transform.position.z);
            var h1 = rect.height;
            var h2 = rect.width / _camera.aspect;
            _newCameraSize = (Mathf.Max(h1, h2) / 1.6f);
        }

        public void ChangeCameraSize(Vector2 wheelVector) {
            if (wheelVector.y == 0) return;
            var _scaleFactor = wheelVector.y > 0 ? 0.75f : 1.33f;
            var newSize = _camera.orthographicSize * _scaleFactor;
            newSize = Mathf.Clamp(newSize, 100, _canvasRectTransform.rect.height / 2);
            _newCameraSize = newSize;
        }

        public void OnDrag(PointerEventData eventData) {
            if (_selectionMode) {
                _currentPoint = eventData.pointerCurrentRaycast.worldPosition / _selection.localScale.x;
                SetSelectorRect();
            }
            else {
                var delta = eventData.delta * _camera.orthographicSize / _deltaMoveCameraDivider;
                _cameraFlyDirection = new Vector2(delta.x, delta.y);
            }
        }

        private void ConfineCameraInCanvas() {
            bool isChanged = false;
            Rect cameraRect = GetCameraRect();
            var canvasRect = _canvasRectTransform.GetWorldRect();
            if (cameraRect.xMin < canvasRect.xMin) {
                cameraRect.xMin = canvasRect.xMin;
                isChanged = true;
            }
            if (cameraRect.yMin < canvasRect.yMin) {
                cameraRect.yMin = canvasRect.yMin;
                isChanged = true;
            }
            if (cameraRect.xMax > canvasRect.xMax) {
                cameraRect.xMax = canvasRect.xMax;
                isChanged = true;
            }
            if (cameraRect.yMax > canvasRect.yMax) {
                cameraRect.yMax = canvasRect.yMax;
                isChanged = true;
            }
            if (isChanged)
                _camera.transform.position = new Vector3(cameraRect.center.x, cameraRect.center.y, _camera.transform.position.z);
        }

        public Rect GetCameraRect() {
            var height = 2 * _camera.orthographicSize;
            var width = height * _camera.aspect;
            var left = _camera.transform.position.x - width / 2;
            var bottom = _camera.transform.position.y - height / 2;
            return new Rect(left, bottom, width, height);
        }

        private Vector3 _beginPoint;
        private Vector3 _currentPoint;

        public void OnPointerDown(PointerEventData eventData) {
            if (!_selectionMode) return;
            _beginPoint = eventData.pointerCurrentRaycast.worldPosition;
            _currentPoint = _beginPoint;
            SetSelectorRect();
            _selection.gameObject.SetActive(true);
        }

        public void SetSelectorRect() {
            var xMin = Mathf.Min(_beginPoint.x, _currentPoint.x);
            var xMax = Mathf.Max(_beginPoint.x, _currentPoint.x);
            var yMin = Mathf.Min(_beginPoint.y, _currentPoint.y);
            var yMax = Mathf.Max(_beginPoint.y, _currentPoint.y);
            _selection.position = new Vector3(xMin, yMin, 0);
            _selection.sizeDelta = new Vector2(xMax - xMin, yMax - yMin);
        }

        public void OnPointerUp(PointerEventData eventData) {
            if (_selectionMode) {
                var editor = gameObject.GetComponent<EditorController>();
                if(editor != null) 
                    editor.OnSelection(_selection.GetWorldRect());

                _selection.gameObject.SetActive(false);
                SelectionMode(false);
            }
            else if (!_isDragging)
                _onClick?.Invoke();

            _isDragging = false;
        }

        public void OnBeginDrag(PointerEventData eventData) {
            _isDragging = true;
        }

        public void SelectionMode(bool state) {
            _selectionMode = state;
            _selectionButtonImage.color = state ? Color.green : Color.white;
        }
    }
}