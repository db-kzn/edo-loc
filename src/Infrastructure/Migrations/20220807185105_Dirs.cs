using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace EDO_FOMS.Infrastructure.Migrations
{
    public partial class Dirs : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Documents_Organizations_IssuerId",
                schema: "doc",
                table: "Documents");

            migrationBuilder.DropIndex(
                name: "IX_Documents_IssuerId",
                schema: "doc",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "IssuerId",
                schema: "doc",
                table: "Documents");

            migrationBuilder.EnsureSchema(
                name: "sys");

            migrationBuilder.EnsureSchema(
                name: "dir");

            migrationBuilder.RenameTable(
                name: "Subscribes",
                newName: "Subscribes",
                newSchema: "sys");

            migrationBuilder.RenameTable(
                name: "DocumentTypes",
                schema: "doc",
                newName: "DocumentTypes",
                newSchema: "dir");

            migrationBuilder.RenameTable(
                name: "ChatHistory",
                newName: "ChatHistory",
                newSchema: "sys");

            migrationBuilder.RenameTable(
                name: "AuditTrails",
                newName: "AuditTrails",
                newSchema: "sys");

            migrationBuilder.AlterColumn<string>(
                name: "Inn",
                schema: "org",
                table: "Organizations",
                type: "character varying(12)",
                maxLength: 12,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(10)",
                oldMaxLength: 10);

            migrationBuilder.AddColumn<int>(
                name: "Color",
                schema: "dir",
                table: "DocumentTypes",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Icon",
                schema: "dir",
                table: "DocumentTypes",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Label",
                schema: "dir",
                table: "DocumentTypes",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RouteId",
                schema: "dir",
                table: "DocumentTypes",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Companies",
                schema: "dir",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    State = table.Column<int>(type: "integer", nullable: false),
                    TfOkato = table.Column<string>(type: "character varying(5)", maxLength: 5, nullable: true),
                    Code = table.Column<string>(type: "character varying(6)", maxLength: 6, nullable: true),
                    Inn = table.Column<string>(type: "character varying(12)", maxLength: 12, nullable: true),
                    Kpp = table.Column<string>(type: "character varying(9)", maxLength: 9, nullable: true),
                    Ogrn = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: true),
                    Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    ShortName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Address = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    AO = table.Column<Guid>(type: "uuid", nullable: true),
                    Phone = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Fax = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    HotLine = table.Column<string>(type: "character varying(13)", maxLength: 13, nullable: true),
                    Email = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: true),
                    SiteUrl = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    HeadName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    HeadLastName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    HeadMidName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Changed = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Companies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Routes",
                schema: "dir",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Number = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    ForOrgTypes = table.Column<int[]>(type: "integer[]", nullable: true),
                    ForUserRole = table.Column<int>(type: "integer", nullable: false),
                    EndAction = table.Column<int>(type: "integer", nullable: false),
                    IsPackage = table.Column<bool>(type: "boolean", nullable: false),
                    CalcHash = table.Column<bool>(type: "boolean", nullable: false),
                    AttachedSign = table.Column<bool>(type: "boolean", nullable: false),
                    DisplayedSign = table.Column<bool>(type: "boolean", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    UseVersioning = table.Column<bool>(type: "boolean", nullable: false),
                    AllowRevocation = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Routes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RouteStages",
                schema: "dir",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RouteId = table.Column<int>(type: "integer", nullable: false),
                    Number = table.Column<int>(type: "integer", nullable: false),
                    Color = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    ActType = table.Column<int>(type: "integer", nullable: false),
                    InSeries = table.Column<bool>(type: "boolean", nullable: false),
                    AllRequred = table.Column<bool>(type: "boolean", nullable: false),
                    DenyRevocation = table.Column<bool>(type: "boolean", nullable: false),
                    Validity = table.Column<TimeSpan>(type: "interval", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RouteStages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RouteStages_Routes_RouteId",
                        column: x => x.RouteId,
                        principalSchema: "dir",
                        principalTable: "Routes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RouteStageSteps",
                schema: "dir",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RouteId = table.Column<int>(type: "integer", nullable: false),
                    StageNumber = table.Column<int>(type: "integer", nullable: false),
                    Number = table.Column<int>(type: "integer", nullable: false),
                    ActType = table.Column<int>(type: "integer", nullable: false),
                    OrgType = table.Column<int>(type: "integer", nullable: false),
                    OnlyHead = table.Column<bool>(type: "boolean", nullable: false),
                    Requred = table.Column<bool>(type: "boolean", nullable: false),
                    SomeParticipants = table.Column<bool>(type: "boolean", nullable: false),
                    AllRequred = table.Column<bool>(type: "boolean", nullable: false),
                    HasAgreement = table.Column<bool>(type: "boolean", nullable: false),
                    HasReview = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RouteStageSteps", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RouteStageSteps_Routes_RouteId",
                        column: x => x.RouteId,
                        principalSchema: "dir",
                        principalTable: "Routes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Documents_EmplOrgId",
                schema: "doc",
                table: "Documents",
                column: "EmplOrgId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentTypes_RouteId",
                schema: "dir",
                table: "DocumentTypes",
                column: "RouteId");

            migrationBuilder.CreateIndex(
                name: "IX_Companies_Code",
                schema: "dir",
                table: "Companies",
                column: "Code");

            migrationBuilder.CreateIndex(
                name: "IX_Companies_Inn",
                schema: "dir",
                table: "Companies",
                column: "Inn");

            migrationBuilder.CreateIndex(
                name: "IX_Companies_TfOkato",
                schema: "dir",
                table: "Companies",
                column: "TfOkato");

            migrationBuilder.CreateIndex(
                name: "IX_RouteStages_RouteId",
                schema: "dir",
                table: "RouteStages",
                column: "RouteId");

            migrationBuilder.CreateIndex(
                name: "IX_RouteStageSteps_RouteId",
                schema: "dir",
                table: "RouteStageSteps",
                column: "RouteId");

            migrationBuilder.AddForeignKey(
                name: "FK_Documents_Organizations_EmplOrgId",
                schema: "doc",
                table: "Documents",
                column: "EmplOrgId",
                principalSchema: "org",
                principalTable: "Organizations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DocumentTypes_Routes_RouteId",
                schema: "dir",
                table: "DocumentTypes",
                column: "RouteId",
                principalSchema: "dir",
                principalTable: "Routes",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Documents_Organizations_EmplOrgId",
                schema: "doc",
                table: "Documents");

            migrationBuilder.DropForeignKey(
                name: "FK_DocumentTypes_Routes_RouteId",
                schema: "dir",
                table: "DocumentTypes");

            migrationBuilder.DropTable(
                name: "Companies",
                schema: "dir");

            migrationBuilder.DropTable(
                name: "RouteStages",
                schema: "dir");

            migrationBuilder.DropTable(
                name: "RouteStageSteps",
                schema: "dir");

            migrationBuilder.DropTable(
                name: "Routes",
                schema: "dir");

            migrationBuilder.DropIndex(
                name: "IX_Documents_EmplOrgId",
                schema: "doc",
                table: "Documents");

            migrationBuilder.DropIndex(
                name: "IX_DocumentTypes_RouteId",
                schema: "dir",
                table: "DocumentTypes");

            migrationBuilder.DropColumn(
                name: "Color",
                schema: "dir",
                table: "DocumentTypes");

            migrationBuilder.DropColumn(
                name: "Icon",
                schema: "dir",
                table: "DocumentTypes");

            migrationBuilder.DropColumn(
                name: "Label",
                schema: "dir",
                table: "DocumentTypes");

            migrationBuilder.DropColumn(
                name: "RouteId",
                schema: "dir",
                table: "DocumentTypes");

            migrationBuilder.RenameTable(
                name: "Subscribes",
                schema: "sys",
                newName: "Subscribes");

            migrationBuilder.RenameTable(
                name: "DocumentTypes",
                schema: "dir",
                newName: "DocumentTypes",
                newSchema: "doc");

            migrationBuilder.RenameTable(
                name: "ChatHistory",
                schema: "sys",
                newName: "ChatHistory");

            migrationBuilder.RenameTable(
                name: "AuditTrails",
                schema: "sys",
                newName: "AuditTrails");

            migrationBuilder.AlterColumn<string>(
                name: "Inn",
                schema: "org",
                table: "Organizations",
                type: "character varying(10)",
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(12)",
                oldMaxLength: 12);

            migrationBuilder.AddColumn<int>(
                name: "IssuerId",
                schema: "doc",
                table: "Documents",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Documents_IssuerId",
                schema: "doc",
                table: "Documents",
                column: "IssuerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Documents_Organizations_IssuerId",
                schema: "doc",
                table: "Documents",
                column: "IssuerId",
                principalSchema: "org",
                principalTable: "Organizations",
                principalColumn: "Id");
        }
    }
}
