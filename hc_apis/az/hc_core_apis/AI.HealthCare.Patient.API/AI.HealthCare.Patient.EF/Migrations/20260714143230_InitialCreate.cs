using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AI.HealthCare.Patient.EF.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Patients",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BirthDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeathDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Ssn = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Drivers = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Passport = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Prefix = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    First = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Middle = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Last = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Suffix = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Maiden = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Marital = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    Race = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    Ethnicity = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    Gender = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    Birthplace = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    Address = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    City = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    State = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    County = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    Fips = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    Zip = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    Lat = table.Column<decimal>(type: "decimal(10,7)", nullable: true),
                    Lon = table.Column<decimal>(type: "decimal(10,7)", nullable: true),
                    HealthcareExpenses = table.Column<decimal>(type: "decimal(12,2)", nullable: true),
                    HealthcareCoverage = table.Column<decimal>(type: "decimal(12,2)", nullable: true),
                    Income = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Patients", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Patients");
        }
    }
}
