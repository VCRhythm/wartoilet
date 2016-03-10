using DG.Tweening;

public class Swing {

    public enum Type
    {
        Slash,
        Full
    }

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
                return 0.3f;
            case Type.Full:
                return 0.5f;
            default:
                return 0.5f;
        }
    }

    private float GetRotationAngle(Type type, Position position)
    {
        switch (type)
        {
            case Type.Slash:
                if (position == Position.Left)
                {
                    return 405;
                }
                else
                {
                    return -45;
                }
            case Type.Full:
                if (position == Position.Left)
                {
                    return 460;
                }
                else
                {
                    return -100;
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
                    return 300;
                }
                else
                {
                    return 60;
                }
            case Type.Full:
                if (position == Position.Left)
                {
                    return 230;
                }
                else
                {
                    return 130;
                }
            default:
                return 0;
        }
    }
}