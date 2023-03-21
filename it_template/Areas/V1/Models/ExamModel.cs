using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace it_template.Areas.V1.Models
{

	[Table("edu_exam")]
	public class ExamModel
	{
		[Key]
		public int id { get; set; }

		public string title { get; set; }
		public string? description { get; set; }
		public int? duration { get; set; }
		public bool? is_random { get; set; }
		public bool? is_active { get; set; }
		public int? point_pass { get; set; }
		public string? list_type_question { get; set; }


		[NotMapped]
		public virtual List<TypeQuestion>? questions
		{
			get
			{
				return JsonSerializer.Deserialize<List<TypeQuestion>>(string.IsNullOrEmpty(list_type_question) ? "[]" : list_type_question);
			}
			set
			{
				list_type_question = JsonSerializer.Serialize(value);
			}
		}

		public DateTime? created_at { get; set; }
		public DateTime? updated_at { get; set; }
		public DateTime? deleted_at { get; set; }

	}

	public class TypeQuestion
	{
		[Key]
		public int id { get; set; }
		public int num_question { get; set; }
		public int num_topic_question { get; set; }
		public int topic_id { get; set; }
	}
}