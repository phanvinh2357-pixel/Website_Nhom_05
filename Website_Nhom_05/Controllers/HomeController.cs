using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Website_Nhom_05.Models;

namespace Website_Nhom_05.Controllers
{
    public class HomeController : Controller
    {
        // Khởi tạo đối tượng kết nối CSDL qua Entity Framework
        QL_DongHoEntities db = new QL_DongHoEntities();

        // 1. Action hiển thị Trang chủ (Lấy 8 đồng hồ đang kinh doanh mới nhất)
        public ActionResult Index()
        {
            List<DongHo> dsDongHo = db.DongHoes
                                      .Where(n => n.TrangThaiSanPham == true)
                                      .OrderByDescending(n => n.MaDongHo)
                                      .Take(8)
                                      .ToList();
            return View(dsDongHo);
        }

        // 2. Child Action: Menu Danh mục Loại Đồng Hồ
        public ActionResult _DSMenuLoai()
        {
            List<LoaiDongHo> dsLoai = db.LoaiDongHoes.ToList();
            return PartialView(dsLoai);
        }

        // 3. Child Action: Menu Dropdown Thương Hiệu
        public ActionResult _DSMenuThuongHieu()
        {
            List<ThuongHieu> dsTH = db.ThuongHieux.OrderBy(n => n.TenThuongHieu).ToList();
            return PartialView(dsTH);
        }

        // 4. Child Action: Hiển thị tổng số lượng mặt hàng trong Giỏ hàng trên Header
        public ActionResult _CartSummary()
        {
            List<CartItem> lstGioHang = Session["GioHang"] as List<CartItem>;
            int tongSoLuong = 0;
            if (lstGioHang != null)
            {
                tongSoLuong = lstGioHang.Sum(n => n.iSoLuong);
            }
            ViewBag.SoLuongGioHang = tongSoLuong;
            return PartialView();
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult Contact()
        {
            return View();
        }
    }
}