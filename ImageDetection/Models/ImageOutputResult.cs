using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImageDetection.Models
{
    public class ImageOutputResult
    {
        public string fileName { get; set; }
        public List<string> labels { get; set; }
        public string unFolderId{get;set;}
    }
}
