﻿namespace Kontur.DBViewer.SampleApi.Impl.Classes
{
    public class TestObjectWithStrings
    {
        [Indexed, Identity]
        public string Id { get; set; }

        [Indexed, Sortable]
        public string Indexed { get; set; }

        public string NotIndexed { get; set; }
    }
}