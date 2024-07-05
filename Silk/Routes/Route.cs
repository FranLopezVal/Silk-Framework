namespace Silk.Routes
{
    public class Route
    {
        private string _url = "";
        private string _method = "GET";
        private bool _isStreaming = false;
        private List<string> _segments = new List<string>(); // url literal parts of URL
        private List<string> _parameters = new List<string>(); // url mutable parts of URL


        public short CountParts => (short)(_segments.Count + _parameters.Count);
        public short CountSegments => (short)_segments.Count;
        public short CountParameters => (short)_parameters.Count;

        public string Method => _method;


        public Route(string url, string targetTypeMethod = "GET")
        {
            SetUrl(url);
            _method = targetTypeMethod;
        }

        public bool IsStreaming { get => _isStreaming;internal set => _isStreaming = value;}

        public void SetUrl(string url)
        {
            _url = url;
            Parse();
        }

        public string GetFullUrl()
        {
            return _url;
        }

        public List<string> GetAll()
        {
            List<string> all = new List<string>();
            all.AddRange(_segments);
            all.AddRange(_parameters);
            return all;
        }

        public bool IsSegment(short index)
        {
            return index >= 0 && index < _segments.Count;
        }

        public bool IsParameter(short index)
        {
            return index >= 0 && index < _parameters.Count;
        }

        public string? GetSegment(short index)
        {
            if (index < 0 || index >= _segments.Count)
            {
                return null;
            }
            return _segments[index];
        }

        public string? GetParameter(short index)
        {
            if (index < 0 || index >= _parameters.Count)
            {
                return null;
            }
            return _parameters[index];
        }

        public string? GetPart(short index)
        {
            if (index < 0 || index >= CountParts)
            {
                return null;
            }
            if (index < _segments.Count)
            {
                return GetSegment(index);
            }
            return GetParameter((short)(index - _segments.Count));
        }

        public List<string> GetParameters()
        {
            return _parameters;
        }
        public List<string> GetSegments()
        {
            return _segments;
        }

        private void Parse()
        {
            var url = _url;
            var segments = url.Split('/', StringSplitOptions.RemoveEmptyEntries);
            var paramPrefix = ':';

            foreach (var segment in segments)
            {
                if (segment[0] == paramPrefix)
                {
                    _parameters.Add(segment.Substring(1));
                }
                else
                {
                    _segments.Add(segment);
                }
            }
        }
    }
}
