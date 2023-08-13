using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MagicVilla_API.Migrations
{
    /// <inheritdoc />
    public partial class AlimentarTablaVilla : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Villas",
                columns: new[] { "Id", "Amenidad", "Detalle", "FechaActualizacion", "FechaCreacion", "ImagenUrl", "MetrosCuadrados", "Nombre", "Ocupantes", "Tarifa" },
                values: new object[,]
                {
                    { 1, "", "Detalle de la villa", new DateTime(2023, 8, 12, 14, 41, 11, 476, DateTimeKind.Local).AddTicks(3577), new DateTime(2023, 8, 12, 14, 41, 11, 476, DateTimeKind.Local).AddTicks(3568), "", 50, "Villa Real", 5, 100.0 },
                    { 2, "", "Detalle a la vista de la playa", new DateTime(2023, 8, 12, 14, 41, 11, 476, DateTimeKind.Local).AddTicks(3579), new DateTime(2023, 8, 12, 14, 41, 11, 476, DateTimeKind.Local).AddTicks(3579), "", 40, "Villa Madrid", 4, 150.0 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Villas",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Villas",
                keyColumn: "Id",
                keyValue: 2);
        }
    }
}
