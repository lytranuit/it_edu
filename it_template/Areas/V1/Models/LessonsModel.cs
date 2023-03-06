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

	[Table("edu_lessons")]
	public class LessonsModel
	{
		[Key]
		public int id { get; set; }

		public string title { get; set; }
		public string? description { get; set; }

		public string? type { get; set; }

		public int? course_id { get; set; }

		[ForeignKey("course_id")]
		public virtual CoursesModel? course { get; set; }

		public int? chapter_id { get; set; }

		[ForeignKey("chapter_id")]
		public virtual ChaptersModel? chapter { get; set; }

		public int? nextLesson { get; set; }

		public int? duration { get; set; } /// SEC

		public DateTime? created_at { get; set; }
		public DateTime? updated_at { get; set; }
		public DateTime? deleted_at { get; set; }

	}
}
