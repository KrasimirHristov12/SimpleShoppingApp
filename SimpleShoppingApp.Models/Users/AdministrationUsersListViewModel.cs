namespace SimpleShoppingApp.Models.Users
{
    public class AdministrationUsersListViewModel
    {
        public AdministrationUsersListViewModel()
        {
            Users = new List<AdministrationUserViewModel>();
        }

        public IEnumerable<AdministrationUserViewModel> Users { get; set; }

        public int TotalPages { get; set; }

        public int CurrentPage { get; set; }
    }
}
