using Godot;

public partial class PlayerBody : CharacterBody3D
{
	[Export] public float MouseSensitivity = 0.2f;
	[Export] public float MoveSpeed = 5f;
	[Export] public float MinPitch = -60f;
	[Export] public float MaxPitch = 60f;

	private float _yaw = 0f;
	private float _pitch = 0f;

	private Node3D _cameraPivot;
	private Node3D _cameraPitch;

	public override void _Ready()
	{
		Input.MouseMode = Input.MouseModeEnum.Captured;

		_cameraPivot = GetNode<Node3D>("CameraPivot");
		_cameraPitch = GetNode<Node3D>("CameraPivot/CameraPitch");
	}

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseMotion motion)
		{
			_yaw -= motion.Relative.X * MouseSensitivity;
			_pitch = Mathf.Clamp(_pitch - motion.Relative.Y * MouseSensitivity, MinPitch, MaxPitch);

			_cameraPivot.Rotation = new Vector3(0, Mathf.DegToRad(_yaw), 0);
			_cameraPitch.Rotation = new Vector3(Mathf.DegToRad(_pitch), 0, 0);
		}

		if (@event is InputEventKey key && key.Pressed && key.Keycode == Key.Escape)
		{
			Input.MouseMode = Input.MouseMode == Input.MouseModeEnum.Captured
				? Input.MouseModeEnum.Visible
				: Input.MouseModeEnum.Captured;
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		Vector2 input2D = Input.GetVector("move_left", "move_right", "move_forward", "move_back");
		Vector3 inputDir = new Vector3(input2D.X, 0, input2D.Y);


		Vector3 forward = _cameraPivot.Transform.Basis.Z;
		Vector3 right = _cameraPivot.Transform.Basis.X;

		Vector3 moveDir = (-forward * inputDir.Z + right * inputDir.X).Normalized();

		Velocity = new Vector3(moveDir.X * MoveSpeed, Velocity.Y, moveDir.Z * MoveSpeed);
		MoveAndSlide();
	}
}
