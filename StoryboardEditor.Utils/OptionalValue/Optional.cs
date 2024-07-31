namespace StoryboardEditor.Utils.OptionalValue;

public sealed class Optional<T>
{
    public bool HasValue { get; }
    public bool HasError => !HasValue;
    
    private readonly OptionalError? _error;

    public OptionalError Error
    {
        get
        {
            if (HasValue)
            {
                throw new InvalidOperationException("Optional value doesn't have an error.");
            }

            return _error!;
        }
    }
    
    private readonly T? _value;
    public T Value
    {
        get
        {
            if (!HasValue)
            {
                throw new InvalidOperationException("Tried accessing an optional value without value.");
            }

            return _value!;
        }
    }
    
    public Optional(T? value)
    {
        if (value == null)
        {
            HasValue = false;
            return;
        }
        
        _value = value;
        HasValue = true;
    }

    public Optional(OptionalError error)
    {
        HasValue = false;
        _error = error;
    }
    
    public static explicit operator T(Optional<T> optional)
    {
        return optional.Value;
    }
    
    public static implicit operator Optional<T>(T value)
    {
        return new Optional<T>(value);
    }
    
    public static implicit operator Optional<T>(OptionalError error)
    {
        return new Optional<T>(error);
    }
    
    public override bool Equals(object? obj)
    {
        if (obj is Optional<T> optional)
        {
            return Equals(optional);
        }
        
        return false;
    }

    public bool Equals(Optional<T> other)
    {
        if (HasValue && other.HasValue)
        {
            return Equals(_value, other._value);
        }
        
        return HasValue == other.HasValue;
    }
    
    public override int GetHashCode()
    {
        return HashCode.Combine(_error, _value, HasValue);
    }
}