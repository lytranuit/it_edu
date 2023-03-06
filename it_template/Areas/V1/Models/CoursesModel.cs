using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Vue.Models;

namespace it_template.Areas.V1.Models
{

	[Table("edu_courses")]
	public class CoursesModel
	{
		[Key]
		public int id { get; set; }

		public string title { get; set; }
		public string? description { get; set; }
		public string? code { get; set; }
		public string? author_id { get; set; }

		public int? type_id { get; set; }

		[ForeignKey("author_id")]
		public virtual UserModel? author { get; set; }


		public DateTime? date_start { get; set; }
		public DateTime? date_end { get; set; }

		public DateTime? created_at { get; set; }
		public DateTime? updated_at { get; set; }
		public DateTime? deleted_at { get; set; }

	}
}
