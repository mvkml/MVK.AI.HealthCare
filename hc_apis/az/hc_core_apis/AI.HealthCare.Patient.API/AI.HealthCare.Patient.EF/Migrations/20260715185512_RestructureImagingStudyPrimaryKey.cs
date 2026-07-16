using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AI.HealthCare.Patient.EF.Migrations
{
    /// <inheritdoc />
    public partial class RestructureImagingStudyPrimaryKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // SQL Server cannot ALTER a column into an identity column in place; it must be
            // dropped and recreated. The table has no rows carried over from the old Guid Id
            // (every import attempt against it failed before this fix), so there is no data to preserve.
            migrationBuilder.DropPrimaryKey(
                name: "PK_ImagingStudies",
                table: "ImagingStudies");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "ImagingStudies");

            migrationBuilder.AddColumn<long>(
                name: "Id",
                table: "ImagingStudies",
                type: "bigint",
                nullable: false)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ImagingStudies",
                table: "ImagingStudies",
                column: "Id");

            migrationBuilder.AddColumn<Guid>(
                name: "StudyId",
                table: "ImagingStudies",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StudyId",
                table: "ImagingStudies");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ImagingStudies",
                table: "ImagingStudies");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "ImagingStudies");

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "ImagingStudies",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "PK_ImagingStudies",
                table: "ImagingStudies",
                column: "Id");
        }
    }
}
