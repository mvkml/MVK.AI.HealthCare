using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AI.HealthCare.Patient.EF.Migrations
{
    /// <inheritdoc />
    public partial class AddImagingStudies : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ImagingStudies",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PatientId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EncounterId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SeriesUid = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    BodysiteCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    BodysiteDescription = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    ModalityCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    ModalityDescription = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    InstanceUid = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    SopCode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    SopDescription = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    ProcedureCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImagingStudies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ImagingStudies_Encounters_EncounterId",
                        column: x => x.EncounterId,
                        principalTable: "Encounters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ImagingStudies_Patients_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Patients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ImagingStudies_EncounterId",
                table: "ImagingStudies",
                column: "EncounterId");

            migrationBuilder.CreateIndex(
                name: "IX_ImagingStudies_PatientId",
                table: "ImagingStudies",
                column: "PatientId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ImagingStudies");
        }
    }
}
