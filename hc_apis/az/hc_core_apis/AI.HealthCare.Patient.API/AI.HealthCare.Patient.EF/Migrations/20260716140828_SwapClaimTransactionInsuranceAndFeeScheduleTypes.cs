using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AI.HealthCare.Patient.EF.Migrations
{
    /// <inheritdoc />
    public partial class SwapClaimTransactionInsuranceAndFeeScheduleTypes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // int <-> uniqueidentifier have no implicit conversion, so ALTER COLUMN can't cross
            // between them. Table is empty at this point in the entity's history, so drop+recreate
            // is safe here.
            migrationBuilder.DropColumn(name: "PatientInsuranceId", table: "ClaimTransactions");
            migrationBuilder.DropColumn(name: "FeeScheduleId", table: "ClaimTransactions");

            migrationBuilder.AddColumn<Guid>(
                name: "PatientInsuranceId",
                table: "ClaimTransactions",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "FeeScheduleId",
                table: "ClaimTransactions",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "PatientInsuranceId", table: "ClaimTransactions");
            migrationBuilder.DropColumn(name: "FeeScheduleId", table: "ClaimTransactions");

            migrationBuilder.AddColumn<int>(
                name: "PatientInsuranceId",
                table: "ClaimTransactions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "FeeScheduleId",
                table: "ClaimTransactions",
                type: "uniqueidentifier",
                nullable: true);
        }
    }
}
