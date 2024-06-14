using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoadingAPI.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedSessionStructure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Scenario",
                table: "Scenario");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Scenario",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .Annotation("Sqlite:Autoincrement", true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Scenario",
                table: "Scenario",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Scenario_SessionId",
                table: "Scenario",
                column: "SessionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Scenario",
                table: "Scenario");

            migrationBuilder.DropIndex(
                name: "IX_Scenario_SessionId",
                table: "Scenario");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Scenario",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .OldAnnotation("Sqlite:Autoincrement", true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Scenario",
                table: "Scenario",
                columns: new[] { "SessionId", "Id" });
        }
    }
}
