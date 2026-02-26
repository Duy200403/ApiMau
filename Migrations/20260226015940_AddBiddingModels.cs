using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ApiWebsite.Migrations
{
    public partial class AddBiddingModels : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AuditLog",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<string>(type: "NVARCHAR(250)", nullable: true),
                    TableName = table.Column<string>(type: "NVARCHAR(100)", nullable: true),
                    Action = table.Column<string>(type: "NVARCHAR(50)", nullable: true),
                    PrimaryKey = table.Column<string>(type: "NVARCHAR(100)", nullable: true),
                    OldValues = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NewValues = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AffectedColumns = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLog", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DonViDeNghi",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "NVARCHAR(100)", nullable: true),
                    Name = table.Column<string>(type: "NVARCHAR(250)", nullable: true),
                    ParentId = table.Column<long>(type: "bigint", nullable: true),
                    Timestamp = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "NVARCHAR(250)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "NVARCHAR(250)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DonViDeNghi", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DonViDeNghi_DonViDeNghi_ParentId",
                        column: x => x.ParentId,
                        principalTable: "DonViDeNghi",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Permission",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoleName = table.Column<string>(type: "NVARCHAR(100)", nullable: true),
                    Resource = table.Column<string>(type: "NVARCHAR(100)", nullable: true),
                    Action = table.Column<string>(type: "NVARCHAR(50)", nullable: true),
                    Timestamp = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "NVARCHAR(250)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "NVARCHAR(250)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Permission", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProcessBranch",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BranchCode = table.Column<string>(type: "NVARCHAR(100)", nullable: true),
                    BranchName = table.Column<string>(type: "NVARCHAR(250)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Timestamp = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "NVARCHAR(250)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "NVARCHAR(250)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProcessBranch", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BiddingProject",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProjectCode = table.Column<string>(type: "NVARCHAR(100)", nullable: true),
                    ProjectName = table.Column<string>(type: "NVARCHAR(500)", nullable: true),
                    ProcessBranchId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CurrentStepOrder = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "NVARCHAR(50)", nullable: true),
                    Timestamp = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "NVARCHAR(250)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "NVARCHAR(250)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BiddingProject", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BiddingProject_ProcessBranch_ProcessBranchId",
                        column: x => x.ProcessBranchId,
                        principalTable: "ProcessBranch",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProcessStep",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProcessBranchId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StepCode = table.Column<string>(type: "NVARCHAR(50)", nullable: true),
                    StepName = table.Column<string>(type: "NVARCHAR(250)", nullable: true),
                    Order = table.Column<int>(type: "int", nullable: false),
                    TargetDonViId = table.Column<long>(type: "bigint", nullable: false),
                    SlaDays = table.Column<int>(type: "int", nullable: false),
                    RequiresAttachment = table.Column<bool>(type: "bit", nullable: false),
                    Timestamp = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "NVARCHAR(250)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "NVARCHAR(250)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProcessStep", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProcessStep_DonViDeNghi_TargetDonViId",
                        column: x => x.TargetDonViId,
                        principalTable: "DonViDeNghi",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProcessStep_ProcessBranch_ProcessBranchId",
                        column: x => x.ProcessBranchId,
                        principalTable: "ProcessBranch",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProjectStep",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BiddingProjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProcessStepId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<string>(type: "NVARCHAR(50)", nullable: true),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompletedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompletedByUserId = table.Column<string>(type: "NVARCHAR(250)", nullable: true),
                    FormDataJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Comments = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Timestamp = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "NVARCHAR(250)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "NVARCHAR(250)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectStep", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectStep_BiddingProject_BiddingProjectId",
                        column: x => x.BiddingProjectId,
                        principalTable: "BiddingProject",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProjectStep_ProcessStep_ProcessStepId",
                        column: x => x.ProcessStepId,
                        principalTable: "ProcessStep",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "StepAttribute",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProcessStepId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AttributeCode = table.Column<string>(type: "NVARCHAR(100)", nullable: true),
                    AttributeName = table.Column<string>(type: "NVARCHAR(250)", nullable: true),
                    InputType = table.Column<int>(type: "int", nullable: false),
                    SelectOptionsJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsRequired = table.Column<bool>(type: "bit", nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    Timestamp = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "NVARCHAR(250)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "NVARCHAR(250)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StepAttribute", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StepAttribute_ProcessStep_ProcessStepId",
                        column: x => x.ProcessStepId,
                        principalTable: "ProcessStep",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProjectStepAttachment",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProjectStepId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FileName = table.Column<string>(type: "NVARCHAR(500)", nullable: true),
                    FilePath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FileExtension = table.Column<string>(type: "NVARCHAR(50)", nullable: true),
                    FileSizeInBytes = table.Column<long>(type: "bigint", nullable: false),
                    Timestamp = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "NVARCHAR(250)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "NVARCHAR(250)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectStepAttachment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectStepAttachment_ProjectStep_ProjectStepId",
                        column: x => x.ProjectStepId,
                        principalTable: "ProjectStep",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BiddingProject_ProcessBranchId",
                table: "BiddingProject",
                column: "ProcessBranchId");

            migrationBuilder.CreateIndex(
                name: "IX_DonViDeNghi_ParentId",
                table: "DonViDeNghi",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_ProcessStep_ProcessBranchId",
                table: "ProcessStep",
                column: "ProcessBranchId");

            migrationBuilder.CreateIndex(
                name: "IX_ProcessStep_TargetDonViId",
                table: "ProcessStep",
                column: "TargetDonViId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectStep_BiddingProjectId",
                table: "ProjectStep",
                column: "BiddingProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectStep_ProcessStepId",
                table: "ProjectStep",
                column: "ProcessStepId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectStepAttachment_ProjectStepId",
                table: "ProjectStepAttachment",
                column: "ProjectStepId");

            migrationBuilder.CreateIndex(
                name: "IX_StepAttribute_ProcessStepId",
                table: "StepAttribute",
                column: "ProcessStepId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuditLog");

            migrationBuilder.DropTable(
                name: "Permission");

            migrationBuilder.DropTable(
                name: "ProjectStepAttachment");

            migrationBuilder.DropTable(
                name: "StepAttribute");

            migrationBuilder.DropTable(
                name: "ProjectStep");

            migrationBuilder.DropTable(
                name: "BiddingProject");

            migrationBuilder.DropTable(
                name: "ProcessStep");

            migrationBuilder.DropTable(
                name: "DonViDeNghi");

            migrationBuilder.DropTable(
                name: "ProcessBranch");
        }
    }
}
