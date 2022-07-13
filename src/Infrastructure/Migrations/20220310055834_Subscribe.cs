using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace EDO_FOMS.Infrastructure.Migrations
{
    public partial class Subscribe : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "HasChanges",
                schema: "doc",
                table: "Documents",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "CertId",
                schema: "doc",
                table: "Agreements",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Subscribes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<string>(type: "text", nullable: true),
                    Email_AgreementIncoming = table.Column<bool>(type: "boolean", nullable: true),
                    Email_AgreementRejected = table.Column<bool>(type: "boolean", nullable: true),
                    Email_AgreementApproved = table.Column<bool>(type: "boolean", nullable: true),
                    Email_AgreementSigned = table.Column<bool>(type: "boolean", nullable: true),
                    Email_AgreementAgreed = table.Column<bool>(type: "boolean", nullable: true),
                    Email_DocumentRejected = table.Column<bool>(type: "boolean", nullable: true),
                    Email_DocumentApproved = table.Column<bool>(type: "boolean", nullable: true),
                    Email_DocumentSigned = table.Column<bool>(type: "boolean", nullable: true),
                    Email_DocumentAgreed = table.Column<bool>(type: "boolean", nullable: true),
                    Telegram_AgreementIncoming = table.Column<bool>(type: "boolean", nullable: true),
                    Telegram_AgreementRejected = table.Column<bool>(type: "boolean", nullable: true),
                    Telegram_AgreementApproved = table.Column<bool>(type: "boolean", nullable: true),
                    Telegram_AgreementSigned = table.Column<bool>(type: "boolean", nullable: true),
                    Telegram_AgreementAgreed = table.Column<bool>(type: "boolean", nullable: true),
                    Telegram_DocumentRejected = table.Column<bool>(type: "boolean", nullable: true),
                    Telegram_DocumentApproved = table.Column<bool>(type: "boolean", nullable: true),
                    Telegram_DocumentSigned = table.Column<bool>(type: "boolean", nullable: true),
                    Telegram_DocumentAgreed = table.Column<bool>(type: "boolean", nullable: true),
                    Chat_AgreementIncoming = table.Column<bool>(type: "boolean", nullable: true),
                    Chat_AgreementRejected = table.Column<bool>(type: "boolean", nullable: true),
                    Chat_AgreementApproved = table.Column<bool>(type: "boolean", nullable: true),
                    Chat_AgreementSigned = table.Column<bool>(type: "boolean", nullable: true),
                    Chat_AgreementAgreed = table.Column<bool>(type: "boolean", nullable: true),
                    Chat_DocumentRejected = table.Column<bool>(type: "boolean", nullable: true),
                    Chat_DocumentApproved = table.Column<bool>(type: "boolean", nullable: true),
                    Chat_DocumentSigned = table.Column<bool>(type: "boolean", nullable: true),
                    Chat_DocumentAgreed = table.Column<bool>(type: "boolean", nullable: true),
                    Sms_AgreementIncoming = table.Column<bool>(type: "boolean", nullable: true),
                    Sms_AgreementRejected = table.Column<bool>(type: "boolean", nullable: true),
                    Sms_AgreementApproved = table.Column<bool>(type: "boolean", nullable: true),
                    Sms_AgreementSigned = table.Column<bool>(type: "boolean", nullable: true),
                    Sms_AgreementAgreed = table.Column<bool>(type: "boolean", nullable: true),
                    Sms_DocumentRejected = table.Column<bool>(type: "boolean", nullable: true),
                    Sms_DocumentApproved = table.Column<bool>(type: "boolean", nullable: true),
                    Sms_DocumentSigned = table.Column<bool>(type: "boolean", nullable: true),
                    Sms_DocumentAgreed = table.Column<bool>(type: "boolean", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subscribes", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Subscribes_UserId",
                table: "Subscribes",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Subscribes");

            migrationBuilder.DropColumn(
                name: "HasChanges",
                schema: "doc",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "CertId",
                schema: "doc",
                table: "Agreements");
        }
    }
}
