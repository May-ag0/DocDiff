using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DocDiff.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Comparisons",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    OriginalFileName = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    UpdatedFileName = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    ComparedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    AddedCount = table.Column<int>(type: "INTEGER", nullable: false),
                    RemovedCount = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comparisons", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Comparisons");
        }
    }
}
