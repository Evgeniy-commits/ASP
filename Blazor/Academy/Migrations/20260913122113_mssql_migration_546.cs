using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Academy.Migrations
{
    /// <inheritdoc />
    public partial class mssql_migration_546 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Directions",
                columns: table => new
                {
                    direction_id = table.Column<byte>(type: "TINYINT", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    direction_name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Directions", x => x.direction_id);
                });

            migrationBuilder.CreateTable(
                name: "Disciplines",
                columns: table => new
                {
                    discipline_id = table.Column<short>(type: "SMALLINT", nullable: false),
                    discipline_name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    number_of_lessons = table.Column<byte>(type: "TINYINT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Disciplines", x => x.discipline_id);
                });

            migrationBuilder.CreateTable(
                name: "Groups",
                columns: table => new
                {
                    group_id = table.Column<int>(type: "int", nullable: false),
                    group_name = table.Column<string>(type: "nchar(10)", fixedLength: true, maxLength: 10, nullable: true),
                    direction = table.Column<byte>(type: "TINYINT", nullable: true),
                    weekdays = table.Column<byte>(type: "TINYINT", nullable: true),
                    start_time = table.Column<TimeOnly>(type: "time(0)", precision: 0, nullable: true),
                    start_date = table.Column<DateOnly>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Groups", x => x.group_id);
                });

            migrationBuilder.CreateTable(
                name: "Teachers",
                columns: table => new
                {
                    teacher_id = table.Column<short>(type: "SMALLINT", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    last_name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    first_name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    middle_name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    birth_date = table.Column<DateOnly>(type: "date", nullable: true),
                    email = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    phone = table.Column<string>(type: "nchar(16)", fixedLength: true, maxLength: 16, nullable: true),
                    photo = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    PhotoMimeType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    work_since = table.Column<DateOnly>(type: "date", nullable: true),
                    rate = table.Column<decimal>(type: "smallmoney", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Teachers", x => x.teacher_id);
                });

            migrationBuilder.CreateTable(
                name: "Students",
                columns: table => new
                {
                    stud_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    last_name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    first_name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    middle_name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    birth_date = table.Column<DateOnly>(type: "date", nullable: false),
                    email = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    phone = table.Column<string>(type: "nchar(16)", fixedLength: true, maxLength: 16, nullable: true),
                    photo = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    PhotoMimeType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    group = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Students", x => x.stud_id);
                    table.ForeignKey(
                        name: "FK_Students_Groups",
                        column: x => x.group,
                        principalTable: "Groups",
                        principalColumn: "group_id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Students_group",
                table: "Students",
                column: "group");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Directions");

            migrationBuilder.DropTable(
                name: "Disciplines");

            migrationBuilder.DropTable(
                name: "Students");

            migrationBuilder.DropTable(
                name: "Teachers");

            migrationBuilder.DropTable(
                name: "Groups");
        }
    }
}
