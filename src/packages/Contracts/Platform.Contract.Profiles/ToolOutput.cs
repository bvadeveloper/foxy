using MemoryPack;

namespace Platform.Contract.Profiles;

[MemoryPackable]
public partial record ToolOutput(string ToolName, string Output)
{
}