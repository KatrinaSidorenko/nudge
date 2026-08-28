namespace Nudge.Shared.Core.Model;

public interface IIdGenerator<T>
{
    T CreateId();
}
