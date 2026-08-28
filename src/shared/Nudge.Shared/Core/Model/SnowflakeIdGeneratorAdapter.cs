using IdGen;

namespace Nudge.Shared.Core.Model;

public class SnowflakeIdGeneratorAdapter : Shared.Core.Model.IIdGenerator<long>
{
    private readonly IdGenerator _idGenerator;

    public SnowflakeIdGeneratorAdapter(IdGenerator idGenerator) => _idGenerator = idGenerator ?? throw new ArgumentNullException(nameof(idGenerator));

    public long CreateId() => _idGenerator.CreateId();
}

// todo: create options and appropriate extensions for its configuration
