namespace Core.Constants;

public static class AppRoles
{
    public const string Admin = "Admin";
    public const string Educator = "Educator";
    public const string User = "User";

    public const string AdminOrEducator = Admin + "," + Educator;
}
