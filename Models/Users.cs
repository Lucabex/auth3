namespace auth3.Models;
public class User
{
    public int Id {get;set;}
    public string Name {get;set;}=string.Empty;
    public string HashedPassword {get; set;}=string.Empty;

}