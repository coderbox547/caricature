using Data.Domain;
using Data.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Data.Contract.Response
{
    public class DashBoardStatResponse
    {

        [JsonIgnore]
        public double TotalCollection { get; set; }

        [JsonIgnore]
        public double TotalPendingCollection { get; set; }

        public int TotalMembers { get; set; }
        public int TotalGroups { get; set; }

        public string TotalCollectionAsString => TotalCollection.ToFigure();
        public string TotalPendingAsString => TotalPendingCollection.ToFigure();
    }
}
