using System;
using System.Collections.Generic;
using System.Text;

namespace HACache.Models
{
    public class ServiceEndpoint
    {
        private Uri uri;
        private string pathPrefix;
        private string host;
        private int port;

        public ServiceEndpoint(string prefix, string host, int port)
        {
            this.pathPrefix = prefix;
            this.host       = host;
            this.port       = port;
            this.uri        = new Uri($"{pathPrefix}{host}:{port}");
        }

        public string PathPrefix { get; }
        public string Host { get; }
        public int Port { get; }
        public Uri Uri { get; }
    }
}
