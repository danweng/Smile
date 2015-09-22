using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace iVoting.Models
{
    public class VotingModel
    {
        [Key]
        public int ID { get; set; }

        [Required]
        [Display(Name = "我是")]
        public string Gender { get; set; }

        [Required]
        [Display(Name = "很興奮")]
        public int Exciting { get; set; }

        [Required]
        [Display(Name = "好開心")]
        public int Happy { get; set; }

        [Required]
        [Display(Name = "很難過")]
        public int Sad { get; set; }

        [Required]
        [Display(Name = "好生氣")]
        public int Upset { get; set; }
    }

    public enum Emotion
    {
        Exciting = 0,
        Happy = 1,
        Sad = 2,
        Upset = 3
    }

    public class VotingDBContext : DbContext
    {
        public DbSet<VotingModel> Votings { get; set; }
    }
}