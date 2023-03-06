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

	[Table("edu_chapters")]
	public class ChaptersModel
	{
		[Key]
		public int id { get; set; }

		public string title { get; set; }
		public int? course_id { get; set; }

		[ForeignKey("course_id")]
		public virtual CoursesModel? course { get; set; }

	}
}
