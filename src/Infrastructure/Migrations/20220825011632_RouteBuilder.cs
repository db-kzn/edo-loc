using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace EDO_FOMS.Infrastructure.Migrations
{
    public partial class RouteBuilder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Agreements_Organizations_OrgId",
                schema: "doc",
                table: "Agreements");

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

            migrationBuilder.AlterColumn<int>(
                name: "OrgId",
                schema: "doc",
                table: "Agreements",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<string>(
                name: "OrgInn",
                schema: "doc",
                table: "Agreements",
                type: "text",
                nullable: true);

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
                    ForUserRole = table.Column<int>(type: "integer", nullable: false),
                    EndAction = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    DateIsToday = table.Column<bool>(type: "boolean", nullable: false),
                    NameOfFile = table.Column<bool>(type: "boolean", nullable: false),
                    ParseFileName = table.Column<bool>(type: "boolean", nullable: false),
                    AllowRevocation = table.Column<bool>(type: "boolean", nullable: false),
                    ReadOnly = table.Column<bool>(type: "boolean", nullable: false),
                    ShowNotes = table.Column<bool>(type: "boolean", nullable: false),
                    UseVersioning = table.Column<bool>(type: "boolean", nullable: false),
                    IsPackage = table.Column<bool>(type: "boolean", nullable: false),
                    CalcHash = table.Column<bool>(type: "boolean", nullable: false),
                    AttachedSign = table.Column<bool>(type: "boolean", nullable: false),
                    DisplayedSign = table.Column<bool>(type: "boolean", nullable: false),
                    HasDetails = table.Column<bool>(type: "boolean", nullable: false),
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
                name: "RouteDocTypes",
                schema: "dir",
                columns: table => new
                {
                    RouteId = table.Column<int>(type: "integer", nullable: false),
                    DocumentTypeId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RouteDocTypes", x => new { x.RouteId, x.DocumentTypeId });
                    table.ForeignKey(
                        name: "FK_RouteDocTypes_DocumentTypes_DocumentTypeId",
                        column: x => x.DocumentTypeId,
                        principalSchema: "dir",
                        principalTable: "DocumentTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RouteDocTypes_Routes_RouteId",
                        column: x => x.RouteId,
                        principalSchema: "dir",
                        principalTable: "Routes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RouteFileParses",
                schema: "dir",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RouteId = table.Column<int>(type: "integer", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Pattern = table.Column<string>(type: "text", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RouteFileParses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RouteFileParses_Routes_RouteId",
                        column: x => x.RouteId,
                        principalSchema: "dir",
                        principalTable: "Routes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RouteOrgTypes",
                schema: "dir",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RouteId = table.Column<int>(type: "integer", nullable: true),
                    OrgType = table.Column<int>(type: "integer", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RouteOrgTypes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RouteOrgTypes_Routes_RouteId",
                        column: x => x.RouteId,
                        principalSchema: "dir",
                        principalTable: "Routes",
                        principalColumn: "Id");
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
                name: "RouteSteps",
                schema: "dir",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RouteId = table.Column<int>(type: "integer", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    StageNumber = table.Column<int>(type: "integer", nullable: false),
                    Number = table.Column<int>(type: "integer", nullable: false),
                    ActType = table.Column<int>(type: "integer", nullable: false),
                    AutoSearch = table.Column<int>(type: "integer", nullable: false),
                    OrgType = table.Column<int>(type: "integer", nullable: false),
                    OrgId = table.Column<int>(type: "integer", nullable: true),
                    Requred = table.Column<bool>(type: "boolean", nullable: false),
                    OnlyHead = table.Column<bool>(type: "boolean", nullable: false),
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
                    table.PrimaryKey("PK_RouteSteps", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RouteSteps_Routes_RouteId",
                        column: x => x.RouteId,
                        principalSchema: "dir",
                        principalTable: "Routes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RouteStepMembers",
                schema: "dir",
                columns: table => new
                {
                    RouteStepId = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    Act = table.Column<int>(type: "integer", nullable: false),
                    IsAdditional = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RouteStepMembers", x => new { x.RouteStepId, x.UserId, x.Act, x.IsAdditional });
                    table.ForeignKey(
                        name: "FK_RouteStepMembers_RouteSteps_RouteStepId",
                        column: x => x.RouteStepId,
                        principalSchema: "dir",
                        principalTable: "RouteSteps",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Documents_EmplOrgId",
                schema: "doc",
                table: "Documents",
                column: "EmplOrgId");

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
                name: "IX_RouteDocTypes_DocumentTypeId",
                schema: "dir",
                table: "RouteDocTypes",
                column: "DocumentTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_RouteFileParses_RouteId",
                schema: "dir",
                table: "RouteFileParses",
                column: "RouteId");

            migrationBuilder.CreateIndex(
                name: "IX_RouteOrgTypes_RouteId",
                schema: "dir",
                table: "RouteOrgTypes",
                column: "RouteId");

            migrationBuilder.CreateIndex(
                name: "IX_RouteStages_RouteId",
                schema: "dir",
                table: "RouteStages",
                column: "RouteId");

            migrationBuilder.CreateIndex(
                name: "IX_RouteSteps_RouteId",
                schema: "dir",
                table: "RouteSteps",
                column: "RouteId");

            migrationBuilder.AddForeignKey(
                name: "FK_Agreements_Organizations_OrgId",
                schema: "doc",
                table: "Agreements",
                column: "OrgId",
                principalSchema: "org",
                principalTable: "Organizations",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Documents_Organizations_EmplOrgId",
                schema: "doc",
                table: "Documents",
                column: "EmplOrgId",
                principalSchema: "org",
                principalTable: "Organizations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Agreements_Organizations_OrgId",
                schema: "doc",
                table: "Agreements");

            migrationBuilder.DropForeignKey(
                name: "FK_Documents_Organizations_EmplOrgId",
                schema: "doc",
                table: "Documents");

            migrationBuilder.DropTable(
                name: "Companies",
                schema: "dir");

            migrationBuilder.DropTable(
                name: "RouteDocTypes",
                schema: "dir");

            migrationBuilder.DropTable(
                name: "RouteFileParses",
                schema: "dir");

            migrationBuilder.DropTable(
                name: "RouteOrgTypes",
                schema: "dir");

            migrationBuilder.DropTable(
                name: "RouteStages",
                schema: "dir");

            migrationBuilder.DropTable(
                name: "RouteStepMembers",
                schema: "dir");

            migrationBuilder.DropTable(
                name: "RouteSteps",
                schema: "dir");

            migrationBuilder.DropTable(
                name: "Routes",
                schema: "dir");

            migrationBuilder.DropIndex(
                name: "IX_Documents_EmplOrgId",
                schema: "doc",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "OrgInn",
                schema: "doc",
                table: "Agreements");

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

            migrationBuilder.AlterColumn<int>(
                name: "OrgId",
                schema: "doc",
                table: "Agreements",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Documents_IssuerId",
                schema: "doc",
                table: "Documents",
                column: "IssuerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Agreements_Organizations_OrgId",
                schema: "doc",
                table: "Agreements",
                column: "OrgId",
                principalSchema: "org",
                principalTable: "Organizations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

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
