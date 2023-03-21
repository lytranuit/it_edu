﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Vue.Models;

namespace it_template.Areas.V1.Models
{

	[Table("edu_course_type_group")]
	public class GroupModel
	{
		[Key]
		public int id { get; set; }

		public string name { get; set; }
		public int? stt { get; set; }

		public List<TypeModel>? types { get; set; }

		public DateTime? created_at { get; set; }
		public DateTime? updated_at { get; set; }
		public DateTime? deleted_at { get; set; }


	}
}
