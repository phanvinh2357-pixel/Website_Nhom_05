using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Data.Entity;
using Website_Nhom_05.Models;

namespace Website_Nhom_05.Controllers
{
    public partial class DongHoController : Controller
    {
        QL_DongHoEntities dbDetails = new QL_DongHoEntities();

        public ActionResult Details(int id)
        {
            // Lấy thông tin đồng hồ và các quan hệ liên quan
            DongHo dongHo = dbDetails.DongHoes
                .Include("ThuongHieu")
                .Include("LoaiDongHo")
                .SingleOrDefault(d => d.MaDongHo == id);

            if (dongHo == null)
            {
                return HttpNotFound();
            }

            // Lấy đánh giá thật từ database
            List<DanhGia> danhSachDanhGia = dbDetails.DanhGias
                .Include("KhachHang")
                .Where(d => d.MaDongHo == id)
                .OrderByDescending(d => d.NgayDanhGia)
                .ToList();

            // Tính điểm trung bình và số lượng đánh giá
            int tongDanhGia = danhSachDanhGia.Count;

            double diemTrungBinh = tongDanhGia > 0
                ? Math.Round(danhSachDanhGia.Average(d => d.DiemDanhGia), 1)
                : 0;

            // Đếm số đánh giá theo từng mức sao
            int sao5 = danhSachDanhGia.Count(d => d.DiemDanhGia == 5);
            int sao4 = danhSachDanhGia.Count(d => d.DiemDanhGia == 4);
            int sao3 = danhSachDanhGia.Count(d => d.DiemDanhGia == 3);
            int sao2 = danhSachDanhGia.Count(d => d.DiemDanhGia == 2);
            int sao1 = danhSachDanhGia.Count(d => d.DiemDanhGia == 1);

            // Truyền dữ liệu đánh giá sang View
            ViewBag.TongDanhGia = tongDanhGia;
            ViewBag.DiemTrungBinh = diemTrungBinh;

            ViewBag.Sao5 = sao5;
            ViewBag.Sao4 = sao4;
            ViewBag.Sao3 = sao3;
            ViewBag.Sao2 = sao2;
            ViewBag.Sao1 = sao1;

            ViewBag.PhanTram5 = tongDanhGia > 0
                ? (int)Math.Round((double)sao5 / tongDanhGia * 100) : 0;

            ViewBag.PhanTram4 = tongDanhGia > 0
                ? (int)Math.Round((double)sao4 / tongDanhGia * 100) : 0;

            ViewBag.PhanTram3 = tongDanhGia > 0
                ? (int)Math.Round((double)sao3 / tongDanhGia * 100) : 0;

            ViewBag.PhanTram2 = tongDanhGia > 0
                ? (int)Math.Round((double)sao2 / tongDanhGia * 100) : 0;

            ViewBag.PhanTram1 = tongDanhGia > 0
                ? (int)Math.Round((double)sao1 / tongDanhGia * 100) : 0;

            ViewBag.DanhSachDanhGia = danhSachDanhGia;

            // Lấy tối đa 4 đồng hồ cùng loại để gợi ý
            List<DongHo> sanPhamLienQuan = dbDetails.DongHoes
                .Where(d => d.MaLoai == dongHo.MaLoai
                         && d.MaDongHo != dongHo.MaDongHo
                         && d.TrangThai == true)
                .OrderByDescending(d => d.MaDongHo)
                .Take(4)
                .ToList();

            ViewBag.SanPhamLienQuan = sanPhamLienQuan;

            return View(dongHo);
        }
    }
}
