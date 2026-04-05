namespace InventoryApp.Mobile.Models
{
    public class CategoryModel
    {
        public Guid CategoryId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Guid OrganizationId { get; set; }

        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }

        public bool IsDeactivated => !IsActive && !IsDeleted;

        public bool ShowDeactivateButton => IsActive && !IsDeleted;
        public bool ShowReactivateButton => !IsActive && !IsDeleted;
        public bool ShowDeleteButton => !IsDeleted;
        public bool ShowEditButton => IsActive && !IsDeleted;
        public bool ShowDeactivatedBadge => !IsActive && !IsDeleted;
    }
}