using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OfficeLeaveCore.Data.Migrations
{
    public partial class EmpLeaveMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LeaveApprovalId",
                table: "AspNetUsers",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "LeaveApprovals",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ApplicationId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    ApprovingFlag = table.Column<bool>(type: "bit", nullable: false),
                    ApprovingOfficerId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AprrovingOfficerId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeaveApprovals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LeaveApprovals_AspNetUsers_AprrovingOfficerId",
                        column: x => x.AprrovingOfficerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LeaveApplications",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    LeaveType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    LeaveStart = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LeaveEnd = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LeaveApprovalId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeaveApplications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LeaveApplications_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LeaveApplications_LeaveApprovals_LeaveApprovalId",
                        column: x => x.LeaveApprovalId,
                        principalTable: "LeaveApprovals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_LeaveApprovalId",
                table: "AspNetUsers",
                column: "LeaveApprovalId");

            migrationBuilder.CreateIndex(
                name: "IX_LeaveApplications_LeaveApprovalId",
                table: "LeaveApplications",
                column: "LeaveApprovalId");

            migrationBuilder.CreateIndex(
                name: "IX_LeaveApplications_UserId",
                table: "LeaveApplications",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_LeaveApprovals_ApplicationId",
                table: "LeaveApprovals",
                column: "ApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_LeaveApprovals_AprrovingOfficerId",
                table: "LeaveApprovals",
                column: "AprrovingOfficerId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_LeaveApprovals_LeaveApprovalId",
                table: "AspNetUsers",
                column: "LeaveApprovalId",
                principalTable: "LeaveApprovals",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LeaveApprovals_LeaveApplications_ApplicationId",
                table: "LeaveApprovals",
                column: "ApplicationId",
                principalTable: "LeaveApplications",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_LeaveApprovals_LeaveApprovalId",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_LeaveApplications_LeaveApprovals_LeaveApprovalId",
                table: "LeaveApplications");

            migrationBuilder.DropTable(
                name: "LeaveApprovals");

            migrationBuilder.DropTable(
                name: "LeaveApplications");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_LeaveApprovalId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "FullName",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "LeaveApprovalId",
                table: "AspNetUsers");
        }
    }
}
