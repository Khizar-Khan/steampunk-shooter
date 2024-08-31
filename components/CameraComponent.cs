using System;
using Godot;

namespace SteampunkShooter.components;

public partial class CameraComponent : Component
{
    // Constant Node Paths
    private const string CameraControllerPath = "../CameraContainer/CameraController";
    private const string CameraContainerPath = "../CameraContainer";
    
    // References
    public CharacterBody3D CharacterBody { get; private set; }
    public Node3D CameraContainer { get; private set; }
    public Node3D CameraController { get; private set; }

    [ExportCategory("Camera Settings")]
    [Export] private float _rotationSensitivity = 0.1f; // Sensitivity in degrees per pixel
    [Export] private float _maxVerticalAngle = 89.0f;
    [Export] private float _minVerticalAngle = -89.0f;
    
    // Cached Values
    private Vector2 _inputDirection;

    protected override void Initialise()
    {
        base.Initialise();

        CharacterBody = Owner as CharacterBody3D ?? throw new NullReferenceException("CameraComponent's Owner is not of type CharacterBody3D or is null.");
        CameraContainer = GetNode<Node3D>(CameraContainerPath) ?? throw new NullReferenceException("CameraContainer not found.");
        CameraController = GetNode<Node3D>(CameraControllerPath) ?? throw new NullReferenceException("CameraController not found.");

        EnableMouseCapture(true);
    }

    private void SetSensitivity(float sensitivity)
    {
        _rotationSensitivity = Mathf.Clamp(sensitivity, 0.01f, 10.0f);
    }

    private void EnableMouseCapture(bool capture)
    {
        if (!IsEnabled)
            return;
        
        Input.SetMouseMode(capture ? Input.MouseModeEnum.Captured : Input.MouseModeEnum.Visible);
    }

    public Vector2 GetInputDirection()
    {
        return _inputDirection;
    }

    // Signal Event Handlers
    public void OnMouseMotion(Vector2 mouseDelta)
    {
        if (!IsEnabled)
            return;

        // Horizontal rotation
        CharacterBody.RotateY(Mathf.DegToRad(-mouseDelta.X * _rotationSensitivity));

        // Vertical rotation
        CameraController.RotationDegrees = new Vector3(
            Mathf.Clamp(CameraController.RotationDegrees.X + -mouseDelta.Y * _rotationSensitivity, _minVerticalAngle, _maxVerticalAngle),
            CameraController.RotationDegrees.Y,
            CameraController.RotationDegrees.Z
        );
    }
    
    public void OnMovementInput(Vector2 inputDirection)
    {
        _inputDirection = inputDirection;
    }
}