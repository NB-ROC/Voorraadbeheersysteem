using System.ComponentModel.DataAnnotations;

namespace Backend.Entities;

public class AuditLog
{
    public int Id { get; set; }
    public DateTime TimeStamp { get; set; }
    public int ActorId { get; set; }
    public User Actor { get; set; } = null!;

    public string Action { get; set; } = null!;      
    public string EntityType { get; set; } = null!;  
    public string EntityId { get; set; } = null!;    
    
    [MaxLength(256)]
    public string Description { get; set; }
    
}