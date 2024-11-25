using System.ComponentModel.DataAnnotations.Schema;
using static GrpcTestProject.TrafficResponse.Types;

namespace GrpcTestProject.Models;

public class Traffic
{
    public int Id { get; set; }
    [ForeignKey("LocationId")]
    public int LocationId { get; set; }
    public TrafficStatus TrafficStatus { get; set; }
    public string Note { get; set; } = "";
    public virtual Location Location { get; set; }
}