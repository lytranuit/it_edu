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

	[Table("edu_lesson")]
	public class LessonModel
	{
		[Key]
		public int id { get; set; }

		public string title { get; set; }
		public string? description { get; set; }

		public int? type { get; set; }
		public int course_id { get; set; }
		[ForeignKey("course_id")]
		public virtual CourseModel? course { get; set; }
		public int? chapter_id { get; set; }

		[ForeignKey("chapter_id")]
		public virtual ChapterModel? chapter { get; set; }

		public virtual List<LessonAttachmentModel>? attachments_up { get; set; }

		public virtual LessonFileModel? file_up { get; set; }

		public int? nextLesson { get; set; }

		public int? duration { get; set; } /// SEC
		public string? text { get; set; } /// 
		public string? file_url { get; set; } /// 
		public string? youtube_id { get; set; } /// 
		public int? exam_id { get; set; } /// 
		// a dummy wrapper

		public DateTime? created_at { get; set; }
		public DateTime? updated_at { get; set; }
		public DateTime? deleted_at { get; set; }
	}
}
