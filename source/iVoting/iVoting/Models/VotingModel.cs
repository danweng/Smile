using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
//using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace iVoting.Models
{
    public class VotingModel
    {
        //[Key]
        public int ID { get; set; }

        [Required(ErrorMessage = "請選擇性別")]
        [Display(Name = "我是")]
        public Gender Gender { get; set; }

        [Required(ErrorMessage = "請選擇感覺")]
        [EnumDataType(typeof(Emotion))]
        public Emotion Emotion { get; set; }

        [Required]
		[Range(0, int.MaxValue, ErrorMessage = "很興奮欄位需大於零!")]
        [Display(Name = "很興奮")]
        public int Exciting { get; set; }

        [Required]
		[Range(0, int.MaxValue, ErrorMessage = "好開心欄位需大於零!")]
        [Display(Name = "好開心")]
        public int Happy { get; set; }

        [Required]
		[Range(0, int.MaxValue, ErrorMessage = "很難過欄位需大於零!")]
        [Display(Name = "很難過")]
        public int Sad { get; set; }

        [Required]
		[Range(0, int.MaxValue, ErrorMessage = "好生氣欄位需大於零!")]
        [Display(Name = "好生氣")]
        public int Upset { get; set; }

		[Display(Name="日期")]
		[DisplayFormat(DataFormatString = "{0: yyyy/MM/dd}", ApplyFormatInEditMode = true)]
        public DateTime VotingDate { get; set; }
    }

    public enum Gender
    { 
        男生 = 0,
        女生 = 1
    }
    public enum Emotion
    {
        很興奮 = 0,
        好開心 = 1,
        很難過 = 2,
        好生氣 = 3
    }

	//public class VotingDBContext : DbContext
	//{
	//	public DbSet<VotingModel> Votings { get; set; }
	//}
}