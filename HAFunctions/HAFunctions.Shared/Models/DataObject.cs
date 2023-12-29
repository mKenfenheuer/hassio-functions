namespace HAFunctions.Shared.Models;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class DataObject
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public string? Key { get; set; }
    public string? JsonData { get; set; }   
}