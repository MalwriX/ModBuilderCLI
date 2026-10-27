namespace ModBuilderCLI.Core.Misc;

public class Stainable<T>(T value, Action<Stainable<T>>? onStain = null)
{
    private T _value = value;

    public T Value
    {
        get => _value;
        set
        {
            if (EqualityComparer<T>.Default.Equals(_value, value)) return;
            _value = value;
            Stain();
        }
    }

    public bool IsDirty { get; private set; }
    public Action<Stainable<T>>? OnStain { get; set; } = onStain;

    public void Stain()
    {
        if (IsDirty) return;
        IsDirty = true;
        OnStain?.Invoke(this);
    }

    public void Clean() => IsDirty = false;

    public static implicit operator Stainable<T>(T value) => new(value);
    public static implicit operator T(Stainable<T> s) => s.Value;
}