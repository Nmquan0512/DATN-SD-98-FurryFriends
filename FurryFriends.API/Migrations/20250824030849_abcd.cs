using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FurryFriends.API.Migrations
{
    /// <inheritdoc />
    public partial class abcd : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PhieuHoanTras",
                columns: table => new
                {
                    PhieuHoanTraId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HoaDonChiTietId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SoLuongHoan = table.Column<int>(type: "int", nullable: false),
                    NgayHoanTra = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LyDoHoanTra = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TrangThai = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhieuHoanTras", x => x.PhieuHoanTraId);
                    table.ForeignKey(
                        name: "FK_PhieuHoanTras_HoaDonChiTiets_HoaDonChiTietId",
                        column: x => x.HoaDonChiTietId,
                        principalTable: "HoaDonChiTiets",
                        principalColumn: "HoaDonChiTietId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "ChucVus",
                keyColumn: "ChucVuId",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                columns: new[] { "NgayCapNhat", "NgayTao" },
                values: new object[] { new DateTime(2025, 8, 24, 3, 8, 47, 725, DateTimeKind.Utc).AddTicks(560), new DateTime(2025, 8, 24, 3, 8, 47, 725, DateTimeKind.Utc).AddTicks(559) });

            migrationBuilder.UpdateData(
                table: "NhanViens",
                keyColumn: "NhanVienId",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                columns: new[] { "NgayCapNhat", "NgayTao" },
                values: new object[] { new DateTime(2025, 8, 24, 3, 8, 47, 725, DateTimeKind.Utc).AddTicks(593), new DateTime(2025, 8, 24, 3, 8, 47, 725, DateTimeKind.Utc).AddTicks(592) });

            migrationBuilder.UpdateData(
                table: "TaiKhoans",
                keyColumn: "TaiKhoanId",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                column: "NgayTaoTaiKhoan",
                value: new DateTime(2025, 8, 24, 3, 8, 47, 725, DateTimeKind.Utc).AddTicks(340));

            migrationBuilder.CreateIndex(
                name: "IX_PhieuHoanTras_HoaDonChiTietId",
                table: "PhieuHoanTras",
                column: "HoaDonChiTietId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PhieuHoanTras");

            migrationBuilder.UpdateData(
                table: "ChucVus",
                keyColumn: "ChucVuId",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                columns: new[] { "NgayCapNhat", "NgayTao" },
                values: new object[] { new DateTime(2025, 8, 18, 11, 51, 41, 22, DateTimeKind.Utc).AddTicks(1909), new DateTime(2025, 8, 18, 11, 51, 41, 22, DateTimeKind.Utc).AddTicks(1789) });

            migrationBuilder.UpdateData(
                table: "NhanViens",
                keyColumn: "NhanVienId",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                columns: new[] { "NgayCapNhat", "NgayTao" },
                values: new object[] { new DateTime(2025, 8, 18, 11, 51, 41, 22, DateTimeKind.Utc).AddTicks(2309), new DateTime(2025, 8, 18, 11, 51, 41, 22, DateTimeKind.Utc).AddTicks(2169) });

            migrationBuilder.UpdateData(
                table: "TaiKhoans",
                keyColumn: "TaiKhoanId",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                column: "NgayTaoTaiKhoan",
                value: new DateTime(2025, 8, 18, 11, 51, 41, 21, DateTimeKind.Utc).AddTicks(8698));
        }
    }
}
