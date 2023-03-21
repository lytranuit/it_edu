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

	[Table("edu_lesson_file")]
	public class LessonFileModel
	{
		[Key]
		public int id { get; set; }

		public string? name { get; set; }
		public string? url { get; set; }

		public long? size { get; set; }
		public string? ext { get; set; }
		public string? mimeType { get; set; }
		public int? lesson_id { get; set; }

		[ForeignKey("lesson_id")]
		public virtual LessonModel? lesson { get; set; }


		public DateTime? created_at { get; set; }
		public DateTime? updated_at { get; set; }
		public DateTime? deleted_at { get; set; }
	}
}
