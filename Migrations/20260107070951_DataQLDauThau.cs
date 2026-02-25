using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ApiWebsite.Migrations
{
    public partial class DataQLDauThau : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Account",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Username = table.Column<string>(type: "NVARCHAR(250)", nullable: false),
                    PasswordHash = table.Column<string>(type: "NVARCHAR(500)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "NVARCHAR(1000)", nullable: true),
                    Pseudonym = table.Column<string>(type: "NVARCHAR(1000)", nullable: true),
                    FullName = table.Column<string>(type: "NVARCHAR(250)", nullable: true),
                    DonViDeNghiId = table.Column<long>(type: "bigint", nullable: false),
                    Roles = table.Column<string>(type: "NVARCHAR(1000)", nullable: true),
                    Email = table.Column<string>(type: "NVARCHAR(100)", nullable: true),
                    Salt = table.Column<string>(type: "NVARCHAR(250)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    TimeLock = table.Column<string>(type: "NVARCHAR(100)", nullable: true),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false),
                    IsLock = table.Column<bool>(type: "bit", nullable: false),
                    VerifacationCode = table.Column<string>(type: "NVARCHAR(10)", nullable: true),
                    Timestamp = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "NVARCHAR(250)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "NVARCHAR(250)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Account", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EmailConfig",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Email = table.Column<string>(type: "NVARCHAR(250)", nullable: false),
                    Password = table.Column<string>(type: "NVARCHAR(250)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    MailServer = table.Column<string>(type: "NVARCHAR(100)", nullable: true),
                    Port = table.Column<int>(type: "int", nullable: false),
                    EnableSSl = table.Column<bool>(type: "bit", nullable: false),
                    EmailTitle = table.Column<string>(type: "NVARCHAR(250)", nullable: true),
                    Timestamp = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "NVARCHAR(250)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "NVARCHAR(250)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailConfig", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FileManager",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "NVARCHAR(250)", nullable: false),
                    PhysicalPath = table.Column<string>(type: "NVARCHAR(250)", nullable: true),
                    PhysicalThumbPath = table.Column<string>(type: "NVARCHAR(250)", nullable: true),
                    FilePath = table.Column<string>(type: "NVARCHAR(250)", nullable: true),
                    ThumbPath = table.Column<string>(type: "NVARCHAR(250)", nullable: true),
                    FileType = table.Column<string>(type: "NVARCHAR(10)", nullable: true),
                    FileSizeInKB = table.Column<int>(type: "int", nullable: false),
                    Timestamp = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "NVARCHAR(250)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "NVARCHAR(250)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FileManager", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Log",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Parameter = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LogLevel = table.Column<string>(type: "NVARCHAR(50)", nullable: false),
                    CreatedDate = table.Column<string>(type: "NVARCHAR(100)", nullable: true),
                    Timestamp = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Log", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LoginHistory",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserName = table.Column<string>(type: "NVARCHAR(250)", nullable: true),
                    LoginDate = table.Column<string>(type: "NVARCHAR(100)", nullable: false),
                    AccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Timestamp = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoginHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LoginHistory_Account_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Account",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Account_Username",
                table: "Account",
                column: "Username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LoginHistory_AccountId",
                table: "LoginHistory",
                column: "AccountId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmailConfig");

            migrationBuilder.DropTable(
                name: "FileManager");

            migrationBuilder.DropTable(
                name: "Log");

            migrationBuilder.DropTable(
                name: "LoginHistory");

            migrationBuilder.DropTable(
                name: "Account");
        }
    }
}
