using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FurryFriends.API.Migrations
{
    /// <inheritdoc />
    public partial class db : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "ChucVus",
                keyColumn: "ChucVuId",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                columns: new[] { "NgayCapNhat", "NgayTao" },
                values: new object[] { new DateTime(2025, 8, 25, 0, 17, 29, 705, DateTimeKind.Utc).AddTicks(8881), new DateTime(2025, 8, 25, 0, 17, 29, 705, DateTimeKind.Utc).AddTicks(8881) });

            migrationBuilder.UpdateData(
                table: "NhanViens",
                keyColumn: "NhanVienId",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                columns: new[] { "NgayCapNhat", "NgayTao" },
                values: new object[] { new DateTime(2025, 8, 25, 0, 17, 29, 705, DateTimeKind.Utc).AddTicks(8917), new DateTime(2025, 8, 25, 0, 17, 29, 705, DateTimeKind.Utc).AddTicks(8916) });

            migrationBuilder.UpdateData(
                table: "TaiKhoans",
                keyColumn: "TaiKhoanId",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                column: "NgayTaoTaiKhoan",
                value: new DateTime(2025, 8, 25, 0, 17, 29, 705, DateTimeKind.Utc).AddTicks(8631));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "ChucVus",
                keyColumn: "ChucVuId",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                columns: new[] { "NgayCapNhat", "NgayTao" },
                values: new object[] { new DateTime(2025, 8, 24, 8, 4, 9, 447, DateTimeKind.Utc).AddTicks(106), new DateTime(2025, 8, 24, 8, 4, 9, 447, DateTimeKind.Utc).AddTicks(105) });

            migrationBuilder.UpdateData(
                table: "NhanViens",
                keyColumn: "NhanVienId",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                columns: new[] { "NgayCapNhat", "NgayTao" },
                values: new object[] { new DateTime(2025, 8, 24, 8, 4, 9, 447, DateTimeKind.Utc).AddTicks(169), new DateTime(2025, 8, 24, 8, 4, 9, 447, DateTimeKind.Utc).AddTicks(168) });

            migrationBuilder.UpdateData(
                table: "TaiKhoans",
                keyColumn: "TaiKhoanId",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                column: "NgayTaoTaiKhoan",
                value: new DateTime(2025, 8, 24, 8, 4, 9, 446, DateTimeKind.Utc).AddTicks(9677));
        }
    }
}
