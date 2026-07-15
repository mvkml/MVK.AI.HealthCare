using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AI.HealthCare.Patient.EF.Migrations
{
    /// <inheritdoc />
    public partial class AddClaimTransactions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ClaimTransactions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClaimId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ChargeId = table.Column<int>(type: "int", nullable: false),
                    PatientId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    Method = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    FromDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ToDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PlaceOfServiceId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ProcedureCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Modifier1 = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    Modifier2 = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    DiagnosisRef1 = table.Column<int>(type: "int", nullable: true),
                    DiagnosisRef2 = table.Column<int>(type: "int", nullable: true),
                    DiagnosisRef3 = table.Column<int>(type: "int", nullable: true),
                    DiagnosisRef4 = table.Column<int>(type: "int", nullable: true),
                    Units = table.Column<int>(type: "int", nullable: false),
                    DepartmentId = table.Column<int>(type: "int", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UnitAmount = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    TransferOutId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    TransferType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Payments = table.Column<decimal>(type: "decimal(12,2)", nullable: true),
                    Adjustments = table.Column<decimal>(type: "decimal(12,2)", nullable: true),
                    Transfers = table.Column<decimal>(type: "decimal(12,2)", nullable: true),
                    Outstanding = table.Column<decimal>(type: "decimal(12,2)", nullable: true),
                    AppointmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LineNote = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    PatientInsuranceId = table.Column<int>(type: "int", nullable: true),
                    FeeScheduleId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ProviderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SupervisingProviderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClaimTransactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClaimTransactions_Claims_ClaimId",
                        column: x => x.ClaimId,
                        principalTable: "Claims",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ClaimTransactions_Encounters_AppointmentId",
                        column: x => x.AppointmentId,
                        principalTable: "Encounters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ClaimTransactions_Organizations_PlaceOfServiceId",
                        column: x => x.PlaceOfServiceId,
                        principalTable: "Organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ClaimTransactions_Patients_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Patients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ClaimTransactions_Providers_ProviderId",
                        column: x => x.ProviderId,
                        principalTable: "Providers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ClaimTransactions_Providers_SupervisingProviderId",
                        column: x => x.SupervisingProviderId,
                        principalTable: "Providers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ClaimTransactions_AppointmentId",
                table: "ClaimTransactions",
                column: "AppointmentId");

            migrationBuilder.CreateIndex(
                name: "IX_ClaimTransactions_ClaimId",
                table: "ClaimTransactions",
                column: "ClaimId");

            migrationBuilder.CreateIndex(
                name: "IX_ClaimTransactions_PatientId",
                table: "ClaimTransactions",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_ClaimTransactions_PlaceOfServiceId",
                table: "ClaimTransactions",
                column: "PlaceOfServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_ClaimTransactions_ProviderId",
                table: "ClaimTransactions",
                column: "ProviderId");

            migrationBuilder.CreateIndex(
                name: "IX_ClaimTransactions_SupervisingProviderId",
                table: "ClaimTransactions",
                column: "SupervisingProviderId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClaimTransactions");
        }
    }
}
