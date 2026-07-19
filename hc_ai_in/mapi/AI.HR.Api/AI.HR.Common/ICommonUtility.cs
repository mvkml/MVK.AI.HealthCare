namespace AI.HR.Common;

/// <summary>
/// Shared, stateless helpers used across multiple projects (AI.HR.Api and fa_upload_doc).
/// Home for small cross-cutting utilities — numbering today, more (e.g. string/date
/// helpers) as they come up. Split into a focused utility class once a distinct group
/// of methods grows large enough to warrant its own home.
/// </summary>
public interface ICommonUtility
{
    /// <summary>Generates a unique Document Control Number: DCN-YY-MM-DD-HH-mm-ss</summary>
    string GenerateDcn();

    /// <summary>Generates a Group Number shared across a batch/zip upload: GRP-YY-MM-DD-HH-mm-ss</summary>
    string GenerateGrp();
}
