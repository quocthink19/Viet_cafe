using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Models.DTOs.Request
{
    public class ProductRequest
    {
        [Required(ErrorMessage = "Tên sản phẩm là bắt buộc")]
        public string? Name { get; set; }
        [Required(ErrorMessage = "Mô tả sản phẩm là bắt buộc")]
        public string? Description { get; set; }
        [Required(ErrorMessage = "Giá sản phẩm là bắt buộc")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Giá phải lớn hơn 0")]
        public double? Price { get; set; }
        [Required(ErrorMessage = "Hình ảnh sản phẩm là bắt buộc")]
        public string? Image { get; set; }

        [Required(ErrorMessage = "Danh mục là bắt buộc")]
        public Guid CategoryId { get; set; }
    }
}
