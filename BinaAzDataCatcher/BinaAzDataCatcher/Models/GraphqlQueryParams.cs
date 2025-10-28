using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BinaAzDataCatcher.Models
{
    public class PersistedQuery
    {
        public int version { get; set; }
        public string sha256Hash { get; set; }
    }
    public class Extensions
    {
        public PersistedQuery persistedQuery { get; set; }
    }
    public class Variables
    {
        public int first { get; set; }
        public object filter { get; set; }
        public string sort { get; set; }
        public string cursor { get; set; }
    }

    public class GraphqlQueryParams
    {
        public string operationName { get; set; }
        public Variables variables { get; set; }
        public Extensions extensions { get; set; }
    }

}
