using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace EVA.EIMS.Entity.ComplexEntities
{
    public class GetUserQuestions
    {
        [Key]
        public int QuestionId { get; set; }
        public string Question { get; set; }



    }
}
