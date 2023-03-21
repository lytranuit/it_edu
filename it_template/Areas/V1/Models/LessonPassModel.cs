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

	[Table("edu_lesson_pass")]
	public class LessonPassModel
	{
		[Key]
		public int id { get; set; }

		public int? course_id { get; set; }
		[ForeignKey("course_id")]
		public virtual CourseModel? course { get; set; }
		public int? lesson_id { get; set; }

		[ForeignKey("lesson_id")]
		public virtual LessonModel? lesson { get; set; }

		public string? user_id { get; set; }

		[ForeignKey("user_id")]
		public virtual UserModel? user { get; set; }
		public DateTime? date_pass { get; set; }

	}
}
