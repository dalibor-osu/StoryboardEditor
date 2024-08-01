namespace StoryboardEditor.Types;

public struct Color3
{
    public ushort Red;
    public ushort Green;
    public ushort Blue;

    public ushort this[int key]
    {
        get
        {
            return key switch
            {
                0 => Red,
                1 => Green,
                2 => Blue,
                _ => throw new ArgumentOutOfRangeException(
                    $"Index {key} out of range. Color3 index must be 0 or greater and less than 3.")
            };
        }
        set
        {
            switch (key)
            {
                case 0:
                    Red = value;
                    break;
                case 1:
                    Green = value;
                    break;
                case 2:
                    Blue = value;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(
                        $"Index {key} out of range. Color3 index must be 0 or greater and less than 3.");
            }
        }
    }
}