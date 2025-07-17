using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskManagementApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateProjectTaskAssignedUserAndAuditFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AssignedToUserId",
                table: "ProjectTasks",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ProjectTasks_AssignedToUserId",
                table: "ProjectTasks",
                column: "AssignedToUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectTasks_Users_AssignedToUserId",
                table: "ProjectTasks",
                column: "AssignedToUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProjectTasks_Users_AssignedToUserId",
                table: "ProjectTasks");

            migrationBuilder.DropIndex(
                name: "IX_ProjectTasks_AssignedToUserId",
                table: "ProjectTasks");

            migrationBuilder.DropColumn(
                name: "AssignedToUserId",
                table: "ProjectTasks");
        }
    }
}
