namespace AI.HR.EF.Entities;

public class Role
{
    public int RoleId { get; set; }
    public string RoleName { get; set; } = string.Empty;
    public int OrderId { get; set; }

    public ICollection<User> Users { get; set; } = new List<User>();
}
