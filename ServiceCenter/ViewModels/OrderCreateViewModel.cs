using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace ServiceCenter.ViewModels
{
    public class OrderCreateViewModel
    {
        [Required(ErrorMessage = "Дата заказа обязательна")]
        [Display(Name = "Дата заказа")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-ddTHH:mm}", ApplyFormatInEditMode = true)]
        public DateTime OrderDate { get; set; } = DateTime.Now;

        [Display(Name = "Дата возврата")]
        public DateTime? ReturnDate { get; set; }

        [Display(Name = "Гарантия")]
        public bool HasWarranty { get; set; }

        [Display(Name = "Срок гарантии (дней)")]
        [Range(0, 365, ErrorMessage = "Срок гарантии должен быть от 0 до 365 дней")]
        public int? WarrantyPeriodDays { get; set; }

        [Required(ErrorMessage = "Цена обязательна")]
        [Range(0, 100000, ErrorMessage = "Цена должна быть положительной")]
        [Display(Name = "Общая цена")]
        public decimal TotalPrice { get; set; }

        [Required(ErrorMessage = "Выберите клиента")]
        [Display(Name = "Клиент")]
        public int CustomerId { get; set; }

        [Required(ErrorMessage = "Серийный номер обязателен")]
        [StringLength(17, MinimumLength = 17, ErrorMessage = "Серийный номер должен содержать 17 символов")]
        [RegularExpression(@"^[A-Z0-9]{17}$", ErrorMessage = "Недопустимые символы в серийном номере")]
        [Display(Name = "Серийный номер автомобиля")]
        public string CarSerialNumber { get; set; }

        [Required(ErrorMessage = "Выберите сотрудника")]
        [Display(Name = "Сотрудник")]
        public int EmployeeId { get; set; }

        [Display(Name = "Неисправности")]
        public List<int> SelectedFaultIds { get; set; } = new List<int>();

        // Для dropdown списков
        public SelectList? Customers { get; set; }
        public SelectList? Employees { get; set; }
        public SelectList? Cars { get; set; }
        public MultiSelectList? FaultTypes { get; set; }
    }
}