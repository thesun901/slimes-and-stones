using UnityEngine;

/// <summary>
/// Abstrakcyjna klasa bazowa dla dowolnego stanu Slima.
/// Ka�dy stan dostaje referencj� do SlimeAI, by m�g� odczytywa� �stats�, Animator itp.
/// </summary>
public abstract class SlimeState
{
    protected SlimeAI slime;

    /// <summary>
    /// Konstruktor: przekazujemy referencj� do SlimeAI.
    /// </summary>
    public SlimeState(SlimeAI slime)
    {
        this.slime = slime;
    }

    /// <summary>
    /// Wywo�ywane raz, gdy wchodzimy do tego stanu.
    /// Ustawmy np. parametry animatora, zainicjujemy timer itp.
    /// </summary>
    public abstract void EnterState();

    /// <summary>
    /// Wywo�ywane w ka�dej klatce Update() SlimeAI dop�ki jeste�my w tym stanie.
    /// Tutaj np. sprawdzamy warunki przej�cia do innego stanu lub wykonujemy ruch.
    /// </summary>
    public abstract void UpdateState();

    /// <summary>
    /// Wywo�ywane raz, gdy opuszczamy ten stan (przej�cie do innego).
    /// Mo�emy tu resetowa� parametry animatora itp.
    /// </summary>
    public abstract void ExitState();
}
