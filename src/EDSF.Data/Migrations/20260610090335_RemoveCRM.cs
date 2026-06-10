using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EDSF.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemoveCRM : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Contacts");

            migrationBuilder.DropTable(
                name: "TaskActivities");

            migrationBuilder.DropTable(
                name: "Opportunities");

            migrationBuilder.DropTable(
                name: "Leads");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Contacts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CustomerId = table.Column<int>(type: "INTEGER", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Email = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    Phone = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true),
                    Position = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Version = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contacts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Contacts_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Leads",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Company = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Email = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    Phone = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true),
                    Source = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Status = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    Version = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Leads", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Opportunities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CustomerId = table.Column<int>(type: "INTEGER", nullable: true),
                    LeadId = table.Column<int>(type: "INTEGER", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ExpectedCloseDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    Probability = table.Column<int>(type: "INTEGER", nullable: false),
                    Stage = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    Title = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    Value = table.Column<decimal>(type: "TEXT", nullable: false),
                    Version = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Opportunities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Opportunities_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Opportunities_Leads_LeadId",
                        column: x => x.LeadId,
                        principalTable: "Leads",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "TaskActivities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    LeadId = table.Column<int>(type: "INTEGER", nullable: true),
                    OpportunityId = table.Column<int>(type: "INTEGER", nullable: true),
                    AssignedTo = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Description = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    DueDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    Status = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    Title = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Type = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    Version = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskActivities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TaskActivities_Leads_LeadId",
                        column: x => x.LeadId,
                        principalTable: "Leads",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_TaskActivities_Opportunities_OpportunityId",
                        column: x => x.OpportunityId,
                        principalTable: "Opportunities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Contacts_CustomerId",
                table: "Contacts",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Opportunities_CustomerId",
                table: "Opportunities",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Opportunities_LeadId",
                table: "Opportunities",
                column: "LeadId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskActivities_LeadId",
                table: "TaskActivities",
                column: "LeadId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskActivities_OpportunityId",
                table: "TaskActivities",
                column: "OpportunityId");
        }
    }
}
