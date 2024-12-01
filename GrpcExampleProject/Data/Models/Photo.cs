using System.ComponentModel.DataAnnotations.Schema;
using GrpcTestProject.Models;
namespace GrpcTestProject.Models;

public class Photo
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string ImagePath { get; set; }
    public int Year { get; set; }
    [ForeignKey("GalleryId")]
    public int GalleryId { get; set; }

    public virtual Gallery Gallery { get; set; }
}