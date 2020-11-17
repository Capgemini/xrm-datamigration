﻿// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.
using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "Failure of ProcessImport is not a fatal exception.", Scope = "member", Target = "~M:Capgemini.Xrm.DataMigration.Engine.DataProcessors.MappEntityProcessor.FindReplacementValue(Capgemini.Xrm.DataMigration.DataStore.EntityWrapper,System.String)~System.Guid")]
[assembly: SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "Failure of DeleteEntity is not a fatal exception.", Scope = "member", Target = "~M:Capgemini.Xrm.DataMigration.Engine.DataProcessors.SyncEntitiesProcessor.ImportCompleted")]
[assembly: SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "Failure of DeleteEntity is not a fatal exception.", Scope = "member", Target = "~M:Capgemini.Xrm.DataMigration.Engine.DataProcessors.ImportCompleted")]
[assembly: SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "Failure of StartPlugins is not a fatal exception.", Scope = "member", Target = "~M:Capgemini.Xrm.DataMigration.Engine.DataProcessors.WorkflowsPluginsProcessor.StartPlugins")]
[assembly: SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "Failure of StartWorkflows is not a fatal exception.", Scope = "member", Target = "~M:Capgemini.Xrm.DataMigration.Engine.DataProcessors.WorkflowsPluginsProcessor.StartWorkflows")]
[assembly: SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "Failure of DeactivateSdkStep is not a fatal exception.", Scope = "member", Target = "~M:Capgemini.Xrm.DataMigration.Engine.DataProcessors.WorkflowsPluginsProcessor.DeactivateSdkStep(Capgemini.Xrm.DataMigration.Model.SdkStep)")]
[assembly: SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "Failure of DeactivateWorkflow is not a fatal exception.", Scope = "member", Target = "~M:Capgemini.Xrm.DataMigration.Engine.DataProcessors.WorkflowsPluginsProcessor.DeactivateWorkflow(Capgemini.Xrm.DataMigration.Model.WorkflowEntity)")]
[assembly: SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "Reviewed", Scope = "member", Target = "~M:Capgemini.Xrm.DataMigration.Engine.DataProcessors.MapEntityProcessor.FindReplacementValue(Capgemini.Xrm.DataMigration.DataStore.EntityWrapper,System.String)~System.Guid")]
[assembly: SuppressMessage("Security", "SCS0005:Weak random generator", Justification = "The degree of randomness provided by the out of box random number generator is sufficient for the current solution", Scope = "member", Target = "~M:Capgemini.Xrm.DataMigration.Engine.Obfuscate.ObfuscationType.Formatting.FormattingOptions.FormattingOptionProcessor.GenerateRandomNumber(System.String,Capgemini.DataMigration.Core.Model.ObfuscationFormatOption)~System.Int32")]
[assembly: SuppressMessage("Security", "SCS0005:Weak random generator", Justification = "The degree of randomness provided by the out of box random number generator is sufficient for the current solution", Scope = "member", Target = "~M:Capgemini.Xrm.DataMigration.Engine.Obfuscate.ObfuscationType.Formatting.FormattingOptions.FormattingOptionProcessor.LookupRandomValue(System.String,Capgemini.DataMigration.Core.Model.ObfuscationLookup)~System.Object")]