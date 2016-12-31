using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Overseer.WebApp.ViewModels.Search
{
    public class _QueryViewModel
    {
        public _QueryViewModel()
        {
            UserQueryResults = new List<UserQueryResult>();
            EnvironmentQueryResults = new List<EnvironmentQueryResult>();
            MachineQueryResults = new List<MachineQueryResult>();
        }

        public List<UserQueryResult> UserQueryResults { get; set; }

        public List<EnvironmentQueryResult> EnvironmentQueryResults { get; set; }

        public List<MachineQueryResult> MachineQueryResults { get; set; }
    }

    public abstract class QueryResult
    {
        public QueryResult()
        {
            MatchedProperties = new List<MatchedProperty>();
        }

        public int QueryResultType { get; set; }

        public string ResultName { get; set; }

        public List<MatchedProperty> MatchedProperties;
    }

    public abstract class GenericQueryResult<T> : QueryResult
    {
        public T ResultId { get; set; }
    }

    public class UserQueryResult : GenericQueryResult<int>
    {
        public UserQueryResult()
        {
            QueryResultType = 0;
        }
    }

    public class EnvironmentQueryResult : GenericQueryResult<int>
    {
        public EnvironmentQueryResult()
        {
            QueryResultType = 1;
        }
    }

    public class MachineQueryResult : GenericQueryResult<Guid>
    {
        public MachineQueryResult()
        {
            QueryResultType = 2;
        }

        public string ParentEnvironmentName { get; set; }
    }

    public class MatchedProperty
    {
        public MatchedProperty()
        {
            PropertyValue = new List<string>();
        }

        public string PropertyName;

        public List<string> PropertyValue;

        public string MatchedSubstring;
    }
}