using Godot;

namespace OpenTrenches.Server.Scripting.Player;

//TODO make modular like godot physics queries
public record class WorldQuery
{
    public virtual bool IncludeAlly() => true;
    public virtual bool IncludeEnemy() => true;
    public virtual bool IncludeStructure() => true;
    public virtual bool IncludeCharacter() => true;

    /// <summary>
    /// Finds very close characters that can quickly reach the character
    /// </summary>
    public record MeeleeThreats : WorldQuery
    {
        public override bool IncludeAlly() => false;
    }
    /// <summary>
    /// objects facing <paramref name="direction"/>
    /// </summary>
    public record RangeForward(Vector2 direction) : WorldQuery;


    /// <summary>
    /// Finds moderately nearby enemies that comfortably shoot at the character.
    /// </summary>
    public record Threats : WorldQuery
    {
        public override bool IncludeAlly() => false;
    }
}
