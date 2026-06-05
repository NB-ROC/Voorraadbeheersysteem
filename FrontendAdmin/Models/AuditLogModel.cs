namespace FrontendAdmin.Models;

public class AuditLogModel
{
    public int Id { get; set; }
    public string Timestamp { get; set; } = "";
    public string ActorName { get; set; } = "";
    public string Action { get; set; } = "";
    public string EntityType { get; set; } = "";
    public string EntityId { get; set; } = "";
    public string Description { get; set; } = "";
}