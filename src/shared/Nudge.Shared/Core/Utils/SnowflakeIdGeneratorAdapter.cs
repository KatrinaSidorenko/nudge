using IdGen;

namespace Nudge.Shared.Core.Utils;

public class SnowflakeIdGeneratorAdapter : IIdGenerator<long>
{
    private readonly IdGenerator _idGenerator;

    public SnowflakeIdGeneratorAdapter(IdGenerator idGenerator) => _idGenerator = idGenerator ?? throw new ArgumentNullException(nameof(idGenerator));

    public long CreateId() => _idGenerator.CreateId();
}

// todo: create options and appropriate extensions for its configuration
