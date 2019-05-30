using MyEvernote.Entities.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyEvernote.BusinessLayer.Results
{
  public class BusinessLayerResult<T>where T:class
    {
        public List<MessageObject> Errors { get; set; }
        public T Result { get; set; }

        public BusinessLayerResult()
        {
            Errors = new List<MessageObject>();
        }
        public void AddError(MessageCodes code,string message)
        {
            Errors.Add(new MessageObject() { Code = code, Error = message });
        }
    }
}
