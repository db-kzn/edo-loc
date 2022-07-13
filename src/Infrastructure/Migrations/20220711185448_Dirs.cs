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
                name: "dic");

            migrationBuilder.RenameTable(
                name: "Subscribes",
                newName: "Subscribes",
                newSchema: "sys");

            migrationBuilder.RenameTable(
                name: "DocumentTypes",
                schema: "doc",
                newName: "DocumentTypes",
                newSchema: "dic");

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

            migrationBuilder.CreateTable(
                name: "Companies",
                schema: "dic",
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
                schema: "dic",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Routes", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Documents_EmplOrgId",
                schema: "doc",
                table: "Documents",
                column: "EmplOrgId");

            migrationBuilder.CreateIndex(
                name: "IX_Companies_Code_Inn",
                schema: "dic",
                table: "Companies",
                columns: new[] { "Code", "Inn" });

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
                name: "FK_Documents_Organizations_EmplOrgId",
                schema: "doc",
                table: "Documents");

            migrationBuilder.DropTable(
                name: "Companies",
                schema: "dic");

            migrationBuilder.DropTable(
                name: "Routes",
                schema: "dic");

            migrationBuilder.DropIndex(
                name: "IX_Documents_EmplOrgId",
                schema: "doc",
                table: "Documents");

            migrationBuilder.RenameTable(
                name: "Subscribes",
                schema: "sys",
                newName: "Subscribes");

            migrationBuilder.RenameTable(
                name: "DocumentTypes",
                schema: "dic",
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
