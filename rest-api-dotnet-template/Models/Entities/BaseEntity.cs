using System.ComponentModel.DataAnnotations;

namespace rest_api_dotnet_template.Models
{
    public abstract class BaseEntity
    {
        [Key]
        public int Id { get; set; }
    }
}