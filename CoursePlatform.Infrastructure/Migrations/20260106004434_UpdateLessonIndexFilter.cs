using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CoursePlatform.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateLessonIndexFilter : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Lessons_CourseId",
                table: "Lessons",
                column: "CourseId");

            migrationBuilder.DropIndex(
                name: "IX_Lesson_CourseId_Order",
                table: "Lessons");

            migrationBuilder.CreateIndex(
                name: "IX_Lesson_CourseId_Order",
                table: "Lessons",
                columns: new[] { "CourseId", "Order" },
                unique: true,
                filter: "IsDeleted = 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Lesson_CourseId_Order",
                table: "Lessons");

            migrationBuilder.CreateIndex(
                name: "IX_Lesson_CourseId_Order",
                table: "Lessons",
                columns: new[] { "CourseId", "Order" },
                unique: true);

            migrationBuilder.DropIndex(
                name: "IX_Lessons_CourseId",
                table: "Lessons");
        }
    }
}
