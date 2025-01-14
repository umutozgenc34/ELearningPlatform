using Core.Models;
using Microsoft.AspNetCore.Identity;

namespace ELearningPlatform.Model.Users.Entity;


public class User : IdentityUser, IAuditEntity
{
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public DateTime Created { get; set; }
    public DateTime Updated { get; set; }
}