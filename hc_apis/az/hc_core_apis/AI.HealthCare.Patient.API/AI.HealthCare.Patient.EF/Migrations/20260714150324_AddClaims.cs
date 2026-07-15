using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AI.HealthCare.Patient.EF.Migrations
{
    /// <inheritdoc />
    public partial class AddClaims : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Claims",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PatientId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProviderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PrimaryPatientInsuranceId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    SecondaryPatientInsuranceId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DepartmentId = table.Column<int>(type: "int", nullable: true),
                    PatientDepartmentId = table.Column<int>(type: "int", nullable: true),
                    Diagnosis1 = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Diagnosis2 = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Diagnosis3 = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Diagnosis4 = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Diagnosis5 = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Diagnosis6 = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Diagnosis7 = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Diagnosis8 = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    ReferringProviderId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    SupervisingProviderId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    AppointmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CurrentIllnessDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ServiceDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status1 = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Status2 = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    StatusP = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Outstanding1 = table.Column<decimal>(type: "decimal(12,2)", nullable: true),
                    Outstanding2 = table.Column<decimal>(type: "decimal(12,2)", nullable: true),
                    OutstandingP = table.Column<decimal>(type: "decimal(12,2)", nullable: true),
                    LastBilledDate1 = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastBilledDate2 = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastBilledDateP = table.Column<DateTime>(type: "datetime2", nullable: true),
                    HealthcareClaimTypeId1 = table.Column<int>(type: "int", nullable: true),
                    HealthcareClaimTypeId2 = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Claims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Claims_Encounters_AppointmentId",
                        column: x => x.AppointmentId,
                        principalTable: "Encounters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Claims_Patients_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Patients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Claims_Payers_PrimaryPatientInsuranceId",
                        column: x => x.PrimaryPatientInsuranceId,
                        principalTable: "Payers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Claims_Payers_SecondaryPatientInsuranceId",
                        column: x => x.SecondaryPatientInsuranceId,
                        principalTable: "Payers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Claims_Providers_ProviderId",
                        column: x => x.ProviderId,
                        principalTable: "Providers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Claims_Providers_ReferringProviderId",
                        column: x => x.ReferringProviderId,
                        principalTable: "Providers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Claims_Providers_SupervisingProviderId",
                        column: x => x.SupervisingProviderId,
                        principalTable: "Providers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Claims_AppointmentId",
                table: "Claims",
                column: "AppointmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Claims_PatientId",
                table: "Claims",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_Claims_PrimaryPatientInsuranceId",
                table: "Claims",
                column: "PrimaryPatientInsuranceId");

            migrationBuilder.CreateIndex(
                name: "IX_Claims_ProviderId",
                table: "Claims",
                column: "ProviderId");

            migrationBuilder.CreateIndex(
                name: "IX_Claims_ReferringProviderId",
                table: "Claims",
                column: "ReferringProviderId");

            migrationBuilder.CreateIndex(
                name: "IX_Claims_SecondaryPatientInsuranceId",
                table: "Claims",
                column: "SecondaryPatientInsuranceId");

            migrationBuilder.CreateIndex(
                name: "IX_Claims_SupervisingProviderId",
                table: "Claims",
                column: "SupervisingProviderId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Claims");
        }
    }
}
