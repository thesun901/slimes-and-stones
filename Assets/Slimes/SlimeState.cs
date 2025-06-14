using UnityEngine;

/// <summary>
/// Abstrakcyjna klasa bazowa dla dowolnego stanu Slima.
/// Ka¿dy stan dostaje referencjê do SlimeAI, by móg³ odczytywaæ „stats”, Animator itp.
/// </summary>
public abstract class SlimeState
{
    protected SlimeAI slime;

    /// <summary>
    /// Konstruktor: przekazujemy referencjê do SlimeAI.
    /// </summary>
    public SlimeState(SlimeAI slime)
    {
        this.slime = slime;
    }

    /// <summary>
    /// Wywo³ywane raz, gdy wchodzimy do tego stanu.
    /// Ustawmy np. parametry animatora, zainicjujemy timer itp.
    /// </summary>
    public abstract void EnterState();

    /// <summary>
    /// Wywo³ywane w ka¿dej klatce Update() SlimeAI dopóki jesteœmy w tym stanie.
    /// Tutaj np. sprawdzamy warunki przejœcia do innego stanu lub wykonujemy ruch.
    /// </summary>
    public abstract void UpdateState();

    /// <summary>
    /// Wywo³ywane raz, gdy opuszczamy ten stan (przejœcie do innego).
    /// Mo¿emy tu resetowaæ parametry animatora itp.
    /// </summary>
    public abstract void ExitState();
}
