// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.

[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "This operation should handle all exceptions thrown during the retry process.", Scope = "member", Target = "~M:Capgemini.DataMigration.Resiliency.Polly.PollyFluentPolicy.Execute``1(System.Func{``0},System.Int32)~``0")]