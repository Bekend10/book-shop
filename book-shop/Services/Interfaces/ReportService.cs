using book_shop.Dto;
using book_shop.Repositories.Interfaces;
using book_shop.Services.Implementations;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using static book_shop.Enums.OrderEnumStatus;

namespace book_shop.Services.Interfaces
{
    public class ReportService : IReportService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IWebHostEnvironment _env;
        private readonly IAuthorRepository _authorRepository;

        public ReportService(IOrderRepository orderRepository, IWebHostEnvironment env, IAuthorRepository authorRepository)
        {
            _orderRepository = orderRepository;
            _env = env;
            _authorRepository = authorRepository;
        }
        public async Task<byte[]> ExportOrderReportAsync(OrderRequestModel query)
        {
            var templatePath = Path.Combine(_env.ContentRootPath, "resources", "excel", "OrdersReport.xlsx");

            var fileInfo = new FileInfo(templatePath);
            using var package = new ExcelPackage(fileInfo);

            var orders = (await _orderRepository.GetAllAsync()).ToList();

            var sheetOrders = package.Workbook.Worksheets["Orders"];
            var sheetOrderDetail = package.Workbook.Worksheets["Order Details"];

            if (query.startDate.HasValue && query.endDate.HasValue)
            {
                orders = orders.Where(o => o.order_date >= query.startDate.Value && o.order_date <= query.endDate.Value).ToList();
                foreach (var cell in sheetOrders.Cells[sheetOrders.Dimension.Address])
                {
                    if (cell.Value is string strVal && strVal.Contains("{"))
                    {
                        cell.Value = strVal
                            .Replace("{0}", query.startDate.Value.Month.ToString())
                            .Replace("{1}", query.startDate.Value.Year.ToString())
                            .Replace("{2}", query.endDate.Value.Month.ToString())
                            .Replace("{3}", query.endDate.Value.Year.ToString());
                    }
                }
                foreach (var cell in sheetOrderDetail.Cells[sheetOrderDetail.Dimension.Address])
                {
                    if (cell.Value is string strVal && strVal.Contains("{"))
                    {
                        cell.Value = strVal
                            .Replace("{0}", query.startDate.Value.Month.ToString())
                            .Replace("{1}", query.startDate.Value.Year.ToString())
                            .Replace("{2}", query.endDate.Value.Month.ToString())
                            .Replace("{3}", query.endDate.Value.Year.ToString());
                    }
                }
            }
            else if (query.startDate.HasValue)
            {
                orders = orders.Where(o => o.order_date >= query.startDate.Value).ToList();
            }
            else if (query.endDate.HasValue)
            {
                orders = orders.Where(o => o.order_date <= query.endDate.Value).ToList();
            }
            else
            {
                foreach (var cell in sheetOrders.Cells[sheetOrders.Dimension.Address])
                {
                    if (cell.Value is string strVal && strVal.Contains("{"))
                    {
                        cell.Value = strVal
                            .Replace("{0}", DateTime.MinValue.Month.ToString())
                            .Replace("{1}", (DateTime.Now.Year - 2).ToString())
                            .Replace("{2}", DateTime.Now.Month.ToString())
                            .Replace("{3}", DateTime.Now.Year.ToString());
                    }
                }

                foreach (var cell in sheetOrderDetail.Cells[sheetOrderDetail.Dimension.Address])
                {
                    if (cell.Value is string strVal && strVal.Contains("{"))
                    {
                        cell.Value = strVal
                            .Replace("{0}", DateTime.MinValue.Month.ToString())
                            .Replace("{1}", (DateTime.Now.Year - 2).ToString())
                            .Replace("{2}", DateTime.Now.Month.ToString())
                            .Replace("{3}", DateTime.Now.Year.ToString());
                    }
                }
            }
            int orderRow = 4;
            int totalBill = 0;
            int totalQuantity = 0;
            for (int i = 0; i < orders.Count; i++)
            {
                var o = orders[i];
                sheetOrders.Cells[orderRow, 1].Value = i + 1;
                sheetOrders.Cells[orderRow, 2].Value = o.order_id;
                sheetOrders.Cells[orderRow, 3].Value = o.order_date.ToString("dd/MM/yyyy");
                sheetOrders.Cells[orderRow, 4].Value = o.User.first_name + " " + o.User.last_name;
                sheetOrders.Cells[orderRow, 5].Value = o.User.phone_number;
                sheetOrders.Cells[orderRow, 6].Value = o.total_amount.ToString("N0") + " VNĐ";
                sheetOrders.Cells[orderRow, 7].Value = o.status switch
                {
                    OrderStatus.Pending => "Chờ xử lý",
                    OrderStatus.Processing => "Đang xử lý",
                    OrderStatus.Delivered => "Đã giao hàng",
                    OrderStatus.Shipped => "Đã giao",
                    OrderStatus.Cancelled => "Đã hủy",
                    _ => "Không rõ"
                };
                totalBill += o.total_amount;
                totalQuantity += o.orderDetail?.quantity ?? 0;
                orderRow++;
            }

            sheetOrders.Cells[orderRow, 5].Value = "Tổng cộng: " + totalBill.ToString("N0") + " VNĐ";
            sheetOrders.Cells[orderRow, 5, orderRow, 6].Merge = true;
            sheetOrders.Cells[orderRow, 5, orderRow, 6].Style.Font.Bold = true;

            sheetOrders.Cells[4, 1, orderRow, 7].AutoFitColumns();
            sheetOrders.Cells[4, 1, orderRow, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            sheetOrders.Cells[4, 1, orderRow, 7].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            sheetOrders.Cells[4, 1, orderRow, 7].Style.WrapText = true;

            for (int row = 4; row <= orderRow; row++)
            {
                for (int col = 1; col <= 7; col++)
                {
                    var cell = sheetOrders.Cells[row, col];
                    cell.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    cell.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    cell.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    cell.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                }
            }

            var orderDetailRow = 4;
            var totalBill2 = 0;
            var totalPrice = 0;
            for (int i = 0; i < orders.Count; i++)
            {
                var o = orders[i];
                var authorResult = await _authorRepository.GetAuthorsByBookId(o.orderDetail?.book?.book_id ?? 0);
                var author = authorResult.name;
                sheetOrderDetail.Cells[orderDetailRow, 1].Value = i + 1;
                sheetOrderDetail.Cells[orderDetailRow, 2].Value = o.order_id;
                if(o.orderDetail == null)
                {
                    sheetOrderDetail.Cells[orderDetailRow, 3].Value = "";
                    sheetOrderDetail.Cells[orderDetailRow, 4].Value = "";
                    sheetOrderDetail.Cells[orderDetailRow, 5].Value = "";
                    sheetOrderDetail.Cells[orderDetailRow, 6].Value = "";
                    sheetOrderDetail.Cells[orderDetailRow, 7].Value = "";
                    sheetOrderDetail.Cells[orderDetailRow, 8].Value = "";
                    orderDetailRow++;
                    continue;
                }
                sheetOrderDetail.Cells[orderDetailRow, 3].Value = o.orderDetail.book.title;
                sheetOrderDetail.Cells[orderDetailRow, 4].Value = o.orderDetail.book.publisher;
                sheetOrderDetail.Cells[orderDetailRow, 5].Value = author;
                sheetOrderDetail.Cells[orderDetailRow, 6].Value = o.orderDetail.quantity;
                sheetOrderDetail.Cells[orderDetailRow, 7].Value = o.orderDetail.unit_price.ToString("N0") + " VNĐ";
                sheetOrderDetail.Cells[orderDetailRow, 8].Value = (o.orderDetail.unit_price * o.orderDetail.quantity).ToString("N0") + " VNĐ";
                totalBill2 += (o.orderDetail.unit_price * o.orderDetail.quantity);
                totalPrice += o.orderDetail.unit_price;
                orderDetailRow++;
            }

            sheetOrderDetail.Cells[orderDetailRow, 5].Value = "Tổng cộng: ";
            sheetOrderDetail.Cells[orderDetailRow, 6].Value = totalQuantity.ToString();
            sheetOrderDetail.Cells[orderDetailRow, 7].Value = totalPrice.ToString("N0") + " VNĐ";
            sheetOrderDetail.Cells[orderDetailRow, 8].Value = totalBill2.ToString("N0") + " VNĐ";
            sheetOrderDetail.Cells[orderDetailRow, 5, orderDetailRow, 8].Style.Font.Bold = true;

            sheetOrderDetail.Cells[4, 1, orderDetailRow, 8].AutoFitColumns();
            sheetOrderDetail.Cells[4, 1, orderDetailRow, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            sheetOrderDetail.Cells[4, 1, orderDetailRow, 8].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            sheetOrderDetail.Cells[4, 1, orderDetailRow, 8].Style.WrapText = true;

            for (int row = 4; row <= orderDetailRow; row++)
            {
                for (int col = 1; col <= 8; col++)
                {
                    var cell = sheetOrderDetail.Cells[row, col];
                    cell.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    cell.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    cell.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    cell.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                }
            }

            return package.GetAsByteArray();
        }
    }
}
