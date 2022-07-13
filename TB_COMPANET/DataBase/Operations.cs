using System.ComponentModel.DataAnnotations;

namespace TB_COMPANET.DataBase;

public class Operation
{
    
    [Key] 
    public int Id { set; get; }
    public string CompanyId { set; get; }
    public string Email { set; get; }
    public long Bill { set; get; }
    
    public string Description { set; get; }
    
    public Operation(string companyId, string email, long bill, string description)
    {
        CompanyId = companyId;
        Email = email;
        Bill = bill;
        Description = description;
    }
}