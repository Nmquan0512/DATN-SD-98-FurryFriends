using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FurryFriends.API.Migrations
{
    /// <inheritdoc />
    public partial class _5664 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HoaDons_NhanViens_NhanVienId",
                table: "HoaDons");

            migrationBuilder.AlterColumn<Guid>(
                name: "NhanVienId",
                table: "HoaDons",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.UpdateData(
                table: "ChucVus",
                keyColumn: "ChucVuId",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                columns: new[] { "NgayCapNhat", "NgayTao" },
                values: new object[] { new DateTime(2025, 7, 31, 5, 56, 0, 656, DateTimeKind.Utc).AddTicks(8989), new DateTime(2025, 7, 31, 5, 56, 0, 656, DateTimeKind.Utc).AddTicks(8989) });

            migrationBuilder.UpdateData(
                table: "NhanViens",
                keyColumn: "NhanVienId",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                columns: new[] { "NgayCapNhat", "NgayTao" },
                values: new object[] { new DateTime(2025, 7, 31, 5, 56, 0, 656, DateTimeKind.Utc).AddTicks(9009), new DateTime(2025, 7, 31, 5, 56, 0, 656, DateTimeKind.Utc).AddTicks(9008) });

            migrationBuilder.UpdateData(
                table: "TaiKhoans",
                keyColumn: "TaiKhoanId",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                column: "NgayTaoTaiKhoan",
                value: new DateTime(2025, 7, 31, 5, 56, 0, 656, DateTimeKind.Utc).AddTicks(8902));

            migrationBuilder.AddForeignKey(
                name: "FK_HoaDons_NhanViens_NhanVienId",
                table: "HoaDons",
                column: "NhanVienId",
                principalTable: "NhanViens",
                principalColumn: "NhanVienId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HoaDons_NhanViens_NhanVienId",
                table: "HoaDons");

            migrationBuilder.AlterColumn<Guid>(
                name: "NhanVienId",
                table: "HoaDons",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "ChucVus",
                keyColumn: "ChucVuId",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                columns: new[] { "NgayCapNhat", "NgayTao" },
                values: new object[] { new DateTime(2025, 7, 23, 13, 39, 50, 156, DateTimeKind.Utc).AddTicks(4201), new DateTime(2025, 7, 23, 13, 39, 50, 156, DateTimeKind.Utc).AddTicks(4201) });

            migrationBuilder.UpdateData(
                table: "NhanViens",
                keyColumn: "NhanVienId",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                columns: new[] { "NgayCapNhat", "NgayTao" },
                values: new object[] { new DateTime(2025, 7, 23, 13, 39, 50, 156, DateTimeKind.Utc).AddTicks(4252), new DateTime(2025, 7, 23, 13, 39, 50, 156, DateTimeKind.Utc).AddTicks(4251) });

            migrationBuilder.UpdateData(
                table: "TaiKhoans",
                keyColumn: "TaiKhoanId",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                column: "NgayTaoTaiKhoan",
                value: new DateTime(2025, 7, 23, 13, 39, 50, 156, DateTimeKind.Utc).AddTicks(4091));

            migrationBuilder.AddForeignKey(
                name: "FK_HoaDons_NhanViens_NhanVienId",
                table: "HoaDons",
                column: "NhanVienId",
                principalTable: "NhanViens",
                principalColumn: "NhanVienId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
