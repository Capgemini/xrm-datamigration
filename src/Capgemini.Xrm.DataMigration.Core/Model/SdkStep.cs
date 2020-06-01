using System;

namespace Capgemini.Xrm.DataMigration.Model
{
    public class SdkStep
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Handler { get; set; }

        public SdkStepState SDKStepState { get; set; }

        public SdkSepStatusCode SDKSepStatusCode { get; set; }
    }
}