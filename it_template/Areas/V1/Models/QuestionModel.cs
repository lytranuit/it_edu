using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using Vue.Models;

namespace it_template.Areas.V1.Models
{

	[Table("edu_question")]
	public class QuestionModel
	{
		[Key]
		public int id { get; set; }

		public string question { get; set; }
		public string? description { get; set; }
		public int? topic_id { get; set; }

		[ForeignKey("topic_id")]
		public virtual TopicModel? topic { get; set; }
		public int? level { get; set; }
		public bool? is_random { get; set; }
		public int? point { get; set; }

		public DateTime? created_at { get; set; }
		public DateTime? updated_at { get; set; }
		public DateTime? deleted_at { get; set; }

		public string? list_anwser { get; set; }

		[NotMapped]
		public virtual List<Anwser>? anwsers
		{
			get
			{
				//Console.WriteLine(settings);
				return JsonSerializer.Deserialize<List<Anwser>>(string.IsNullOrEmpty(list_anwser) ? "[]" : list_anwser);
			}
			set
			{
				list_anwser = JsonSerializer.Serialize(value);
			}
		}
	}

	public class Anwser
	{
		[Key]
		public string id { get; set; }
		public string anwser { get; set; }
		public bool is_true { get; set; }
	}
}