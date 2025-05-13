namespace COSMESTIC.Models.User
{
    public class ListUserModel
    {
        public List<UserViewModel> Users { get; set; }
        public string Name { get; set; }
        public List<string> Roles { get; set; }
        public List<string> Statuses { get; set; }
        public string SelectedRole { get; set; }
        public string SelectedStatus { get; set; }
    }
}
