using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HC.AI.MAPI.Models
{
    public class BaseModel
    {
        public string Message { get; set; }
        public bool IsNotValid { get; set; }
        public string LLMProviderName { get; set; }
    }


}
