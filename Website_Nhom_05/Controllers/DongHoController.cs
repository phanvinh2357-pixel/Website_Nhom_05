using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using Website_Nhom_05.Models;

namespace Website_Nhom_05.Controllers
{
    // Partial class: chung với DongHoController.Details.cs (Nguyễn Ngọc Nhi)
    public partial class DongHoController : Controller
    {
        private const int PageSize = 9; // lưới 3 cột x 3 hàng
        private readonly QL_DongHoEntities dbDanhMuc = new QL_DongHoEntities();

        // GET: DongHo  - Tất cả đồng hồ
        public ActionResult Index(int[] loai, int[] thuongHieu, string mucGia, bool? conHang, string sapXep, int? page)
        {
            ViewBag.TieuDe = "Tất cả đồng hồ";
            var query = dbDanhMuc.DongHoes.Where(d => d.TrangThaiSanPham == true);
            return DanhSach(query, loai, thuongHieu, mucGia, conHang, sapXep, page, true, true);
        }

        // GET: DongHo/DongHoTheoLoai/1 (Nam / Nữ / Đôi)
        public ActionResult DongHoTheoLoai(int id, int[] thuongHieu, string mucGia, bool? conHang, string sapXep, int? page)
        {
            var loai = dbDanhMuc.LoaiDongHoes.Find(id);
            if (loai == null) return HttpNotFound();
            ViewBag.MaLoai = id;
            ViewBag.TieuDe = loai.TenLoai;
            var query = dbDanhMuc.DongHoes.Where(d => d.TrangThaiSanPham == true && d.MaLoai == id);
            return DanhSach(query, null, thuongHieu, mucGia, conHang, sapXep, page, false, true);
        }

        // GET: DongHo/DongHoTheoThuongHieu/1
        public ActionResult DongHoTheoThuongHieu(int id, int[] loai, string mucGia, bool? conHang, string sapXep, int? page)
        {
            var th = dbDanhMuc.ThuongHieux.Find(id);
            if (th == null) return HttpNotFound();
            ViewBag.MaThuongHieu = id;
            ViewBag.TieuDe = "Thương hiệu " + th.TenThuongHieu;
            var query = dbDanhMuc.DongHoes.Where(d => d.TrangThaiSanPham == true && d.MaThuongHieu == id);
            return DanhSach(query, loai, null, mucGia, conHang, sapXep, page, true, false);
        }

        // GET: DongHo/TimKiem?tuKhoa=...
        public ActionResult TimKiem(string tuKhoa, int[] loai, int[] thuongHieu, string mucGia, bool? conHang, string sapXep, int? page)
        {
            tuKhoa = (tuKhoa ?? "").Trim();
            ViewBag.TuKhoa = tuKhoa;
            ViewBag.TieuDe = "Kết quả tìm kiếm";
            var query = dbDanhMuc.DongHoes.Where(d => d.TrangThaiSanPham == true);
            if (tuKhoa.Length > 0)
            {
                query = query.Where(d => d.TenDongHo.Contains(tuKhoa)
                                      || d.ThuongHieu.TenThuongHieu.Contains(tuKhoa)
                                      || d.LoaiDongHo.TenLoai.Contains(tuKhoa));
            }
            return DanhSach(query, loai, thuongHieu, mucGia, conHang, sapXep, page, true, true);
        }

        // Lọc + sắp xếp + phân trang dùng chung
        private ActionResult DanhSach(IQueryable<DongHo> query, int[] loai, int[] thuongHieu, string mucGia,
            bool? conHang, string sapXep, int? page, bool hienLoc_Loai, bool hienLoc_ThuongHieu)
        {
            if (loai != null && loai.Length > 0) query = query.Where(d => loai.Contains(d.MaLoai));
            if (thuongHieu != null && thuongHieu.Length > 0) query = query.Where(d => thuongHieu.Contains(d.MaThuongHieu));

            switch (mucGia)
            {
                case "duoi2": query = query.Where(d => d.GiaBan < 2000000m); break;
                case "2-5": query = query.Where(d => d.GiaBan >= 2000000m && d.GiaBan < 5000000m); break;
                case "5-10": query = query.Where(d => d.GiaBan >= 5000000m && d.GiaBan < 10000000m); break;
                case "tren10": query = query.Where(d => d.GiaBan >= 10000000m); break;
            }

            if (conHang == true) query = query.Where(d => d.SoLuongTon > 0);

            switch (sapXep)
            {
                case "gia-tang": query = query.OrderBy(d => d.GiaBan); break;
                case "gia-giam": query = query.OrderByDescending(d => d.GiaBan); break;
                default: query = query.OrderByDescending(d => d.MaDongHo); break; // mới nhất
            }

            int pageNumber = (page ?? 1) < 1 ? 1 : (page ?? 1);
            ViewBag.Loai = dbDanhMuc.LoaiDongHoes.ToList();
            ViewBag.ThuongHieu = dbDanhMuc.ThuongHieux.ToList();
            ViewBag.SelLoai = loai ?? new int[0];
            ViewBag.SelThuongHieu = thuongHieu ?? new int[0];
            ViewBag.MucGia = mucGia;
            ViewBag.ConHang = conHang == true;
            ViewBag.SapXep = sapXep;
            ViewBag.HienLoc_Loai = hienLoc_Loai;
            ViewBag.HienLoc_ThuongHieu = hienLoc_ThuongHieu;
            ViewBag.Action = RouteData.Values["action"];

            return View(query.ToPagedList(pageNumber, PageSize));
        }
    }
}
