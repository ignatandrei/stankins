using System;

namespace StankinsVariousConsole
{
    internal class Artifacts
    {


        public int total_count { get; set; }
        public Artifact[] artifacts { get; set; }
    }

    public class Artifact
    {
        public int id { get; set; }
        public string node_id { get; set; }
        public string name { get; set; }
        public int size_in_bytes { get; set; }
        public string url { get; set; }
        public string archive_download_url { get; set; }
        public bool expired { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
    }

}