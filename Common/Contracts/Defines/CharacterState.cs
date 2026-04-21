using System;

namespace OpenTrenches.Common.Contracts.Defines;

[Flags]
public enum CharacterState : uint
{
    Aiming      = 1 << 1, // Can shoot out of trenches, but becomes vulnerable
    Shooting    = 1 << 2,
    Building    = 1 << 4,
    Jumping     = 1 << 5,
}