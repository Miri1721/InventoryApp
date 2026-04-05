namespace InventoryApp.Mobile.Models
{
    public class ItemModel
    {
        public Guid ItemId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public Guid CategoryId { get; set; }
        public Guid OrganizationId { get; set; }
        public string Unit { get; set; } = string.Empty;
        public double CurrentQuantity { get; set; }
        public double MinimumThreshold { get; set; }
        public string Supplier { get; set; } = string.Empty;
        public bool IsBelowThreshold { get; set; }

        public double MissingQuantity => MinimumThreshold > CurrentQuantity
                        ? MinimumThreshold - CurrentQuantity
                        : 0;
        public string MissingText => $"Missing: {MissingQuantity}";

        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }

        public bool IsDeactivated => !IsActive && !IsDeleted;

        public bool ShowDeactivateButton => IsActive && !IsDeleted;
        public bool ShowReactivateButton => !IsActive && !IsDeleted;
        public bool ShowDeleteButton => !IsDeleted;
        public bool ShowEditButton => IsActive && !IsDeleted;
        public bool ShowRecordMovementButton => IsActive && !IsDeleted;
        public bool ShowViewHistoryButton => !IsDeleted;
        public bool ShowDeactivatedBadge => !IsActive && !IsDeleted;

        public bool ShowBelowThresholdWarning => !IsDeleted && IsBelowThreshold;
        public bool IsActiveAndBelowThreshold => !IsDeleted && IsActive && IsBelowThreshold;
    }
}