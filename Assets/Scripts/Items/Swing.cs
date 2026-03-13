using DG.Tweening;

namespace WarToilet.Items
{
public class Swing {

    public enum Type
    {
        Slash,
        Full
    }

    private const float SlashSpeed = 0.3f;
    private const float FullSpeed = 0.5f;
    private const float SlashLeftBackSwing = 300f;
    private const float SlashRightBackSwing = 60f;
    private const float FullLeftBackSwing = 230f;
    private const float FullRightBackSwing = 130f;
    private const float SlashLeftFollowThrough = 405f;
    private const float SlashRightFollowThrough = -45f;
    private const float FullLeftFollowThrough = 460f;
    private const float FullRightFollowThrough = -100f;

    public Type type;
    public Position position;
    public float speed;
    public float backSwingRotationAngle;
    public float followThroughRotationAngle;
    public RotateMode followThroughRotateMode;

    public Swing(Type type, Position position)
    {
        this.type = type;
        this.position = position;
        followThroughRotateMode = GetRotateMode(position);
        backSwingRotationAngle = GetBackSwingRotationAngle(type, position);
        followThroughRotationAngle = GetRotationAngle(type, position);
        speed = GetSpeed(type);
    }

    private RotateMode GetRotateMode(Position position)
    {
        switch(position)
        {
            default:
                return RotateMode.FastBeyond360;
        }
    }

    private float GetSpeed(Type type)
    {
        switch(type)
        {
            case Type.Slash:
                return SlashSpeed;
            case Type.Full:
                return FullSpeed;
            default:
                return FullSpeed;
        }
    }

    private float GetRotationAngle(Type type, Position position)
    {
        switch (type)
        {
            case Type.Slash:
                if (position == Position.Left)
                {
                    return SlashLeftFollowThrough;
                }
                else
                {
                    return SlashRightFollowThrough;
                }
            case Type.Full:
                if (position == Position.Left)
                {
                    return FullLeftFollowThrough;
                }
                else
                {
                    return FullRightFollowThrough;
                }
            default:
                return 0;
        }
    }

    private float GetBackSwingRotationAngle(Type type, Position position)
    {
        switch(type)
        {
            case Type.Slash:
                if (position == Position.Left)
                {
                    return SlashLeftBackSwing;
                }
                else
                {
                    return SlashRightBackSwing;
                }
            case Type.Full:
                if (position == Position.Left)
                {
                    return FullLeftBackSwing;
                }
                else
                {
                    return FullRightBackSwing;
                }
            default:
                return 0;
        }
    }
}
}
