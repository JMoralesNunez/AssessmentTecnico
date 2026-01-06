using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CoursePlatform.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixLessonIndexWithIsDeleted : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Lesson_CourseId_Order",
                table: "Lessons");

            migrationBuilder.CreateIndex(
                name: "IX_Lesson_CourseId_Order_IsDeleted",
                table: "Lessons",
                columns: new[] { "CourseId", "Order", "IsDeleted" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Lesson_CourseId_Order_IsDeleted",
                table: "Lessons");

            migrationBuilder.CreateIndex(
                name: "IX_Lesson_CourseId_Order",
                table: "Lessons",
                columns: new[] { "CourseId", "Order" },
                unique: true,
                filter: "IsDeleted = 0");
        }
    }
}
