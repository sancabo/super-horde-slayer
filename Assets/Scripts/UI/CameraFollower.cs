using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollower : MonoBehaviour
{
    //
    [SerializeField] private Transform _player;
    [SerializeField] private float _smoothSpeed = 0.5f;
    [SerializeField] private Collider2D _cameraBounds;
    private Vector3 _smoothPos;
    private float _xMin, _xMax, _yMin, _yMax;
    private float _camY,_camX;

    private float _camOrthsize;
    private float cameraRatio;

    private Camera _camera;

    private void Start()
    {
        //we obtain all variables on  start so we don't need to recalculate
        _xMin = _cameraBounds.bounds.min.x;
        _xMax = _cameraBounds.bounds.max.x;
        _yMin = _cameraBounds.bounds.min.y;
        _yMax = _cameraBounds.bounds.max.y;
        _camera = GetComponent<Camera>();
        // field of view. Half of the camera size.the amount of space that can go out of bounds
        _camOrthsize = 0f; // _camera.orthographicSize / 2f;
    }

    void FixedUpdate()
    {
        if (_player != null)
        {
            //Set to player pos, but it cannot move anymore if half the camera field would exceed the bounds collider.
            _camY = Mathf.Clamp(_player.position.y, _yMin + _camOrthsize, _yMax - _camOrthsize);
            _camX = Mathf.Clamp(_player.position.x, _xMin + _camOrthsize, _xMax - _camOrthsize);
            _smoothPos = Vector3.Lerp(transform.position, new Vector3(_camX, _camY, transform.position.z), _smoothSpeed);
            transform.position = _smoothPos;

        }
    }
}
