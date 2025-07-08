using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infra.Database.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "public");

            migrationBuilder.CreateTable(
                name: "books",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    title = table.Column<string>(type: "text", nullable: false),
                    author = table.Column<string>(type: "text", nullable: false),
                    publish = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    total_remaining = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    deleted_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_books", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "outbox_events",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    aggregate = table.Column<int>(type: "integer", nullable: false),
                    @event = table.Column<int>(name: "event", type: "integer", nullable: false),
                    payload = table.Column<string>(type: "text", nullable: false),
                    processed = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    deleted_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_outbox_events", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "user",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    first_name = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    last_name = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    password_hash = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    deleted_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "loans",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    loan_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    return_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    book_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    deleted_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_loans", x => x.id);
                    table.ForeignKey(
                        name: "fk_loans_books_book_id",
                        column: x => x.book_id,
                        principalSchema: "public",
                        principalTable: "books",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_loans_book_id",
                schema: "public",
                table: "loans",
                column: "book_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_user_email",
                schema: "public",
                table: "user",
                column: "email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "loans",
                schema: "public");

            migrationBuilder.DropTable(
                name: "outbox_events",
                schema: "public");

            migrationBuilder.DropTable(
                name: "user",
                schema: "public");

            migrationBuilder.DropTable(
                name: "books",
                schema: "public");
        }
    }
}
