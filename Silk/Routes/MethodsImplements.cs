using Silk.Routes;

namespace Silk
{

    public class StreamingAttribute : Attribute
    {}


    public abstract class BidingMethodAttribute : Attribute
    {
        protected Route _route { get; }
        protected string _method { get; set; }
        public Route Route { get { return _route; } }

        public BidingMethodAttribute(string route, string method)
        {
            if (string.IsNullOrEmpty("/"))
            {
                _route = new Route("/");
                return;
            }
            _method = method;
            _route = new Route(route);
        }
    }

    /// <summary>
    /// Define a method that will be called when a GET request is made to the specified route.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public class GetAttribute : BidingMethodAttribute
    {
        public GetAttribute(string route) : base(route, "GET")
        {}
    }

    /// <summary>
    /// Define a method that will be called when a POST request is made to the specified route.
    /// </summary>
    /// <param name="route"> 
    /// Route to bind the method to.
    /// </param>
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public class PostAttribute : BidingMethodAttribute
    {
        public PostAttribute(string route) : base(route, "POST")
        { }
    }
}
