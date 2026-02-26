using System;
using System.Collections.Generic;

namespace BilkoNavigator_.ViewModels
{
    public class AdminUsersPageViewModel
    {
        public List<AdminUserRowViewModel> Users { get; set; } = new();
    }

    public class AdminUserRowViewModel
    {
        public string UserId { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public int TotalFindings { get; set; }
        public List<AdminUserHerbSummaryViewModel> HerbSummaries { get; set; } = new();
    }

    public class AdminUserHerbSummaryViewModel
    {
        public string HerbName { get; set; } = string.Empty;
        public int Count { get; set; }
        public DateTime LastFoundOn { get; set; }
    }
}
