﻿using System;
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

            migrationBuilder.RenameColumn(
                name: "IssuerId",
                schema: "doc",
                table: "Documents",
                newName: "RecipientId");

            migrationBuilder.RenameIndex(
                name: "IX_Documents_IssuerId",
                schema: "doc",
                table: "Documents",
                newName: "IX_Documents_RecipientId");

            migrationBuilder.RenameColumn(
                name: "Step",
                schema: "doc",
                table: "Agreements",
                newName: "StageNumber");

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

            migrationBuilder.AddColumn<string>(
                name: "BuhgId",
                schema: "org",
                table: "Organizations",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HeadId",
                schema: "org",
                table: "Organizations",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OmsCode",
                schema: "org",
                table: "Organizations",
                type: "character varying(6)",
                maxLength: 6,
                nullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "Date",
                schema: "doc",
                table: "Documents",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AddColumn<int>(
                name: "DepartmentId",
                schema: "doc",
                table: "Documents",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ExecutorId",
                schema: "doc",
                table: "Documents",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "KeyOrgId",
                schema: "doc",
                table: "Documents",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "SignStartAt",
                schema: "doc",
                table: "Documents",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AlterColumn<int>(
                name: "OrgId",
                schema: "doc",
                table: "Agreements",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<bool>(
                name: "InQueue",
                schema: "doc",
                table: "Agreements",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsAdditional",
                schema: "doc",
                table: "Agreements",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsRequired",
                schema: "doc",
                table: "Agreements",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "OmsCode",
                schema: "doc",
                table: "Agreements",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OrgInn",
                schema: "doc",
                table: "Agreements",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RouteStepId",
                schema: "doc",
                table: "Agreements",
                type: "integer",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Short",
                schema: "dir",
                table: "DocumentTypes",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "NameEn",
                schema: "dir",
                table: "DocumentTypes",
                type: "character varying(300)",
                maxLength: 300,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                schema: "dir",
                table: "DocumentTypes",
                type: "character varying(300)",
                maxLength: 300,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                schema: "dir",
                table: "DocumentTypes",
                type: "character varying(4000)",
                maxLength: 4000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Code",
                schema: "dir",
                table: "DocumentTypes",
                type: "character varying(10)",
                maxLength: 10,
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
                type: "character varying(100)",
                maxLength: 100,
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
                    Region = table.Column<int>(type: "integer", nullable: true),
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
                name: "Departments",
                schema: "sys",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    OrgId = table.Column<int>(type: "integer", nullable: true),
                    Code = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    Label = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Short = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Name = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Departments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "JobTitles",
                schema: "sys",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    OrgId = table.Column<int>(type: "integer", nullable: true),
                    Code = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    Label = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Short = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Name = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobTitles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ParamGroups",
                schema: "sys",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Version = table.Column<int>(type: "integer", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ParamGroups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Employees",
                schema: "org",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    OrgId = table.Column<int>(type: "integer", nullable: false),
                    OrganizationId = table.Column<int>(type: "integer", nullable: true),
                    DepartmentId = table.Column<int>(type: "integer", nullable: true),
                    JobTitleId = table.Column<int>(type: "integer", nullable: true),
                    UserId = table.Column<string>(type: "text", nullable: true),
                    ChangerId = table.Column<string>(type: "text", nullable: true),
                    InnLe = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    Snils = table.Column<string>(type: "character varying(11)", maxLength: 11, nullable: true),
                    Inn = table.Column<string>(type: "character varying(12)", maxLength: 12, nullable: true),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Surname = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    GivenName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employees", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Employees_Departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalSchema: "sys",
                        principalTable: "Departments",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Employees_JobTitles_JobTitleId",
                        column: x => x.JobTitleId,
                        principalSchema: "sys",
                        principalTable: "JobTitles",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Employees_Organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalSchema: "org",
                        principalTable: "Organizations",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Routes",
                schema: "dir",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Number = table.Column<int>(type: "integer", nullable: false),
                    Code = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    Short = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Name = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    Description = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    ForUserRole = table.Column<int>(type: "integer", nullable: false),
                    EndAction = table.Column<int>(type: "integer", nullable: false),
                    ExecDepartId = table.Column<int>(type: "integer", nullable: true),
                    DepartmentId = table.Column<int>(type: "integer", nullable: true),
                    ExecJobTitleId = table.Column<int>(type: "integer", nullable: true),
                    JobTitleId = table.Column<int>(type: "integer", nullable: true),
                    ExecutorId = table.Column<string>(type: "text", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    DateIsToday = table.Column<bool>(type: "boolean", nullable: false),
                    NameOfFile = table.Column<bool>(type: "boolean", nullable: false),
                    ParseFileName = table.Column<bool>(type: "boolean", nullable: false),
                    AllowRevocation = table.Column<bool>(type: "boolean", nullable: false),
                    ProtectedMode = table.Column<bool>(type: "boolean", nullable: false),
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
                    table.ForeignKey(
                        name: "FK_Routes_Departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalSchema: "sys",
                        principalTable: "Departments",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Routes_JobTitles_JobTitleId",
                        column: x => x.JobTitleId,
                        principalSchema: "sys",
                        principalTable: "JobTitles",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Params",
                schema: "sys",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ParamGroupId = table.Column<int>(type: "integer", nullable: false),
                    Property = table.Column<string>(type: "text", nullable: true),
                    Value = table.Column<string>(type: "text", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Params", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Params_ParamGroups_ParamGroupId",
                        column: x => x.ParamGroupId,
                        principalSchema: "sys",
                        principalTable: "ParamGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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
                    RouteId = table.Column<int>(type: "integer", nullable: false),
                    PatternType = table.Column<int>(type: "integer", nullable: false),
                    Pattern = table.Column<string>(type: "text", nullable: true),
                    ValueType = table.Column<int>(type: "integer", nullable: false),
                    ValueFlag = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RouteFileParses", x => new { x.RouteId, x.PatternType });
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
                name: "RoutePacketFiles",
                schema: "dir",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RouteId = table.Column<int>(type: "integer", nullable: false),
                    FileType = table.Column<string>(type: "text", nullable: true),
                    FileMask = table.Column<string>(type: "text", nullable: true),
                    FileAccept = table.Column<string>(type: "text", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoutePacketFiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoutePacketFiles_Routes_RouteId",
                        column: x => x.RouteId,
                        principalSchema: "dir",
                        principalTable: "Routes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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
                    IsExpanded = table.Column<bool>(type: "boolean", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    ActType = table.Column<int>(type: "integer", nullable: false),
                    InSeries = table.Column<bool>(type: "boolean", nullable: false),
                    AllRequred = table.Column<bool>(type: "boolean", nullable: false),
                    IgnoreProtected = table.Column<bool>(type: "boolean", nullable: false),
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
                    MemberGroup = table.Column<int>(type: "integer", nullable: false),
                    OrgType = table.Column<int>(type: "integer", nullable: false),
                    OrgId = table.Column<int>(type: "integer", nullable: true),
                    IsKeyMember = table.Column<bool>(type: "boolean", nullable: false),
                    Requred = table.Column<bool>(type: "boolean", nullable: false),
                    SomeParticipants = table.Column<bool>(type: "boolean", nullable: false),
                    AllRequred = table.Column<bool>(type: "boolean", nullable: false),
                    AutoSearch = table.Column<int>(type: "integer", nullable: false),
                    HasAgreement = table.Column<bool>(type: "boolean", nullable: false),
                    HasReview = table.Column<bool>(type: "boolean", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
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
                name: "DocPacketFiles",
                schema: "doc",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DocumentId = table.Column<int>(type: "integer", nullable: false),
                    RoutePacketFileId = table.Column<int>(type: "integer", nullable: true),
                    URL = table.Column<string>(type: "text", nullable: true),
                    StoragePath = table.Column<string>(type: "text", nullable: true),
                    Name = table.Column<string>(type: "text", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocPacketFiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DocPacketFiles_Documents_DocumentId",
                        column: x => x.DocumentId,
                        principalSchema: "doc",
                        principalTable: "Documents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DocPacketFiles_RoutePacketFiles_RoutePacketFileId",
                        column: x => x.RoutePacketFileId,
                        principalSchema: "dir",
                        principalTable: "RoutePacketFiles",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "RouteStepMembers",
                schema: "dir",
                columns: table => new
                {
                    RouteStepId = table.Column<int>(type: "integer", nullable: false),
                    Act = table.Column<int>(type: "integer", nullable: false),
                    IsAdditional = table.Column<bool>(type: "boolean", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: false)
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
                name: "IX_Documents_DepartmentId",
                schema: "doc",
                table: "Documents",
                column: "DepartmentId");

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
                name: "IX_DocPacketFiles_DocumentId",
                schema: "doc",
                table: "DocPacketFiles",
                column: "DocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_DocPacketFiles_RoutePacketFileId",
                schema: "doc",
                table: "DocPacketFiles",
                column: "RoutePacketFileId");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_DepartmentId",
                schema: "org",
                table: "Employees",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_JobTitleId",
                schema: "org",
                table: "Employees",
                column: "JobTitleId");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_OrganizationId",
                schema: "org",
                table: "Employees",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_Params_ParamGroupId",
                schema: "sys",
                table: "Params",
                column: "ParamGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_RouteDocTypes_DocumentTypeId",
                schema: "dir",
                table: "RouteDocTypes",
                column: "DocumentTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_RouteOrgTypes_RouteId",
                schema: "dir",
                table: "RouteOrgTypes",
                column: "RouteId");

            migrationBuilder.CreateIndex(
                name: "IX_RoutePacketFiles_RouteId",
                schema: "dir",
                table: "RoutePacketFiles",
                column: "RouteId");

            migrationBuilder.CreateIndex(
                name: "IX_Routes_DepartmentId",
                schema: "dir",
                table: "Routes",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Routes_JobTitleId",
                schema: "dir",
                table: "Routes",
                column: "JobTitleId");

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
                name: "FK_Documents_Departments_DepartmentId",
                schema: "doc",
                table: "Documents",
                column: "DepartmentId",
                principalSchema: "sys",
                principalTable: "Departments",
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

            migrationBuilder.AddForeignKey(
                name: "FK_Documents_Organizations_RecipientId",
                schema: "doc",
                table: "Documents",
                column: "RecipientId",
                principalSchema: "org",
                principalTable: "Organizations",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Agreements_Organizations_OrgId",
                schema: "doc",
                table: "Agreements");

            migrationBuilder.DropForeignKey(
                name: "FK_Documents_Departments_DepartmentId",
                schema: "doc",
                table: "Documents");

            migrationBuilder.DropForeignKey(
                name: "FK_Documents_Organizations_EmplOrgId",
                schema: "doc",
                table: "Documents");

            migrationBuilder.DropForeignKey(
                name: "FK_Documents_Organizations_RecipientId",
                schema: "doc",
                table: "Documents");

            migrationBuilder.DropTable(
                name: "Companies",
                schema: "dir");

            migrationBuilder.DropTable(
                name: "DocPacketFiles",
                schema: "doc");

            migrationBuilder.DropTable(
                name: "Employees",
                schema: "org");

            migrationBuilder.DropTable(
                name: "Params",
                schema: "sys");

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
                name: "RoutePacketFiles",
                schema: "dir");

            migrationBuilder.DropTable(
                name: "ParamGroups",
                schema: "sys");

            migrationBuilder.DropTable(
                name: "RouteSteps",
                schema: "dir");

            migrationBuilder.DropTable(
                name: "Routes",
                schema: "dir");

            migrationBuilder.DropTable(
                name: "Departments",
                schema: "sys");

            migrationBuilder.DropTable(
                name: "JobTitles",
                schema: "sys");

            migrationBuilder.DropIndex(
                name: "IX_Documents_DepartmentId",
                schema: "doc",
                table: "Documents");

            migrationBuilder.DropIndex(
                name: "IX_Documents_EmplOrgId",
                schema: "doc",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "BuhgId",
                schema: "org",
                table: "Organizations");

            migrationBuilder.DropColumn(
                name: "HeadId",
                schema: "org",
                table: "Organizations");

            migrationBuilder.DropColumn(
                name: "OmsCode",
                schema: "org",
                table: "Organizations");

            migrationBuilder.DropColumn(
                name: "DepartmentId",
                schema: "doc",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "ExecutorId",
                schema: "doc",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "KeyOrgId",
                schema: "doc",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "SignStartAt",
                schema: "doc",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "InQueue",
                schema: "doc",
                table: "Agreements");

            migrationBuilder.DropColumn(
                name: "IsAdditional",
                schema: "doc",
                table: "Agreements");

            migrationBuilder.DropColumn(
                name: "IsRequired",
                schema: "doc",
                table: "Agreements");

            migrationBuilder.DropColumn(
                name: "OmsCode",
                schema: "doc",
                table: "Agreements");

            migrationBuilder.DropColumn(
                name: "OrgInn",
                schema: "doc",
                table: "Agreements");

            migrationBuilder.DropColumn(
                name: "RouteStepId",
                schema: "doc",
                table: "Agreements");

            migrationBuilder.DropColumn(
                name: "Code",
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

            migrationBuilder.RenameColumn(
                name: "RecipientId",
                schema: "doc",
                table: "Documents",
                newName: "IssuerId");

            migrationBuilder.RenameIndex(
                name: "IX_Documents_RecipientId",
                schema: "doc",
                table: "Documents",
                newName: "IX_Documents_IssuerId");

            migrationBuilder.RenameColumn(
                name: "StageNumber",
                schema: "doc",
                table: "Agreements",
                newName: "Step");

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

            migrationBuilder.AlterColumn<DateTime>(
                name: "Date",
                schema: "doc",
                table: "Documents",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

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

            migrationBuilder.AlterColumn<string>(
                name: "Short",
                schema: "doc",
                table: "DocumentTypes",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "NameEn",
                schema: "doc",
                table: "DocumentTypes",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(300)",
                oldMaxLength: 300,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                schema: "doc",
                table: "DocumentTypes",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(300)",
                oldMaxLength: 300,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                schema: "doc",
                table: "DocumentTypes",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(4000)",
                oldMaxLength: 4000,
                oldNullable: true);

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
