using System.Globalization;
using AI.HealthCare.Patient.Models.ImagingStudy;

namespace AI.HealthCare.Patient.BL;

// Maps a row from Synthea's imaging_studies.csv:
// Id,DATE,PATIENT,ENCOUNTER,SERIES_UID,BODYSITE_CODE,BODYSITE_DESCRIPTION,MODALITY_CODE,MODALITY_DESCRIPTION,INSTANCE_UID,SOP_CODE,SOP_DESCRIPTION,PROCEDURE_CODE
// The Id column is Synthea's study Id, not a unique row Id -- one study spans many series/instance rows,
// so it maps to StudyId. The row's own primary key (Id, long) is server-generated; rows are matched for
// upsert purposes by InstanceUid, which is unique per row.
public class ImagingStudyBLMapper : IImagingStudyBLMapper
{
    private const int ExpectedColumnCount = 13;

    public ImagingStudyItem ToModel(string[] row)
    {
        if (row.Length < ExpectedColumnCount)
            throw new FormatException($"Expected {ExpectedColumnCount} columns, got {row.Length}.");

        return new ImagingStudyItem
        {
            StudyId = Guid.Parse(row[0]),
            Date = DateTime.Parse(row[1], CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal),
            PatientId = Guid.Parse(row[2]),
            EncounterId = Guid.Parse(row[3]),
            SeriesUid = NullIfEmpty(row[4]),
            BodysiteCode = NullIfEmpty(row[5]),
            BodysiteDescription = NullIfEmpty(row[6]),
            ModalityCode = NullIfEmpty(row[7]),
            ModalityDescription = NullIfEmpty(row[8]),
            InstanceUid = NullIfEmpty(row[9]),
            SopCode = NullIfEmpty(row[10]),
            SopDescription = NullIfEmpty(row[11]),
            ProcedureCode = NullIfEmpty(row[12]),
        };
    }

    public ImagingStudyItem ToItem(ImagingStudyRequest request) => new()
    {
        StudyId = request.StudyId ?? Guid.Empty,
        Date = request.Date,
        PatientId = request.PatientId,
        EncounterId = request.EncounterId,
        SeriesUid = request.SeriesUid,
        BodysiteCode = request.BodysiteCode,
        BodysiteDescription = request.BodysiteDescription,
        ModalityCode = request.ModalityCode,
        ModalityDescription = request.ModalityDescription,
        InstanceUid = request.InstanceUid,
        SopCode = request.SopCode,
        SopDescription = request.SopDescription,
        ProcedureCode = request.ProcedureCode
    };

    public ImagingStudyResponse ToResponse(ImagingStudyItem item) => new()
    {
        Id = item.Id,
        StudyId = item.StudyId,
        Date = item.Date,
        PatientId = item.PatientId,
        EncounterId = item.EncounterId,
        SeriesUid = item.SeriesUid,
        BodysiteCode = item.BodysiteCode,
        BodysiteDescription = item.BodysiteDescription,
        ModalityCode = item.ModalityCode,
        ModalityDescription = item.ModalityDescription,
        InstanceUid = item.InstanceUid,
        SopCode = item.SopCode,
        SopDescription = item.SopDescription,
        ProcedureCode = item.ProcedureCode
    };

    private static string? NullIfEmpty(string value) =>
        string.IsNullOrWhiteSpace(value) ? null : value;
}
