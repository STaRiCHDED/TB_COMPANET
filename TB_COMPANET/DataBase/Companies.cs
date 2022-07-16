using System.ComponentModel.DataAnnotations;

namespace TB_COMPANET.DataBase;

public class Company
{
    [Key]
    public int Id { set; get; }
    
    public string CompanyId { set; get; }
    public string Name { set; get; }
    
    public string? Email { set; get; }
    public string Description { set; get; }

    public Company(string name,string companyId,string? email,string description)
    {
        CompanyId = companyId;
        Name = name;
        Email = email;
        Description = description;
    }
}