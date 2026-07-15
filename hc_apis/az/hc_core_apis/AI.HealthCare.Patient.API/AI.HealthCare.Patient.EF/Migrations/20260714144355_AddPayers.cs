using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AI.HealthCare.Patient.EF.Migrations
{
    /// <inheritdoc />
    public partial class AddPayers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Payers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Ownership = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Address = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    City = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    StateHeadquartered = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    Zip = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    AmountCovered = table.Column<decimal>(type: "decimal(14,2)", nullable: true),
                    AmountUncovered = table.Column<decimal>(type: "decimal(14,2)", nullable: true),
                    Revenue = table.Column<decimal>(type: "decimal(14,2)", nullable: true),
                    CoveredEncounters = table.Column<int>(type: "int", nullable: true),
                    UncoveredEncounters = table.Column<int>(type: "int", nullable: true),
                    CoveredMedications = table.Column<int>(type: "int", nullable: true),
                    UncoveredMedications = table.Column<int>(type: "int", nullable: true),
                    CoveredProcedures = table.Column<int>(type: "int", nullable: true),
                    UncoveredProcedures = table.Column<int>(type: "int", nullable: true),
                    CoveredImmunizations = table.Column<int>(type: "int", nullable: true),
                    UncoveredImmunizations = table.Column<int>(type: "int", nullable: true),
                    UniqueCustomers = table.Column<int>(type: "int", nullable: true),
                    QolsAvg = table.Column<decimal>(type: "decimal(6,4)", nullable: true),
                    MemberMonths = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payers", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Payers");
        }
    }
}
