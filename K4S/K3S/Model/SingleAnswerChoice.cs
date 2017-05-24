using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace K3S.Model
{
    public class SingleAnswerChoice
    {
        public string SingleChoice_View_Answer_AnswerText { get; set; }
        public int SingleChoice_View_Answer_AnswerIndex { get; set; }
        public int SingleChoice_View_Answer_AnswerCoding { get; set; }
        public string SingleChoice_View_Answer_AnswerCodes_VariableName { get; set; }
        public int SingleChoice_View_Answer_AnswerCodes_UnCheckedCode { get; set; }
        public string SingleChoice_View_Answer_AnswerCodes_CheckedCode { get; set; }
        public bool SingleChoice_View_Answer_AnswerCodes_OtherSpecify { get; set; }

        public string SingleChoice_View_Answer_AnswerCodes_OtherSpecify_VariableName { get; set; }
        public string SingleChoice_View_Answer_AnswerCodes_OtherSpecify_InType { get; set; }     
        public int IsRandom { get; set; }

    }
}
