// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("Security", "SCS0005:Weak random generator", Justification = "The degree of randomness provided by the out of box random number generator is sufficient for the current solution", Scope = "member", Target = "~M:Capgemini.DataScrambler.Scramblers.DoubleScrambler.CalculateRandomDouble(System.Int32)~System.Double")]
[assembly: SuppressMessage("Security", "SCS0005:Weak random generator", Justification = "The degree of randomness provided by the out of box random number generator is sufficient for the current solution", Scope = "member", Target = "~M:Capgemini.DataScrambler.Scramblers.IntegerScrambler.Scramble(System.Int32,System.Int32,System.Int32)~System.Int32")]
[assembly: SuppressMessage("Security", "SCS0005:Weak random generator", Justification = "The degree of randomness provided by the out of box random number generator is sufficient for the current solution", Scope = "member", Target = "~M:Capgemini.DataScrambler.Scramblers.StringScrambler.ScrambleString(System.String,System.Int32,System.Int32)~System.String")]